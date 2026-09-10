using HarmonyLib;
using UnityEngine;
using static DiamondRemover.LogTools;

namespace DiamondRemover;

// Tools/Summoners/*Summoner.SummonerRevive.revivalMinutes
[HarmonyPatch(typeof(SummonerRevive))]
public static class SummonerRevivePatch
{
  [HarmonyPrefix]
  [HarmonyPatch("KillPet")]
  public static void KillPetPrefix(SummonerRevive __instance)
  {
    __instance.revivalMinutes = 5.0;
  }
}

[HarmonyPatch(typeof(SummonerRevive), "OnEnable")]
public static class InventoryPetButtonBGRemover2
{

  public static void Prefix(SummonerRevive __instance)
  {
    Transform petRevivalButton = __instance.revivalSpeedUp.gameObject.transform;

    Transform backgroundObj = petRevivalButton.transform.Find("SpeedUpButton/SlicedSprite");
    if (backgroundObj == null) return;
    Object.Destroy(backgroundObj.gameObject);
    Verbose("Deleted Sliced Sprite");
  }

}

[HarmonyPatch(typeof(SummonerRevive), "OnPurchase")]
public static class InventoryPetButtonDiamondRevivalDisable
{

  public static bool Prefix()
  {
    return true;
  }
}
