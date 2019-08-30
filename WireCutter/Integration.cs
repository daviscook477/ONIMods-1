using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Harmony;

using UnityEngine;

namespace WireCutter {
    public static class Mod_OnLoad {
        public static void OnLoad() {
            Assembly currentAssembly = Assembly.GetAssembly(typeof(WireCutterAssets));
            string currentAssemblyDirectory = Path.GetDirectoryName(currentAssembly.Location);
            WireCutterAssets.WIRECUTTER_PATH_CONFIGFILE = currentAssemblyDirectory + "/config.json";
            WireCutterAssets.WIRECUTTER_PATH_KEYCODESFILE = currentAssemblyDirectory + "/keycodes.txt";

            IOUtilities.CreateKeycodeHintFile();
            if (File.Exists(WireCutterAssets.WIRECUTTER_PATH_CONFIGFILE)) {
                IOUtilities.ReadConfig();
            }

            else {
                IOUtilities.CreateDefaultConfig();
            }

            WireCutterAssets.WIRECUTTER_ICON_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("WireCutter.image_wirecutter_button.dds"), 32, 32);
            WireCutterAssets.WIRECUTTER_ICON_SPRITE.name = WireCutterAssets.WIRECUTTER_ICON_NAME;
            WireCutterAssets.WIRECUTTER_VISUALIZER_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("WireCutter.image_wirecutter_visualizer.dds"), 256, 256);

            WireCutterAssets.WIRECUTTER_TOOLCOLLECTION = ToolMenu.CreateToolCollection(WireCutterAssets.WIRECUTTER_NAME, WireCutterAssets.WIRECUTTER_ICON_NAME, Action.NumActions, WireCutterAssets.WIRECUTTER_TOOLNAME, WireCutterAssets.WIRECUTTER_TOOLTIP, true);

            Debug.Log("WireCutter Loaded: Version " + currentAssembly.GetName().Version);
        }
    }

    namespace Patches {
        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public static class PlayerController_OnPrefabInit {
            public static void Postfix(PlayerController __instance) {
                List<InterfaceTool> interfaceTools = new List<InterfaceTool>(__instance.tools);


                GameObject wireCutterTool = new GameObject(WireCutterAssets.WIRECUTTER_TOOLNAME);
                wireCutterTool.AddComponent<WireCutterTool>();

                wireCutterTool.transform.SetParent(__instance.gameObject.transform);
                wireCutterTool.gameObject.SetActive(true);
                wireCutterTool.gameObject.SetActive(false);

                interfaceTools.Add(wireCutterTool.GetComponent<InterfaceTool>());


                __instance.tools = interfaceTools.ToArray();
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "OnPrefabInit")]
        public static class ToolMenu_OnPrefabInit {
            public static void Postfix(ToolMenu __instance, List<Sprite> ___icons) {
                __instance.gameObject.AddComponent<ToolMenuInputManager>();
                ___icons.Add(WireCutterAssets.WIRECUTTER_ICON_SPRITE);
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
        public static class ToolMenu_CreateBasicTools {
            public static void Prefix(ToolMenu __instance) {
                __instance.basicTools.Add(WireCutterAssets.WIRECUTTER_TOOLCOLLECTION);
            }
        }

        [HarmonyPatch(typeof(Game), "DestroyInstances")]
        public static class Game_DestroyInstances {
            public static void Postfix() {
                WireCutterTool.DestroyInstance();
            }
        }
    }
}