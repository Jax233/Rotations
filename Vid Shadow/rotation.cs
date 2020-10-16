using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{
    /// <summary>
    /// This is an example rotation. It is a garbage rotation.  Just trying to show some examples of using the Aimsharp API.
    /// Check API-DOC for detailed documentation.
    /// </summary>
    public class VidShadow : Rotation
    {


        public override void LoadSettings()
        {
            List<string> MajorAzeritePower = new List<string>(new string[] { "Guardian of Azeroth", "Focused Azerite Beam", "Concentrated Flame", "Worldvein Resonance", "Memory of Lucid Dreams", "Blood of the Enemy", "None" });
            Settings.Add(new Setting("Major Power", MajorAzeritePower, "None"));

            List<string> Trinkets = new List<string>(new string[] { "Azshara's Font of Power", "Shiver Venom Relic", "Generic", "None" });
            Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
            Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));

            List<string> Potions = new List<string>(new string[] { "Potion of Unbridled Fury", "Potion of Empowered Proximity", "Potion of Prolonged Power", "None" });
            Settings.Add(new Setting("Potion Type", Potions, "Potion of Unbridled Fury"));

            Settings.Add(new Setting("Chorus of Insanity Trait?", true));

            Settings.Add(new Setting("Searing Dialogue Trait?", false));
            
            Settings.Add(new Setting("Auto Shadow Mend Self @ HP%", 0, 100, 30));
            
            Settings.Add(new Setting("Auto Vampiric Embrace @ HP%", 0, 100, 45));
            

        }

        string MajorPower;
        string TopTrinket;
        string BotTrinket;
        public override void Initialize() {
            Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Vid Shadow - v 1.0", Color.Blue);
            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx NoAOE", Color.Blue);
            Aimsharp.PrintMessage("--Toggles AOE mode on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx Potions", Color.Blue);
            Aimsharp.PrintMessage("--Toggles using buff potions on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns", Color.Blue);
            Aimsharp.PrintMessage("--Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);


            Aimsharp.Latency = 50;
            Aimsharp.QuickDelay = 100;
            Aimsharp.SlowDelay = 125;

            MajorPower = GetDropDown("Major Power");
            TopTrinket = GetDropDown("Top Trinket");
            BotTrinket = GetDropDown("Bot Trinket");

            Spellbook.Add(MajorPower);
            Spellbook.Add("Void Eruption");
            Spellbook.Add("Dark Ascension");
            Spellbook.Add("Void Bolt");
            Spellbook.Add("Mind Sear");
            Spellbook.Add("Shadow Word: Death");
            Spellbook.Add("Surrender to Madness");
            Spellbook.Add("Dark Void");
            Spellbook.Add("Mindbender");
            Spellbook.Add("Shadowfiend");
            Spellbook.Add("Shadow Crash");
            Spellbook.Add("Mind Blast");
            Spellbook.Add("Void Torrent");
            Spellbook.Add("Shadow Word: Pain");
            Spellbook.Add("Vampiric Touch");
            Spellbook.Add("Mind Flay");
            Spellbook.Add("Shadowform");
            Spellbook.Add("Shadow Word: Void");
            Spellbook.Add("Devouring Plague");
            Spellbook.Add("Power Word: Fortitude");
            Spellbook.Add("Shadow Mend");
            Spellbook.Add("Vampiric Embrace");
            Spellbook.Add("Searing Nightmare");
            Spellbook.Add("Power Word: Shield");

            Buffs.Add("Bloodlust");
            Buffs.Add("Heroism");
            Buffs.Add("Time Warp");
            Buffs.Add("Ancient Hysteria");
            Buffs.Add("Netherwinds");
            Buffs.Add("Drums of Rage");
            Buffs.Add("Chorus of Insanity");
            Buffs.Add("Lifeblood");
            Buffs.Add("Harvested Thoughts");
            Buffs.Add("Voidform");
            Buffs.Add("Shadowform");
            Buffs.Add("Unfurling Darkness");
            Buffs.Add("Power Word: Fortitude");

            Debuffs.Add("Shadow Word: Pain");
            Debuffs.Add("Vampiric Touch");
            Debuffs.Add("Shiver Venom");
            Debuffs.Add("Unfurling Darkness");
            Debuffs.Add("Weakened Soul");

            Items.Add(TopTrinket);
            Items.Add(BotTrinket);
            Items.Add(GetDropDown("Potion Type"));

            Macros.Add(TopTrinket, "/use " + TopTrinket);
            Macros.Add(BotTrinket, "/use " + BotTrinket);
            Macros.Add("TopTrink", "/use 13");
            Macros.Add("BotTrink", "/use 14");
            Macros.Add("potion", "/use " + GetDropDown("Potion Type"));
            Macros.Add("crash cursor", "/cast [@cursor] Shadow Crash");

            CustomCommands.Add("NoAOE");
            CustomCommands.Add("Prepull");
            CustomCommands.Add("Potions");
            CustomCommands.Add("SaveCooldowns");
			

        }





        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick()
        {
            int GCD = Aimsharp.GCD();
            int Latency = Aimsharp.Latency;
            float Haste = Aimsharp.Haste() / 100f;
            int SWPRemains = Aimsharp.DebuffRemaining("Shadow Word: Pain", "target") - GCD;
            int SWPRemainsFocus = Aimsharp.DebuffRemaining("Shadow Word: Pain", "focus") - GCD;
            int SWPRemainsBoss1 = Aimsharp.DebuffRemaining("Shadow Word: Pain", "focus") - GCD;
            int VTRemains = Aimsharp.DebuffRemaining("Vampiric Touch", "target") - GCD;
            int DVPRemains = Aimsharp.DebuffRemaining("Devouring Plague", "target") - GCD;
            bool Fighting = Aimsharp.Range("target") <= 45 && Aimsharp.TargetIsEnemy();
            bool UsePotion = Aimsharp.IsCustomCodeOn("Potions");
            string PotionType = GetDropDown("Potion Type");
            bool HasLust = Aimsharp.HasBuff("Bloodlust", "player", false) || Aimsharp.HasBuff("Heroism", "player", false) || Aimsharp.HasBuff("Time Warp", "player", false) || Aimsharp.HasBuff("Ancient Hysteria", "player", false) || Aimsharp.HasBuff("Netherwinds", "player", false) || Aimsharp.HasBuff("Drums of Rage", "player", false);
            int TargetTimeToDie = 1000000000;
            int TargetHealth = Aimsharp.Health("target");
            int PlayerHealth = Aimsharp.Health("player");
            //int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            bool DotsUp = SWPRemains > 0 && VTRemains > 0;
            bool AllDotsUp = DotsUp && DVPRemains > 0;
            bool NoAOE = Aimsharp.IsCustomCodeOn("NoAOE");
            int Insanity = Aimsharp.Power("player");
            bool VoidformUp = Aimsharp.HasBuff("Voidform");
            bool NoCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            int VoidformStacks = Aimsharp.BuffStacks("Voidform");
            int GCDMAX = (int)((1500f / (Haste + 1f)) + GCD);
            float InsanityDrain = 6f + .68f * VoidformStacks;
            int MindBlastCastTime = (int)((1500f / (Haste + 1f)));
            int ChorusStacks = Aimsharp.BuffStacks("Chorus of Insanity");
            bool ChorusEnabled = GetCheckBox("Chorus of Insanity Trait?");
            bool IsMoving = Aimsharp.PlayerIsMoving();
            bool FontEquipped = Aimsharp.IsEquipped("Azshara's Font of Power");
            bool CanUseFont = Aimsharp.CanUseItem("Azshara's Font of Power");
            bool SearingDialogueEnabled = GetCheckBox("Searing Dialogue Trait?");
            bool IsChanneling = Aimsharp.IsChanneling("player");
            int PlayerCastingID = Aimsharp.CastingID("player");
            bool TalentSearingNightmareEnabled = Aimsharp.Talent(3, 3);
            bool SearingNightmaresCutoff = EnemiesNearTarget > 2;
            bool HasHarvestedThoughts = Aimsharp.HasBuff("Harvested Thoughts");
            int VoidBoltCD = Aimsharp.SpellCooldown("Void Bolt") - GCD;
            int SWDCharges = Aimsharp.SpellCharges("Shadow Word: Death");
            int SWDFullRecharge = (int)(Aimsharp.RechargeTime("Shadow Word: Death") - GCD + (9000f) * (1f - Aimsharp.SpellCharges("Shadow Word: Death")));
            bool SWVTalent = Aimsharp.Talent(1, 3);
            float SWVChargesFractional_temp = Aimsharp.SpellCharges("Shadow Word: Void") + (1-(Aimsharp.RechargeTime("Shadow Word: Void") - GCD) / (9000f));
            float SWVChargesFractional = SWVChargesFractional_temp > Aimsharp.MaxCharges("Shadow Word: Void") ? Aimsharp.MaxCharges("Shadow Word: Void") : SWVChargesFractional_temp;
            bool SWPRefreshable = SWPRemains < 4800;
            bool TalentMiseryEnabled = Aimsharp.Talent(3, 2);
            bool TalentDarkVoidEnabled = Aimsharp.Talent(3, 3);
            bool VTRefreshable = VTRemains < 6300;
            int FlameFullRecharge = (int)(Aimsharp.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
            bool LegacyEnabled = Aimsharp.Talent(7, 1);
            bool CooldownMindbenderUp = Aimsharp.SpellCooldown("Mindbender") <= 0;
            bool BuffVoidformUp = Aimsharp.HasBuff("Voidform");
            bool BuffDarkThoughtsUp = Aimsharp.HasBuff("Dark THoughts");
            bool BuffUnfurlingDarknessUp = Aimsharp.HasBuff("Unfurling Darkness");
            bool TalentPsychicLinkEnabled = Aimsharp.Talent(5, 2);
            bool TalentHungeringVoidEnabled = Aimsharp.Talent(7, 2);
            bool DebuffUnfurlingDarknessUp = Aimsharp.HasDebuff("Unfurling Darkness", "player");
            bool DebuffWeakenedSoulUp = Aimsharp.HasDebuff("Weakened Soul", "player");



            
            
            
            if (Aimsharp.CanCast("Shadow Mend", "player")) {
                if (PlayerHealth <= GetSlider("Auto Shadow Mend Self @ HP%")) {
                    Aimsharp.Cast("Shadow Mend");
                    return true;
                }
            }
            
			
            if (Aimsharp.CanCast("Vampiric Embrace")) {
                if (PlayerHealth <= GetSlider("Auto Vampiric Embrace @ HP%")) {
                    Aimsharp.Cast("Vampiric Embrace");
                    return true;
                }
            }

            if (NoAOE)
            {
                EnemiesNearTarget = 1;
                EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
            }

            if (IsChanneling && (PlayerCastingID == 48045 || PlayerCastingID == 296962 || PlayerCastingID == 263165 ||
                                 PlayerCastingID == 15407)) {
                if ((EnemiesNearTarget > 2 || EnemiesNearTarget > 1 && SWPRefreshable) &&
                    Aimsharp.CanCast("Searing Nightmare", "player")) {
                    Aimsharp.Cast("Searing Nightmare");
                    return true;
                }

                if (TalentSearingNightmareEnabled && SWPRefreshable && EnemiesNearTarget > 2 && Aimsharp.CanCast("Searing Nightmare", "player")) {
                    Aimsharp.Cast("Searing Nightmare");
                    return true;
                }
                

                
                if (Aimsharp.CanCast("Mind Blast")) {
                    Aimsharp.Cast("Mind Blast");
                    return true;
                }
                

                return false;
            }

            if (IsMoving && !DebuffWeakenedSoulUp) {
                Aimsharp.Cast("Power Word: Shield");
                return true;
            }


            if (UsePotion)
            {
                if (Aimsharp.CanUseItem(PotionType, false)) // don't check if equipped
                {
                    if ((HasLust || TargetTimeToDie <= 80000 || TargetHealth < 35))
                    {
                        Aimsharp.Cast("potion", true);
                        return true;
                    }
                }
            }



            if (!NoCooldowns && Aimsharp.CanCast("Void Eruption", "player") && !IsMoving && Fighting && Insanity >= 40)
            {
                Aimsharp.Cast("Void Eruption");
                return true;
            }

            #region Cooldowns

            if (Aimsharp.CanCast("Power Infusion") && BuffVoidformUp) {
                Aimsharp.Cast("Void Form");
                return true;
            }

            if (Aimsharp.CanCast("Mindgames") && Insanity < 90 && (AllDotsUp || VoidformUp)) {
                Aimsharp.Cast("Mindgames");
                return true;
            }
            
            

            #endregion

            if (Aimsharp.CanCast("Mind Sear") && TalentSearingNightmareEnabled && EnemiesNearTarget > 4 &&
                SWPRemains <= 0 && !CooldownMindbenderUp) {
                Aimsharp.Cast("Mind Sear");
                return true;
            }

            if (Aimsharp.CanCast("Damnation") && !AllDotsUp) {
                Aimsharp.Cast("Damnation");
                return true;
            }

            if (Aimsharp.CanCast("Void Bolt") && Insanity <= 85 &&
                ((TalentHungeringVoidEnabled && EnemiesNearTarget < 5) || EnemiesNearTarget == 1)) {
                Aimsharp.Cast("Void Bolt");
                return true;
            }
            
            //actions.main+=/devouring_plague,target_if=(refreshable|insanity>75)&!variable.pi_or_vf_sync_condition&(!talent.searing_nightmare.enabled|(talent.searing_nightmare.enabled&!variable.searing_nightmare_cutoff))
            if (Aimsharp.CanCast("Devouring Plague") && (Insanity > 75 || DVPRemains < 1800) &&
                (!TalentSearingNightmareEnabled || (TalentSearingNightmareEnabled && !SearingNightmaresCutoff))) {
                Aimsharp.Cast("Devouring Plague");
                return true;
            }

            if (Aimsharp.CanCast("Void Bolt") && EnemiesNearTarget < 4 && Insanity <= 85) {
                Aimsharp.Cast("Void Bolt");
                return true;
            }

            if (Aimsharp.CanCast("Shadow Word: Death") && TargetHealth <= 20) {
                Aimsharp.Cast("Shadow Word: Death");
                return true;
            }
            
            if (!NoCooldowns && Aimsharp.CanCast("Shadowfiend") && VTRemains > 0 &&
                ((TalentSearingNightmareEnabled && EnemiesNearTarget > 4) || SWPRemains > 0)) {
                Aimsharp.Cast("Shadowfiend");
                return true;
            }

            
            if (Aimsharp.CanCast("Void Torrent") && AllDotsUp && !BuffVoidformUp) {
                Aimsharp.Cast("Void Torrent");
                return true;
            }

            if (Aimsharp.CanCast("Mind Sear") && EnemiesNearTarget > 3 && BuffDarkThoughtsUp) {
                Aimsharp.Cast("Mind Sear");
                return true;
            }

            if (Aimsharp.CanCast("Mind Blast") && DotsUp && EnemiesNearTarget < 4) {
                Aimsharp.Cast("Mind Blast");
                return true;
            }

            
            if (Aimsharp.CanCast("Mind Flay") && BuffDarkThoughtsUp && DotsUp) {
                Aimsharp.Cast("Mind Flay");
                return true;
            }

            if (Aimsharp.CanCast("Vampiric Touch") && !(!BuffUnfurlingDarknessUp && DebuffUnfurlingDarknessUp && Aimsharp.LastCast() == "Vampiric Touch") &&
                (VTRefreshable || (TalentMiseryEnabled && SWPRefreshable) || BuffUnfurlingDarknessUp)) {
                Aimsharp.Cast("Vampiric Touch");
                return true;
            }
            
            if (Aimsharp.CanCast("Shadow Word: Pain") && IsMoving) {
                Aimsharp.Cast("Shadow Word: Pain");
                return true;
            }

            if (Aimsharp.CanCast("Shadow Word: Pain") && (SWPRefreshable && !TalentMiseryEnabled &&
                                                          TalentPsychicLinkEnabled && EnemiesNearTarget > 2)) {
                Aimsharp.Cast("Shadow Word: Pain");
                return true;
            }

            if (Aimsharp.CanCast("Shadow Word: Pain") && (SWPRefreshable && !TalentMiseryEnabled &&
                                                          !(TalentSearingNightmareEnabled && EnemiesNearTarget > 4) &&
                                                          (!TalentPsychicLinkEnabled || (TalentPsychicLinkEnabled &&
                                                              EnemiesNearTarget <= 2)))) {
                Aimsharp.Cast("Shadow Word: Pain");
                return true;
            }

            if (Aimsharp.CanCast("Mind Sear") || Aimsharp.CanCast("Mind Flay")) {
                Aimsharp.PrintMessage("Enemies near target" + EnemiesNearTarget);
                Aimsharp.PrintMessage("CanCast Mind Sear" + Aimsharp.CanCast("Mind Sear"));
            }
            
            if (Fighting && (!IsChanneling || PlayerCastingID == 15407) && GCD <= 0 && EnemiesNearTarget >= 2) {
                Aimsharp.Cast("Mind Sear");
                return true;
            }
            Aimsharp.PrintMessage("CanCast Mind Flay"+ Aimsharp.CanCast("Mind Flay"));
            if(Fighting && !IsChanneling && GCD <= 0 && EnemiesNearTarget <3) {
                Aimsharp.Cast("Mind Flay");
                return true;
            }

            

            



            return false;
        }

        public override bool OutOfCombatTick()
        {
            bool IsMoving = Aimsharp.PlayerIsMoving();
            bool DebuffWeakenedSoulUp = Aimsharp.HasDebuff("Weakened Soul", "player");
            if (!Aimsharp.HasBuff("Shadowform"))
            {
                if (Aimsharp.CanCast("Shadowform", "player"))
                {
                    Aimsharp.Cast("Shadowform");
                    return true;
                }
            }
            
            if (Aimsharp.CanCast("Power Word: Fortitude", "player") && !Aimsharp.HasBuff("Power Word: Fortitude", "player", false)) {
                Aimsharp.Cast("Power Word: Fortitude");
                return true;
            }
            
            if (IsMoving && !DebuffWeakenedSoulUp) {
                Aimsharp.Cast("Power Word: Shield");
                return true;
            }
            return false;
        }

    }
}

