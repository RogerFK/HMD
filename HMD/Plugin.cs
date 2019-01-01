using scp4aiur;

namespace HMD
{
    public class Plugin : Smod2.Plugin
    {
        public override void Register()
        {
            Timing.Init(this);

            AddEventHandlers(new EventHandlers());
        }

        public override void OnEnable() { }
        public override void OnDisable() { }
    }
}
