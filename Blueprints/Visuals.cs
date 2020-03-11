﻿using UnityEngine;

namespace Blueprints {
    public static class VisualsUtilities {
        public static void SetVisualizerColor(int cell, Color color, GameObject visualizer, BuildingConfig buildingConfig) {
            if (buildingConfig.BuildingDef.isKAnimTile && buildingConfig.BuildingDef.BlockTileAtlas != null && !BlueprintsState.ColoredCells.ContainsKey(cell)) {
                BlueprintsState.ColoredCells.Add(cell, new CellColorPayload(color, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer));
                TileVisualizer.RefreshCell(cell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
            }

            if (visualizer.GetComponent<KBatchedAnimController>() != null) {
                visualizer.GetComponent<KBatchedAnimController>().TintColour = color;
            }
        }
    }

    public interface IVisual {
        GameObject Visualizer { get; }
        Vector2I Offset { get; }

        bool IsPlaceable(int cell);
        void MoveVisualizer(int cell);
        bool TryUse(int cell);
    }

    public interface ICleanableVisual : IVisual {
        int DirtyCell { get; }

        void Clean();
    }

    public sealed class BuildingVisual : IVisual {
        public GameObject Visualizer { get; private set; }
        public Vector2I Offset { get; private set; }

        private readonly BuildingConfig buildingConfig;
        private int cell;

        public BuildingVisual(BuildingConfig buildingConfig, int cell) {
            Offset = buildingConfig.Offset;
            this.buildingConfig = buildingConfig;
            this.cell = cell;

            Vector3 positionCBC = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
            Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCBC, Grid.SceneLayer.Ore, "BlueprintModBuildingVisualizer", LayerMask.NameToLayer("Place"));
            Visualizer.transform.SetPosition(positionCBC);
            Visualizer.SetActive(true);

            if (Visualizer.GetComponent<Rotatable>() != null) {
                Visualizer.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
            }

            KBatchedAnimController batchedAnimController = Visualizer.GetComponent<KBatchedAnimController>();
            if (batchedAnimController != null) {
                batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
                batchedAnimController.isMovable = true;
                batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset() + buildingConfig.BuildingDef.placementPivot;
                batchedAnimController.TintColour = GetVisualizerColor(cell);

                batchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
            }

            else {
                Visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
            }
        }

        public bool IsPlaceable(int cell) {
            return ValidCell(cell) && HasTech();
        }

        public void MoveVisualizer(int cell) {
            if (cell != this.cell) {
                Visualizer.transform.SetPosition(Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer));

                if (Visualizer.GetComponent<KBatchedAnimController>() != null) {
                    Visualizer.GetComponent<KBatchedAnimController>().TintColour = GetVisualizerColor(cell);
                }

                this.cell = cell;
            }
        }

        public bool TryUse(int cell) {
            if (BlueprintsState.InstantBuild) {
                if (ValidCell(cell)) {
                    Vector3 positionCBC = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
                    GameObject building = buildingConfig.BuildingDef.Create(positionCBC, null, buildingConfig.SelectedElements, buildingConfig.BuildingDef.CraftRecipe, 293.15F, buildingConfig.BuildingDef.BuildingComplete);
                    if (building == null) {
                        return false;
                    }

                    buildingConfig.BuildingDef.MarkArea(cell, buildingConfig.Orientation, buildingConfig.BuildingDef.ObjectLayer, building);

                    if (building.GetComponent<Deconstructable>() != null) {
                        building.GetComponent<Deconstructable>().constructionElements = buildingConfig.SelectedElements.ToArray();
                    }

                    if (building.GetComponent<Rotatable>() != null) {
                        building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                    }

                    if (Visualizer.GetComponent<KBatchedAnimController>() != null) {
                        Visualizer.GetComponent<KBatchedAnimController>().TintColour = BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
                    }

                    building.SetActive(true);
                    CopyUtilities.CopySettingsWithDelegateAndDestroy(building, CopyUtilities.DeserializeGameObject(buildingConfig.SettingsSource));
                    return true;
                }
            }

            else if (IsPlaceable(cell)) {
                Vector3 positionCBC = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
                GameObject building = buildingConfig.BuildingDef.Instantiate(positionCBC, buildingConfig.Orientation, buildingConfig.SelectedElements);
                building.AddComponent<ConstructableSettings>().settingsSource = CopyUtilities.DeserializeGameObject(buildingConfig.SettingsSource);
                if (building == null) {
                    return false;
                }

                if (building.GetComponent<Rotatable>() != null) {
                    building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                }

                if (Visualizer.GetComponent<KBatchedAnimController>() != null) {
                    Visualizer.GetComponent<KBatchedAnimController>().TintColour = BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
                }

                if (ToolMenu.Instance != null) {
                    building.FindOrAddComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
                }

                building.SetActive(true);
                return true;
            }

            return false;
        }

        public bool ValidCell(int cell) {
            return Grid.IsValidCell(cell) && Grid.IsVisible(cell) && buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cell, buildingConfig.Orientation, out string _);
        }

        public bool HasTech() {
            return (BlueprintsState.InstantBuild || !BlueprintsAssets.Options.RequireConstructable || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID));
        }

        public Color GetVisualizerColor(int cell) {
            if (!ValidCell(cell)) {
                return BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
            }

            else if (!HasTech()) {
                return BlueprintsAssets.BLUEPRINTS_COLOR_NOTECH;
            }

            else {
                return BlueprintsAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
            }
        }
    }

    public sealed class TileVisual : ICleanableVisual {
        public GameObject Visualizer { get; private set; }
        public Vector2I Offset { get; private set; }
        public int DirtyCell { get; private set; } = -1;

        private readonly BuildingConfig buildingConfig;
        private readonly bool hasReplacementLayer;

        public TileVisual(BuildingConfig buildingConfig, int cell) {
            Offset = buildingConfig.Offset;
            this.buildingConfig = buildingConfig;
            hasReplacementLayer = buildingConfig.BuildingDef.ReplacementLayer != ObjectLayer.NumLayers;

            Vector3 positionCBC = Grid.CellToPosCBC(cell, Grid.SceneLayer.Building);
            Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCBC, Grid.SceneLayer.Ore, "BlueprintModTileVisualizer", LayerMask.NameToLayer("Place"));
            Visualizer.transform.SetPosition(positionCBC);
            Visualizer.SetActive(true);

            if (Visualizer.GetComponent<Rotatable>() != null) {
                Visualizer.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
            }

            KBatchedAnimController batchedAnimController = Visualizer.GetComponent<KBatchedAnimController>();
            if (batchedAnimController != null) {
                batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
                batchedAnimController.isMovable = true;
                batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset() + buildingConfig.BuildingDef.placementPivot;
            }

            VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
            DirtyCell = cell;
            UpdateGrid(cell);
        }

        public bool IsPlaceable(int cell) {
            return ValidCell(cell) && HasTech();
        }

        public void MoveVisualizer(int cell) {
            Visualizer.transform.SetPosition(Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer));
            VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
            UpdateGrid(cell);
        }

        public bool TryUse(int cell) {
            Clean();

            if (BlueprintsState.InstantBuild) {
                if (ValidCell(cell)) {
                    Vector3 positionCBC = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
                    GameObject building = buildingConfig.BuildingDef.Create(positionCBC, null, buildingConfig.SelectedElements, buildingConfig.BuildingDef.CraftRecipe, 293.15F, buildingConfig.BuildingDef.BuildingComplete);
                    if (building == null) {
                        return false;
                    }

                    buildingConfig.BuildingDef.MarkArea(cell, buildingConfig.Orientation, buildingConfig.BuildingDef.TileLayer, building);
                    buildingConfig.BuildingDef.RunOnArea(cell, buildingConfig.Orientation, cell0 => TileVisualizer.RefreshCell(cell0, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer));

                    if (building.GetComponent<Deconstructable>() != null) {
                        building.GetComponent<Deconstructable>().constructionElements = buildingConfig.SelectedElements.ToArray();
                    }

                    if (building.GetComponent<Rotatable>() != null) {
                        building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                    }

                    VisualsUtilities.SetVisualizerColor(cell, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);
                    return true;
                }
            }

            else if (IsPlaceable(cell)) {
                Vector3 positionCBC = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
                GameObject building = buildingConfig.BuildingDef.Instantiate(positionCBC, buildingConfig.Orientation, buildingConfig.SelectedElements);
                if (building == null) {
                    return false;
                }

                if (building.GetComponent<Rotatable>() != null) {
                    building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                }

                if (Visualizer.GetComponent<KBatchedAnimController>() != null) {
                    Visualizer.GetComponent<KBatchedAnimController>().TintColour = BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
                }

                VisualsUtilities.SetVisualizerColor(cell, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);
                return true;
            }

            return false;
        }

        public void Clean() {
            if (DirtyCell != -1 && Grid.IsValidBuildingCell(DirtyCell)) {
                if (Grid.Objects[DirtyCell, (int) buildingConfig.BuildingDef.TileLayer] == Visualizer) {
                    Grid.Objects[DirtyCell, (int) buildingConfig.BuildingDef.TileLayer] = null;
                }

                if (hasReplacementLayer && Grid.Objects[DirtyCell, (int) buildingConfig.BuildingDef.ReplacementLayer] == Visualizer) {
                    Grid.Objects[DirtyCell, (int) buildingConfig.BuildingDef.ReplacementLayer] = null;
                }

                if (buildingConfig.BuildingDef.isKAnimTile) {
                    GameObject tileLayerObject = Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.TileLayer];
                    if (tileLayerObject == null || tileLayerObject.GetComponent<Constructable>() == null) {
                        World.Instance.blockTileRenderer.RemoveBlock(buildingConfig.BuildingDef, false, SimHashes.Void, DirtyCell);
                        TileVisualizer.RefreshCell(DirtyCell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
                    }

                    GameObject replacementLayerObject = hasReplacementLayer ? null : Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer];
                    if (replacementLayerObject == null || replacementLayerObject == Visualizer) {
                        World.Instance.blockTileRenderer.RemoveBlock(buildingConfig.BuildingDef, true, SimHashes.Void, DirtyCell);
                        TileVisualizer.RefreshCell(DirtyCell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
                    }
                }
            }

            DirtyCell = -1;
        }

        public bool ValidCell(int cell) {
            return Grid.IsValidCell(cell) && Grid.IsVisible(cell) && !HasTile(cell) && buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cell, buildingConfig.Orientation, out string _);
        }

        public bool HasTech() {
            return BlueprintsState.InstantBuild || !BlueprintsAssets.Options.RequireConstructable || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID);
        }

        public bool HasTile(int cell) {
            return Grid.Objects[cell, (int) buildingConfig.BuildingDef.TileLayer] != null;
        }

        public Color GetVisualizerColor(int cell) {
            if (!ValidCell(cell)) {
                return BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
            }

            else if (!HasTech()) {
                return BlueprintsAssets.BLUEPRINTS_COLOR_NOTECH;
            }

            else {
                return BlueprintsAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
            }
        }

        private bool CanReplace(int cell) {
            CellOffset[] placementOffsets = buildingConfig.BuildingDef.PlacementOffsets;

            for (int index = 0; index < placementOffsets.Length; ++index) {
                CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(placementOffsets[index], buildingConfig.Orientation);
                int offsetCell = Grid.OffsetCell(cell, rotatedCellOffset);

                if (!Grid.IsValidBuildingCell(cell) || Grid.Objects[offsetCell, (int) buildingConfig.BuildingDef.ObjectLayer] == null || Grid.Objects[offsetCell, (int) buildingConfig.BuildingDef.ReplacementLayer] != null) {
                    return false;
                }
            }

            return true;
        }

        private void UpdateGrid(int cell) {
            Clean();

            if (Grid.IsValidBuildingCell(cell)) {
                bool visualizerSeated = false;

                if (Grid.Objects[cell, (int) buildingConfig.BuildingDef.TileLayer] == null) {
                    Grid.Objects[cell, (int) buildingConfig.BuildingDef.TileLayer] = Visualizer;
                    visualizerSeated = true;
                }

                if (buildingConfig.BuildingDef.isKAnimTile) {
                    GameObject tileLayerObject = Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.TileLayer];
                    GameObject replacementLayerObject = hasReplacementLayer ? Grid.Objects[DirtyCell, (int)buildingConfig.BuildingDef.ReplacementLayer] : null;

                    if (tileLayerObject == null || tileLayerObject.GetComponent<Constructable>() == null && replacementLayerObject == null) {
                        if (buildingConfig.BuildingDef.BlockTileAtlas != null) {
                            if (!ValidCell(cell)) {
                                VisualsUtilities.SetVisualizerColor(cell, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);
                            }

                            bool replacing = hasReplacementLayer && CanReplace(cell);
                            World.Instance.blockTileRenderer.AddBlock(LayerMask.NameToLayer("Overlay"), buildingConfig.BuildingDef, replacing, SimHashes.Void, cell);
                            if (replacing && !visualizerSeated && Grid.Objects[DirtyCell, (int) buildingConfig.BuildingDef.ReplacementLayer] == null) {
                                Grid.Objects[cell, (int) buildingConfig.BuildingDef.ReplacementLayer] = Visualizer;
                            }
                        }
                    }

                    TileVisualizer.RefreshCell(cell, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer);
                }

                DirtyCell = cell;
            }
        }
    }

    public sealed class UtilityVisual : IVisual {
        public GameObject Visualizer { get; private set; }
        public Vector2I Offset { get; private set; }

        private readonly BuildingConfig buildingConfig;
        private int cell;

        public UtilityVisual(BuildingConfig buildingConfig, int cell) {
            Offset = buildingConfig.Offset;
            this.buildingConfig = buildingConfig;
            this.cell = cell;

            Vector3 positionCBC = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
            Visualizer = GameUtil.KInstantiate(buildingConfig.BuildingDef.BuildingPreview, positionCBC, Grid.SceneLayer.Ore, "BlueprintModUtilityVisualizer", LayerMask.NameToLayer("Place"));
            Visualizer.transform.SetPosition(positionCBC);
            Visualizer.SetActive(true);

            if (Visualizer.GetComponent<Rotatable>() != null) {
                Visualizer.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
            }

            KBatchedAnimController batchedAnimController = Visualizer.GetComponent<KBatchedAnimController>();
            if (batchedAnimController != null) {
                IUtilityNetworkMgr utilityNetworkManager = buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager();

                if (utilityNetworkManager != null) {
                    string animation = utilityNetworkManager.GetVisualizerString((UtilityConnections) buildingConfig.Flags) + "_place";

                    if (batchedAnimController.HasAnimation(animation)) {
                        batchedAnimController.Play(animation);
                    }
                }

                batchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.Always;
                batchedAnimController.isMovable = true;
                batchedAnimController.Offset = buildingConfig.BuildingDef.GetVisualizerOffset() + buildingConfig.BuildingDef.placementPivot;
                batchedAnimController.TintColour = GetVisualizerColor(cell);

                batchedAnimController.SetLayer(LayerMask.NameToLayer("Place"));
            }

            else {
                Visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
            }

            VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);
        }

        public bool IsPlaceable(int cell) {
            return ValidCell(cell) && HasTech();
        }

        public void MoveVisualizer(int cell) {
            if (cell != this.cell) {
                Visualizer.transform.SetPosition(Grid.CellToPosCBC(cell, Grid.SceneLayer.Building));
                VisualsUtilities.SetVisualizerColor(cell, GetVisualizerColor(cell), Visualizer, buildingConfig);

                this.cell = cell;
            }
        }

        public bool TryUse(int cell) {
            if (BlueprintsState.InstantBuild) {
                if (ValidCell(cell)) {
                    Vector3 positionCBC = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
                    GameObject building = buildingConfig.BuildingDef.Create(positionCBC, null, buildingConfig.SelectedElements, buildingConfig.BuildingDef.CraftRecipe, 293.15F, buildingConfig.BuildingDef.BuildingComplete);
                    if (building == null) {
                        return false;
                    }

                    buildingConfig.BuildingDef.MarkArea(cell, buildingConfig.Orientation, buildingConfig.BuildingDef.TileLayer, building);
                    buildingConfig.BuildingDef.RunOnArea(cell, buildingConfig.Orientation, cell0 => TileVisualizer.RefreshCell(cell0, buildingConfig.BuildingDef.TileLayer, buildingConfig.BuildingDef.ReplacementLayer));

                    if (building.GetComponent<Rotatable>() != null) {
                        building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                    }

                    if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && building.GetComponent<KAnimGraphTileVisualizer>() != null) {
                        building.GetComponent<KAnimGraphTileVisualizer>().UpdateConnections((UtilityConnections) buildingConfig.Flags);
                    }

                    VisualsUtilities.SetVisualizerColor(cell, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);
                    return true;
                }
            }

            else if (IsPlaceable(cell)) {
                Vector3 positionCBC = Grid.CellToPosCBC(cell, buildingConfig.BuildingDef.SceneLayer);
                GameObject building = buildingConfig.BuildingDef.Instantiate(positionCBC, buildingConfig.Orientation, buildingConfig.SelectedElements);

                if (building == null) {
                    return false;
                }

                if (building.GetComponent<Rotatable>() != null) {
                    building.GetComponent<Rotatable>().SetOrientation(buildingConfig.Orientation);
                }

                if (buildingConfig.BuildingDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && building.GetComponent<KAnimGraphTileVisualizer>() != null) {
                    building.GetComponent<KAnimGraphTileVisualizer>().UpdateConnections((UtilityConnections) buildingConfig.Flags);
                }

                if (ToolMenu.Instance != null) {
                    building.FindOrAddComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
                }

                VisualsUtilities.SetVisualizerColor(cell, BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT, Visualizer, buildingConfig);
                return true;
            }

            return false;
        }

        public bool ValidCell(int cell) {
            return Grid.IsValidCell(cell) && Grid.IsVisible(cell) && !HasUtility(cell) && buildingConfig.BuildingDef.IsValidPlaceLocation(Visualizer, cell, buildingConfig.Orientation, out string _);
        }

        public bool HasTech() {
            return (BlueprintsState.InstantBuild || !BlueprintsAssets.Options.RequireConstructable || Db.Get().TechItems.IsTechItemComplete(buildingConfig.BuildingDef.PrefabID));
        }

        public bool HasUtility(int cell) {
            return Grid.Objects[cell, (int) buildingConfig.BuildingDef.TileLayer] != null;
        }

        public Color GetVisualizerColor(int cell) {
            if (!ValidCell(cell)) {
                return BlueprintsAssets.BLUEPRINTS_COLOR_INVALIDPLACEMENT;
            }

            else if (!HasTech()) {
                return BlueprintsAssets.BLUEPRINTS_COLOR_NOTECH;
            }

            else {
                return BlueprintsAssets.BLUEPRINTS_COLOR_VALIDPLACEMENT;
            }
        }
    }

    public sealed class DigVisual : IVisual {
        public GameObject Visualizer { get; private set; }
        public Vector2I Offset { get; private set; }

        public DigVisual(int cell, Vector2I offset) {
            Visualizer = GameUtil.KInstantiate(DigTool.Instance.visualizer, Grid.CellToPosCBC(cell, DigTool.Instance.visualizerLayer), DigTool.Instance.visualizerLayer, "BlueprintModDigVisualizer");
            Visualizer.SetActive(IsPlaceable(cell));
            Offset = offset;
        }

        public bool IsPlaceable(int cell) {
            return Grid.IsValidCell(cell) && Grid.IsVisible(cell) && Grid.Solid[cell] && !Grid.Foundation[cell] && Grid.Objects[cell, 7] == null;
        }

        public void MoveVisualizer(int cell) {
            Visualizer.transform.SetPosition(Grid.CellToPosCBC(cell, DigTool.Instance.visualizerLayer));
            Visualizer.SetActive(IsPlaceable(cell));
        }

        public bool TryUse(int cell) {
            if (IsPlaceable(cell)) {
                if (BlueprintsState.InstantBuild) {
                    WorldDamage.Instance.DestroyCell(cell);
                }

                else {
                    GameObject digVisualizer = Util.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")));
                    digVisualizer.transform.SetPosition(Grid.CellToPosCBC(cell, DigTool.Instance.visualizerLayer) - new Vector3(0F, 0F, 0.15F));
                    digVisualizer.GetComponent<Prioritizable>().SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
                    digVisualizer.SetActive(true);

                    Grid.Objects[cell, 7] = digVisualizer;
                    Visualizer.SetActive(false);
                }

                return true;
            }

            return false;
        }
    }
}
