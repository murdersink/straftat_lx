using System;
using System.Collections.Generic;
using UnityEngine;
using STRAFTAT_CC;

public class Config
{
    public static Config Instance = new Config();

    private const string TAB_RAGEBOT = "Ragebot";
    private const string TAB_LEGITBOT = "Legitbot";
    private const string TAB_VISUALS = "Visuals";
    private const string TAB_EXPLOITS = "Exploits";
    private const string TAB_MISC = "Misc";
    private const string TAB_ANTIAIM = "AntiAim";
    private const string TAB_TESTING = "Testing";
    private const string TAB_CUSTOMIZE = "Customize";
    private const int MAX_LOGS = 90;

    private string currentMenu = TAB_RAGEBOT;
    private readonly List<string> _debugLogs = new List<string>();
    private Vector2 _menuScrollPosition;
    private Vector2 _debugScrollPosition;

    private string _hoverTooltip;
    private Vector2 _hoverMousePos;
    private bool _waitingForMenuKey;
    private bool _waitingForTestEntityKey;

    private readonly List<Texture2D> _generatedTextures = new List<Texture2D>();
    private float _lastScale = -1f;
    private float _lastOpacity = -1f;
    private bool _lastCompactMode;
    private Color _lastAccent;
    private Color _lastBackground;
    private Color _lastPanel;
    private Color _lastText;
    private string _themeName = "CS2 Steel";

    // Visuals
    // Keep the default state vanilla-safe; enable optional features explicitly.
    public bool enableESP = false;
    public bool enable3DBoxESP = false;
    public bool enableCornerBoxESP = true;
    public bool enable2DBoxESP = false;
    public bool enableHealthESP = true;
    public bool enableDistanceESP = true;
    public bool enableNameESP = true;
    public bool showHealthBar = true;
    public bool showArmorBar = true;
    public bool showWeaponText = true;
    public bool showDistanceText = true;
    public bool showHealthNumber = false;
    public bool enableSkeletonESP = false;
    public bool enableLineToPlayer = false;
    public bool onlyShowVisiblePlayers = false;

    // ESP colors
    public Color enemyBoxColor = new Color(1f, 0.25f, 0.2f, 1f);
    public Color friendlyBoxColor = new Color(0.2f, 0.8f, 0.3f, 1f);
    public Color textColor = Color.white;
    public float boxThickness = 2f;

    // Exploits
    public bool InfiniteAmmo = false;
    public bool RapidFire = false;
    public bool InstaKill = false;
    public bool NoSpread = false;
    public bool WeaponSpeed = false;

    // Exploit tuning
    public int infiniteAmmoAmount = 999;
    public float rapidFireInterval = 0f;
    public float instaKillDamage = 999f;
    public float noSpreadValue = 0f;
    public float weaponMovementFactor = 3f;
    public float weaponFireSlowdownFactor = 0f;
    public float weaponFireSlowdownDuration = 0f;
    public bool restoreWeaponDefaults = true;

    public bool Aimbot = false;
    public bool AutoShoot = false;
    public bool LegitAimbot = false;
    public bool LegitAutoShoot = false;
    public bool FlyMode = false;
    public bool GodMode = false;
    public bool MagicBullet = false;
    public bool FreezeEnemy = false;
    public bool requestTeleportToEnemy = false;

    public bool TestEntityEnabled = false;
    public float TestEntityHeight = 1.7f;

    public float speedValue = 6f;
    public float jumpSpeedValue = 8f;
    public float jumpHeightValue = 2f;
    public float crouchSpeedValue = 3f;
    public float gravityValue = 20f;
    public float flyVerticalSpeed = 6f;

    public bool enableSpeedHack = false;
    public bool infiniteJumpEnabled = false;
    public bool enableBhop = false;

    // Performance
    public bool lowPerformanceMode = false;
    public bool autoApplyLowPerformancePreset = true;

    public float legitSmoothing = 1.0f;
    public float legitFOV = 10.0f;
    public bool drawLegitFOV = true;
    public float aimbotSmoothing = 10f;

    // Aimbot settings
    public bool aimbotStickyAim = true;
    public enum TargetMode { Distance, FOV }
    public TargetMode aimbotTargetMode = TargetMode.FOV;
    public bool aimbotPredict = false;
    public float aimbotJitter = 0.05f;
    public bool aimbotBoneSelection = true;
    public float aimbotReactionTime = 0.15f;
    public float aimbotKillDelay = 0.35f;
    public bool aimbotRandomBones = true;
    public float aimbotDeadzone = 0.5f;
    public bool aimbotSoftRCS = true;
    public float aimbotRCSSale = 0.8f;

    // FOV overlay
    public Color fovCircleColor = new Color(1f, 0.68f, 0.35f, 0.95f);
    public float fovCircleThickness = 2f;

    public bool enableIsSlide = false;
    public bool enableIsGrounded = false;
    public bool enableIsSprinting = false;
    public bool enableIsScopeAiming = false;
    public bool enableIsLeaning = false;

    // Stream proof
    public bool streamProof = false;

    // UI customization
    public bool showDebugPanel = true;
    public bool showStatusStrip = true;
    public bool compactMode = false;
    public float uiScaleMultiplier = 1f;
    public float uiOpacity = 0.95f;
    public KeyCode menuToggleKey = KeyCode.F6;
    public Color uiAccentColor = new Color(0.96f, 0.56f, 0.16f, 1f);
    public Color uiBackgroundColor = new Color(0.10f, 0.12f, 0.14f, 1f);
    public Color uiPanelColor = new Color(0.15f, 0.18f, 0.22f, 1f);
    public Color uiTextColor = new Color(0.95f, 0.96f, 0.98f, 1f);

    // Binds
    public KeyCode boundKey_aimbot = KeyCode.E;
    public KeyCode boundKey_legitbot = KeyCode.Q;
    public KeyCode boundKey_createTestEntity = KeyCode.T;
    public bool waitingForKey = false;
    public bool aimbotKeyBindsAutoShoot = false;
    public bool aimbotKeyBindsMagicBullet = false;
    public List<KeyCode> aimbotKeys = new List<KeyCode> { KeyCode.E };
    public bool addingAimbotKey = false;

    // Styles
    private GUIStyle _windowStyle;
    private GUIStyle _sidebarStyle;
    private GUIStyle _panelStyle;
    private GUIStyle _debugPanelStyle;
    private GUIStyle _sectionStyle;
    private GUIStyle _tabStyle;
    private GUIStyle _activeTabStyle;
    private GUIStyle _titleStyle;
    private GUIStyle _subtitleStyle;
    private GUIStyle _rowStyle;
    private GUIStyle _labelStyle;
    private GUIStyle _valueStyle;
    private GUIStyle _toggleStyle;
    private GUIStyle _toggleOnStyle;
    private GUIStyle _buttonStyle;
    private GUIStyle _primaryButtonStyle;
    private GUIStyle _warningStyle;
    private GUIStyle _infoStyle;
    private GUIStyle _tooltipStyle;
    private GUIStyle _tooltipPanelStyle;
    private GUIStyle _statusStyle;
    private GUIStyle _logStyle;

    private float Scale
    {
        get
        {
            float baseScale = Cheat.Instance != null ? Cheat.Instance.GetScaleFactor() : 1f;
            return Mathf.Clamp(baseScale * uiScaleMultiplier, 0.65f, 2.1f);
        }
    }

    private bool ColorsClose(Color a, Color b)
    {
        return Mathf.Abs(a.r - b.r) < 0.001f &&
               Mathf.Abs(a.g - b.g) < 0.001f &&
               Mathf.Abs(a.b - b.b) < 0.001f &&
               Mathf.Abs(a.a - b.a) < 0.001f;
    }

    private bool NeedsStyleRebuild()
    {
        if (_windowStyle == null || _sidebarStyle == null || _panelStyle == null || _debugPanelStyle == null ||
            _sectionStyle == null || _tabStyle == null || _activeTabStyle == null || _titleStyle == null ||
            _subtitleStyle == null || _rowStyle == null || _labelStyle == null || _valueStyle == null ||
            _toggleStyle == null || _toggleOnStyle == null || _buttonStyle == null || _primaryButtonStyle == null ||
            _warningStyle == null || _infoStyle == null || _tooltipStyle == null || _tooltipPanelStyle == null ||
            _statusStyle == null || _logStyle == null)
        {
            return true;
        }

        if (Mathf.Abs(_lastScale - Scale) > 0.01f) return true;
        if (Mathf.Abs(_lastOpacity - uiOpacity) > 0.01f) return true;
        if (_lastCompactMode != compactMode) return true;
        if (!ColorsClose(_lastAccent, uiAccentColor)) return true;
        if (!ColorsClose(_lastBackground, uiBackgroundColor)) return true;
        if (!ColorsClose(_lastPanel, uiPanelColor)) return true;
        if (!ColorsClose(_lastText, uiTextColor)) return true;
        return false;
    }

    private Texture2D MakeTexture(Color color)
    {
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.hideFlags = HideFlags.DontSave;
        Color[] pixels = { color, color, color, color };
        texture.SetPixels(pixels);
        texture.Apply();
        _generatedTextures.Add(texture);
        return texture;
    }

    private void ReleaseTextures()
    {
        for (int i = 0; i < _generatedTextures.Count; i++)
        {
            if (_generatedTextures[i] != null) UnityEngine.Object.Destroy(_generatedTextures[i]);
        }
        _generatedTextures.Clear();
    }

    private void BuildStyles()
    {
        ReleaseTextures();

        float s = Scale;
        int pad = Mathf.RoundToInt((compactMode ? 6f : 10f) * s);
        int rowPad = Mathf.RoundToInt((compactMode ? 5f : 7f) * s);

        Color win = uiBackgroundColor; win.a = uiOpacity;
        Color panel = uiPanelColor; panel.a = Mathf.Clamp01(uiOpacity * 0.95f);
        Color row = Color.Lerp(panel, Color.black, 0.2f);
        Color hover = Color.Lerp(row, uiAccentColor, 0.2f);
        Color subtle = Color.Lerp(uiTextColor, new Color(0.66f, 0.7f, 0.76f, 1f), 0.5f);

        _windowStyle = new GUIStyle(GUI.skin.box);
        _windowStyle.normal.background = MakeTexture(win);
        _windowStyle.padding = new RectOffset(pad, pad, pad, pad);

        _sidebarStyle = new GUIStyle(GUI.skin.box);
        _sidebarStyle.normal.background = MakeTexture(Color.Lerp(panel, Color.black, 0.08f));
        _sidebarStyle.padding = new RectOffset(pad, pad, pad, pad);

        _panelStyle = new GUIStyle(GUI.skin.box);
        _panelStyle.normal.background = MakeTexture(panel);
        _panelStyle.padding = new RectOffset(pad, pad, pad, pad);
        _debugPanelStyle = new GUIStyle(_panelStyle);

        _sectionStyle = new GUIStyle(GUI.skin.box);
        _sectionStyle.normal.background = MakeTexture(Color.Lerp(panel, Color.black, 0.14f));
        _sectionStyle.padding = new RectOffset(pad, pad, pad, pad);

        _tabStyle = new GUIStyle(GUI.skin.button);
        _tabStyle.normal.background = MakeTexture(row);
        _tabStyle.hover.background = MakeTexture(hover);
        _tabStyle.normal.textColor = subtle;
        _tabStyle.fontStyle = FontStyle.Bold;
        _tabStyle.alignment = TextAnchor.MiddleLeft;
        _tabStyle.fontSize = Mathf.RoundToInt(12f * s);
        _tabStyle.padding = new RectOffset(Mathf.RoundToInt(10f * s), Mathf.RoundToInt(8f * s), Mathf.RoundToInt(7f * s), Mathf.RoundToInt(7f * s));

        _activeTabStyle = new GUIStyle(_tabStyle);
        _activeTabStyle.normal.background = MakeTexture(Color.Lerp(row, uiAccentColor, 0.35f));
        _activeTabStyle.normal.textColor = Color.white;

        _titleStyle = new GUIStyle(GUI.skin.label);
        _titleStyle.normal.textColor = uiAccentColor;
        _titleStyle.fontStyle = FontStyle.Bold;
        _titleStyle.fontSize = Mathf.RoundToInt(15f * s);

        _subtitleStyle = new GUIStyle(GUI.skin.label);
        _subtitleStyle.normal.textColor = subtle;
        _subtitleStyle.fontStyle = FontStyle.Bold;
        _subtitleStyle.fontSize = Mathf.RoundToInt(10.5f * s);

        _rowStyle = new GUIStyle(GUI.skin.box);
        _rowStyle.normal.background = MakeTexture(row);
        _rowStyle.padding = new RectOffset(rowPad, rowPad, rowPad, rowPad);

        _labelStyle = new GUIStyle(GUI.skin.label);
        _labelStyle.normal.textColor = uiTextColor;
        _labelStyle.fontStyle = FontStyle.Bold;
        _labelStyle.fontSize = Mathf.RoundToInt(11f * s);

        _valueStyle = new GUIStyle(_labelStyle);
        _valueStyle.alignment = TextAnchor.MiddleRight;
        _valueStyle.normal.textColor = uiAccentColor;

        _toggleStyle = new GUIStyle(GUI.skin.button);
        _toggleStyle.normal.background = MakeTexture(Color.Lerp(row, Color.black, 0.2f));
        _toggleStyle.hover.background = MakeTexture(hover);
        _toggleStyle.normal.textColor = subtle;
        _toggleStyle.fontStyle = FontStyle.Bold;
        _toggleStyle.fontSize = Mathf.RoundToInt(10f * s);

        _toggleOnStyle = new GUIStyle(_toggleStyle);
        _toggleOnStyle.normal.background = MakeTexture(Color.Lerp(uiAccentColor, Color.black, 0.05f));
        _toggleOnStyle.normal.textColor = Color.white;

        _buttonStyle = new GUIStyle(GUI.skin.button);
        _buttonStyle.normal.background = MakeTexture(row);
        _buttonStyle.hover.background = MakeTexture(hover);
        _buttonStyle.normal.textColor = uiTextColor;
        _buttonStyle.fontStyle = FontStyle.Bold;
        _buttonStyle.fontSize = Mathf.RoundToInt(10.5f * s);

        _primaryButtonStyle = new GUIStyle(_buttonStyle);
        _primaryButtonStyle.normal.background = MakeTexture(Color.Lerp(uiAccentColor, Color.black, 0.08f));
        _primaryButtonStyle.hover.background = MakeTexture(Color.Lerp(uiAccentColor, Color.white, 0.08f));

        _warningStyle = new GUIStyle(_labelStyle);
        _warningStyle.normal.textColor = new Color(1f, 0.42f, 0.3f, 1f);

        _infoStyle = new GUIStyle(GUI.skin.label);
        _infoStyle.normal.background = MakeTexture(Color.Lerp(Color.black, uiAccentColor, 0.25f));
        _infoStyle.normal.textColor = uiAccentColor;
        _infoStyle.alignment = TextAnchor.MiddleCenter;
        _infoStyle.fontStyle = FontStyle.Bold;
        _infoStyle.fontSize = Mathf.RoundToInt(11f * s);

        _tooltipPanelStyle = new GUIStyle(GUI.skin.box);
        _tooltipPanelStyle.normal.background = MakeTexture(new Color(0.05f, 0.07f, 0.09f, 0.96f));
        _tooltipPanelStyle.padding = new RectOffset(Mathf.RoundToInt(8f * s), Mathf.RoundToInt(8f * s), Mathf.RoundToInt(6f * s), Mathf.RoundToInt(6f * s));

        _tooltipStyle = new GUIStyle(GUI.skin.label);
        _tooltipStyle.normal.textColor = Color.white;
        _tooltipStyle.wordWrap = true;
        _tooltipStyle.fontSize = Mathf.RoundToInt(10.5f * s);

        _statusStyle = new GUIStyle(_subtitleStyle);
        _statusStyle.fontSize = Mathf.RoundToInt(10f * s);
        _statusStyle.fontStyle = FontStyle.Bold;

        _logStyle = new GUIStyle(GUI.skin.label);
        _logStyle.normal.textColor = uiTextColor;
        _logStyle.wordWrap = true;
        _logStyle.fontSize = Mathf.RoundToInt(10f * s);

        _lastScale = Scale;
        _lastOpacity = uiOpacity;
        _lastCompactMode = compactMode;
        _lastAccent = uiAccentColor;
        _lastBackground = uiBackgroundColor;
        _lastPanel = uiPanelColor;
        _lastText = uiTextColor;
    }

    public void Draw()
    {
        if (NeedsStyleRebuild()) BuildStyles();
        _hoverTooltip = null;

        float s = Scale;
        float gap = 8f * s;

        GUILayout.BeginHorizontal(_windowStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

        DrawSidebar(Mathf.RoundToInt(220f * s));
        GUILayout.Space(gap);
        DrawMainPanel();
        if (showDebugPanel)
        {
            GUILayout.Space(gap);
            DrawDebugPanel(Mathf.RoundToInt(300f * s));
        }

        GUILayout.EndHorizontal();
        DrawTooltip();
    }

    private void DrawSidebar(float width)
    {
        GUILayout.BeginVertical(_sidebarStyle, GUILayout.Width(width), GUILayout.ExpandHeight(true));
        GUILayout.Label("STRAFTAT", _titleStyle);
        GUILayout.Label("TACTICAL PANEL", _subtitleStyle);
        GUILayout.Space(6f * Scale);

        DrawTab("Aimbot", TAB_RAGEBOT, "Aimbot profile, target mode and bind controls.");
        DrawTab("Legitbot", TAB_LEGITBOT, "Legit assist settings and activation key.");
        DrawTab("Visuals", TAB_VISUALS, "ESP overlays, bars, text and colors.");
        DrawTab("Exploits", TAB_EXPLOITS, "Weapon and combat modifiers.");
        DrawTab("Movement", TAB_MISC, "Movement mods, fly mode and teleport.");
        DrawTab("Anti-Aim", TAB_ANTIAIM, "Experimental state controls.");
        DrawTab("Testing", TAB_TESTING, "Test entities and debug utilities.");
        DrawTab("Customize", TAB_CUSTOMIZE, "Theme and UI customization.");

        GUILayout.FlexibleSpace();
        GUILayout.Label("Theme: " + _themeName, _subtitleStyle);
        GUILayout.Label("Menu Key: " + menuToggleKey, _subtitleStyle);
        GUILayout.EndVertical();
    }

    private void DrawTab(string name, string menu, string tooltip)
    {
        GUILayout.BeginHorizontal();
        GUIStyle style = currentMenu == menu ? _activeTabStyle : _tabStyle;
        if (GUILayout.Button(name, style, GUILayout.Height(30f * Scale), GUILayout.ExpandWidth(true)))
        {
            currentMenu = menu;
        }
        DrawInfo(tooltip);
        GUILayout.EndHorizontal();
    }

    private void DrawMainPanel()
    {
        GUILayout.BeginVertical(_panelStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        if (showStatusStrip) DrawStatusStrip();

        GUILayout.Label(currentMenu, _titleStyle);
        _menuScrollPosition = GUILayout.BeginScrollView(_menuScrollPosition, false, true);
        DrawMenuContent();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawDebugPanel(float width)
    {
        GUILayout.BeginVertical(_debugPanelStyle, GUILayout.Width(width), GUILayout.ExpandHeight(true));
        GUILayout.Label("Debug Log", _titleStyle);
        _debugScrollPosition = GUILayout.BeginScrollView(_debugScrollPosition);
        for (int i = 0; i < _debugLogs.Count; i++) GUILayout.Label(_debugLogs[i], _logStyle);
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear", _buttonStyle)) _debugLogs.Clear();
        if (GUILayout.Button("Copy Last", _buttonStyle) && _debugLogs.Count > 0) GUIUtility.systemCopyBuffer = _debugLogs[_debugLogs.Count - 1];
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void DrawStatusStrip()
    {
        GUILayout.BeginHorizontal(_sectionStyle);
        Status("Aim", Aimbot);
        Status("Legit", LegitAimbot);
        Status("ESP", enableESP);
        Status("Auto", AutoShoot || LegitAutoShoot);
        Status("Fly", FlyMode);
        Status("Stream", streamProof);
        GUILayout.EndHorizontal();
    }

    private void Status(string label, bool enabled)
    {
        Color prev = GUI.color;
        GUI.color = enabled ? uiAccentColor : new Color(1f, 1f, 1f, 0.4f);
        GUILayout.Label($"{label}:{(enabled ? "ON" : "OFF")}", _statusStyle);
        GUI.color = prev;
    }

    private void BeginSection(string title)
    {
        GUILayout.BeginVertical(_sectionStyle);
        GUILayout.Label(title, _titleStyle);
    }

    private void EndSection()
    {
        GUILayout.EndVertical();
        GUILayout.Space(6f * Scale);
    }

    private void ToggleRow(string label, ref bool value, string tip)
    {
        GUILayout.BeginHorizontal(_rowStyle);
        GUILayout.Label(label, _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo(tip);
        GUIStyle style = value ? _toggleOnStyle : _toggleStyle;
        if (GUILayout.Button(value ? "ON" : "OFF", style, GUILayout.Width(58f * Scale), GUILayout.Height(22f * Scale)))
        {
            value = !value;
            AddDebugLog(label + ": " + (value ? "Enabled" : "Disabled"));
        }
        GUILayout.EndHorizontal();
    }

    private void SliderRow(string label, ref float value, float min, float max, string fmt, string tip)
    {
        GUILayout.BeginVertical(_rowStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo(tip);
        GUILayout.Label(value.ToString(fmt), _valueStyle, GUILayout.Width(66f * Scale));
        GUILayout.EndHorizontal();
        value = GUILayout.HorizontalSlider(value, min, max);
        GUILayout.EndVertical();
    }

    private void IntSliderRow(string label, ref int value, int min, int max, string tip)
    {
        float temp = value;
        SliderRow(label, ref temp, min, max, "F0", tip);
        value = Mathf.RoundToInt(temp);
    }

    private void ColorRow(string label, ref Color color, string tip)
    {
        GUILayout.BeginVertical(_rowStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo(tip);

        Rect preview = GUILayoutUtility.GetRect(32f * Scale, 14f * Scale, GUILayout.Width(32f * Scale), GUILayout.Height(14f * Scale));
        Color prev = GUI.color;
        GUI.color = color;
        GUI.DrawTexture(preview, Texture2D.whiteTexture);
        GUI.color = prev;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("R", _subtitleStyle, GUILayout.Width(12f * Scale));
        color.r = GUILayout.HorizontalSlider(color.r, 0f, 1f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("G", _subtitleStyle, GUILayout.Width(12f * Scale));
        color.g = GUILayout.HorizontalSlider(color.g, 0f, 1f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("B", _subtitleStyle, GUILayout.Width(12f * Scale));
        color.b = GUILayout.HorizontalSlider(color.b, 0f, 1f);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("A", _subtitleStyle, GUILayout.Width(12f * Scale));
        color.a = GUILayout.HorizontalSlider(color.a, 0f, 1f);
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    private void DrawInfo(string tip)
    {
        if (string.IsNullOrEmpty(tip)) return;
        float size = 16f * Scale;
        Rect rect = GUILayoutUtility.GetRect(size, size, GUILayout.Width(size), GUILayout.Height(size));
        GUI.Label(rect, "?", _infoStyle);
        if (rect.Contains(Event.current.mousePosition))
        {
            _hoverTooltip = tip;
            _hoverMousePos = Event.current.mousePosition;
        }
    }

    private void DrawTooltip()
    {
        if (string.IsNullOrEmpty(_hoverTooltip)) return;
        float padding = 8f * Scale;
        float width = 320f * Scale;
        GUIContent gc = new GUIContent(_hoverTooltip);
        float height = _tooltipStyle.CalcHeight(gc, width - padding * 2f) + padding * 2f;
        Rect rect = new Rect(_hoverMousePos.x + 16f * Scale, _hoverMousePos.y + 16f * Scale, width, height);
        if (rect.xMax > Screen.width - 8f) rect.x = Screen.width - rect.width - 8f;
        if (rect.yMax > Screen.height - 8f) rect.y = Screen.height - rect.height - 8f;
        GUI.Box(rect, GUIContent.none, _tooltipPanelStyle);
        GUI.Label(new Rect(rect.x + padding, rect.y + padding, rect.width - padding * 2f, rect.height - padding * 2f), gc, _tooltipStyle);
    }

    private bool TryCaptureBind(out KeyCode key)
    {
        key = KeyCode.None;
        Event e = Event.current;
        if (e != null)
        {
            if (e.isKey && e.keyCode != KeyCode.None)
            {
                key = e.keyCode;
                e.Use();
                return true;
            }
            if (e.isMouse && e.button >= 0 && e.button <= 6)
            {
                key = (KeyCode)((int)KeyCode.Mouse0 + e.button);
                e.Use();
                return true;
            }
        }

        KeyCode[] mice = { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6 };
        for (int i = 0; i < mice.Length; i++)
        {
            if (Input.GetKeyDown(mice[i]))
            {
                key = mice[i];
                return true;
            }
        }
        return false;
    }

    private void KeybindRow(string label, ref KeyCode key, ref bool waiting, string tip)
    {
        GUILayout.BeginVertical(_rowStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{label}: {key}", _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo(tip);
        if (!waiting)
        {
            if (GUILayout.Button("Rebind", _buttonStyle, GUILayout.Width(84f * Scale), GUILayout.Height(22f * Scale))) waiting = true;
        }
        else
        {
            if (GUILayout.Button("Cancel", _buttonStyle, GUILayout.Width(84f * Scale), GUILayout.Height(22f * Scale))) waiting = false;
        }
        GUILayout.EndHorizontal();

        if (waiting)
        {
            GUILayout.Label("Press any key or mouse button...", _subtitleStyle);
            if (TryCaptureBind(out KeyCode captured))
            {
                key = captured;
                waiting = false;
                AddDebugLog($"{label} set to {captured}");
            }
        }
        GUILayout.EndVertical();
    }

    private void DrawAimbotBinds()
    {
        if (aimbotKeys == null) aimbotKeys = new List<KeyCode> { KeyCode.E };

        for (int i = 0; i < aimbotKeys.Count; i++)
        {
            KeyCode key = aimbotKeys[i];
            GUILayout.BeginHorizontal(_rowStyle);
            GUILayout.Label("Bind " + (i + 1) + ": " + key, _labelStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove", _buttonStyle, GUILayout.Width(80f * Scale), GUILayout.Height(22f * Scale)))
            {
                aimbotKeys.RemoveAt(i);
                AddDebugLog("Removed bind: " + key);
                i--;
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginVertical(_rowStyle);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Add Aimbot Bind", _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo("Adds another keyboard/mouse hold bind for aimbot activation.");
        if (!addingAimbotKey)
        {
            if (GUILayout.Button("Add", _primaryButtonStyle, GUILayout.Width(80f * Scale), GUILayout.Height(22f * Scale))) addingAimbotKey = true;
        }
        else
        {
            if (GUILayout.Button("Cancel", _buttonStyle, GUILayout.Width(80f * Scale), GUILayout.Height(22f * Scale))) addingAimbotKey = false;
        }
        GUILayout.EndHorizontal();

        if (addingAimbotKey)
        {
            GUILayout.Label("Press any key or mouse button...", _subtitleStyle);
            if (TryCaptureBind(out KeyCode captured))
            {
                if (!aimbotKeys.Contains(captured))
                {
                    aimbotKeys.Add(captured);
                    AddDebugLog("Added bind: " + captured);
                }
                addingAimbotKey = false;
            }
        }
        GUILayout.EndVertical();
    }

    private void DrawMenuContent()
    {
        switch (currentMenu)
        {
            case TAB_RAGEBOT: DrawRagebotContent(); break;
            case TAB_LEGITBOT: DrawLegitbotContent(); break;
            case TAB_VISUALS: DrawVisualsContent(); break;
            case TAB_EXPLOITS: DrawExploitsContent(); break;
            case TAB_MISC: DrawMiscContent(); break;
            case TAB_ANTIAIM: DrawAntiAimContent(); break;
            case TAB_TESTING: DrawTestingContent(); break;
            case TAB_CUSTOMIZE: DrawCustomizeContent(); break;
        }
    }

    private void DrawRagebotContent()
    {
        BeginSection("Core");
        ToggleRow("Aimbot", ref Aimbot, "Master aimbot switch.");
        ToggleRow("Auto Shoot", ref AutoShoot, "Automatically fires when target is confirmed.");
        ToggleRow("Draw FOV Ring", ref drawLegitFOV, "Draw target FOV ring in center.");
        SliderRow("Aimbot Smoothing", ref aimbotSmoothing, 0.1f, 20f, "F1", "Lower = stronger snap.");
        SliderRow("Target FOV", ref legitFOV, 1f, 50f, "F1", "Target acquisition cone.");
        SliderRow("FOV Ring Thickness", ref fovCircleThickness, 1f, 4f, "F1", "FOV circle line width.");
        ColorRow("FOV Ring Color", ref fovCircleColor, "FOV ring color and alpha.");
        EndSection();

        BeginSection("Targeting");
        ToggleRow("Sticky Aim", ref aimbotStickyAim, "Keeps lock until target invalid.");
        ToggleRow("Prediction", ref aimbotPredict, "Predicts moving targets.");
        ToggleRow("Bone Selection", ref aimbotBoneSelection, "Uses visible bone priority.");
        ToggleRow("Randomized Bones", ref aimbotRandomBones, "Adds subtle target variation.");
        ToggleRow("Soft RCS", ref aimbotSoftRCS, "Recoil control while firing.");
        if (aimbotSoftRCS) SliderRow("RCS Strength", ref aimbotRCSSale, 0f, 1.5f, "F2", "Recoil compensation intensity.");
        SliderRow("Aim Jitter", ref aimbotJitter, 0f, 0.5f, "F3", "Micro-random offset.");
        SliderRow("Reaction Delay", ref aimbotReactionTime, 0f, 0.5f, "F3", "Delay before locking a new target.");
        SliderRow("Kill Delay", ref aimbotKillDelay, 0f, 1f, "F3", "Delay before swapping after kill.");
        SliderRow("Deadzone", ref aimbotDeadzone, 0f, 5f, "F2", "No movement zone near crosshair.");

        GUILayout.BeginHorizontal(_rowStyle);
        GUILayout.Label("Target Priority", _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo("FOV = closest angle. Distance = nearest world distance.");
        if (GUILayout.Button(aimbotTargetMode.ToString(), _buttonStyle, GUILayout.Width(112f * Scale), GUILayout.Height(22f * Scale)))
        {
            aimbotTargetMode = aimbotTargetMode == TargetMode.FOV ? TargetMode.Distance : TargetMode.FOV;
        }
        GUILayout.EndHorizontal();
        EndSection();

        BeginSection("Keybind Actions");
        DrawAimbotBinds();
        ToggleRow("Bind Auto Shoot To Aimbot Key", ref aimbotKeyBindsAutoShoot, "Auto Shoot only while aimbot key is held.");
        ToggleRow("Bind Magic Bullet To Aimbot Key", ref aimbotKeyBindsMagicBullet, "Magic Bullet only while aimbot key is held.");
        EndSection();
    }

    private void DrawLegitbotContent()
    {
        BeginSection("Legitbot");
        ToggleRow("Legit Aimbot", ref LegitAimbot, "Softer aim mode.");
        ToggleRow("Legit Auto Shoot", ref LegitAutoShoot, "Auto shoot with legit mode.");
        ToggleRow("Draw FOV Ring", ref drawLegitFOV, "Uses shared FOV ring.");
        SliderRow("Legit Smoothing", ref legitSmoothing, 0.1f, 10f, "F1", "Higher = smoother.");
        SliderRow("Legit FOV", ref legitFOV, 1f, 50f, "F1", "Target selection FOV.");
        KeybindRow("Legit Key", ref boundKey_legitbot, ref waitingForKey, "Key/mouse used to activate legitbot.");
        EndSection();
    }

    private void DrawVisualsContent()
    {
        BeginSection("ESP");
        ToggleRow("ESP Enabled", ref enableESP, "Master switch for overlays.");
        ToggleRow("Stream Proof", ref streamProof, "Hide from capture APIs when supported.");
        EndSection();

        if (!enableESP) return;

        BeginSection("Boxes");
        ToggleRow("Corner Box", ref enableCornerBoxESP, "Corner style box.");
        ToggleRow("2D Box", ref enable2DBoxESP, "Full rectangle box.");
        ToggleRow("3D Box", ref enable3DBoxESP, "3D world wire box.");
        ToggleRow("Skeleton ESP", ref enableSkeletonESP, "Draw bone lines.");
        ToggleRow("Snaplines", ref enableLineToPlayer, "Line from center to target.");
        ToggleRow("Visible Only", ref onlyShowVisiblePlayers, "Only draw visible players.");
        SliderRow("Box Thickness", ref boxThickness, 1f, 5f, "F1", "Box and line thickness.");
        EndSection();

        BeginSection("Info");
        ToggleRow("Name ESP", ref enableNameESP, "Show player names.");
        ToggleRow("Health ESP", ref enableHealthESP, "Master health display toggle.");
        ToggleRow("Distance ESP", ref enableDistanceESP, "Master distance display toggle.");
        ToggleRow("Weapon Text", ref showWeaponText, "Show weapon label.");
        ToggleRow("Distance Text", ref showDistanceText, "Show distance text.");
        ToggleRow("Health Number", ref showHealthNumber, "Show numeric HP.");
        ToggleRow("Health Bar", ref showHealthBar, "Show health bar.");
        ToggleRow("Armor Bar", ref showArmorBar, "Show armor bar if data available.");
        EndSection();

        BeginSection("Colors");
        ColorRow("Enemy Color", ref enemyBoxColor, "Color for enemy boxes/skeleton/snaplines.");
        ColorRow("Friendly Color", ref friendlyBoxColor, "Reserved for team-aware visuals.");
        ColorRow("Text Color", ref textColor, "Overlay text color.");
        EndSection();
    }

    private void DrawExploitsContent()
    {
        BeginSection("Toggles");
        ToggleRow("Rapid Fire", ref RapidFire, "Fast fire interval.");
        ToggleRow("No Spread", ref NoSpread, "Force spread toward configured value.");
        ToggleRow("Insta Kill", ref InstaKill, "Set high damage.");
        ToggleRow("Infinite Ammo", ref InfiniteAmmo, "Refill ammo constantly.");
        ToggleRow("Weapon Speed", ref WeaponSpeed, "Remove weapon movement slowdown.");
        ToggleRow("Magic Bullet", ref MagicBullet, "Damage nearest target on fire.");
        ToggleRow("God Mode [HOST]", ref GodMode, "Host-side health lock.");
        ToggleRow("Freeze Enemy [HOST]", ref FreezeEnemy, "Host-side movement freeze.");
        ToggleRow("Restore Defaults", ref restoreWeaponDefaults, "Restores original weapon values when exploit toggles are off.");
        EndSection();

        BeginSection("Values");
        IntSliderRow("Infinite Ammo Count", ref infiniteAmmoAmount, 60, 5000, "Written while Infinite Ammo is ON.");
        SliderRow("Rapid Fire Interval", ref rapidFireInterval, 0f, 0.2f, "F3", "Written while Rapid Fire is ON.");
        SliderRow("Insta Kill Damage", ref instaKillDamage, 50f, 3000f, "F0", "Written while Insta Kill is ON.");
        SliderRow("Spread Value", ref noSpreadValue, 0f, 0.2f, "F3", "Written while No Spread is ON.");
        SliderRow("Move Factor", ref weaponMovementFactor, 1f, 6f, "F2", "Written while Weapon Speed is ON.");
        SliderRow("Fire Slowdown Factor", ref weaponFireSlowdownFactor, 0f, 1f, "F2", "Written while Weapon Speed is ON.");
        SliderRow("Fire Slowdown Duration", ref weaponFireSlowdownDuration, 0f, 0.5f, "F2", "Written while Weapon Speed is ON.");
        EndSection();
    }

    private void DrawMiscContent()
    {
        BeginSection("Movement");
        ToggleRow("Bunnyhop", ref enableBhop, "Auto bunnyhop while holding Space.");
        ToggleRow("Speed Hack", ref enableSpeedHack, "Override local movement speed.");
        if (enableSpeedHack) SliderRow("Speed Value", ref speedValue, 6f, 50f, "F1", "Applied walk speed.");
        ToggleRow("Fly Mode", ref FlyMode, "Q down / E up vertical movement.");
        if (FlyMode) SliderRow("Fly Vertical Speed", ref flyVerticalSpeed, 1f, 30f, "F1", "Vertical movement speed.");

        GUILayout.BeginHorizontal(_rowStyle);
        GUILayout.Label("Teleport Closest Enemy", _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo("Queues one-time teleport request (same as Z hotkey).");
        if (GUILayout.Button("Teleport", _primaryButtonStyle, GUILayout.Width(90f * Scale), GUILayout.Height(22f * Scale)))
        {
            requestTeleportToEnemy = true;
            AddDebugLog("Teleport requested");
        }
        GUILayout.EndHorizontal();
        EndSection();

        BeginSection("Performance");
        bool prevLow = lowPerformanceMode;
        ToggleRow("Low Performance Mode", ref lowPerformanceMode, "Disables heavy visuals when enabled.");
        ToggleRow("Auto Apply Preset", ref autoApplyLowPerformancePreset, "Automatically applies low-performance visual preset.");
        if (!prevLow && lowPerformanceMode && autoApplyLowPerformancePreset) ApplyLowPerformancePreset();
        EndSection();
    }

    private void DrawAntiAimContent()
    {
        BeginSection("Experimental");
        GUILayout.BeginHorizontal(_rowStyle);
        GUILayout.Label("Anti-aim options are unstable and game-version dependent.", _warningStyle);
        GUILayout.EndHorizontal();
        ToggleRow("Enable Slide State", ref enableIsSlide, "Toggle sliding state.");
        ToggleRow("Enable Grounded State", ref enableIsGrounded, "Toggle grounded state.");
        ToggleRow("Enable Sprinting State", ref enableIsSprinting, "Toggle sprinting state.");
        ToggleRow("Enable Scope State", ref enableIsScopeAiming, "Toggle scope-aiming state.");
        ToggleRow("Enable Lean State", ref enableIsLeaning, "Toggle leaning state.");
        EndSection();
    }

    private void DrawTestingContent()
    {
        BeginSection("Test Entities");
        ToggleRow("Enable Test Entities", ref TestEntityEnabled, "Local fake targets for ESP/aim testing.");
        KeybindRow("Create Entity Key", ref boundKey_createTestEntity, ref _waitingForTestEntityKey, "Key used to spawn a fake target.");

        GUILayout.BeginHorizontal(_rowStyle);
        GUILayout.Label("Create Test Entity", _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo("Creates a fake target near local player.");
        if (GUILayout.Button("Create", _primaryButtonStyle, GUILayout.Width(90f * Scale), GUILayout.Height(22f * Scale)))
        {
            if (Cheat.Instance != null && Cheat.Instance.Cache != null && Cheat.Instance.Cache.TestEntity != null)
            {
                Cheat.Instance.Cache.TestEntity.CreateTestEntity();
            }
        }
        GUILayout.EndHorizontal();
        EndSection();

        BeginSection("UI");
        GUILayout.BeginHorizontal(_rowStyle);
        GUILayout.Label("Reload Menu Styles", _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo("Rebuilds style and texture cache.");
        if (GUILayout.Button("Reload", _buttonStyle, GUILayout.Width(90f * Scale), GUILayout.Height(22f * Scale)))
        {
            BuildStyles();
            AddDebugLog("Styles reloaded");
        }
        GUILayout.EndHorizontal();
        EndSection();
    }

    private void DrawCustomizeContent()
    {
        BeginSection("Theme Presets");
        GUILayout.BeginHorizontal(_rowStyle);
        if (GUILayout.Button("CS2 Steel", _primaryButtonStyle, GUILayout.Height(24f * Scale))) ApplyTheme(0);
        if (GUILayout.Button("Carbon Ember", _buttonStyle, GUILayout.Height(24f * Scale))) ApplyTheme(1);
        if (GUILayout.Button("Slate Ocean", _buttonStyle, GUILayout.Height(24f * Scale))) ApplyTheme(2);
        if (GUILayout.Button("Night Ops", _buttonStyle, GUILayout.Height(24f * Scale))) ApplyTheme(3);
        GUILayout.EndHorizontal();
        EndSection();

        BeginSection("Layout");
        SliderRow("UI Scale Multiplier", ref uiScaleMultiplier, 0.75f, 1.60f, "F2", "Global UI scale.");
        SliderRow("UI Opacity", ref uiOpacity, 0.70f, 1.0f, "F2", "Window transparency.");
        ToggleRow("Show Status Strip", ref showStatusStrip, "Top quick status line.");
        ToggleRow("Show Debug Panel", ref showDebugPanel, "Right debug panel.");
        ToggleRow("Compact Mode", ref compactMode, "Tighter spacing.");
        KeybindRow("Menu Toggle Key", ref menuToggleKey, ref _waitingForMenuKey, "Key used to open/close menu.");
        EndSection();

        BeginSection("Menu Colors");
        ColorRow("Accent Color", ref uiAccentColor, "Primary menu highlight color.");
        ColorRow("Background Color", ref uiBackgroundColor, "Window background color.");
        ColorRow("Panel Color", ref uiPanelColor, "Panel/card color.");
        ColorRow("Text Color", ref uiTextColor, "Primary text color.");
        EndSection();

        BeginSection("Reset");
        GUILayout.BeginHorizontal(_rowStyle);
        GUILayout.Label("Reset UI Customization", _labelStyle);
        GUILayout.FlexibleSpace();
        DrawInfo("Resets theme, scale, opacity and key to defaults.");
        if (GUILayout.Button("Reset", _primaryButtonStyle, GUILayout.Width(90f * Scale), GUILayout.Height(22f * Scale)))
        {
            ResetCustomize();
            AddDebugLog("Customization reset");
        }
        GUILayout.EndHorizontal();
        EndSection();
    }

    private void ApplyTheme(int id)
    {
        switch (id)
        {
            case 0:
                _themeName = "CS2 Steel";
                uiAccentColor = new Color(0.96f, 0.56f, 0.16f, 1f);
                uiBackgroundColor = new Color(0.10f, 0.12f, 0.14f, 1f);
                uiPanelColor = new Color(0.15f, 0.18f, 0.22f, 1f);
                uiTextColor = new Color(0.95f, 0.96f, 0.98f, 1f);
                break;
            case 1:
                _themeName = "Carbon Ember";
                uiAccentColor = new Color(0.96f, 0.38f, 0.24f, 1f);
                uiBackgroundColor = new Color(0.13f, 0.10f, 0.11f, 1f);
                uiPanelColor = new Color(0.21f, 0.16f, 0.18f, 1f);
                uiTextColor = new Color(0.98f, 0.94f, 0.92f, 1f);
                break;
            case 2:
                _themeName = "Slate Ocean";
                uiAccentColor = new Color(0.27f, 0.69f, 0.98f, 1f);
                uiBackgroundColor = new Color(0.07f, 0.11f, 0.16f, 1f);
                uiPanelColor = new Color(0.12f, 0.18f, 0.25f, 1f);
                uiTextColor = new Color(0.90f, 0.95f, 1f, 1f);
                break;
            default:
                _themeName = "Night Ops";
                uiAccentColor = new Color(0.51f, 0.85f, 0.34f, 1f);
                uiBackgroundColor = new Color(0.07f, 0.09f, 0.07f, 1f);
                uiPanelColor = new Color(0.12f, 0.16f, 0.12f, 1f);
                uiTextColor = new Color(0.93f, 0.97f, 0.90f, 1f);
                break;
        }
        AddDebugLog("Theme: " + _themeName);
    }

    private void ResetCustomize()
    {
        uiScaleMultiplier = 1f;
        uiOpacity = 0.95f;
        showStatusStrip = true;
        showDebugPanel = true;
        compactMode = false;
        menuToggleKey = KeyCode.F6;
        ApplyTheme(0);
    }

    private void ApplyLowPerformancePreset()
    {
        enable3DBoxESP = false;
        enableSkeletonESP = false;
        enableLineToPlayer = false;
        showArmorBar = false;
        showWeaponText = false;
        AddDebugLog("Low performance preset applied");
    }

    public void AddDebugLog(string message)
    {
        _debugLogs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        if (_debugLogs.Count > MAX_LOGS) _debugLogs.RemoveAt(0);
    }

    public bool IsAimbotKeyPressed()
    {
        if (aimbotKeys == null) aimbotKeys = new List<KeyCode> { KeyCode.E };
        if (aimbotKeys.Count == 0) return Input.GetKey(boundKey_aimbot);

        for (int i = 0; i < aimbotKeys.Count; i++)
        {
            if (Input.GetKey(aimbotKeys[i])) return true;
        }
        return false;
    }
}
