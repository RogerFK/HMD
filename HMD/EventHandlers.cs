using System.Linq;
using Smod2.EventHandlers;
using Smod2.Events;
using UnityEngine;

namespace HMD
{
    public class EventHandlers : IEventHandlerRoundStart, IEventHandlerElevatorUse, IEventHandlerWarheadStartCountdown, IEventHandlerWarheadStopCountdown
    {
        public void OnRoundStart(RoundStartEvent ev)
        {
            Plugin.lockElevator = new int[ev.Server.Map.GetElevators().Count];
            Plugin.lockDoor = Object.FindObjectsOfType<Door>().ToDictionary(x => x, x => 0);
            Plugin.lockDoorStatus = Plugin.lockDoor.ToDictionary(x => x.Key, x => false);
        }

        public void OnElevatorUse(PlayerElevatorUseEvent ev)
        {
            if (Plugin.lockElevator[(int) ev.Elevator.ElevatorType] > 0)
            {
                ev.AllowUse = false;
            }
        }

        public void OnStartCountdown(WarheadStartEvent ev)
        {
            if (Plugin.lockWarhead > 0)
            {
                ev.Cancel = true;
            }
        }

        public void OnStopCountdown(WarheadStopEvent ev)
        {
            if (Plugin.lockWarhead > 0)
            {
                ev.Cancel = true;
            }
        }
    }
}
