using ItemManager;
using scp4aiur;
using Smod2.API;
using UnityEngine;

namespace HMD
{
    public class Hmd : CustomWeapon
    {
        // Found in PlayerInteract
        private const int InteractMask = 0;
        private const float InteractDistance = 0;

        private const float ShotDamage = 90f;
        private const int Krakatoa = 15;
        private const int SuppressedKrakatoa = 7;

        public override ItemType DefaultItemId => ItemType.E11_STANDARD_RIFLE;
        public override int MagazineCapacity => 5;
        public override float FireRate => 5f;
        public float DoubleDropWindow => 0.25f;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Sight = 3;
            Barrel = 3;
            MiscAttachment = 3;
        }

        public override void OnShoot(GameObject target, ref float damage)
        {
            Sight = 4;
            Timing.In(x => Sight = 3, FireRate);
            
            damage = ShotDamage;

            WeaponManager weps = PlayerObject.GetComponent<WeaponManager>();
            int shots = Barrel == 1 ? SuppressedKrakatoa : Krakatoa;
            for (int i = 0; i < shots; i++)
            {
                weps.CallRpcConfirmShot(false, weps.curWeapon);
            }

        }
    }
}
