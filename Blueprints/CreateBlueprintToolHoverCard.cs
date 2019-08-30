using System.Collections.Generic;

namespace Blueprints {
    public sealed class CreateBlueprintToolHoverCard : HoverTextConfiguration {
        public CreateBlueprintToolHoverCard() {
            ToolName = "CREATE BLUEPRINT TOOL";
        }

        public override void UpdateHoverElements(List<KSelectable> hoveredObjects) {
            HoverTextScreen screenInstance = HoverTextScreen.Instance;
            HoverTextDrawer drawer = screenInstance.BeginDrawing();
            drawer.BeginShadowBar(false);

            DrawTitle(screenInstance, drawer);
            drawer.NewLine(26);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
            drawer.DrawText("DRAG", Styles_Instruction.Standard);
            drawer.AddIndent(8);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
            drawer.DrawText("BACK", Styles_Instruction.Standard);

            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
