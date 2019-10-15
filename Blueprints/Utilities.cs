using Harmony;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STRINGS;
using System.Collections.Generic;
using System.IO;
using TMPro;
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

        public static void ReloadBlueprints(bool ingame) {
            BlueprintsState.LoadedBlueprints.Clear();
            LoadFolder(GetBlueprintDirectory());

            if (ingame && BlueprintsState.HasBlueprints()) {
                BlueprintsState.ClearVisuals();
                BlueprintsState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), BlueprintsState.SelectedBlueprint);
            }
        }

        public static void LoadFolder(string folder) {
            string[] files = Directory.GetFiles(folder);
            string[] subfolders = Directory.GetDirectories(folder);

            foreach (string file in files) {
                if (file.EndsWith(".blueprint") || file.EndsWith(".json")) {
                    Blueprint blueprint = new Blueprint(file);
                    bool valid = false;

                    if (blueprint.ReadBinary() && !blueprint.IsEmpty()) {
                        valid = true;
                    }

                    else {
                        blueprint.ReadJSON();

                        if (!blueprint.IsEmpty()) {
                            valid = true;
                        }
                    }

                    if (valid) {
                        PlaceIntoFolder(blueprint);
                        blueprint.InferFileLocation();

                        if (blueprint.FilePath != file) {
                            File.Delete(file);
                            blueprint.Write();
                        }
                    }
                }
            }

            foreach (string subfolder in subfolders) {
                LoadFolder(subfolder);
            }
        }

        public static void PlaceIntoFolder(Blueprint blueprint) {
            int index = -1;

            for (int i = 0; i < BlueprintsState.LoadedBlueprints.Count; ++i) {
                if (BlueprintsState.LoadedBlueprints[i].Name == blueprint.Folder) {
                    index = i;
                    break;
                }
            }

            if (index == -1) {
                BlueprintFolder newFolder = new BlueprintFolder(blueprint.Folder);
                newFolder.AddBlueprint(blueprint);

                BlueprintsState.LoadedBlueprints.Add(newFolder);
            }

            else {
                BlueprintsState.LoadedBlueprints[index].AddBlueprint(blueprint);
            }
        }

        public static bool TryParseEnum<T>(string input, out T output) {
            string inputLower = input.ToLower();
            foreach (T enumeration in System.Enum.GetValues(typeof(T))) {
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

    public static class UIUtilities {
        public static FileNameDialog CreateTextDialog(string title, bool allowEmpty = false, System.Action<string, FileNameDialog> onConfirm = null) {
            GameObject textDialogParent = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
            FileNameDialog textDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, textDialogParent);
            textDialog.name = "BlueprintsMod_TextDialog_" + title;

            TMP_InputField inputField = Traverse.Create(textDialog).Field("inputField").GetValue<TMP_InputField>();
            KButton confirmButton = Traverse.Create(textDialog).Field("confirmButton").GetValue<KButton>();
            if (inputField != null && confirmButton && confirmButton != null && allowEmpty) {
                confirmButton.onClick += delegate () {
                    if (textDialog.onConfirm != null && inputField.text != null && inputField.text.Length == 0) {
                        textDialog.onConfirm.Invoke(inputField.text);
                    }
                };
            }

            if (onConfirm != null) {
                textDialog.onConfirm += delegate (string result) {
                    onConfirm.Invoke(result.Substring(0, Mathf.Max(0, result.Length - 4)), textDialog);
                };
            }

            Transform titleTransform = textDialog.transform.Find("Panel")?.Find("Title_BG")?.Find("Title");
            if (titleTransform != null && titleTransform.GetComponent<LocText>() != null) {
                titleTransform.GetComponent<LocText>().text = title;
            }

            return textDialog;
        }
    }

    public static class IOUtilities {
        public static void CreateDefaultConfig() {
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
            }

            using TextWriter textWriter = File.CreateText(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE);
            using JsonTextWriter jsonWriter = new JsonTextWriter(textWriter) {
                Formatting = Formatting.Indented
            };

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("keybind_createtool");
            jsonWriter.WriteValue(KeyCode.None.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_cyclefolder_up");
            jsonWriter.WriteValue(KeyCode.UpArrow.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_cyclefolder_down");
            jsonWriter.WriteValue(KeyCode.DownArrow.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_cycleblueprint_left");
            jsonWriter.WriteValue(KeyCode.LeftArrow.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_cycleblueprint_right");
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

        public static void CreateKeycodeHintFile() {
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
            }

            if (!File.Exists(BlueprintsAssets.BLUEPRINTS_PATH_KEYCODESFILE)) {
                using TextWriter textWriter = File.CreateText(BlueprintsAssets.BLUEPRINTS_PATH_KEYCODESFILE);

                foreach (KeyCode keycode in System.Enum.GetValues(typeof(KeyCode))) {
                    textWriter.WriteLine(keycode.ToString());
                }
            }
        }

        public static void ReadConfig() {
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
                return;
            }

            using StreamReader reader = File.OpenText(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE);
            using JsonTextReader jsonReader = new JsonTextReader(reader);

            JObject rootObject = (JObject) JToken.ReadFrom(jsonReader).Root;

            JToken kCreateToolToken = rootObject.SelectToken("keybind_createtool");
            JToken kUseToolToken = rootObject.SelectToken("keybind_usetool");
            JToken kReloadToken = rootObject.SelectToken("keybind_usetool_reload");
            JToken kCycleFolderUpToken = rootObject.SelectToken("keybind_usetool_cyclefolder_up");
            JToken kCycleFolderDownToken = rootObject.SelectToken("keybind_usetool_cyclefolder_down");
            JToken kCycleBlueprintLeftToken = rootObject.SelectToken("keybind_usetool_cycleblueprint_left");
            JToken kCycleBlueprintRightToken = rootObject.SelectToken("keybind_usetool_cycleblueprint_right");
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

            if (kCycleFolderUpToken != null && kCycleFolderUpToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCycleFolderUpToken.Value<string>(), out KeyCode kCycleFolderUp)) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_UP = kCycleFolderUp;
            }

            if (kCycleFolderDownToken != null && kCycleFolderDownToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCycleFolderDownToken.Value<string>(), out KeyCode kCycleFolderDown)) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_DOWN = kCycleFolderDown;
            }

            if (kCycleBlueprintLeftToken != null && kCycleBlueprintLeftToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCycleBlueprintLeftToken.Value<string>(), out KeyCode kCycleBlueprintLeft)) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_LEFT = kCycleBlueprintLeft;
            }

            if (kCycleBlueprintRightToken != null && kCycleBlueprintRightToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kCycleBlueprintRightToken.Value<string>(), out KeyCode kCycleBlueprintRight)) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_RIGHT = kCycleBlueprintRight;
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
