using Exiled.Events.EventArgs;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.API.Enums;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using CustomPlayerEffects;

namespace LuckyPillsRework
{
    public static class EventHandlers
    {

        static Config config = LuckyPills.Singleton.Config;

        // Picking up item
        public static void OnPickingUpItem(PickingUpItemEventArgs ev)
        {
            Player player = ev.Player;
            ItemType pickupType = ev.Pickup.Type;

            if (pickupType == ItemType.Painkillers)
            {
                LuckyPills.DebugOutput($"Painkillers equipped!");
                player.ShowHint(LuckyPills.Singleton.Config.PickupMessage);
            }
        }

        // Using Item stuff
        static void SpawnGrenadeOnPlayer(Player player, GrenadeType grenadeType, float velocity = 1f)
        {
            bool fullForce = velocity > 1;
            player.ThrowGrenade(grenadeType, fullForce);
        }

        static IEnumerator<float> VomitItem(Player player, GrenadeType grenadeType, float randomTimer)
        {
            for (int i = 0; i < randomTimer * 10.0 && player.IsAlive; ++i)
            {
                yield return Timing.WaitForSeconds(config.VomitIntervals[grenadeType]);
                player.Hurt(config.VomitDamage[grenadeType]);

                SpawnGrenadeOnPlayer(player, grenadeType, 5f);
            }
        }
        public static void OnUsingItem(UsingItemEventArgs ev)
        {
            Player player = ev.Player;
            ItemType itemType = ev.Item.Type;

            if (itemType == ItemType.Painkillers)
            {
                Timing.RunCoroutine(RunPillCoroutine(ev));
            }
        }

        static string NextEffect() => config.EnabledEffects[Random.Range(0, config.EnabledEffects.Count)];
        static IEnumerator<float> RunPillCoroutine(UsingItemEventArgs ev)
        {
            yield return Timing.WaitForSeconds(3f);

            Item item = ev.Item;
            Player player = ev.Player;

            if (player.IsInPocketDimension) yield break;

            string effectType = NextEffect();
            float duration = Mathf.Ceil(Random.Range(config.MinEffectDuration, config.MaxEffectDuration));

            player.RemoveItem(item);
            player.EnableEffect(effectType, duration, true);

            switch (effectType)
            {
                case "amnesia":
                    player.EnableEffect<Amnesia>(duration);
                    player.ShowHint($"You've been given amnesia for {duration} seconds");

                    break;
                case "bleeding":
                    player.EnableEffect<Bleeding>(duration);
                    player.ShowHint($"You've been given bleeding for {duration} seconds");

                    break;
                case "bombvomit":
                    Timing.RunCoroutine(VomitItem(player, GrenadeType.FragGrenade, duration));
                    player.ShowHint($"You've been given bomb vomit for {duration} seconds");

                    break;
                case "ballvomit":
                    Timing.RunCoroutine(VomitItem(player, GrenadeType.Scp018, duration));
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
                    FlashGrenade flashGrenade = (FlashGrenade)Item.Create(ItemType.GrenadeFlash);
                    flashGrenade.FuseTime = .5f;
                    flashGrenade.SpawnActive(ev.Player.Position);

                    player.ShowHint("You've been flashed");

                    break;
                case "flashvomit":
                    Timing.RunCoroutine(VomitItem(player, GrenadeType.Flashbang, duration));
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
                    player.SetRole(RoleType.Scp0492, SpawnReason.ForceClass, true);

                    Timing.CallDelayed(duration, () => player.SetRole(cachedMutatorRole, SpawnReason.ForceClass, true));

                    player.ShowHint($"You've been mutated for {duration} seconds");

                    break;
                case "paper":
                    player.Scale = new Vector3(1f, 1f, 0.01f);
                    Timing.CallDelayed(duration, () => player.Scale = new Vector3(1f, 1f, 1f));
                    player.ShowHint($"You've been turned into paper for {duration} seconds");

                    break;
                case "sinkhole":
                    player.EnableEffect<SinkHole>(duration);
                    player.ShowHint($"You've been given sinkhole effect for {duration} seconds");

                    break;
                case "scp268":
                    player.IsInvisible = true;
                    Timing.CallDelayed(duration, () => player.IsInvisible = false);

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
