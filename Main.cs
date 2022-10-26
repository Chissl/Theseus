
global using Assets.Scripts.Models.Towers;
global using Assets.Scripts.Models.GenericBehaviors;
global using Assets.Scripts.Unity.Display;
global using Assets.Scripts.Utils;
global using Assets.Scripts.Models.Towers.Behaviors;
global using Assets.Scripts.Models.Towers.Behaviors.Emissions;
global using Assets.Scripts.Models.Towers.Projectiles.Behaviors;
global using Assets.Scripts.Unity;
global using Assets.Scripts.Models.Towers.Behaviors.Abilities;
global using Assets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
global using Assets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
global using Assets.Scripts.Models.Towers.Weapons.Behaviors;
global using BTD_Mod_Helper;
global using BTD_Mod_Helper.Extensions;
global using BTD_Mod_Helper.Api.Towers;
global using System.Linq;
global using uObject = UnityEngine.Object;
global using HarmonyLib;
global using MelonLoader;
global using UnityEngine;
global using System;
global using System.Reflection;
global using Theseus;
using Assets.Scripts.Simulation.Towers.Weapons;
using BTD_Mod_Helper.Api.Display;
using UnityEngine.UI;
using Assets.Scripts.Simulation.Towers;

[assembly: MelonInfo(typeof(Theseus.Main), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace Theseus;

public class Main : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {

        foreach (var asset in MelonAssembly.Assembly.GetManifestResourceNames())
            MelonLogger.Msg(asset);
        //previous two lines are for debugging/finding names of assets

        assetBundle = AssetBundle.LoadFromMemory(ExtractResource("doublegun.bundle"));// if using unityexplorer, there is an error, but everything still works
        ModHelper.Msg<Main>("Theseus");
    }

    public static AssetBundle assetBundle;


    private byte[] ExtractResource(String filename)
    {
        Assembly a = MelonAssembly.Assembly; // get the assembly
        return a.GetEmbeddedResource(filename).GetByteArray(); // get the embedded bundle as a raw file that unity can read
    }
}

[HarmonyPatch(typeof(Factory.__c__DisplayClass21_0), nameof(Factory.__c__DisplayClass21_0._CreateAsync_b__0))]
static class FactoryCreateAsyncPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref Factory.__c__DisplayClass21_0 __instance, ref UnityDisplayNode prototype)
    {
        GameObject gObj;

        switch (__instance.objectId.guidRef) // makes sure to support loading more than one custom display
        {
            case "TheseusBase-Prefab":
                gObj = UnityEngine.Object.Instantiate(Main.assetBundle.LoadAsset("xxl").Cast<GameObject>(), __instance.__4__this.DisplayRoot); //load the asset from the asset bundle and instantiates/creates it
                break;
            case "Point Light":
                gObj = UnityEngine.Object.Instantiate(Main.assetBundle.LoadAsset("untitled").Cast<GameObject>(), __instance.__4__this.DisplayRoot); //load the asset from the asset bundle and instantiates/creates it
                break;
            case "Laser":
                gObj = UnityEngine.Object.Instantiate(Main.assetBundle.LoadAsset("OrbitalStrike").Cast<GameObject>(), __instance.__4__this.DisplayRoot);
                break;
            case "Lvl7":
                gObj = UnityEngine.Object.Instantiate(Main.assetBundle.LoadAsset("Lvl7").Cast<GameObject>(), __instance.__4__this.DisplayRoot);
                break;
            case "Lvl18":
                gObj = UnityEngine.Object.Instantiate(Main.assetBundle.LoadAsset("Lvl18").Cast<GameObject>(), __instance.__4__this.DisplayRoot);
                break;
            case "Lvl20":
                gObj = UnityEngine.Object.Instantiate(Main.assetBundle.LoadAsset("Lvl20").Cast<GameObject>(), __instance.__4__this.DisplayRoot);
                break;
            default:
                return true; //if the display is not custom, let the game create the base display
        }
        gObj.name = __instance.objectId.guidRef; //should be optional in theory, but i left it because its good for debugging/organization
        gObj.transform.position = new Vector3(Factory.kOffscreenPosition.x, 0, 0); //move the object offscreen so the game doesn't try to render it when its not needed 
        
        gObj.AddComponent<UnityDisplayNode>(); //adds a UnityDisplayNode component to the object, this is needed for the game to recognize it as a display
        var outline = gObj.AddComponent<Outline>();
        
        prototype = gObj.GetComponent<UnityDisplayNode>(); //gets the UnityDisplayNode component from the object
        
        __instance.__4__this.active.Add(prototype); //adds the object to the active list, this is needed for the game to show the display
        __instance.onComplete.Invoke(prototype); //calls the onComplete delegate thats automatically created by the game, this is needed for the game to use it as a display

        return false; //prevents the game from creating the base display once a custom display is created
    }
}
[HarmonyPatch(typeof(Weapon), "SpawnDart")]
public class WeaponSpawnDart_Patch
{
    [HarmonyPostfix]
    public static void Postfix(ref Weapon __instance)
    {
        //MelonLogger.Msg(__instance.attack.tower.towerModel.name);
        if (__instance.attack.tower.towerModel.name.StartsWith("Theseus"))
        {
            __instance.attack.tower.Node.graphic.GetComponent<Animator>().Play("AttackR"
                , 0, 1-(.5f*__instance.attack.tower.towerModel.GetAttackModel().weapons[0].rate));
        }
    }
}


