using scp4aiur;
using Smod2.EventHandlers;
using Smod2.Events;
using Smod2.EventSystem.Events;
using System.Collections.Generic;
using System.Linq;
using ItemManager;
using Smod2.API;
using UnityEngine;

namespace HMD
{
    public class EventHandlers : IEventHandlerRoundStart, IEventHandlerTeamRespawn
    {
        public void OnRoundStart(RoundStartEvent ev)
        {
            Plugin.ReloadConfig();

            foreach (int role in Plugin.roleSpawns)
            {
                Vector vector = ev.Server.Map.GetRandomSpawnPoint((Role)role);

                Items.CreateItem(Plugin.HmdId, new Vector3(
                        vector.x,
                        vector.y,
                        vector.z),
                    Quaternion.Euler(0, 0, 0));
            }

            Pickup[] pickups = Object.FindObjectsOfType<Pickup>();
            foreach (int item in Plugin.itemSpawns)
            {
                foreach (Vector3 pos in pickups.Where(x => x.info.itemId == item).Select(x => x.transform.position))
                {
                    Items.CreateItem(Plugin.HmdId, pos, Quaternion.Euler(0, 0, 0));
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            Timing.InTicks(() =>
            {
                List<Player> filteredPossibleSnipers = ev.SpawnChaos ? 
                    ev.PlayerList :
                    ev.PlayerList.Where(x => Plugin.mtfHmdRoles.Contains((int)x.TeamRole.Role)).ToList();

                if (filteredPossibleSnipers.Count > 0)
                {
                    DistributeHmd(filteredPossibleSnipers, ev.SpawnChaos ? Plugin.chaosHmds : Plugin.mtfHmds);
                }
            }, 2);
        }

        private static void DistributeHmd(IReadOnlyCollection<Player> players, int count)
        {
            List<Player> possibleSnipers = players.ToList();
            for (int i = 0; i < count; i++)
            {
                Player sniper = possibleSnipers[Random.Range(0, possibleSnipers.Count)];
                possibleSnipers.Remove(sniper);
                sniper.GiveItem(Plugin.HmdId);

                if (possibleSnipers.Count == 0)
                {
                    possibleSnipers = players.ToList();
                }
            }
        }
    }
}
