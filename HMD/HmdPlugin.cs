using ItemManager;
using ItemManager.Utilities;
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
        SmodRevision = 2,
        version = "1.0.1"
        )]
    public class HmdPlugin : Smod2.Plugin
    {
        public const int HmdId = 102;

        public static HmdPlugin Instance { get; private set; }
        public CustomWeaponHandler<Hmd> Handler { get; private set; }

        public static float DoubleDropTime { get; private set; }

        public int[] RoleSpawns { get; private set; }
        public int[] ItemSpawns { get; private set; }

        public static float BodyDamage { get; private set; }
        public static float HeadDamage { get; private set; }
        public static float LegDamage { get; private set; }
        public static float Scp106Damage { get; private set; }
        public static float TagDamage { get; private set; }

        public static float FireRate { get; private set; }
        public static int Magazine { get; private set; }

        public static int Krakatoa { get; private set; }
        public static int SuppressedKrakatoa { get; private set; }

        public static bool OverChargeable { get; private set; }
        public static float OverChargeRadius { get; private set; }
        public static float OverChargeDamage { get; private set; }
        public static bool OverCharageNukeEffect { get; private set; }

        public static float TagTime { get; private set; }
        public static int TagGlitches { get; private set; }

        public int ChaosHmds { get; private set; }
        public int MtfHmds { get; private set; }
        public int[] MtfHmdRoles { get; private set; }

        public override void Register()
        {
            Instance = this;

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
            Handler = new CustomWeaponHandler<Hmd>(102)
            {
                AmmoName = "Heavy Masses",
                DroppedAmmoCount = 5,
                DefaultType = ItemType.E11_STANDARD_RIFLE
            };
            ReloadConfig();
            Handler.Register();
            AddEventHandlers(new EventHandlers(this), Priority.Low);
        }

        public void ReloadConfig()
        {
            DoubleDropTime = GetConfigFloat("hmd_doubledrop_time");

            RoleSpawns = GetConfigIntList("hmd_role_spawns");
            ItemSpawns = GetConfigIntList("hmd_item_spawns");

            BodyDamage = GetConfigFloat("hmd_body_damage");
            HeadDamage = GetConfigFloat("hmd_head_damage");
            LegDamage = GetConfigFloat("hmd_leg_damage");
            Scp106Damage = GetConfigFloat("hmd_106_damage");
            TagDamage = GetConfigFloat("hmd_tag_damage");

            FireRate = GetConfigFloat("hmd_fire_rate");
            Magazine = GetConfigInt("hmd_magazine");
            Handler.DefaultReserveAmmo = GetConfigInt("hmd_reserve_ammo");

            Krakatoa = GetConfigInt("hmd_krakatoa");
            SuppressedKrakatoa = GetConfigInt("hmd_suppressed_krakatoa");

            OverChargeable = GetConfigBool("hmd_overchargeable");
            OverChargeRadius = GetConfigFloat("hmd_overcharge_radius");
            OverChargeDamage = GetConfigFloat("hmd_overcharge_damage");
            OverCharageNukeEffect = GetConfigBool("hmd_overcharge_glitch");

            TagTime = GetConfigFloat("hmd_tag_time");
            TagGlitches = GetConfigInt("hmd_tag_glitches");

            ChaosHmds = GetConfigInt("hmd_chaos_count");
            MtfHmds = GetConfigInt("hmd_mtf_count");
            MtfHmdRoles = GetConfigIntList("hmd_mtf_roles");
        }

        public override void OnEnable()
        {
            Info("HMD enabled.");
        }

        public override void OnDisable()
        {
            Info("HMD disabled.");
        }
    }
}
