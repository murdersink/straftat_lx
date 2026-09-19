using BepInEx;

namespace STRAFTAT_CC
{
    [BepInPlugin("straftat.cc", "STRAFTAT.CC", "1.0.0")]
    public sealed class BepInExPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Loader.Init();
        }
    }
}
