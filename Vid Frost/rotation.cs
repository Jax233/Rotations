using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API;

namespace AimsharpWow.Modules {
    public class VidFrostDeathknight : Rotation {

        public override void LoadSettings() {
            List < string > MajorAzeritePower = new List < string > (new string[] {
                "Guardian of Azeroth",
                "Focused Azerite Beam",
                "Concentrated Flame",
                "Worldvein Resonance",
                "Memory of Lucid Dreams",
                "Blood of the Enemy",
                "The Unbound Force",
                "Reaping Flames",
                "None"
            });
            
            List < string > Trinkets = new List < string > (new string[] {
                "Azshara's Font of Power",
                "Ashvane's Razor Coral",
                "Pocket-Sized Computation Device",
                "Galecaller's Boon",
                "Shiver Venom Relic",
                "Lurker's Insidious Gift",
                "Notorious Gladiator's Badge",
                "Sinister Gladiator's Badge",
                "Sinister Gladiator's Medallion",
                "Notorious Gladiator's Medallion",
                "Vial of Animated Blood",
                "First Mate's Spyglass",
                "Jes' Howler",
                "Ashvane's Razor Coral",
                "Generic",
                "None"
            });
            
            Settings.Add(new Setting("First 5 Letters of the Addon:", "xxxxx"));
            
            List < string > Race = new List < string > (new string[] {
                "Orc",
                "Troll",
                "Dark Iron Dwarf",
                "Mag'har Orc",
                "Lightforged Draenei",
                "None"
            });
            
            Settings.Add(new Setting("Racial Power", Race, "None"));
            Settings.Add(new Setting("Healthstone @ HP%", 0, 100, 20));
            Aimsharp.PrintMessage("Default mode: PVE, AOE ON, USE CDs/Pots", Color.Yellow);
        }

        private string RacialPower;
        private string FiveLetters;
        private string MajorPower;

        public override void Initialize() {
            Aimsharp.PrintMessage("Vid Frost Deathknight 1.00", Color.Yellow);
            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Yellow);
            Aimsharp.PrintMessage("/xxxxx Potions --Toggles using buff potions on/off.", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns --Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx noaoe --Toggles to turn off PVE AOE on/off.", Color.Orange);
            Aimsharp.PrintMessage("/xxxxx savepp -- Toggles the use of prepull", Color.Orange);
            Aimsharp.PrintMessage("/xxxxx StormBolt --Queues Storm Bolt up to be used on the next GCD.", Color.Red);
            Aimsharp.PrintMessage("/xxxxx RallyingCry --Queues Rallying Cry up to be used on the next GCD.", Color.Red);
            Aimsharp.PrintMessage("/xxxxx IntimidatingShout --Queues Intimidating SHout up to be used on the next GCD.", Color.Red);
            Aimsharp.PrintMessage("/xxxxx Bladestorm --Queues Bladestorm up to be used on the next GCD.", Color.Red);
            Aimsharp.PrintMessage("/xxxxx pvp --Toggles PVP Mode.", Color.Red);
            Aimsharp.PrintMessage("/xxxxx Burst --Toggles Burst for pvp on.", Color.Red);
            
            Aimsharp.Latency = 100;
            Aimsharp.QuickDelay = 125;

            MajorPower = GetDropDown("Major Power");
            RacialPower = GetDropDown("Racial Power");
            FiveLetters = GetString("First 5 Letters of the Addon:");
            
            #region Spells
            
            if (RacialPower == "Orc") Spellbook.Add("Blood Fury");
            if (RacialPower == "Troll") Spellbook.Add("Berserking");
            if (RacialPower == "Dark Iron Dwarf") Spellbook.Add("Fireblood");
            if (RacialPower == "Mag'har Orc") Spellbook.Add("Ancestral Call");
            if (RacialPower == "Lightforged Draenei") Spellbook.Add("Light's Judgment");

            Spellbook.Add("Howling BLast");
            Spellbook.Add("Obliterate");
            Spellbook.Add("Frost Strike");
            Spellbook.Add("Remorseless Winter");
            Spellbook.Add("Anti-Magic Shell");
            Spellbook.Add("Breath of Sindragosa");
            Spellbook.Add("Glacial Advance");

            #endregion

            #region Buffs/Procs

            Buffs.Add("Icy Talons");
            Buffs.Add("Pillar of Frost");
            Buffs.Add("Breath of Sindragosa");

            #endregion

            #region Debuffs

            Debuffs.Add("Frost Fever");

            #endregion
        }

        public override bool CombatTick() {
            #region MiscSetup
            
            bool Fighting = Aimsharp.InCombat("target") && Aimsharp.InCombat("player");
            bool Moving = Aimsharp.PlayerIsMoving();
            float Haste = Aimsharp.Haste();
            int Range = Aimsharp.Range("target");
            int TargetHealth = Aimsharp.Health("target");
            int PlayerHealth = Aimsharp.Health("player");
            int Runes = Aimsharp.PlayerSecondaryPower();
            int RunicPower = Aimsharp.Power("player");
            string LastCast = Aimsharp.LastCast();
            bool IsChanneling = Aimsharp.IsChanneling("player");
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            int GCD = Aimsharp.GCD();
            bool HasLust = Aimsharp.HasBuff("Bloodlust", "player", false) || Aimsharp.HasBuff("Heroism", "player", false) || Aimsharp.HasBuff("Time Warp", "player", false) || Aimsharp.HasBuff("Ancient Hysteria", "player", false) || Aimsharp.HasBuff("Netherwinds", "player", false) || Aimsharp.HasBuff("Drums of Rage", "player", false);
           
            #endregion

            #region CommandsSetup

            bool NoCooldown = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            bool OffAOE = Aimsharp.IsCustomCodeOn("noaoe");
            bool PVPmode = Aimsharp.IsCustomCodeOn("pvp");
            bool StormBolt = Aimsharp.IsCustomCodeOn("StormBolt");
            bool RallyingCry = Aimsharp.IsCustomCodeOn("RallyingCry");
            bool IntimidatingShout = Aimsharp.IsCustomCodeOn("IntimidatingShout");
            bool Bladestorm = Aimsharp.IsCustomCodeOn("Bladestorm");


            #endregion

            #region Cooldowns

            int CooldownBoSRemains = Aimsharp.SpellCooldown("Breath of Sindragosa") - GCD;

            #endregion

            #region Durations

            int FrostFeverRemains = Aimsharp.DebuffRemaining("Frost Fever", "target");
            bool DebuffFrostFeverUp = FrostFeverRemains > 0;

            int BuffIcyTalonsRemains = Aimsharp.BuffRemaining("Icy Talons");
            bool BuffIcyTalonsUp = BuffIcyTalonsRemains > 0;

            int BuffPillarOfFrostRemains = Aimsharp.BuffRemaining("Pillar of Frost");
            bool BuffPillarOfFrostUp = BuffPillarOfFrostRemains > 0;

            int BuffBreathOfSindragosaRemains = Aimsharp.BuffRemaining("Breath of Sindragosa");
            bool BuffBreathOfSindragosaUp = BuffBreathOfSindragosaRemains > 0;
            

            #endregion

            #region CanCasts

            bool CanCastHowlingBlast = Aimsharp.TimeUntilRunes(1) < GCD;
            bool CanCastGlacialAdvance = Aimsharp.SpellCooldown("Glacial Advance") < GCD && RunicPower >=30;
            bool CanCastFrostStrike = RunicPower >= 25;

            #endregion

            #region Rotation

            if (IsChanneling) return false;

            #region EveryTimeActorIsAvailable

            if (!DebuffFrostFeverUp && (!TalentBoSEnabled || CooldownBoSRemains > 15000) && CanCastHowlingBlast) {
                Aimsharp.Cast("HowlingBlast");
                return true;
            }

            if (BuffIcyTalonsRemains <= GCD && BuffIcyTalonsUp && EnemiesInMelee >= 2 &&
                (!TalentBoSEnabled || CooldownBoSRemains > 15000) && CanCastGlacialAdvance) {
                Aimsharp.Cast("Glacial Advance");
                return true;
            }

            if (BuffIcyTalonsRemains <= GCD && BuffIcyTalonsUp && (!TalentBoSEnabled || CooldownBoSRemains > 15000) &&
                CanCastFrostStrike) {
                Aimsharp.Cast("Frost Strike");
                return true;
            }

            #region Essences

            if (MajorPower == "Blood of the Enemy" && EnemiesInMelee > 0) {
                if (BuffPillarOfFrostUp &&
                    (BuffPillarOfFrostRemains < 10000 &&
                     (BuffBreathOfSindragosaUp || !TalentBoSEnabled && !AzeriteIcyCitadelEnabled) ||
                     BuffIcyCitadelUp && TalentIcecapEnabled) && (EnemiesInMelee == 1 || !TalentIcecapEnabled) ||
                    EnemiesInMelee >= 2 && TalentIcecapEnabled && CooldownPillarOfFrostReady &&
                    (AzeriteIcyCitadelRank >= 1 && BuffIcyCitadelUp || !AzeriteIcyCitadelEnabled) &&
                    Aimsharp.CanCast("Blood of the Enemy")) {
                    Aimsharp.Cast("Blood of the Enemy");
                    return true;
                }
            }

            if (MajorPower == "Guardian of Azeroth") {
                if (!TalentIcecapEnabled ||
                    TalentIcecapEnabled && AzeriteIcyCitadelEnabled && BuffPillarOfFrostRemains < 6000 &&
                    BuffPillarOfFrostUp || TalentIcecapEnabled && !AzeriteIcyCitadelEnabled &&
                    Aimsharp.CanCast("Guardian of Azeroth")) {
                    Aimsharp.Cast("Guardian of Azeroth");
                    return true;
                }
            }
            
            

            #endregion

            #endregion

            #endregion





        }

    }
}