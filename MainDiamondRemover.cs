using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DiamondRemover;

[BepInPlugin("dev.dustie.diamondremover", "Diamond Remover", "1.0.0")]
public class DiamondRemover : BaseUnityPlugin
{
  internal static ManualLogSource? Log;

  private static ConfigEntry<bool>? _configMuteVerbose;
  private const bool _muteVerboseDefault = true;

  internal static bool Verbose
  {
    get => !_configMuteVerbose?.Value ?? _muteVerboseDefault;
  }

  private void Awake()
  {

    Log = Logger;

    Configure();
    var harmony = new Harmony("dev.dustie.diamondremover");
    harmony.PatchAll();
  }

  private void Configure()
  {
    _configMuteVerbose = Config.Bind(
        "Options",
        "Mute Verbose",
        _muteVerboseDefault,
        "Should only enable this for development or debugging. Will likely spam your log file."
        );
  }
}
