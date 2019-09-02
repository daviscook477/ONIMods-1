using System;

using UnityEngine;

namespace Blueprints {
    public sealed class UseBlueprintToolInput : MonoBehaviour {
        public UseBlueprintTool ParentTool { get; set; }

        public void Update() {
            if ((ParentTool?.hasFocus ?? false) && BlueprintsState.LoadedBlueprints.Count > 0) {
                bool blueprintChanged = false;

                if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_RENAME)) {
                    FileNameDialog blueprintNameDialog = Utilities.CreateBlueprintRenameDialog();
                    SpeedControlScreen.Instance.Pause(false);

                    blueprintNameDialog.onConfirm = new Action<string>(blueprintName => {
                        BlueprintsState.SelectedBlueprint.Rename(blueprintName.Substring(0, blueprintName.Length - 4));
                        BlueprintsState.SelectedBlueprint.Write();

                        blueprintNameDialog.Deactivate();
                        SpeedControlScreen.Instance.Unpause(false);
                    });

                    blueprintNameDialog.Activate();
                }

                else if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_DELETE)) {
                    BlueprintsState.SelectedBlueprint.DeleteFile();
                    PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, "Deleted \"" + BlueprintsState.SelectedBlueprint.FriendlyName + "\" (" + (BlueprintsState.LoadedBlueprints.Count - 1) + " remaining)", null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_FXTIME);

                    BlueprintsState.LoadedBlueprints.RemoveAt(BlueprintsState.SelectedBlueprintIndex);
                    if (BlueprintsState.SelectedBlueprintIndex >= BlueprintsState.LoadedBlueprints.Count) {
                        BlueprintsState.SelectedBlueprintIndex = BlueprintsState.LoadedBlueprints.Count - 1;
                    }

                    blueprintChanged = true;
                }

                if (BlueprintsState.LoadedBlueprints.Count > 1) {
                    if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_CYCLELEFT)) {
                        if (--BlueprintsState.SelectedBlueprintIndex < 0) {
                            BlueprintsState.SelectedBlueprintIndex = BlueprintsState.LoadedBlueprints.Count - 1;
                        }

                        blueprintChanged = true;
                    }

                    else if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL_CYCLERIGHT)) {
                        if (++BlueprintsState.SelectedBlueprintIndex >= BlueprintsState.LoadedBlueprints.Count) {
                            BlueprintsState.SelectedBlueprintIndex = 0;
                        }

                        blueprintChanged = true;
                    }
                }

                if (blueprintChanged) {
                    BlueprintsState.ClearVisuals();

                    if(BlueprintsState.LoadedBlueprints.Count > 0) {
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
