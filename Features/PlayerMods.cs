using System.Reflection;
using UnityEngine;

namespace STRAFTAT_CC.Features
{
    public class PlayerMods
    {
        private static readonly FieldInfo WalkSpeedField = typeof(FirstPersonController).GetField("walkSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo SprintSpeedField = typeof(FirstPersonController).GetField("sprintSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo HealthField = typeof(FirstPersonController).GetField("health", BindingFlags.NonPublic | BindingFlags.Instance);

        private static bool _capturedSpeedDefaults = false;
        private static bool _speedHackApplied = false;
        private static float _defaultWalkSpeed = 6f;
        private static float _defaultSprintSpeed = 9f;

        private static void CaptureSpeedDefaults(FirstPersonController controller)
        {
            if (_capturedSpeedDefaults || controller == null || WalkSpeedField == null)
                return;

            object walkObj = WalkSpeedField.GetValue(controller);
            if (walkObj is float walkSpeed)
            {
                _defaultWalkSpeed = walkSpeed;
            }

            if (SprintSpeedField != null)
            {
                object sprintObj = SprintSpeedField.GetValue(controller);
                if (sprintObj is float sprintSpeed)
                {
                    _defaultSprintSpeed = sprintSpeed;
                }
            }

            _capturedSpeedDefaults = true;
        }

        private static void ApplySpeedHack(FirstPersonController controller)
        {
            if (controller == null || WalkSpeedField == null)
                return;

            CaptureSpeedDefaults(controller);

            float speedValue = Mathf.Max(1f, Config.Instance.speedValue);
            WalkSpeedField.SetValue(controller, speedValue);
            if (SprintSpeedField != null)
            {
                SprintSpeedField.SetValue(controller, speedValue * 1.5f);
            }

            _speedHackApplied = true;
        }

        private static void RestoreSpeedDefaults(FirstPersonController controller)
        {
            if (!_speedHackApplied || controller == null || WalkSpeedField == null)
                return;

            if (_capturedSpeedDefaults)
            {
                WalkSpeedField.SetValue(controller, _defaultWalkSpeed);
                if (SprintSpeedField != null)
                {
                    SprintSpeedField.SetValue(controller, _defaultSprintSpeed);
                }
            }

            _speedHackApplied = false;
        }

        private static void ApplyAntiAimState(FirstPersonController controller)
        {
            if (controller == null)
                return;

            if (Config.Instance.enableIsSlide)
            {
                controller.isSliding = true;
            }
        }

        public void Update()
        {
            FirstPersonController localController = Cheat.Instance.Cache.LocalController;
            if (localController == null)
                return;

            if (Config.Instance.GodMode && HealthField != null)
            {
                HealthField.SetValue(localController, 100f);
            }

            if (Config.Instance.FlyMode)
            {
                float verticalStep = Mathf.Max(0.5f, Config.Instance.flyVerticalSpeed) * Time.deltaTime;
                if (Input.GetKey(KeyCode.Q))
                {
                    localController.transform.position += Vector3.down * verticalStep;
                }
                if (Input.GetKey(KeyCode.E))
                {
                    localController.transform.position += Vector3.up * verticalStep;
                }
            }

            ApplyAntiAimState(localController);

            if (Config.Instance.enableSpeedHack)
            {
                ApplySpeedHack(localController);
            }
            else
            {
                RestoreSpeedDefaults(localController);
            }
        }
    }
}
