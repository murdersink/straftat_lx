
using STRAFTAT_CC.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace STRAFTAT_CC
{
    public class Cheat : MonoBehaviour
    {
        private Cache _cache = new Cache(1);
        private PlayerMods _playermods = new PlayerMods();
        private Features.Misc _misc;
        private Vector2 _watermarkPos = new Vector2(10, 10);

        private bool _menuOpen = true;
        private Rect _windowRect = new Rect(100, 100, 1075, 480);
        private Vector2 _lastScreenSize = Vector2.zero;
        private float _scaleFactor = 1f;
        private bool _lastStreamProof = false;
        private const int MenuWindowId = 0x5A17;

        public Cache Cache { get => _cache; }
        public PlayerMods PlayerMods { get => _playermods; }
        public Features.Misc Misc { get => _misc; }
        public static Cheat Instance { get; private set; }

        public float GetScaleFactor() => _scaleFactor;

        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
            {
                Instance = this;
                _misc = new Features.Misc();
            }
        }

        private void Start()
        {
            _menuOpen = true;
            RepositionWindow(Screen.width, Screen.height);
        }

        private void RepositionWindow(float screenWidth, float screenHeight)
        {
            if (screenWidth <= 0f || screenHeight <= 0f)
                return;

            float width = Mathf.Min(1075f * _scaleFactor, screenWidth - 20f);
            float height = Mathf.Min(480f * _scaleFactor, screenHeight - 20f);
            _windowRect.width = Mathf.Max(320f, width);
            _windowRect.height = Mathf.Max(220f, height);
            _windowRect.x = Mathf.Max(10f, (screenWidth - _windowRect.width) * 0.5f);
            _windowRect.y = Mathf.Max(10f, (screenHeight - _windowRect.height) * 0.5f);
        }

        private void Update()
        {
            KeyCode menuKey = Config.Instance != null ? Config.Instance.menuToggleKey : KeyCode.F6;
            if (menuKey == KeyCode.None)
                menuKey = KeyCode.F6;

            bool menuTogglePressed = Input.GetKeyDown(menuKey);
            // MacBook keyboards generally do not expose Insert. Keep Insert for
            // existing Windows configs, but provide a usable Mac fallback.
            if (!menuTogglePressed && menuKey == KeyCode.Insert)
                menuTogglePressed = Input.GetKeyDown(KeyCode.F6);

            if (menuTogglePressed)
                _menuOpen = !_menuOpen;

            
            Cache.Update();

            WeaponMods.Update();
            PlayerMods.Update();
            _misc.Update();

            if (Config.Instance.streamProof != _lastStreamProof)
            {
                _lastStreamProof = Config.Instance.streamProof;
                if (!PlatformInterop.TrySetStreamProof(_lastStreamProof) && _lastStreamProof)
                {
                    Config.Instance.AddDebugLog("Stream proof is unavailable on this platform.");
                }
            }
        }

        private void Menu(int id)
        {
            Config.Instance.Draw();
            GUI.DragWindow();
        }

        private void OnGUI()
        {
            // Handle dynamic scaling
            Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
            if (_lastScreenSize != currentScreenSize)
            {
                // Base resolution (adjust to your preferred base resolution)
                float baseWidth = 1920f;
                float baseHeight = 1080f;
                
                _scaleFactor = Mathf.Min(Screen.width / baseWidth, Screen.height / baseHeight);
                
                // Scale window size and position
                RepositionWindow(Screen.width, Screen.height);
                _lastScreenSize = currentScreenSize;
            }
            
            if (_menuOpen)
            {
                // Force recalculate window rect if scaling changed
                _windowRect.width = 1075 * _scaleFactor;
                _windowRect.height = 480 * _scaleFactor;
                
                _windowRect = GUI.Window(MenuWindowId, _windowRect, Menu, "STRAFTAT.CC");
            }

            ESP.OnGUI();
            Cache.Aimbot.OnGUI();
            Cache.LegitBot.OnGUI();
        }
    }
}
