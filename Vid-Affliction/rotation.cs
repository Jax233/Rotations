using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API


namespace AimsharpWow.Modules
{
   
    public class VidAffliction : Rotation {


        public override void LoadSettings() {
            List<string> MajorAzeritePower = new List<string>(new string[] {
                "Guardian of Azeroth", "Focused Azerite Beam", "Concentrated Flame", "Worldvein Resonance",
                "Memory of Lucid Dreams", "Blood of the Enemy", "None"
            });
            Settings.Add(new Setting("Major Power", MajorAzeritePower, "None"));

            List<string> Trinkets = new List<string>(new string[]
                {"Azshara's Font of Power", "Shiver Venom Relic", "Generic", "None"});
            Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
            Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));
            
            Settings.Add(new Setting("Use item: Case Sens", "None"));
            Settings.Add(new Setting("Use item @ HP%", 0, 100, 100));

            List<string> Potions = new List<string>(new string[]
                {"Potion of Unbridled Fury", "Potion of Empowered Proximity", "Potion of Prolonged Power", "None"});
            Settings.Add(new Setting("Potion Type", Potions, "Potion of Unbridled Fury"));
            
            Settings.Add(new Setting("Auto Shadow Mend Self @ HP%", 0, 100, 25));

            Settings.Add(new Setting("Auto Vampiric Embrace @ HP%", 0, 100, 45));
            
            Settings.Add(new Setting("Auto Desperate Prayer @ HP%", 0, 100, 35));
            
            Settings.Add(new Setting("Auto Healthstone @ HP%", 0, 100, 25));
            
            Settings.Add(new Setting("Don't dot below HP%", 0, 100, 3));
            
            Settings.Add(new Setting("Don't dot below HP Amount", 0, 20000, 4000));

            Settings.Add(new Setting("First 5 Letters of the Addon:", "xxxxx"));


        }

        string MajorPower;
        string TopTrinket;
        string BotTrinket;
        

        

        private int FirstTime = 0;

        private int FirstLife = 0;

        private int UpdateTime = 0;
        
        
        private int TTK = 10000000;

        private int DPS = 0;
        
        bool VTLastCast;

        private int FirstMaxHP = 10000000;


        private string FiveLetters;

        private int AmountHP;
        private int PercentHP;




        public override void Initialize() {
            //Aimsharp.DebugMode();
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
            Aimsharp.PrintMessage("/xxxxx MassDispel", Color.Blue);
            Aimsharp.PrintMessage("--Casts Mass Dispel at cursor.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx Dispersion", Color.Blue);
            Aimsharp.PrintMessage("--Casts Dispersion.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx S2M", Color.Blue);
            Aimsharp.PrintMessage("--Casts Surrender to Madness at target.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx DispelMagic", Color.Blue);
            Aimsharp.PrintMessage("--Casts Dispel Magic at target.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx MindControl", Color.Blue);
            Aimsharp.PrintMessage("--Casts Mind Control at target.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx Psychic Scream", Color.Blue);
            Aimsharp.PrintMessage("--Casts Psychic Scream.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx Psychic Horror", Color.Blue);
            Aimsharp.PrintMessage("--Casts Psychic Horror at target.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("/xxxxx CouncilDotsOff", Color.Blue);
            Aimsharp.PrintMessage("--Will keep SW:P and VT up on focus and Boss 1-4.", Color.Blue);
            Aimsharp.PrintMessage("/xxxxx StartCombat", Color.Blue);
            Aimsharp.PrintMessage("--Will initiate combat on by itself.", Color.Blue);
            Aimsharp.PrintMessage(" ");
            Aimsharp.PrintMessage("--Replace xxxxx with first 5 letters of your addon, lowercase.", Color.Blue);


            Aimsharp.Latency = 50;
            Aimsharp.QuickDelay = 125;
            Aimsharp.SlowDelay = 250;

            MajorPower = GetDropDown("Major Power");
            TopTrinket = GetDropDown("Top Trinket");
            BotTrinket = GetDropDown("Bot Trinket");
            FiveLetters = GetString("First 5 Letters of the Addon:");
            AmountHP = GetSlider("Don't dot below HP Amount");
            PercentHP = GetSlider("Don't dot below HP%");
                

            Spellbook.Add(MajorPower);
            Spellbook.Add("Agony");
            Spellbook.Add("Corruption");
            Spellbook.Add("Seed of Corruption");
            Spellbook.Add("Unstable Affliction");
            Spellbook.Add("Summon Darkglare");
            Spellbook.Add("Unending Resolve");
            Spellbook.Add("Command Demon");
            Spellbook.Add("Malefic Rapture");
            Spellbook.Add("Siphon Life");
            Spellbook.Add("Vile Taint");
            Spellbook.Add("Phantom Singularity");
            Spellbook.Add("Shadow Bolt");
            Spellbook.Add("Drain Soul");
            Spellbook.Add("Haunt");
            

            Buffs.Add("Bloodlust");
            Buffs.Add("Heroism");
            Buffs.Add("Time Warp");
            Buffs.Add("Ancient Hysteria");
            Buffs.Add("Netherwinds");
            Buffs.Add("Drums of Rage");
            Buffs.Add("Lifeblood");
            

            
            Debuffs.Add("Concentrated Flame");
            Debuffs.Add("Corruption");
            Debuffs.Add("Siphon Life");
            Debuffs.Add("Agony");
            Debuffs.Add("Unstable Affliction");
            Debuffs.Add("Vile Taint");
            


            Items.Add(TopTrinket);
            Items.Add(BotTrinket);
            Items.Add(GetDropDown("Potion Type"));

            Macros.Add(TopTrinket, "/use " + TopTrinket);
            Macros.Add(BotTrinket, "/use " + BotTrinket);
            Macros.Add("TopTrink", "/use 13");
            Macros.Add("BotTrink", "/use 14");
            Macros.Add("potion", "/use " + GetDropDown("Potion Type"));
            Macros.Add("crash cursor", "/cast [@cursor] Shadow Crash");
            Macros.Add("MassDispel", "/cast [@cursor] Mass Dispel");
            Macros.Add("MassDispelOff", "/" + FiveLetters + " MassDispel");
            Macros.Add("S2MOff", "/" + FiveLetters + " S2M");
            Macros.Add("DispelOff", "/" + FiveLetters + " DispelMagic");
            Macros.Add("DispersionOff", "/" + FiveLetters + " Dispersion");
            Macros.Add("MindControlOff", "/" + FiveLetters + " MindControl");
            Macros.Add("PsychicScreamOff", "/" + FiveLetters + " PsychicScream");
            Macros.Add("PsychicHorrorOff", "/" + FiveLetters + " PsychicHorror");
            Macros.Add("DispelMagicOff", "/" + FiveLetters + " DispelMagic");

            Macros.Add("AgonyFocus", "/cast [@focus] Agony");
            Macros.Add("AgonyBoss1", "/cast [@boss1] Agony");
            Macros.Add("AgonyBoss2", "/cast [@boss2] Agony");
            Macros.Add("AgonyBoss3", "/cast [@boss3] Agony");
            Macros.Add("AgonyBoss4", "/cast [@boss4] Agony");

            Macros.Add("CorruptionFocus", "/cast [@focus] Corruption");
            Macros.Add("CorruptionBoss1", "/cast [@boss1] Corruption");
            Macros.Add("CorruptionBoss2", "/cast [@boss2] Corruption");
            Macros.Add("CorruptionBoss3", "/cast [@boss3] Corruption");
            Macros.Add("CorruptionBoss4", "/cast [@boss4] Corruption");
            
            Macros.Add("SLFocus", "/cast [@focus] Siphon Life");
            Macros.Add("SLBoss1", "/cast [@boss1] Siphon Life");
            Macros.Add("SLBoss2", "/cast [@boss2] Siphon Life");
            Macros.Add("SLBoss3", "/cast [@boss3] Siphon Life");
            Macros.Add("SLBoss4", "/cast [@boss4] Siphon Life");

            


            CustomCommands.Add("NoAOE");
            CustomCommands.Add("Prepull");
            CustomCommands.Add("Potions");
            CustomCommands.Add("SaveCooldowns");
            CustomCommands.Add("MassDispel");
            CustomCommands.Add("JustEssences");
            CustomCommands.Add("CouncilDotsOff");
            CustomCommands.Add("S2M");
            CustomCommands.Add("DispelMagic");
            CustomCommands.Add("Dispersion");
            CustomCommands.Add("PsychicScream");
            CustomCommands.Add("PsychicHorror");
            CustomCommands.Add("MindControl");
            CustomCommands.Add("StartCombat");






        }
        
        





        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick() {
            int GCD = Aimsharp.GCD();
            int Latency = Aimsharp.Latency;
            float Haste = Aimsharp.Haste() / 100f;
            int CRRemains = Aimsharp.DebuffRemaining("Corruption", "target") - GCD;
            int CRRemainsFocus = Aimsharp.DebuffRemaining("Corruption", "focus") - GCD;
            int CRRemainsBoss1 = Aimsharp.DebuffRemaining("Corruption", "boss1") - GCD;
            int CRRemainsBoss2 = Aimsharp.DebuffRemaining("Corruption", "boss2") - GCD;
            int CRRemainsBoss3 = Aimsharp.DebuffRemaining("Corruption", "boss3") - GCD;
            int CRRemainsBoss4 = Aimsharp.DebuffRemaining("Corruption", "boss4") - GCD;
            int AGRemains = Aimsharp.DebuffRemaining("Agony", "target") - GCD;
            int AGRemainsFocus = Aimsharp.DebuffRemaining("Agony", "focus") - GCD;
            int AGRemainsBoss1 = Aimsharp.DebuffRemaining("Agony", "boss1") - GCD;
            int AGRemainsBoss2 = Aimsharp.DebuffRemaining("Agony", "boss2") - GCD;
            int AGRemainsBoss3 = Aimsharp.DebuffRemaining("Agony", "boss3") - GCD;
            int AGRemainsBoss4 = Aimsharp.DebuffRemaining("Agony", "boss4") - GCD;
            int SLRemains = Aimsharp.DebuffRemaining("Siphon Life", "target") - GCD;
            int SLRemainsFocus = Aimsharp.DebuffRemaining("Siphon Life", "focus") - GCD;
            int SLRemainsBoss1 = Aimsharp.DebuffRemaining("Siphon Life", "boss1") - GCD;
            int SLRemainsBoss2 = Aimsharp.DebuffRemaining("Siphon Life", "boss2") - GCD;
            int SLRemainsBoss3 = Aimsharp.DebuffRemaining("Siphon Life", "boss3") - GCD;
            int SLRemainsBoss4 = Aimsharp.DebuffRemaining("Siphon Life", "boss4") - GCD;
            int UARemains = Aimsharp.DebuffRemaining("Unstable Affliction", "target") - GCD;
            
            bool Fighting = Aimsharp.Range("target") <= 45 && Aimsharp.TargetIsEnemy();
            bool UsePotion = Aimsharp.IsCustomCodeOn("Potions");
            string PotionType = GetDropDown("Potion Type");
            bool HasLust = Aimsharp.HasBuff("Bloodlust", "player", false) ||
                           Aimsharp.HasBuff("Heroism", "player", false) ||
                           Aimsharp.HasBuff("Time Warp", "player", false) ||
                           Aimsharp.HasBuff("Ancient Hysteria", "player", false) ||
                           Aimsharp.HasBuff("Netherwinds", "player", false) ||
                           Aimsharp.HasBuff("Drums of Rage", "player", false);
            int TargetTimeToDie = 1000000000;
            int TargetHealth = Aimsharp.Health("target");
            int PlayerHealth = Aimsharp.Health("player");

            int EnemiesNearTarget = Aimsharp.EnemiesNearTarget();

            int EnemiesInMelee = Aimsharp.EnemiesInMelee();
            
            bool NoAOE = Aimsharp.IsCustomCodeOn("NoAOE");
            int Insanity = Aimsharp.Power("player");
            bool VoidformUp = Aimsharp.HasBuff("Voidform");
            bool NoCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
            int VoidformStacks = Aimsharp.BuffStacks("Voidform");
            int GCDMAX = (int) ((1500f / (Haste + 1f)) + GCD);
            float InsanityDrain = 6f + .68f * VoidformStacks;
            int MindBlastCastTime = (int) ((1500f / (Haste + 1f)));
            

            bool IsMoving = Aimsharp.PlayerIsMoving();
            bool FontEquipped = Aimsharp.IsEquipped("Azshara's Font of Power");
            bool CanUseFont = Aimsharp.CanUseItem("Azshara's Font of Power");
            bool IsChanneling = Aimsharp.IsChanneling("player");
            int PlayerCastingID = Aimsharp.CastingID("player");
            bool TalentSearingNightmareEnabled = Aimsharp.Talent(3, 3);
            bool SearingNightmaresCutoff = EnemiesNearTarget > 2;
            bool HasHarvestedThoughts = Aimsharp.HasBuff("Harvested Thoughts");
            int VoidBoltCD = Aimsharp.SpellCooldown("Void Bolt") - GCD;
            int SWDCharges = Aimsharp.SpellCharges("Shadow Word: Death");
            int SWDFullRecharge = (int) (Aimsharp.RechargeTime("Shadow Word: Death") - GCD +
                                         (9000f) * (1f - Aimsharp.SpellCharges("Shadow Word: Death")));
            bool SWVTalent = Aimsharp.Talent(1, 3);
            float SWVChargesFractional_temp = Aimsharp.SpellCharges("Shadow Word: Void") +
                                              (1 - (Aimsharp.RechargeTime("Shadow Word: Void") - GCD) / (9000f));
            float SWVChargesFractional = SWVChargesFractional_temp > Aimsharp.MaxCharges("Shadow Word: Void")
                ? Aimsharp.MaxCharges("Shadow Word: Void")
                : SWVChargesFractional_temp;
            bool CouncilDotsOff = Aimsharp.IsCustomCodeOn("CouncilDotsOff");
            

            bool AGRefreshable = AGRemains < 5400 - GCD && Aimsharp.TargetExactCurrentHP() > AmountHP && TargetHealth > PercentHP;
            bool AGFocusRefreshable = AGRemainsFocus < 5400;
            bool AGBoss1Refreshable = AGRemainsBoss1 < 5400;
            bool AGBoss2Refreshable = AGRemainsBoss2 < 5400;
            bool AGBoss3Refreshable = AGRemainsBoss3 < 5400;
            bool AGBoss4Refreshable = AGRemainsBoss4 < 5400;


            bool TalentPhantomSingularityEnabled = Aimsharp.Talent(4, 2);
            bool TalentVileTaint = Aimsharp.Talent(4, 3);

            bool CRRefreshable = CRRemains < 4200 - GCD && Aimsharp.TargetExactCurrentHP() > AmountHP;
            bool CRFocusRefreshable = CRRemainsFocus < 4200;
            bool CRBoss1Refreshable = CRRemainsBoss1 < 4200;
            bool CRBoss2Refreshable = CRRemainsBoss2 < 4200;
            bool CRBoss3Refreshable = CRRemainsBoss3 < 4200;
            bool CRBoss4Refreshable = CRRemainsBoss4 < 4200 ;
            
            bool SLRefreshable = SLRemains < 4500 - GCD && Aimsharp.TargetExactCurrentHP() > AmountHP;
            bool SLFocusRefreshable = SLRemainsFocus < 4500;
            bool SLBoss1Refreshable = SLRemainsBoss1 < 4500;
            bool SLBoss2Refreshable = SLRemainsBoss2 < 4500;
            bool SLBoss3Refreshable = SLRemainsBoss3 < 4500;
            bool SLBoss4Refreshable = SLRemainsBoss4 < 4500;
            int SoulShard = Aimsharp.PlayerSecondaryPower();

            int CDSummonDarkglareRemains = Aimsharp.SpellCooldown("Summon Darkglare");
            int DotPhantomSingularityRemains = Aimsharp.DebuffRemaining("Phantom Singularity", "target");
            int DotVileTaintRemains = Aimsharp.DebuffRemaining("Vile Taint", "target");
            int BuffInevitableDemiseStack = Aimsharp.BuffStacks("Inevitable Demise", "player");

            bool UARefreshable = UARemains < 4800;

            int FlameFullRecharge = (int) (Aimsharp.RechargeTime("Concentrated Flame") - GCD +
                                           (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
            
            int DebuffConcentratedFlameRemaining = Aimsharp.DebuffRemaining("Concentrated Flame", "target");
           
            bool PetActive = Aimsharp.TotemTimer() > GCD;

            bool CastingUA = PlayerCastingID == 316099;
            

            /*
            int ChannelDuration = (int) ((4500f / (Haste + 1f)));
            int TickTimeChannel = ChannelDuration / 6;
            bool CancelTicks = IsChanneling && (PlayerCastingID == 15407 || PlayerCastingID == 48045) &&
                               (Aimsharp.CastingElapsed("player") <= TickTimeChannel);
            bool TwoTicksChanneled = (Aimsharp.CastingElapsed("player") >= (TickTimeChannel + 2));
            bool CancelChannel = (PlayerCastingID == 15407 &&
                                  TwoTicksChanneled && VoidBoltCD <= 0) ||
                                 (PlayerCastingID == 48045 && TwoTicksChanneled);
                                 
            */


            int CDMassDispel = Aimsharp.SpellCooldown("Mass Dispel");
            int CDDispersion = Aimsharp.SpellCooldown(("Dispersion"));
            int CDPsychicHorror = Aimsharp.SpellCooldown("Psychic Horror");
            int CDPsychicScream = Aimsharp.SpellCooldown("Psychic Scream");
            int CDS2M = Aimsharp.SpellCooldown("Surrender to Madness");

            bool MassDispel = Aimsharp.IsCustomCodeOn("MassDispel");
            bool JustEssences = Aimsharp.IsCustomCodeOn("JustEssences");
            bool BuffSurrenderToMadnessUp = Aimsharp.HasBuff("Surrender to Madness");
            bool MindControl = Aimsharp.IsCustomCodeOn("MindControl");
            bool DispelMagic = Aimsharp.IsCustomCodeOn("DispelMagic");
            bool S2M = Aimsharp.IsCustomCodeOn("S2M");
            bool PsychicScream = Aimsharp.IsCustomCodeOn("PsychicScream");
            bool PsychicHorror = Aimsharp.IsCustomCodeOn("PsychicHorror");
            bool Dispersion = Aimsharp.IsCustomCodeOn("Dispersion");







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
                    }


                }

                if (FirstLife == 0 && CurrentHP > 0) {
                    FirstLife = MaxHP;
                }
            }

            Aimsharp.PrintMessage("TTK: " + TimeSpan.FromMilliseconds(TTK).ToString());
            
            

            
            
            
            



            #endregion








            //Variable to switch between syncing cooldown usage to Power Infusion or Void Eruption depending whether priest_self_power_infusion is in use or we don't have power infusion learned.
            //actions+=/variable,name=pi_or_vf_sync_condition,op=set,value=(priest.self_power_infusion|runeforge.twins_of_the_sun_priestess.equipped)&level>=58&cooldown.power_infusion.up|(level<58|!priest.self_power_infusion&!runeforge.twins_of_the_sun_priestess.equipped)&cooldown.void_eruption.up
            int PlayerLevel = Aimsharp.GetPlayerLevel();
            




            #region Utillity

            // QUEUED MD
            if (CDMassDispel > 5000 && MassDispel) {
                Aimsharp.Cast("MassDispelOff");
                return true;
            }
            
            if (MassDispel && Aimsharp.CanCast("Mass Dispel", "player")) {
                Aimsharp.PrintMessage("Queued Mass Dispel");
                Aimsharp.Cast("MassDispel");
                return true;
            }
            
            // QUEUED DISPERSION
            if (CDDispersion > 5000 && Dispersion) {
                Aimsharp.Cast("DispersionOff");
                return true;
            }
            
            if (Dispersion && Aimsharp.CanCast("Dispersion", "player")) {
                Aimsharp.PrintMessage("Queued Dispersion");
                Aimsharp.Cast("Dispersion");
                return true;
            }
            
            // QUEUED DISPEL MAGIC
            if (Aimsharp.LastCast() == "Dispel Magic" && DispelMagic) {
                Aimsharp.Cast("DispelOff");
                return true;
            }
            
            if (DispelMagic && Aimsharp.CanCast("Dispel Magic")) {
                Aimsharp.PrintMessage("Queued Dispel Magic");
                Aimsharp.Cast("Dispel Magic");
                return true;
            }
            
            // QUEUED MC
            if (Aimsharp.LastCast() == "Mind Control" && MindControl) {
                Aimsharp.Cast("MindControlOff");
                return true;
            }
            
            if (MindControl && Aimsharp.CanCast("Mind Control", "target")) {
                Aimsharp.PrintMessage("Queued Mind Control");
                Aimsharp.Cast("Mind Control");
                return true;
            }
            
            // QUEUED Psychic Scream
            if (CDPsychicScream > 5000 && PsychicScream) {
                Aimsharp.Cast("PsychicScreamOff");
                return true;
            }
            
            if (PsychicScream && Aimsharp.CanCast("Psychic Scream", "player")) {
                Aimsharp.PrintMessage("Queued Psychic Scream");
                Aimsharp.Cast("Psychic Scream");
                return true;
            }
            
            // QUEUED Psychic Horror
            if (CDPsychicHorror > 5000 && PsychicHorror) {
                Aimsharp.Cast("PsychicHorrorOff");
                return true;
            }
            
            if (PsychicHorror && Aimsharp.CanCast("Psychic Horror", "target")) {
                Aimsharp.PrintMessage("Queued Psychic Horror");
                Aimsharp.Cast("Psychic Horror");
                return true;
            }
            
            // QUEUED S2m
            if (CDS2M > 5000 && S2M) {
                Aimsharp.Cast("S2MOff");
                return true;
            }
            
            if (S2M && Aimsharp.CanCast("Surrender to Madness")) {
                Aimsharp.PrintMessage("Queued S2M");
                Aimsharp.Cast("Surrender to Madness");
                return true;
            }

            if (Aimsharp.CanCast("Shadow Mend", "player")) {
                if (PlayerHealth <= GetSlider("Auto Shadow Mend Self @ HP%")) {
                    Aimsharp.Cast("Shadow Mend");
                    return true;
                }
            }
            
            if (Aimsharp.CanCast("Desperate Prayer", "player")) {
                if (PlayerHealth <= GetSlider("Auto Desperate Prayer @ HP%")) {
                    Aimsharp.Cast("Desperate Prayer");
                    return true;
                }
            }
            
            //Auto Healthstone
            if (Aimsharp.CanUseItem("Healthstone")) {
                if (PlayerHealth <= GetSlider("Auto Healthstone @ HP%")) {
                    Aimsharp.CanUseItem("Healthstone");
                    return true;
                }
            }


            if (Aimsharp.CanCast("Vampiric Embrace", "player") && Aimsharp.GroupSize() > 0) {
                if (PlayerHealth <= GetSlider("Auto Vampiric Embrace @ HP%")) {
                    Aimsharp.Cast("Vampiric Embrace");
                    return true;
                }
            }

            #endregion


            if (NoAOE) {
                EnemiesNearTarget = 1;
                EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
            }

            if (IsChanneling && PlayerCastingID != 198590) {
                return false;
            }


            if (Fighting) {
                
                //actions=phantom_singularity
                if (Aimsharp.CanCast("Phantom Singularity")) {
                    Aimsharp.Cast("Phantom Singularity");
                    return true;
                }
                
                //actions+=/vile_taint,if=soul_shard>1
                if (Aimsharp.CanCast("Vile Taint", "player") && (SoulShard > 10)) {
                    Aimsharp.Cast("Vile Taint");
                    return true;
                }
                
                //actions+=/siphon_life,if=refreshable
                if (Aimsharp.CanCast("Siphon Life") && SLRefreshable) {
                    Aimsharp.Cast("Siphon Life");
                    return true;
                }

                if (!CouncilDotsOff) {
                    if (Aimsharp.CanCast("Siphon Life", "focus") && SLFocusRefreshable) {
                        Aimsharp.Cast("SLFocus");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Siphon Life", "boss1") && SLBoss1Refreshable) {
                        Aimsharp.Cast("SLBoss1");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Siphon Life", "boss2") && SLBoss2Refreshable) {
                        Aimsharp.Cast("SLBoss2");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Siphon Life", "boss3") && SLBoss3Refreshable) {
                        Aimsharp.Cast("SLBoss3");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Siphon Life", "boss4") && SLBoss4Refreshable) {
                        Aimsharp.Cast("SLBoss4");
                        return true;
                    }
                }
                
                //actions+=/agony,if=refreshable
                if (Aimsharp.CanCast("Agony") && AGRefreshable) {
                    Aimsharp.Cast("Agony");
                    return true;
                }

                if (!CouncilDotsOff) {
                    if (Aimsharp.CanCast("Agony", "focus") && AGFocusRefreshable) {
                        Aimsharp.Cast("AgonyFocus");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Agony", "boss1") && AGBoss1Refreshable) {
                        Aimsharp.Cast("AgonyBoss1");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Agony", "boss2") && AGBoss2Refreshable) {
                        Aimsharp.Cast("AgonyBoss2");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Agony", "boss3") && AGBoss3Refreshable) {
                        Aimsharp.Cast("AgonyBoss3");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Agony", "boss4") && AGBoss4Refreshable) {
                        Aimsharp.Cast("AgonyBoss4");
                        return true;
                    }
                }
                
                //actions+=/unstable_affliction,if=refreshable
                if (Aimsharp.CanCast("Unstable Affliction") && UARefreshable && !CastingUA) {
                    Aimsharp.Cast("Unstable Affliction");
                    return true;
                }

                if (Aimsharp.CanCast("Seed of Corruption") && EnemiesNearTarget > 2) {
                    Aimsharp.Cast("Seed of Corruption");
                    return true;
                }
                
                //actions+=/corruption,if=refreshable
                if (Aimsharp.CanCast("Corruption") && CRRefreshable && EnemiesNearTarget < 3) {
                    Aimsharp.Cast("Corruption");
                    return true;
                }

                if (!CouncilDotsOff) {
                    if (Aimsharp.CanCast("Corruption", "focus") && CRFocusRefreshable) {
                        Aimsharp.Cast("CorruptionFocus");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Corruption", "boss1") && CRBoss1Refreshable) {
                        Aimsharp.Cast("CorruptionFocus");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Corruption", "boss2") && CRBoss2Refreshable) {
                        Aimsharp.Cast("CorruptionFocus");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Corruption", "boss3") && CRBoss3Refreshable) {
                        Aimsharp.Cast("CorruptionFocus");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Corruption", "boss4") && CRBoss4Refreshable) {
                        Aimsharp.Cast("CorruptionFocus");
                        return true;
                    }
                }

                //actions+=/haunt
                if (Aimsharp.CanCast("Haunt")) {
                    Aimsharp.Cast("Haunt");
                    return true;
                }
                
                //actions+=/call_action_list,name=darkglare_prep,if=cooldown.summon_darkglare.remains<2&(dot.phantom_singularity.remains>2|!talent.phantom_singularity.enabled)
                if (CDSummonDarkglareRemains < 2000 &&
                    (DotPhantomSingularityRemains > 2000 || !TalentPhantomSingularityEnabled) && !NoCooldowns) {
                    //actions.darkglare_prep=vile_taint
                    if (Aimsharp.CanCast("Vile Taint", "player")) {
                        Aimsharp.Cast("Vile Taint");
                        return true;
                    }
                    
                    //actions.darkglare_prep+=/dark_soul
                    if (Aimsharp.CanCast("Dark Soul")) {
                        Aimsharp.Cast("Dark Soul");
                        return true;
                    }
                    
                    //actions.darkglare_prep+=/potion
                    if (UsePotion) {
                        if (Aimsharp.CanUseItem(PotionType, false)) {
                            Aimsharp.Cast("potion", true);
                            return true;
                        }
                    }
                    
                    //actions.darkglare_prep+=/use_items
                    
                    //actions.darkglare_prep+=/fireblood
                    if (Aimsharp.CanCast("Fireblood")) {
                        Aimsharp.Cast("Fireblood");
                        return true;
                    }
                    
                    //actions.darkglare_prep+=/blood_fury
                    if (Aimsharp.CanCast("Blood Fury")) {
                        Aimsharp.Cast("Blood Fury");
                        return true;
                    }
                    
                    //actions.darkglare_prep+=/berserking
                    if (Aimsharp.CanCast("Berserking")) {
                        Aimsharp.Cast("Berserking");
                        return true;
                    }
                    
                    //actions.darkglare_prep+=/summon_darkglare
                    if (Aimsharp.CanCast("Summon Darkglare")) {
                        Aimsharp.Cast("Summon Darkglare");
                        return true;
                    }
                }

                //actions+=/dark_soul,if=cooldown.summon_darkglare.remains>time_to_die
                if (Aimsharp.CanCast("Dark Soul") && CDSummonDarkglareRemains > TTK && !NoCooldowns) {
                    Aimsharp.Cast("Dark Soul");
                    return true;
                }
                
                //actions+=/call_action_list,name=cooldowns
                
                /*
                actions.cooldowns=worldvein_resonance
                actions.cooldowns+=/memory_of_lucid_dreams
                actions.cooldowns+=/blood_of_the_enemy
                actions.cooldowns+=/guardian_of_azeroth
                actions.cooldowns+=/ripple_in_space
                actions.cooldowns+=/focused_azerite_beam
                actions.cooldowns+=/purifying_blast
                actions.cooldowns+=/reaping_flames
                actions.cooldowns+=/concentrated_flame
                actions.cooldowns+=/the_unbound_force,if=buff.reckless_force.remains
                */

                Aimsharp.PrintMessage("Soulshards" + SoulShard);

                //actions+=/malefic_rapture,if=dot.vile_taint.ticking
                if (Aimsharp.CanCast("Malefic Rapture", "player") && DotVileTaintRemains > GCD) {
                    Aimsharp.Cast("Malefic Rapture");
                    return true;
                }
                
                //actions+=/malefic_rapture,if=!talent.vile_taint.enabled
                if (Aimsharp.CanCast("Malefic Rapture", "player") && !TalentVileTaint) {
                    Aimsharp.Cast("Malefic Rapture");
                    return true;
                }
                
                //actions+=/drain_life,if=buff.inevitable_demise.stack>30
                if (Aimsharp.CanCast("Drain Life") && BuffInevitableDemiseStack > 30) {
                    Aimsharp.Cast("Drain Life");
                    return true;
                }
                
                //actions+=/drain_soul
                if (Aimsharp.CanCast("Drain Soul")) {
                    Aimsharp.Cast("Drain Soul");
                    return true;
                }
                
                //actions+=/shadow_bolt
                if (Aimsharp.CanCast("Shadow Bolt")) {
                    Aimsharp.Cast("Shadow Bolt");
                    return true;
                }
                    
                
                    
                
                
            }

			
            





            return false;
        }


        public override bool OutOfCombatTick() {
            bool IsMoving = Aimsharp.PlayerIsMoving();
            bool DebuffWeakenedSoulUp = Aimsharp.HasDebuff("Weakened Soul", "player");
            bool IsChanneling = Aimsharp.IsChanneling("player");
            bool StartCombat = Aimsharp.IsCustomCodeOn("StartCombat");
            int PlayerHealth = Aimsharp.Health("player");
            
            int CDMassDispel = Aimsharp.SpellCooldown("Mass Dispel");
            int CDDispersion = Aimsharp.SpellCooldown(("Dispersion"));
            int CDPsychicHorror = Aimsharp.SpellCooldown("Psychic Horror");
            int CDPsychicScream = Aimsharp.SpellCooldown("Psychic Scream");
            int CDS2M = Aimsharp.SpellCooldown("Surrender to Madness");

            bool MassDispel = Aimsharp.IsCustomCodeOn("MassDispel");
            bool JustEssences = Aimsharp.IsCustomCodeOn("JustEssences");
            bool BuffSurrenderToMadnessUp = Aimsharp.HasBuff("Surrender to Madness");
            bool MindControl = Aimsharp.IsCustomCodeOn("MindControl");
            bool DispelMagic = Aimsharp.IsCustomCodeOn("DispelMagic");
            bool S2M = Aimsharp.IsCustomCodeOn("S2M");
            bool PsychicScream = Aimsharp.IsCustomCodeOn("PsychicScream");
            bool PsychicHorror = Aimsharp.IsCustomCodeOn("PsychicHorror");
            bool Dispersion = Aimsharp.IsCustomCodeOn("Dispersion");

            if (!IsChanneling) {
                
                
                 #region UtillityOOC

            // QUEUED MD
            if (CDMassDispel > 5000 && MassDispel) {
                Aimsharp.Cast("MassDispelOff");
                return true;
            }
            
            if (MassDispel && Aimsharp.CanCast("Mass Dispel", "player")) {
                Aimsharp.PrintMessage("Queued Mass Dispel");
                Aimsharp.Cast("MassDispel");
                return true;
            }
            
            // QUEUED DISPERSION
            if (CDDispersion > 5000 && Dispersion) {
                Aimsharp.Cast("DispersionOff");
                return true;
            }
            
            if (Dispersion && Aimsharp.CanCast("Dispersion", "player")) {
                Aimsharp.PrintMessage("Queued Dispersion");
                Aimsharp.Cast("Dispersion");
                return true;
            }
            
            // QUEUED DISPEL MAGIC
            if (Aimsharp.LastCast() == "Dispel Magic" && DispelMagic) {
                Aimsharp.Cast("DispelOff");
                return true;
            }
            
            if (DispelMagic && Aimsharp.CanCast("Dispel Magic")) {
                Aimsharp.PrintMessage("Queued Dispel Magic");
                Aimsharp.Cast("Dispel Magic");
                return true;
            }
            
            // QUEUED MC
            if (Aimsharp.LastCast() == "Mind Control" && MindControl) {
                Aimsharp.Cast("MindControlOff");
                return true;
            }
            
            if (MindControl && Aimsharp.CanCast("Mind Control", "target")) {
                Aimsharp.PrintMessage("Queued Mind Control");
                Aimsharp.Cast("Mind Control");
                return true;
            }
            
            // QUEUED Psychic Scream
            if (CDPsychicScream > 5000 && PsychicScream) {
                Aimsharp.Cast("PsychicScreamOff");
                return true;
            }
            
            if (PsychicScream && Aimsharp.CanCast("Psychic Scream", "player")) {
                Aimsharp.PrintMessage("Queued Psychic Scream");
                Aimsharp.Cast("Psychic Scream");
                return true;
            }
            
            // QUEUED Psychic Horror
            if (CDPsychicHorror > 5000 && PsychicHorror) {
                Aimsharp.Cast("PsychicHorrorOff");
                return true;
            }
            
            if (PsychicHorror && Aimsharp.CanCast("Psychic Horror", "target")) {
                Aimsharp.PrintMessage("Queued Psychic Horror");
                Aimsharp.Cast("Psychic Horror");
                return true;
            }
            
            // QUEUED S2m
            if (CDS2M > 5000 && S2M) {
                Aimsharp.Cast("S2MOff");
                return true;
            }
            
            if (S2M && Aimsharp.CanCast("Surrender to Madness")) {
                Aimsharp.PrintMessage("Queued S2M");
                Aimsharp.Cast("Surrender to Madness");
                return true;
            }

            if (Aimsharp.CanCast("Shadow Mend", "player")) {
                if (PlayerHealth <= GetSlider("Auto Shadow Mend Self @ HP%")) {
                    Aimsharp.Cast("Shadow Mend");
                    return true;
                }
            }
            
            if (Aimsharp.CanCast("Desperate Prayer", "player")) {
                if (PlayerHealth <= GetSlider("Auto Desperate Prayer @ HP%")) {
                    Aimsharp.Cast("Desperate Prayer");
                    return true;
                }
            }
            
            //Auto Healthstone
            if (Aimsharp.CanUseItem("Healthstone")) {
                if (PlayerHealth <= GetSlider("Auto Healthstone @ HP%")) {
                    Aimsharp.CanUseItem("Healthstone");
                    return true;
                }
            }


            if (Aimsharp.CanCast("Vampiric Embrace", "player") && Aimsharp.GroupSize() > 0) {
                if (PlayerHealth <= GetSlider("Auto Vampiric Embrace @ HP%")) {
                    Aimsharp.Cast("Vampiric Embrace");
                    return true;
                }
            }

            #endregion


                if (!Aimsharp.HasBuff("Shadowform")) {
                    if (Aimsharp.CanCast("Shadowform", "player")) {
                        Aimsharp.Cast("Shadowform");
                        return true;
                    }
                }

                if (Aimsharp.CanCast("Power Word: Fortitude", "player") &&
                    !Aimsharp.HasBuff("Power Word: Fortitude", "player", false)) {
                    Aimsharp.Cast("Power Word: Fortitude");
                    return true;
                }

                if (!IsMoving && Aimsharp.CanCast("Agony") && StartCombat) {
                    Aimsharp.Cast("Agony");
                    return true;
                }
                
                

                if (IsMoving && !DebuffWeakenedSoulUp) {
                    Aimsharp.Cast("ShieldSelf");
                    return true;
                }

                if (IsMoving && Aimsharp.CanCast("Corruption") && StartCombat) {
                    Aimsharp.Cast("Corruption");
                    return true;
                }
            }

            return false;
        }
    }
    
    
    
    
    
    
    
}

    

