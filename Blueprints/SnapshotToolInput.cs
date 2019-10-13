using UnityEngine;

namespace Blueprints {
    public sealed class SnapshotToolInput : MonoBehaviour {
        public SnapshotTool ParentTool { get; set; }

        public void Update() {
            if ((ParentTool?.hasFocus ?? false) && Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT_NEWSNAPSHOT)) {
                ParentTool.DeleteBlueprint();
                GridCompositor.Instance.ToggleMajor(false);
            }
        }
    }
}
