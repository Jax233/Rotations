using System.Linq;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Drawing;
using AimsharpWow.API; //needed to access Aimsharp API

namespace AimsharpWow.Modules {
	public class VidFuryWarrior: Rotation {
		
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
			Settings.Add(new Setting("Major Power", MajorAzeritePower, "None"));
			
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
			Settings.Add(new Setting("Top Trinket", Trinkets, "None"));
			Settings.Add(new Setting("Top Trinket AOE?", false));
			Settings.Add(new Setting("Bot Trinket", Trinkets, "None"));
			Settings.Add(new Setting("Bot Trinket AOE?", false));
			Settings.Add(new Setting("Use item: Case Sens", "None"));
			Settings.Add(new Setting("Use item @ HP%", 0, 100, 100));
			List < string > Race = new List < string > (new string[] {
				"Orc",
				"Troll",
				"Dark Iron Dwarf",
				"Mag'har Orc",
				"Lightforged Draenei",
				"None"
			});
			Settings.Add(new Setting("Racial Power", Race, "None"));
			Settings.Add(new Setting("Cold Steel, Hot Blood Trait Number:", 0, 3, 2));
			Settings.Add(new Setting("Mitigation"));
			Settings.Add(new Setting("Auto Victory Rush @ HP%", 0, 100, 70));
			Settings.Add(new Setting("Auto Shout @ HP%", 0, 100, 15));
			Settings.Add(new Setting("Auto Enraged Regeneration @ HP%", 0, 100, 30));
			Settings.Add(new Setting("First 5 Letters of the Addon:", "xxxxx"));

		}
		
		string MajorPower;
		string TopTrinket;
		string BotTrinket;
		string RacialPower;
		string usableitems;
		int AzeriteColdSteel;
		string FiveLetters;
		
		public override void Initialize() {
			Aimsharp.DebugMode();
			
			Aimsharp.PrintMessage("Vid Fury Warrior 1.02", Color.Yellow);
			Aimsharp.PrintMessage("Recommended Single Target talents: 2133123", Color.Yellow);
			Aimsharp.PrintMessage("Recommended Dungeon talents: 3133222", Color.Yellow);
			Aimsharp.PrintMessage("Default mode: PVE, AOE ON, USE CDs/Pots", Color.Yellow);
			Aimsharp.PrintMessage("These macros can be used for manual control:", Color.Yellow);
			Aimsharp.PrintMessage("/xxxxx Potions --Toggles using buff potions on/off.", Color.Blue);
			Aimsharp.PrintMessage("/xxxxx SaveCooldowns --Toggles the use of big cooldowns on/off.", Color.Blue);
			Aimsharp.PrintMessage("/xxxxx noaoe --Toggles to turn off PVE AOE on/off.", Color.Orange);
			Aimsharp.PrintMessage("/xxxxx AzeriteBeamOff --Toggles to turn off AzeriteBeam usage.", Color.Orange);
			Aimsharp.PrintMessage("/xxxxx savepp -- Toggles the use of prepull", Color.Orange);
			Aimsharp.PrintMessage("/xxxxx StormBolt --Queues Storm Bolt up to be used on the next GCD.", Color.Red);
			Aimsharp.PrintMessage("/xxxxx RallyingCry --Queues Rallying Cry up to be used on the next GCD.", Color.Red);
			Aimsharp.PrintMessage("/xxxxx IntimidatingShout --Queues Intimidating SHout up to be used on the next GCD.", Color.Red);
			Aimsharp.PrintMessage("/xxxxx Bladestorm --Queues Bladestorm up to be used on the next GCD.", Color.Red);
			
			
			
			Aimsharp.Latency = 100;
			Aimsharp.QuickDelay = 125;
			
			MajorPower = GetDropDown("Major Power");
			TopTrinket = GetDropDown("Top Trinket");
			BotTrinket = GetDropDown("Bot Trinket");
			RacialPower = GetDropDown("Racial Power");
			usableitems = GetString("Use item: Case Sens");
			AzeriteColdSteel = GetSlider("Cold Steel, Hot Blood Trait Number:");
			FiveLetters = GetString("First 5 Letters of the Addon:");
			
			Spellbook.Add(MajorPower);
			
			if (RacialPower == "Orc") Spellbook.Add("Blood Fury");
			if (RacialPower == "Troll") Spellbook.Add("Berserking");
			if (RacialPower == "Dark Iron Dwarf") Spellbook.Add("Fireblood");
			if (RacialPower == "Mag'har Orc") Spellbook.Add("Ancestral Call");
			if (RacialPower == "Lightforged Draenei") Spellbook.Add("Light's Judgment");
			
			
			
			Spellbook.Add("Victory Rush");
			Spellbook.Add("Enraged Regeneration");
			
			Spellbook.Add("Avatar");
			
			Spellbook.Add("Battle Shout");
			
			Spellbook.Add("Heroic Throw");
			Spellbook.Add("Hamstring");
			Spellbook.Add("Rallying Cry");
			Spellbook.Add("Defensive Stance");
			
			
            Spellbook.Add("Charge");
            Spellbook.Add("Heroic Leap");
            Spellbook.Add("Rampage");
            Spellbook.Add("Recklessness");
            Spellbook.Add("Siegebreaker");
            Spellbook.Add("Whirlwind");
            Spellbook.Add("Execute");
            Spellbook.Add("Furious Slash");
            Spellbook.Add("Bladestorm");
            Spellbook.Add("Bloodthirst");
            Spellbook.Add("Dragon Roar");
            Spellbook.Add("Raging Blow");
			Spellbook.Add("Storm Bolt");
			Spellbook.Add("Intimidating Shout");
			Spellbook.Add("Rallying Cry");
			Spellbook.Add("Onslaught");
			Spellbook.Add("Enraged Regeneration");
			Spellbook.Add("Bloodbath");
			Spellbook.Add("Crushing Blow");
			
			
			
			Buffs.Add("Bloodlust");
			Buffs.Add("Heroism");
			Buffs.Add("Time Warp");
			Buffs.Add("Ancient Hysteria");
			Buffs.Add("Netherwinds");
			Buffs.Add("Drums of Rage");
			Buffs.Add("Lifeblood");
			Buffs.Add("Memory of Lucid Dreams");
			Buffs.Add("Reckless Force");
			Buffs.Add("Guardian of Azeroth");
			
			
			Buffs.Add("Bloodlust");
            Buffs.Add("Heroism");
            Buffs.Add("Time Warp");
            Buffs.Add("Ancient Hysteria");
            Buffs.Add("Netherwinds");
            Buffs.Add("Drums of Rage");
            Buffs.Add("Lifeblood");
            Buffs.Add("Memory of Lucid Dreams");
            Buffs.Add("Reckless Force");
            Buffs.Add("Guardian of Azeroth");

            Buffs.Add("Recklessness");
            Buffs.Add("Meat Cleaver");
            Buffs.Add("Enrage");
            Buffs.Add("Furious Slash");
            Buffs.Add("Whirlwind");

            Debuffs.Add("Razor Coral");
            Debuffs.Add("Conductive Ink");
            Debuffs.Add("Shiver Venom");
            Debuffs.Add("Siegebreaker");
			
			Buffs.Add("Berserker Rage");
			Buffs.Add("Meat Cleaver");
			Buffs.Add("Enrage");
			Buffs.Add("Furious Slash");
			Buffs.Add("Whirlwind");
			Buffs.Add("Test of Might");
			Buffs.Add("Avatar");
			Buffs.Add("Sharpen Blade");
			Buffs.Add("Battle Shout");
			Buffs.Add("Overpower");
			Buffs.Add("Bladestorm");
			Buffs.Add("Defensive Stance");
			Buffs.Add("Sweeping Strikes");
			Buffs.Add("Sudden Death");
			
			Debuffs.Add("Razor Coral");
			Debuffs.Add("Conductive Ink");
			Debuffs.Add("Shiver Venom");
			Debuffs.Add("Siegebreaker");
			Debuffs.Add("Rend");
			Debuffs.Add("Hamstring");
			Debuffs.Add("Deep Wounds");
			
			Items.Add(TopTrinket);
			Items.Add(BotTrinket);
			Items.Add(usableitems);
			
			Macros.Add("ItemUse", "/use " + usableitems);
			Macros.Add("TopTrink", "/use 13");
			Macros.Add("BotTrink", "/use 14");
			Macros.Add("StormBoltOff", "/"+FiveLetters+" StormBolt");
			Macros.Add("IntimidatingShoutOff", "/"+FiveLetters+" IntimidatingShout");
			Macros.Add("RallyingCryOff", "/"+FiveLetters+" RallyingCry");
			Macros.Add("BladestormOff", "/"+FiveLetters+" Bladestorm");
			
			CustomCommands.Add("Potions");
			CustomCommands.Add("SaveCooldowns");
			CustomCommands.Add("noaoe");
			CustomCommands.Add("pvp");
			CustomCommands.Add("savepp");
			CustomCommands.Add("StormBolt");
			CustomCommands.Add("RallyingCry");
			CustomCommands.Add("IntimidatingShout");
			CustomCommands.Add("Bladestorm");
			CustomCommands.Add("AzeriteBeamOff");
			
		}
		
		// optional override for the CombatTick which executes while in combat
		
		public override bool CombatTick() {
			
			
			bool Fighting = Aimsharp.Range("target") <= 8 && Aimsharp.TargetIsEnemy();
			bool Moving = Aimsharp.PlayerIsMoving();
			float Haste = Aimsharp.Haste() / 100f;
			int Time = Aimsharp.CombatTime();
			int Range = Aimsharp.Range("target");
			int TargetHealth = Aimsharp.Health("target");
			int PlayerHealth = Aimsharp.Health("player");
			string LastCast = Aimsharp.LastCast();
			bool IsChanneling = Aimsharp.IsChanneling("player");
			
			int EnemiesInMelee = Aimsharp.EnemiesInMelee();
			int GCDMAX = (int)(1500f / (Haste + 1f));
			int GCD = Aimsharp.GCD();
			int Latency = Aimsharp.Latency;
			bool HasLust = Aimsharp.HasBuff("Bloodlust", "player", false) || Aimsharp.HasBuff("Heroism", "player", false) || Aimsharp.HasBuff("Time Warp", "player", false) || Aimsharp.HasBuff("Ancient Hysteria", "player", false) || Aimsharp.HasBuff("Netherwinds", "player", false) || Aimsharp.HasBuff("Drums of Rage", "player", false);
			int FlameFullRecharge = (int)(Aimsharp.RechargeTime("Concentrated Flame") - GCD + (30000f) * (1f - Aimsharp.SpellCharges("Concentrated Flame")));
			int ShiverVenomStacks = Aimsharp.DebuffStacks("Shiver Venom");
			
			int CDGuardianOfAzerothRemains = Aimsharp.SpellCooldown("Guardian of Azeroth") - GCD;
			bool BuffGuardianOfAzerothUp = Aimsharp.HasBuff("Guardian of Azeroth");
			int CDBloodOfTheEnemyRemains = Aimsharp.SpellCooldown("Blood of the Enemy") - GCD;
			int BuffMemoryOfLucidDreamsRemains = Aimsharp.BuffRemaining("Memory of Lucid Dreams") - GCD;
			int CDMemoryOfLucidDreamsRemains = Aimsharp.SpellCooldown("Memory of Lucid Dreams") - GCD;
			bool BuffMemoryOfLucidDreamsUp = BuffMemoryOfLucidDreamsRemains > 0;
			bool DebuffRazorCoralUp = Aimsharp.HasDebuff("Razor Coral");
			bool DebuffConductiveInkUp = Aimsharp.HasDebuff("Conductive Ink");
			
			int Rage = Aimsharp.Power("player");
			
			#region CommandsSetup
			// Commands
			bool UsePotion = Aimsharp.IsCustomCodeOn("Potions");
			bool NoCooldowns = Aimsharp.IsCustomCodeOn("SaveCooldowns");
			bool OffAOE = Aimsharp.IsCustomCodeOn("noaoe");
			bool PVPmode = Aimsharp.IsCustomCodeOn("pvp");
			bool StormBolt = Aimsharp.IsCustomCodeOn("StormBolt");
			bool RallyingCry = Aimsharp.IsCustomCodeOn("RallyingCry");
			bool IntimidatingShout = Aimsharp.IsCustomCodeOn("IntimidatingShout");
			bool Bladestorm = Aimsharp.IsCustomCodeOn("Bladestorm");
			bool AzeriteBeamOff = Aimsharp.IsCustomCodeOn("AzeriteBeamOff");
			#endregion
			
			#region CooldownsSetup

			
			// CDS
			int CDBladestormRemains = Aimsharp.SpellCooldown("Bladestorm");
			bool DebuffHamstringUp = Aimsharp.DebuffRemaining("Hamstring") - GCD > 0;
			int BerserkerRageRemains = Aimsharp.BuffRemaining("Berserker Rage");
			bool BuffBerserkerRageUp = BerserkerRageRemains > 0;
			int CDStormBoltRemains = Aimsharp.SpellCooldown("Storm Bolt");
			bool CDStormBoltUp = CDStormBoltRemains > GCD;
			int CDIntimidatingShoutRemains = Aimsharp.SpellCooldown("Intimidating Shout");
			int CDRallyingCryRemains = Aimsharp.SpellCooldown("Rallying Cry");
			int CDBloodthirstRemaining = Aimsharp.SpellCooldown("Bloodthirst");
			int CDSiegebreakerRemaining = Aimsharp.SpellCooldown("Siegebreaker");
			int CDDragonRoarRemaining = Aimsharp.SpellCooldown("Dragon Roar");
			int CDRampageRemaining = Aimsharp.SpellCooldown("Rampage");
			int CDRagingBlowRemaining = Aimsharp.SpellCooldown("Raging Blow");
			int CDOnslaughtRemaining = Aimsharp.SpellCooldown("Onslaught");
			int CDRecklessnessRemains = Aimsharp.SpellCooldown("Recklessness");

			

			#endregion
			
			#region TalentSetup
			//Talents
			
			bool TalentDoubleTime = Aimsharp.Talent(2, 1);
			bool TalentStormbolt = Aimsharp.Talent(2, 3);
			bool TalentMassacre = Aimsharp.Talent(3, 1);
			bool TalentSeetheEnabled = Aimsharp.Talent(5, 1);
			bool TalentFrothingBerserkerEnabled = Aimsharp.Talent(5, 2);
			bool TalentCarnage = Aimsharp.Talent(5, 1);
			bool TalentRecklessAbondonEnabled = Aimsharp.Talent(7, 2);
			bool TalentSiegebreakerEnabled = Aimsharp.Talent(7, 3);
			
			#endregion
			
			//Buffs
			bool SiegebreakerUp = Aimsharp.HasBuff("Siegebreaker");
			bool RecklessnessUp = Aimsharp.HasBuff("Recklessness", "player");
			int BuffRecklessnessRemains = Aimsharp.BuffRemaining("Recklessness", "player");
			
			
			bool BladestormUp = Aimsharp.HasBuff("Bladestorm", "player");
			bool BattleShoutUp = Aimsharp.HasBuff("Battle Shout", "player");
			bool BuffSharpenBladeUp = Aimsharp.HasBuff("Sharpen Blade", "player");
			bool BuffOverpower = Aimsharp.HasBuff("Overpower", "player");
			int BuffEnrageRemains = Aimsharp.BuffRemaining("Enrage", "player");
			bool BloodlustUp = Aimsharp.HasBuff("Bloodlust", "player");
			int FuriousSlashRemains = Aimsharp.BuffRemaining("Furious Slash", "player");
			bool BuffEnrageUp = Aimsharp.HasBuff("Enrage", "player");
		    int BuffRagingBlowStacks = Aimsharp.SpellCharges("Raging Blow");
		    int BuffCrushingBlowStacks = Aimsharp.SpellCharges("Crushing Blow");
		    int BuffWhirlwindRemains = Aimsharp.BuffRemaining("Whirlwind");
		    bool BuffWhirlwindUp = BuffWhirlwindRemains > 0;

		    
			
			
			//Debuffs
			bool DebuffDeepWoundsUp = Aimsharp.HasDebuff("Deep Wounds", "target");
			int DebuffDeepWoundsRemaining = Aimsharp.DebuffRemaining("Deep Wounds", "target");
			bool DebuffConcentratedFlameUp = Aimsharp.HasDebuff("Concentrated Flame", "target");
			bool DebuffBloodOfTheEnemyUp = Aimsharp.HasDebuff("Blood of the Enemy", "target");

			#region CanCasts

			bool CanCastBloodthirst = Aimsharp.CanCast("Bloodthirst");
			bool CanCastBloodbath = Aimsharp.CanCast("Bloodbath");
			bool CanCastDragonRoar = Aimsharp.CanCast("Dragon Roar", "player");
			bool CanCastOnslaught = Aimsharp.CanCast("Onslaught");
			bool CanCastRagingBlow = Aimsharp.CanCast("Raging Blow");
			bool CanCastCrushingBlow = Aimsharp.CanCast("Crushing Blow");
			bool CanCastRampage = Aimsharp.CanCast("Rampage");
			bool CanCastExecute = Aimsharp.CanCast("Execute");
			bool CanCastSiegebreaker = Aimsharp.CanCast("Siegebreaker");
			bool CanCastWhirlwind = Aimsharp.CanCast("Whirlwind", "player");
			bool CanCastFocusedAzeriteBeam = Aimsharp.CanCast("Focused Azerite Beam") ||  (Aimsharp.SpellCooldown("Focused Azerite Beam") <= GCD && MajorPower == "Focused Azerite Beam");
			bool CanCastGuardianOfAzeroth = Aimsharp.CanCast("Guardian of Azeroth") || (Aimsharp.SpellCooldown("Guardian of Azeroth") <= GCD && MajorPower == "Guardian of Azeroth");
			bool CanCastBloodOfTheEnemy = Aimsharp.CanCast("Blood of the Enemy") || (Aimsharp.SpellCooldown("Blood of the Enemy") <= GCD && MajorPower == "Blood of the Enemy");
			bool CanCastConcentratedFlame = Aimsharp.CanCast("Concentrated Flame") ||
			                                (Aimsharp.SpellCooldown("Concentrated Flame") <= GCD &&
			                                 MajorPower == "Concentrated Flame");

			#endregion
			
			
			// Options
			bool TopTrinketAOE = GetCheckBox("Top Trinket AOE?");
			bool BotTrinketAOE = GetCheckBox("Bot Trinket AOE?");

			
			
			
			if (IsChanneling) return false;

			

			
			#region Utility
			// Utility
			
				// QUEUED STORMBOLT
				if(CDStormBoltRemains > 5000 && StormBolt) {
					Aimsharp.Cast("StormBoltOff");
					return true;
				}
				
				if (StormBolt && Aimsharp.CanCast("Storm Bolt")) {
					Aimsharp.PrintMessage("Queued Storm Bolt");
					Aimsharp.Cast("Storm Bolt");
					return true;
				}
				
				// QUEUED INTIMIDATING SHOUT
				if(CDIntimidatingShoutRemains > 5000 && IntimidatingShout) {
					Aimsharp.Cast("IntimidatingShoutOff");
					return true;
				}
				
				if (IntimidatingShout && Aimsharp.CanCast("Intimidating Shout")) {
					Aimsharp.PrintMessage("Queued Intimidating Shout");
					Aimsharp.Cast("Intimidating Shout");
					return true;
				}
				
				// QUEUED RALLYING CRY
				if(CDRallyingCryRemains > 5000 && RallyingCry) {
					Aimsharp.Cast("RallyingCryOff");
					return true;
				}
				
				if (RallyingCry && Aimsharp.CanCast("Rallying Cry","player")) {
					Aimsharp.PrintMessage("Queued Rallying Cry");
					Aimsharp.Cast("Rallying Cry");
					return true;
				}
				
				// QUEUED BLADESTORM
				if(CDBladestormRemains > 5000 && Bladestorm) {
					Aimsharp.Cast("BladestormOff");
					return true;
				}
				
				if (Bladestorm && Aimsharp.CanCast("Bladestorm","player")) {
					Aimsharp.PrintMessage("Queued Bladestorm");
					Aimsharp.Cast("Bladestorm");
					return true;
				}
				
				// Auto Victory Rush
				if (Aimsharp.CanCast("Victory Rush")) {
					if (PlayerHealth <= GetSlider("Auto Victory Rush @ HP%")) {
						Aimsharp.Cast("Victory Rush");
						return true;
					}
				}
				
				// Auto Commanding Shout
				if (Aimsharp.CanCast("Rallying Cry", "player")) {
					if (PlayerHealth <= GetSlider("Auto Shout @ HP%")) {
						Aimsharp.Cast("Rallying Cry");
						return true;
					}
				}
				
				
				if (Aimsharp.CanCast("Enraged Regeneration", "player")) {
					if (PlayerHealth <= GetSlider("Auto Enraged Regeneration @ HP%")) {
						Aimsharp.Cast("Enraged Regeneration");
						return true;
					}
				}
			
			#endregion

			#region Cooldowns

			if (CanCastRampage && CDRecklessnessRemains < 3 && TalentRecklessAbondonEnabled) {
				Aimsharp.Cast("Rampage");
				return true;
			}

			if (CanCastBloodOfTheEnemy && !NoCooldowns && ((RecklessnessUp || CDRecklessnessRemains < 1) &&
			                               (Rage > 80 && (BuffWhirlwindUp && BuffEnrageUp || EnemiesInMelee == 1)))) {
				Aimsharp.Cast("Blood of the Enemy");
				return true;
			}

			if (CanCastConcentratedFlame && !RecklessnessUp && !SiegebreakerUp && !DebuffConcentratedFlameUp)
			{
				Aimsharp.Cast("Concentrated Flame");
				return true;
			}
			
			if (CanCastFocusedAzeriteBeam && !NoCooldowns && !RecklessnessUp && !SiegebreakerUp) {
				Aimsharp.Cast("Focused Azerite Beam");
				return true;
			}

			if (CanCastGuardianOfAzeroth && !NoCooldowns && !RecklessnessUp) {
				Aimsharp.Cast("Guardian of Azeroth");
				return true;
			}
			
			//RECKLESSNESS
			if(CDRecklessnessRemains <= 0 && !NoCooldowns) {
				if((MajorPower != "Condensed Life-Force" && MajorPower != "Blood of the Enemy") || CDGuardianOfAzerothRemains > 1 || BuffGuardianOfAzerothUp || DebuffBloodOfTheEnemyUp) {
					Aimsharp.Cast("Recklessness");
					return true;
				}
			}
			
			
			if(EnemiesInMelee > 1 && !BuffWhirlwindUp && CanCastWhirlwind && !OffAOE) {
				Aimsharp.Cast("Whirlwind");
				return true;
			}

			if (RecklessnessUp && Aimsharp.CanCast("Blood Fury", "player")) {
				Aimsharp.Cast("Blood Fury");
				return true;
			}
			
			if (RecklessnessUp && Aimsharp.CanCast("Berserking", "player")) {
				Aimsharp.Cast("Berserking");
				return true;
			}

			if (!RecklessnessUp && !SiegebreakerUp && Aimsharp.CanCast("Light's Judgement")) {
				Aimsharp.Cast("Light's Judgement");
				return true;
			}
			
			if (RecklessnessUp && Aimsharp.CanCast("Fire Blood", "player")) {
				Aimsharp.Cast("Fire Blood");
				return true;
			}
			
			if (RecklessnessUp && Aimsharp.CanCast("Ancestral Call", "player")) {
				Aimsharp.Cast("Ancestral Call");
				return true;
			}
			
			

			#endregion

			
			#region Fury Rotation
			// PVE Rotation
			
				
				if (Fighting) {
					
					//WHIRLWIND PROC
					
					
					if(CanCastSiegebreaker) {
						Aimsharp.Cast("Siegebreaker");
						return true;
					}
					
					//RAMPAGE
					if (CanCastRampage && ((RecklessnessUp || BuffMemoryOfLucidDreamsUp) || (BuffEnrageRemains < GCD || Rage > 90) )) {
						Aimsharp.Cast("Rampage");
						return true;
					}

					if(CanCastExecute) {
						Aimsharp.Cast("Execute");
						return true;
					}
					
					if (Aimsharp.CanCast("Bladestorm", "player") && Aimsharp.LastCast() == "Rampage") {
						Aimsharp.Cast("Bladestorm");
						return true;
					}

					if (CanCastBloodbath && (!BuffEnrageUp || AzeriteColdSteel > 1)) {
						Aimsharp.Cast("Bloodbath");
						return true;
					}
					
					if ((!BuffEnrageUp || AzeriteColdSteel > 1) && CanCastBloodthirst){
						Aimsharp.Cast("Bloodthirst");
						return true;
					}

					if (CanCastOnslaught) {
						Aimsharp.Cast("Onslaught");
						return true;
					}

					if (CanCastDragonRoar && BuffEnrageUp) {
						Aimsharp.Cast("Dragon Roar");
						return true;
					}
					
					
					if (CanCastCrushingBlow && BuffCrushingBlowStacks == 2) {
						Aimsharp.Cast("Crushing Blow");
						return true;
					}
					
					if(CanCastRagingBlow && BuffRagingBlowStacks == 2) {
						Aimsharp.Cast("Raging Blow");
						return true;
					}

					if (CanCastBloodbath) {
						Aimsharp.Cast("Bloodbath");
						return true;
					}
					
					if(CanCastBloodthirst) {
						Aimsharp.Cast("Bloodthirst");
						return true;
					}

					if (CanCastRagingBlow) {
						Aimsharp.Cast("Raging Blow");
						return true;
					}

					if (CanCastCrushingBlow) {
						Aimsharp.Cast("Crushing Blow");
						return true;
					}

					
					if (Aimsharp.CanCast("Whirlwind", "player")) {
						Aimsharp.Cast("Whirlwind");
						return true;
					}
					
					}
				
				
			#endregion
			
			
			
			
			
			
			return false;
			
			
		}
		
		public override bool OutOfCombatTick() {
						
			if (Aimsharp.CanCast("Battle Shout", "player") && !Aimsharp.HasBuff("Battle Shout", "player", false)) {
				Aimsharp.Cast("Battle Shout");
				return true;
			}

			
			
			
			return false;
		}
	}
}																																																												