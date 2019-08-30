using UnityEngine;

namespace Blueprints {
    public sealed class SnapshotToolInput : MonoBehaviour {
        public SnapshotTool ParentTool { get; set; }

        public void Update() {
            if (ParentTool != null && Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_SNAPSHOTTOOL_DELETE)) {
                ParentTool.DeleteBlueprint();
            } 
        }
    }
}
