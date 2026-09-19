using FishNet.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace STRAFTAT_CC
{
    public class PlayerCache
    {
        private float ScaledFontSize => 14 * (Cheat.Instance != null ? Cheat.Instance.GetScaleFactor() : 1f);
        private readonly Vector3 OFFSET = new Vector3(0, 0.5f, 0);
        private static readonly FieldInfo ArmorField = typeof(PlayerHealth).GetField("armor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        public GameObject GameObject { get; private set; }
        public Color color { get; set; } = Color.green;
        public Transform HeadTransform { get; private set; }
        public Collider Collider { get; private set; }
        public PlayerHealth PlayerHealth { get; private set; }
        public string PlayerName { get; private set; } = "Unknown";
        public bool IsValid { get; private set; } = false;
        public Vector3 Velocity { get; private set; } = Vector3.zero;
        private Vector3 _lastPosition = Vector3.zero;
        private float _lastUpdateTime = 0f;

        public Dictionary<string, Transform> Bones { get; private set; } = new Dictionary<string, Transform>();

        public PlayerCache(GameObject gameObject)
        {
            this.GameObject = gameObject;
            PlayerHealth = gameObject.GetComponent<PlayerHealth>();
            Collider = gameObject.GetComponent<Collider>();
            HeadTransform = Utils.RecursiveFind(gameObject.transform, "Head_Col");
            string resolvedName = gameObject.name;
            if (PlayerHealth != null && PlayerHealth.playerValues != null && PlayerHealth.playerValues.playerClient != null)
            {
                string sourceName = PlayerHealth.playerValues.playerClient.PlayerName;
                if (!string.IsNullOrEmpty(sourceName))
                {
                    resolvedName = sourceName;
                }
            }
            PlayerName = resolvedName;
            
            InitializeBones();
            _lastPosition = gameObject.transform.position;
            _lastUpdateTime = Time.time;
            IsValid = PlayerHealth != null;
        }

        private void InitializeBones()
        {
            string[] boneNames = { "Hips", "Spine", "Head_Col", "LeftShoulder", "RightShoulder", "LeftKnee", "RightKnee" };
            foreach (var name in boneNames)
            {
                Transform t = Utils.RecursiveFind(GameObject.transform, name);
                if (t != null) Bones[name] = t;
            }
        }

        public void UpdateVelocity()
        {
            if (GameObject == null) return;
            
            float deltaTime = Time.time - _lastUpdateTime;
            if (deltaTime > 0)
            {
                Vector3 currentPosition = GameObject.transform.position;
                Velocity = (currentPosition - _lastPosition) / deltaTime;
                _lastPosition = currentPosition;
                _lastUpdateTime = Time.time;
            }
        }

        public void Draw(Camera camera)
        {
            if (GameObject == null || GameObject.transform == null || camera == null || Collider == null || PlayerHealth == null)
                return;
            Vector3 screenPos = camera.WorldToScreenPoint(GameObject.transform.position - OFFSET);
            if (screenPos.z < 0)
                return;

            Vector2 screenPos2D = new Vector2(screenPos.x, Screen.height - screenPos.y);
            if (!Utils.IsOnScreen(screenPos2D))
                return;
            screenPos.y = Screen.height - screenPos.y;
            Utils.SetupExtentsBounds(Collider.bounds);
            Vector3 minScreen = camera.WorldToScreenPoint(Collider.bounds.min);
            Vector3 maxScreen = camera.WorldToScreenPoint(Collider.bounds.max);
            if (minScreen.z < 0 || maxScreen.z < 0)
                return;
            minScreen.y = Screen.height - minScreen.y;
            maxScreen.y = Screen.height - maxScreen.y;
            Rect boxRect = new Rect(
                Mathf.Min(minScreen.x, maxScreen.x),
                Mathf.Min(minScreen.y, maxScreen.y),
                Mathf.Abs(maxScreen.x - minScreen.x),
                Mathf.Abs(maxScreen.y - minScreen.y)
            );

            if (Config.Instance.onlyShowVisiblePlayers && !IsVisibleFromCamera(camera))
                return;
            
            float scale = Cheat.Instance != null ? Cheat.Instance.GetScaleFactor() : 1f;

            if (Config.Instance.enable3DBoxESP)
                Utils.Draw3DBox(camera, Config.Instance.enemyBoxColor);
            
            if (Config.Instance.enableCornerBoxESP)
            {
                Drawing.DrawCornerBox(boxRect, Config.Instance.boxThickness * scale, Config.Instance.enemyBoxColor);
            }
            else if (Config.Instance.enable2DBoxESP)
            {
                Drawing.DrawBox(boxRect, Config.Instance.boxThickness * scale, Config.Instance.enemyBoxColor);
            }
            
            // Draw skeleton ESP
            if (Config.Instance.enableSkeletonESP)
            {
                DrawSkeleton(camera);
            }
            
            // Draw Health Bar
            if (Config.Instance.enableHealthESP && Config.Instance.showHealthBar)
            {
                float barWidth = 4f * scale;
                float barHeight = boxRect.height;
                float healthPercent = PlayerHealth.health / 100f; // Assuming 100 max health, adjust if needed
                if (healthPercent > 1f) healthPercent = 1f; // Clamp
                
                // Background
                Rect hpBarBg = new Rect(boxRect.x - barWidth - 2, boxRect.y, barWidth, barHeight);
                Drawing.DrawFilledBox(hpBarBg, new Color(0, 0, 0, 0.5f));
                
                // Foreground
                float fillHeight = barHeight * healthPercent;
                Rect hpBarFill = new Rect(boxRect.x - barWidth - 2, boxRect.y + (barHeight - fillHeight), barWidth, fillHeight);
                Drawing.DrawFilledBox(hpBarFill, Utils.DoubleColorLerp(healthPercent, Color.green, Color.yellow, Color.red));
                
                // Draw outlines for the bar
                Drawing.DrawBox(hpBarBg, 1, Color.black);
            }

            if (Config.Instance.enableHealthESP && Config.Instance.showArmorBar)
            {
                float armorPercent = TryGetArmorPercent();
                if (armorPercent >= 0f)
                {
                    float barWidth = 4f * scale;
                    float barHeight = boxRect.height;
                    Rect armorBg = new Rect(boxRect.x + boxRect.width + 2, boxRect.y, barWidth, barHeight);
                    Drawing.DrawFilledBox(armorBg, new Color(0, 0, 0, 0.5f));

                    float fillHeight = barHeight * armorPercent;
                    Rect armorFill = new Rect(boxRect.x + boxRect.width + 2, boxRect.y + (barHeight - fillHeight), barWidth, fillHeight);
                    Drawing.DrawFilledBox(armorFill, new Color(0.25f, 0.55f, 1f, 1f));
                    Drawing.DrawBox(armorBg, 1, Color.black);
                }
            }

            // Draw line to player
            if (Config.Instance.enableLineToPlayer)
            {
                Vector3 linePos = camera.WorldToScreenPoint(GameObject.transform.position);
                if (linePos.z > 0)
                {
                    linePos.y = Screen.height - linePos.y;
                    Drawing.DrawLine(new Vector2(Screen.width / 2, Screen.height / 2), new Vector2(linePos.x, linePos.y), 1f * scale, Config.Instance.enemyBoxColor);
                }
            }
            
            float scaledFont = ScaledFontSize;
            float textY = boxRect.yMax + 2;

            if (Config.Instance.enableNameESP)
            {
                Utils.DrawText(new Vector2(boxRect.center.x, boxRect.y - scaledFont - 2), PlayerName, Config.Instance.textColor, (int)scaledFont, true);
            }

            if (Config.Instance.showWeaponText)
            {
                 // Replace with actual weapon name fetch if available, placeholder for now
                 // In a real scenario, you'd access PlayerHealth.playerValues.playerClient.ItemInHand or similar
                 string wpn = "Weapon"; 
                 Utils.DrawText(new Vector2(boxRect.center.x, textY), wpn, new Color(1f, 1f, 1f), (int)(scaledFont * 0.9f), true);
                 textY += scaledFont;
            }

            if (Config.Instance.enableDistanceESP && Config.Instance.showDistanceText)
            {
                float dist = Vector3.Distance(camera.transform.position, GameObject.transform.position);
                Utils.DrawText(new Vector2(boxRect.center.x, textY), $"{(int)dist}m", new Color(1f, 1f, 1f), (int)(scaledFont * 0.9f), true);
                textY += scaledFont;
            }

            if (Config.Instance.enableHealthESP && Config.Instance.showHealthNumber)
            {
                int hp = Mathf.Clamp((int)(PlayerHealth.health), 0, 100);
                 Utils.DrawText(
                    new Vector2(boxRect.x + boxRect.width + 4, boxRect.y),
                    hp + " HP",
                    Utils.DoubleColorLerp(hp / 100.0f, Color.green, Color.yellow, Color.red),
                    (int)scaledFont,
                    false
                );
            }
        }

        private bool IsVisibleFromCamera(Camera camera)
        {
            if (camera == null || GameObject == null)
                return false;

            Vector3 target = HeadTransform != null ? HeadTransform.position : GameObject.transform.position + Vector3.up * 1.2f;
            Vector3 direction = target - camera.transform.position;
            float distance = direction.magnitude;
            if (distance <= 0.01f)
                return false;

            RaycastHit hit;
            if (Physics.Raycast(camera.transform.position, direction.normalized, out hit, distance + 0.05f))
            {
                PlayerHealth hitHealth = hit.transform.GetComponentInParent<PlayerHealth>();
                return hitHealth != null && hitHealth.gameObject == GameObject;
            }

            return false;
        }

        private float TryGetArmorPercent()
        {
            if (PlayerHealth == null || ArmorField == null)
                return -1f;

            object rawValue = ArmorField.GetValue(PlayerHealth);
            if (rawValue == null)
                return -1f;

            try
            {
                float armor = Convert.ToSingle(rawValue);
                if (armor < 0f)
                    return -1f;
                return Mathf.Clamp01(armor / 100f);
            }
            catch
            {
                return -1f;
            }
        }

        private void DrawSkeleton(Camera camera)
        {
            if (GameObject == null) return;

            // Try to find bones by common names (these may need adjustment based on actual game structure)
            Transform hips = Utils.RecursiveFind(GameObject.transform, "Hips") ?? Utils.RecursiveFind(GameObject.transform, "hip");
            Transform spine = Utils.RecursiveFind(GameObject.transform, "Spine") ?? Utils.RecursiveFind(GameObject.transform, "spine");
            Transform head = Utils.RecursiveFind(GameObject.transform, "Head_Col") ?? Utils.RecursiveFind(GameObject.transform, "head");
            
            Transform leftShoulder = Utils.RecursiveFind(GameObject.transform, "LeftShoulder") ?? Utils.RecursiveFind(GameObject.transform, "left_shoulder");
            Transform leftElbow = Utils.RecursiveFind(GameObject.transform, "LeftElbow") ?? Utils.RecursiveFind(GameObject.transform, "left_elbow");
            Transform leftHand = Utils.RecursiveFind(GameObject.transform, "LeftHand") ?? Utils.RecursiveFind(GameObject.transform, "left_hand");
            
            Transform rightShoulder = Utils.RecursiveFind(GameObject.transform, "RightShoulder") ?? Utils.RecursiveFind(GameObject.transform, "right_shoulder");
            Transform rightElbow = Utils.RecursiveFind(GameObject.transform, "RightElbow") ?? Utils.RecursiveFind(GameObject.transform, "right_elbow");
            Transform rightHand = Utils.RecursiveFind(GameObject.transform, "RightHand") ?? Utils.RecursiveFind(GameObject.transform, "right_hand");
            
            Transform leftKnee = Utils.RecursiveFind(GameObject.transform, "LeftKnee") ?? Utils.RecursiveFind(GameObject.transform, "left_knee");
            Transform leftFoot = Utils.RecursiveFind(GameObject.transform, "LeftFoot") ?? Utils.RecursiveFind(GameObject.transform, "left_foot");
            
            Transform rightKnee = Utils.RecursiveFind(GameObject.transform, "RightKnee") ?? Utils.RecursiveFind(GameObject.transform, "right_knee");
            Transform rightFoot = Utils.RecursiveFind(GameObject.transform, "RightFoot") ?? Utils.RecursiveFind(GameObject.transform, "right_foot");

            // Draw body spine
            if (hips != null && spine != null)
                DrawBoneLine(camera, hips.position, spine.position);
            
            if (spine != null && head != null)
                DrawBoneLine(camera, spine.position, head.position);

            // Draw left arm
            if (spine != null && leftShoulder != null)
                DrawBoneLine(camera, spine.position, leftShoulder.position);
            if (leftShoulder != null && leftElbow != null)
                DrawBoneLine(camera, leftShoulder.position, leftElbow.position);
            if (leftElbow != null && leftHand != null)
                DrawBoneLine(camera, leftElbow.position, leftHand.position);

            // Draw right arm
            if (spine != null && rightShoulder != null)
                DrawBoneLine(camera, spine.position, rightShoulder.position);
            if (rightShoulder != null && rightElbow != null)
                DrawBoneLine(camera, rightShoulder.position, rightElbow.position);
            if (rightElbow != null && rightHand != null)
                DrawBoneLine(camera, rightElbow.position, rightHand.position);

            // Draw left leg
            if (hips != null && leftKnee != null)
                DrawBoneLine(camera, hips.position, leftKnee.position);
            if (leftKnee != null && leftFoot != null)
                DrawBoneLine(camera, leftKnee.position, leftFoot.position);

            // Draw right leg
            if (hips != null && rightKnee != null)
                DrawBoneLine(camera, hips.position, rightKnee.position);
            if (rightKnee != null && rightFoot != null)
                DrawBoneLine(camera, rightKnee.position, rightFoot.position);
        }

        private void DrawBoneLine(Camera camera, Vector3 start, Vector3 end)
        {
            Vector3 startScreen = camera.WorldToScreenPoint(start);
            Vector3 endScreen = camera.WorldToScreenPoint(end);
            
            if (startScreen.z < 0 || endScreen.z < 0)
                return;
            
            startScreen.y = Screen.height - startScreen.y;
            endScreen.y = Screen.height - endScreen.y;
            
            float scale = Cheat.Instance != null ? Cheat.Instance.GetScaleFactor() : 1f;
            Drawing.DrawLine(new Vector2(startScreen.x, startScreen.y), new Vector2(endScreen.x, endScreen.y), 1.5f * scale, Config.Instance.enemyBoxColor);
        }
    }
}
