﻿using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using System.Reflection;
using PeterHan.PLib;
using KSerialization;

namespace Blueprints {
    public static class Utilities {
        public static bool CopySettingsWithDelegate(GameObject gameObject, GameObject sourceGameObject) {
            if (gameObject == null || gameObject == sourceGameObject)
                return false;
            KPrefabID sourceId = sourceGameObject.GetComponent<KPrefabID>();
            if (sourceId == null)
                return false;
            KPrefabID targetId = gameObject.GetComponent<KPrefabID>();
            if (targetId == null)
                return false;
            CopyBuildingSettings sourceCopySettings = sourceGameObject.GetComponent<CopyBuildingSettings>();
            if (sourceCopySettings == null)
                return false;
            CopyBuildingSettings targetCopySettings = gameObject.GetComponent<CopyBuildingSettings>();
            if (targetCopySettings == null)
                return false;
            if (sourceCopySettings.copyGroupTag != Tag.Invalid) {
                if (sourceCopySettings.copyGroupTag != targetCopySettings.copyGroupTag)
                    return false;
            }
            else if (targetId.PrefabID() != sourceId.PrefabID())
                return false;
            gameObject.AddComponent<DelayedSettingsCopy>().settingsSource = sourceGameObject;
            return true;
        }

        public static bool CopySettingsWithReflection(GameObject gameObject, GameObject sourceGameObject) {
            foreach (KMonoBehaviour component in gameObject.GetComponents<KMonoBehaviour>()) {
                Type type = component.GetType();
                Component tmpComponent = sourceGameObject.GetComponent(type);
                if (!(tmpComponent is KMonoBehaviour)) {
                    PUtil.LogDebug($"Could not copy serialized settings on component of type {component.GetType().Name} as it did not inherit from KMonoBehavior");
                    return false;
                }
                KMonoBehaviour sourceComponent = (KMonoBehaviour)tmpComponent;
                BindingFlags searchFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                foreach (FieldInfo field in sourceComponent.GetType().GetFields(searchFlags)) {
                    if (Attribute.IsDefined(field, typeof(Serialize))) {
                        object value = field.GetValue(sourceComponent);
                        Stream stream = new MemoryStream();
                        BinaryWriter templateWriter = new BinaryWriter(stream);
                        SerializationTemplate serTemplate = new SerializationTemplate(value.GetType());
                        serTemplate.SerializeTemplate(templateWriter);
                        stream.Flush();
                        KBinaryReader templateReader = new KBinaryReader(stream);
                        DeserializationTemplate deserTemplate = new DeserializationTemplate(value.GetType().ToString(), templateReader);
                        object copy = Activator.CreateInstance(value.GetType());
                        DeserializationMapping mapping = new DeserializationMapping(deserTemplate, serTemplate);
                        Stream objStream = new MemoryStream();
                        BinaryWriter objWriter = new BinaryWriter(objStream);
                        Serializer.Serialize(value, objWriter);
                        objWriter.Flush();
                        KBinaryReader objReader = new KBinaryReader(objStream);
                        mapping.Deserialize(copy, objReader);
                        if (value != null && copy == null) {
                            PUtil.LogDebug($"Could not copy field {field.Name} on type {type.ToString()} because serialization/deserialization failed");
                        }
                        type.GetField(field.Name, searchFlags).SetValue(component, copy);
                    }
                }
            }
            return true;
        }

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

        public static bool IsBuildable(this BuildingDef buildingDef) {
            if (!BlueprintsAssets.Options.RequireConstructable) {
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
}
