using HarmonyLib;
using UnityEngine;
using static DiamondRemover.LogTools;
namespace DiamondRemover;


[HarmonyPatch(typeof(StoreInventory), nameof(StoreInventory.GetRandomItems))]
public static class StoreInventoryPatch
{
  private static void Prefix(StoreInventory __instance)
  {
    int removedCount = __instance.possibleItems.RemoveAll(item => item.ask.name == "Diamond");

    if (removedCount > 0)
      Verbose($"Removed {removedCount} store items involving diamonds.");

  }
}


[HarmonyPatch(typeof(Inventory), "ShowStoreNPC")]
public static class ShowStoreNPC_Patch
{
  private static void Prefix(Inventory __instance)
  {
    if (__instance.storeNPC == null) return;

    Transform diamonds = __instance.storeNPC.transform.Find("Transform/Panel/Diamonds");

    if (diamonds == null)
    {
      return;
    }

    Object.Destroy(diamonds.gameObject);
  }
}
