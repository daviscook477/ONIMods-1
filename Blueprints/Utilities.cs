using Harmony;
using ModKeyBinding;
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

        public static FileNameDialog CreateFolderDialog(System.Action<string, FileNameDialog> onConfirm = null) {
            string title = Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_FOLDERBLUEPRINT_TITLE);

            FileNameDialog folderDialog = CreateTextDialog(title, true, onConfirm);
            folderDialog.name = "BlueprintsMod_FolderDialog_" + title;

            return folderDialog;
        }
    }

    public static class IOUtilities {
        public static void WriteConfig() {
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
            }

            using TextWriter textWriter = File.CreateText(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFILE);
            using JsonTextWriter jsonWriter = new JsonTextWriter(textWriter) {
                Formatting = Formatting.Indented
            };

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("keybind_createtool");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_CREATE.ToString());

            jsonWriter.WritePropertyName("keybind_usetool");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_reload");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RELOAD.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_cyclefolder_up");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_UP.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_cyclefolder_down");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_DOWN.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_cycleblueprint_left");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_LEFT.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_cycleblueprint_right");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_RIGHT.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_folder");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_FOLDER.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_rename");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RENAME.ToString());

            jsonWriter.WritePropertyName("keybind_usetool_delete");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_DELETE.ToString());

            jsonWriter.WritePropertyName("keybind_snapshottool");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT.ToString());

            jsonWriter.WritePropertyName("keybind_snapshottool_newsnapshot");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT_NEWSNAPSHOT.ToString());

            jsonWriter.WritePropertyName("require_constructable");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_CONFIG_REQUIRECONSTRUCTABLE);

            jsonWriter.WritePropertyName("compress_blueprints");
            jsonWriter.WriteValue(BlueprintsAssets.BLUEPRINTS_CONFIG_COMPRESBLUEPRINTS);

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
            JToken kFolderToken = rootObject.SelectToken("keybind_usetool_folder");
            JToken kRenameToken = rootObject.SelectToken("keybind_usetool_rename");
            JToken kDeleteToken = rootObject.SelectToken("keybind_usetool_delete");
            JToken kSnapshotToolToken = rootObject.SelectToken("keybind_snapshottool");
            JToken kSnapshotNewSnapshotToken = rootObject.SelectToken("keybind_snapshottool_newsnapshot");

            JToken bRequireConstructable = rootObject.SelectToken("require_constructable");
            JToken bCompressBlueprints = rootObject.SelectToken("compress_blueprints");

            if (kCreateToolToken != null && kCreateToolToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_CREATE = new KeyBinding(kCreateToolToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_CREATE.AssignIfEmpty(KeyCode.None);
            }

            if (kUseToolToken != null && kUseToolToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE = new KeyBinding(kUseToolToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE.AssignIfEmpty(KeyCode.None);
            }

            if (kReloadToken != null && kReloadToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RELOAD = new KeyBinding(kUseToolToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RELOAD.AssignIfEmpty(KeyCode.LeftShift);
            }

            if (kCycleFolderUpToken != null && kCycleFolderUpToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_UP = new KeyBinding(kCycleFolderUpToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_UP.AssignIfEmpty(KeyCode.UpArrow);
            }

            if (kCycleFolderDownToken != null && kCycleFolderDownToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_DOWN = new KeyBinding(kCycleFolderDownToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_DOWN.AssignIfEmpty(KeyCode.UpArrow);
            }

            if (kCycleBlueprintLeftToken != null && kCycleBlueprintLeftToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_LEFT = new KeyBinding(kCycleBlueprintLeftToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_LEFT.AssignIfEmpty(KeyCode.LeftArrow);
            }

            if (kCycleBlueprintRightToken != null && kCycleBlueprintRightToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_RIGHT = new KeyBinding(kCycleBlueprintRightToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_RIGHT.AssignIfEmpty(KeyCode.RightArrow);
            }

            if (kFolderToken != null && kFolderToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_FOLDER = new KeyBinding(kFolderToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_FOLDER.AssignIfEmpty(KeyCode.Home);
            }

            if (kRenameToken != null && kRenameToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RENAME = new KeyBinding(kRenameToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RENAME.AssignIfEmpty(KeyCode.End);
            }

            if (kDeleteToken != null && kDeleteToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_DELETE = new KeyBinding(kDeleteToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_DELETE.AssignIfEmpty(KeyCode.Delete);
            }

            if (kSnapshotToolToken != null && kSnapshotToolToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT = new KeyBinding(kSnapshotToolToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT.AssignIfEmpty(KeyCode.None);
            }

            if (kSnapshotNewSnapshotToken != null && kSnapshotNewSnapshotToken.Type == JTokenType.String) {
                BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT_NEWSNAPSHOT = new KeyBinding(kSnapshotNewSnapshotToken.Value<string>());
                BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT_NEWSNAPSHOT.AssignIfEmpty(KeyCode.Delete);
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
