using Harmony;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.UI;

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

        public static void ExplodeTransform(Transform transform, int index = 0) {
            Debug.Log(new string('#', index) + " " + transform.name + " -> " + transform);

            Component[] components = transform.GetComponents<Component>();
            for (int i = 0; i < components.Length; ++i) {
                Debug.Log(new string('#', index) + "> " + components[i]);
            }

            Debug.Log("\n");

            for (int i = 0; i < transform.childCount; ++i) {
                ExplodeTransform(transform.GetChild(i), index + 1);
            }
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

    public static class UIUtilities {
        public static FileNameDialog CreateTextEntryDialog(string name, string title, bool createToggle = false, string toggleText = "", Action<bool> valueChangedListener = null) {
            GameObject blueprintNameDialogParent = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
            FileNameDialog blueprintNameDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, blueprintNameDialogParent);

            blueprintNameDialog.name = name;
            Transform panelTransform = blueprintNameDialog.transform.Find("Panel");

            if (panelTransform != null) {
                if (createToggle) {
                    Transform bodyTransform = panelTransform.Find("Body");

                    if (bodyTransform != null) {
                        RectTransform textAreaTransform = bodyTransform.Find("LocTextInputField") as RectTransform;
                        RectTransform confirmButtonTransform = bodyTransform.Find("ConfirmButton") as RectTransform;

                        if (textAreaTransform != null && confirmButtonTransform != null) {
                            List<OverlayLegend.OverlayInfo> overlayInformation = Traverse.Create(OverlayLegend.Instance).Field("overlayInfoList").GetValue<List<OverlayLegend.OverlayInfo>>();
                            OverlayLegend.OverlayInfo germOverlay = overlayInformation.Find(overlay => overlay.name == "GERM OVERLAY");
                            Transform checkbox = null;

                            foreach (GameObject diagram in germOverlay.diagrams) {
                                checkbox = diagram.transform.Find("Background")?.Find("Contents")?.Find("CheckBoxContainer");
                                if (checkbox != null) {
                                    break;
                                }
                            }

                            if (checkbox != null) {
                                RectTransform kToggleTransform = Util.KInstantiateUI<RectTransform>(checkbox.gameObject, bodyTransform.gameObject);
                                kToggleTransform.gameObject.name = name + "KToggle";
                                kToggleTransform.position = new Vector3((textAreaTransform.position.x + textAreaTransform.rect.xMin) + 9, (confirmButtonTransform.position.y - confirmButtonTransform.rect.yMin) - (confirmButtonTransform.rect.height - kToggleTransform.rect.height) / 2F, confirmButtonTransform.position.z);
                                kToggleTransform.sizeDelta = new Vector2(confirmButtonTransform.position.x - ((kToggleTransform.position.x - kToggleTransform.rect.width) + 8), 0);

                                LocText toggleTitle = kToggleTransform.Find("ThresholdPrefix")?.GetComponent<LocText>();
                                if (toggleTitle != null) {
                                    toggleTitle.text = toggleText;
                                    toggleTitle.fontSizeMin *= 1.5F;
                                }

                                KToggle kToggle = kToggleTransform.Find("Checkbox")?.GetComponent<KToggle>();
                                if (kToggle != null) {
                                    kToggle.isOn = false;

                                    if (valueChangedListener != null) {
                                        kToggle.onValueChanged += valueChangedListener;
                                    }
                                }
                            }
                        }
                    }
                }

                Transform titleTransform = panelTransform.Find("Title_BG")?.Find("Title");
                if (titleTransform != null && titleTransform.GetComponent<LocText>() != null) {
                    titleTransform.GetComponent<LocText>().text = title;
                }
            }

            return blueprintNameDialog;
        }

        public static FileNameDialog CreateEnterAccountIDDialog(string name, Blueprint blueprint, int attempt = 1) {
            GameObject uploadBlueprintDialogParent = GameScreenManager.Instance.GetParent(GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
            FileNameDialog uploadBlueprintDialog = Util.KInstantiateUI<FileNameDialog>(ScreenPrefabs.Instance.FileNameDialog.gameObject, uploadBlueprintDialogParent);

            uploadBlueprintDialog.name = name;
            Transform panelTransform = uploadBlueprintDialog.transform.Find("Panel");

            if (panelTransform != null) {
                Transform titleTransform = panelTransform.Find("Title_BG")?.Find("Title");
                LocText confirmButtonText = panelTransform.Find("Body")?.Find("ConfirmButton")?.Find("Text")?.GetComponent<LocText>();

                if (titleTransform != null && titleTransform.GetComponent<LocText>() != null) {
                    titleTransform.GetComponent<LocText>().text = "ENTER ACCOUNT ID ATTEMPT #" + attempt;
                }

                if (confirmButtonText != null) {
                    confirmButtonText.text = "Update ID";
                }
            }

            uploadBlueprintDialog.onConfirm = delegate (string accountKey) {
                accountKey = accountKey.Substring(0, accountKey.Length - 4);

                if (IOUtilities.IsValidAccountKey(accountKey)) {
                    SpeedControlScreen.Instance.Unpause(false);
                    BlueprintsAssets.BLUEPRINTS_ACCOUNTKEY = accountKey;
                    IOUtilities.WriteAccountKey();

                    if (IOUtilities.UploadBlueprint(blueprint)) {
                        PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, "Successfully uploaded blueprint \"" + blueprint.FriendlyName + "\"!", null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME);
                    }

                    else {
                        //I presume the API will give a response, that could be shown here. make it more useful to the end user.
                        PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, "Failed to upload blueprint \"" + blueprint.FriendlyName + "\"!", null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME);
                    }
                }

                else {
                    CreateEnterAccountIDDialog(name, blueprint, attempt + 1); //try again!
                }
            };

            uploadBlueprintDialog.onCancel = delegate {
                SpeedControlScreen.Instance.Unpause(false);

                PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, "Upload cancelled! Blueprint still created.", null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME);
                uploadBlueprintDialog.Deactivate();
            };

            uploadBlueprintDialog.Activate();
            return uploadBlueprintDialog;
        }
    }

    public static class IOUtilities {
        private const string STRING_API_DOMAIN = "http://localhost:8081/api";
        private const string STRING_API_ACCOUNTKEY = STRING_API_DOMAIN + "/account-key?";
        private const string STRING_API_ACCOUNTKEY_QUERY = "account-key=";

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

        public static void CreateAccountIDFile() {
            using (File.CreateText(BlueprintsAssets.BLUEPRINTS_PATH_ACCOUNTIDFILE)) { }
        }

        public static void ReadAccountID() {
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
                return;
            }

            using (StreamReader reader = File.OpenText(BlueprintsAssets.BLUEPRINTS_PATH_ACCOUNTIDFILE)) {
                BlueprintsAssets.BLUEPRINTS_ACCOUNTKEY = reader.ReadToEnd();
            }
        }

        public static void WriteAccountKey() {
            if (!Directory.Exists(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(BlueprintsAssets.BLUEPRINTS_PATH_CONFIGFOLDER);
            }

            using (StreamWriter writer = File.CreateText(BlueprintsAssets.BLUEPRINTS_PATH_ACCOUNTIDFILE)) {
                writer.WriteLine(BlueprintsAssets.BLUEPRINTS_ACCOUNTKEY);
            }
        }

        public static bool IsValidAccountKey(string accountKey) {
            if (string.IsNullOrEmpty(accountKey) || accountKey.Trim().Length == 0) {
                return false;
            }

            try {
                HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(STRING_API_ACCOUNTKEY + STRING_API_ACCOUNTKEY_QUERY + accountKey);
                httpWebRequest.Method = "GET";

                using (HttpWebResponse httpWebResponse = (HttpWebResponse) httpWebRequest.GetResponse()) {
                    Debug.Log((int) httpWebResponse.StatusCode);
                }

                return true;
            }

            catch (Exception exception) {
                Debug.LogError("Error when querying account key: " + accountKey + ",\n" + nameof(exception) + ": " + exception.Message);
                return false;
            }
        }

        public static bool UploadBlueprint(Blueprint blueprint) {
            //SOME API CALL
            return true;
        }
    }
}
