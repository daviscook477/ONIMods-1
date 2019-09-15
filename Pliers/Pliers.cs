using Harmony;

using UnityEngine;

namespace Pliers {
    public static class PliersStringIDs {
        public const string STRING_PLIERS_NAME = "PLIERS.NAME";
        public const string STRING_PLIERS_TOOLTIP = "PLIERS.TOOLTIP";
        public const string STRING_PLIERS_TOOLTIP_TITLE = "PLIERS.TOOLTIP.TITLE";
        public const string STRING_PLIERS_ACTION_DRAG = "PLIERS.ACTION.DRAG";
        public const string STRING_PLIERS_ACTION_BACK = "PLIERS.ACTION.BACK";
    }

    public static class PliersAssets {
        public static string PLIERS_TOOLNAME = "PliersTool";
        public static string PLIERS_ICON_NAME = "PLIERS.TOOL.ICON";
        public static Sprite PLIERS_ICON_SPRITE;
        public static Sprite PLIERS_VISUALIZER_SPRITE;
        public static ToolMenu.ToolCollection PLIERS_TOOLCOLLECTION;

        public static Color PLIERS_COLOR_DRAG = new Color32(255, 140, 105, 255);

        public static KeyCode PLIERS_INPUT_KEYBIND_TOOL = KeyCode.None;

        public static string PLIERS_PATH_CONFIGFOLDER;
        public static string PLIERS_PATH_CONFIGFILE;
        public static string PLIERS_PATH_KEYCODESFILE;
    }

    public sealed class ToolMenuInputManager : MonoBehaviour {
        public void Update() {
            if (Input.GetKeyDown(PliersAssets.PLIERS_INPUT_KEYBIND_TOOL) && ToolMenu.Instance.currentlySelectedCollection != PliersAssets.PLIERS_TOOLCOLLECTION) {
                Traverse.Create(ToolMenu.Instance).Method("ChooseCollection", PliersAssets.PLIERS_TOOLCOLLECTION, true).GetValue();
            }
        }
    }
}