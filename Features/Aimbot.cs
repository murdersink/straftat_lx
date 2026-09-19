using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Serializing;
using FishNet;
using UnityEngine.SocialPlatforms;
using System.Reflection;
using System.Threading;
using static Mono.Security.X509.X520;
using DG.Tweening;
using STRAFTAT_CC;

namespace STRAFTAT_CC.Features
{
    public class Aimbot
    {
        public void FlyMode(FirstPersonController playerShootInstance, bool type)
        {
            try
            {
                var flyModeField = typeof(FirstPersonController).GetField("flymode", BindingFlags.NonPublic | BindingFlags.Instance);

                if (flyModeField != null)
                {
                    flyModeField.SetValue(playerShootInstance, type);

                    Config.Instance.AddDebugLog(type ? "Fly mode Enabled." : "Fly mode Disabled.");
                }
                else
                {
                    Config.Instance.AddDebugLog("Error: flymode field not found.");
                }
            }
            catch (Exception ex)
            {
                Config.Instance.AddDebugLog("Error setting flymode field via reflection: " + ex.Message);
            }
        }

        public void PlayHitMarker(Weapon _weapon)
        {
            try
            {
                _weapon.marker = UnityEngine.Object.Instantiate<GameObject>(_weapon.hitMarker, Crosshair.Instance.transform.position, Quaternion.identity, PauseManager.Instance.transform);
                _weapon.marker.transform.DOPunchScale(new Vector3(2.5f, 2.5f, 2.5f), 0.3f, 8, 2f);
                _weapon.marker.GetComponent<UnityEngine.UI.Image>().color = Color.red;
                UnityEngine.Object.Destroy(_weapon.marker, 0.3f);
                _weapon.audio.PlayOneShot(_weapon.headHitClip);
            }
            catch (Exception ex)
            {
                Config.Instance.AddDebugLog("Error hitmarker" + ex.Message);
            }
        }

        private readonly Cache _cache;
        private bool _isEnabled = false;
        private const float HEAD_OFFSET = 1.5f;
        private bool _isMouseHold = false;
        private bool _previousFlyMode = false;
        private bool _previousFreezeEnemy = false;
        private PlayerCache _lockedTarget = null; // Lock onto one target
        private Vector3 _transitionPosition = Vector3.zero; // Smooth transition target position
        private bool _isTransitioning = false; // Whether we're transitioning between targets
        private float _transitionProgress = 0f; // Progress of transition (0 to 1)
        private float _timeSinceTargetFound = 0f;
        private Vector3 _lastJitterOffset = Vector3.zero;
        private float _killDelayEndTime = 0f;
        private float _targetVisibilityStartTime = 0f;
        private PlayerCache _potentialTarget = null;
        private float _randomSmoothingFactor = 1f;


        public bool IsEnabled => _isEnabled;

        public Aimbot(Cache cache)
        {
            _cache = cache;
        }

        public void OnGUI()
        {
            if (Config.Instance.drawLegitFOV)
            {
                DrawFOVOverlay();
            }
        }

        private void DrawFOVOverlay()
        {
            float fov = Config.Instance.legitFOV;
            float screenCenterX = Screen.width / 2f;
            float screenCenterY = Screen.height / 2f;
            
            // Fixed radius regardless of scoping/sprinting/etc
            // Base radius on screen dimensions for consistent visual size
            float radius = (Screen.height * fov) / 180f; // Scales linearly with FOV value
            
            // Draw circle using GUI.DrawTexture in a for loop
            int segments = 64;
            float angleStep = 360f / segments;
            
            Color circleColor = Config.Instance.fovCircleColor;
            float lineThickness = Mathf.Max(1f, Config.Instance.fovCircleThickness * (Cheat.Instance != null ? Cheat.Instance.GetScaleFactor() : 1f));
            
            for (int i = 0; i < segments; i++)
            {
                float angle1 = i * angleStep * Mathf.Deg2Rad;
                float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
                
                Vector2 point1 = new Vector2(
                    screenCenterX + Mathf.Cos(angle1) * radius,
                    screenCenterY + Mathf.Sin(angle1) * radius
                );
                Vector2 point2 = new Vector2(
                    screenCenterX + Mathf.Cos(angle2) * radius,
                    screenCenterY + Mathf.Sin(angle2) * radius
                );
                
                DrawLine(point1, point2, lineThickness, circleColor);
            }
        }
        
        private void DrawLine(Vector2 pointA, Vector2 pointB, float width, Color color)
        {
            float angle = Mathf.Atan2(pointB.y - pointA.y, pointB.x - pointA.x) * 180f / Mathf.PI;
            float length = Vector2.Distance(pointA, pointB);
            
            Matrix4x4 matrix = GUI.matrix;
            Color savedColor = GUI.color;
            
            GUI.color = color;
            GUI.matrix = Matrix4x4.TRS(pointA, Quaternion.Euler(0, 0, angle), Vector3.one);
            GUI.DrawTexture(new Rect(0, -width / 2, length, width), Texture2D.whiteTexture);
            
            GUI.matrix = matrix;
            GUI.color = savedColor;
        }

        public void Update()
        {
            if (Config.Instance.MagicBullet && _cache.LocalWeaponRight != null && _cache.LocalWeaponRight.fire1.IsPressed())
            {
                var enemyHealthController = GetClosestTarget();
                if (enemyHealthController != null && enemyHealthController.PlayerHealth.health > 0)
                {
                    PlayHitMarker(_cache.LocalWeaponRight);
                    enemyHealthController.PlayerHealth.ChangeKilledState(true);
                    enemyHealthController.PlayerHealth.RemoveHealth(10f);
                    enemyHealthController.PlayerHealth.SetKiller(_cache.LocalWeaponLeft.transform);
                }
            }

            if (Config.Instance.FlyMode != _previousFlyMode)
            {
                var playerController = _cache?.LocalController;

                if (playerController != null)
                {


                    FlyMode(playerController, Config.Instance.FlyMode);
                }
                else
                {
                    Config.Instance.AddDebugLog("Error: Player controller not found.");

                }

                _previousFlyMode = Config.Instance.FlyMode;
            }



            if (Config.Instance.FreezeEnemy != _previousFreezeEnemy)
            {
                var enemy = GetClosestTarget();
                if (enemy != null && enemy.PlayerHealth.health > 0 && _cache.LocalPlayer != null && _cache.LocalPlayer.PlayerHealth != null)
                    enemy.PlayerHealth.controller.sync___set_value_canMove(!Config.Instance.FreezeEnemy, _cache.LocalPlayer.PlayerHealth.IsHost);
                _previousFreezeEnemy = Config.Instance.FreezeEnemy;
            }


            if (Config.Instance.IsAimbotKeyPressed()) // Holding any bound key
            {
                if (!Config.Instance.Aimbot)
                {
                    Config.Instance.Aimbot = true;
                    Config.Instance.AddDebugLog("Aimbot: Activated");
                }
                
                // Trigger additional actions bound to aimbot key
                if (Config.Instance.aimbotKeyBindsAutoShoot)
                {
                    Config.Instance.AutoShoot = true;
                }
                
                if (Config.Instance.aimbotKeyBindsMagicBullet)
                {
                    Config.Instance.MagicBullet = true;
                }
                
                AimAtClosestPlayer(); // Call aimbot function while holding
            }
            else
            {
                if (Config.Instance.Aimbot)
                {
                    Config.Instance.Aimbot = false;
                    Config.Instance.AddDebugLog("Aimbot: Deactivated");
                    MouseRelease(); // Release mouse if it was held
                    _lockedTarget = null; // Clear locked target when disabled
                    _isTransitioning = false; // Reset transition state
                    _transitionProgress = 0f;
                }
                
                // Deactivate additional actions when key is released
                if (Config.Instance.aimbotKeyBindsAutoShoot)
                {
                    Config.Instance.AutoShoot = false;
                }
                
                if (Config.Instance.aimbotKeyBindsMagicBullet)
                {
                    Config.Instance.MagicBullet = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.Z) || Config.Instance.requestTeleportToEnemy)
            {
                Config.Instance.requestTeleportToEnemy = false;
                PlayerCache _closest = GetClosestTarget();
                if (_closest != null && _closest.HeadTransform != null && _cache.LocalController != null)
                {
                    _cache.LocalController.Teleport(_closest.HeadTransform.position, 0f, false, _closest.HeadTransform, 1, 1, false);
                    if (_closest.PlayerHealth != null)
                    {
                        _closest.PlayerHealth.sync___set_value_health(100f, true);
                    }
                }
                else
                {
                    Config.Instance.AddDebugLog("Teleport failed: no valid target.");
                }

            }

            if (!Config.Instance.Aimbot || _cache.LocalPlayer == null || !_cache.LocalPlayer.IsValid || !_cache.MainCamera)
            {
                _timeSinceTargetFound = 0f;
                return;
            }
            AimAtClosestPlayer();
        }

        private Vector3 GetBestBone(PlayerCache player)
        {
            if (!Config.Instance.aimbotBoneSelection)
            {
                return player.HeadTransform != null ? player.HeadTransform.position : player.GameObject.transform.position + Vector3.up * HEAD_OFFSET;
            }

            // Priority: Head -> Spine -> Hips
            string[] searchBones = { "Head_Col", "Spine", "Hips" };
            foreach (var boneName in searchBones)
            {
                if (player.Bones.TryGetValue(boneName, out Transform t))
                {
                    if (IsVisible(t.position, player.GameObject))
                        return t.position;
                }
            }

            // Fallback to head if nothing visible or found
            return player.HeadTransform != null ? player.HeadTransform.position : player.GameObject.transform.position + Vector3.up * HEAD_OFFSET;
        }

        private bool IsVisible(Vector3 position, GameObject targetObj)
        {
            RaycastHit hit;
            if (Physics.Raycast(_cache.MainCamera.transform.position, (position - _cache.MainCamera.transform.position).normalized, out hit))
            {
                PlayerHealth hitPlayerHealth = hit.transform.GetComponentInParent<PlayerHealth>();
                return hitPlayerHealth != null && hitPlayerHealth.gameObject == targetObj;
            }
            return false;
        }



        public void SimulateLeftMouseClick(GameObject targetObject)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
                position = Input.mousePosition
            };

            ExecuteEvents.Execute(targetObject, pointer, ExecuteEvents.pointerClickHandler);

            Debug.Log("Simulated left mouse click on " + targetObject.name);
        }


        public static void MouseHold()
        {
            PlatformInterop.SetMouseButton(true);
        }

        public static void MouseRelease()
        {
            PlatformInterop.SetMouseButton(false);
        }



        private void AimAtClosestPlayer()
        {
            PlayerCache closestPlayer = GetClosestTarget();
            if (closestPlayer == null || closestPlayer.PlayerHealth.health <= 0)
            {
                _timeSinceTargetFound = 0f;
                return;
            }

            // Deadzone check
            Vector3 targetPosition = GetBestBone(closestPlayer);
            Vector3 currentDir = _cache.MainCamera.transform.forward;
            Vector3 targetDir = (targetPosition - _cache.MainCamera.transform.position).normalized;
            float angleToTarget = Vector3.Angle(currentDir, targetDir);

            if (angleToTarget < Config.Instance.aimbotDeadzone)
            {
                // Already close enough, don't move (human-like behavior)
                return;
            }

            _timeSinceTargetFound += Time.deltaTime;

            // Prediction logic
            if (Config.Instance.aimbotPredict)
            {
                float distance = Vector3.Distance(_cache.MainCamera.transform.position, targetPosition);
                float travelTime = distance / 500f; // Dummy bullet speed value
                targetPosition += closestPlayer.Velocity * travelTime;
            }

            // Smooth transition logic
            if (_isTransitioning)
            {
                _transitionProgress += Time.deltaTime * 3f; // Faster transition
                if (_transitionProgress >= 1f)
                {
                    _transitionProgress = 1f;
                    _isTransitioning = false;
                }
                targetPosition = Vector3.Lerp(_transitionPosition, targetPosition, _transitionProgress);
            }
            else if (_lockedTarget != null && _lockedTarget != closestPlayer)
            {
                _isTransitioning = true;
                _transitionProgress = 0f;
                _transitionPosition = GetBestBone(_lockedTarget);
            }

            _lockedTarget = closestPlayer;

            Vector3 direction = (targetPosition - _cache.MainCamera.transform.position).normalized;
            
            // Apply micro-jitter for natural feel
            if (Config.Instance.aimbotJitter > 0)
            {
                _lastJitterOffset = Vector3.Lerp(_lastJitterOffset, UnityEngine.Random.insideUnitSphere * Config.Instance.aimbotJitter, Time.deltaTime * 5f);
                direction = (direction + _lastJitterOffset).normalized;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Advanced Smoothing with Sine Easing and Randomization
            float baseSmoothing = Config.Instance.aimbotSmoothing * _randomSmoothingFactor;
            float t = Mathf.Clamp01(_timeSinceTargetFound * (21f - baseSmoothing) * 0.2f);
            float easedSmoothing = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease out sine
            
            float damping = Mathf.Lerp(0.01f, 1f, easedSmoothing);
            float dampedSpeed = Time.deltaTime * (21f - baseSmoothing) * damping;
            
            // Soft RCS Implementation
            if (Config.Instance.aimbotSoftRCS && _isMouseHold)
            {
                float rcsForce = Config.Instance.aimbotRCSSale * Time.deltaTime * 50f;
                targetRotation *= Quaternion.Euler(rcsForce, 0, 0); // Pull down
            }
            
            // Smooth rotations
            float currentYaw = _cache.LocalController.transform.eulerAngles.y;
            float targetYaw = targetRotation.eulerAngles.y;
            float smoothedYaw = Mathf.LerpAngle(currentYaw, targetYaw, dampedSpeed);
            _cache.LocalController.transform.rotation = Quaternion.Euler(0, smoothedYaw, 0);
            
            float targetPitch = targetRotation.eulerAngles.x;
            if (targetPitch > 180) targetPitch -= 360;
            targetPitch = Mathf.Clamp(targetPitch, -89f, 89f);
            
            float currentPitch = _cache.MainCamera.transform.eulerAngles.x;
            if (currentPitch > 180) currentPitch -= 360;
            float smoothedPitch = Mathf.LerpAngle(currentPitch, targetPitch, dampedSpeed);
            float smoothedYawCamera = Mathf.LerpAngle(_cache.MainCamera.transform.eulerAngles.y, targetRotation.eulerAngles.y, dampedSpeed);
            
            _cache.MainCamera.transform.rotation = Quaternion.Euler(smoothedPitch, smoothedYawCamera, 0);

            if (Config.Instance.AutoShoot)
            {
                RaycastHit hit;
                if (Physics.Raycast(_cache.MainCamera.transform.position, direction, out hit))
                {
                    Transform rootTransform = hit.transform;
                    while (rootTransform.parent != null)
                    {
                        rootTransform = rootTransform.parent;
                    }

                    PlayerHealth hitPlayerHealth = hit.transform.GetComponentInParent<PlayerHealth>();
                    if (hitPlayerHealth != null)
                    {
                        if (hitPlayerHealth.gameObject == closestPlayer.GameObject && closestPlayer.PlayerHealth.health > 0)
                        {
                            // Only hold if not already holding
                            if (!_isMouseHold)
                            {
                                MouseHold();
                                _isMouseHold = true;
                            }
                        }
                        else
                        {
                            // Only release if currently holding
                            if (_isMouseHold)
                            {
                                MouseRelease();
                                _isMouseHold = false;
                            }
                        }
                    }
                }
            }
            else
            {
                // Only release if currently holding
                if (_isMouseHold)
                {
                    MouseRelease();
                    _isMouseHold = false;
                }
            }
        }

        private PlayerCache GetClosestTarget()
        {
            // Kill Delay Logic
            if (Time.time < _killDelayEndTime) return null;

            // Sticky Aim Logic
            if (Config.Instance.aimbotStickyAim && _lockedTarget != null && _lockedTarget.IsValid && _lockedTarget.PlayerHealth != null && _lockedTarget.PlayerHealth.health > 0)
            {
                Vector3 targetPos = GetBestBone(_lockedTarget);
                float angle = Vector3.Angle(_cache.MainCamera.transform.forward, (targetPos - _cache.MainCamera.transform.position).normalized);
                
                if (angle <= Config.Instance.legitFOV && IsVisible(targetPos, _lockedTarget.GameObject))
                {
                    return _lockedTarget;
                }
                
                // Target lost or died - trigger kill delay
                if (_lockedTarget.PlayerHealth.health <= 0)
                {
                    _killDelayEndTime = Time.time + Config.Instance.aimbotKillDelay;
                }
                _lockedTarget = null;
                _targetVisibilityStartTime = 0f;
                _potentialTarget = null;
            }
            
            PlayerCache closestPlayer = null;
            float bestScore = float.MaxValue;

            foreach (var player in _cache.Players)
            {
                if (!player.IsValid || player.PlayerHealth == null || player.PlayerHealth.health <= 0) continue;

                Vector3 targetPosition = GetBestBone(player);
                Vector3 direction = (targetPosition - _cache.MainCamera.transform.position).normalized;
                float angle = Vector3.Angle(_cache.MainCamera.transform.forward, direction);
                
                if (angle > Config.Instance.legitFOV) continue;
                if (!IsVisible(targetPosition, player.GameObject)) continue;

                float score = 0f;
                if (Config.Instance.aimbotTargetMode == Config.TargetMode.FOV)
                {
                    score = angle;
                }
                else
                {
                    score = Vector3.Distance(_cache.LocalPlayer.GameObject.transform.position, player.GameObject.transform.position);
                }

                if (score < bestScore)
                {
                    bestScore = score;
                    closestPlayer = player;
                }
            }

            // Check test entities
            if (Config.Instance.TestEntityEnabled)
            {
                var testEntity = _cache.TestEntity.GetClosestTestEntity();
                if (testEntity != null && testEntity.IsValid)
                {
                    Vector3 targetPosition = GetBestBone(testEntity);
                    float angle = Vector3.Angle(_cache.MainCamera.transform.forward, (targetPosition - _cache.MainCamera.transform.position).normalized);
                    
                    if (angle <= Config.Instance.legitFOV && IsVisible(targetPosition, testEntity.GameObject))
                    {
                        float score = Config.Instance.aimbotTargetMode == Config.TargetMode.FOV ? angle : Vector3.Distance(_cache.LocalPlayer.GameObject.transform.position, testEntity.GameObject.transform.position);
                        if (score < bestScore)
                        {
                            bestScore = score;
                            closestPlayer = testEntity;
                        }
                    }
                }
            }
            
            // Reaction Time Logic
            if (closestPlayer != null)
            {
                if (closestPlayer != _potentialTarget)
                {
                    _potentialTarget = closestPlayer;
                    _targetVisibilityStartTime = Time.time;
                }
                
                if (Time.time - _targetVisibilityStartTime < Config.Instance.aimbotReactionTime)
                {
                    return null; // Haven't "reacted" yet
                }
                
                // New target locked - generate random smoothing factor
                if (closestPlayer != _lockedTarget)
                {
                    _randomSmoothingFactor = UnityEngine.Random.Range(0.8f, 1.2f);
                }
            }
            else
            {
                _potentialTarget = null;
                _targetVisibilityStartTime = 0f;
            }
            
            return closestPlayer;
        }
    }
}
