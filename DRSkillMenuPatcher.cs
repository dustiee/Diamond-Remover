using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

using static DiamondRemover.LogTools;

namespace DiamondRemover;

// Diamonds purchase buttons
[HarmonyPatch(typeof(StatsCollector), "Start")]
public class StatsCollector_Start_Patch
{
  static void Prefix(StatsCollector __instance)
  {
    try
    {
      UILabel?[] labels = __instance.purchaseSkillLabel;

      if (labels == null || labels.Length == 0 || labels[0] == null)
      {
        Verbose("purchaseSkillLabel[0] is null or missing");
        return;
      }

      Transform? current = labels[0]!.transform;
      Transform? tableTransform = null;

      while (current != null)
      {
        if (current.name == "Table")
        {
          tableTransform = current;
          break;
        }

        current = current.parent;
      }

      if (tableTransform == null)
      {
        Verbose("'Table' not found in hierarchy");
        return;
      }

      Verbose("Found 'Table'");

      Transform[] allChildren = tableTransform.GetComponentsInChildren<Transform>(true);

      int disabledCount = 0;

      foreach (Transform t in allChildren)
      {
        if (t.name == "PurchaseButton")
        {
          t.gameObject.SetActive(false);
          disabledCount++;
        }
      }

      Verbose($"Disabled {disabledCount} PurchaseButton objects");

      int backCount = 0;
      int titleCount = 0;

      foreach (Transform firstChild in tableTransform)
      {
        foreach (Transform child in firstChild)
        {
          if (child.name == "Back")
          {
            Vector3 scale = child.localScale;
            scale.x = 440f;
            child.localScale = scale;
            backCount++;
          }
          else if (child.name == "Title")
          {
            Vector3 pos = child.localPosition;
            pos.x = 30f;
            child.localPosition = pos;
            titleCount++;
          }
        }
      }

      Verbose($"Modified {backCount} Back objects");
      Verbose($"Modified {titleCount} Title objects");
    }
    catch (System.Exception ex)
    {
      Error($"Exception in StatsCollector.Start prefix: {ex}");
    }
  }
}

[HarmonyPatch(typeof(StatsCollector), "Start")]
public class StatsCollector_Start_PanelPatch
{
  static void Prefix(StatsCollector __instance)
  {
    try
    {
      UILabel?[] labels = __instance.purchaseSkillLabel;

      if (labels == null || labels.Length == 0 || labels[0] == null)
      {
        Verbose("purchaseSkillLabel[0] is null or missing");
        return;
      }

      Transform? current = labels[0]!.transform;
      Transform? skillsCamera = null;

      while (current != null)
      {
        if (current.name == "SkillsCamera")
        {
          skillsCamera = current;
          break;
        }

        current = current.parent;
      }

      if (skillsCamera == null)
      {
        Verbose("'SkillsCamera' not found");
        return;
      }

      Verbose("Found 'SkillsCamera', scanning Panels");

      IEnumerable<Transform>? panels = skillsCamera
          .GetComponentsInChildren<Transform>(true)
          .Where(t => t.name == "Panel");

      int affectedPanels = 0;

      foreach (Transform panel in panels)
      {
        Transform? resetButton = null;
        Transform? diamonds = null;
        Transform? skills = null;

        foreach (Transform child in panel)
        {
          if (child.name == "ResetButton")
          {
            resetButton = child;
          }
          else if (child.name == "Diamonds")
          {
            diamonds = child;
          }
          else if (child.name == "Skills")
          {
            skills = child;
          }
        }

        if (resetButton != null && diamonds != null && skills != null)
        {
          resetButton.gameObject.SetActive(false);
          diamonds.gameObject.SetActive(false);
          affectedPanels++;

          Verbose($"Modified Panel: {panel.name}");
        }
      }

      Verbose($"Total Panels modified: {affectedPanels}");
    }
    catch (System.Exception ex)
    {
      Verbose($"Exception in Panel patch: {ex}");
    }
  }
}
