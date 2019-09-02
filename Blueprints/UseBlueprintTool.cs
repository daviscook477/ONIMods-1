using Harmony;

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

            if(BlueprintsState.LoadedBlueprints.Count > 0) {
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
