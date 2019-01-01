using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ItemManager;
using ItemManager.Events;
using RemoteAdmin;
using scp4aiur;
using ServerMod2.API;
using Smod2.API;
using UnityEngine;

namespace HMD
{
    public class Hmd : CustomWeapon, IDoubleDroppable
    {
        // Found in PlayerInteract
        private const int InteractMask = 0;
        private const float InteractDistance = 0;

        private const float shotDamage = 90f;
        private const int krakatoa = 15;
        private const int suppressedKrakatoa = 7;
        private const float superChargeRadius = 10f;
        private const bool friendlyFire = true;

        private const float scElevatorLock = 5f;
        private const float scWarheadLock = 5f;
        private const float scDoorLock = 5f;

        private bool superCharged;

        public override ItemType DefaultItemId => ItemType.E11_STANDARD_RIFLE;
        public override int MagazineCapacity => 5;
        public override float FireRate => 5f;
        public float DoubleDropWindow => 0.25f;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Sight = 4;
            Barrel = 3;
            MiscAttachment = 3;
        }

        public override void OnShoot(GameObject target, ref float damage)
        {
            Sight = 0;
            Timing.In(x => Sight = 4, FireRate);

            if (superCharged)
            {
                superCharged = false;
                damage = 0;
                MagazineAmmo = 0;

                Ray ray = new Ray(PlayerObject.GetComponent<Scp049PlayerScript>().plyCam.transform.position, PlayerObject.GetComponent<Scp049PlayerScript>().plyCam.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 500f, InteractMask))
                {
                    Vector3[] superChargeTargets = null;

                    Door door = hit.transform.GetComponentInParent<Door>();
                    if (door != null)
                    {
                        if (Plugin.lockDoor[door]++ == 0)
                        {
                            Plugin.lockDoorStatus[door] = door.isOpen;
                        }

                        door.NetworkisOpen = true;
                        door.Networklocked = true;

                        Timing.In(x =>
                        {
                            if (--Plugin.lockDoor[door] == 0)
                            {
                                door.Networklocked = false;
                                door.NetworkisOpen = Plugin.lockDoorStatus[door];
                            }
                        }, scDoorLock);

                        superChargeTargets = door.buttons.Select(x => x.transform.position).ToArray();
                    }
                    else if (hit.transform.CompareTag("ElevatorButton"))
                    {
                        Vector3 pos = PlayerObject.GetComponent<PlyMovementSync>().position;

                        Lift lift = UnityEngine.Object.FindObjectsOfType<Lift>().FirstOrDefault(x => x.elevators.Any(y => Vector3.Distance(pos, y.door.transform.position) < InteractDistance * 1.5));
                        int type = (int)new SmodElevator(lift).ElevatorType;
                        Plugin.lockElevator[type]++;
                        Timing.In(x => Plugin.lockElevator[type]--, scElevatorLock);

                        superChargeTargets = lift?.elevators.Select(x => x.door.transform.position).ToArray();
                    }
                    else if (hit.transform.CompareTag("AW_Panel") || hit.transform.CompareTag("AW_Detonation"))
                    {
                        Plugin.lockWarhead++;
                        Timing.In(x => Plugin.lockWarhead--, scWarheadLock);

                        superChargeTargets = new[]
                        {
                            AlphaWarheadOutsitePanel.nukeside.transform.position,
                            GameObject.Find("OutsitePanelScript").transform.position
                        };
                    }

                    if (superChargeTargets != null)
                    {
                        CharacterClassManager ccm = PlayerObject.GetComponent<CharacterClassManager>();
                        Team team = ccm.klasy[ccm.curClass].team;

                        foreach (Vector3 superChargeTarget in superChargeTargets)
                        {
                            foreach (KeyValuePair<GameObject, float> player in PlayerManager.singleton.players
                                .Except(new[] { PlayerObject })
                                .ToDictionary(x => x, x =>
                                {
                                    float dist = Vector3.Distance(x.transform.position, superChargeTarget);
                                    if (dist < superChargeRadius)
                                    {
                                        return 1 / Mathf.InverseLerp(0, superChargeRadius, dist);
                                    }

                                    return 0f;
                                })
                                .Where(x =>
                                {
                                    CharacterClassManager xCcm = x.Key.GetComponent<CharacterClassManager>();
                                    return x.Value != 0f && (xCcm.klasy[xCcm.curClass].team != team || friendlyFire);
                                }))
                            {
                                player.Key.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(
                                    shotDamage * player.Value,
                                    PlayerObject.GetComponent<NicknameSync>().myNick + " (" +
                                    PlayerObject.GetComponent<CharacterClassManager>().SteamId + ")",
                                    DamageTypes.Tesla,
                                    PlayerObject.GetComponent<QueryProcessor>().PlayerId
                                ), player.Key);
                            }
                        }
                    }
                }
            }
            else
            {
                damage = shotDamage;
            }

            WeaponManager weps = PlayerObject.GetComponent<WeaponManager>();
            int shots = Barrel == 1 ? suppressedKrakatoa : krakatoa;
            for (int i = 0; i < shots; i++)
            {
                weps.CallRpcConfirmShot(false, weps.curWeapon);
            }

        }

        public bool OnDoubleDrop()
        {
            if (superCharged)
            {
                superCharged = false;
                Sight = 4;
            }
            else if (Durability == (PlayerObject.GetComponent<WeaponManager>().weapons.FirstOrDefault(x => x.inventoryID == (int)Type)?.maxAmmo ?? -1))
            {
                superCharged = true;
                Sight = 3;
            }

            return false;
        }
    }
}
