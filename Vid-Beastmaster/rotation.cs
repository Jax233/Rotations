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
    public class VidBeastmaster : Rotation
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
            
            Settings.Add(new Setting("Azerite Primal Instincts", false));
            Settings.Add(new Setting("Azerite Rapid Reload", false));
            Settings.Add(new Setting("Azerite Dance of Death Rank: ", 0, 3, 0));
            
            
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

        private bool AzeriteRapidReloadEnabled;

        private bool AzeritePrimalInstinctsEnabled;

        private int AzeriteDanceOfDeathRank;
        

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
            AzeritePrimalInstinctsEnabled = GetCheckBox("Azerite Primal Instincts");
            AzeriteRapidReloadEnabled = GetCheckBox(("Azerite Rapid Reload"));
            AzeriteDanceOfDeathRank = GetSlider("Azerite Dance of Death Rank: ");
            

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
            Spellbook.Add("Bestial Wrath");
            Spellbook.Add("Hunter's Mark");
            Spellbook.Add("Kill Command");
            Spellbook.Add("Cobra Shot");
            Spellbook.Add("Multi-Shot");
            Spellbook.Add("Barbed Shot");
            Spellbook.Add("Barrage");
            Spellbook.Add("Dire Beast");
            Spellbook.Add("Stampede");
			Spellbook.Add("Kill Shot");
            Spellbook.Add("Bloodshed");
            Spellbook.Add("Exhilaration");
            Spellbook.Add("Arcane Shot");
            Spellbook.Add("Aspect of the Wild");
            
            
            

            Buffs.Add("Lifeblood");
            Buffs.Add("Beast Cleave");
            Buffs.Add("Trick Shots");
            Buffs.Add("Trueshot");
            Buffs.Add("Memory of Lucid Dreams");
            

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
            bool BuffMemoryOfLucidDreamsUp = Aimsharp.HasBuff("Memory of Lucid Dream", "player");

            int AimedShotCastTime = (int) (2500f / (Haste + 1f));
            bool IsMoving = Aimsharp.PlayerIsMoving();

            bool TalentCarefulAim = Aimsharp.Talent(2, 1);
            bool CAUp = TargetHealth > 70 && TalentCarefulAim;

            int CDBestialWrathRemains = Aimsharp.SpellCooldown("Bestial Wrath");
            int CDKillCommandRemains = Aimsharp.SpellCooldown("Kill Command");
            int CDAspectOfTheWildRemains = Aimsharp.SpellCooldown("Aspect of the Wild");

            bool BuffFrenzyUp = Aimsharp.HasBuff("Frenzy", "player");
            int BuffBeastCleaveRemains = Aimsharp.BuffRemaining("Beast Cleave", "player");
            int BuffFrenzyRemains = Aimsharp.BuffRemaining("Frenzy", "player");
            int BuffDanceOfDeathRemains = Aimsharp.BuffRemaining("Dance of Death", "player");
            int BuffBestialWrathRemains = Aimsharp.BuffRemaining("Bestial Wrath", "player");
            int BarbedShotRechargeTime = Aimsharp.RechargeTime("Barbed Shot");
            int BarbedShotFullRecharge = (int)(Aimsharp.RechargeTime("Barbed Shot") - GCD + (BarbedShotRechargeTime) * (1f - Aimsharp.SpellCharges("Barbed Shot")));
            bool TalentScentOfBlood = Aimsharp.Talent(2, 1);
            bool TalentOneWithThePack = Aimsharp.Talent(1, 2);
            bool BuffBestialWrathUp = Aimsharp.HasBuff("Bestial Wrath", "player");
            bool BuffAspectOfTheWildUp = Aimsharp.HasBuff("Aspect of the Wild", "player");
            int BarbedShotCharges = Aimsharp.SpellCharges("Barbed Shot");
            
            


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

                


                

                if (EnemiesNearTarget < 2) {
                    if (Aimsharp.CanCast("Kill Shot")) {
                        Aimsharp.Cast("Kill Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Bloodshed")) {
                        Aimsharp.Cast("Bloodshed");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barbed Shot") && (BuffFrenzyUp && BuffFrenzyRemains < GCD || CDBestialWrathRemains > 0 && (BarbedShotFullRecharge < GCD || AzeritePrimalInstinctsEnabled && CDAspectOfTheWildRemains < GCD) || CDBestialWrathRemains < 12000 + GCD && TalentScentOfBlood)) {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Aspect of the Wild", "player") && (!BuffAspectOfTheWildUp &&
                                                                   (BarbedShotCharges < 1 ||
                                                                    !AzeritePrimalInstinctsEnabled))) {
                        Aimsharp.Cast("Aspect of the Wild");
                        return true;
                    }
                    
                    if(Aimsharp.CanCast("Stampede") && (BuffAspectOfTheWildUp && BuffBestialWrathUp || TTK < 15000))

                    if (Aimsharp.CanCast("A Murder of Crows")) {
                        Aimsharp.Cast("A Murder of Crows");
                        return true;
                    }

                    if (Aimsharp.CanCast("Bestial Wrath", "player") && (TalentScentOfBlood ||
                                                              TalentOneWithThePack && BuffBestialWrathRemains < GCD ||
                                                              !BuffBestialWrathUp && CDAspectOfTheWildRemains > 15000 ||
                                                              TTK < 15000 + GCD)) {
                        Aimsharp.Cast("Bestial Wrath");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barbed Shot") &&
                        (AzeriteDanceOfDeathRank > 1 && BuffDanceOfDeathRemains < GCD)) {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Kill Command")) {
                        Aimsharp.Cast("Kill Command");
                        return true;
                    }

                    if (Aimsharp.CanCast("Chimaera Shot") ) {
                        Aimsharp.Cast("Chimaera Shot");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Dire Beast", "player") ) {
                        Aimsharp.Cast("Dire Beast");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barbed Shot") &&
                        (TalentOneWithThePack && CDAspectOfTheWildRemains < BuffFrenzyRemains - GCD &&
                            AzeritePrimalInstinctsEnabled || TTK < 9000)) {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barrage", "player")) {
                        Aimsharp.Cast("Barrage");
                        return true;
                    }

                    if (Aimsharp.CanCast("Cobra Shot") &&
                        ((Focus - 35 + FocusRegen * (CDKillCommandRemains - 1) > 30 ||
                          CDKillCommandRemains > 1 + GCD && CDBestialWrathRemains > FocusTimeToMax ||
                          BuffMemoryOfLucidDreamsUp) && CDKillCommandRemains > 1 || TTK < 3000)) {
                        Aimsharp.Cast("Cobra Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barbed Shot") && (BuffFrenzyRemains - GCD > BarbedShotFullRecharge)) {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }
                }
                
                

                if (EnemiesNearTarget > 1) {

                    //actions.cleave=barbed_shot,target_if=min:dot.barbed_shot.remains,if=pet.main.buff.frenzy.up&pet.main.buff.frenzy.remains<=gcd.max
                    if (Aimsharp.CanCast("Barbed Shot") && (BuffFrenzyUp && BuffFrenzyRemains <= GCDMAX || CDBestialWrathRemains < 12000 + GCD && TalentScentOfBlood)) {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Multi-Shot") && (GCDMAX - BuffBeastCleaveRemains > 250)) {
                        Aimsharp.Cast("Multi-Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barbed Shot") && (BarbedShotFullRecharge < GCDMAX && CDBestialWrathRemains > 0)) {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Aspect of the Wild", "player")) {
                        Aimsharp.Cast("Aspect of the Wild");
                        return true;
                    }

                    if (Aimsharp.CanCast("Stampede", "player") && (BuffAspectOfTheWildUp && BuffBestialWrathUp || TTK < 15000)) {
                        Aimsharp.Cast("Stampede");
                        return true;
                    }

                    if (Aimsharp.CanCast("Bestial Wrath", "player") && (TalentScentOfBlood || CDAspectOfTheWildRemains > 20000 || TalentOneWithThePack || TTK < 15000) ) {
                        Aimsharp.Cast("Bestial Wrath");
                        return true;
                    }
                    
                    //buff.trick_shots.up & ( buff.precise_shots.down | cooldown.aimed_shot.full_recharge_time < action.aimed_shot.cast_time | buff.trueshot.up )
                    if (Aimsharp.CanCast("Chimaera Shot")) {
                        Aimsharp.Cast("Chimaera Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("A Murder of Crows")) {
                        Aimsharp.Cast("A Murder of Crows");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barrage", "player")) {
                        Aimsharp.Cast("Barrage");
                        return true;
                    }

                    if (Aimsharp.CanCast("Kill Command") && EnemiesNearTarget < 4) {
                        Aimsharp.Cast("Kill Command");
                        return true;
                    }

                    if (Aimsharp.CanCast("Dire Beast", "player")) {
                        Aimsharp.Cast("Dire Beast");
                        return true;
                    }

                    if (Aimsharp.CanCast("Barbed Shot") && !BuffFrenzyUp &&
                        (BarbedShotFullRecharge < GCDMAX || BuffBestialWrathUp) ||
                        CDAspectOfTheWildRemains < BuffFrenzyRemains - GCD && AzeritePrimalInstinctsEnabled ||
                        BarbedShotFullRecharge > GCDMAX || TTK < 9000) {
                        Aimsharp.Cast("Barbed Shot");
                        return true;
                    }
                        
                        
                        
                    
                    

                    if (Aimsharp.CanCast("Multi-Shot") && (AzeriteRapidReloadEnabled && EnemiesNearTarget > 2)) {
                        Aimsharp.Cast("Multi-Shot");
                        return true;
                    }

                    if (Aimsharp.CanCast("Cobra Shot") && (CDKillCommandRemains > FocusTimeToMax &&
                                                           (EnemiesNearTarget < 3 || !AzeriteRapidReloadEnabled))) {
                        Aimsharp.Cast("Cobra Shot");
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
