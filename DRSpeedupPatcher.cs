using System;
using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace DiamondRemover;

[HarmonyPatch(typeof(PurchaseSpeedUp), "OnEnable")]
public class PurchaseSpeedUpPatch
{
  static void Postfix(PurchaseSpeedUp __instance)
  {
    __instance.StartCoroutine(FixLayoutNextFrame(__instance));
  }

  static IEnumerator FixLayoutNextFrame(PurchaseSpeedUp instance)
  {
    yield return null;

    if (instance.purchaseButton == null)
    {
      yield break;
    }

    Transform button = instance.purchaseButton.transform;

    Transform diamond = button.Find("DiamondAnchor");
    if (diamond != null)
    {
      GameObject.Destroy(diamond.gameObject);
    }

    Transform extra = button.Find("Back/Extra");
    if (extra != null)
    {
      GameObject.Destroy(extra.gameObject);
    }

    Transform timer = button.Find("Timer");
    if (timer == null)
    {
      yield break;
    }

    Transform t = timer.transform;

    t.localPosition = new Vector3(
        t.localPosition.x + 20,
        t.localPosition.y,
        t.localPosition.z
    );
  }
}

[HarmonyPatch(typeof(PurchaseSpeedUp), "Purchase")]
public class PurchaseSpeedUpPurchasePatch
{
  static bool Prefix(PurchaseSpeedUp __instance)
  {
    Traverse onPurchaseEvent = Traverse.Create(__instance).Field("OnPurchase");

    if (onPurchaseEvent.GetValue() != null)
    {
      Delegate eventDelegate = onPurchaseEvent.GetValue<Delegate>();

      if (eventDelegate != null)
      {
        eventDelegate.DynamicInvoke();
      }
    }

    return false;
  }
}

[HarmonyPatch(typeof(CraftingOutputSlotSpeedUp))]
[HarmonyPatch("OnPurchase")]
public static class CraftingOutputSlotSpeedUp_OnPurchase_Patch
{
  static bool Prefix(CraftingOutputSlotSpeedUp __instance)
  {
    __instance.timedFill.startTime =
        __instance.timedFill.startTime.AddSeconds(-3);

    __instance.timedFill.finalTime =
        __instance.timedFill.finalTime.AddSeconds(-3);

    Traverse.Create(__instance)
        .Method("CalculateMaxCraftTime")
        .GetValue();

    return false;
  }
}

[HarmonyPatch(typeof(RepairSlot))]
[HarmonyPatch("OnPurchase")]
public static class RepairSlot_OnPurchase_Patch
{
  static bool Prefix(RepairSlot __instance)
  {
    Traverse traverse = Traverse.Create(__instance);

    Traverse startTimeProp = traverse.Property("startTime");
    Traverse finalTimeProp = traverse.Property("finalTime");

    startTimeProp.SetValue(
        startTimeProp.GetValue<DateTime>().AddSeconds(-3)
    );

    finalTimeProp.SetValue(
        finalTimeProp.GetValue<DateTime>().AddSeconds(-3)
    );

    Traverse repairTickField = traverse.Field("repairTick");
    DateTime repairTick =
        repairTickField.GetValue<DateTime>().AddSeconds(-3);

    repairTickField.SetValue(repairTick);

    if (repairTick < DateTime.Now)
    {
      traverse.Method("ActivateRepairTick").GetValue();
      traverse.Method("CalculateCost").GetValue();
      traverse.Method("CalculateRepairTime").GetValue();
    }

    return false;
  }
}

[HarmonyPatch]
public static class RepairSlot_PercentRepairPatch
{
  private const float _repairPercentPerTick = 0.01f;

  private static int GetPerTick(InvGameItem repairItem)
  {
    return Mathf.Max(
        1,
        Mathf.FloorToInt(
            repairItem.durability * _repairPercentPerTick
        )
    );
  }

  private static int GetTicksNeeded(InvGameItem repairItem)
  {
    int perTick = GetPerTick(repairItem);
    int damage = repairItem.damage;

    return (damage + perTick - 1) / perTick;
  }

  [HarmonyPatch(typeof(RepairSlot), nameof(RepairSlot.Repair))]
  [HarmonyPrefix]
  private static bool Repair_Prefix(RepairSlot __instance, int count)
  {
    InvGameItem repairItem = __instance.repairItem;
    int damage = repairItem.damage;

    if (damage <= 0)
    {
      Debug.LogWarning(
          "REPAIR: Attempted to repair when item is not damaged"
      );

      return false;
    }

    InvGameItem hammerItem = __instance.hammer;

    if (hammerItem == null)
    {
      return false;
    }

    int perTick = GetPerTick(repairItem);
    int ticksNeeded = GetTicksNeeded(repairItem);
    int ticksToUse = Mathf.Min(count, ticksNeeded);

    int durabilityRestored =
        Mathf.Min(damage, ticksToUse * perTick);

    hammerItem.count -= ticksToUse;
    repairItem.count += durabilityRestored;

    return false;
  }

  [HarmonyPatch(
      typeof(RepairSlot),
      nameof(RepairSlot.RepairCount),
      MethodType.Getter
  )]
  [HarmonyPostfix]
  private static void RepairCount_Postfix(
      RepairSlot __instance,
      ref int __result
  )
  {
    InvGameItem hammerItem = __instance.hammer;
    InvGameItem repairItem = __instance.repairItem;

    if (
        hammerItem != null
        && hammerItem.count > 0
        && repairItem != null
        && !repairItem.filledStack
    )
    {
      __result = Mathf.Min(
          hammerItem.count,
          GetTicksNeeded(repairItem)
      );
    }
    else
    {
      __result = 0;
    }
  }

  [HarmonyPatch(
      typeof(RepairSlot),
      nameof(RepairSlot.RepairPurchaseCount),
      MethodType.Getter
  )]
  [HarmonyPostfix]
  private static void RepairPurchaseCount_Postfix(
      RepairSlot __instance,
      ref int __result
  )
  {
    InvGameItem hammerItem = __instance.hammer;
    InvGameItem repairItem = __instance.repairItem;

    if (
        hammerItem != null
        && hammerItem.count > 0
        && repairItem != null
        && !repairItem.filledStack
    )
    {
      __result = GetTicksNeeded(repairItem);
    }
    else
    {
      __result = 0;
    }
  }
}
