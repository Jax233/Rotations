using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{
    /// <summary>
    /// Check API-DOC for detailed documentation.
    /// </summary>
    public class VidMarksman : Rotation
    {
        public override void LoadSettings()
        {
            Aimsharp.Latency = 50;
            Aimsharp.QuickDelay = 125;
            Aimsharp.SlowDelay = 250;

            List<string> MajorAzeritePower = new List<string>(new string[] { "Guardian of Azeroth", "Focused Azerite Beam", "Concentrated Flame", "Worldvein Resonance", "Memory of Lucid Dreams", "Blood of the Enemy", "Reaping Flames", "None" });
            Settings.Add(new Setting("Major Power", MajorAzeritePower, "Blood of the Enemy"));

            List<string> Trinkets = new List<string>(new string[] { "Azshara's Font of Power", "Ashvane's Razor Coral", "Pocket-Sized Computation Device", "Galecaller's Boon", "Shiver Venom Relic", "Lurker's Insidious Gift", "Notorious Gladiator's Badge", "Sinister Gladiator's Badge", "Sinister Gladiator's Medallion", "Notorious Gladiator's Medallion", "Vial of Animated Blood", "First Mate's Spyglass", "Jes' Howler", "Ashvane's Razor Coral", "Generic", "None" });
            Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
            Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));

            Settings.Add(new Setting("Use item: Case Sens", "None"));
            Settings.Add(new Setting("Use item @ HP%", 0, 100, 100));
            
            Settings.Add(new Setting("Azerite Focused Fire", false));
            Settings.Add(new Setting("Azerite Surging Shots", false));
            Settings.Add(new Setting("Azerite In the Rythm Rank: ", 0, 3, 0));
            
            Settings.Add(new Setting("Auto Exhilaration @ HP%", 0, 100, 35));

            List<string> Race = new List<string>(new string[] { "Orc", "Troll", "Dark Iron Dwarf", "Mag'har Orc", "Lightforged Draenei", "None" });
            Settings.Add(new Setting("Racial Power", Race, "None"));
        }

        string MajorPower;
        string TopTrinket;
        string BotTrinket;
        string RacialPower;
        string usableitems;
        
        private int TTK = 10000000;

        private int DPS = 0;
        
        private int FirstTime = 0;

        private int FirstLife = 0;

        private int UpdateTime = 0;

        
        private int FirstMaxHP = 10000000;
        private int AmountHP;
        private int PercentHP;

        private bool LastCastAimshot;
        private bool LastCastRapid;

        private bool AzeriteFocusedFireEnabled;

        private bool AzeriteSurgingShotsEnabled;

        private int AzeriteInTheRythmRank;
        

        public override void Initialize() {

            Aimsharp.DebugMode();
            Aimsharp.PrintMessage("Vid Marksman - v 1.0", Color.Yellow);
            
            Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx NoAOE --Toggles AOE mode on/off.", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx Potions --Toggles using buff potions on/off.", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx SaveCooldowns --Toggles the use of big cooldowns on/off.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);


            MajorPower = GetDropDown("Major Power");
            TopTrinket = GetDropDown("Top Trinket");
            BotTrinket = GetDropDown("Bot Trinket");
            RacialPower = GetDropDown("Racial Power");
            usableitems = GetString("Use item: Case Sens");
            AzeriteFocusedFireEnabled = GetCheckBox("Azerite Focused Fire");
            AzeriteSurgingShotsEnabled = GetCheckBox(("Azerite Surging Shots"));
            AzeriteInTheRythmRank = GetSlider("Azerite In the Rythm Rank: ");
            

            if (RacialPower == "Orc")
                Spellbook.Add("Blood Fury");
            if (RacialPower == "Troll")
                Spellbook.Add("Berserking");
            if (RacialPower == "Dark Iron Dwarf")
                Spellbook.Add("Fireblood");
            if (RacialPower == "Mag'har Orc")
                Spellbook.Add("Ancestral Call");
            if (RacialPower == "Lightforged Draenei")
                Spellbook.Add("Light's Judgment");

            Spellbook.Add(MajorPower);
            Spellbook.Add("Rapid Fire");
            Spellbook.Add("Hunter's Mark");
            Spellbook.Add("Aimed Shot");
            Spellbook.Add("Steady Shot");
            Spellbook.Add("Multi-Shot");
            Spellbook.Add("Trueshot");
            Spellbook.Add("Barrage");
            Spellbook.Add("Explosive Shot");
            Spellbook.Add("Arcane Shot");
			Spellbook.Add("Kill Shot");
            Spellbook.Add("Bursting Shot");
            Spellbook.Add("Exhilaration");
            Spellbook.Add("Double Tap");
            
            
            

            Buffs.Add("Lifeblood");
            Buffs.Add("Precise Shots");
            Buffs.Add("Trick Shots");
            Buffs.Add("Trueshot");
            

            Debuffs.Add("Razor Coral");
            Debuffs.Add("Hunter's Mark");

            Items.Add(TopTrinket);
            Items.Add(BotTrinket);
            Items.Add(usableitems);

            Macros.Add("ItemUse", "/use " + usableitems);
            Macros.Add("TopTrink", "/use 13");
            Macros.Add("BotTrink", "/use 14");

            CustomCommands.Add("NoAOE");
            CustomCommands.Add("Potions");
            CustomCommands.Add("SaveCooldowns");
        }

        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick()
        {
            
            int PetHealth = Aimsharp.Health("pet");
            int GCD = Aimsharp.GCD();
            int Latency = Aimsharp.Latency;
            bool Moving = Aimsharp.PlayerIsMoving();
            bool IsChanneling = Aimsharp.IsChanneling("player");
            bool Fighting = Aimsharp.Range("target") <= 45 && Aimsharp.TargetIsEnemy();
            bool FontEquipped = Aimsharp.IsEquipped("Azshara's Font of Power");
            bool CanUseFont = Aimsharp.CanUseItem("Azshara's Font of Power");
            bool CoralEquipped = Aimsharp.IsEquipped("Ashvane's Razor Coral");
            bool CanUseCoral = Aimsharp.CanUseItem("Ashvane's Razor Coral");
            bool CycloEquipped = Aimsharp.IsEquipped("Pocket-Sized Computation Device");
            bool CanUseCyclo = Aimsharp.CanUseItem("Pocket-Sized Computation Device");
            bool CoralDebuffUp = Aimsharp.HasDebuff("Razor Coral", "target");
            string PrevGCD = Aimsharp.LastCast();
            string LastCast = Aimsharp.LastCast();
            int TargetHealth = Aimsharp.Health("target");
            int PlayerHealth = Aimsharp.Health("player");
            int TargetTimeToDie = 1000000000;
            int CycloCD = Aimsharp.ItemCooldown("Cyclotronic Blast") - GCD;
            float Haste = Aimsharp.Haste() / 100f;
            int GCDMAX = (int)((1500f / (Haste + 1f)));
            bool UsePotion = Aimsharp.IsCustomCodeOn("Potions");
            bool NoAOE = Aimsharp.IsCustomCodeOn("NoAOE");
            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();
            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            int Focus = Aimsharp.Power("player");
            float FocusRegen = 10f * (1f + Haste);
            int BarbedShotBuffCount = Aimsharp.BuffStacks("Barbed Shot");
            int CDAncestralCall = Aimsharp.SpellCooldown("Ancestral Call");

            int AimedShotCastTime = (int) (2500f / (Haste + 1f));
            bool IsMoving = Aimsharp.PlayerIsMoving();

            bool TalentCarefulAim = Aimsharp.Talent(2, 1);
            bool CAUp = TargetHealth > 70 && TalentCarefulAim;
            

            bool TalentStreamline = Aimsharp.Talent(4, 2);
            bool TalentLethalShots = Aimsharp.Talent(6, 1);

            
            

            int STRemains = Aimsharp.DebuffRemaining("Serpent Sting", "target") - GCD;
            bool STRefreshable = STRemains < (18000 / 3);

            bool DebuffHunterMark = Aimsharp.HasDebuff("Hunter's Mark", "target");
            bool BuffPreciseShots = Aimsharp.HasBuff("Precise Shots", "player");
            bool BuffDoubleTap = Aimsharp.HasBuff("Double Tap", "player");
            bool BuffTrickShotsUp = Aimsharp.HasBuff("Trick Shots", "player");
            bool BuffTrueShotUp = Aimsharp.HasBuff("Trickshot", "player");
            int BuffTrueShotRemains = Aimsharp.BuffRemaining("Trueshot", "player");

            int CDTrueshotRemains = Aimsharp.SpellCooldown("Trueshot");
            int CDAimedShotRemains = Aimsharp.SpellCooldown("Aimed Shot");
            int CDRapidFireRemains = Aimsharp.SpellCooldown("Rapid Fire");
            

            

            float AimedShotRechargeTime = (float) ((12000f / (Haste + 1f)));
            int AimedShotFullRecharge = (int)(Aimsharp.RechargeTime("Aimed Shot") - GCD + (AimedShotRechargeTime) * (1f - Aimsharp.SpellCharges("Aimed Shot")));

            int FocusMax = Aimsharp.PlayerMaxPower();
            bool LastCastAimshot = Aimsharp.CastingID("player") == 19434;
            bool LastCastRapidFire = Aimsharp.CastingID("player") == 257044;
            float CritPercent = Aimsharp.Crit() / 100f;
            float FocusTimeToMax = (FocusMax - Focus) * 1000f / FocusRegen;
            int FlameFullRecharge = (int)(Aimsharp.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
            bool NoCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            
            
            #region TTK

            int CurrentTime = Aimsharp.CombatTime();
            int CurrentHP = Aimsharp.TargetExactCurrentHP();
            int MaxHP = Aimsharp.TargetExactMaxHP();
            if (FirstMaxHP != MaxHP) {
                UpdateTime = 0;
                TTK = 100000000;
                FirstLife = CurrentHP;
                FirstMaxHP = MaxHP;
            }
            if (CurrentTime >= UpdateTime + 1000) {
                if (CurrentHP < FirstLife) {
                    int HPDiff = FirstLife - CurrentHP;
                    int TimeDiff = CurrentTime - UpdateTime;
                    if (TimeDiff > 0) {
                        DPS = HPDiff / TimeDiff;
                    }

                    if (DPS > 0) {

                        TTK = CurrentHP / DPS;
                        Aimsharp.PrintMessage("TTK: " + TimeSpan.FromMilliseconds(TTK).ToString());
                    }


                }

                if (FirstLife == 0 && CurrentHP > 0) {
                    FirstLife = MaxHP;
                }
            }

            
            
            

            
            
            
            



            #endregion


            #region Utillity

            if (Aimsharp.CanCast("Exhilaration", "player")) {
                if (PlayerHealth <= GetSlider("Auto Exhilaration @ HP%")) {
                    Aimsharp.Cast("Exhilaration");
                    return true;
                }
            }

            #endregion
            

            if (Fighting) {

                if (NoAOE) {
                    EnemiesNearTarget = 1;
                    EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
                    EnemiesNearTarget = EnemiesNearTarget > 0 ? 1 : 0;
                }

                if (IsChanneling || Aimsharp.CastingID("player") == 295261 || Aimsharp.CastingID("player") == 299338 ||
                    Aimsharp.CastingID("player") == 295258 || Aimsharp.CastingID("player") == 299336 ||
                    Aimsharp.CastingID("player") == 295273 || Aimsharp.CastingID("player") == 295264 ||
                    Aimsharp.CastingID("player") == 295261 || Aimsharp.CastingID("player") == 295263 ||
                    Aimsharp.CastingID("player") == 295262 || Aimsharp.CastingID("player") == 295272 ||
                    Aimsharp.CastingID("player") == 299564) {
                    return false;
                }

                


                if (!NoCooldowns) {

                    if (Aimsharp.CanCast("Double Tap", "player") &&
                        (CDRapidFireRemains < GCD || CDRapidFireRemains < CDAimedShotRemains || TTK < 20000)) {
                        Aimsharp.Cast("Double Tap");
                        return true;
                    }
                    
                    //actions.cds+=/ancestral_call,if=buff.trueshot.remains>14&(target.time_to_die>cooldown.ancestral_call.duration+duration|(target.health.pct<20|!talent.careful_aim.enabled))|target.time_to_die<16
                    if (Aimsharp.CanCast("Ancestral Call", "player") && (BuffTrueShotRemains > 14000 && (TTK > CDAncestralCall + 15000) || (TargetHealth < 20 || !TalentCarefulAim)) || TTK < 16000) {
                        Aimsharp.Cast("Ancestral Call");
                        return true;
                    }

                    
                    //actions.cds+=/trueshot,if=buff.trueshot.down&cooldown.rapid_fire.remains|target.time_to_die<15
                    if (Aimsharp.CanCast("Trueshot", "player") && ((CDRapidFireRemains > 0 && !BuffTrueShotUp) || TTK < 15000)) {
                        Aimsharp.Cast("Trueshot");
                        return true;
                    }



                }

                if (EnemiesNearTarget < 3) {
                    if (Aimsharp.CanCast("Kill Shot")) {
                        Aimsharp.Cast("Kill Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Explosive Shot")) {
                        Aimsharp.Cast("Explosive Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barrage") && EnemiesNearTarget > 1) {
                        Aimsharp.Cast("Barrage");
                        return true;
                    }

                    if (Aimsharp.CanCast("A Murder of Crows")) {
                        Aimsharp.Cast("A Murder of Crows");
                        return true;
                    }

                    if (Aimsharp.CanCast("Volley", "player")) {
                        Aimsharp.Cast("Volley");
                        return true;
                    }

                    if (Aimsharp.CanCast("Serpent Sting") && Aimsharp.LastCast() != "Serpent Sting" && STRefreshable) {
                        Aimsharp.Cast("Serpent Sting");
                        return true;
                    }

                    if (Aimsharp.CanCast("Rapid Fire") &&
                        (!BuffTrueShotUp || Focus < 35 || Focus < 60 && !TalentLethalShots)) {
                        Aimsharp.Cast("Rapid Fire");
                        return true;
                    }

                    if (Aimsharp.CanCast("Aimed Shot") && !IsMoving && (BuffTrueShotUp ||
                                                           (!BuffDoubleTap || CAUp) && !BuffPreciseShots ||
                                                           AimedShotFullRecharge < AimedShotCastTime &&
                                                           CDTrueshotRemains > 0)) {
                        Aimsharp.Cast("Aimed Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Arcane Shot") &&
                        (!BuffTrueShotUp && (BuffPreciseShots && (Focus > 55) || (Focus > 75 || TTK < 5000)))) {
                        Aimsharp.Cast("Arcane Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Chimaera Shot") &&
                        (!BuffTrueShotUp && (BuffPreciseShots && (Focus > 55) || Focus > 75 || TTK < 5000))) {
                        Aimsharp.Cast("Chimaera Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Steady Shot")) {
                        Aimsharp.Cast("Steady Shot");
                        return true;
                    }
                }
                
                

                if (EnemiesNearTarget > 2) {

                    
                    if (Aimsharp.CanCast("Kill Shot")) {
                        Aimsharp.Cast("Kill Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Volley")) {
                        Aimsharp.Cast("Volley");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barrage")) {
                        Aimsharp.Cast("Barrage");
                        return true;
                    }

                    if (Aimsharp.CanCast("Explosive Shot")) {
                        Aimsharp.Cast("Explosive Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Aimed Shot") && !IsMoving && BuffTrickShotsUp && CAUp && BuffDoubleTap && !LastCastAimshot && !LastCastRapidFire) {
                        Aimsharp.Cast("Aimed Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Rapid Fire") && !LastCastAimshot && !LastCastRapidFire && (BuffTrickShotsUp && (AzeriteFocusedFireEnabled ||
                        AzeriteInTheRythmRank > 1 ||
                        AzeriteSurgingShotsEnabled ||
                        TalentStreamline))) {
                        Aimsharp.Cast("Rapid Fire");
                        return true;
                    }
                    
                    //buff.trick_shots.up & ( buff.precise_shots.down | cooldown.aimed_shot.full_recharge_time < action.aimed_shot.cast_time | buff.trueshot.up )
                    if (Aimsharp.CanCast("Aimed Shot") && !LastCastAimshot && !LastCastRapidFire && (BuffTrickShotsUp && !IsMoving &&
                                                           (!BuffPreciseShots ||
                                                            AimedShotFullRecharge < AimedShotCastTime ||
                                                            BuffTrueShotUp))) {
                        Aimsharp.Cast("Aimed Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Rapid Fire") && !LastCastAimshot && !LastCastRapidFire && BuffTrickShotsUp) {
                        Aimsharp.Cast("Rapid Fire");
                        return true;
                    }

                    if (Aimsharp.CanCast("Multi-Shot") &&
                        (!BuffTrickShotsUp || (BuffPreciseShots && !BuffTrueShotUp) || Focus > 70)) {
                        Aimsharp.Cast("Multi-Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("A Murder of Crows")) {
                        Aimsharp.Cast("A Murder of Crows");
                        return true;
                    }

                    if (Aimsharp.CanCast("Serpent Sting") && Aimsharp.LastCast() != "Serpent Sting" && STRefreshable) {
                        Aimsharp.Cast("Serpent Sting");
                        return true;
                    }

                    if (Aimsharp.CanCast("Steady Shot")) {
                        Aimsharp.Cast("Steady Shot");
                        return true;
                    }



                }
                
            }

            return false;
        }

        public override bool OutOfCombatTick()
        {
            return false;
        }

    }
}
