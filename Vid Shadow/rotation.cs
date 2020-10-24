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
    public class VidShadow : Rotation {


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

            List<string> Potions = new List<string>(new string[]
                {"Potion of Unbridled Fury", "Potion of Empowered Proximity", "Potion of Prolonged Power", "None"});
            Settings.Add(new Setting("Potion Type", Potions, "Potion of Unbridled Fury"));
            
            Settings.Add(new Setting("Auto Shadow Mend Self @ HP%", 0, 100, 25));

            Settings.Add(new Setting("Auto Vampiric Embrace @ HP%", 0, 100, 45));
            
            Settings.Add(new Setting("Auto Desperate Prayer @ HP%", 0, 100, 35));
            
            Settings.Add(new Setting("Auto Healthstone @ HP%", 0, 100, 25));
            
            Settings.Add(new Setting("Don't dot below HP%", 0, 100, 3));
            
            Settings.Add(new Setting("Don't dot below HP Amount", 0, 20000, 6000));

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
            Aimsharp.PrintMessage("/xxxxx CouncilDots", Color.Blue);
            Aimsharp.PrintMessage("--Will keep SW:P and VT up on focus and Boss 1-4.", Color.Blue);
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
            Spellbook.Add("Void Eruption");
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
            Spellbook.Add("Devouring Plague");
            Spellbook.Add("Power Word: Fortitude");
            Spellbook.Add("Shadow Mend");
            Spellbook.Add("Vampiric Embrace");
            Spellbook.Add("Searing Nightmare");
            Spellbook.Add("Power Word: Shield");
            Spellbook.Add("Power Infusion");
            Spellbook.Add("Mass Dispel");
            Spellbook.Add("Psychic Scream");
            Spellbook.Add("Dispel Magic");
            Spellbook.Add("Desperate Prayer");
            Spellbook.Add("Mind Control");
            Spellbook.Add("Dispersion");
            Spellbook.Add("Dispel Magic");
            Spellbook.Add("Psychic Horror");

            Buffs.Add("Bloodlust");
            Buffs.Add("Heroism");
            Buffs.Add("Time Warp");
            Buffs.Add("Ancient Hysteria");
            Buffs.Add("Netherwinds");
            Buffs.Add("Drums of Rage");
            Buffs.Add("Lifeblood");
            Buffs.Add("Harvested Thoughts");
            Buffs.Add("Voidform");
            Buffs.Add("Shadowform");
            Buffs.Add("Unfurling Darkness");
            Buffs.Add("Power Word: Fortitude");
            Buffs.Add("Fae Guardian");
            Buffs.Add("Surrender to Madness");

            Debuffs.Add("Shadow Word: Pain");
            Debuffs.Add("Vampiric Touch");
            Debuffs.Add("Shiver Venom");
            Debuffs.Add("Unfurling Darkness");
            Debuffs.Add("Weakened Soul");
            Debuffs.Add("Wrathful Faerie");
            Debuffs.Add("Concentrated Flame");


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

            Macros.Add("VTFocus", "/cast [@focus] Vampiric Touch");
            Macros.Add("VTBoss1", "/cast [@boss1] Vampiric Touch");
            Macros.Add("VTBoss2", "/cast [@boss2] Vampiric Touch");
            Macros.Add("VTBoss3", "/cast [@boss3] Vampiric Touch");
            Macros.Add("VTBoss4", "/cast [@boss4] Vampiric Touch");

            Macros.Add("SWPFocus", "/cast [@focus] Shadow Word: Pain");
            Macros.Add("SWPBoss1", "/cast [@boss1] Shadow Word: Pain");
            Macros.Add("SWPBoss2", "/cast [@boss2] Shadow Word: Pain");
            Macros.Add("SWPBoss3", "/cast [@boss3] Shadow Word: Pain");
            Macros.Add("SWPBoss4", "/cast [@boss4] Shadow Word: Pain");

            Macros.Add("ShieldSelf", "/cast [@player] Power Word: Shield");


            CustomCommands.Add("NoAOE");
            CustomCommands.Add("Prepull");
            CustomCommands.Add("Potions");
            CustomCommands.Add("SaveCooldowns");
            CustomCommands.Add("MassDispel");
            CustomCommands.Add("JustEssences");
            CustomCommands.Add("CouncilDots");
            CustomCommands.Add("S2M");
            CustomCommands.Add("DispelMagic");
            CustomCommands.Add("Dispersion");
            CustomCommands.Add("PsychicScream");
            CustomCommands.Add("PsychicHorror");
            CustomCommands.Add("MindControl");
            





        }
        
        





        // optional override for the CombatTick which executes while in combat
        public override bool CombatTick() {
            int GCD = Aimsharp.GCD();
            int Latency = Aimsharp.Latency;
            float Haste = Aimsharp.Haste() / 100f;
            int SWPRemains = Aimsharp.DebuffRemaining("Shadow Word: Pain", "target") - GCD;
            int SWPRemainsFocus = Aimsharp.DebuffRemaining("Shadow Word: Pain", "focus") - GCD;
            int SWPRemainsBoss1 = Aimsharp.DebuffRemaining("Shadow Word: Pain", "boss1") - GCD;
            int SWPRemainsBoss2 = Aimsharp.DebuffRemaining("Shadow Word: Pain", "boss2") - GCD;
            int SWPRemainsBoss3 = Aimsharp.DebuffRemaining("Shadow Word: Pain", "boss3") - GCD;
            int SWPRemainsBoss4 = Aimsharp.DebuffRemaining("Shadow Word: Pain", "boss4") - GCD;
            int VTRemains = Aimsharp.DebuffRemaining("Vampiric Touch", "target") - GCD;
            int VTRemainsFocus = Aimsharp.DebuffRemaining("Vampiric Touch", "focus") - GCD;
            int VTRemainsBoss1 = Aimsharp.DebuffRemaining("Vampiric Touch", "boss1") - GCD;
            int VTRemainsBoss2 = Aimsharp.DebuffRemaining("Vampiric Touch", "boss2") - GCD;
            int VTRemainsBoss3 = Aimsharp.DebuffRemaining("Vampiric Touch", "boss3") - GCD;
            int VTRemainsBoss4 = Aimsharp.DebuffRemaining("Vampiric Touch", "boss4") - GCD;
            int DVPRemains = Aimsharp.DebuffRemaining("Devouring Plague", "target") - GCD;
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
            bool DotsUp = SWPRemains > 0 && VTRemains > 0;
            bool AllDotsUp = DotsUp && DVPRemains > 0;
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
            bool CouncilDots = Aimsharp.IsCustomCodeOn("CouncilDots");

            bool SWPRefreshable = SWPRemains < 4800 && Aimsharp.TargetExactCurrentHP() > AmountHP && TargetHealth > PercentHP;
            bool SWPFocusRefreshable = SWPRemainsFocus < 4800;
            bool SWPBoss1Refreshable = SWPRemainsBoss1 < 4800;
            bool SWPBoss2Refreshable = SWPRemainsBoss2 < 4800;
            bool SWPBoss3Refreshable = SWPRemainsBoss3 < 4800;
            bool SWPBoss4Refreshable = SWPRemainsBoss4 < 4800;


            bool TalentMiseryEnabled = Aimsharp.Talent(3, 2);
            bool TalentDarkVoidEnabled = Aimsharp.Talent(3, 3);

            bool VTRefreshable = VTRemains < 6300 && Aimsharp.TargetExactCurrentHP() > AmountHP;
            bool VTFocusRefreshable = VTRemainsFocus < 6300;
            bool VTBoss1Refreshable = VTRemainsBoss1 < 6300;
            bool VTBoss2Refreshable = VTRemainsBoss2 < 6300;
            bool VTBoss3Refreshable = VTRemainsBoss3 < 6300;
            bool VTBoss4Refreshable = VTRemainsBoss4 < 6300;

            int FlameFullRecharge = (int) (Aimsharp.RechargeTime("Concentrated Flame") - GCD +
                                           (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
            bool LegacyEnabled = Aimsharp.Talent(7, 1);
            bool CooldownMindbenderUp = Aimsharp.SpellCooldown("Mindbender") <= 0;
            bool BuffVoidformUp = Aimsharp.HasBuff("Voidform");
            bool BuffDarkThoughtsUp = Aimsharp.SpellCharges("Mind Blast") > 1;
            bool BuffUnfurlingDarknessUp = Aimsharp.HasBuff("Unfurling Darkness");
            bool TalentPsychicLinkEnabled = Aimsharp.Talent(5, 2);
            bool TalentHungeringVoidEnabled = Aimsharp.Talent(7, 2);
            int DebuffConcentratedFlameRemaining = Aimsharp.DebuffRemaining("Concentrated Flame", "target");
            bool DebuffUnfurlingDarknessUp = Aimsharp.HasDebuff("Unfurling Darkness", "player");
            bool DebuffWeakenedSoulUp = Aimsharp.HasDebuff("Weakened Soul", "player");
            bool DebuffWrathfulFaerieUp = Aimsharp.HasDebuff("Wrathful Faerie", "target");
            bool CooldownVoidEruptionUp = Aimsharp.SpellCooldown("Void Eruption") <= 0;
            bool CooldownPowerInfusionUp = Aimsharp.SpellCooldown("Power Infusion") <= 0;
            bool BuffFaeGuardiansUp = Aimsharp.HasBuff("Fae Guardians", "player");
            bool PetActive = Aimsharp.TotemTimer() > GCD;
            int MindSearCutOff = 1;
            bool SelfPowerInfusion = true;
            bool TalenTwistOfFateEnabled = Aimsharp.Talent(3, 1);
            bool ChannelingMindFlay = PlayerCastingID == 15407;
            bool ChannelingMindSear = PlayerCastingID == 48045;
            bool CastingVampiricTouch = PlayerCastingID == 34914;

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
            
            

            
            
            
            



            #endregion







            bool LegendaryShadowflamePrismEquipped = false;
            bool LegendaryTwinsOfTheSunEquipped = false;

            //Variable to switch between syncing cooldown usage to Power Infusion or Void Eruption depending whether priest_self_power_infusion is in use or we don't have power infusion learned.
            //actions+=/variable,name=pi_or_vf_sync_condition,op=set,value=(priest.self_power_infusion|runeforge.twins_of_the_sun_priestess.equipped)&level>=58&cooldown.power_infusion.up|(level<58|!priest.self_power_infusion&!runeforge.twins_of_the_sun_priestess.equipped)&cooldown.void_eruption.up
            int PlayerLevel = Aimsharp.GetPlayerLevel();
            bool PiOrVe =
                ((SelfPowerInfusion || LegendaryTwinsOfTheSunEquipped) && PlayerLevel > 58 &&
                CooldownPowerInfusionUp ) ||
                ((PlayerLevel < 58 || !SelfPowerInfusion && !LegendaryTwinsOfTheSunEquipped) &&
                CooldownVoidEruptionUp);





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


            if (Fighting) {

                #region CWC

                
                if (IsChanneling && !ChannelingMindFlay && !ChannelingMindSear) {
                    return false;
                }

                if (IsChanneling && (ChannelingMindFlay || ChannelingMindSear)) {
                    if ((SearingNightmaresCutoff && !PiOrVe) || (SWPRefreshable && EnemiesNearTarget > 1) &&
                        Aimsharp.CanCast("Searing Nightmare", "player")) {
                        Aimsharp.Cast("Searing Nightmare");
                        return true;
                    }

                    if (TalentSearingNightmareEnabled && SWPRefreshable && EnemiesNearTarget > 2 &&
                        Aimsharp.CanCast("Searing Nightmare", "player")) {
                        Aimsharp.Cast("Searing Nightmare");
                        return true;
                    }



                    if ((Aimsharp.CanCast("Mind Blast", "target") || Aimsharp.SpellCharges("Mind Blast") >= 1) &&
                        (EnemiesNearTarget < 5 && !TalentSearingNightmareEnabled  || TalentSearingNightmareEnabled && BuffDarkThoughtsUp)) {
                        Aimsharp.Cast("Mind Blast");
                        return true;
                    }

                    
                }

                #endregion




                if (UsePotion) {
                    if (Aimsharp.CanUseItem(PotionType, false)) // don't check if equipped
                    {
                        if ((HasLust || TargetTimeToDie <= 80000 || TargetHealth < 35)) {
                            Aimsharp.Cast("potion", true);
                            return true;
                        }
                    }
                }



                if ((!IsMoving || BuffSurrenderToMadnessUp) && !NoCooldowns &&
                    Aimsharp.CanCast("Void Eruption", "player") && Insanity >= 40 && PiOrVe) {
                    Aimsharp.Cast("Void Eruption");
                    return true;
                }

                // Make sure you put up SW:P ASAP on the target if Wrathful Faerie isn't active.
                // actions.main+=/shadow_word_pain,if=buff.fae_guardians.up&!debuff.wrathful_faerie.up
                if (Aimsharp.CanCast("Shadow Word: Pain") && BuffFaeGuardiansUp && !DebuffWrathfulFaerieUp) {
                    Aimsharp.Cast("Shadow Word: Pain");
                    return true;
                }
                
                
                #region Cooldowns

                if (!NoCooldowns) {
                    if (Aimsharp.CanCast("Power Infusion") && BuffVoidformUp) {
                        Aimsharp.Cast("Void Form");
                        return true;
                    }

                    if (Aimsharp.CanCast("Mindgames") && Insanity < 90 && (AllDotsUp || VoidformUp)) {
                        Aimsharp.Cast("Mindgames");
                        return true;
                    }


                }

                #region Essences

                if (!NoCooldowns || JustEssences) {
                    if (MajorPower == "Memory of Lucid Dreams") {
                        if (Aimsharp.CanCast("Memory of Lucid Dreams", "player")) {
                            Aimsharp.Cast("Lucid Dreams");
                            return true;
                        }
                    }

                    if (MajorPower == "Blood of the Enemy") {
                        if (Aimsharp.CanCast("Blood of the Enemy", "player") && EnemiesInMelee >= 1) {
                            Aimsharp.Cast("Blood of the Enemy");
                            return true;
                        }
                    }

                    if (MajorPower == "Guardian of Azeroth") {
                        if (Aimsharp.CanCast("Guardian of Azeroth", "player")) {
                            Aimsharp.Cast("Guardian of Azeroth");
                            return true;
                        }
                    }

                    if (MajorPower == "Focused Azerite Beam") {
                        if (Aimsharp.CanCast("Focused Azerite Beam", "player") && EnemiesNearTarget >= 2) {
                            Aimsharp.Cast("Focused Azerite Beam");
                            return true;
                        }
                    }

                    if (MajorPower == "Purifying Blast") {
                        if (Aimsharp.CanCast("Purifying Blast", "player") && EnemiesNearTarget >= 2) {
                            Aimsharp.Cast("Purifying Blast");
                            return true;
                        }
                    }

                    if (MajorPower == "Concentrated Flame") {
                        if (Aimsharp.CanCast("Concentrated Flame") && FlameFullRecharge < GCD) {
                            Aimsharp.Cast("Concentrated Flame");
                            return true;
                        }
                    }
                }

                #endregion

                if (Aimsharp.CanUseItem(TopTrinket)) {
                    Aimsharp.Cast(TopTrinket, true);
                    return true;
                }

                if (Aimsharp.CanUseItem(BotTrinket)) {
                    Aimsharp.Cast(BotTrinket, true);
                }

                if (Aimsharp.CanUseTrinket(0) && TopTrinket == "Generic") {

                    Aimsharp.Cast("TopTrink", true);
                    return true;

                }

                if (Aimsharp.CanUseTrinket(1) && BotTrinket == "Generic") {

                    Aimsharp.Cast("BotTrink", true);
                    return true;

                }



                #endregion
                
                //High Priority Mind Sear action to refresh DoTs with Searing Nightmare
                //actions.main+=/mind_sear,target_if=talent.searing_nightmare.enabled&spell_targets.mind_sear>(variable.mind_sear_cutoff+1)&!dot.shadow_word_pain.ticking&!cooldown.mindbender.up
                if ((!IsMoving || BuffSurrenderToMadnessUp) && Aimsharp.CanCast("Mind Sear", "target") &&
                    EnemiesNearTarget > (MindSearCutOff + 1) && TalentSearingNightmareEnabled &&
                    SWPRemains <= 0 && !CooldownMindbenderUp && !ChannelingMindSear) {
                    Aimsharp.Cast("Mind Sear");
                    return true;
                }



                if (Aimsharp.CanCast("Damnation") && !AllDotsUp) {
                    Aimsharp.Cast("Damnation");
                    return true;
                }
                
                //Use Void Bolt at higher priority with Hungering Void up to 4 targets, or other talents on ST.
                //actions.main+=/void_bolt,if=insanity<=85&((talent.hungering_void.enabled&spell_targets.mind_sear<5)|spell_targets.mind_sear=1)
                if (Aimsharp.CanCast("Void Bolt") && VoidformUp && (Insanity <= 85 &&
                                                                    ((TalentHungeringVoidEnabled &&
                                                                      EnemiesNearTarget < 5) ||
                                                                     EnemiesNearTarget == 1))) {
                    Aimsharp.Cast("Void Bolt");
                }

                //Don't use Devouring Plague if you can get into Voidform instead, or if Searing Nightmare is talented and will hit enough targets.
                //actions.main+=/devouring_plague,target_if=(refreshable|insanity>75)&!variable.pi_or_vf_sync_condition&(!talent.searing_nightmare.enabled|(talent.searing_nightmare.enabled&!variable.searing_nightmare_cutoff))
                if (Aimsharp.CanCast("Devouring Plague") &&  Insanity > 75 && (!PiOrVe || PlayerLevel < 23) &&
                    (!TalentSearingNightmareEnabled || (TalentSearingNightmareEnabled && !SearingNightmaresCutoff))) {
                    Aimsharp.Cast("Devouring Plague");
                    return true;
                }

                // Use VB on CD if you don't need to cast Devouring Plague, and there are less than 4 targets out (5 with conduit).
                // actions.main+=/void_bolt,if=spell_targets.mind_sear<(4+conduit.dissonant_echoes.enabled)&insanity<=85
                if (Aimsharp.CanCast("Void Bolt") && EnemiesNearTarget < 4 && Insanity <= 85 && VoidformUp) {
                    Aimsharp.Cast("Void Bolt");
                    return true;
                }
                
                //Use Shadow Word: Death if the target is about to die or you have Shadowflame Prism equipped with Mindbender or Shadowfiend active.
                //actions.main+=/shadow_word_death,target_if=(target.health.pct<20&spell_targets.mind_sear<4)|(pet.fiend.active&runeforge.shadowflame_prism.equipped)
                if (Aimsharp.CanCast("Shadow Word: Death") && (TargetHealth <= 20 && EnemiesNearTarget < 4) ||
                    (PetActive && LegendaryShadowflamePrismEquipped)) {
                    Aimsharp.Cast("Shadow Word: Death");
                    return true;
                }
                
                //actions.main+=/surrender_to_madness,target_if=target.time_to_die<25&buff.voidform.down
                if (Aimsharp.CanCast("Surrender to Madness") && TTK < 25000 && !BuffVoidformUp) {
                    Aimsharp.Cast("Surrender to Madness");
                    return true;
                }

                //actions.main+=/mindbender,if=dot.vampiric_touch.ticking&((talent.searing_nightmare.enabled&spell_targets.mind_sear>(variable.mind_sear_cutoff+1))|dot.shadow_word_pain.ticking)
                if (!NoCooldowns && Aimsharp.CanCast("Shadowfiend") && VTRemains > 0 &&
                    ((TalentSearingNightmareEnabled && (EnemiesNearTarget > (MindSearCutOff + 1))) || SWPRemains > 0)) {
                    Aimsharp.Cast("Shadowfiend");
                    return true;
                }

                //Use Void Torrent only if SW:P and VT are active and the target won't die during the channel.
                //actions.main+=/void_torrent,target_if=variable.dots_up&target.time_to_die>4&buff.voidform.down&spell_targets.mind_sear<(5+(6*talent.twist_of_fate.enabled))
                if ((!IsMoving || BuffSurrenderToMadnessUp) && TTK > 4 && Aimsharp.CanCast("Void Torrent") && AllDotsUp &&
                    !BuffVoidformUp &&
                    EnemiesNearTarget < (5 + (6 * (TalenTwistOfFateEnabled ? 1 : 0)))) {
                    Aimsharp.Cast("Void Torrent");
                    return true;
                }

                // Use SW:D with Painbreaker Psalm unless the target will be below 20% before the cooldown comes back
                // actions.main+=/shadow_word_death,if=runeforge.painbreaker_psalm.equipped&variable.dots_up&target.time_to_pct_20>(cooldown.shadow_word_death.duration+gcd)


                //Use Mind Sear to consume Dark Thoughts procs on AOE. TODO Confirm is this is a higher priority than redotting on AOE unless dark thoughts is about to time out
                //actions.main+=/mind_sear,target_if=spell_targets.mind_sear>variable.mind_sear_cutoff&buff.dark_thoughts.up,chain=1,interrupt_immediate=1,interrupt_if=ticks>=2
                if ((!IsMoving || BuffSurrenderToMadnessUp) &&
                    EnemiesNearTarget > MindSearCutOff &&
                    BuffDarkThoughtsUp && Aimsharp.CanCast("Mind Sear") &&
                    !ChannelingMindSear) {
                    Aimsharp.Cast("Mind Sear");
                    return true;
                }

                //Use Mind Flay to consume Dark Thoughts procs on ST. TODO Confirm if this is a higher priority than redotting unless dark thoughts is about to time out
                //actions.main+=/mind_flay,if=buff.dark_thoughts.up&variable.dots_up,chain=1,interrupt_immediate=1,interrupt_if=ticks>=2&cooldown.void_bolt.up
                if ((!IsMoving || BuffSurrenderToMadnessUp) && BuffDarkThoughtsUp && DotsUp &&
                    EnemiesNearTarget <= MindSearCutOff && Aimsharp.CanCast("Mind Flay") &&
                    !ChannelingMindFlay ) {
                    Aimsharp.Cast("Mind Flay");
                    return true;
                }

                //actions.main+=/mind_blast,if=variable.dots_up&raid_event.movement.in>cast_time+0.5&spell_targets.mind_sear<4
                if ((!IsMoving || BuffSurrenderToMadnessUp) && Aimsharp.CanCast("Mind Blast") && DotsUp &&
                    EnemiesNearTarget < 4) {
                    Aimsharp.Cast("Mind Blast");
                    return true;
                }

                #region Dots
                
                if ((!IsMoving || BuffSurrenderToMadnessUp) && TTK > 6000 && Aimsharp.CanCast("Vampiric Touch") && !CastingVampiricTouch &&
                    (VTRefreshable || (TalentMiseryEnabled && SWPRefreshable) || BuffUnfurlingDarknessUp)) {
                    
                    Aimsharp.Cast("Vampiric Touch");
                    return true;
                }
                



                //Special condition to stop casting SW:P on off-targets when fighting 3 or more stacked mobs and using Psychic Link and NOT Misery.
                //actions.main+=/shadow_word_pain,if=refreshable&target.time_to_die>4&!talent.misery.enabled&talent.psychic_link.enabled&spell_targets.mind_sear>2
                if (Aimsharp.CanCast("Shadow Word: Pain") && TTK > 4000 && (SWPRefreshable && !TalentMiseryEnabled &&
                                                                         TalentPsychicLinkEnabled && EnemiesNearTarget > 2)) {
                    Aimsharp.Cast("Shadow Word: Pain");
                    return true;
                }

                //Keep SW:P up on as many targets as possible, except when fighting 3 or more stacked mobs with Psychic Link.
                //actions.main+=/shadow_word_pain,target_if=refreshable&target.time_to_die>4&!talent.misery.enabled&!(talent.searing_nightmare.enabled&spell_targets.mind_sear>(variable.mind_sear_cutoff+1))&(!talent.psychic_link.enabled|(talent.psychic_link.enabled&spell_targets.mind_sear<=2))


                if (Aimsharp.CanCast("Shadow Word: Pain") && TTK > 4000 && (SWPRefreshable && !TalentMiseryEnabled &&
                                                              !(TalentSearingNightmareEnabled &&
                                                                EnemiesNearTarget > (MindSearCutOff + 1)) &&
                                                              (!TalentPsychicLinkEnabled || (TalentPsychicLinkEnabled &&
                                                                  EnemiesNearTarget <= 2)))) {
                    Aimsharp.Cast("Shadow Word: Pain");
                    return true;
                }

                #region Council + Focus

                if (CouncilDots) {
                    if (!Aimsharp.TargetIsUnit("focus") && Aimsharp.Range("focus") < 40) {
                        if (!CastingVampiricTouch && (!IsMoving || BuffSurrenderToMadnessUp) &&
                            Aimsharp.CanCast("Vampiric Touch", "focus") &&
                            (VTFocusRefreshable || (TalentMiseryEnabled && SWPFocusRefreshable))) {
                            Aimsharp.Cast("VTFocus");

                            return true;
                        }

                        if (Aimsharp.CanCast("Shadow Word: Pain", "focus") &&
                            (SWPFocusRefreshable && !TalentMiseryEnabled)) {
                            Aimsharp.Cast("SWPFocus");
                            return true;
                        }
                    }

                    if (!Aimsharp.TargetIsUnit("boss1") && Aimsharp.Range("boss1") < 40) {
                        if (!CastingVampiricTouch && (!IsMoving || BuffSurrenderToMadnessUp) &&
                            Aimsharp.CanCast("Vampiric Touch", "boss1") &&
                            (VTBoss1Refreshable || (TalentMiseryEnabled && SWPBoss1Refreshable))) {
                            Aimsharp.Cast("VTBoss1");
                            return true;
                        }

                        if (Aimsharp.CanCast("Shadow Word: Pain", "boss1") &&
                            (SWPBoss1Refreshable && !TalentMiseryEnabled)) {
                            Aimsharp.Cast("SWPBoss1");
                            return true;
                        }
                    }

                    if (!Aimsharp.TargetIsUnit("boss2") && Aimsharp.Range("boss2") < 40) {
                        if (!CastingVampiricTouch && (!IsMoving || BuffSurrenderToMadnessUp) &&
                            Aimsharp.CanCast("Vampiric Touch", "boss2") &&
                            (VTBoss2Refreshable || (TalentMiseryEnabled && SWPBoss2Refreshable))) {
                            Aimsharp.Cast("VTBoss2");
                            return true;
                        }

                        if (Aimsharp.CanCast("Shadow Word: Pain", "boss2") &&
                            (SWPBoss2Refreshable && !TalentMiseryEnabled)) {
                            Aimsharp.Cast("SWPBoss2");
                            return true;
                        }
                    }

                    if (!Aimsharp.TargetIsUnit("boss3") && Aimsharp.Range("boss3") < 40) {
                        if (!CastingVampiricTouch && (!IsMoving || BuffSurrenderToMadnessUp) &&
                            Aimsharp.CanCast("Vampiric Touch", "boss1") &&
                            (VTBoss3Refreshable || (TalentMiseryEnabled && SWPBoss3Refreshable))) {
                            Aimsharp.Cast("VTBoss3");
                            return true;
                        }

                        if (Aimsharp.CanCast("Shadow Word: Pain", "boss2") &&
                            (SWPBoss3Refreshable && !TalentMiseryEnabled)) {
                            Aimsharp.Cast("SWPBoss3");
                            return true;
                        }
                    }

                    if (!Aimsharp.TargetIsUnit("boss4") && Aimsharp.Range("boss4") < 40) {
                        if (!CastingVampiricTouch && (!IsMoving || BuffSurrenderToMadnessUp) &&
                            Aimsharp.CanCast("Vampiric Touch", "boss1") &&
                            (VTBoss4Refreshable || (TalentMiseryEnabled && SWPBoss4Refreshable))) {
                            Aimsharp.Cast("VTBoss4");
                            return true;
                        }

                        if (Aimsharp.CanCast("Shadow Word: Pain", "boss4") &&
                            (SWPBoss4Refreshable && !TalentMiseryEnabled)) {
                            Aimsharp.Cast("SWPBoss4");
                            return true;
                        }
                    }
                }

                #endregion

                



                #endregion

                if ((!IsMoving || BuffSurrenderToMadnessUp) &&
                    EnemiesNearTarget > MindSearCutOff && 
                    Aimsharp.CanCast("Mind Sear", "target") &&
                    !ChannelingMindSear) {
                    Aimsharp.Cast("Mind Sear");
                    return true;
                }


                if ((!IsMoving || BuffSurrenderToMadnessUp) &&  EnemiesNearTarget < 2 && 
                    Aimsharp.CanCast("Mind Flay", "target") &&
                    !ChannelingMindFlay) {
                    Aimsharp.Cast("Mind Flay");
                    return true;
                }

                if (IsMoving) {
                    if (!DebuffWeakenedSoulUp) {
                        Aimsharp.Cast("ShieldSelf");
                        return true;
                    }

                    if (Aimsharp.CanCast("Shadow Word: Death")) {
                        Aimsharp.Cast("Shadow Word: Death");
                        return true;
                    }
                    
                    if (Aimsharp.CanCast("Shadow Word: Pain")) {
                        Aimsharp.Cast("Shadow Word: Pain");
                        return true;
                    }
                }
            }

			if (!DebuffWeakenedSoulUp && IsMoving) {
                 Aimsharp.Cast("ShieldSelf");
                 return true;
            }
            





            return false;
        }


        public override bool OutOfCombatTick() {
            bool IsMoving = Aimsharp.PlayerIsMoving();
            bool DebuffWeakenedSoulUp = Aimsharp.HasDebuff("Weakened Soul", "player");
            bool IsChanneling = Aimsharp.IsChanneling("player");

            if (!IsChanneling) {


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

                if (IsMoving && !DebuffWeakenedSoulUp) {
                    Aimsharp.Cast("ShieldSelf");
                    return true;
                }
            }

            return false;
        }
    }
    
    
    
    
    
    
    
}

    


