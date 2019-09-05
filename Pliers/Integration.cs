using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Harmony;

using UnityEngine;

namespace Pliers {
    public static class Mod_OnLoad {
        public static void OnLoad() {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string currentAssemblyDirectory = Path.GetDirectoryName(currentAssembly.Location);
            PliersAssets.PLIERS_PATH_CONFIGFOLDER = currentAssemblyDirectory + "/config";
            PliersAssets.PLIERS_PATH_CONFIGFILE = PliersAssets.PLIERS_PATH_CONFIGFOLDER + "/config.json";
            PliersAssets.PLIERS_PATH_KEYCODESFILE = PliersAssets.PLIERS_PATH_CONFIGFOLDER + "/keycodes.txt";

            IOUtilities.CreateKeycodeHintFile();
            if (File.Exists(PliersAssets.PLIERS_PATH_CONFIGFILE)) {
                IOUtilities.ReadConfig();
            }

            else {
                IOUtilities.CreateDefaultConfig();
            }

            PliersAssets.PLIERS_ICON_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Pliers.image_wirecutter_button.dds"), 32, 32);
            PliersAssets.PLIERS_ICON_SPRITE.name = PliersAssets.PLIERS_ICON_NAME;
            PliersAssets.PLIERS_VISUALIZER_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Pliers.image_wirecutter_visualizer.dds"), 256, 256);

            PliersAssets.PLIERS_TOOLCOLLECTION = ToolMenu.CreateToolCollection(PliersAssets.PLIERS_NAME, PliersAssets.PLIERS_ICON_NAME, Action.NumActions, PliersAssets.PLIERS_TOOLNAME, PliersAssets.PLIERS_TOOLTIP, false);

            Debug.Log("Pliers Loaded: Version " + currentAssembly.GetName().Version);
        }
    }

    namespace Patches {
        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public static class PlayerController_OnPrefabInit {
            public static void Postfix(PlayerController __instance) {
                List<InterfaceTool> interfaceTools = new List<InterfaceTool>(__instance.tools);


                GameObject pliersTool = new GameObject(PliersAssets.PLIERS_TOOLNAME);
                pliersTool.AddComponent<PliersTool>();

                pliersTool.transform.SetParent(__instance.gameObject.transform);
                pliersTool.gameObject.SetActive(true);
                pliersTool.gameObject.SetActive(false);

                interfaceTools.Add(pliersTool.GetComponent<InterfaceTool>());


                __instance.tools = interfaceTools.ToArray();
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "OnPrefabInit")]
        public static class ToolMenu_OnPrefabInit {
            public static void Postfix(ToolMenu __instance, List<Sprite> ___icons) {
                __instance.gameObject.AddComponent<ToolMenuInputManager>();
                ___icons.Add(PliersAssets.PLIERS_ICON_SPRITE);
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
        public static class ToolMenu_CreateBasicTools {
            public static void Prefix(ToolMenu __instance) {
                __instance.basicTools.Add(PliersAssets.PLIERS_TOOLCOLLECTION);
            }
        }

        [HarmonyPatch(typeof(Game), "DestroyInstances")]
        public static class Game_DestroyInstances {
            public static void Postfix() {
                PliersTool.DestroyInstance();
            }
        }
    }
}