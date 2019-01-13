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
        private readonly HmdPlugin plugin;

        public EventHandlers(HmdPlugin plugin)
        {
            this.plugin = plugin;
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            plugin.ReloadConfig();

            foreach (int role in plugin.RoleSpawns)
            {
                Vector vector = ev.Server.Map.GetRandomSpawnPoint((Role)role);

                plugin.Handler.CreateOfType(new Vector3(
                        vector.x,
                        vector.y,
                        vector.z),
                    Quaternion.Euler(0, 0, 0));
            }

            Pickup[] pickups = Object.FindObjectsOfType<Pickup>();
            foreach (int item in plugin.ItemSpawns)
            {
                foreach (Vector3 pos in pickups.Where(x => x.info.itemId == item).Select(x => x.transform.position + Vector3.up))
                {
                    plugin.Handler.CreateOfType(pos, Quaternion.Euler(0, 0, 0));
                }
            }
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
            Timing.InTicks(() =>
            {
                List<Player> filteredPossibleSnipers = ev.SpawnChaos ? 
                    ev.PlayerList :
                    ev.PlayerList.Where(x => plugin.MtfHmdRoles.Contains((int)x.TeamRole.Role)).ToList();

                if (filteredPossibleSnipers.Count > 0)
                {
                    DistributeHmd(filteredPossibleSnipers, ev.SpawnChaos ? plugin.ChaosHmds : plugin.MtfHmds);
                }
            }, 2);
        }

        private void DistributeHmd(IReadOnlyCollection<Player> players, int count)
        {
            List<Player> possibleSnipers = players.ToList();
            for (int i = 0; i < count; i++)
            {
                Player sniper = possibleSnipers[Random.Range(0, possibleSnipers.Count)];
                possibleSnipers.Remove(sniper);
                plugin.Handler.Create(((GameObject) sniper.GetGameObject()).GetComponent<Inventory>());

                if (possibleSnipers.Count == 0)
                {
                    possibleSnipers = players.ToList();
                }
            }
        }
    }
}
