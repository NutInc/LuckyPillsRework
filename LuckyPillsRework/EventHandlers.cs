using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Enums;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using CustomPlayerEffects;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;

namespace LuckyPillsRework
{
    public static class EventHandlers
    {
        private static readonly Config Config = LuckyPills.Singleton.Config;

        // Picking up item
        public static void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            var player = ev.Player;
            var pickupType = ev.Pickup.Type;

            if (pickupType != ItemType.Painkillers) return;
            Log.Debug("Painkillers equipped!");
            player.ShowHint(LuckyPills.Singleton.Config.PickupMessage);
        }

        // Using Item stuff
        private static void SpawnGrenadeOnPlayer(Player player, ProjectileType grenadeType, float velocity = 1f)
        {
            var fullForce = velocity > 1;
            player.ThrowGrenade(grenadeType, fullForce);
        }

        private static IEnumerator<float> VomitItem(Player player, ProjectileType grenadeType, float randomTimer)
        {
            for (var i = 0; i < randomTimer * 10.0 && player.IsAlive; ++i)
            {
                yield return Timing.WaitForSeconds(Config.VomitIntervals[grenadeType]);
                player.Hurt(Config.VomitDamage[grenadeType]);

                SpawnGrenadeOnPlayer(player, grenadeType, 5f);
            }
        }
        public static void OnUsingItem(UsingItemEventArgs ev)
        {
            var itemType = ev.Item.Type;

            if (itemType == ItemType.Painkillers)
            {
                Timing.RunCoroutine(RunPillCoroutine(ev));
            }
        }

        private static string NextEffect() => Config.EnabledEffects[Random.Range(0, Config.EnabledEffects.Count)];

        private static IEnumerator<float> RunPillCoroutine(UsingItemEventArgs ev)
        {
            yield return Timing.WaitForSeconds(3f);

            Item item = ev.Item;
            var player = ev.Player;

            if (player.IsInPocketDimension) yield break;

            var effectType = NextEffect();
            var duration = Mathf.Ceil(Random.Range(Config.MinEffectDuration, Config.MaxEffectDuration));

            player.RemoveItem(item);
            player.EnableEffect(effectType, duration, true);

            switch (effectType)
            {
                case "amnesia":
                    player.EnableEffect<AmnesiaVision>(duration);
                    player.ShowHint($"You've been given amnesia for {duration} seconds");

                    break;
                case "bleeding":
                    player.EnableEffect<Bleeding>(duration);
                    player.ShowHint($"You've been given bleeding for {duration} seconds");

                    break;
                case "bombvomit":
                    Timing.RunCoroutine(VomitItem(player, ProjectileType.FragGrenade, duration));
                    player.ShowHint($"You've been given bomb vomit for {duration} seconds");

                    break;
                case "ballvomit":
                    Timing.RunCoroutine(VomitItem(player, ProjectileType.Scp018, duration));
                    player.ShowHint($"You've been given ball vomit for {duration} seconds");

                    break;
                case "corroding":
                    player.EnableEffect<Corroding>(duration);
                    player.ShowHint("You've been sent to the pocket dimension");

                    break;
                case "decontaminating":
                    player.EnableEffect<Decontaminating>(duration);
                    player.ShowHint($"You've been given decontamination for {duration} seconds");

                    break;
                case "explode":
                    ExplosiveGrenade explosiveGrenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);

                    explosiveGrenade.FuseTime = .5f;
                    explosiveGrenade.SpawnActive(ev.Player.Position);

                    if (player.IsAlive)
                        player.Kill(DamageType.Explosion);

                    player.ShowHint("You've been exploded");

                    break;
                case "flashed":
                    var flashGrenade = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                    flashGrenade.FuseTime = .5f;
                    flashGrenade.SpawnActive(ev.Player.Position);

                    player.ShowHint("You've been flashed");

                    break;
                case "flashvomit":
                    Timing.RunCoroutine(VomitItem(player, ProjectileType.Flashbang, duration));
                    player.ShowHint($"You've been given flash vomit for {duration} seconds");

                    break;
                case "god":
                    player.IsGodModeEnabled = true;
                    Timing.CallDelayed(duration, () => player.IsGodModeEnabled = false);
                    player.ShowHint($"You've been given god mode for {duration} seconds");

                    break;
                case "hemorrhage":
                    player.EnableEffect<Hemorrhage>(duration);
                    player.ShowHint($"You've been hemorrhaged for {duration} seconds");

                    break;
                case "mutate":
                    Exiled.API.Features.Roles.Role cachedMutatorRole = player.Role;

                    player.DropItems();
                    player.Role.Set(RoleTypeId.Scp0492);

                    Timing.CallDelayed(duration, () => player.Role.Set(cachedMutatorRole));

                    player.ShowHint($"You've been mutated for {duration} seconds");

                    break;
                case "paper":
                    player.Scale = new Vector3(1f, 1f, 0.01f);
                    Timing.CallDelayed(duration, () => player.Scale = new Vector3(1f, 1f, 1f));
                    player.ShowHint($"You've been turned into paper for {duration} seconds");

                    break;
                case "sinkhole":
                    player.EnableEffect<Sinkhole>(duration);
                    player.ShowHint($"You've been given sinkhole effect for {duration} seconds");

                    break;
                case "scp268":
                    player.EnableEffect<Invisible>();
                    Timing.CallDelayed(duration, () => player.DisableEffect<Invisible>());

                    player.ShowHint($"You've been turned invisible for {duration} seconds");

                    break;
                case "upsidedown":
                    player.Scale = new Vector3(1f, -1f, 1f);
                    Timing.CallDelayed(duration, () =>
                    {
                        player.Scale = new Vector3(1f, 1f, 1f);
                        player.Position += new Vector3(0, 1, 0);
                    });

                    player.ShowHint($"You've been converted to Australian for {duration} seconds");

                    break;
                default:
                    player.ShowHint($"You've been {effectType} for {duration} seconds");
                    break;
            }
        }
    }
}
