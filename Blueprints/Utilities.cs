﻿using Harmony;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using PeterHan.PLib;
using KSerialization;

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

    public static class CopyUtilities {
        private static bool DestroyAndReturnFalse(GameObject sourceGameObject) {
            UnityEngine.Object.Destroy(sourceGameObject);
            return false;
        }

        /// <summary>
        /// Copies settings from a source GameObject to another GameObject using the copy settings
        /// event/delegate system already used within the base game's "copy" settings button.
        /// </summary>
        /// <param name="gameObject">The GameObject to paste settings onto</param>
        /// <param name="sourceGameObject">The GameObject to copy settings from</param>
        /// <returns></returns>
        public static bool CopySettingsWithDelegateAndDestroy(GameObject gameObject, GameObject sourceGameObject) {
            if (gameObject == null || gameObject == sourceGameObject)
                return DestroyAndReturnFalse(sourceGameObject);
            KPrefabID sourceId = sourceGameObject.GetComponent<KPrefabID>();
            if (sourceId == null)
                return DestroyAndReturnFalse(sourceGameObject);
            KPrefabID targetId = gameObject.GetComponent<KPrefabID>();
            if (targetId == null)
                return DestroyAndReturnFalse(sourceGameObject);
            CopyBuildingSettings sourceCopySettings = sourceGameObject.GetComponent<CopyBuildingSettings>();
            if (sourceCopySettings == null)
                return DestroyAndReturnFalse(sourceGameObject);
            CopyBuildingSettings targetCopySettings = gameObject.GetComponent<CopyBuildingSettings>();
            if (targetCopySettings == null)
                return DestroyAndReturnFalse(sourceGameObject);
            if (sourceCopySettings.copyGroupTag != Tag.Invalid) {
                if (sourceCopySettings.copyGroupTag != targetCopySettings.copyGroupTag)
                    return DestroyAndReturnFalse(sourceGameObject);
            }
            else if (targetId.PrefabID() != sourceId.PrefabID())
                return DestroyAndReturnFalse(sourceGameObject);
            // When copying settings we need to delay it until the building is fully intialized so we queue
            // the copy to happen later by adding a custom component that does exactly that.
            gameObject.AddComponent<DelayedSettingsCopy>().settingsSource = sourceGameObject;
            return true;
        }

        /// <summary>
        /// Serializes a building into a form that can be safely passed around without leaking memory
        /// and that can be deserialized into a deep copy.
        /// </summary>
        /// <param name="sourceGameObject">The GameObject backing the building</param>
        /// <returns></returns>
        public static SerializedBuilding SerializeBuilding(GameObject sourceGameObject)
        {
            SaveLoadRoot saveLoadRoot = sourceGameObject.GetComponent<SaveLoadRoot>();
            if (saveLoadRoot == null)
            {
                PUtil.LogDebug("Unable to serialize GameObject because it lacks a SaveLoadRoot component.");
                return null;
            }
            Manager.Clear();
            using (MemoryStream objStream = new MemoryStream())
            {
                // Process serialization in two stages. The object must be serialized
                // first to populate the templates in the serialization Manager.
                using (BinaryWriter objWriter = new BinaryWriter(objStream))
                {
                    saveLoadRoot.SaveWithoutTransform(objWriter);
                }
                using (MemoryStream fullStream = new MemoryStream())
                {
                    using (BinaryWriter fullWriter = new BinaryWriter(fullStream))
                    {
                        // But the templates must be serialized first on the final stream
                        // before the object in order for the deserialization utility to
                        // properly reconstruct the type.
                        Manager.SerializeDirectory(fullWriter);
                        fullWriter.Write(objStream.ToArray());
                        return new SerializedBuilding {
                            PrefabID = sourceGameObject.PrefabID(),
                            Serialization = fullStream.ToArray()
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Deserializes a serialized building back into a GameObject that is a deep copy
        /// of whatever building was originally serialized.
        /// </summary>
        /// <param name="serializedBuilding">The serialization of a building that we want to reconstruct</param>
        /// <returns></returns>
        public static GameObject DeserializeGameObject(SerializedBuilding serializedBuilding)
        {
            Tag prefabID = serializedBuilding.PrefabID;
            GameObject prefab = SaveLoader.Instance.saveManager.GetPrefab(prefabID);
            GameObject cloneGameObject = Util.KInstantiate(prefab,
                Vector3.zero,
                Quaternion.identity,
                null, null, false, 0);
            cloneGameObject.transform.localScale = Vector3.one;
            cloneGameObject.SetActive(true);
            Manager.Clear();
            IReader reader = new FastReader(serializedBuilding.Serialization);
            Manager.DeserializeDirectory(reader);
            Traverse.Create(typeof(SaveLoadRoot)).CallMethod("LoadInternal", cloneGameObject, reader);
            cloneGameObject.SetActive(false);
            return cloneGameObject;
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
