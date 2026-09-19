using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRAFTAT_CC.Features
{
    public class ESP
    {
        public static void OnGUI()
        {
            if (Config.Instance.enableESP)
            {
                foreach (var player in Cheat.Instance.Cache.Players)
                    player.Draw(Cheat.Instance.Cache.MainCamera);

                if (Config.Instance.TestEntityEnabled)
                {
                    Cheat.Instance.Cache.TestEntity.OnGUI();
                }
            }
        }
    }
}
