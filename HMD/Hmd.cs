﻿using System.Linq;
using ItemManager;
using ItemManager.Events;
using RemoteAdmin;
using scp4aiur;
using Smod2;
using Smod2.API;
using UnityEngine;
using UnityEngine.Networking;

namespace HMD
{
    public class Hmd : CustomWeapon, IDoubleDroppable
    {
        private const int WorldMask = 1207976449;
        private const int PlayerMask = 1208246273;

        private bool superCharged;

        private const float ShotDamage = 90f;
        private const int Krakatoa = 15;
        private const int SuppressedKrakatoa = 7;
        private const float SuperChargeRadius = 15f;
        private const float SuperChargeDamage = 30f;

        public override ItemType DefaultItemId => ItemType.E11_STANDARD_RIFLE;
        public override int MagazineCapacity => 5;
        public override float FireRate => 5f;

        public bool OnDoubleDrop()
        {
            superCharged = !superCharged;

            Sight = superCharged ? 3 : 4;

            return false;
        }

        public float DoubleDropWindow => 0.25f;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Sight = 4;
            Barrel = 3;
            MiscAttachment = 3;
        }

        private void TargetShake(GameObject target)
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

        protected override void OnValidShoot(GameObject target, ref float damage)
        {
            WeaponManager weps = PlayerObject.GetComponent<WeaponManager>();
            if (superCharged)
            {
                Transform cam = PlayerObject.GetComponent<Scp049PlayerScript>().plyCam.transform;
                Ray ray = new Ray(cam.position, cam.forward);
                Physics.Raycast(cam.position + cam.forward, cam.forward, out RaycastHit playerHit, PlayerMask);

                if (playerHit.collider.GetComponent<HitboxIdentity>() == null && Physics.Raycast(ray, out RaycastHit hit, 500f, WorldMask))
                {
                    Timing.In(x =>
                    {
                        foreach (GameObject player in  PlayerManager.singleton.players.Except(new[] {PlayerObject})
                            .Where(y => Vector3.Distance(y.GetComponent<PlyMovementSync>().position, hit.point) < SuperChargeRadius &&
                                        weps.GetShootPermission(y.GetComponent<CharacterClassManager>())))
                        {
                            player.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(
                                SuperChargeDamage,
                                PlayerObject.GetComponent<NicknameSync>().myNick + " (" +
                                PlayerObject.GetComponent<CharacterClassManager>().SteamId + ")",
                                DamageTypes.Tesla,
                                PlayerObject.GetComponent<QueryProcessor>().PlayerId
                            ), player);
                            
                            TargetShake(player);
                        }
                    }, DetonateFlash(hit.point));
                }
            }
            else
            {
                damage = ShotDamage;
            }

            
            int shots = Barrel == 1 ? SuppressedKrakatoa : Krakatoa;
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
