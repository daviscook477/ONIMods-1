using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Blueprints {
    public static class Utilities {
        public static Sprite CreateSpriteDXT5(Stream inputStream, int width, int height) {
            byte[] buffer = new byte[inputStream.Length - 128];
            inputStream.Seek(128, SeekOrigin.Current);
            inputStream.Read(buffer, 0, buffer.Length);

            Texture2D texture = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture.LoadRawTextureData(buffer);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5F, 0.5F));
        }

        public static string GetBlueprintDirectory() {
            string folderLocation = Util.RootFolder() + "/blueprints";
            if (!Directory.Exists(folderLocation)) {
                Directory.CreateDirectory(folderLocation);
            }

            return folderLocation;
        }

        public static string GetNewBlueprintLocation(string name) {
            string returnString = GetBlueprintName(name, -1);
            for (int i = 0; File.Exists(returnString); returnString = GetBlueprintName(name, i), ++i) { }
            return returnString;
        }

        public static string GetBlueprintName(string name, int index) {
            if (index == -1) {
                return GetBlueprintDirectory() + "/" + name + ".blueprint";
            }

            else {
                return GetBlueprintDirectory() + "/" + name + "-" + index + ".blueprint";
            }
        }

        public static void ReloadBlueprints(bool ingame) {
            BlueprintsState.LoadedBlueprints.Clear();
            string[] blueprintFiles = Directory.GetFiles(GetBlueprintDirectory());

            foreach (string blueprintFile in blueprintFiles) {
                if (blueprintFile.EndsWith(".blueprint") || blueprintFile.EndsWith(".json")) {
                    Blueprint blueprint = new Blueprint(blueprintFile);

                    if(blueprint.ReadBinary() && !blueprint.IsEmpty()) {
                        BlueprintsState.LoadedBlueprints.Add(blueprint);
                    }

                    else if (blueprint.ReadJSON() && !blueprint.IsEmpty()) {
                        BlueprintsState.LoadedBlueprints.Add(blueprint);
                    }
                }
            }

            if (ingame && BlueprintsState.LoadedBlueprints.Count > 0) {
                BlueprintsState.SelectedBlueprintIndex = 0;
                BlueprintsState.ClearVisuals();
                BlueprintsState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), BlueprintsState.SelectedBlueprint);
            }
        }

        public static FileNameDialog CreateBlueprintRenameDialog() {
            GameObject blueprintNameDialogParent = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
            FileNameDialog blueprintNameDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, blueprintNameDialogParent);
            blueprintNameDialog.name = "BlueprintNameDialog";

            Transform titleTransform = blueprintNameDialog.transform?.Find("Panel")?.Find("Title_BG")?.Find("Title");
            if (titleTransform != null && titleTransform.GetComponent<LocText>() != null) {
                titleTransform.GetComponent<LocText>().text = "NAME BLUEPRINT";
            }

            return blueprintNameDialog;
        }

        public static bool TryParseEnum<T>(string input, out T output) {
            string inputLower = input.ToLower();
            foreach (T enumeration in Enum.GetValues(typeof(T))) {
                if (enumeration.ToString().ToLower() == inputLower) {
                    output = enumeration;
                    return true;
                }
            }

            output = default;
            return false;
        }

        public static string GetKeyCodeString(KeyCode keyCode) {
            return UI.FormatAsHotkey("[" + keyCode.ToString().ToUpper() + "]");
        }

        public static bool IsBuildable(this BuildingDef buildingDef) {
            if (!BlueprintsAssets.BLUEPRINTS_BOOL_REQUIRECONSTRUCTABLE) {
                return true;
            }

            if (buildingDef.ShowInBuildMenu && !buildingDef.Deprecated) {
                foreach (PlanScreen.PlanInfo planScreen in TUNING.BUILDINGS.PLANORDER) {
                    foreach (string buildingID in planScreen.data as IList<string>) {
                        if (buildingID == buildingDef.PrefabID) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }

    public static class IOUtilities {
        public static void CreateDefaultConfig() {
            using (TextWriter textWriter = File.CreateText(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)) {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("keybind_createtool");
                jsonWriter.WriteValue(KeyCode.None.ToString());

                jsonWriter.WritePropertyName("keybind_usetool_cycleleft");
                jsonWriter.WriteValue(KeyCode.LeftArrow.ToString());

                jsonWriter.WritePropertyName("keybind_usetool_cycleright");
                jsonWriter.WriteValue(KeyCode.RightArrow.ToString());

                jsonWriter.WritePropertyName("keybind_usetool_rename");
                jsonWriter.WriteValue(KeyCode.End.ToString());

                jsonWriter.WritePropertyName("keybind_usetool_delete");
                jsonWriter.WriteValue(KeyCode.Delete.ToString());

                jsonWriter.WritePropertyName("keybind_snapshottool");
                jsonWriter.WriteValue(KeyCode.None.ToString());

                jsonWriter.WritePropertyName("keybind_snapshottool_delete");
                jsonWriter.WriteValue(KeyCode.Delete.ToString());

                jsonWriter.WritePropertyName("require_constructable");
                jsonWriter.WriteValue(true);

                jsonWriter.WritePropertyName("compress_blueprints");
                jsonWriter.WriteValue(true);

                jsonWriter.WriteEndObject();
            }
        }

        public static void CreateKeycodeHintFile() {
            if(!File.Exists(BlueprintsAssets.BLUEPRINTS_PATH_KEYCODESFILE)) {
                using (TextWriter textWriter = File.CreateText(BlueprintsAssets.BLUEPRINTS_PATH_KEYCODESFILE)) {
                    foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode))) {
                        textWriter.WriteLine(keycode.ToString());
                    }
                }
            }
        }

        public static void ReadConfig() {
            using (StreamReader reader = File.OpenText(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE))
            using (JsonTextReader jsonReader = new JsonTextReader(reader)) {
                JObject rootObject = (JObject) JToken.ReadFrom(jsonReader).Root;

                JToken kCreateToolToken = rootObject.SelectToken("keybind_createtool");
                JToken kUseToolToken = rootObject.SelectToken("keybind_usetool");
                JToken kReloadToken = rootObject.SelectToken("keybind_usetool_reload");
                JToken kCycleLeftToken = rootObject.SelectToken("keybind_usetool_cycleleft");
                JToken kCycleRightToken = rootObject.SelectToken("keybind_usetool_cycleright");
                JToken kRenameToken = rootObject.SelectToken("keybind_usetool_rename");
                JToken kUseDeleteToken = rootObject.SelectToken("keybind_usetool_delete");
                JToken kSnapshotToolToken = rootObject.SelectToken("keybind_snapshottool");
                JToken kSnapshotDeleteToken = rootObject.SelectToken("keybind_snapshottool_delete");

                JToken bRequireConstructable = rootObject.SelectToken("require_constructable");
                JToken bCompressBlueprints = rootObject.SelectToken("compress_blueprints");

                if (kCreateToolToken != null && kCreateToolToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCreateToolToken.Value<string>(), out KeyCode kCreateTool)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_CREATETOOL = kCreateTool;
                    BlueprintsAssets.BLUEPRINTS_CREATE_TOOLTIP = "Create blueprint " + Utilities.GetKeyCodeString(kCreateTool);
                }

                if (kUseToolToken != null && kUseToolToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kUseToolToken.Value<string>(), out KeyCode kUseTool)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL = kUseTool;
                    BlueprintsAssets.BLUEPRINTS_USE_TOOLTIP = "Use blueprint " + Utilities.GetKeyCodeString(kUseTool) + "\n\nWhen activating the tool hold " + Utilities.GetKeyCodeString(KeyCode.LeftShift) + " to reload blueprints";
                }

                if (kReloadToken != null && kReloadToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kReloadToken.Value<string>(), out KeyCode kReload)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_RELOAD = kReload;
                    BlueprintsAssets.BLUEPRINTS_USE_TOOLTIP = "Use blueprint " + Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL) + "\n\nWhen activating the tool hold " + Utilities.GetKeyCodeString(kReload) + " to reload blueprints";
                }

                if (kCycleLeftToken != null && kCycleLeftToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCycleLeftToken.Value<string>(), out KeyCode kCycleLeft)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_CYCLELEFT = kCycleLeft;
                    BlueprintsAssets.BLUEPRINTS_STRING_CYCLEBLUEPRINTS = "Use " + Utilities.GetKeyCodeString(kCycleLeft) + " and " + Utilities.GetKeyCodeString(KeyCode.RightArrow) + " to cycle between blueprints";
                }

                if (kCycleRightToken != null && kCycleRightToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCycleRightToken.Value<string>(), out KeyCode kCycleRight)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_CYCLERIGHT = kCycleRight;
                    BlueprintsAssets.BLUEPRINTS_STRING_CYCLEBLUEPRINTS = "Use " + Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_CYCLELEFT) + " and " + Utilities.GetKeyCodeString(kCycleRight) + " to cycle between blueprints";
                }

                if (kRenameToken != null && kRenameToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kRenameToken.Value<string>(), out KeyCode kRename)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_RENAME = kRename;
                    BlueprintsAssets.BLUEPRINTS_STRING_RENAMEBLUEPRINT = "Press " + Utilities.GetKeyCodeString(kRename) + " to rename selected blueprint";
                }

                if (kUseDeleteToken != null && kUseDeleteToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kUseDeleteToken.Value<string>(), out KeyCode kUseDelete)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_DELETE = kUseDelete;
                    BlueprintsAssets.BLUEPRINTS_STRING_DELETEBLUEPRINT = "Press " + Utilities.GetKeyCodeString(kUseDelete) + " to delete selected blueprint";
                }

                if (kSnapshotToolToken != null && kSnapshotToolToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kSnapshotToolToken.Value<string>(), out KeyCode kSnapshotTool)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_SNAPSHOTTOOL = kSnapshotTool;
                    BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLTIP = "Take a snapshot " + Utilities.GetKeyCodeString(KeyCode.None) + "\n\nCreate a blueprint and quickly place it elsewhere while not cluttering your blueprint collection! \nSnapshots do not persist between games.";
                }

                if (kSnapshotDeleteToken != null && kSnapshotDeleteToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kSnapshotDeleteToken.Value<string>(), out KeyCode kSnapshotDelete)) {
                    BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_SNAPSHOTTOOL_DELETE = kSnapshotDelete;
                    BlueprintsAssets.BLUEPRINTS_STRING_DELETESNAPSHOT = "Press " + Utilities.GetKeyCodeString(kSnapshotDelete) + " to take new snapshot";
                }

                if (bRequireConstructable != null && bRequireConstructable.Type == JTokenType.Boolean) {
                    BlueprintsAssets.BLUEPRINTS_BOOL_REQUIRECONSTRUCTABLE = bRequireConstructable.Value<bool>();
                }

                if (bCompressBlueprints != null && bCompressBlueprints.Type == JTokenType.Boolean) {
                    BlueprintsAssets.BLUEPRINTS_BOOL_COMPRESBLUEPRINTS = bCompressBlueprints.Value<bool>();
                }
            }
        }
    }
}
