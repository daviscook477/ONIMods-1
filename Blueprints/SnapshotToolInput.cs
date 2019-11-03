using UnityEngine;

namespace Blueprints {
    public sealed class SnapshotToolInput : MonoBehaviour {
        public SnapshotTool ParentTool { get; set; }

        public void Update() {
            if ((ParentTool?.hasFocus ?? false) && BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT_NEWSNAPSHOT.IsActive()) {
                ParentTool.DeleteBlueprint();
                GridCompositor.Instance.ToggleMajor(false);
            }
        }
    }
}
