using PheggMod.API.Events;

namespace ExamplePlugin
{
    internal class Playerhurt : IEventHandlerPlayerHurt, IEventHandlerAdminQuery
    {
        private ExamplePlugin examplePlugin;
        public Playerhurt(ExamplePlugin examplePlugin) => this.examplePlugin = examplePlugin;

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            examplePlugin.Info($"{ev.Player.name} was injured {ev.DamageType.name}");
        }

        public void OnAdminQuery(AdminQueryEvent ev)
        {
            examplePlugin.Info($"{ev.Admin.name} ran command {ev.Query}");
        }
    }
}