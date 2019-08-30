using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Harmony;

using UnityEngine;
using TMPro;

namespace Blueprints {
    public static class Mod_OnLoad {
        public static void OnLoad() {
            Assembly currentAssembly = Assembly.GetAssembly(typeof(BlueprintsAssets));
            string currentAssemblyDirectory = Path.GetDirectoryName(currentAssembly.Location);
            BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE = currentAssemblyDirectory + "/config.json";
            BlueprintsAssets.BLUEPRINTS_PATH_KEYCODESFILE = currentAssemblyDirectory + "/keycodes.txt";

            IOUtilities.CreateKeycodeHintFile();
            if (File.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE)) {
                IOUtilities.ReadConfig();
            }

            else {
                IOUtilities.CreateDefaultConfig();
            }

            BlueprintsAssets.BLUEPRINTS_USE_ICON_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_useblueprint_button.dds"), 32, 32);
            BlueprintsAssets.BLUEPRINTS_USE_ICON_SPRITE.name = BlueprintsAssets.BLUEPRINTS_USE_ICON_NAME;
            BlueprintsAssets.BLUEPRINTS_USE_VISUALIZER_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_useblueprint_visualizer.dds"), 256, 256);

            BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_createblueprint_button.dds"), 32, 32);
            BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE.name = BlueprintsAssets.BLUEPRINTS_CREATE_ICON_NAME;
            BlueprintsAssets.BLUEPRINTS_CREATE_VISUALIZER_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_createblueprint_visualizer.dds"), 256, 256);

            BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_snapshot_button.dds"), 32, 32);
            BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE.name = BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_NAME;
            BlueprintsAssets.BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_snapshot_visualizer.dds"), 256, 256);

            BlueprintsAssets.BLUEPRINTS_CREATE_TOOLCOLLECTION = ToolMenu.CreateToolCollection(BlueprintsAssets.BLUEPRINTS_CREATE_NAME, BlueprintsAssets.BLUEPRINTS_CREATE_ICON_NAME, Action.NumActions, BlueprintsAssets.BLUEPRINTS_CREATE_TOOLNAME, BlueprintsAssets.BLUEPRINTS_CREATE_TOOLTIP, true);
            BlueprintsAssets.BLUEPRINTS_USE_TOOLCOLLECTION = ToolMenu.CreateToolCollection(BlueprintsAssets.BLUEPRINTS_USE_NAME, BlueprintsAssets.BLUEPRINTS_USE_ICON_NAME, Action.NumActions, BlueprintsAssets.BLUEPRINTS_USE_TOOLNAME, BlueprintsAssets.BLUEPRINTS_USE_TOOLTIP, true);
            BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION = ToolMenu.CreateToolCollection(BlueprintsAssets.BLUEPRINTS_SNAPSHOT_NAME, BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_NAME, Action.NumActions, BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLNAME, BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLTIP, true);

            Debug.Log("Blueprints Loaded: Version " + currentAssembly.GetName().Version);
        }
    }

    namespace Patches {
        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public static class PlayerController_OnPrefabInit {
            public static void Postfix(PlayerController __instance) {
                List<InterfaceTool> interfaceTools = new List<InterfaceTool>(__instance.tools);


                GameObject createBlueprintTool = new GameObject(BlueprintsAssets.BLUEPRINTS_CREATE_TOOLNAME);
                createBlueprintTool.AddComponent<CreateBlueprintTool>();

                createBlueprintTool.transform.SetParent(__instance.gameObject.transform);
                createBlueprintTool.gameObject.SetActive(true);
                createBlueprintTool.gameObject.SetActive(false);

                interfaceTools.Add(createBlueprintTool.GetComponent<InterfaceTool>());


                GameObject useBlueprintTool = new GameObject(BlueprintsAssets.BLUEPRINTS_USE_TOOLNAME);
                useBlueprintTool.AddComponent<UseBlueprintTool>();

                useBlueprintTool.transform.SetParent(__instance.gameObject.transform);
                useBlueprintTool.gameObject.SetActive(true);
                useBlueprintTool.gameObject.SetActive(false);

                interfaceTools.Add(useBlueprintTool.GetComponent<InterfaceTool>());


                GameObject snapshotTool = new GameObject(BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLNAME);
                snapshotTool.AddComponent<SnapshotTool>();

                snapshotTool.transform.SetParent(__instance.gameObject.transform);
                snapshotTool.gameObject.SetActive(true);
                snapshotTool.gameObject.SetActive(false);

                interfaceTools.Add(snapshotTool.GetComponent<InterfaceTool>());


                __instance.tools = interfaceTools.ToArray();

            }
        }

        [HarmonyPatch(typeof(ToolMenu), "OnPrefabInit")]
        public static class ToolMenu_OnPrefabInit {
            public static void Postfix(ToolMenu __instance, List<Sprite> ___icons) {
                __instance.gameObject.AddComponent<ToolMenuInputManager>();

                ___icons.Add(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE);
                ___icons.Add(BlueprintsAssets.BLUEPRINTS_USE_ICON_SPRITE);

                if (BlueprintsAssets.BLUEPRINTS_BOOL_ENABLESNAPSHOTTOOL) {
                    ___icons.Add(BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE);
                }
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
        public static class ToolMenu_CreateBasicTools {
            public static void Prefix(ToolMenu __instance) {
                __instance.basicTools.Add(BlueprintsAssets.BLUEPRINTS_CREATE_TOOLCOLLECTION);
                __instance.basicTools.Add(BlueprintsAssets.BLUEPRINTS_USE_TOOLCOLLECTION);
                
                if (BlueprintsAssets.BLUEPRINTS_BOOL_ENABLESNAPSHOTTOOL) {
                    __instance.basicTools.Add(BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION);
                }

                Utilities.ReloadBlueprints(false);
            }
        }
        
        [HarmonyPatch(typeof(FileNameDialog), "OnSpawn")]
        public static class FileNameDialog_OnSpawn {
            public static void Postfix(FileNameDialog __instance, TMP_InputField ___inputField) {
                if (__instance.name == "BlueprintNameDialog") {
                    ___inputField.onValueChanged.RemoveAllListeners();
                    ___inputField.onEndEdit.RemoveAllListeners();
                }
            }
        }

        [HarmonyPatch(typeof(Game), "DestroyInstances")]
        public static class Game_DestroyInstances {
            public static void Postfix() {
                CreateBlueprintTool.DestroyInstance();
                UseBlueprintTool.DestroyInstance();
                SnapshotTool.DestroyInstance();
            }
        }

        [HarmonyPatch(typeof(Rendering.BlockTileRenderer), "GetCellColour")]
        public static class BlockTileRenderer_GetCellColour {
            public static void Postfix(int cell, SimHashes element, ref Color __result) {
                if (__result != Color.red && element == SimHashes.Void) {
                    if (BlueprintsState.ColoredCells.ContainsKey(cell)) {
                        __result = BlueprintsState.ColoredCells[cell].Color;
                    }
                }
            }
        }
    }
}