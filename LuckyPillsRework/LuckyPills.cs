using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace LuckyPillsRework
{
    public class LuckyPills : Plugin<Config>
    {
        public static LuckyPills Singleton;
        public override string Author { get; } = "imskyyc @ Nut Inc";
        public override string Name { get; } = "LuckyPillsRework";
        public override Version Version { get; } = new Version(3, 4, 0);
        public override Version RequiredExiledVersion { get; } = new Version(6, 0, 0);
        public override PluginPriority Priority => PluginPriority.Low;

        public override void OnEnabled()
        {
            Singleton = this;

            Log.Debug("Registering events");
            PlayerEvent.UsingItem += EventHandlers.OnUsingItem;
            PlayerEvent.PickingUpItem += EventHandlers.OnPickingUpItem;

            Log.Debug("Base onEnabled");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            PlayerEvent.UsingItem -= EventHandlers.OnUsingItem;
            PlayerEvent.PickingUpItem -= EventHandlers.OnPickingUpItem;

            base.OnDisabled();
        }
    }
}
