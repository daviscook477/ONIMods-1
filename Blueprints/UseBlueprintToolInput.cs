using Harmony;
using System.IO;
using TMPro;
using UnityEngine;

namespace Blueprints {
    public sealed class UseBlueprintToolInput : MonoBehaviour {
        public UseBlueprintTool ParentTool { get; set; }

        public void Update() {
            if ((ParentTool?.hasFocus ?? false) && BlueprintsState.LoadedBlueprints.Count > 0) {
                bool blueprintChanged = false;

                if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_FOLDER)) {
                    void onConfirmDelegate(string blueprintFolder, FileNameDialog parent) {
                        string newFolder = blueprintFolder.Trim(' ', '/', '\\');

                        if (newFolder == BlueprintsState.SelectedBlueprint.Folder) {
                            PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_FOLDERBLUEPRINT_NA), null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME * 2, false, false);
                        }

                        else {
                            string blueprintName = BlueprintsState.SelectedBlueprint.FriendlyName;
                            
                            BlueprintsState.SelectedBlueprint.SetFolder(newFolder);
                            PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_MOVEDBLUEPRINT), blueprintName, newFolder), null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME * 4, false, false);
                        }
                        

                        SpeedControlScreen.Instance.Unpause(false);
                        parent.Deactivate();
                    }

                    FileNameDialog blueprintFolderDialog = UIUtilities.CreateTextDialog(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_FOLDERBLUEPRINT_TITLE), true, onConfirmDelegate);
                    SpeedControlScreen.Instance.Pause(false);
                    blueprintFolderDialog.Activate();
                }

                else if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RENAME)) {
                    void onConfirmDelegate(string blueprintName, FileNameDialog parent) {
                        BlueprintsState.SelectedBlueprint.Rename(blueprintName);
  
                        SpeedControlScreen.Instance.Unpause(false);
                        parent.Deactivate();
                    }

                    FileNameDialog blueprintNameDialog = UIUtilities.CreateTextDialog(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_NAMEBLUEPRINT_TITLE), false, onConfirmDelegate);
                    SpeedControlScreen.Instance.Pause(false);
                    blueprintNameDialog.Activate();
                }

                else if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_DELETE)) {
                    BlueprintsState.SelectedBlueprint.DeleteFile();
                    BlueprintsState.SelectedFolder.DeleteBlueprint(BlueprintsState.SelectedBlueprint);
                    
                    if(!BlueprintsState.HasBlueprints()) {
                        GridCompositor.Instance.ToggleMajor(false);
                    }

                    blueprintChanged = true;
                }

                else if (BlueprintsState.LoadedBlueprints.Count > 0) {
                    if (BlueprintsState.SelectedFolder.BlueprintCount > 1) {
                        if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_LEFT)) {
                            blueprintChanged = BlueprintsState.SelectedFolder.PreviousBlueprint();
                        }

                        else if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_RIGHT)) {
                            blueprintChanged = BlueprintsState.SelectedFolder.NextBlueprint();
                        }
                    }

                    if (BlueprintsState.LoadedBlueprints.Count > 1) {
                        if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_UP)) {
                            if (++BlueprintsState.SelectedBlueprintFolderIndex >= BlueprintsState.LoadedBlueprints.Count) {
                                BlueprintsState.SelectedBlueprintFolderIndex = 0;
                            }

                            blueprintChanged = true;
                        }

                        else if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_DOWN)) {
                            if (--BlueprintsState.SelectedBlueprintFolderIndex < 0) {
                                BlueprintsState.SelectedBlueprintFolderIndex = BlueprintsState.LoadedBlueprints.Count - 1;
                            }

                            blueprintChanged = true;
                        }
                    }
                }

                if (blueprintChanged) {
                    BlueprintsState.ClearVisuals();

                    if (BlueprintsState.HasBlueprints()) {
                        BlueprintsState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), BlueprintsState.SelectedBlueprint);
                    }

                    else {
                        UseBlueprintTool.Instance.CreateVisualizer();
                    }
                }
            }
        }
    }
}
