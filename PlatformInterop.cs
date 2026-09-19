using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace STRAFTAT_CC
{
    /// <summary>
    /// Keeps host-OS APIs out of the gameplay code. The Mac Unity build has no
    /// user32.dll, so every Windows P/Invoke must be guarded before it is used.
    /// </summary>
    internal static class PlatformInterop
    {
        private const uint MouseEventLeftDown = 0x02;
        private const uint MouseEventLeftUp = 0x04;
        private const uint WindowDisplayAffinityNone = 0x00;
        private const uint WindowDisplayAffinityExcludeFromCapture = 0x11;

        private static bool IsWindows => Environment.OSVersion.Platform == PlatformID.Win32NT;

        [DllImport("user32.dll", EntryPoint = "mouse_event", CallingConvention = CallingConvention.StdCall)]
        private static extern void WindowsMouseEvent(uint flags, uint dx, uint dy, uint buttons, uint extraInfo);

        [DllImport("user32.dll", EntryPoint = "GetActiveWindow")]
        private static extern IntPtr WindowsGetActiveWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowDisplayAffinity")]
        private static extern uint WindowsSetWindowDisplayAffinity(IntPtr windowHandle, uint affinity);

        public static void SetMouseButton(bool pressed)
        {
            if (IsWindows)
            {
                try
                {
                    WindowsMouseEvent(pressed ? MouseEventLeftDown : MouseEventLeftUp, 0, 0, 0, 0);
                }
                catch (DllNotFoundException)
                {
                    // The game can still run without synthetic mouse input.
                }
                catch (EntryPointNotFoundException)
                {
                    // Some Unity/Mono distributions do not expose this Win32 entry point.
                }

                return;
            }

            // Unity's Input System is available in the Mac player and avoids
            // depending on a platform-specific desktop input API.
            if (Mouse.current != null)
            {
                InputState.Change(Mouse.current.leftButton, pressed ? 1f : 0f);
            }
        }

        public static bool TrySetStreamProof(bool enabled)
        {
            if (!IsWindows)
                return false;

            try
            {
                IntPtr windowHandle = WindowsGetActiveWindow();
                WindowsSetWindowDisplayAffinity(
                    windowHandle,
                    enabled ? WindowDisplayAffinityExcludeFromCapture : WindowDisplayAffinityNone);
                return true;
            }
            catch (DllNotFoundException)
            {
                return false;
            }
            catch (EntryPointNotFoundException)
            {
                return false;
            }
        }
    }
}
