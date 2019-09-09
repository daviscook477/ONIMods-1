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
            string folderLocation = Path.Combine(Util.RootFolder(), "blueprints");
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
                return Path.Combine(GetBlueprintDirectory(), name + ".blueprint");
            }

            else {
                return Path.Combine(GetBlueprintDirectory(), name + "-" + index + ".blueprint");
            }
        }

        public static void ReloadBlueprints(bool ingame) {
            BlueprintsState.LoadedBlueprints.Clear();
            string[] blueprintFiles = Directory.GetFiles(GetBlueprintDirectory());

            foreach (string blueprintFile in blueprintFiles) {
                if (blueprintFile.EndsWith(".blueprint") || blueprintFile.EndsWith(".json")) {
                    Blueprint blueprint = new Blueprint(blueprintFile);

                    if (blueprint.ReadBinary() && !blueprint.IsEmpty()) {
                        BlueprintsState.LoadedBlueprints.Add(blueprint);
                    }

                    else {
                        blueprint.ReadJSON();

                        if (!blueprint.IsEmpty()) {
                            BlueprintsState.LoadedBlueprints.Add(blueprint);
                        }
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

            Transform titleTransform = blueprintNameDialog.transform.Find("Panel")?.Find("Title_BG")?.Find("Title");
            if (titleTransform != null && titleTransform.GetComponent<LocText>() != null) {
                titleTransform.GetComponent<LocText>().text = Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_NAMEBLUEPRINT_TITLE);
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
            if (!BlueprintsAssets.BLUEPRINTS_CONFIG_REQUIRECONSTRUCTABLE) {
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
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
            }

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

                jsonWriter.WritePropertyName("keybind_snapshottool_newsnapshot");
                jsonWriter.WriteValue(KeyCode.Delete.ToString());

                jsonWriter.WritePropertyName("require_constructable");
                jsonWriter.WriteValue(true);

                jsonWriter.WritePropertyName("compress_blueprints");
                jsonWriter.WriteValue(true);

                jsonWriter.WriteEndObject();
            }
        }

        public static void CreateKeycodeHintFile() {
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
            }

            if (!File.Exists(BlueprintsAssets.BLUEPRINTS_PATH_KEYCODESFILE)) {
                using (TextWriter textWriter = File.CreateText(BlueprintsAssets.BLUEPRINTS_PATH_KEYCODESFILE)) {
                    foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode))) {
                        textWriter.WriteLine(keycode.ToString());
                    }
                }
            }
        }

        public static void ReadConfig() {
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
                return;
            }

            using (StreamReader reader = File.OpenText(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE))
            using (JsonTextReader jsonReader = new JsonTextReader(reader)) {
                JObject rootObject = (JObject)JToken.ReadFrom(jsonReader).Root;

                JToken kCreateToolToken = rootObject.SelectToken("keybind_createtool");
                JToken kUseToolToken = rootObject.SelectToken("keybind_usetool");
                JToken kReloadToken = rootObject.SelectToken("keybind_usetool_reload");
                JToken kCycleLeftToken = rootObject.SelectToken("keybind_usetool_cycleleft");
                JToken kCycleRightToken = rootObject.SelectToken("keybind_usetool_cycleright");
                JToken kRenameToken = rootObject.SelectToken("keybind_usetool_rename");
                JToken kUseDeleteToken = rootObject.SelectToken("keybind_usetool_delete");
                JToken kSnapshotToolToken = rootObject.SelectToken("keybind_snapshottool");
                JToken kSnapshotNewSnapshotToken = rootObject.SelectToken("keybind_snapshottool_newsnapshot");

                JToken bRequireConstructable = rootObject.SelectToken("require_constructable");
                JToken bCompressBlueprints = rootObject.SelectToken("compress_blueprints");

                if (kCreateToolToken != null && kCreateToolToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCreateToolToken.Value<string>(), out KeyCode kCreateTool)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_CREATE = kCreateTool;
                }

                if (kUseToolToken != null && kUseToolToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kUseToolToken.Value<string>(), out KeyCode kUseTool)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_USE = kUseTool;
                }

                if (kReloadToken != null && kReloadToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kReloadToken.Value<string>(), out KeyCode kReload)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RELOAD = kReload;
                }

                if (kCycleLeftToken != null && kCycleLeftToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCycleLeftToken.Value<string>(), out KeyCode kCycleLeft)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLELEFT = kCycleLeft;
                }

                if (kCycleRightToken != null && kCycleRightToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCycleRightToken.Value<string>(), out KeyCode kCycleRight)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLERIGHT = kCycleRight;
                }

                if (kRenameToken != null && kRenameToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kRenameToken.Value<string>(), out KeyCode kRename)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RENAME = kRename;
                }

                if (kUseDeleteToken != null && kUseDeleteToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kUseDeleteToken.Value<string>(), out KeyCode kUseDelete)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_DELETE = kUseDelete;
                }

                if (kSnapshotToolToken != null && kSnapshotToolToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kSnapshotToolToken.Value<string>(), out KeyCode kSnapshotTool)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT = kSnapshotTool;
                }

                if (kSnapshotNewSnapshotToken != null && kSnapshotNewSnapshotToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kSnapshotNewSnapshotToken.Value<string>(), out KeyCode kSnapshotDelete)) {
                    BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT_NEWSNAPSHOT = kSnapshotDelete;
                }

                if (bRequireConstructable != null && bRequireConstructable.Type == JTokenType.Boolean) {
                    BlueprintsAssets.BLUEPRINTS_CONFIG_REQUIRECONSTRUCTABLE = bRequireConstructable.Value<bool>();
                }

                if (bCompressBlueprints != null && bCompressBlueprints.Type == JTokenType.Boolean) {
                    BlueprintsAssets.BLUEPRINTS_CONFIG_COMPRESBLUEPRINTS = bCompressBlueprints.Value<bool>();
                }
            }
        }
    }
}
