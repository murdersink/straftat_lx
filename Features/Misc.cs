using System;
using System.Reflection;
using UnityEngine;

namespace STRAFTAT_CC.Features
{
    public class Misc
    {
        private FieldInfo isGroundedField;
        private FieldInfo moveDirectionField;
        private FieldInfo jumpForceField;

        private bool _initialized = false;

        public Misc()
        {
            InitializeReflection();
        }

        private void InitializeReflection()
        {
            try
            {
                Type fpcType = typeof(FirstPersonController);
                isGroundedField = fpcType.GetField("isGrounded", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                moveDirectionField = fpcType.GetField("moveDirection", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                jumpForceField = fpcType.GetField("jumpForce", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                
                _initialized = true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Misc] Failed to initialize reflection: {e.Message}");
            }
        }

        public void Update()
        {
            if (!_initialized) return;
            
            if (Config.Instance.enableBhop)
            {
                Bhop();
            }
        }

        private void Bhop()
        {
            if (!Input.GetKey(KeyCode.Space)) return;

            var controller = Cheat.Instance.Cache.LocalController;
            if (controller == null) return;

            bool isGrounded = false;
            
            // Try reflection first
            if (isGroundedField != null)
            {
                isGrounded = (bool)isGroundedField.GetValue(controller);
            }
            else
            {
                // Fallback to CharacterController
                CharacterController cc = controller.GetComponent<CharacterController>();
                if (cc != null) isGrounded = cc.isGrounded;
            }

            if (isGrounded)
            {
                float jumpForce = 8f; // Default if field not found
                if (jumpForceField != null)
                {
                    jumpForce = (float)jumpForceField.GetValue(controller);
                }

                if (moveDirectionField != null)
                {
                    Vector3 moveDirection = (Vector3)moveDirectionField.GetValue(controller);
                    moveDirection.y = jumpForce;
                    moveDirectionField.SetValue(controller, moveDirection);
                }
            }
        }
    }
}
