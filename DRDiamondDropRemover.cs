using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using static DiamondRemover.LogTools;

namespace DiamondRemover;

[HarmonyPatch(typeof(Digger))]
public static class DiggerPatch
{
  [HarmonyPostfix]
  [HarmonyPatch("OnEnable")]
  private static void OnEnablePostfix()
  {
    Digger.dropDiamondChance.lootDrop.probability = 0;
  }
}

// Transpiler to remove diamondLoot.AttemptDrop(trans) from the end of TryDropCoin
[HarmonyPatch(typeof(CoinLoot), nameof(CoinLoot.TryDropCoin))]
internal static class CoinLoot_TryDropCoin_Patch
{
  static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
  {
    var matcher = new CodeMatcher(instructions);

    var diamondField = AccessTools.Field(typeof(CoinLoot), nameof(CoinLoot.diamondLoot));

    matcher.MatchForward(
        false,
        new CodeMatch(OpCodes.Ldarg_0),
        new CodeMatch(OpCodes.Ldfld, diamondField),
        new CodeMatch(OpCodes.Ldarg_1),
        new CodeMatch(OpCodes.Callvirt),
        new CodeMatch(OpCodes.Pop)
    );

    if (matcher.IsValid)
    {
      matcher
          .SetAndAdvance(OpCodes.Nop, null) // was ldarg.0
          .SetAndAdvance(OpCodes.Nop, null) // was ldfld
          .SetAndAdvance(OpCodes.Nop, null) // was ldarg.1
          .SetAndAdvance(OpCodes.Nop, null) // was callvirt
          .SetAndAdvance(OpCodes.Nop, null); // was pop
      Print("Removed diamond drop.");
    }
    else
    {
      Print("Failed to find diamond drop.");
    }

    return matcher.InstructionEnumeration();
  }
}
