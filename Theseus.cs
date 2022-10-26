using Assets.Scripts.Models.Towers.Behaviors;
using Assets.Scripts.Models.Towers.Behaviors.Abilities;
using Assets.Scripts.Models.Towers.Behaviors.Attack;
using Assets.Scripts.Models.Towers.Filters;
using Assets.Scripts.Simulation.Towers;
using Assets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Assets.Scripts.Simulation.Towers.Weapons;
using Assets.Scripts.Unity.Towers;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using Theseus.Displays;
using JetBrains.Annotations;
using System.Data.SqlTypes;
using CreateEffectOnExpireModel = Assets.Scripts.Models.Towers.Projectiles.Behaviors.CreateEffectOnExpireModel;
using Assets.Scripts.Unity.Towers.Weapons;
using System.Drawing;
using Assets.Scripts.Models.Bloons.Behaviors;

namespace Theseus

{
    public class Theseus : ModHero
    {
        public override string BaseTower => "EngineerMonkey";
        public override string Name => "Theseus";
        public override int Cost => 975;
        public override string DisplayName => "Theseus";
        public override string Title => "The Adventurer";
        public override string Level1Description => "Pops bloons with his laser guns, getting extra cash per pop.";
        public override string Description => "With equipment and experience from his decades of scouring the Bloon Wasteland, Theseus decimates bloons with his powerful collection of laser weapons. Tip: Use your extra cash to level up this hero.";
        public override string NameStyle => TowerType.Gwendolin;
        public override int MaxLevel => 20;
        public override float XpRatio => 2f;

        public override void ModifyBaseTowerModel(TowerModel towerModel)
        {
            towerModel.range = 40;
            towerModel.GetAttackModel().range = 40;

            towerModel.AddBehavior(new CashIncreaseModel("CashIncrease", 0, 2f)); //remember to set back to 2

            towerModel.display = new() { guidRef = "TheseusBase-Prefab" }; //required for custom displays to be recognized
            towerModel.GetBehavior<DisplayModel>().display = new() { guidRef = "TheseusBase-Prefab" }; //required for custom displays to be recognized
            towerModel.GetBehavior<DisplayModel>().positionOffset = new Assets.Scripts.Simulation.SMath.Vector3(0, 0, -.15f);
            towerModel.GetAttackModel().weapons[0] = Game.instance.model.GetTowerFromId("EngineerMonkey-003").GetAttackModel().weapons[0].Duplicate();
            towerModel.radius = 8;
            var weapons = towerModel.GetAttackModel().weapons[0];
            weapons.rate = .6f;
            weapons.projectile.pierce = 4;
            weapons.projectile.GetDamageModel().damage = 2;
            weapons.projectile.RemoveBehavior<SlowOnPopModel>();
            weapons.projectile.GetDamageModel().immuneBloonProperties = BloonProperties.Purple;
            weapons.projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 3, false, true));
            weapons.projectile.ApplyDisplay<BlueLaserDisplay>();
            weapons.projectile.scale *= .8f;
            weapons.ejectY += 5;
            weapons.ejectZ += 2;
        }
        public class L2 : ModHeroLevel<Theseus>
        {
            public override string Description => "Lasers have more popping power.";
            public override int Level => 2;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetAttackModel().weapons[0].projectile.pierce += 3;
            }
        }
        public class L3 : ModHeroLevel<Theseus>
        {
            public override string AbilityName => "Orbital Strike";
            public override string AbilityDescription => "Theseus calls a devastating Orbital Strike at his targeted location.";
            public override string Description => $"{AbilityName}: {AbilityDescription}";
            public override int Level => 3;

            public override void ApplyUpgrade(TowerModel towerModel)
            {

                var ability3 = Game.instance.model.GetTowerFromId("ObynGreenfoot 10").GetBehaviors<AbilityModel>()[1].Duplicate();
                var ability3attack = ability3.GetBehavior<ActivateAttackModel>();
                var ability3weapon = Game.instance.model.GetTowerFromId("MortarMonkey").GetAttackModel().weapons[0].Duplicate();

                ability3weapon.rate = .2f;
                ability3weapon.GetDescendant<RandomTargetSpreadModel>().spread = 30;
                ability3weapon.projectile.GetBehavior<CreateEffectOnExhaustFractionModel>().effectModel.assetId = CreatePrefabReference<OrbitalStrikeDisplay>();
                var ability3projectile = ability3weapon.projectile;
                var ability3explosion = ability3projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile;

                ability3projectile.GetBehavior<CreateEffectOnExpireModel>().assetId = new Assets.Scripts.Utils.PrefabReference() { guidRef = "6d84b13b7622d2744b8e8369565bc058" };
                ability3weapon.RemoveBehavior<EjectEffectModel>();
                ability3explosion.radius = 15;
                ability3explosion.GetDamageModel().damage = 3;
                ability3explosion.GetDamageModel().immuneBloonProperties = BloonProperties.None;
                ability3explosion.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 3, false, true));
                ability3attack.lifespan = 1.5f;
                ability3weapon.RemoveBehavior<RotateToTargetModel>();
                ability3attack.attacks[0].weapons[0] = ability3weapon;
                ability3attack.attacks[0].RemoveBehavior<RotateToTargetModel>();

                ability3.displayName = AbilityName;
                ability3.addedViaUpgrade = Id;
                ability3.cooldown = 40;
                ability3.icon = GetSpriteReference(mod, "OrbitalStrike-Icon");

                towerModel.AddBehavior(ability3);
                towerModel.towerSelectionMenuThemeId = "SelectPointInput";
                towerModel.GetDescendant<TargetSelectedPointModel>().lockToInsideTowerRange = false;
            }
        }
        public class L4 : ModHeroLevel<Theseus>
        {
            public override string Description => "Increased range and Orbital Strike radius is increased.";
            public override int Level => 4;
            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.range += 15;
                towerModel.GetAttackModel().range += 15;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.radius += 5;

            }
        }
        public class L5 : ModHeroLevel<Theseus>
        {
            public override string Description => "Laser Weapon and Orbital Strike damage increased.";
            public override int Level => 5;
            public override void ApplyUpgrade(TowerModel towerModel)
            {

                towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage += 1;
                towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 4, false, true));
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetDamageModel().damage += 2;
            }
        }
        public class L6 : ModHeroLevel<Theseus>
        {
            public override string Description => "Night-Vision Goggles grants Theseus and nearby towers camo detection and increased range.";
            public override int Level => 6;
            public override int Priority => -1;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.AddBehavior(new OverrideCamoDetectionModel("CamoDetection", true));
                towerModel.range += 5;
                towerModel.GetAttackModel().range += 5;
                var camosupport = Game.instance.model.GetTowerFromId("MonkeyVillage-020").GetBehavior<VisibilitySupportModel>();
                var rangesupport = Game.instance.model.GetTowerFromId("MonkeyVillage-020").GetBehavior<RangeSupportModel>();
                towerModel.AddBehavior(rangesupport);
                towerModel.AddBehavior(camosupport);
            }
        }
        public class L7 : ModHeroLevel<Theseus>
        {
            public override string Description => "Lasers have even more popping power. Archimedes cooldown reduced.";
            public override int Level => 7;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetAttackModel().weapons[0].projectile.pierce += 2;
                towerModel.GetAbility(0).cooldown *= .8f;
                towerModel.display = new() { guidRef = "Lvl7" };
                towerModel.GetBehavior<DisplayModel>().display = new() { guidRef = "Lvl7" };
            }
        }
        public class L8 : ModHeroLevel<Theseus>
        {
            public override string Description => "Shoots lasers faster!";
            public override int Level => 8;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetAttackModel().weapons[0].rate *= .75f;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].rate -= .1f;
            }
        }
        public class L9 : ModHeroLevel<Theseus>
        {
            public override string Description => "Cash per pop increased.";
            public override int Level => 9;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetBehavior<CashIncreaseModel>().multiplier *= 2;
            }
        }
        public class L10 : ModHeroLevel<Theseus>
        {
            public override int Level => 10;
            public override string AbilityName => "Disruptor Shot";
            public override string AbilityDescription => "Shoots a crippling plasma blast at the largest Bloon on screen, making it more vulnerable to damage!";
            public override string Description => $"{AbilityName}: {AbilityDescription}";

            public override void ApplyUpgrade(TowerModel towerModel)
            {

                var ability10 = Game.instance.model.GetTowerFromId("BombShooter-040").GetBehavior<AbilityModel>().Duplicate();
                var projectile = ability10.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile;
                var explosion = projectile.GetBehavior<CreateProjectileOnContactModel>().projectile;
                explosion.radius = 30;
                explosion.pierce = 10;
                explosion.GetDamageModel().damage = 200;
                explosion.collisionPasses = new[] { -1, 0 };
                var explosionslowmodel = Game.instance.model.GetTowerFromId("BombShooter-500").GetDescendant<SlowModel>().Duplicate();
                explosionslowmodel.lifespan = 1.5f;
                explosion.AddBehavior(explosionslowmodel);
                var superbrittle = Game.instance.model.GetTowerFromId("IceMonkey-500").GetDescendant<AddBonusDamagePerHitToBloonModel>().Duplicate();
                superbrittle.perHitDamageAddition = 4;
                superbrittle.lifespan = 3;
                superbrittle.mutationId = "DisruptorDebuff";
                ability10.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].ejectY = 10;
                explosion.GetDamageModel().immuneBloonProperties = BloonProperties.None;
                projectile.GetDamageModel().damage = 1000;
                explosion.collisionPasses = new[] { -1, 0 };
                projectile.collisionPasses = new[] { -1, 0 };
                projectile.ApplyDisplay<DisruptorProjectileDisplay>();
                projectile.GetBehavior<TravelStraitModel>().speed *= .5f;
                projectile.scale *= 1.2f;
                var slowmodel = explosionslowmodel.Duplicate();
                slowmodel.lifespan = 3;
                projectile.AddBehavior(slowmodel);
                projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 15000, false, true));
                ability10.cooldown = 75;
                explosion.AddBehavior(superbrittle);
                projectile.AddBehavior(superbrittle);
                var lasershock = Game.instance.model.GetTowerFromId("DartlingGunner-200").GetDescendant<AddBehaviorToBloonModel>().Duplicate();
                //lasershock.filters = null;
                lasershock.GetBehavior<DamageOverTimeModel>().damage = 0;
                lasershock.lifespan = superbrittle.lifespan * 1.5f;
                explosion.AddBehavior(lasershock);
                projectile.AddBehavior(lasershock);
                ability10.icon = GetSpriteReference(mod, "DisruptorShot-Icon");
                towerModel.AddBehavior(ability10);
            }
        }
        public class L11 : ModHeroLevel<Theseus>
        {
            public override string Description => "Upgraded Ion Lasers make bloons take more damage.";
            public override int Level => 11;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var superbrittle = Game.instance.model.GetTowerFromId("IceMonkey-500").GetDescendant<AddBonusDamagePerHitToBloonModel>().Duplicate();
                superbrittle.perHitDamageAddition = 2;
                superbrittle.lifespan = 3;
                superbrittle.mutationId = "BasicDebuff";
                var lasershock = Game.instance.model.GetTowerFromId("DartlingGunner-200").GetDescendant<AddBehaviorToBloonModel>().Duplicate();
                lasershock.GetBehavior<DamageOverTimeModel>().damage = 0;
                lasershock.lifespan = superbrittle.lifespan;
                towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(lasershock);
                towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(superbrittle);
                towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new[] { 0, 1 };
            }
        }
        public class L12 : ModHeroLevel<Theseus>
        {
            public override string Description => "Lasers deal more damage to Ceramic and Fortified Bloons.";
            public override int Level => 12;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var projectile = towerModel.GetAttackModel().weapons[0].projectile;
                var ability1projectile = towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile;
                projectile.AddBehavior(new DamageModifierForTagModel("Ceramic", "Ceramic", 1, 4, false, true));
                projectile.AddBehavior(new DamageModifierForTagModel("Fortified", "Fortified", 1, 3, false, true));
                projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 6, false, true));
                projectile.GetDamageModel().damage += 3;
                ability1projectile.AddBehavior(new DamageModifierForTagModel("Fortified", "Fortified", 1, 3, false, true));
                ability1projectile.AddBehavior(new DamageModifierForTagModel("Ceramic", "Ceramic", 1, 3, false, true));
                ability1projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 8, false, true));

            }
        }
        public class L13 : ModHeroLevel<Theseus>
        {
            public override string Description => "Orbital Strike deals significantly more damage. Disruptor Shot stun duration increased.";
            public override int Level => 13;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].rate *= .5f;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetDamageModel().damage += 4;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().lifespan += 1;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 18, false, true));
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<SlowModel>().lifespan *= 1.5f;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().lifespan *= 1.5f;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().lifespan *= 1.5f;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().lifespan *= 1.5f;

            }
        }
        public class L14 : ModHeroLevel<Theseus>
        {
            public override string Description => "Lasers deal more damage to Moab-class bloons and can now pop purples.";
            public override int Level => 14;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
                towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Moabs", "Moabs", 1, 4, false, false));
                towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 12, false, true));
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.AddBehavior(new DamageModifierForTagModel("Moabs", "Moabs", 1, 12, false, true));
            }
        }
        public class L15 : ModHeroLevel<Theseus>
        {
            public override string Description => "Laser attacks and abilities have larger range.";
            public override int Level => 15;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.range += 15;
                towerModel.GetAttackModel().range += 15;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.radius += 10;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].GetDescendant<RandomTargetSpreadModel>().spread += 20;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.radius += 10;
            }
        }
        public class L16 : ModHeroLevel<Theseus>
        {
            public override string Description => "Disruptor shot power significantly increased.";
            public override int Level => 16;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var ability10 = towerModel.GetAbility(1);
                var projectile = ability10.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile;
                var explosionprojectile = projectile.GetBehavior<CreateProjectileOnContactModel>().projectile;

                projectile.GetBehavior<SlowModel>().lifespan *= 1.5f;
                explosionprojectile.GetBehavior<SlowModel>().lifespan *= 1.5f;
                explosionprojectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().lifespan *= 1.5f;
                explosionprojectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().perHitDamageAddition += 3;
                explosionprojectile.pierce += 25;
                explosionprojectile.GetDamageModel().damage += 800;
                projectile.GetDamageModel().damage += 4000;
                projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 75000, false, true));
                ability10.cooldown -= 15;
            }
        }
        public class L17 : ModHeroLevel<Theseus>
        {
            public override string Description => "Orbital Strike duration and rate increased";
            public override int Level => 17;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].rate *= .5f;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().lifespan *= 1.25f;
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateEffectOnExpireModel>().assetId = new PrefabReference { guidRef = "687e8d737f9d8874d9197037d9971c59" };
                //towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateEffectOnExpireModel>().projectile.display = new PrefabReference { guidRef = "687e8d737f9d8874d9197037d9971c59" };
            }
        }
        public class L18 : ModHeroLevel<Theseus>
        {
            public override string Description => "Attacks faster and deals more damage.";
            public override int Level => 18;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                var weapon = towerModel.GetAttackModel().weapons[0];
                weapon.rate *= .75f;
                weapon.projectile.GetDamageModel().damage += 2;
                weapon.projectile.AddBehavior(new DamageModifierForTagModel("Moabs", "Moabs", 1, 10, false, false));
                towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 28, false, true));
                weapon.projectile.pierce += 5;
                towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().perHitDamageAddition += 6;
                towerModel.display = new() { guidRef = "Lvl18" };
                towerModel.GetBehavior<DisplayModel>().display = new() { guidRef = "Lvl18" };
            }
        }
       
        public class L19 : ModHeroLevel<Theseus>
        {
            public override string Description => "Disruptor shot duration and damage improved. Orbital strike damage improved.";
            public override int Level => 19;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.AddBehavior(new DamageModifierForTagModel("Moabs", "Moabs", 1, 40, false, true));
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 200, false, true));
                towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetDamageModel().damage += 15;

                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.AddBehavior(new DamageModifierForTagModel("Moabs", "Moabs", 1, 40, false, true));
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 30000, false, true));
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetDamageModel().damage += 5000;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetDamageModel().damage += 30000;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 500000, false, true));
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<SlowModel>().lifespan *= 1.5f;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<SlowModel>().lifespan *= 1.5f;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().lifespan *= 1.5f;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnContactModel>().projectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().perHitDamageAddition *= 3;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().perHitDamageAddition *= 3;
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<AddBonusDamagePerHitToBloonModel>().lifespan *= 1.5f;
            }
        }
        public class L20 : ModHeroLevel<Theseus>
        {
            public override string AbilityName => "Project Archimedes";
            public override string AbilityDescription => "I am become Death, the destroyer of worlds...";
            public override string Description => $"{AbilityName}: {AbilityDescription}";
            public override int Level => 20;

            public override void ApplyUpgrade(TowerModel towerModel)
            {
                towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<RedLaserDisplay>();
                towerModel.GetAbility(1).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.ApplyDisplay<Lvl20DisruptorProjectileDisplay>();
                towerModel.display = new() { guidRef = "Lvl20" };
                towerModel.GetBehavior<DisplayModel>().display = new() { guidRef = "Lvl20" };
                var archimedes = towerModel.GetAbility(0).Duplicate();
                archimedes.icon = GetSpriteReference(mod, "ProjectArchimedes-Icon");
                //archimedes.GetBehavior<ActivateAttackModel>().attacks[0] = Game.instance.model.GetTowerFromId("MortarMonkey-002").GetAttackModel(0).Duplicate();
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].fireWithoutTarget = true;
                archimedes.GetBehavior<ActivateAttackModel>().cancelIfNoTargets = false;
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].RemoveBehavior<TargetSelectedPointModel>();
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].targetProvider = new CloseTargetTrackModel("", 999999, false, false, 0);
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].AddBehavior(new CloseTargetTrackModel("", 999999, false, false, 0));

                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile = Game.instance.model.GetTowerFromId("MonkeyAce-050").GetAttackModel(1).weapons[0].projectile.Duplicate();
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<FallToGroundModel>().timeToTake = 10;
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.display = new() { guidRef = "" };
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<DisplayModel>().display = new() { guidRef = "" };
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetDamageModel().damage = 100000;
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.pierce = 10000;
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.AddBehavior(new DamageModifierForTagModel("Boss", "Boss", 1, 250000, false, true));
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.radius = 999999;
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.display = new() { guidRef = "" };
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<DisplayModel>().display = new() { guidRef = "" };
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.RemoveBehavior<CreateEffectOnExpireModel>();
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.RemoveBehavior<CreateEffectOnContactModel>();
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].projectile.RemoveBehavior<CreateEffectOnExhaustFractionModel>();
                var lighteffect = towerModel.GetAbility(0).GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].Duplicate();
                var dummy = Game.instance.model.GetTowerFromId("MonkeyVillage").Duplicate();
                dummy.ignoreTowerForSelection = true;
                dummy.display = new() { guidRef = "Point Light" };
                dummy.GetBehavior<DisplayModel>().display = new() { guidRef = "Point Light" };
                dummy.RemoveBehavior<RangeSupportModel>();
                dummy.AddBehavior(new TowerExpireModel("", 13.5f, 1, false, false));
                dummy.doesntRotate = true;
                lighteffect.fireWithoutTarget = true;
                lighteffect.rate = 30;
                lighteffect.projectile.AddBehavior(new CreateTowerModel("", dummy, 0, true, true, false, false, true));
                lighteffect.projectile.RemoveBehavior<CreateEffectOnExhaustFractionModel>();
                lighteffect.projectile.RemoveBehavior<CreateProjectileOnExhaustFractionModel>();
                //lighteffect.projectile.GetBehavior<CreateEffectOnExpireModel>().assetId = new() { guidRef = "Laser" };
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].AddWeapon(lighteffect);
                archimedes.name = "Archimedes II";
                archimedes.displayName = "Archimedes II";
                archimedes.GetBehavior<ActivateAttackModel>().attacks[0].weapons[0].rate = .05f;
                archimedes.GetBehavior<ActivateAttackModel>().lifespan = 1;
                archimedes.cooldown = 240;
                var abilitydisplay = Game.instance.model.GetTowerFromId("MonkeyAce-050").GetAbility(0).GetBehaviors<CreateEffectOnAbilityModel>()[1].Duplicate();
                //abilitydisplay.effectModel.lifespan = archimedes.GetBehavior<ActivateAttackModel>().lifespan;

                archimedes.AddBehavior(abilitydisplay);
                towerModel.AddBehavior(archimedes);

            }
        }
    }
}

