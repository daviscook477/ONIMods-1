using System.Collections.Generic;

namespace Blueprints {
    public sealed class UseBlueprintToolHoverCard : HoverTextConfiguration {
        public UseBlueprintToolHoverCard() {
            ToolName = "USE BLUEPRINT TOOL";
        }

        public override void UpdateHoverElements(List<KSelectable> hoveredObjects) {
            HoverTextScreen screenInstance = HoverTextScreen.Instance;
            HoverTextDrawer drawer = screenInstance.BeginDrawing();
            drawer.BeginShadowBar(false);

            DrawTitle(screenInstance, drawer);
            drawer.NewLine(26);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
            drawer.DrawText("CLICK", Styles_Instruction.Standard);
            drawer.AddIndent(8);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
            drawer.DrawText("BACK", Styles_Instruction.Standard);
            drawer.NewLine(32);

            if (BlueprintsState.LoadedBlueprints.Count > 0) {
                drawer.DrawText(BlueprintsAssets.BLUEPRINTS_STRING_CYCLEBLUEPRINTS, Styles_Instruction.Standard);
                drawer.NewLine(20);

                drawer.DrawText(BlueprintsAssets.BLUEPRINTS_STRING_RENAMEBLUEPRINT, Styles_Instruction.Standard);
                drawer.NewLine(20);

                drawer.DrawText(BlueprintsAssets.BLUEPRINTS_STRING_DELETEBLUEPRINT, Styles_Instruction.Standard);
                drawer.NewLine(32);

                drawer.DrawText("Selected \"" + BlueprintsState.SelectedBlueprint.FriendlyName + "\"    (" + (BlueprintsState.SelectedBlueprintIndex + 1) + "/" + BlueprintsState.LoadedBlueprints.Count + ")", Styles_Instruction.Standard);
            }

            else {
                drawer.DrawText("No blueprints loaded!", Styles_Instruction.Standard);
            }
                
            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
