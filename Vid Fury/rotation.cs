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
			Settings.Add(new Setting("Racial Power", Race, "Orc"));
			Settings.Add(new Setting("Cold Steel, Hot Blood Trait Number:", 0, 3, 0));
			Settings.Add(new Setting("Mitigation"));
			Settings.Add(new Setting("Auto Victory Rush @ HP%", 0, 100, 100));
			Settings.Add(new Setting("Auto Shout @ HP%", 0, 100, 75));
			Settings.Add(new Setting("Auto Die by the Sword @ HP%", 0, 100, 50));
			Settings.Add(new Setting("Auto Stance @ HP%", 0, 100, 30));
			Settings.Add(new Setting("Unstance @ HP%", 0, 100, 80));
		}
		
		string MajorPower;
		string TopTrinket;
		string BotTrinket;
		string RacialPower;
		string usableitems;
		int ColdSteel;
		string FiveLetters;
		
		public override void Initialize() {
			//Aimsharp.DebugMode();
			
			Aimsharp.PrintMessage("Vid Fury Warrior 1.02", Color.Yellow);
			Aimsharp.PrintMessage("Recommended PVE talents: 2133123", Color.Yellow);
			Aimsharp.PrintMessage("Recommended PVP talents: 2333211", Color.Yellow);
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
			Aimsharp.PrintMessage("/xxxxx pvp --Toggles PVP Mode.", Color.Red);
			
			
			Aimsharp.Latency = 100;
			Aimsharp.QuickDelay = 125;
			
			MajorPower = GetDropDown("Major Power");
			TopTrinket = GetDropDown("Top Trinket");
			BotTrinket = GetDropDown("Bot Trinket");
			RacialPower = GetDropDown("Racial Power");
			usableitems = GetString("Use item: Case Sens");
			ColdSteel = GetSlider("Cold Steel, Hot Blood Trait Number:");
			
			Spellbook.Add(MajorPower);
			
			if (RacialPower == "Orc") Spellbook.Add("Blood Fury");
			if (RacialPower == "Troll") Spellbook.Add("Berserking");
			if (RacialPower == "Dark Iron Dwarf") Spellbook.Add("Fireblood");
			if (RacialPower == "Mag'har Orc") Spellbook.Add("Ancestral Call");
			if (RacialPower == "Lightforged Draenei") Spellbook.Add("Light's Judgment");
			
			
			
			Spellbook.Add("Victory Rush");
			Spellbook.Add("Die by the Sword");
			
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
			
			Debuffs.Add("Razor Coral");
			Debuffs.Add("Conductive Ink");
			Debuffs.Add("Shiver Venom");
			Debuffs.Add("Siegebreaker");
			Debuffs.Add("Colossus Smash");
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
			int CDSweepingStrikesRemains = Aimsharp.SpellCooldown("Sweeping Strikes") - GCD;
			int ColossusSmashRemains = Aimsharp.DebuffRemaining("Colossus Smash", "target") - GCD;
			int CDColossusSmashRemains = Aimsharp.SpellCooldown("Colossus Smash") - GCD;
			int CDBladestormRemains = Aimsharp.SpellCooldown("Bladestorm") -GCD;
			bool DebuffColossusSmashUp = ColossusSmashRemains > 0;
			int RendRemains = Aimsharp.DebuffRemaining("Rend") - GCD;
			bool DebuffRendUp = RendRemains > 0;
			bool DebuffHamstringUp = Aimsharp.DebuffRemaining("Hamstring") - GCD > 0;
			int BerserkerRageRemains = Aimsharp.BuffRemaining("Berserker Rage");
			bool BuffBerserkerRageUp = BerserkerRageRemains > 0;
			int CDStormBoltRemains = Aimsharp.SpellCooldown("Storm Bolt");
			bool CDStormBoltUp = CDStormBoltRemains > GCD;
			int CDIntimidatingShoutRemains = Aimsharp.SpellCooldown("Intimidating Shout");
			int CDRallyingCryRemains = Aimsharp.SpellCooldown("Rallying Cry");
			int CDBloodthirstRemaining = Aimsharp.SpellCooldown("Bloodthirst") - GCD;
			int CDSiegebreakerRemaining = Aimsharp.SpellCooldown("Siegebreaker") - GCD;
			int CDDragonRoarRemaining = Aimsharp.SpellCooldown("Dragon Roar") - GCD;
			int CDRampageRemaining = Aimsharp.SpellCooldown("Rampage") - GCD;
			int CDRagingBlowRemaining = Aimsharp.SpellCooldown("Raging Blow") - GCD;

			#endregion
			
			#region TalentSetup
			//Talents
			bool TalentDoubleTime = Aimsharp.Talent(2, 1);
			bool TalentStormbolt = Aimsharp.Talent(2, 3);
			bool TalentMassacre = Aimsharp.Talent(3, 1);
			bool TalentFervorOfBattle = Aimsharp.Talent(3, 2);
			bool TalentCollateralDamage = Aimsharp.Talent(5, 1);
			bool TalentWarbreaker = Aimsharp.Talent(5, 2);
			bool TalentCleave = Aimsharp.Talent(5, 3);
			bool TalentAvatar = Aimsharp.Talent(6, 2);
			bool TalentFrothingBerserker = Aimsharp.Talent(5, 3);
			bool TalentCarnage = Aimsharp.Talent(5, 1);
			#endregion
			
			//Buffs
			bool WhirlwindUp = Aimsharp.HasBuff("Whirlwind");
			bool SiegebreakerUp = Aimsharp.HasBuff("Siegebreaker");
			bool RecklessnessUp = Aimsharp.HasBuff("Recklessness", "player");
			int RecklessnessRemains = Aimsharp.BuffRemaining("Recklessness", "player") - GCD;
			bool SweepingStrikesUp = Aimsharp.HasBuff("Sweeping Strikes", "player");
			
			bool BladestormUp = Aimsharp.HasBuff("Bladestorm", "player");
			bool BattleShoutUp = Aimsharp.HasBuff("Battle Shout", "player");
			bool BuffSharpenBladeUp = Aimsharp.HasBuff("Sharpen Blade", "player");
			bool BuffOverpower = Aimsharp.HasBuff("Overpower", "player");
			int BuffStacksOP = Aimsharp.BuffStacks("Overpower", "player");
			bool BuffCrushingAssault = Aimsharp.HasBuff("Crushing Assault", "player");
			int BuffEnrageRemains = Aimsharp.BuffRemaining("Enrage", "player") - GCD;
			bool BloodlustUp = Aimsharp.HasBuff("Bloodlust", "player");
			int FuriousSlashRemains = Aimsharp.BuffRemaining("Furious Slash", "player") - GCD;
			bool EnrageUp = Aimsharp.HasBuff("Enrage", "player");
		    int RagingBlowStacks = Aimsharp.SpellCharges("Raging Blow");
		    int BuffWhirlwindRemains = Aimsharp.BuffRemaining("Whirlwind") - GCD;
		    bool BuffWhirlwindUp = BuffWhirlwindRemains > 0;
			
			
			
			
			//Debuffs
			bool DebuffDeepWoundsUp = Aimsharp.HasDebuff("Deep Wounds", "target");
			int DebuffDeepWoundsRemaining = Aimsharp.DebuffRemaining("Deep Wounds", "target");
			bool DebuffConcentratedFlameUp = Aimsharp.HasDebuff("Concentrated Flame", "target");
			
			
			// Options
			bool TopTrinketAOE = GetCheckBox("Top Trinket AOE?");
			bool BotTrinketAOE = GetCheckBox("Bot Trinket AOE?");
			
			
			if (OffAOE) {
				EnemiesInMelee = EnemiesInMelee > 0 ? 1 : 0;
			}
			
			if (IsChanneling) return false;

			#region Fury Cooldowns
			//COOLDOWNS
			if (!NoCooldowns && Fighting) {
				
				//TRINKET 1
				if (Aimsharp.CanUseTrinket(0) && TopTrinket == "Generic") {
					if (!TopTrinketAOE) {
						Aimsharp.Cast("TopTrink", true);
						return true;
					} 
					else if (EnemiesInMelee >= 1) {
						Aimsharp.Cast("TopTrink", true);
						return true;
					}
				}
				
				//TRINKET 2
				if (Aimsharp.CanUseTrinket(1) && BotTrinket == "Generic") {
					if (!BotTrinketAOE) {
						Aimsharp.Cast("BotTrink", true);
						return true;
					} 
					else if (EnemiesInMelee >= 1) {
						Aimsharp.Cast("BotTrink", true);
						return true;
					}
				}
				
				//POTION
				if (Aimsharp.CanUseItem(usableitems) && usableitems != "None" && !UsePotion) {
					if (EnemiesInMelee >= 1 && PlayerHealth <= GetSlider("Use item @ HP%")) {
						Aimsharp.Cast("ItemUse", true);
						return true;
					}
				}
				
				//RAMPAGE
				if (Aimsharp.CanCast("Rampage") && RecklessnessRemains <3) {
					Aimsharp.Cast("Rampage");
					return true;
				}
				
				//BLOOD OF THE ENEMY
				if (MajorPower == "Blood of the Enemy" && EnemiesInMelee > 0) {
					if (Aimsharp.CanCast("Blood of the Enemy", "player") && RecklessnessUp) {
						Aimsharp.Cast("Blood of the Enemy");
						return true;
					}
				}
				
				//PURIFYING BLAST
				if (MajorPower == "Purifying Blast") {
					if (!RecklessnessUp && !SiegebreakerUp && Aimsharp.CanCast("Purifying Blast")) {
						Aimsharp.Cast("Purifying Blast");
						return true;
					}
					
				}
				
				//RIPPLE IN SPACE
				if (MajorPower == "Ripple in Space") {
					if (!RecklessnessUp && !SiegebreakerUp && Aimsharp.CanCast("Ripple in Space")) {
						Aimsharp.Cast("Ripple in Space");
						return true;
					}
					
				}
				
				//WORLDVEIN RESONANCE
				if (MajorPower == "Worldvein Resonance") {
					if (Aimsharp.CanCast("Worldvein Resonance", "player")) {
						if (!RecklessnessUp && !SiegebreakerUp) {
							Aimsharp.Cast("Worldvein Resonance");
							return true;
						}
					}
				}
				
				//FOCUSED AZERITE BEAM
				if (MajorPower == "Focused Azerite Beam" && !AzeriteBeamOff) {
					if (Aimsharp.CanCast("Focused Azerite Beam", "player")) {
						if (!RecklessnessUp && !SiegebreakerUp) {
							Aimsharp.Cast("Focused Azerite Beam");
							return true;
						}
					}
				}
				
				//REAPING FLAMES
				if (MajorPower == "Reaping Flames") {
					if(Aimsharp.CanCast("Reaping Flames")) {
						if(!RecklessnessUp && !SiegebreakerUp) {
							Aimsharp.Cast("Reaping Flames");
							return true;
						}
					}
				}
				
				//CONCENTRATED FLAME
				if (MajorPower == "Concentrated Flame") {
					if (Aimsharp.CanCast("Concentrated Flame")) {
						if (!RecklessnessUp && !SiegebreakerUp && !DebuffConcentratedFlameUp) {
							Aimsharp.Cast("Concentrated Flame");
							return true;
						}
					}
				}
				
				//THE UNBOUND FORCE
				if (MajorPower == "The Unbound Force") {
					if (Aimsharp.CanCast("The Unbound Force")) {
						Aimsharp.Cast("The Unbound Force");
						return true;
					}
				}
				
				//GUARDIAN OF AZEROTH
				if (MajorPower == "Guardian of Azeroth") {
					if (Aimsharp.CanCast("Guardian of Azeroth", "player")) {
						if (CDColossusSmashRemains<10) {
							Aimsharp.Cast("Guardian of Azeroth");
							return true;
						}
					}
				}
				
				//MEMORY OF LUCID DREAMS TODO: TTK maybe?
				if (MajorPower == "Memory of Lucid Dreams") {
					if (Aimsharp.CanCast("Memory of Lucid Dreams", "player")) {
						if (CDColossusSmashRemains<GCD) {
							Aimsharp.Cast("Memory of Lucid Dreams");
							return true;
						}
					}
				}
				
				//AVATAR								
				if (TalentAvatar) {
					if (Aimsharp.CanCast("Avatar", "player")) {
						if(CDColossusSmashRemains < 8 ) {
							Aimsharp.Cast("Avatar");
							return true;
						}
						
					}
				}
				
				//actions +=/ use_item,name = ashvanes_razor_coral,if= !Debuff.razor_coral_Debuff.up | (target.health.pct < 30.1 & Debuff.conductive_ink_Debuff.up) | (!Debuff.conductive_ink_Debuff.up & buff.memory_of_lucid_dreams.up | prev_gcd.2.guardian_of_azeroth | prev_gcd.2.recklessness & (!essence.memory_of_lucid_dreams.major & !essence.condensed_lifeforce.major))
				if (Aimsharp.CanUseItem("Ashvane's Razor Coral")) {
					if (!DebuffRazorCoralUp || (TargetHealth <= 30 && DebuffConductiveInkUp) || (!DebuffConductiveInkUp && BuffMemoryOfLucidDreamsUp || CDGuardianOfAzerothRemains > 180000 - GCDMAX * 2 || (MajorPower != "Memory of Lucid Dreams" && MajorPower != "Guardian of Azeroth"))) {
						Aimsharp.Cast("Ashvane's Razor Coral", true);
						return true;
					}
				}
				
				//BLOOD FURY
				if (RacialPower == "Orc") {
					if (Aimsharp.CanCast("Blood Fury", "player")) {
						if((BuffMemoryOfLucidDreamsRemains < 5 || (MajorPower != "Memory of Lucid Dreams" && DebuffColossusSmashUp))) {
							Aimsharp.Cast("Blood Fury", true);
							return true;
						}
					}
				}
				
				//BERSERKING
				if (RacialPower == "Troll") {
					if (Aimsharp.CanCast("Berserking", "player") && (BuffMemoryOfLucidDreamsUp || (MajorPower != "Memory of Lucid Dreams" && DebuffColossusSmashUp))) {
						Aimsharp.Cast("Berserking", true);
						return true;
					}
				}
				
				//actions+=/lights_judgment,if=buff.recklessness.down
				if (RacialPower == "Lightforged Draenei") {
					if (Aimsharp.CanCast("Light's Judgment", "player") && !DebuffColossusSmashUp) {
						Aimsharp.Cast("Light's Judgment", true);
						return true;
					}
				}
				
				//actions+=/fireblood
				if (RacialPower == "Dark Iron Dwarf") {
					if (Aimsharp.CanCast("Fireblood", "player")&& (BuffMemoryOfLucidDreamsRemains < 5 || (MajorPower != "Memory of Lucid Dreams" && DebuffColossusSmashUp))) {
						Aimsharp.Cast("Fireblood", true);
						return true;
					}
				}
				
				//actions+=/ancestral_call
				if (RacialPower == "Mag'har Orc") {
					if (Aimsharp.CanCast("Ancestral Call", "player")&& (BuffMemoryOfLucidDreamsRemains < 5 || (MajorPower != "Memory of Lucid Dreams" && DebuffColossusSmashUp))) {
						Aimsharp.Cast("Ancestral Call", true);
						return true;
					}
				}
			}
			#endregion
			
			#region Utility
			// Utility
			if (Fighting) {
				// QUEUED STORMBOLT
				if(CDStormBoltRemains > 25000 && StormBolt) {
					Aimsharp.Cast("StormBoltOff");
					return true;
				}
				
				if (StormBolt && Aimsharp.CanCast("Storm Bolt")) {
					Aimsharp.PrintMessage("Queued Storm Bolt");
					Aimsharp.Cast("Storm Bolt");
					return true;
				}
				
				// QUEUED INTIMIDATING SHOUT
				if(CDIntimidatingShoutRemains > 25000 && IntimidatingShout) {
					Aimsharp.Cast("IntimidatingShoutOff");
					return true;
				}
				
				if (IntimidatingShout && Aimsharp.CanCast("Intimidating Shout")) {
					Aimsharp.PrintMessage("Queued Intimidating Shout");
					Aimsharp.Cast("Intimidating Shout");
					return true;
				}
				
				// QUEUED RALLYING CRY
				if(CDRallyingCryRemains > 25000 && RallyingCry) {
					Aimsharp.Cast("RallyingCryOff");
					return true;
				}
				
				if (RallyingCry && Aimsharp.CanCast("Rallying Cry","player")) {
					Aimsharp.PrintMessage("Queued Rallying Cry");
					Aimsharp.Cast("Rallying Cry");
					return true;
				}
				
				// QUEUED BLADESTORM
				if(CDBladestormRemains > 25000 && Bladestorm) {
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
				
				// Auto Defensive Stance
				if (!Aimsharp.HasBuff("Defensive Stance", "player") && Aimsharp.CanCast("Defensive Stance", "player")) {
					if (PlayerHealth <= GetSlider("Auto Stance @ HP%")) {
						Aimsharp.Cast("Defensive Stance");
						return true;
					}
				}
				
				if (Aimsharp.HasBuff("Defensive Stance", "player") && Aimsharp.CanCast("Defensive Stance", "player")) {
					if (PlayerHealth >= GetSlider("Unstance @ HP%")) {
						Aimsharp.Cast("Defensive Stance");
						return true;
					}
				}
				
				if (Aimsharp.CanCast("Die by the Sword", "player")) {
					if (PlayerHealth <= GetSlider("Auto Die by the Sword @ HP%")) {
						Aimsharp.Cast("Die by the Sword");
						return true;
					}
				}
			}
			#endregion
			
			#region Fury Rotation
			// PVE Rotation
			if (!PVPmode) {
				
				if (Fighting) {
				
					
					//RAMPAGE
					if ((Aimsharp.CanCast("Rampage") || CDRampageRemaining <= 0 && Rage >= 75) && RecklessnessRemains <3000) {
						Aimsharp.Cast("Rampage");
						return true;
					}
					
					//RECKLESSNESS
					if(Aimsharp.CanCast("Recklessness", "player") && !NoCooldowns) {
						if((MajorPower != "Condensed Life-Force" && MajorPower != "Blood of the Enemy") || CDGuardianOfAzerothRemains > 1 || BuffGuardianOfAzerothUp || CDBloodOfTheEnemyRemains < GCD) {
							Aimsharp.Cast("Recklessness");
							return true;
						}
					}
					
					//WHIRLWIND PROC
					if(EnemiesInMelee > 1 && !WhirlwindUp && Aimsharp.CanCast("Whirlwind", "player")) {
						Aimsharp.Cast("Whirlwind");
						return true;
					}
					
					
					
					
					//NO AOE
					if (EnemiesInMelee >= 1) {
						
						if(Aimsharp.CanCast("Siegebreaker") || CDSiegebreakerRemaining <= 0) {
							Aimsharp.Cast("Siegebreaker");
							return true;
						}
						
						if(Aimsharp.CanCast("Rampage") || (CDRampageRemaining <= 0 && Rage >= 75)) {
							if((RecklessnessUp || BuffMemoryOfLucidDreamsUp) || (TalentFrothingBerserker || TalentCarnage && (BuffEnrageRemains < GCD || Rage > 90) || TalentMassacre && (BuffEnrageRemains<GCD || Rage > 90))) {
								Aimsharp.Cast("Rampage");
								return true;
								
							}
						}
						
						if(Aimsharp.CanCast("Execute")) {
							Aimsharp.Cast("Execute");
							return true;
						}
						
						if(Aimsharp.CanCast("Furious Slash")) {
							
							if(!BloodlustUp && FuriousSlashRemains <3000) {
								Aimsharp.Cast("Furious Slash");
								return true;
							}
						}

						if (Aimsharp.CanCast("Bladestorm", "player") && Aimsharp.LastCast() == "Rampage") {
							Aimsharp.Cast("Bladestorm");
							return true;
						}
						
							
						
						
						if(Aimsharp.CanCast("Bloodthirst") || CDBloodthirstRemaining <= 0) {
							if(!EnrageUp || ColdSteel > 1 ) {
								Aimsharp.Cast("Bloodthirst");
								return true;
							}
						}
						
						if(Aimsharp.CanCast("Dragon Roar", "player") || CDDragonRoarRemaining <=0) {
							
							
							if(EnrageUp) {
								Aimsharp.Cast("Dragon Roar");
								return true;
							}
						}
						
						if((Aimsharp.CanCast("Raging Blow") || CDRagingBlowRemaining <= 0)&& RagingBlowStacks==2) {
							Aimsharp.Cast("Raging Blow");
							return true;
						}
						
						if(Aimsharp.CanCast("Bloodthirst") || CDBloodthirstRemaining <= 0) {
							Aimsharp.Cast("Bloodthirst");
							return true;
						}
						
						if(Aimsharp.CanCast("Raging Blow") || CDRagingBlowRemaining <= 0) {
							if(TalentCarnage || (TalentMassacre && Rage<80) || (TalentFrothingBerserker && Rage <90)) {
								Aimsharp.Cast("Raging Blow");
								return true;
							}
						}
						
						if(Aimsharp.CanCast("Furious Slash")) {
							Aimsharp.Cast("Furious Slash");
							return true;
						}
						
						if(Aimsharp.CanCast("Whirlwind", "player")) {
							Aimsharp.Cast("Whirlwind");
							return true;
						}
						
						
						
						
						//SINGLE TARGET OVER
						
						
					}
				}
				}
			#endregion
			
			
			
			
			
			
			return false;
			
			
		}
		
		public override bool OutOfCombatTick() {
			bool Prepull = Aimsharp.IsCustomCodeOn("savepp");
			
			if (Aimsharp.CanCast("Battle Shout", "player") && !Aimsharp.HasBuff("Battle Shout", "player", false)) {
				Aimsharp.Cast("Battle Shout");
				return true;
			}
			
			
			return false;
		}
	}
}																																																												