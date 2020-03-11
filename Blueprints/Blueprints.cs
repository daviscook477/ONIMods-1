﻿using Harmony;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PeterHan.PLib;
using STRINGS;
using System;
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
        public static BlueprintsOptions Options { get; set; } = new BlueprintsOptions();

        public static string BLUEPRINTS_CREATE_TOOLNAME = "CreateBlueprintTool";
        public static string BLUEPRINTS_CREATE_ICON_NAME = "BLUEPRINTS.TOOL.CREATE_BLUEPRINT.ICON";
        public static Sprite BLUEPRINTS_CREATE_ICON_SPRITE;
        public static Sprite BLUEPRINTS_CREATE_VISUALIZER_SPRITE;
        public static PAction BLUEPRINTS_CREATE_OPENTOOL;
        public static ToolMenu.ToolCollection BLUEPRINTS_CREATE_TOOLCOLLECTION;

        public static string BLUEPRINTS_USE_TOOLNAME = "UseBlueprintTool";
        public static string BLUEPRINTS_USE_ICON_NAME = "BLUEPRINTS.TOOL.USE_BLUEPRINT.ICON";
        public static Sprite BLUEPRINTS_USE_ICON_SPRITE;
        public static Sprite BLUEPRINTS_USE_VISUALIZER_SPRITE;
        public static PAction BLUEPRINTS_USE_OPENTOOL;
        public static PAction BLUEPRINTS_USE_CREATEFOLDER;
        public static PAction BLUEPRINTS_USE_RENAME;
        public static PAction BLUEPRINTS_USE_CYCLEBLUEPRINTS_PREVIOUS;
        public static PAction BLUEPRINTS_USE_CYCLEBLUEPRINTS_NEXT;
        public static PAction BLUEPRINTS_USE_CYCLEFOLDERS_PREVIOUS;
        public static PAction BLUEPRINTS_USE_CYCLEFOLDERS_NEXT;
        public static ToolMenu.ToolCollection BLUEPRINTS_USE_TOOLCOLLECTION;

        public static string BLUEPRINTS_SNAPSHOT_TOOLNAME = "SnapshotTool";
        public static string BLUEPRINTS_SNAPSHOT_ICON_NAME = "BLUEPRINTS.TOOL.SNAPSHOT.ICON";
        public static Sprite BLUEPRINTS_SNAPSHOT_ICON_SPRITE;
        public static Sprite BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE;
        public static PAction BLUEPRINTS_SNAPSHOT_OPENTOOL;
        public static ToolMenu.ToolCollection BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION;

        public static PAction BLUEPRINTS_MULTI_DELETE;

        public static Color BLUEPRINTS_COLOR_VALIDPLACEMENT = Color.white;
        public static Color BLUEPRINTS_COLOR_INVALIDPLACEMENT = Color.red;
        public static Color BLUEPRINTS_COLOR_NOTECH = new Color32(30, 144, 255, 255);
        public static Color BLUEPRINTS_COLOR_BLUEPRINT_DRAG = new Color32(0, 119, 145, 255);

        public static HashSet<char> BLUEPRINTS_FILE_DISALLOWEDCHARACTERS;
        public static HashSet<char> BLUEPRINTS_PATH_DISALLOWEDCHARACTERS;

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
                                    //GameObject settingsSource = CopyUtilities.CopyGameObjectWithSerialization(gameObject);

                                    BuildingConfig buildingConfig = new BuildingConfig {
                                        Offset = new Vector2I(centre.x - topLeft.x, blueprintHeight - (topLeft.y - centre.y)),
                                        BuildingDef = building.Def,
                                        Orientation = building.Orientation,
                                        SettingsSource = CopyUtilities.SerializeBuilding(gameObject)
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

            foundationVisuals.ForEach(foundationVisual => UnityEngine.Object.DestroyImmediate(foundationVisual.Visualizer));
            foundationVisuals.Clear();

            dependantVisuals.ForEach(dependantVisual => UnityEngine.Object.DestroyImmediate(dependantVisual.Visualizer));
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