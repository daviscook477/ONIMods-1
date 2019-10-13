
using UnityEngine;

namespace Blueprints {
    public sealed class UseBlueprintTool : InterfaceTool {
        public static UseBlueprintTool Instance { get; private set; }

        public UseBlueprintTool() {
            Instance = this;
        }

        public static void DestroyInstance() {
            Instance = null;
        }

        public void CreateVisualizer() {
            if (visualizer != null) {
                Destroy(visualizer);
            }

            visualizer = new GameObject("UseBlueprintVisualizer");
            visualizer.SetActive(false);

            GameObject offsetObject = new GameObject();
            SpriteRenderer spriteRenderer = offsetObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = BlueprintsAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
            spriteRenderer.sprite = BlueprintsAssets.BLUEPRINTS_USE_VISUALIZER_SPRITE;

            offsetObject.transform.SetParent(visualizer.transform);
            offsetObject.transform.localPosition = new Vector3(0, Grid.HalfCellSizeInMeters);
            offsetObject.transform.localScale = new Vector3(
                Grid.CellSizeInMeters / (spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit),
                Grid.CellSizeInMeters / (spriteRenderer.sprite.texture.height / spriteRenderer.sprite.pixelsPerUnit)
            );

            offsetObject.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
            visualizer.transform.SetParent(transform);

            OnMouseMove(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
        }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();
            gameObject.AddComponent<UseBlueprintToolHoverCard>();
        }

        protected override void OnActivateTool() {
            base.OnActivateTool();

            gameObject.AddComponent<UseBlueprintToolInput>();
            gameObject.GetComponent<UseBlueprintToolInput>().ParentTool = this;
            ToolMenu.Instance.PriorityScreen.Show(true);

            if (BlueprintsState.HasBlueprints()) {
                GridCompositor.Instance.ToggleMajor(true);
            }

            if (Input.GetKey(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RELOAD) || Input.GetKeyUp(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RELOAD)) {
                int oldBlueprintCount = BlueprintsState.LoadedBlueprints.Count;
                Utilities.ReloadBlueprints(true);

                int blueprintCountDelta = BlueprintsState.LoadedBlueprints.Count - oldBlueprintCount;
                PopFXManager.Instance.SpawnFX(BlueprintsAssets.BLUEPRINTS_CREATE_ICON_SPRITE, string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS), Mathf.Abs(blueprintCountDelta) + " " + (blueprintCountDelta >= 0 ? Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS_ADDITIONAL) : Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS_FEWER)), BlueprintsState.LoadedBlueprints.Count), null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), BlueprintsAssets.BLUEPRINTS_CONFIG_FXTIME * 4);
            }

            if (BlueprintsState.HasBlueprints()) {
                BlueprintsState.VisualizeBlueprint(Grid.PosToXY(PlayerController.GetCursorPos(KInputManager.GetMousePos())), BlueprintsState.SelectedBlueprint);
                if (visualizer != null) {
                    Destroy(visualizer);
                    visualizer = null;
                }
            }

            else {
                CreateVisualizer();
            }
        }

        protected override void OnDeactivateTool(InterfaceTool newTool) {
            base.OnDeactivateTool(newTool);

            if (gameObject.GetComponent<UseBlueprintToolInput>() != null) {
                Destroy(gameObject.GetComponent<UseBlueprintToolInput>());
            }

            BlueprintsState.ClearVisuals();
            ToolMenu.Instance.PriorityScreen.Show(false);
            GridCompositor.Instance.ToggleMajor(false);
        }

        public override void OnLeftClickDown(Vector3 cursorPos) {
            base.OnLeftClickDown(cursorPos);

            if (hasFocus) {
                BlueprintsState.UseBlueprint(Grid.PosToXY(cursorPos));
            }
        }

        public override void OnMouseMove(Vector3 cursorPos) {
            base.OnMouseMove(cursorPos);

            if (hasFocus) {
                BlueprintsState.UpdateVisual(Grid.PosToXY(cursorPos));
            }
        }
    }
}
