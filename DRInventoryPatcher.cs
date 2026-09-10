using HarmonyLib;
using UnityEngine;

using static DiamondRemover.LogTools;

namespace DiamondRemover;

[HarmonyPatch(typeof(Inventory))]
[HarmonyPatch("Awake")]
public class InventoryAwakePatch
{
  static void Postfix(Inventory __instance)
  {
    var root = __instance.hud?.transform;

    if (root == null)
    {
      return;
    }

    // Existing diamond removal
    var diamond = root.Find("Diamond");

    if (diamond != null)
    {
      diamond.gameObject.SetActive(false);
    }

    var questRoot = __instance.questGameObject.transform.parent.gameObject;

    DisableChild(questRoot, "PurchaseDiamondsWindow");
    DisableChild(questRoot, "SkillsCamera/Panel/Diamonds/CurDiamonds");
    DisableChild(questRoot, "FortunaWheel");
    DisableChild(questRoot, "GiftDialog");
    DisableChild(questRoot, "RecipeBook/Panel/Diamonds");
    DisableChild(questRoot, "AchieveCamera/Panel/Diamonds");
    DisableChild(questRoot, "RecipeBook/Panel/DiamondsNoIap");
    DisableChild(questRoot, "DiamondGiftWindow");

  }

  private static void DisableChild(GameObject root, string path)
  {
    var obj = root.transform.Find(path);

    if (obj != null)
    {
      obj.gameObject.SetActive(false);
      Verbose($"Disabled: {path}");
    }
    else
    {
      Verbose($"Missing: {path}");
    }
  }
}
