using ItemManager;
using scp4aiur;
using Smod2.API;
using Smod2.Attributes;
using Smod2.Config;
using Smod2.Events;

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
        public const int HmdId = 102;

        private static Plugin instance;

        internal static float doubleDropTime;

        internal static int[] roleSpawns;
        internal static int[] itemSpawns;

        internal static float bodyDamage;
        internal static float headDamage;
        internal static float legDamage;
        internal static float scp106Damage;
        internal static float tagDamage;

        internal static float fireRate;
        internal static int magazine;

        internal static int krakatoa;
        internal static int suppressedKrakatoa;

        internal static bool overChargeable;
        internal static float overChargeRadius;
        internal static float overChargeDamage;
        internal static bool overCharageNukeEffect;

        internal static float tagTime;
        internal static int tagGlitches;

        internal static int chaosHmds;
        internal static int mtfHmds;
        internal static int[] mtfHmdRoles;

        public override void Register()
        {
            instance = this;

            AddConfig(new ConfigSetting("hmd_doubledrop_time", 0.25f, SettingType.FLOAT, true, "Time that a player must double right click in order to toggle overcharge."));

            AddConfig(new ConfigSetting("hmd_role_spawns", new int[0], SettingType.NUMERIC_LIST, true, "Role spawn locations where the HMD should spawn."));
            AddConfig(new ConfigSetting("hmd_item_spawns", new[]
            {
                (int)ItemType.MTF_COMMANDER_KEYCARD
            }, SettingType.NUMERIC_LIST, true, "Item spawn locations where the HMD should spawn."));

            AddConfig(new ConfigSetting("hmd_body_damage", 80f, SettingType.FLOAT, true, "Damage per shot of the rifle on bodies."));
            AddConfig(new ConfigSetting("hmd_head_damage", 105f, SettingType.FLOAT, true, "Damage per shot of the rifle on heads."));
            AddConfig(new ConfigSetting("hmd_leg_damage", 60f, SettingType.FLOAT, true, "Damage per shot of the rifle on legs."));
            AddConfig(new ConfigSetting("hmd_106_damage", 12f, SettingType.FLOAT, true, "Damage per shot of the rifle on SCP-106."));
            AddConfig(new ConfigSetting("hmd_tag_damage", 10f, SettingType.FLOAT, true, "Damage per shot of the rifle when overcharged."));

            AddConfig(new ConfigSetting("hmd_fire_rate", 2f, SettingType.FLOAT, true, "Time (in seconds) between each shot."));
            AddConfig(new ConfigSetting("hmd_magazine", 5, SettingType.NUMERIC, true, "Amount of shots per magazine."));
            AddConfig(new ConfigSetting("hmd_reserve_ammo", 15, SettingType.NUMERIC, true, "Amount of HMD masses in reserve. Refreshed on server restart."));

            AddConfig(new ConfigSetting("hmd_krakatoa", 15, SettingType.NUMERIC, true, "Additional shot sounds per HMD shot."));
            AddConfig(new ConfigSetting("hmd_suppressed_krakatoa", 7, SettingType.NUMERIC, true, "Additional shot sounds pre suppressed HMD shot."));

            AddConfig(new ConfigSetting("hmd_overchargeable", true, SettingType.BOOL, true, "Allows toggling of overcharge mode."));
            AddConfig(new ConfigSetting("hmd_overcharge_radius", 15f, SettingType.FLOAT, true, "Radius of the overcharge device's bodyDamage."));
            AddConfig(new ConfigSetting("hmd_overcharge_damage", 30f, SettingType.FLOAT, true, "Damage of the overcharge device per person."));
            AddConfig(new ConfigSetting("hmd_overcharge_glitch", true, SettingType.BOOL, true, "Whether or not to apply the glitchy (nuke) effect to players hit by the overcharge device."));

            AddConfig(new ConfigSetting("hmd_tag_time", 5f, SettingType.FLOAT, true, "Time after tagging someone with overcharge to detonation."));
            AddConfig(new ConfigSetting("hmd_tag_glitches", 2, SettingType.NUMERIC, true, "Additional glitch effects to play when an overcharge device detonates on the tagged player."));

            AddConfig(new ConfigSetting("hmd_chaos_count", 0, SettingType.NUMERIC, true, "Amount of HMDs given to Chaos Insurgency respawns."));
            AddConfig(new ConfigSetting("hmd_mtf_count", 1, SettingType.NUMERIC, true, "Amount of HMDs given to MTF respawns."));
            AddConfig(new ConfigSetting("hmd_mtf_roles", new[]
            {
                (int)Role.NTF_COMMANDER
            }, SettingType.NUMERIC_LIST, true, "MTF roles that can receive HMDs."));

            Timing.Init(this);
            Items.RegisterWeapon<Hmd>(102, GetConfigInt("hmd_reserve_ammo"));
            AddEventHandlers(new EventHandlers(), Priority.Low);
        }

        public static void ReloadConfig()
        {
            doubleDropTime = instance.GetConfigFloat("hmd_doubledrop_time");

            roleSpawns = instance.GetConfigIntList("hmd_role_spawns");
            itemSpawns = instance.GetConfigIntList("hmd_item_spawns");

            bodyDamage = instance.GetConfigFloat("hmd_body_damage");
            headDamage = instance.GetConfigFloat("hmd_head_damage");
            legDamage = instance.GetConfigFloat("hmd_leg_damage");
            scp106Damage = instance.GetConfigFloat("hmd_106_damage");
            tagDamage = instance.GetConfigFloat("hmd_tag_damage");

            fireRate = instance.GetConfigFloat("hmd_fire_rate");
            magazine = instance.GetConfigInt("hmd_magazine");

            krakatoa = instance.GetConfigInt("hmd_krakatoa");
            suppressedKrakatoa = instance.GetConfigInt("hmd_suppressed_krakatoa");

            overChargeable = instance.GetConfigBool("hmd_overchargeable");
            overChargeRadius = instance.GetConfigFloat("hmd_overcharge_radius");
            overChargeDamage = instance.GetConfigFloat("hmd_overcharge_damage");
            overCharageNukeEffect = instance.GetConfigBool("hmd_overcharge_glitch");

            tagTime = instance.GetConfigFloat("hmd_tag_time");
            tagGlitches = instance.GetConfigInt("hmd_tag_glitches");

            chaosHmds = instance.GetConfigInt("hmd_chaos_count");
            mtfHmds = instance.GetConfigInt("hmd_mtf_count");
            mtfHmdRoles = instance.GetConfigIntList("hmd_mtf_roles");
        }

        public override void OnEnable() { }
        public override void OnDisable() { }
    }
}
