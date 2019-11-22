using Harmony;
using ModKeyBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using STRINGS;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Blueprints {
    /// <summary>
    /// Contains the internal names for each of the strings used by the mod.
    /// 
    /// <para>Class is self documenting.</para>
    /// </summary>
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

    /// <summary>
    /// Contains assets used by the mod.
    /// 
    /// <para>Class is mostly self documenting.</para>
    /// </summary>
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

        public static KeyBinding BLUEPRINTS_KEYBIND_CREATE = new KeyBinding(KeyBindingType.Press, KeyCode.None);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE = new KeyBinding(KeyBindingType.Press, KeyCode.None);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_RELOAD = new KeyBinding(KeyBindingType.Press, KeyCode.LeftShift);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_UP = new KeyBinding(KeyBindingType.Press, KeyCode.UpArrow);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_CYCLEFOLDER_DOWN = new KeyBinding(KeyBindingType.Press, KeyCode.DownArrow);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_LEFT = new KeyBinding(KeyBindingType.Press, KeyCode.LeftArrow);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_CYCLEBLUEPRINT_RIGHT = new KeyBinding(KeyBindingType.Press, KeyCode.RightArrow);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_FOLDER = new KeyBinding(KeyBindingType.Press, KeyCode.Home);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_RENAME = new KeyBinding(KeyBindingType.Press, KeyCode.End);
        public static KeyBinding BLUEPRINTS_KEYBIND_USE_DELETE = new KeyBinding(KeyBindingType.Press, KeyCode.Delete);
        public static KeyBinding BLUEPRINTS_KEYBIND_SNAPSHOT = new KeyBinding(KeyBindingType.Press, KeyCode.None);
        public static KeyBinding BLUEPRINTS_KEYBIND_SNAPSHOT_NEWSNAPSHOT = new KeyBinding(KeyBindingType.Press, KeyCode.Delete);

        public static HashSet<char> BLUEPRINTS_FILE_DISALLOWEDCHARACTERS;

        public static HashSet<char> BLUEPRINTS_PATH_DISALLOWEDCHARACTERS;
        public static string BLUEPRINTS_PATH_CONFIGFOLDER;
        public static string BLUEPRINTS_PATH_CONFIGFILE;
        public static string BLUEPRINTS_PATH_KEYCODESFILE;

        public static bool BLUEPRINTS_CONFIG_REQUIRECONSTRUCTABLE = true;
        public static bool BLUEPRINTS_CONFIG_COMPRESBLUEPRINTS = true;
        public static float BLUEPRINTS_CONFIG_FXTIME = 0.75F; //Time in seconds.

        /// <summary>
        /// Sets up the disallowed file and path characters using information from the operating system.
        /// </summary>
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

    /// <summary>
    /// The global state of the mod.
    /// </summary>
    public static class BlueprintsState {
        //Backing variable for "SelectedBlueprintFolderIndex" below.
        private static int selectedBlueprintFolderIndex;

        /// <summary>
        /// The zero-based index of the selected blueprint folder in the folder list.
        /// </summary>
        public static int SelectedBlueprintFolderIndex {
            get {
                return selectedBlueprintFolderIndex;
            }

            set {
                selectedBlueprintFolderIndex = Mathf.Clamp(value, 0, LoadedBlueprints.Count - 1);
            }
        }

        /// <summary>
        /// The loaded blueprint folders.
        /// </summary>
        public static List<BlueprintFolder> LoadedBlueprints { get; } = new List<BlueprintFolder>();

        /// <summary>
        /// The selected blueprint folder.
        /// </summary>
        public static BlueprintFolder SelectedFolder => LoadedBlueprints[SelectedBlueprintFolderIndex];

        /// <summary>
        /// The selected blueprint.
        /// </summary>
        public static Blueprint SelectedBlueprint => SelectedFolder.SelectedBlueprint;

        /// <summary>
        /// Whether the player has enabled instant build via either debug or sandbox mode.
        /// </summary>
        public static bool InstantBuild => DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild;

        //Current visualized blueprint. Dependant and foundation visuals must be separated because foundations must be updated before dependant visuals to maintain accuracy.
        private static readonly List<IVisual> foundationVisuals = new List<IVisual>();
        private static readonly List<IVisual> dependantVisuals = new List<IVisual>();

        //Any visuals that require cleaning when they are moved around the scene. Contains duplicates from "foundationVisuals" and "dependantVisuals".
        private static readonly List<ICleanableVisual> cleanableVisuals = new List<ICleanableVisual>();

        /// <summary>
        /// Keeps track of all of the colored cells in the game. Also stores all relevant information for undoing the coloring.
        /// </summary>
        public static readonly Dictionary<int, CellColorPayload> ColoredCells = new Dictionary<int, CellColorPayload>();

        /// <summary>
        /// Returns whether the user has loaded any blueprints.
        /// </summary>
        /// <returns>True if there are any loaded blueprints, false otherwise</returns>
        public static bool HasBlueprints() {
            //Return false if there are no folders.
            if (LoadedBlueprints.Count == 0) {
                return false;
            }

            //Check if there are any non-empty blueprint folders.
            foreach (BlueprintFolder blueprintFolder in LoadedBlueprints) {
                if (blueprintFolder.BlueprintCount > 0) {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Creates a blueprint by sampling an area.
        /// </summary>
        /// <param name="topLeft">The top left corner of the area to sample in grid coordinates</param>
        /// <param name="bottomRight">The bottom right of thr area to sample in grid coordinates</param>
        /// <param name="originTool">The tool used to do the sampling, null if no tool</param>
        /// <returns>The blueprint created by sampling the area</returns>
        public static Blueprint CreateBlueprint(Vector2I topLeft, Vector2I bottomRight, FilteredDragTool originTool = null) {
            Blueprint blueprint = new Blueprint("unnamed", "");
            int blueprintHeight = (topLeft.y - bottomRight.y);

            for (int x = topLeft.x; x <= bottomRight.x; ++x) {
                for (int y = bottomRight.y; y <= topLeft.y; ++y) {
                    int cell = Grid.XYToCell(x, y);

                    //Only sample cells the player can actually see.
                    if (Grid.IsVisible(cell)) {
                        bool emptyCell = true;

                        //Iterate through each of the layers contained within this cell.
                        for (int layer = 0; layer < Grid.ObjectLayers.Length; ++layer) {
                            GameObject gameObject = Grid.Objects[cell, layer];            
                            
                            if (gameObject != null) {
                                //Whether or not the selected tool is configured to sample this building.
                                bool toolAllowed = originTool == null || originTool.IsActiveLayer(originTool.GetFilterLayerFromGameObject(gameObject));

                                Building building;

                                //Proceed only if the object is a building, it is buildable and if the tool is configured to sample it.
                                if ((building = gameObject.GetComponent<Building>()) != null && building.Def.IsBuildable() && toolAllowed) {
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

                            //Attempt to add dig commands to cells that contained no buildings when sampled.
                            if (emptyCell) {
                                //Whether or not the sampled object is a dig placer
                                bool isDigPlacer = Grid.Objects[cell, 7] != null && Grid.Objects[cell, 7].name == "DigPlacer";

                                //Proceed only if the cell is empty (would need to be dug to replicate) or if the cell contains a queued dig command.
                                if (!Grid.IsSolidCell(cell) || isDigPlacer) {
                                    Vector2I digLocation = new Vector2I(x - topLeft.x, blueprintHeight - (topLeft.y - y));

                                    //Do not add duplicate dig locations. Probably not actually necessary.
                                    if (!blueprint.DigLocations.Contains(digLocation)) {
                                        blueprint.DigLocations.Add(digLocation);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return blueprint;
        }

        /// <summary>
        /// Visualize a blueprint for placement.
        /// </summary>
        /// <param name="bottomLeft">The bottom left coordinate of the area to place the blueprint visualizer</param>
        /// <param name="blueprint">The blueprint to visualize</param>
        public static void VisualizeBlueprint(Vector2I bottomLeft, Blueprint blueprint) {
            if (blueprint == null) {
                return;
            }

            int errors = 0;

            //Clear existing visualizers
            ClearVisuals();
 
            foreach (BuildingConfig buildingConfig in blueprint.BuildingConfiguration) {
                //Skip if there are any errors.
                if (buildingConfig.BuildingDef == null || buildingConfig.SelectedElements.Count == 0) {
                    ++errors;
                    continue;
                }

                if (buildingConfig.BuildingDef.BuildingPreview != null) {
                    int cell = Grid.XYToCell(bottomLeft.x + buildingConfig.Offset.x, bottomLeft.y + buildingConfig.Offset.y);

                    if (buildingConfig.BuildingDef.IsTilePiece) {
                        //Utility interacting buildings (such as pipes, wires and c
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
                foundationVisuals.Add(new DigVisual(Grid.XYToCell(bottomLeft.x + digLocation.x, bottomLeft.y + digLocation.y), digLocation));
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