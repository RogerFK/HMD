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
        }

        public void OnTeamRespawn(TeamRespawnEvent ev)
        {
                Timing.Next(() =>
                {
                    List<Player> filteredPossibleSnipers = ev.SpawnChaos ? 
                        ev.PlayerList :
                        ev.PlayerList.Where(x => Plugin.mtfHmdRoles.Contains((int)x.TeamRole.Role)).ToList();

                    if (filteredPossibleSnipers.Count > 0)
                    {
                        DistributeHmd(filteredPossibleSnipers, ev.SpawnChaos ? Plugin.chaosHmds : Plugin.mtfHmds);
                    }
                });
        }

        private static void DistributeHmd(IReadOnlyCollection<Player> players, int count)
        {
            List<Player> possibleSnipers = players.ToList();
            for (int i = 0; i < count; i++)
            {
                Player sniper = possibleSnipers[Random.Range(0, possibleSnipers.Count)];
                possibleSnipers.Remove(sniper);
                sniper.GiveItem(102);

                if (possibleSnipers.Count == 0)
                {
                    possibleSnipers = players.ToList();
                }
            }
        }
    }
}
