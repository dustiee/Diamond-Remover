using HarmonyLib;

namespace DiamondRemover;

[HarmonyPatch(typeof(DisplayCurrency))]
[HarmonyPatch("OnEnable")]
public class DisplayCurrencyPatch
{
  static bool Prefix(DisplayRecipe __instance)
  {
    return false;
  }
}

[HarmonyPatch(typeof(DisplayCurrency))]
[HarmonyPatch("OnCurrencyChange")]
public class DisplayCurrencyChangePatch
{
  static bool Prefix(DisplayRecipe __instance, int points)
  {
    return false;
  }
}

[HarmonyPatch(typeof(DisplayCurrency))]
[HarmonyPatch("SetLabel")]
public class DisplayLabelPatch
{
  static bool Prefix(DisplayRecipe __instance, int points)
  {
    return false;
  }
}
