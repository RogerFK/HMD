using ItemManager;
using scp4aiur;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;

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
        private static Plugin instance;

        internal static int damage;
        internal static float fireRate;
        internal static int magazine;

        internal static int krakatoa;
        internal static int suppressedKrakatoa;

        internal static bool overChargeable;
        internal static float overChargeRadius;
        internal static float overChargeDamage;
        internal static bool overCharageNukeEffect;

        internal static int chaosHmds;
        internal static int mtfHmds;
        internal static int[] mtfHmdRoles;

        public override void Register()
        {
            instance = this;
            
            AddConfig(new ConfigSetting("hmd_damage", 80, SettingType.NUMERIC, true, "Damage per shot of the rifle."));
            AddConfig(new ConfigSetting("hmd_fire_rate", 2f, SettingType.FLOAT, true, "Time (in seconds) between each shot."));
            AddConfig(new ConfigSetting("hmd_magazine", 5, SettingType.NUMERIC, true, "Amount of shots per magazine."));

            AddConfig(new ConfigSetting("hmd_krakatoa", 15, SettingType.NUMERIC, true, "Additional shot sounds per HMD shot."));
            AddConfig(new ConfigSetting("hmd_suppressed_krakatoa", 7, SettingType.NUMERIC, true, "Additional shot sounds pre suppressed HMD shot."));

            AddConfig(new ConfigSetting("hmd_overchargeable", true, SettingType.BOOL, true, "Allows toggling of overcharge mode."));
            AddConfig(new ConfigSetting("hmd_overcharge_radius", 15f, SettingType.FLOAT, true, "Radius of the overcharge device's damage."));
            AddConfig(new ConfigSetting("hmd_overcharge_damage", 30, SettingType.NUMERIC, true, "Damage of the overcharge device per person."));
            AddConfig(new ConfigSetting("hmd_overcharge_glitch", true, SettingType.BOOL, true, "Whether or not to apply the glitchy (nuke) effect to players hit by the overcharge device."));

            AddConfig(new ConfigSetting("hmd_chaos_count", 0, SettingType.NUMERIC, true, "Amount of HMDs given to Chaos Insurgency respawns."));
            AddConfig(new ConfigSetting("hmd_mtf_count", 1, SettingType.NUMERIC, true, "Amount of HMDs given to MTF respawns."));
            AddConfig(new ConfigSetting("hmd_mtf_roles", new[]
            {
                (int)Role.NTF_LIEUTENANT
            }, SettingType.NUMERIC_LIST, true, "MTF roles that can receive HMDs."));

            Timing.Init(this);
            Items.RegisterWeapon<Hmd>(102, 15);
            AddEventHandlers(new ItemManager.EventHandlers());
        }

        public static void ReloadConfig()
        {
            damage = instance.GetConfigInt("hmd_damage");
            fireRate = instance.GetConfigFloat("hmd_fire_rate");
            magazine = instance.GetConfigInt("hmd_magazine");

            krakatoa = instance.GetConfigInt("hmd_krakatoa");
            suppressedKrakatoa = instance.GetConfigInt("hmd_suppressed_krakatoa");

            overChargeable = instance.GetConfigBool("hmd_overchargeable");
            overChargeRadius = instance.GetConfigFloat("hmd_overcharge_radius");
            overChargeDamage = instance.GetConfigFloat("hmd_overcharge_damage");
            overCharageNukeEffect = instance.GetConfigBool("hmd_overcharge_glitch");

            chaosHmds = instance.GetConfigInt("hmd_chaos_count");
            mtfHmds = instance.GetConfigInt("hmd_mtf_count");
            mtfHmdRoles = instance.GetConfigIntList("hmd_mtf_roles");
        }

        public override void OnEnable() { }
        public override void OnDisable() { }
    }
}
