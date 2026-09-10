using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace DiamondRemover;

[HarmonyPatch(typeof(DisplayRecipe))]
[HarmonyPatch("ShowRecipe")]
public class DisplayRecipePatch
{
  private const float _adjustment = 155f;

  static void Postfix(DisplayRecipe __instance)
  {
    // Hide buy button
    if (__instance.buy != null)
    {
      __instance.buy.SetActive(false);
    }

    GameObject achieveCamera = GameObject.Find("AchieveCamera");

    bool isUnderAchieveCamera =
        achieveCamera != null && __instance.transform.IsChildOf(achieveCamera.transform);

    // Doing this so achievements don't get messed up
    if (!isUnderAchieveCamera)
    {
      __instance.StartCoroutine(ApplyOffsetDelayed(__instance));
      RepositionNGUIElements(__instance);
    }
  }

  static IEnumerator ApplyOffsetDelayed(DisplayRecipe instance)
  {
    // Wait for NGUI layout/repositioning to finish
    yield return null;
    // yield return null;

    Vector3 pos = instance.transform.localPosition;
    pos.x = _adjustment;
    instance.transform.localPosition = pos;

  }

  static void RepositionNGUIElements(DisplayRecipe instance)
  {
    UIGrid[] grids = instance.GetComponentsInChildren<UIGrid>();
    UITable[] tables = instance.GetComponentsInChildren<UITable>();

    foreach (UIGrid grid in grids)
    {
      grid.Reposition();
    }

    foreach (UITable table in tables)
    {
      table.Reposition();
    }

    UIPanel panel = instance.GetComponent<UIPanel>();
    if (panel != null)
    {
      panel.Refresh();
    }
  }
}
