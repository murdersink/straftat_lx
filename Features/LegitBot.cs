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
    public class LegitBot
    {
        private const float HEAD_OFFSET = 1.5f;

        private readonly Cache _cache;
        private bool _isEnabled = false;
        private bool _isMouseHold = false;
        private float _smoothing = 1.0f;
        private float _fov = 5.0f;

        public bool IsEnabled => _isEnabled;

        public LegitBot(Cache cache)
        {
            _cache = cache;
        }

        public void Update()
        {
            if (Input.GetKey(Config.Instance.boundKey_legitbot))
            {
                if (!Config.Instance.LegitAimbot)
                {
                    Config.Instance.LegitAimbot = true;
                    Config.Instance.AddDebugLog("Legit Aimbot: Activated");
                }
                AimAtClosestPlayer();
            }
            else
            {
                if (Config.Instance.LegitAimbot)
                {
                    Config.Instance.LegitAimbot = false;
                    Config.Instance.AddDebugLog("Legit Aimbot: Deactivated");
                    MouseRelease();
                }
            }

            if (!Config.Instance.LegitAimbot || _cache.LocalPlayer == null || !_cache.LocalPlayer.IsValid || !_cache.MainCamera)
            {
                return;
            }
            AimAtClosestPlayer();
        }

        public void OnGUI()
        {
            // FOV drawing disabled - using main aimbot FOV overlay instead
        }

        private void DrawFOV()
        {
            if (!_cache.MainCamera) return;

            float fov = Config.Instance.legitFOV;
            float distance = 100f;
            float radius = Mathf.Tan(fov * Mathf.Deg2Rad) * distance;

            Vector3 center = _cache.MainCamera.transform.position + _cache.MainCamera.transform.forward * distance;
            
            // Draw a circle using multiple line segments
            GL.PushMatrix();
            GL.LoadProjectionMatrix(_cache.MainCamera.projectionMatrix);
            GL.modelview = _cache.MainCamera.worldToCameraMatrix;

            GL.Begin(GL.LINES);
            
            // Draw outer circle (red)
            GL.Color(new Color(1f, 0f, 0f, 0.8f));
            int segments = 64;
            for (int i = 0; i < segments; i++)
            {
                float angle1 = (i / (float)segments) * 360f * Mathf.Deg2Rad;
                float angle2 = ((i + 1) / (float)segments) * 360f * Mathf.Deg2Rad;
                
                Vector3 point1 = center + (_cache.MainCamera.transform.right * Mathf.Cos(angle1) + _cache.MainCamera.transform.up * Mathf.Sin(angle1)) * radius;
                Vector3 point2 = center + (_cache.MainCamera.transform.right * Mathf.Cos(angle2) + _cache.MainCamera.transform.up * Mathf.Sin(angle2)) * radius;
                
                GL.Vertex(point1);
                GL.Vertex(point2);
            }
            
            // Draw crosshair lines
            GL.Color(new Color(1f, 0f, 0f, 0.5f));
            Vector3 rightPoint = center + _cache.MainCamera.transform.right * radius;
            Vector3 leftPoint = center - _cache.MainCamera.transform.right * radius;
            Vector3 upPoint = center + _cache.MainCamera.transform.up * radius;
            Vector3 downPoint = center - _cache.MainCamera.transform.up * radius;
            
            GL.Vertex(rightPoint);
            GL.Vertex(leftPoint);
            GL.Vertex(upPoint);
            GL.Vertex(downPoint);

            GL.End();
            GL.PopMatrix();
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
            if (closestPlayer == null || closestPlayer.PlayerHealth.health <= 0) return;

            Vector3 targetPosition;
            if (closestPlayer.HeadTransform != null)
            {
                targetPosition = closestPlayer.HeadTransform.position;
            }
            else
            {
                targetPosition = closestPlayer.GameObject.transform.position + Vector3.up * HEAD_OFFSET;
            }

            Vector3 direction = (targetPosition - _cache.MainCamera.transform.position).normalized;
            float angle = Vector3.Angle(_cache.MainCamera.transform.forward, direction);

            if (angle > Config.Instance.legitFOV)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Inverted smoothing: low values = strong, high values = weak
            float smoothing = Config.Instance.legitSmoothing;
            float invertedSmoothing = 11f - smoothing; // Invert: 0.1 -> 10.9, 10 -> 1
            
            // Smooth rotation for body
            _cache.LocalController.transform.rotation = Quaternion.Slerp(_cache.LocalController.transform.rotation, Quaternion.Euler(0, targetRotation.eulerAngles.y, 0), Time.deltaTime * invertedSmoothing);
            
            // Smooth rotation for camera with proper pitch clamping
            float targetPitch = targetRotation.eulerAngles.x;
            if (targetPitch > 180) targetPitch -= 360;
            targetPitch = Mathf.Clamp(targetPitch, -89f, 89f);
            
            _cache.MainCamera.transform.rotation = Quaternion.Slerp(_cache.MainCamera.transform.rotation, Quaternion.Euler(targetPitch, targetRotation.eulerAngles.y, 0), Time.deltaTime * invertedSmoothing);

            if (Config.Instance.LegitAutoShoot)
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

        public PlayerCache GetClosestTarget()
        {
            float closestDistance = float.MaxValue;
            PlayerCache closestPlayer = null;

            foreach (PlayerCache player in _cache.Players)
            {
                if (!player.IsValid || player.PlayerHealth.health <= 0)
                    continue;

                // Check if target is within FOV
                Vector3 direction = (player.GameObject.transform.position - _cache.MainCamera.transform.position).normalized;
                float angle = Vector3.Angle(_cache.MainCamera.transform.forward, direction);
                
                if (angle > Config.Instance.legitFOV)
                    continue;

                float distance = Vector3.Distance(_cache.LocalPlayer.GameObject.transform.position, player.GameObject.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPlayer = player;
                }
            }

            return closestPlayer;
        }
    }
} 
