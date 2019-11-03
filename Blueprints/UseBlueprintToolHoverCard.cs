using System.Collections.Generic;

namespace Blueprints {
    public sealed class UseBlueprintToolHoverCard : HoverTextConfiguration {
        public int PrefabErrorCount = 0;

        public UseBlueprintToolHoverCard() {
            ToolName = Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_TOOLTIP_TITLE);
        }

        public override void UpdateHoverElements(List<KSelectable> hoveredObjects) {
            HoverTextScreen screenInstance = HoverTextScreen.Instance;
            HoverTextDrawer drawer = screenInstance.BeginDrawing();
            drawer.BeginShadowBar(false);

            DrawTitle(screenInstance, drawer);
            drawer.NewLine(26);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_left"), 20);
            drawer.DrawText(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_ACTION_CLICK), Styles_Instruction.Standard);
            drawer.AddIndent(8);

            drawer.DrawIcon(screenInstance.GetSprite("icon_mouse_right"), 20);
            drawer.DrawText(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_ACTION_BACK), Styles_Instruction.Standard);
            drawer.NewLine(32);

            if (BlueprintsState.HasBlueprints()) {
                if (BlueprintsState.SelectedFolder.BlueprintCount > 0) {
                    drawer.DrawText(string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_CYCLEFOLDERS), BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_UP.GetStringFormatted(), BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_DOWN.GetStringFormatted()), Styles_Instruction.Standard);
                    drawer.NewLine(20);

                    drawer.DrawText(string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_CYCLEBLUEPRINTS), BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_LEFT.GetStringFormatted(), BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_RIGHT.GetStringFormatted()), Styles_Instruction.Standard);
                    drawer.NewLine(32);

                    drawer.DrawText(string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_FOLDERBLUEPRINT), BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_FOLDER.GetStringFormatted()), Styles_Instruction.Standard);
                    drawer.NewLine(20);

                    drawer.DrawText(string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_NAMEBLUEPRINT),BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_RENAME.GetStringFormatted()), Styles_Instruction.Standard);
                    drawer.NewLine(20);

                    drawer.DrawText(string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_DELETEBLUEPRINT), BlueprintsAssets.BLUEPRINTS_KEYBIND_USE_DELETE.GetStringFormatted()), Styles_Instruction.Standard);

                    if (PrefabErrorCount > 0) {
                        drawer.NewLine(32);
                        drawer.DrawIcon(screenInstance.GetSprite("iconWarning"), 18);
                        drawer.DrawText(string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_ERRORMESSAGE), PrefabErrorCount), Styles_Instruction.Selected);
                    }

                    drawer.NewLine(32);
                    drawer.DrawText(string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_SELECTEDBLUEPRINT), BlueprintsState.SelectedBlueprint.FriendlyName, BlueprintsState.SelectedFolder.SelectedBlueprintIndex + 1, BlueprintsState.SelectedFolder.BlueprintCount, BlueprintsState.SelectedFolder.Name, BlueprintsState.SelectedBlueprintFolderIndex + 1, BlueprintsState.LoadedBlueprints.Count), Styles_Instruction.Standard);
                }

                else {
                    drawer.DrawText(string.Format(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_FOLDEREMPTY), BlueprintsState.SelectedFolder.Name), Styles_Instruction.Standard);
                }
            }

            else {
                drawer.DrawText(Strings.Get(BlueprintsStrings.STRING_BLUEPRINTS_USE_NOBLUEPRINTS), Styles_Instruction.Standard);
            }

            drawer.EndShadowBar();
            drawer.EndDrawing();
        }
    }
}
