using System.Reflection;

using Harmony;

using UnityEngine;

namespace WireCutter {
    public sealed class WireCutterTool : DragTool {
        private static readonly UtilityConnections[] connections = {
            UtilityConnections.Left,
            UtilityConnections.Right,
            UtilityConnections.Up,
            UtilityConnections.Down
        };

        public static WireCutterTool Instance { get; private set; }

        public WireCutterTool() {
            Instance = this;
        }

        public static void DestroyInstance() {
            Instance = null;
        }

        protected override void OnPrefabInit() {
            base.OnPrefabInit();

            visualizer = new GameObject("WireCutterVisualizer");
            visualizer.SetActive(false);

            GameObject offsetObject = new GameObject();
            SpriteRenderer spriteRenderer = offsetObject.AddComponent<SpriteRenderer>();
            spriteRenderer.color = WireCutterAssets.WIRECUTTER_COLOR_DRAG; ;
            spriteRenderer.sprite = WireCutterAssets.WIRECUTTER_VISUALIZER_SPRITE;

            offsetObject.transform.SetParent(visualizer.transform);
            offsetObject.transform.localPosition = new Vector3(0, Grid.HalfCellSizeInMeters);
            offsetObject.transform.localScale = new Vector3(
                Grid.CellSizeInMeters / (spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit),
                Grid.CellSizeInMeters / (spriteRenderer.sprite.texture.height / spriteRenderer.sprite.pixelsPerUnit)
            );

            offsetObject.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
            visualizer.transform.SetParent(transform);

            FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");
            FieldInfo areaVisualizerSpriteRendererField = AccessTools.Field(typeof(DragTool), "areaVisualizerSpriteRenderer");

            GameObject areaVisualizer = Util.KInstantiate((GameObject) AccessTools.Field(typeof(DeconstructTool), "areaVisualizer").GetValue(DeconstructTool.Instance));
            areaVisualizer.SetActive(false);

            areaVisualizer.name = "WireCutterAreaVisualizer";
            areaVisualizerSpriteRendererField.SetValue(this, areaVisualizer.GetComponent<SpriteRenderer>());
            areaVisualizer.transform.SetParent(transform);
            areaVisualizer.GetComponent<SpriteRenderer>().color = WireCutterAssets.WIRECUTTER_COLOR_DRAG;
            areaVisualizer.GetComponent<SpriteRenderer>().material.color = WireCutterAssets.WIRECUTTER_COLOR_DRAG;

            areaVisualizerField.SetValue(this, areaVisualizer);

            gameObject.AddComponent<WireCutterToolHoverCard>();
        }

        protected override void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp) {
            base.OnDragComplete(cursorDown, cursorUp);

            if (hasFocus) {
                Grid.PosToXY(cursorDown, out int x0, out int y0);
                Grid.PosToXY(cursorUp, out int x1, out int y1);

                if (x0 > x1) {
                    Util.Swap(ref x0, ref x1);
                }

                if (y0 < y1) {
                    Util.Swap(ref y0, ref y1);
                }

                for (int x = x0; x <= x1; ++x) {
                    for (int y = y1; y <= y0; ++y) {
                        int cell = Grid.XYToCell(x, y);

                        if (Grid.IsVisible(cell)) {
                            for (int layer = 0; layer < Grid.ObjectLayers.Length; ++layer) {
                                GameObject gameObject = Grid.Objects[cell, layer];                         
                                Building building;

                                if (gameObject != null && (building = gameObject.GetComponent<Building>()) != null ) {
                                    IHaveUtilityNetworkMgr utilityNetworkManager;

                                    if ((utilityNetworkManager = building.Def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>()) != null) {
                                        UtilityConnections connectionsToRemove = 0;
                                        UtilityConnections buildingConnections = utilityNetworkManager.GetNetworkManager().GetConnections(cell, false);

                                        foreach (UtilityConnections utilityConnection in connections) {
                                            if((buildingConnections & utilityConnection) != utilityConnection) {
                                                continue;
                                            }

                                            int offsetCell = Grid.OffsetCell(cell, Utilities.ConnectionsToOffset(utilityConnection));
                                            if (Grid.IsValidBuildingCell(offsetCell)) {
                                                Grid.CellToXY(offsetCell, out int x2, out int y2);

                                                if (x2 >= x0 && x2 <= x1 && y2 >= y1 && y2 <= y0) {
                                                    GameObject otherGameObject = Grid.Objects[offsetCell, layer];
                                                    Building otherBuiilding;

                                                    if (otherGameObject != null && (otherBuiilding = otherGameObject.GetComponent<Building>()) != null && otherBuiilding.Def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null) {
                                                        connectionsToRemove |= utilityConnection;
                                                    }
                                                }
                                            }
                                        }

                                        if(connectionsToRemove != 0) {
                                            if (building.GetComponent<KAnimGraphTileVisualizer>() != null) {
                                                building.GetComponent<KAnimGraphTileVisualizer>().UpdateConnections(buildingConnections & ~connectionsToRemove);
                                                building.GetComponent<KAnimGraphTileVisualizer>().Refresh();
                                            }

                                            TileVisualizer.RefreshCell(cell, building.Def.TileLayer, building.Def.ReplacementLayer);
                                            utilityNetworkManager.GetNetworkManager().ForceRebuildNetworks();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
