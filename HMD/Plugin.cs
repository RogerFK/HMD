using System.Collections.Generic;
using ItemManager;
using scp4aiur;
using Smod2.API;
using Smod2.Attributes;

namespace HMD
{
    [PluginDetails(
        author = "Androx",
        description = "Adds a heavy sniper weapon.",
        id = "androx.custom.hmd",
        name = "HMD",
        SmodMajor = 3,
        SmodMinor = 2,
        SmodRevision = 0,
        version = "1.0.0"
        )]
    public class Plugin : Smod2.Plugin
    {
        internal static int[] lockElevator;
        internal static int lockWarhead;
        internal static Dictionary<Door, int> lockDoor;
        internal static Dictionary<Door, bool> lockDoorStatus;

        public override void Register()
        {
            Timing.Init(this);

            Items.RegisterWeapon<Hmd>(102, 15);
        }

        public override void OnEnable() { }
        public override void OnDisable() { }
    }
}
