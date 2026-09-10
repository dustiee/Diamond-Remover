using HarmonyLib;
using UnityEngine;

namespace DiamondRemover;

[HarmonyPatch(typeof(RecipeItemDescription))]
public static class RecipeItemDescriptionPatch
{
  [HarmonyPatch(
      "Initialize",
      [typeof(InvGameItem), typeof(Color), typeof(NumberExperiment), typeof(bool)]
  )]
  [HarmonyPostfix]
  private static void InitializePostfix(RecipeItemDescription __instance)
  {
    if (__instance.purchaseButton != null)
    {
      __instance.purchaseButton.SetActive(false);
    }

    if (__instance.amountSlider != null)
    {
      __instance.amountSlider.gameObject.SetActive(false);
    }

    if (__instance.itemCount != null)
    {
      __instance.itemCount.gameObject.SetActive(false);
    }

    if (__instance.itemCost != null)
    {
      __instance.itemCost.gameObject.SetActive(false);
    }

    var returnLabel = __instance
        .panel.transform.Find("Transform/Button/Name")
        ?.GetComponent<UILabel>();

    if (returnLabel != null)
    {
      returnLabel.text = "Ok";
    }
  }
}
