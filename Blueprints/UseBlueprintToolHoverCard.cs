using System.Collections.Generic;

namespace Blueprints {
    public sealed class UseBlueprintToolHoverCard : HoverTextConfiguration {
        public int PrefabErrorCount = 0;

        public UseBlueprintToolHoverCard() {
            ToolName = Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_TOOLTIP_TITLE);
        }

        public override void UpdateHoverElements(List<KSelectable> hoveredObjects) {
            HoverTextScreen screenInstance = HoverTextScreen.Instance;
            HoverTextDrawer drawer = screenInstance.BeginDrawing();
            drawer.BeginShadowBar(false);

            DrawTitle(screenInstance, drawer);
            drawer.NewLine(26);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
            drawer.DrawText(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_ACTION_CLICK), Styles_Instruction.Standard);
            drawer.AddIndent(8);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
            drawer.DrawText(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_ACTION_BACK), Styles_Instruction.Standard);
            drawer.NewLine(32);

            if (BlueprintsState.LoadedBlueprints.Count > 0) {
                drawer.DrawText(string.Format(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_CYCLEBLUEPRINTS), Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLELEFT), Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLERIGHT)), Styles_Instruction.Standard);
                drawer.NewLine(20);

                drawer.DrawText(string.Format(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_NAMEBLUEPRINT), Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RENAME)), Styles_Instruction.Standard);
                drawer.NewLine(20);

                drawer.DrawText(string.Format(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_DELETEBLUEPRINT), Utilities.GetKeyCodeString(BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_DELETE)), Styles_Instruction.Standard);

                if (PrefabErrorCount > 0) {
                    drawer.NewLine(32);
                    drawer.DrawIcon(screenInstance.GetSprite("iconWarning"), 18);
                    drawer.DrawText(string.Format(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_ERRORMESSAGE), PrefabErrorCount), Styles_Instruction.Selected);
                }

                drawer.NewLine(32);
                drawer.DrawText(string.Format(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_SELECTEDBLUEPRINT), BlueprintsState.SelectedBlueprint.FriendlyName, BlueprintsState.SelectedBlueprintIndex + 1, BlueprintsState.LoadedBlueprints.Count), Styles_Instruction.Standard);
            }

            else {
                drawer.DrawText(Strings.Get(BlueprintsStringIDs.STRING_BLUEPRINTS_USE_NOBLUEPRINTS), Styles_Instruction.Standard);
            }

            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
