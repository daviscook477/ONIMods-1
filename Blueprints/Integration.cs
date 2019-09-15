using Harmony;
using ModFramework;
using Rendering;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace Blueprints {
    public static class Integration {
        public static void OnLoad() {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            string currentAssemblyDirectory = Path.GetDirectoryName(currentAssembly.Location);

            BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER = Path.Combine(currentAssemblyDirectory, "config");
            BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE = Path.Combine(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER, "config.json");
            BlueprintsAssets.BLUEPRINTS_PATH_KEYCODESFILE = Path.Combine(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER, "keycodes.txt");
            BlueprintsAssets.BLUEPRINTS_PATH_ACCOUNTIDFILE = Path.Combine(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER, "accountid.txt");

            IOUtilities.CreateKeycodeHintFile();
            if (File.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE)) {
                IOUtilities.ReadConfig();
            }

            else {
                IOUtilities.CreateDefaultConfig();
            }

            if (File.Exists(BlueprintsAssets.BLUEPRINTS_PATH_ACCOUNTIDFILE)) {
                IOUtilities.ReadAccountID();
            }

            else {
                IOUtilities.CreateAccountIDFile();
            }

            BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_createblueprint_button.dds"), 32, 32);
            BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE.name = BlueprintsAssets.BLUEPRINTS_CREATE_ICON_NAME;
            BlueprintsAssets.BLUEPRINTS_CREATE_VISUALIZER_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_createblueprint_visualizer.dds"), 256, 256);

            BlueprintsAssets.BLUEPRINTS_USE_ICON_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_useblueprint_button.dds"), 32, 32);
            BlueprintsAssets.BLUEPRINTS_USE_ICON_SPRITE.name = BlueprintsAssets.BLUEPRINTS_USE_ICON_NAME;
            BlueprintsAssets.BLUEPRINTS_USE_VISUALIZER_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_useblueprint_visualizer.dds"), 256, 256);

            BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_snapshot_button.dds"), 32, 32);
            BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE.name = BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_NAME;
            BlueprintsAssets.BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE = Utilities.CreateSpriteDXT5(Assembly.GetExecutingAssembly().GetManifestResourceStream("Blueprints.image_snapshot_visualizer.dds"), 256, 256);

            ModLocalization.LocalizationCompleteEvent += ModLocalizedHandler;
            ModLocalization.DefaultLocalization = new string[] {
                BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_NAME, "New Blueprint",
                BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_TOOLTIP, "Create blueprint {0}",
                BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_EMPTY, "Blueprint would have been empty!",
                BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_CREATED, "Created blueprint!",
                BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_CANCELLED, "Cancelled blueprint!",
                BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_TOOLTIP_TITLE, "CREATE BLUEPRINT TOOL",
                BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_ACTION_DRAG, "DRAG",
                BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_ACTION_BACK, "BACK",

                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_NAME, "Use Blueprint",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_TOOLTIP, "Use blueprint {0} \n\nWhen selecting the tool hold {1} to reload blueprints",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS, "Loaded {0} blueprints! ({1} total)",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS_ADDITIONAL, "additional",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS_FEWER, "fewer",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_TOOLTIP_TITLE, "USE BLUEPRINT TOOL",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_ACTION_CLICK, "CLICK",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_ACTION_BACK, "BACK",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_CYCLEBLUEPRINTS, "Use {0} and {1} to cycle blueprints.",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_NAMEBLUEPRINT, "Press {0} to rename blueprint.",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_DELETEBLUEPRINT, "Press {0} to delete blueprint.",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_ERRORMESSAGE, "This blueprint contained {0} misconfigured or missing prefabs which have been omitted!",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_SELECTEDBLUEPRINT, "Selected \"{0}\" ({1}/{2})",
                BlueprintsStringIDs.STRING_BLUEPRINTS_USE_NOBLUEPRINTS, "No blueprints loaded!",

                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_NAME, "Take Snapshot",
                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP, "Take snapshot {0} \n\nCreate a blueprint and quickly place it elsewhere while not cluttering your blueprint collection! \nSnapshots do not persist between games or worlds.",
                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_EMPTY, "Snapshot would have been empty!",
                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_TAKEN, "Snapshot taken!",
                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP_TITLE, "SNAPSHOT TOOL",
                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_ACTION_CLICK, "CLICK",
                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_ACTION_DRAG, "DRAG",
                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_ACTION_BACK, "BACK",
                BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_NEWSNAPSHOT, "Press {0} to take new snapshot.",

                BlueprintsStringIDs.STRING_BLUEPRINTS_NAMEBLUEPRINT_TITLE, "NAME BLUEPRINT"
            };

            Debug.Log("Blueprints Loaded: Version " + currentAssembly.GetName().Version);
        }

        private static void ModLocalizedHandler(string languageCode) {
            BlueprintsAssets.BLUEPRINTS_CREATE_TOOLCOLLECTION = ToolMenu.CreateToolCollection(
                Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_NAME).String,
                BlueprintsAssets.BLUEPRINTS_CREATE_ICON_NAME,
                Action.NumActions,
                BlueprintsAssets.BLUEPRINTS_CREATE_TOOLNAME,
                string.Format(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_TOOLTIP), Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_KEYBIND_CREATE)),
                true
            );

            BlueprintsAssets.BLUEPRINTS_USE_TOOLCOLLECTION = ToolMenu.CreateToolCollection(
                Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_NAME).String,
                BlueprintsAssets.BLUEPRINTS_USE_ICON_NAME,
                Action.NumActions,
                BlueprintsAssets.BLUEPRINTS_USE_TOOLNAME,
                string.Format(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_TOOLTIP), Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE), Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RELOAD)),
                true
            );

            BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION = ToolMenu.CreateToolCollection(
                Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_NAME).String,
                BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_NAME,
                Action.NumActions,
                BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLNAME,
                string.Format(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP), Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT)),
                false
           );
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
                ___icons.Add(BlueprintsAssets.BLUEPRINTS_SNAPSHOT_ICON_SPRITE);
            }
        }

        [HarmonyPatch(typeof(ToolMenu), "CreateBasicTools")]
        public static class ToolMenu_CreateBasicTools {
            public static void Prefix(ToolMenu __instance) {
                __instance.basicTools.Add(BlueprintsAssets.BLUEPRINTS_CREATE_TOOLCOLLECTION);
                __instance.basicTools.Add(BlueprintsAssets.BLUEPRINTS_USE_TOOLCOLLECTION);
                __instance.basicTools.Add(BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION);

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

        [HarmonyPatch(typeof(BlockTileRenderer), "GetCellColour")]
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