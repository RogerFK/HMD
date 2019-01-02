using System.Linq;
using ItemManager;
using ItemManager.Events;
using RemoteAdmin;
using scp4aiur;
using Smod2.API;
using UnityEngine;
using UnityEngine.Networking;

namespace HMD
{
    public class Hmd : CustomWeapon, IDoubleDroppable
    {
        private const int WorldMask = 1207976449;
        private const int PlayerMask = 1208246273;
        private const float GlitchLength = 1.9f;

        private bool overCharged;

        public override ItemType DefaultItemId => ItemType.E11_STANDARD_RIFLE;
        public override int MagazineCapacity => Plugin.magazine;
        public override float FireRate => Plugin.fireRate;

        public float DoubleDropWindow => Plugin.overChargeable ? Plugin.doubleDropTime : 0;
        public bool OnDoubleDrop()
        {
            overCharged = !overCharged;

            Sight = overCharged ? 3 : 4;

            return false;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Sight = 4;
            Barrel = 3;
            MiscAttachment = 3;
        }

        private static void TargetShake(GameObject target)
        {
            int rpcId = -737840022;

            NetworkWriter writer = new NetworkWriter();
            writer.Write((short)0);
            writer.Write((short)2);
            writer.WritePackedUInt32((uint)rpcId);
            writer.Write(target.GetComponent<NetworkIdentity>().netId);
            writer.FinishMessage();
            target.GetComponent<CharacterClassManager>().connectionToClient.SendWriter(writer, 0);
        }

        private void OverchargeDetonate(WeaponManager weps, Vector3 hit)
        {
            foreach (GameObject player in PlayerManager.singleton.players.Except(new[] { PlayerObject })
                .Where(y => Vector3.Distance(y.GetComponent<PlyMovementSync>().position, hit) < Plugin.overChargeRadius &&
                            weps.GetShootPermission(y.GetComponent<CharacterClassManager>())))
            {
                player.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(
                    Plugin.overChargeDamage,
                    PlayerObject.GetComponent<NicknameSync>().myNick + " (" +
                    PlayerObject.GetComponent<CharacterClassManager>().SteamId + ")",
                    DamageTypes.Tesla,
                    PlayerObject.GetComponent<QueryProcessor>().PlayerId
                ), player);

                if (Plugin.overCharageNukeEffect)
                {
                    TargetShake(player);
                }
            }
        }

        protected override void OnValidShoot(GameObject target, ref float damage)
        {
            WeaponManager weps = PlayerObject.GetComponent<WeaponManager>();
            Transform cam = PlayerObject.GetComponent<Scp049PlayerScript>().plyCam.transform;

            Ray ray = new Ray(cam.position, cam.forward);
            Physics.Raycast(cam.position + cam.forward, cam.forward, out RaycastHit playerHit, PlayerMask);
            HitboxIdentity hitbox = playerHit.collider?.GetComponent<HitboxIdentity>();

            if (overCharged)
            {
                damage = Plugin.tagDamage;

                if (hitbox != null)
                {
                    Timing.In(x =>
                    {
                        OverchargeDetonate(weps, target.GetComponent<PlyMovementSync>().position);
                        if (weps.GetShootPermission(target.GetComponent<CharacterClassManager>()))
                        {
                            float glitchTime = x;
                            for (int i = 0; i < Plugin.tagGlitches; i++)
                            {
                                Timing.In(y => TargetShake(target), glitchTime += GlitchLength);
                            }
                        }
                    }, Plugin.tagTime);
                }
                else if (Physics.Raycast(ray, out RaycastHit hit, 500f, WorldMask))
                {
                    Timing.In(x =>
                    {
                        OverchargeDetonate(weps, hit.point);
                    }, DetonateFlash(hit.point));
                }
            }
            else
            {
                switch (hitbox?.id.ToUpper())
                {
                    case "HEAD":
                        damage = Plugin.headDamage;
                        break;

                    case "LEG":
                        damage = Plugin.legDamage;
                        break;

                    case "SCP106":
                        damage = Plugin.scp106Damage;
                        break;

                    default:
                        damage = Plugin.bodyDamage;
                        break;
                }
            }

            
            int shots = Barrel == 1 ? Plugin.suppressedKrakatoa : Plugin.krakatoa;
            for (int i = 0; i < shots; i++)
            {
                weps.CallRpcConfirmShot(false, weps.curWeapon);
            }
        }

        private float DetonateFlash(Vector3 pos)
        {
            const int id = 1;
            Vector3 dir = Vector3.zero;
            const float throwForce = 0;

            GrenadeManager component1 = PlayerObject.GetComponent<GrenadeManager>();
            Grenade component2 = Object.Instantiate(component1.availableGrenades[id].grenadeInstance).GetComponent<Grenade>();
            component2.id = PlayerObject.GetComponent<QueryProcessor>().PlayerId + ":" + (component1.smThrowInteger + 4096);
            GrenadeManager.grenadesOnScene.Add(component2);
            component2.SyncMovement(component1.availableGrenades[id].GetStartPos(PlayerObject), (PlayerObject.GetComponent<Scp049PlayerScript>().plyCam.transform.forward + Vector3.up / 4f).normalized * throwForce, Quaternion.Euler(component1.availableGrenades[id].startRotation), component1.availableGrenades[id].angularVelocity);
            component1.CallRpcThrowGrenade(id, PlayerObject.GetComponent<QueryProcessor>().PlayerId, component1.smThrowInteger++ + 4096, dir, true, pos, false, 0);
            
            return component1.availableGrenades[id].timeUnitilDetonation;
        }
    }
}
