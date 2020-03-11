using Newtonsoft.Json;
using PeterHan.PLib;

namespace Blueprints {
    [JsonObject]
    public class BlueprintsOptions {
        [Option("Require Constructable", "Whether buildings must be constructable by the player to be used in blueprints.")]
        public bool RequireConstructable { get; set; } = true;

        [Option("Compress Blueprints", "Whether blueprints are compressed when writting to disk.")]
        public bool CompressBlueprints { get; set; } = true;

        [Option("Copy Building Settings", "Whether settings that can be copied between buildings are copied when using blueprints. Enabling this option will increase the amount of disk space taken by blueprints with buildings that have copiable settings.")]
        public bool BlueprintCopySettings { get; set; } = true;

        [Option("Copy Snapshot Settings", "Whether settings that can be copied between buildings are copied when using snapshots.")]
        public bool SnapshotCopySettings { get; set; } = true;

        [Option("FX Time", "How long FX created by Blueprints remain on the screen. Measured in seconds.")]
        public float FXTime { get; set; } = 4;
    }
}
