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
            Buffs.Add("Empowered Rune Weapon");
            Buffs.Add("Cold Heart");
            Buffs.Add("Unholy Strength");
            Buffs.Add("Reckless Force");
            Buffs.Add("Seething Rage");
            

            #endregion

            #region Debuffs

            Debuffs.Add("Frost Fever");
            Debuffs.Add("Razor Ice");

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
            int RuneDeficit = 6 - Runes;
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

            #region CooldownsRemaining

            int CooldownBoSRemains = Aimsharp.SpellCooldown("Breath of Sindragosa");
            int CooldownEmpowerRuneWeaponRemains = Aimsharp.SpellCooldown("Empowered Rune Weapon");
            int CooldownPillarOfFrostRemains = Aimsharp.SpellCooldown("Pillar of Frost");
            int CooldownFrostwyrmsFuryRemains = Aimsharp.SpellCooldown("Frostwyrm's Fury");
            bool CooldownFrostwyrmsFuryReady = CooldownFrostwyrmsFuryRemains <= 0;
            int CooldownPillarOfFrostRemaining = Aimsharp.SpellCooldown("Pillar of Frost");
            bool CooldownPillarOfFrostReady = CooldownPillarOfFrostRemaining <= 0;

            #endregion

            #region Durations

            int DebuffFrostFeverRemains = Aimsharp.DebuffRemaining("Frost Fever", "target");
            bool DebuffFrostFeverUp = DebuffFrostFeverRemains > 0;

            int BuffIcyTalonsRemains = Aimsharp.BuffRemaining("Icy Talons", "player");
            bool BuffIcyTalonsUp = BuffIcyTalonsRemains > 0;

            int BuffPillarOfFrostRemains = Aimsharp.BuffRemaining("Pillar of Frost", "player");
            bool BuffPillarOfFrostUp = BuffPillarOfFrostRemains > 0;

            int BuffBreathOfSindragosaRemains = Aimsharp.BuffRemaining("Breath of Sindragosa", "player");
            bool BuffBreathOfSindragosaUp = BuffBreathOfSindragosaRemains > 0;

            int BuffColdHeartStack = Aimsharp.BuffStacks("Cold Heart", "player");

            int BuddIcyCitadelRemains = Aimsharp.BuffRemaining("Icy Citadel", "player");
            bool BuffIcyCitadelUp = BuddIcyCitadelRemains > 0;

            int BuffRecklessForceStacks = Aimsharp.BuffStacks("Reckless Force", "player");
            int BuffRecklessForceRemains = Aimsharp.BuffRemaining("Reckless Force", "player");
            bool BuffRecklessForceUp = BuffRecklessForceStacks > 0;

            int BuffSeethingRageRemains = Aimsharp.BuffRemaining("Seething Rage", "player");
            bool BuffSeethingRageUp = BuffSeethingRageRemains > 0;

            int BuffUnholyStrengthRemains = Aimsharp.BuffRemaining("Unholy Strength", "player");
            bool BuffUnholyStrengthUp = BuffUnholyStrengthRemains > 0;

            int BuffEmpowerRuneWeaponRemaining = Aimsharp.BuffRemaining("Empower Rune Weapon", "player");
            bool BuffEmpowerRuneWeaponUp = BuffEmpowerRuneWeaponRemaining > 0;

            int DebuffRazorIceStack = Aimsharp.DebuffStacks("Razor Ice", "target", true);

			int BuffRimeRemaining = Aimsharp.BuffRemaining("Rime");

			


            #endregion

            #region CanCasts

            bool CanCastHowlingBlast = Aimsharp.TimeUntilRunes(1) <= GCD || BuffRimeRimeRemaining > GCD;
            bool CanCastGlacialAdvance = Aimsharp.SpellCooldown("Glacial Advance") <= GCD && RunicPower >=30;
            bool CanCastFrostStrike = RunicPower >= 25;
            bool CanCastObliterate = Aimsharp.TimeUntilRunes(2) <= GCD;
            bool CanCastChainsOfIce = Aimsharp.TimeUntilRunes(1) <= GCD;
            

            #endregion

            #region Rotation

            if (Fighting) {
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

                if (BuffIcyTalonsRemains <= GCD && BuffIcyTalonsUp &&
                    (!TalentBoSEnabled || CooldownBoSRemains > 15000) &&
                    CanCastFrostStrike) {
                    Aimsharp.Cast("Frost Strike");
                    return true;
                }

                #endregion

                #region Essences

                if (!NoCooldown || AlwaysEssence) {
                    if (MajorPower == "Blood of the Enemy" && EnemiesInMelee > 0) {
                        if (BuffPillarOfFrostUp &&
                            (BuffPillarOfFrostRemains < 10000 &&
                             (BuffBreathOfSindragosaUp || !TalentBoSEnabled && !AzeriteIcyCitadelEnabled) ||
                             BuffIcyCitadelUp && TalentIcecapEnabled) &&
                            (EnemiesInMelee == 1 || !TalentIcecapEnabled) ||
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

                    //actions.essences+=/chill_streak,if=buff.pillar_of_frost.remains<5&buff.pillar_of_frost.up|target.1.time_to_die<5
                    if (MajorPower == "Chill Streak") {
                        if (BuffPillarOfFrostRemains < 5000 && (BuffPillarOfFrostUp || TargetHealth < 4) &&
                            Aimsharp.CanCast("Chill Streak")) {
                            Aimsharp.Cast("Chill Streak");
                            return true;
                        }
                    }

                    //actions.essences+=/the_unbound_force,if=buff.reckless_force.up|buff.reckless_force_counter.stack<11
                    if (MajorPower == "The Unbound Force") {
                        if ((BuffRecklessForceUp || BuffRecklessForceStacks < 11) &&
                            Aimsharp.CanCast("The Unbound Force")) {
                            Aimsharp.Cast("The Unbound Force");
                            return true;
                        }
                    }

                    //actions.essences+=/focused_azerite_beam,if=!buff.pillar_of_frost.up&!buff.breath_of_sindragosa.up
                    if (MajorPower == "Focused Azerite Beam") {
                        if (!BuffBreathOfSindragosaUp && !BuffBreathOfSindragosaUp &&
                            Aimsharp.CanCast("Focused Azerite Beam", "player")) {
                            Aimsharp.Cast("Focused Azerite Beam");
                            return true;
                        }
                    }

                    //actions.essences+=/concentrated_flame,if=!buff.pillar_of_frost.up&!buff.breath_of_sindragosa.up&dot.concentrated_flame_burn.remains=0
                    if (MajorPower == "Concentrated Flame") {
                        if (Aimsharp.CanCast("Concentrated Flame") && !BuffPillarOfFrostUp &&
                            !BuffBreathOfSindragosaUp &&
                            !DotConcentratedFlameUp) {
                            Aimsharp.Cast("Concentrated Flame");
                            return true;
                        }
                    }

                    //actions.essences+=/purifying_blast,if=!buff.pillar_of_frost.up&!buff.breath_of_sindragosa.up
                    if (MajorPower == "Purifying Blast") {
                        if (Aimsharp.CanCast("Purifying Blast") && !BuffPillarOfFrostUp && !BuffBreathOfSindragosaUp) {
                            Aimsharp.Cast("Purifying Blast");
                            return true;
                        }
                    }

                    //actions.essences+=/worldvein_resonance,if=buff.pillar_of_frost.up|buff.empower_rune_weapon.up|cooldown.breath_of_sindragosa.remains>60+15|equipped.ineffable_truth|equipped.ineffable_truth_oh

                    //actions.essences+=/ripple_in_space,if=!buff.pillar_of_frost.up&!buff.breath_of_sindragosa.up

                    //actions.essences+=/memory_of_lucid_dreams,if=buff.empower_rune_weapon.remains<5&buff.breath_of_sindragosa.up|(rune.time_to_2>gcd&runic_power<50)

                    if (MajorPower == "Reaping Flames") {
                        if (Aimsharp.CanCast("Reaping Flames")) {
                            Aimsharp.Cast("Reaping Flames");
                            return true;
                        }
                    }


                }

                #endregion

                #region Cooldowns

                if (!NoCooldown) {
                    #region Racials

                    
                    //actions.cooldowns+=/blood_fury,if=buff.pillar_of_frost.up&buff.empower_rune_weapon.up
                    if (Aimsharp.CanCast("Blood Fury", "player") && BuffPillarOfFrostUp && BuffEmpowerRuneWeaponUp) {
                        Aimsharp.Cast("Blood Fury");
                        return true;
                    }
                    
                    //actions.cooldowns+=/berserking,if=buff.pillar_of_frost.up
                    if (Aimsharp.CanCast("Berserking", "player") && BuffPillarOfFrostUp) {
                        Aimsharp.Cast("Berserking");
                        return true;
                    }
                    
                    //actions.cooldowns+=/arcane_pulse,if=(!buff.pillar_of_frost.up&active_enemies>=2)|!buff.pillar_of_frost.up&(rune.deficit>=5&runic_power.deficit>=60)
                    if (Aimsharp.CanCast("Arcane Pulse", "player") && ((!BuffPillarOfFrostUp && EnemiesInMelee >= 2) ||
                                                             !BuffPillarOfFrostUp &&
                                                             (RuneDeficit >= 50 && RunicPowerDeficit >= 60))) {
                        Aimsharp.Cast("Arcane Pulse");
                        return true;
                    }
                    
                    //actions.cooldowns+=/lights_judgment,if=buff.pillar_of_frost.up
                    if (Aimsharp.CanCast("Light's Judgement", "player") && BuffPillarOfFrostUp) {
                        Aimsharp.Cast("Light's Judgement");
                        return true;
                    }
                    
                    //actions.cooldowns+=/ancestral_call,if=buff.pillar_of_frost.up&buff.empower_rune_weapon.up
                    
                    //actions.cooldowns+=/fireblood,if=buff.pillar_of_frost.remains<=8&buff.empower_rune_weapon.up

                    //actions.cooldowns+=/bag_of_tricks,if=buff.pillar_of_frost.up&(buff.pillar_of_frost.remains<5&talent.cold_heart.enabled|!talent.cold_heart.enabled&buff.pillar_of_frost.remains<3)&active_enemies=1|buff.seething_rage.up&active_enemies=1

                    #endregion

                    #region FrostCooldowns

                    //actions.cooldowns+=/pillar_of_frost,if=(cooldown.empower_rune_weapon.remains|talent.icecap.enabled)&!buff.pillar_of_frost.up
                    if (Aimsharp.CanCast("Pillar of Frost", "player") && (CooldownEmpowerRuneWeaponRemains > 0 || TalentIcecapEnabled) && !BuffPillarOfFrostUp) {
                        Aimsharp.Cast("Pillar of Frost");
                        return true;
                    }

                    //actions.cooldowns+=/breath_of_sindragosa,use_off_gcd=1,if=cooldown.empower_rune_weapon.remains&cooldown.pillar_of_frost.remains
                    if (Aimsharp.CanCast("Breath of Sindragosa", "player") && CooldownEmpowerRuneWeaponRemains > 0 &&
                        CooldownPillarOfFrostRemains > 0 && GCD > 0) {
                        Aimsharp.Cast("Breath of Sindragosa");
                        return true;
                    }

                    //actions.cooldowns+=/empower_rune_weapon,if=cooldown.pillar_of_frost.ready&talent.obliteration.enabled&rune.time_to_5>gcd&runic_power.deficit>=10|target.1.time_to_die<20
                    if (Aimsharp.CanCast("Empower Rune Weapon", "player") && CooldownPillarOfFrostReady &&
                        TalentObliterationEnabled && Aimsharp.TimeUntilRunes(5) > GCD && RuneDeficit >= 10) {
                        Aimsharp.Cast("Empower Rune Weapon");
                        return true;
                    }
                    
                    //actions.cooldowns+=/empower_rune_weapon,if=(cooldown.pillar_of_frost.ready|target.1.time_to_die<20)&talent.breath_of_sindragosa.enabled&runic_power>60
                    if (Aimsharp.CanCast("Empower Rune Weapon", "player") && CooldownPillarOfFrostReady &&
                        TalentBreathOfSindragosaEnabled && RunicPower > 60) {
                        Aimsharp.Cast("Empower Rune Weapon");
                        return true;
                    }
                    
                    //actions.cooldowns+=/empower_rune_weapon,if=talent.icecap.enabled&rune<3
                    if (Aimsharp.CanCast("Empower Rune Weapon", "player") && TalentIcecapEnabled && Runes < 30) {
                        Aimsharp.Cast("Empower Rune Weapon");
                        return true;
                    }

                    #region ColdHeart
                    //actions.cooldowns+=/call_action_list,name=cold_heart,if=talent.cold_heart.enabled&((buff.cold_heart.stack>=10&debuff.razorice.stack=5)|target.1.time_to_die<=gcd)
                    if (TalentColdHeartEnabled && (BuffColdHeartStack >= 10 && DebuffRazorIceStack == 5)) {
                        //actions.cold_heart=chains_of_ice,if=buff.cold_heart.stack>5&target.1.time_to_die<gcd
                        //actions.cold_heart+=/chains_of_ice,if=(buff.seething_rage.remains<gcd)&buff.seething_rage.up
                        if (CanCastChainsOfIce && (BuffSeethingRageRemains < GCD) &&
                            BuffSeethingRageUp) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=(buff.pillar_of_frost.remains<=gcd*(1+cooldown.frostwyrms_fury.ready)|buff.pillar_of_frost.remains<rune.time_to_3)&buff.pillar_of_frost.up&(azerite.icy_citadel.rank<=1|buff.breath_of_sindragosa.up)&!talent.icecap.enabled
                        if ( CanCastChainsOfIce &&
                            ((BuffPillarOfFrostRemains <= GCD * (1 + CooldownFrostwyrmsFuryReady ? 1 : 0) ||
                             BuffPillarOfFrostRemains < Aimsharp.TimeUntilRunes(3)) && BuffPillarOfFrostUp &&
                            (AzeriteIcyCitadelRank <= 1 || BuffBreathOfSindragosaUp) && !TalentIcecapEnabled)) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=buff.pillar_of_frost.remains<8&buff.unholy_strength.remains<gcd*(1+cooldown.frostwyrms_fury.ready)&buff.unholy_strength.remains&buff.pillar_of_frost.up&(azerite.icy_citadel.rank<=1|buff.breath_of_sindragosa.up)&!talent.icecap.enabled
                        if (CanCastChainsOfIce && (BuffPillarOfFrostRemains < 8 &&
                                        BuffUnholyStrengthRemains <
                                        GCD * (1 + CooldownFrostwyrmsFuryReady ? 1 : 0) &&
                                        BuffUnholyStrengthRemains > 0 &&
                                        BuffPillarOfFrostUp &&
                                        (AzeriteIcyCitadelRank <= 1 ||
                                         BuffBreathOfSindragosaUp) && !TalentIcecapEnabled)) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=(buff.icy_citadel.remains<4|buff.icy_citadel.remains<rune.time_to_3)&buff.icy_citadel.up&azerite.icy_citadel.rank>=2&!buff.breath_of_sindragosa.up&!talent.icecap.enabled
                        if(CanCastChainsOfIce && ((BuddIcyCitadelRemains < 4000 || BuddIcyCitadelRemains < Aimsharp.TimeUntilRunes(3)) BuffIcyCitadelUp && AzeriteIcyCitadelRank >=2 && !BuffBreathOfSindragosaUp && !TalentIcecapEnabled))
                        {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                            
                        

                        //actions.cold_heart+=/chains_of_ice,if=buff.icy_citadel.up&buff.unholy_strength.up&azerite.icy_citadel.rank>=2&!buff.breath_of_sindragosa.up&!talent.icecap.enabled
                        if (CanCastChainsOfIce && (BuffIcyCitadelUp && BuffUnholyStrengthUp &&
                                        AzeriteIcyCitadelRank >= 2 &&
                                        !BuffBreathOfSindragosaUp && !TalentIcecapEnabled)) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=buff.pillar_of_frost.remains<4&buff.pillar_of_frost.up&talent.icecap.enabled&buff.cold_heart.stack>=18&azerite.icy_citadel.rank<=1
                        if (CanCastChainsOfIce && (BuffPillarOfFrostRemains < 4 && BuffPillarOfFrostUp &&
                                        TalentIcecapEnabled && BuffColdHeartStack >= 18 &&
                                        AzeriteIcyCitadelRank <= 1)) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=buff.pillar_of_frost.up&talent.icecap.enabled&azerite.icy_citadel.rank>=2&(buff.cold_heart.stack>=19&buff.icy_citadel.remains<gcd&buff.icy_citadel.up|buff.unholy_strength.up&buff.cold_heart.stack>=18)
                        if (CanCastChainsOfIce && (BuffPillarOfFrostUp && TalentIcecapEnabled &&
                                        AzeriteIcyCitadelRank >= 2 &&
                                        (BuffColdHeartStack >= 19 && BuffIcyCitadelUp ||
                                         BuffUnholyStrengthUp && BuffColdHeartStack >= 18))) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                    }
                    

                    
                    //actions.cooldowns+=/call_action_list,name=cold_heart,if=talent.cold_heart.enabled&((buff.cold_heart.stack>=10&debuff.razorice.stack=5)|target.1.time_to_die<=gcd)
                    if (TalentColdHeartEnabled && ((BuffColdHeartStack >= 10 && DebuffRazorIceStack == 5))) {
                        //actions.cold_heart=chains_of_ice,if=buff.cold_heart.stack>5&target.1.time_to_die<gcd
                        //actions.cold_heart+=/chains_of_ice,if=(buff.seething_rage.remains<gcd)&buff.seething_rage.up
                        if (CanCastChainsOfIce && (BuffSeethingRageRemains < GCD) &&
                            BuffSeethingRageUp) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=(buff.pillar_of_frost.remains<=gcd*(1+cooldown.frostwyrms_fury.ready)|buff.pillar_of_frost.remains<rune.time_to_3)&buff.pillar_of_frost.up&(azerite.icy_citadel.rank<=1|buff.breath_of_sindragosa.up)&!talent.icecap.enabled
                        if (CanCastChainsOfIce &&
                            ((BuffPillarOfFrostRemains <= GCD * (1 + CooldownFrostwyrmsFuryReady ? 1 : 0) ||
                             BuffPillarOfFrostRemains < Aimsharp.TimeUntilRunes(3)) && BuffPillarOfFrostUp &&
                            (AzeriteIcyCitadelRank <= 1 || BuffBreathOfSindragosaUp) && !TalentIcecapEnabled)) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=buff.pillar_of_frost.remains<8&buff.unholy_strength.remains<gcd*(1+cooldown.frostwyrms_fury.ready)&buff.unholy_strength.remains&buff.pillar_of_frost.up&(azerite.icy_citadel.rank<=1|buff.breath_of_sindragosa.up)&!talent.icecap.enabled
                        if (CanCastChainsOfIce && (BuffPillarOfFrostRemains < 8 &&
                                        BuffUnholyStrengthRemains <
                                        GCD * (1 + CooldownFrostwyrmsFuryReady ? 1 : 0) &&
                                        BuffUnholyStrengthRemains > 0 &&
                                        BuffPillarOfFrostUp &&
                                        (AzeriteIcyCitadelRank <= 1 ||
                                         BuffBreathOfSindragosaUp) && !TalentIcecapEnabled)) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=(buff.icy_citadel.remains<4|buff.icy_citadel.remains<rune.time_to_3)&buff.icy_citadel.up&azerite.icy_citadel.rank>=2&!buff.breath_of_sindragosa.up&!talent.icecap.enabled
                        if(CanCastChainsOfIce && ((BuddIcyCitadelRemains < 4000 || BuddIcyCitadelRemains < Aimsharp.TimeUntilRunes(3)) BuffIcyCitadelUp && AzeriteIcyCitadelRank >=2 && !BuffBreathOfSindragosaUp && !TalentIcecapEnabled))
                        {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                            
                        

                        //actions.cold_heart+=/chains_of_ice,if=buff.icy_citadel.up&buff.unholy_strength.up&azerite.icy_citadel.rank>=2&!buff.breath_of_sindragosa.up&!talent.icecap.enabled
                        if (CanCastChainsOfIce && (BuffIcyCitadelUp && BuffUnholyStrengthUp &&
                                        AzeriteIcyCitadelRank >= 2 &&
                                        !BuffBreathOfSindragosaUp && !TalentIcecapEnabled)) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=buff.pillar_of_frost.remains<4&buff.pillar_of_frost.up&talent.icecap.enabled&buff.cold_heart.stack>=18&azerite.icy_citadel.rank<=1
                        if (CanCastChainsOfIce && (BuffPillarOfFrostRemains < 4 && BuffPillarOfFrostUp &&
                                        TalentIcecapEnabled && BuffColdHeartStack >= 18 &&
                                        AzeriteIcyCitadelRank <= 1)) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                        
                        //actions.cold_heart+=/chains_of_ice,if=buff.pillar_of_frost.up&talent.icecap.enabled&azerite.icy_citadel.rank>=2&(buff.cold_heart.stack>=19&buff.icy_citadel.remains<gcd&buff.icy_citadel.up|buff.unholy_strength.up&buff.cold_heart.stack>=18)
                        if (CanCastChainsOfIce && (BuffPillarOfFrostUp && TalentIcecapEnabled &&
                                        AzeriteIcyCitadelRank >= 2 &&
                                        (BuffColdHeartStack >= 19 && BuffIcyCitadelUp ||
                                         BuffUnholyStrengthUp && BuffColdHeartStack >= 18))) {
                            Aimsharp.Cast("Chains of Ice");
                            return true;
                        }
                    }
                    #endregion
                    
                    //actions.cooldowns+=/frostwyrms_fury,if=(buff.pillar_of_frost.up&azerite.icy_citadel.rank<=1&(buff.pillar_of_frost.remains<=gcd|buff.unholy_strength.remains<=gcd&buff.unholy_strength.up))
                    if (Aimsharp.CanCast("Frostwyrm's Fury", "player") && (BuffPillarOfFrostUp && AzeriteIcyCitadelRank <= 1 &&
                                                                (BuffPillarOfFrostRemains >= GCD ||
                                                                 BuffUnholyStrengthRemains >= GCD &&
                                                                 BuffUnholyStrengthRemains > 0))) {
                        Aimsharp.Cast("Frostwyrm's Fury");
                        return true;
                    } 
                    
                    //actions.cooldowns+=/frostwyrms_fury,if=(buff.icy_citadel.up&!talent.icecap.enabled&(buff.unholy_strength.up|buff.icy_citadel.remains<=gcd))|buff.icy_citadel.up&buff.icy_citadel.remains<=gcd&talent.icecap.enabled&buff.pillar_of_frost.up
                    if (Aimsharp.CanCast("Frostwyrm's Fury", "player") &&
                        ((BuffIcyCitadelUp && !TalentIcecapEnabled &&
                          (BuffUnholyStrengthUp || BuffIcyTalonsRemains <= GCD)) || BuffIcyCitadelUp &&
                            BuffIcyCitadelRemains <= GCD && TalentIcecapEnabled && BuffPillarOfFrostUp)) {
                        Aimsharp.Cast("Frostwyrm's Fury");
                        return true;
                    }
                    
                    //actions.cooldowns+=/frostwyrms_fury,if=target.1.time_to_die<gcd|(target.1.time_to_die<cooldown.pillar_of_frost.remains&buff.unholy_strength.up)
                    

                    #endregion

                }

                #endregion

                #region BoSTicking

                
                if (BuffBreathOfSindragosaUp) {
                    //actions.bos_ticking=obliterate,target_if=(debuff.razorice.stack<5|debuff.razorice.remains<10)&runic_power<=32&!talent.frostscythe.enabled
                    if (CanCastObliterate && ((DebuffRazorIceStack < 5 || DebuffRazorIceRemains < 10) &&
                                                           RunicPower <= 32 && !TalentFrostscytheEnabled)) {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                    
                    //actions.bos_ticking+=/obliterate,if=runic_power<=32
                    if (CanCastObliterate && RunicPower <= 32) {
                        Aimsharp.Cast("Obliterate");
                        return true;
                    }
                    
                    //actions.bos_ticking+=/remorseless_winter,if=talent.gathering_storm.enabled
                    if
                    actions.bos_ticking+=/howling_blast,if=buff.rime.up
                    actions.bos_ticking+=/obliterate,target_if=(debuff.razorice.stack<5|debuff.razorice.remains<10)&rune.time_to_5<gcd|runic_power<=45&!talent.frostscythe.enabled
                    actions.bos_ticking+=/obliterate,if=rune.time_to_5<gcd|runic_power<=45
                    actions.bos_ticking+=/frostscythe,if=buff.killing_machine.up&spell_targets.frostscythe>=2
                    actions.bos_ticking+=/horn_of_winter,if=runic_power.deficit>=32&rune.time_to_3>gcd
                    actions.bos_ticking+=/remorseless_winter
                    actions.bos_ticking+=/frostscythe,if=spell_targets.frostscythe>=2
                    actions.bos_ticking+=/obliterate,target_if=(debuff.razorice.stack<5|debuff.razorice.remains<10)&runic_power.deficit>25|rune>3&!talent.frostscythe.enabled
                    actions.bos_ticking+=/obliterate,if=runic_power.deficit>25|rune>3
                    actions.bos_ticking+=/arcane_torrent,if=runic_power.deficit>50
                }
                #endregion
            }

            #endregion

            #endregion





        }

    }
}