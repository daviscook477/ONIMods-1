using Harmony;
using System.Reflection;
using UnityEngine;

namespace Blueprints {
    public sealed class CreateBlueprintTool : FilteredDragTool {
        public static CreateBlueprintTool Instance { get; private set; }

        private bool uploadSelected = false;

        public CreateBlueprintTool() {
            Instance = this;
        }

        public static void DestroyInstance() {
            Instance = null;
        }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();

            visualizer = new GameObject("CreateBlueprintVisualizer");
            visualizer.SetActive(false);

            GameObject offsetObject = new GameObject();
            SpriteRenderer spriteRenderer = offsetObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = BlueprintsAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
            spriteRenderer.sprite = BlueprintsAssets.BLUEPRINTS_CREATE_VISUALIZER_SPRITE;

            offsetObject.transform.SetParent(visualizer.transform);
            offsetObject.transform.localPosition = new Vector3(0, Grid.HalfCellSizeInMeters);
            offsetObject.transform.localScale = new Vector3(
                Grid.CellSizeInMeters / (spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit),
                Grid.CellSizeInMeters / (spriteRenderer.sprite.texture.height / spriteRenderer.sprite.pixelsPerUnit)
            );

            offsetObject.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
            visualizer.transform.SetParent(transform);

            FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");
            FieldInfo areaVisualizerSpriteRendererField = AccessTools.Field(typeof(DragTool), "areaVisualizerSpriteRenderer");

            GameObject areaVisualizer = Util.KInstantiate(Traverse.Create(DeconstructTool.Instance).Field("areaVisualizer").GetValue<GameObject>());
            areaVisualizer.SetActive(false);

            areaVisualizer.name = "CreateBlueprintAreaVisualizer";
            areaVisualizerSpriteRendererField.SetValue(this, areaVisualizer.GetComponent<SpriteRenderer>());
            areaVisualizer.transform.SetParent(transform);
            areaVisualizer.GetComponent<SpriteRenderer>().color = BlueprintsAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
            areaVisualizer.GetComponent<SpriteRenderer>().material.color = BlueprintsAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;

            areaVisualizerField.SetValue(this, areaVisualizer);

            gameObject.AddComponent<CreateBlueprintToolHoverCard>();
        }

        protected override void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp) {
            base.OnDragComplete(cursorDown, cursorUp);

            if (hasFocus) {
                Grid.PosToXY(cursorDown, out int x0, out int y0);
                Grid.PosToXY(cursorUp, out int x1, out int y1);

                if (x0 > x1) {
                    Util.Swap(ref x0, ref x1);
                }

                if (y0 < y1) {
                    Util.Swap(ref y0, ref y1);
                }

                Blueprint blueprint = BlueprintsState.CreateBlueprint(new Vector2I(x0, y0), new Vector2I(x1, y1), this);
                if (blueprint.IsEmpty()) {
                    PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_EMPTY), null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME);
                }

                else {
                    FileNameDialog blueprintNameDialog = UIUtilities.CreateTextEntryDialog("BlueprintRenameDialog", Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_NAMEBLUEPRINT_TITLE), true, "Upload Blueprint", delegate(bool value) {
                        uploadSelected = value;
                    });

                    SpeedControlScreen.Instance.Pause(false);
                    blueprintNameDialog.onConfirm = delegate (string blueprintName) {
                        blueprint.Rename(blueprintName.Substring(0, blueprintName.Length - 4));
                        blueprintNameDialog.Deactivate();
                        blueprint.Write();
                       
                        BlueprintsState.LoadedBlueprints.Add(blueprint);
                        BlueprintsState.SelectedBlueprintIndex = BlueprintsState.LoadedBlueprints.Count - 1;

                        if (uploadSelected) {
                            if(IOUtilities.IsValidAccountKey(BlueprintsAssets.BLUEPRINTS_ACCOUNTKEY)) {
                                //THIS SHOULD PROBABLY GO INTO ITS OWN THREAD TO NOT GRIND THE GAME TO A HALT WHILE IT'S UPLOADING/IF THERE ARE CONNECTION ISSUES..

                                if (IOUtilities.UploadBlueprint(blueprint)) {
                                    PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, "Successfully uploaded blueprint \"" + blueprint.FriendlyName + "\"!", null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME);
                                }

                                else {
                                    //I presume the API will give a response, that could be shown here. make it more useful to the end user.
                                    PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, "Failed to upload blueprint \"" + blueprint.FriendlyName + "\"!", null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME);
                                }
                            }

                            else {
                                UIUtilities.CreateEnterAccountIDDialog("BlueprintUploadDialog", blueprint);
                            }                   
                        }

                        else {
                            SpeedControlScreen.Instance.Unpause(false);
                            PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_CREATED), null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME);
                        }
                    };

                    blueprintNameDialog.onCancel = delegate {
                        SpeedControlScreen.Instance.Unpause(false);

                        PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_CREATE_CANCELLED), null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME);
                        blueprintNameDialog.Deactivate();
                    };

                    blueprintNameDialog.Activate();
                }
            }
        }
    }
}
