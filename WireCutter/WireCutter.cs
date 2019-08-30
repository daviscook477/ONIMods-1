using Harmony;

using UnityEngine;

namespace WireCutter {
    public static class WireCutterAssets {
        public static string                  WIRECUTTER_NAME = "Wire Cutter";
        public static string                  WIRECUTTER_TOOLNAME = "WireCutterTool";
        public static string                  WIRECUTTER_TOOLTIP = "Disconnect Utilities " + Utilities.GetKeyCodeString(KeyCode.None);
        public static string                  WIRECUTTER_ICON_NAME = "WIRECUTTER.TOOL.ICON";
        public static Sprite                  WIRECUTTER_ICON_SPRITE;
        public static Sprite                  WIRECUTTER_VISUALIZER_SPRITE;
        public static ToolMenu.ToolCollection WIRECUTTER_TOOLCOLLECTION;

        public static Color                   WIRECUTTER_COLOR_DRAG = new Color32(255, 140, 105, 255);

        public static KeyCode                 WIRECUTTER_INPUT_KEYBIND_TOOL = KeyCode.None;

        public static string                  WIRECUTTER_PATH_CONFIGFILE;
        public static string                  WIRECUTTER_PATH_KEYCODESFILE;
    }

    public sealed class ToolMenuInputManager : MonoBehaviour {
        public void Update() {
            if (Input.GetKeyDown(WireCutterAssets.WIRECUTTER_INPUT_KEYBIND_TOOL) && ToolMenu.Instance.currentlySelectedCollection != WireCutterAssets.WIRECUTTER_TOOLCOLLECTION) {
                AccessTools.Method(typeof(ToolMenu), "ChooseCollection").Invoke(ToolMenu.Instance, new object[] { WireCutterAssets.WIRECUTTER_TOOLCOLLECTION, true });
            }
        }
    }
}