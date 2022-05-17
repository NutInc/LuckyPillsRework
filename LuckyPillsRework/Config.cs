﻿using Exiled.API.Interfaces;
using Exiled.API.Enums;
using System.ComponentModel;
using System.Collections.Generic;

namespace LuckyPillsRework
{
    public class Config : IConfig
    {
        [Description("Should the plugin be loaded?")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should extra debug messages be output to the console?")]
        public bool DebugEnabled { get; set; } = false;

        public Dictionary<GrenadeType, float> VomitIntervals { get; set; } = new Dictionary<GrenadeType, float>()
        {
            { GrenadeType.Scp018, 5 },
            { GrenadeType.FragGrenade, 0.1f },
            { GrenadeType.Flashbang, 0.1f },
        };

        [Description("The amount of damage dealt to the player with the specified vomit effect.")]
        public Dictionary<GrenadeType, int> VomitDamage { get; set; } = new Dictionary<GrenadeType, int>()
        {
            { GrenadeType.Scp018, 1 },
            { GrenadeType.Flashbang, 1 },
            { GrenadeType.FragGrenade, 0 },
        };

        [Description("Message to be sent to the player when picking up pills.")]
        public string PickupMessage = "You have picked up SCP-5854.";

        [Description("The list of pill effects that are enabled. (See https://github.com/NutInc/LuckyPills for a list of effects).")]
        public List<string> EnabledEffects { get; set; } = new List<string>()
        {
            "explode",
            "mutate",
            "god",
            "paper",
            "upsidedown",
            "flattened",
            "bombvomit",
            "flashvomit",
            "scp268",
            "amnesia",
            "bleeding",
            "corroding",
            "decontaminating",
            "hemorrhage",
            "panic",
            "sinkhole"
        };

        public float MinEffectDuration { get; set; } = 5f;
        public float MaxEffectDuration { get; set; } = 30f;
    };
}
