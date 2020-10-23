#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it
using CustomPlayerEffects;
using Mirror;
using MonoMod;

namespace PheggMod.CustomEffects
{
    [MonoModPatch("global::PlayerEffectsController")]
    class PMPlayerEffectsController : PlayerEffectsController
    {
        private ReferenceHub hub;

        private extern void orig_Awake();
#pragma warning disable IDE0051 // Remove unused private members
        private void Awake()
        {
            hub = ReferenceHub.GetHub(gameObject);

            AllEffects.Add(typeof(Amnesia), new Amnesia(hub));
            AllEffects.Add(typeof(ArtificialRegen), new ArtificialRegen(hub));
            AllEffects.Add(typeof(Asphyxiated), new Asphyxiated(hub));
            AllEffects.Add(typeof(Bleeding), new Bleeding(hub));
            AllEffects.Add(typeof(Blinded), new Blinded(hub));
            AllEffects.Add(typeof(Burned), new Burned(hub));
            AllEffects.Add(typeof(Concussed), new Concussed(hub));
            AllEffects.Add(typeof(Corroding), new Corroding(hub));
            AllEffects.Add(typeof(Deafened), new Deafened(hub));
            AllEffects.Add(typeof(Decontaminating), new Decontaminating(hub));
            AllEffects.Add(typeof(Disabled), new Disabled(hub));
            AllEffects.Add(typeof(Disarmed), new Disarmed(hub));
            AllEffects.Add(typeof(Discharge), new Discharge(hub));
            AllEffects.Add(typeof(Ensnared), new Ensnared(hub));
            AllEffects.Add(typeof(Exhausted), new Exhausted(hub));
            AllEffects.Add(typeof(Exsanguination), new Exsanguination(hub));
            AllEffects.Add(typeof(Flashed), new Flashed(hub));
            AllEffects.Add(typeof(Hemorrhage), new Hemorrhage(hub));
            AllEffects.Add(typeof(Invigorated), new Invigorated(hub));
            AllEffects.Add(typeof(Panic), new Panic(hub));
            AllEffects.Add(typeof(Poisoned), new Poisoned(hub));
            AllEffects.Add(typeof(Scp207), new Scp207(hub));
            AllEffects.Add(typeof(Scp268), new Scp268(hub));
            AllEffects.Add(typeof(SinkHole), new SinkHole(hub));
            AllEffects.Add(typeof(Visuals939), new Visuals939(hub));

            AllEffects.Add(typeof(SCP008), new SCP008(hub));

            if (NetworkServer.active) Resync();
        }
#pragma warning restore IDE0051 // Remove unused private members

        [Server]
        public new bool ChangeByString(string type, byte intens, float duration = 0f)
        {
            foreach (var item in AllEffects)
            {
                if (string.Equals(item.Key.ToString(), "customplayereffects." + type, System.StringComparison.InvariantCultureIgnoreCase) ||
                    string.Equals(item.Key.ToString(), "PheggMod.CustomEffects." + type, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    item.Value.ServerChangeIntensity(intens);
                    if (duration > 0)
                        item.Value.ServerChangeDuration(duration);
                    return true;
                }
            }

            return false;
        }
    }
}
