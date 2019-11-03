using Harmony;
using ModKeyBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STRINGS;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Blueprints {
    public static class BlueprintsStrings {
        public const string STRING_BLUEPRINTS_CREATE_NAME = "BLUEPRINTS.CREATE.NAME";
        public const string STRING_BLUEPRINTS_CREATE_TOOLTIP = "BLUEPRINTS.CREATE.TOOLTIP";
        public const string STRING_BLUEPRINTS_CREATE_EMPTY = "BLUEPRINTS.CREATE.EMPTY";
        public const string STRING_BLUEPRINTS_CREATE_CREATED = "BLUEPRINTS.CREATE.CREATED";
        public const string STRING_BLUEPRINTS_CREATE_CANCELLED = "BLUEPRINTS.CREATE.CANCELLED";
        public const string STRING_BLUEPRINTS_CREATE_TOOLTIP_TITLE = "BLUEPRINTS.CREATE.TOOLTIP.TITLE";
        public const string STRING_BLUEPRINTS_CREATE_ACTION_DRAG = "BLUEPRINTS.CREATE.ACTION.DRAG";
        public const string STRING_BLUEPRINTS_CREATE_ACTION_BACK = "BLUEPRINTS.CREATE.ACTION.BACK";

        public const string STRING_BLUEPRINTS_USE_NAME = "BLUEPRINTS.USE.NAME";
        public const string STRING_BLUEPRINTS_USE_TOOLTIP = "BLUEPRINTS.USE.TOOLTIP";
        public const string STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS = "BLUEPRINTS.USE.LOADEDBLUEPRINTS";
        public const string STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS_ADDITIONAL = "BLUEPRINTS.USE.LOADEDBLUEPRINTS.ADDITIONAL";
        public const string STRING_BLUEPRINTS_USE_LOADEDBLUEPRINTS_FEWER = "BLUEPRINTS.USE.LOADEDBLUEPRINTS.FEWER";
        public const string STRING_BLUEPRINTS_USE_TOOLTIP_TITLE = "BLUEPRINTS.USE.TOOLTIP.TITLE";
        public const string STRING_BLUEPRINTS_USE_ACTION_CLICK = "BLUEPRINTS.USE.ACTION.CLICK";
        public const string STRING_BLUEPRINTS_USE_ACTION_BACK = "BLUEPRINTS.USE.ACTION.BACK";
        public const string STRING_BLUEPRINTS_USE_CYCLEFOLDERS = "BLUEPRINTS.USE.CYCLEFOLDERS";
        public const string STRING_BLUEPRINTS_USE_CYCLEBLUEPRINTS = "BLUEPRINTS.USE.CYCLEBLUEPRINTS";
        public const string STRING_BLUEPRINTS_USE_FOLDERBLUEPRINT = "BLUEPRINTS.USE.FOLDERBLUEPRINT";
        public const string STRING_BLUEPRINTS_USE_FOLDERBLUEPRINT_NA = "BLUEPRINTS.USE.FOLDERBLUEPRINT.NA";
        public const string STRING_BLUEPRINTS_USE_MOVEDBLUEPRINT = "BLUEPRINTS.USE.MOVEDBLUEPRINT";
        public const string STRING_BLUEPRINTS_USE_NAMEBLUEPRINT = "BLUEPRINTS.USE.NAMEBLUEPRINT";
        public const string STRING_BLUEPRINTS_USE_DELETEBLUEPRINT = "BLUEPRINTS.USE.DELETEBLUEPRINT";
        public const string STRING_BLUEPRINTS_USE_ERRORMESSAGE = "BLUEPRINTS.USE.ERRORMESSAGE";
        public const string STRING_BLUEPRINTS_USE_SELECTEDBLUEPRINT = "BLUEPRINTS.USE.SELECTEDBLUEPRINT";
        public const string STRING_BLUEPRINTS_USE_FOLDEREMPTY = "BLUEPRINTS.USE.FOLDEREMPTY";
        public const string STRING_BLUEPRINTS_USE_NOBLUEPRINTS = "BLUEPRINTS.USE.NOBLUEPRINTS";

        public const string STRING_BLUEPRINTS_SNAPSHOT_NAME = "BLUEPRINTS.SNAPSHOT.NAME";
        public const string STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP = "BLUEPRINTS.SNAPSHOT.TOOLTIP";
        public const string STRING_BLUEPRINTS_SNAPSHOT_EMPTY = "BLUEPRINTS.SNAPSHOT.EMPTY";
        public const string STRING_BLUEPRINTS_SNAPSHOT_TAKEN = "BLUEPRINTS.SNAPSHOT.TAKEN";
        public const string STRING_BLUEPRINTS_SNAPSHOT_TOOLTIP_TITLE = "BLUEPRINTS.SNAPSHOT.TOOLTIP.TITLE";
        public const string STRING_BLUEPRINTS_SNAPSHOT_ACTION_CLICK = "BLUEPRINTS.SNAPSHOT.ACTION.CLICK";
        public const string STRING_BLUEPRINTS_SNAPSHOT_ACTION_DRAG = "BLUEPRINTS.SNAPSHOT.ACTION.DRAG";
        public const string STRING_BLUEPRINTS_SNAPSHOT_ACTION_BACK = "BLUEPRINTS.SNAPSHOT.ACTION.BACK";
        public const string STRING_BLUEPRINTS_SNAPSHOT_NEWSNAPSHOT = "BLUEPRINTS.SNAPSHOT.NEWSNAPSHOT";

        public const string STRING_BLUEPRINTS_NAMEBLUEPRINT_TITLE = "BLUEPRINTS.NAMEBLUEPRINT.TITLE";
        public const string STRING_BLUEPRINTS_FOLDERBLUEPRINT_TITLE = "BLUEPRINTS.FOLDERBLUEPRINT.TITLE";
    }

    public static class BlueprintsAssets {
        public static string BLUEPRINTS_CREATE_TOOLNAME = "CreateBlueprintTool";
        public static string BLUEPRINTS_CREATE_ICON_NAME = "BLUEPRINTS.TOOL.CREATE_BLUEPRINT.ICON";
        public static Sprite BLUEPRINTS_CREATE_ICON_SPRITE;
        public static Sprite BLUEPRINTS_CREATE_VISUALIZER_SPRITE;
        public static ToolMenu.ToolCollection BLUEPRINTS_CREATE_TOOLCOLLECTION;

        public static string BLUEPRINTS_USE_TOOLNAME = "UseBlueprintTool";
        public static string BLUEPRINTS_USE_ICON_NAME = "BLUEPRINTS.TOOL.USE_BLUEPRINT.ICON";
        public static Sprite BLUEPRINTS_USE_ICON_SPRITE;
        public static Sprite BLUEPRINTS_USE_VISUALIZER_SPRITE;
        public static ToolMenu.ToolCollection BLUEPRINTS_USE_TOOLCOLLECTION;

        public static string BLUEPRINTS_SNAPSHOT_TOOLNAME = "SnapshotTool";
        public static string BLUEPRINTS_SNAPSHOT_ICON_NAME = "BLUEPRINTS.TOOL.SNAPSHOT.ICON";
        public static Sprite BLUEPRINTS_SNAPSHOT_ICON_SPRITE;
        public static Sprite BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE;
        public static ToolMenu.ToolCollection BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION;

        public static Color BLUEPRINTS_COLOR_VALIDPLACEMENT = Color.white;
        public static Color BLUEPRINTS_COLOR_INVALIDPLACEMENT = Color.red;
        public static Color BLUEPRINTS_COLOR_NOTECH = new Color32(30, 144, 255, 255);
        public static Color BLUEPRINTS_COLOR_BLUEPRINT_DRAG = new Color32(0, 119, 145, 255);

        public static KeyBinding BLUEPRINTS_KEYBIND_CREATE = new KeyBinding(KeyCode.None);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE = new KeyBinding(KeyCode.None);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_RELOAD = new KeyBinding(KeyCode.LeftShift);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_UP = new KeyBinding(KeyCode.UpArrow);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_DOWN = new KeyBinding(KeyCode.DownArrow);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_LEFT = new KeyBinding(KeyCode.LeftArrow);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_RIGHT = new KeyBinding(KeyCode.RightArrow);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_FOLDER = new KeyBinding(KeyCode.Home);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_RENAME = new KeyBinding(KeyCode.End);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_DELETE = new KeyBinding(KeyCode.Delete);
        public static KeyBinding BLUEPRINTS_KEYBIND_SNAPSHOT = new KeyBinding(KeyCode.None);
        public static KeyBinding BLUEPRINTS_KEYBIND_SNAPSHOT_NEWSNAPSHOT = new KeyBinding(KeyCode.Delete);

        public static HashSet<char> BLUEPRINTS_FILE_DISALLOWEDCHARACTERS;

        public static HashSet<char> BLUEPRINTS_PATH_DISALLOWEDCHARACTERS;
        public static string BLUEPRINTS_PATH_CONFIGFOLDER;
        public static string BLUEPRINTS_PATH_CONFIGFILE;
        public static string BLUEPRINTS_PATH_KEYCODESFILE;

        public static bool BLUEPRINTS_CONFIG_REQUIRECONSTRUCTABLE = true;
        public static bool BLUEPRINTS_CONFIG_COMPRESBLUEPRINTS = true;
        public static float BLUEPRINTS_CONFIG_FXTIME = 0.75F;

        static BlueprintsAssets() {
            BLUEPRINTS_FILE_DISALLOWEDCHARACTERS = new HashSet<char>();
            BLUEPRINTS_FILE_DISALLOWEDCHARACTERS.UnionWith(Path.GetInvalidFileNameChars());

            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS = new HashSet<char>();
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.UnionWith(Path.GetInvalidFileNameChars());
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.UnionWith(Path.GetInvalidPathChars());

            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove('/');
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove('\\');
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove(Path.DirectorySeparatorChar);
            BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Remove(Path.AltDirectorySeparatorChar);
        }
    }

    public static class BlueprintsState {
        private static int selectedBlueprintFolderIndex;
        public static int SelectedBlueprintFolderIndex {
            get {
                return selectedBlueprintFolderIndex;
            }

            set {
                selectedBlueprintFolderIndex = Mathf.Clamp(value, 0, LoadedBlueprints.Count - 1);
            }
        }

        public static List<BlueprintFolder> LoadedBlueprints { get; } = new List<BlueprintFolder>();
        public static BlueprintFolder SelectedFolder => LoadedBlueprints[SelectedBlueprintFolderIndex];
        public static Blueprint SelectedBlueprint => SelectedFolder.SelectedBlueprint;

        public static bool InstantBuild => DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

        private static readonly List<IVisual> foundationVisuals = new List<IVisual>();
        private static readonly List<IVisual> dependantVisuals = new List<IVisual>();
        private static readonly List<ICleanableVisual> cleanableVisuals = new List<ICleanableVisual>();

        public static readonly Dictionary<int, CellColorPayload> ColoredCells = new Dictionary<int, CellColorPayload>();

        public static bool HasBlueprints() {
            if (LoadedBlueprints.Count == 0) {
                return false;
            }

            foreach (BlueprintFolder blueprintFolder in LoadedBlueprints) {
                if (blueprintFolder.BlueprintCount > 0) {
                    return true;
                }
            }

            return false;
        }

        public static Blueprint CreateBlueprint(Vector2I topLeft, Vector2I bottomRight, FilteredDragTool originTool = null) {
            Blueprint blueprint = new Blueprint("unnamed", "");
            int blueprintHeight = (topLeft.y - bottomRight.y);

            for (int x = topLeft.x; x <= bottomRight.x; ++x) {
                for (int y = bottomRight.y; y <= topLeft.y; ++y) {
                    int cell = Grid.XYToCell(x, y);

                    if (Grid.IsVisible(cell)) {
                        bool emptyCell = true;

                        for (int layer = 0; layer < Grid.ObjectLayers.Length; ++layer) {
                            if (layer == 7) {
                                continue;
                            }

                            GameObject gameObject = Grid.Objects[cell, layer];
                            Building building;

                            if (gameObject != null && (building = gameObject.GetComponent<Building>()) != null && building.Def.IsBuildable() && (originTool == null || originTool.IsActiveLayer(originTool.GetFilterLayerFromGameObject(gameObject)))) {
                                PrimaryElement primaryElement;

                                if ((primaryElement = building.GetComponent<PrimaryElement>()) != null) {
                                    Vector2I centre = Grid.CellToXY(GameUtil.NaturalBuildingCell(building));

                                    BuildingConfig buildingConfig = new BuildingConfig {
                                        Offset = new Vector2I(centre.x - topLeft.x, blueprintHeight - (topLeft.y - centre.y)),
                                        BuildingDef = building.Def,
                                        Orientation = building.Orientation
                                    };

                                    buildingConfig.SelectedElements.Add(primaryElement.ElementID.CreateTag());
                                    if (building.Def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null) {
                                        buildingConfig.Flags = (int) building.Def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager()?.GetConnections(cell, false);
                                    }

                                    if (!blueprint.BuildingConfiguration.Contains(buildingConfig)) {
                                        blueprint.BuildingConfiguration.Add(buildingConfig);
                                    }
                                }

                                emptyCell = false;
                            }
                        }

                        if (emptyCell && (!Grid.IsSolidCell(cell) || Grid.Objects[cell, 7] != null && Grid.Objects[cell, 7].name == "DigPlacer")) {
                            Vector2I digLocation = new Vector2I(x - topLeft.x, blueprintHeight - (topLeft.y - y));

                            if (!blueprint.DigLocations.Contains(digLocation)) {
                                blueprint.DigLocations.Add(digLocation);
                            }
                        }
                    }
                }
            }

            return blueprint;
        }

        public static void VisualizeBlueprint(Vector2I topLeft, Blueprint blueprint) {
            if (blueprint == null) {
                return;
            }

            int errors = 0;
            ClearVisuals();

            foreach (BuildingConfig buildingConfig in blueprint.BuildingConfiguration) {
                if (buildingConfig.BuildingDef == null || buildingConfig.SelectedElements.Count == 0) {
                    ++errors;
                    continue;
                }

                if (buildingConfig.BuildingDef.BuildingPreview != null) {
                    int cell = Grid.XYToCell(topLeft.x + buildingConfig.Offset.x, topLeft.y + buildingConfig.Offset.y);

                    if (buildingConfig.BuildingDef.IsTilePiece) {
                        if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null) {
                            AddVisual(new UtilityVisual(buildingConfig, cell), buildingConfig.BuildingDef);
                        }

                        else {
                            AddVisual(new TileVisual(buildingConfig, cell), buildingConfig.BuildingDef);
                        }
                    }

                    else {
                        AddVisual(new BuildingVisual(buildingConfig, cell), buildingConfig.BuildingDef);
                    }
                }
            }

            foreach (Vector2I digLocation in blueprint.DigLocations) {
                foundationVisuals.Add(new DigVisual(Grid.XYToCell(topLeft.x + digLocation.x, topLeft.y + digLocation.y), digLocation));
            }

            if (UseBlueprintTool.Instance.GetComponent<UseBlueprintToolHoverCard>() != null) {
                UseBlueprintTool.Instance.GetComponent<UseBlueprintToolHoverCard>().PrefabErrorCount = errors;
            }
        }

        private static void AddVisual(IVisual visual, BuildingDef buildingDef) {
            if (buildingDef.IsFoundation) {
                foundationVisuals.Add(visual);
            }

            else {
                dependantVisuals.Add(visual);
            }

            if (visual is ICleanableVisual) {
                cleanableVisuals.Add((ICleanableVisual) visual);
            }
        }

        public static void UpdateVisual(Vector2I topLeft) {
            CleanDirtyVisuals();

            foundationVisuals.ForEach(foundationVisual => foundationVisual.MoveVisualizer(Grid.XYToCell(topLeft.x + foundationVisual.Offset.x, topLeft.y + foundationVisual.Offset.y)));
            dependantVisuals.ForEach(dependantVisual => dependantVisual.MoveVisualizer(Grid.XYToCell(topLeft.x + dependantVisual.Offset.x, topLeft.y + dependantVisual.Offset.y)));
        }

        public static void UseBlueprint(Vector2I topLeft) {
            CleanDirtyVisuals();

            foundationVisuals.ForEach(foundationVisual => foundationVisual.TryUse(Grid.XYToCell(topLeft.x + foundationVisual.Offset.x, topLeft.y + foundationVisual.Offset.y)));
            dependantVisuals.ForEach(dependantVisual => dependantVisual.TryUse(Grid.XYToCell(topLeft.x + dependantVisual.Offset.x, topLeft.y + dependantVisual.Offset.y)));
        }

        public static void ClearVisuals() {
            CleanDirtyVisuals();
            cleanableVisuals.Clear();

            foundationVisuals.ForEach(foundationVisual => Object.DestroyImmediate(foundationVisual.Visualizer));
            foundationVisuals.Clear();

            dependantVisuals.ForEach(dependantVisual => Object.DestroyImmediate(dependantVisual.Visualizer));
            dependantVisuals.Clear();
        }

        public static void CleanDirtyVisuals() {
            foreach (int cell in ColoredCells.Keys) {
                CellColorPayload cellColorPayload = ColoredCells[cell];
                TileVisualizer.RefreshCell(cell, cellColorPayload.TileLayer, cellColorPayload.ReplacementLayer);
            }

            ColoredCells.Clear();
            cleanableVisuals.ForEach(cleanableVisual => cleanableVisual.Clean());
        }
    }

    public sealed class ToolMenuInputManager : MonoBehaviour {
        public void Update() {
            ToolMenu.ToolCollection currentlySelectedCollection = ToolMenu.Instance.currentlySelectedCollection;

            if (BlueprintsAssets.BLUEPRINTS_KEYBIND_CREATE.IsActive() && currentlySelectedCollection != BlueprintsAssets.BLUEPRINTS_CREATE_TOOLCOLLECTION) {
                Traverse.Create(ToolMenu.Instance).Method("ChooseCollection", BlueprintsAssets.BLUEPRINTS_CREATE_TOOLCOLLECTION, true).GetValue();
            }

            else if (BlueprintsAssets.BLUEPRINTS_KEYBIND_USE.IsActive() && currentlySelectedCollection != BlueprintsAssets.BLUEPRINTS_USE_TOOLCOLLECTION) {
                Traverse.Create(ToolMenu.Instance).Method("ChooseCollection", BlueprintsAssets.BLUEPRINTS_USE_TOOLCOLLECTION, true).GetValue();
            }

            else if (BlueprintsAssets.BLUEPRINTS_KEYBIND_SNAPSHOT.IsActive() && currentlySelectedCollection != BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION) {
                Traverse.Create(ToolMenu.Instance).Method("ChooseCollection", BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION, true).GetValue();
            }
        }
    }

    public sealed class BuildingConfig : System.IEquatable<BuildingConfig> {
        public Vector2I Offset { get; set; } = new Vector2I(0, 0);
        public BuildingDef BuildingDef { get; set; }
        public List<Tag> SelectedElements { get; } = new List<Tag>();
        public Orientation Orientation { get; set; } = 0;
        public int Flags { get; set; } = 0;

        public void WriteBinary(BinaryWriter binaryWriter) {
            binaryWriter.Write(Offset.X);
            binaryWriter.Write(Offset.y);
            binaryWriter.Write(BuildingDef.PrefabID);
            binaryWriter.Write(SelectedElements.Count);
            SelectedElements.ForEach(selectedElement => binaryWriter.Write(selectedElement.GetHash()));
            binaryWriter.Write((int) Orientation);
            binaryWriter.Write(Flags);
        }

        public void WriteJSON(JsonWriter jsonWriter) {
            jsonWriter.WriteStartObject();

            if (Offset.x != 0 || Offset.y != 0) {
                jsonWriter.WritePropertyName("offset");
                jsonWriter.WriteStartObject();

                if (Offset.x != 0) {
                    jsonWriter.WritePropertyName("x");
                    jsonWriter.WriteValue(Offset.x);
                }
                
                if (Offset.y != 0) {
                    jsonWriter.WritePropertyName("y");
                    jsonWriter.WriteValue(Offset.y);
                }

                jsonWriter.WriteEndObject();
            }
            
            jsonWriter.WritePropertyName("buildingdef");
            jsonWriter.WriteValue(BuildingDef.PrefabID);

            jsonWriter.WritePropertyName("selected_elements");
            jsonWriter.WriteStartArray();
            SelectedElements.ForEach(elementTag => jsonWriter.WriteValue(elementTag.GetHash()));
            jsonWriter.WriteEndArray();

            if (Orientation != 0) {
                jsonWriter.WritePropertyName("orientation");
                jsonWriter.WriteValue((int) Orientation);
            }
            
            if (Flags != 0) {
                jsonWriter.WritePropertyName("flags");
                jsonWriter.WriteValue(Flags);
            }
            
            jsonWriter.WriteEndObject();
        }

        public bool ReadBinary(BinaryReader binaryReader) {
            try {
                Offset = new Vector2I(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                BuildingDef = Assets.GetBuildingDef(binaryReader.ReadString());

                int selectedElementCount = binaryReader.ReadInt32();
                for (int i = 0; i < selectedElementCount; ++i) {
                    Tag elementTag;

                    if (ElementLoader.GetElement(elementTag = new Tag(binaryReader.ReadInt32())) != null) {
                        SelectedElements.Add(elementTag);
                    }
                }

                Orientation = (Orientation) binaryReader.ReadInt32();
                Flags = binaryReader.ReadInt32();
                return true;
            }

            catch (System.Exception) {
                return false;
            }
        }

        public void ReadJSON(JObject rootObject) {
            JToken offsetToken = rootObject.SelectToken("offset");
            JToken buildingDefToken = rootObject.SelectToken("buildingdef");
            JToken selectedElementsToken = rootObject.SelectToken("selected_elements");
            JToken orientationToken = rootObject.SelectToken("orientation");
            JToken flagsToken = rootObject.SelectToken("flags");

            if (offsetToken != null && offsetToken.Type == JTokenType.Object) {
                JToken xToken = offsetToken.SelectToken("x");
                JToken yToken = offsetToken.SelectToken("y");

                if (xToken != null && xToken.Type == JTokenType.Integer || yToken != null && yToken.Type == JTokenType.Integer) {
                    Offset = new Vector2I(xToken == null ? 0 : xToken.Value<int>(), yToken == null ? 0 : yToken.Value<int>());
                }
            }

            if (buildingDefToken != null && buildingDefToken.Type == JTokenType.String) {
                BuildingDef = Assets.GetBuildingDef(buildingDefToken.Value<string>());
            }

            if (selectedElementsToken != null && selectedElementsToken.Type == JTokenType.Array) {
                JArray selectedElementTokens = selectedElementsToken.Value<JArray>();

                if (selectedElementTokens != null) {
                    foreach (JToken selectedElement in selectedElementTokens) {
                        Tag elementTag;

                        if (selectedElement.Type == JTokenType.Integer && ElementLoader.GetElement(elementTag = new Tag(selectedElement.Value<int>())) != null) {
                            SelectedElements.Add(elementTag);
                        }
                    }
                }
            }

            if (orientationToken != null && orientationToken.Type == JTokenType.Integer) {
                Orientation = (Orientation) orientationToken.Value<int>();
            }

            if (flagsToken != null && flagsToken.Type == JTokenType.Integer) {
                Flags = flagsToken.Value<int>();
            }
        }

        public bool Equals(BuildingConfig otherBuildingConfig) {
            return Offset == otherBuildingConfig.Offset && BuildingDef == otherBuildingConfig.BuildingDef;
        }
    }

    public struct CellColorPayload {
        public Color Color { get; private set; }
        public ObjectLayer TileLayer { get; private set; }
        public ObjectLayer ReplacementLayer { get; private set; }

        public CellColorPayload(Color color, ObjectLayer tileLayer, ObjectLayer replacementLayer) {
            Color = color;
            TileLayer = tileLayer;
            ReplacementLayer = replacementLayer;
        }
    }
}