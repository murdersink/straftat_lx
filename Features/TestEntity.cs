using System.Collections.Generic;
using UnityEngine;

namespace STRAFTAT_CC.Features
{
    public class TestEntity
    {
        private readonly Cache _cache;
        private readonly List<FakeEntity> _testEntities = new List<FakeEntity>();
        private int _selectedEntityIndex = -1;

        private GameObject _proxyTargetObject;
        private Transform _proxyTargetHead;
        private PlayerHealth _proxyTargetHealth;
        private PlayerCache _proxyTargetCache;

        private class FakeEntity
        {
            public Vector3 Position;
            public float Health;
            public Vector3 HeadPosition;
            public bool IsValid;

            public FakeEntity(Vector3 position)
            {
                Position = position;
                HeadPosition = position + new Vector3(0f, 1.7f, 0f);
                Health = 60f;
                IsValid = true;
            }
        }

        public TestEntity(Cache cache)
        {
            _cache = cache;
        }

        public void Update()
        {
            if (!Config.Instance.TestEntityEnabled)
            {
                ClearAllEntities();
                return;
            }

            if (Input.GetKeyDown(Config.Instance.boundKey_createTestEntity))
            {
                CreateTestEntity();
            }
        }

        public void CreateEntityButtonPressed()
        {
            CreateTestEntity();
        }

        public void CreateTestEntity()
        {
            if (_cache.LocalPlayer == null || _cache.LocalPlayer.GameObject == null)
                return;

            Transform local = _cache.LocalPlayer.GameObject.transform;
            Vector3 spawnPos = local.position + (local.forward * 5f) + (local.right * Random.Range(-1.5f, 1.5f));
            spawnPos.y = local.position.y + 0.2f;

            _testEntities.Add(new FakeEntity(spawnPos));
            Config.Instance.AddDebugLog($"Created test entity {_testEntities.Count}");
        }

        private void ClearAllEntities()
        {
            _testEntities.Clear();
            _selectedEntityIndex = -1;
            DestroyProxy();
        }

        private void DestroyProxy()
        {
            if (_proxyTargetObject != null)
            {
                Object.Destroy(_proxyTargetObject);
            }
            _proxyTargetObject = null;
            _proxyTargetHead = null;
            _proxyTargetHealth = null;
            _proxyTargetCache = null;
        }

        private void EnsureProxy()
        {
            if (_proxyTargetObject != null)
                return;

            _proxyTargetObject = new GameObject("TestEntityProxy");
            _proxyTargetObject.hideFlags = HideFlags.HideAndDontSave;
            try
            {
                _proxyTargetObject.tag = "Player";
            }
            catch
            {
                // Tag might not be available in some contexts.
            }

            _proxyTargetHealth = _proxyTargetObject.AddComponent<PlayerHealth>();
            SphereCollider collider = _proxyTargetObject.AddComponent<SphereCollider>();
            collider.radius = 0.35f;
            collider.center = new Vector3(0f, 0.85f, 0f);

            GameObject head = new GameObject("Head_Col");
            head.transform.SetParent(_proxyTargetObject.transform);
            _proxyTargetHead = head.transform;

            _proxyTargetCache = new PlayerCache(_proxyTargetObject);
        }

        public void OnGUI()
        {
            if (!Config.Instance.TestEntityEnabled || _cache.MainCamera == null)
                return;

            float windowWidth = 220f;
            float windowHeight = 300f;
            float windowX = Screen.width - windowWidth - 10f;
            float windowY = 10f;

            GUI.Box(new Rect(windowX, windowY, windowWidth, windowHeight), "Test Entities");

            float contentY = windowY + 28f;
            const float buttonHeight = 24f;
            const float spacing = 4f;

            for (int i = 0; i < _testEntities.Count; i++)
            {
                FakeEntity entity = _testEntities[i];
                if (!entity.IsValid)
                    continue;

                bool isSelected = i == _selectedEntityIndex;
                if (GUI.Button(new Rect(windowX + 10f, contentY, windowWidth - 20f, buttonHeight), $"Entity {i + 1}" + (isSelected ? " [Selected]" : "")))
                {
                    _selectedEntityIndex = i;
                }
                contentY += buttonHeight + spacing;
            }

            if (_selectedEntityIndex >= 0 && _selectedEntityIndex < _testEntities.Count)
            {
                FakeEntity selected = _testEntities[_selectedEntityIndex];
                if (selected.IsValid)
                {
                    contentY += spacing;
                    GUI.Label(new Rect(windowX + 10f, contentY, windowWidth - 20f, buttonHeight), $"Health: {selected.Health:F1}");
                    contentY += buttonHeight;

                    float distance = Vector3.Distance(_cache.MainCamera.transform.position, selected.Position);
                    GUI.Label(new Rect(windowX + 10f, contentY, windowWidth - 20f, buttonHeight), $"Distance: {distance:F1}m");
                    contentY += buttonHeight;

                    if (GUI.Button(new Rect(windowX + 10f, contentY, windowWidth - 20f, buttonHeight), "Destroy Selected"))
                    {
                        _testEntities.RemoveAt(_selectedEntityIndex);
                        _selectedEntityIndex = -1;
                        Config.Instance.AddDebugLog("Destroyed selected test entity");
                    }
                }
            }

            DrawEntityEsp();
        }

        private void DrawEntityEsp()
        {
            for (int i = 0; i < _testEntities.Count; i++)
            {
                FakeEntity entity = _testEntities[i];
                if (!entity.IsValid)
                    continue;

                Vector3 feet = _cache.MainCamera.WorldToScreenPoint(entity.Position);
                if (feet.z <= 0f)
                    continue;

                Vector3 head = _cache.MainCamera.WorldToScreenPoint(entity.HeadPosition);
                if (head.z <= 0f)
                    continue;

                float distance = Vector3.Distance(_cache.MainCamera.transform.position, entity.Position);
                float boxHeight = Mathf.Abs(feet.y - head.y);
                float boxWidth = boxHeight * 0.55f;
                Rect box = new Rect(head.x - (boxWidth / 2f), Screen.height - head.y, boxWidth, boxHeight);

                if (Config.Instance.enable3DBoxESP)
                {
                    Bounds bounds = new Bounds(entity.Position + Vector3.up * 0.85f, new Vector3(0.5f, 1.7f, 0.5f));
                    Utils.SetupExtentsBounds(bounds);
                    Utils.Draw3DBox(_cache.MainCamera, Config.Instance.enemyBoxColor);
                }
                else if (Config.Instance.enableCornerBoxESP)
                {
                    Drawing.DrawCornerBox(box, Config.Instance.boxThickness, Config.Instance.enemyBoxColor);
                }
                else if (Config.Instance.enable2DBoxESP)
                {
                    Drawing.DrawBox(box, Config.Instance.boxThickness, Config.Instance.enemyBoxColor);
                }

                if (Config.Instance.enableHealthESP && Config.Instance.showHealthBar)
                {
                    float pct = Mathf.Clamp01(entity.Health / 100f);
                    Rect bg = new Rect(box.x - 6f, box.y, 4f, box.height);
                    Rect fill = new Rect(bg.x, bg.y + (box.height * (1f - pct)), bg.width, box.height * pct);
                    Drawing.DrawFilledBox(bg, new Color(0f, 0f, 0f, 0.5f));
                    Drawing.DrawFilledBox(fill, Utils.DoubleColorLerp(pct, Color.green, Color.yellow, Color.red));
                    Drawing.DrawBox(bg, 1f, Color.black);
                }

                float textY = box.y + box.height + 2f;
                if (Config.Instance.enableNameESP)
                {
                    Utils.DrawText(new Vector2(box.center.x, box.y - 14f), "Test Entity", Config.Instance.textColor, 12, true);
                }
                if (Config.Instance.enableDistanceESP && Config.Instance.showDistanceText)
                {
                    Utils.DrawText(new Vector2(box.center.x, textY), $"{distance:F1}m", Config.Instance.textColor, 11, true);
                    textY += 12f;
                }
                if (Config.Instance.enableHealthESP && Config.Instance.showHealthNumber)
                {
                    Utils.DrawText(new Vector2(box.center.x, textY), $"{entity.Health:F0} HP", Config.Instance.textColor, 11, true);
                }
            }
        }

        public PlayerCache GetClosestTestEntity()
        {
            if (!Config.Instance.TestEntityEnabled || _testEntities.Count == 0 || _cache.LocalPlayer == null || _cache.LocalPlayer.GameObject == null)
                return null;

            FakeEntity closest = null;
            float closestDistance = float.MaxValue;
            Vector3 localPos = _cache.LocalPlayer.GameObject.transform.position;

            for (int i = 0; i < _testEntities.Count; i++)
            {
                FakeEntity entity = _testEntities[i];
                if (!entity.IsValid)
                    continue;

                float d = Vector3.Distance(localPos, entity.Position);
                if (d < closestDistance)
                {
                    closestDistance = d;
                    closest = entity;
                }
            }

            if (closest == null)
                return null;

            EnsureProxy();
            _proxyTargetObject.transform.position = closest.Position;
            _proxyTargetHead.position = closest.HeadPosition;
            if (_proxyTargetHealth != null)
            {
                _proxyTargetHealth.health = closest.Health;
            }

            if (_proxyTargetCache == null)
            {
                _proxyTargetCache = new PlayerCache(_proxyTargetObject);
            }

            return _proxyTargetCache;
        }
    }
}
