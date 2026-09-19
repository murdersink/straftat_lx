using System.Collections.Generic;
using UnityEngine;

namespace STRAFTAT_CC.Features
{
    public class WeaponMods
    {
        private struct WeaponSnapshot
        {
            public int AmmoCharge;
            public int CurrentAmmo;
            public bool OnePressShoot;
            public float TimeBetweenBullets;
            public float TimeBetweenFire;
            public float Damage;
            public float MinSpread;
            public float MaxSpread;
            public float MovementFactor;
            public float FireSlowDownFactor;
            public float FireSlowDownDuration;
        }

        private static readonly Dictionary<int, WeaponSnapshot> _weaponDefaults = new Dictionary<int, WeaponSnapshot>();
        private static readonly Dictionary<int, bool> _weaponWasModified = new Dictionary<int, bool>();

        private static bool IsValid(Weapon weapon)
        {
            return weapon != null;
        }

        private static void CaptureDefaults(Weapon weapon)
        {
            if (!IsValid(weapon))
                return;

            int id = weapon.GetInstanceID();
            if (_weaponDefaults.ContainsKey(id))
                return;

            _weaponDefaults[id] = new WeaponSnapshot
            {
                AmmoCharge = weapon.ammoCharge,
                CurrentAmmo = weapon.currentAmmo,
                OnePressShoot = weapon.onePressShoot,
                TimeBetweenBullets = weapon.timeBetweenBullets,
                TimeBetweenFire = weapon.timeBetweenFire,
                Damage = weapon.damage,
                MinSpread = weapon.minSpread,
                MaxSpread = weapon.maxSpread,
                MovementFactor = weapon.movementFactor,
                FireSlowDownFactor = weapon.fireSlowDownFactor,
                FireSlowDownDuration = weapon.fireSlowDownDuration
            };
        }

        private static void ApplySnapshot(Weapon weapon, WeaponSnapshot snapshot)
        {
            if (!IsValid(weapon))
                return;

            weapon.ammoCharge = snapshot.AmmoCharge;
            weapon.currentAmmo = snapshot.CurrentAmmo;
            weapon.onePressShoot = snapshot.OnePressShoot;
            weapon.timeBetweenBullets = snapshot.TimeBetweenBullets;
            weapon.timeBetweenFire = snapshot.TimeBetweenFire;
            weapon.damage = snapshot.Damage;
            weapon.minSpread = snapshot.MinSpread;
            weapon.maxSpread = snapshot.MaxSpread;
            weapon.movementFactor = snapshot.MovementFactor;
            weapon.fireSlowDownFactor = snapshot.FireSlowDownFactor;
            weapon.fireSlowDownDuration = snapshot.FireSlowDownDuration;
        }

        private static void ApplyConfiguredMods(Weapon weapon)
        {
            if (!IsValid(weapon))
                return;

            CaptureDefaults(weapon);
            int id = weapon.GetInstanceID();
            if (!_weaponDefaults.TryGetValue(id, out WeaponSnapshot baseline))
                return;

            bool hasActiveMods = Config.Instance.InfiniteAmmo ||
                                 Config.Instance.RapidFire ||
                                 Config.Instance.InstaKill ||
                                 Config.Instance.NoSpread ||
                                 Config.Instance.WeaponSpeed;

            bool wasModified = _weaponWasModified.TryGetValue(id, out bool previousState) && previousState;

            // Nothing enabled: only restore once after transitioning from modified->normal.
            if (!hasActiveMods)
            {
                if (Config.Instance.restoreWeaponDefaults && wasModified)
                {
                    ApplySnapshot(weapon, baseline);
                }

                _weaponWasModified[id] = false;
                return;
            }

            // Start from a clean baseline once when entering a modified state.
            if (Config.Instance.restoreWeaponDefaults && !wasModified)
            {
                ApplySnapshot(weapon, baseline);
            }

            if (Config.Instance.InfiniteAmmo)
            {
                int ammo = Mathf.Max(1, Config.Instance.infiniteAmmoAmount);
                weapon.ammoCharge = ammo;
                weapon.currentAmmo = ammo;
            }

            if (Config.Instance.RapidFire)
            {
                float interval = Mathf.Max(0f, Config.Instance.rapidFireInterval);
                weapon.onePressShoot = false;
                weapon.timeBetweenBullets = interval;
                weapon.timeBetweenFire = interval;
            }

            if (Config.Instance.InstaKill)
            {
                weapon.damage = Mathf.Max(1f, Config.Instance.instaKillDamage);
            }

            if (Config.Instance.NoSpread)
            {
                float spread = Mathf.Max(0f, Config.Instance.noSpreadValue);
                weapon.minSpread = spread;
                weapon.maxSpread = spread;
            }

            if (Config.Instance.WeaponSpeed)
            {
                weapon.movementFactor = Mathf.Max(0f, Config.Instance.weaponMovementFactor);
                weapon.fireSlowDownFactor = Mathf.Max(0f, Config.Instance.weaponFireSlowdownFactor);
                weapon.fireSlowDownDuration = Mathf.Max(0f, Config.Instance.weaponFireSlowdownDuration);
            }

            _weaponWasModified[id] = true;
        }

        public static void Update()
        {
            Weapon leftWeapon = Cheat.Instance.Cache.LocalWeaponLeft;
            Weapon rightWeapon = Cheat.Instance.Cache.LocalWeaponRight;

            ApplyConfiguredMods(leftWeapon);
            ApplyConfiguredMods(rightWeapon);
        }
    }
}
