using System.Collections.Generic;
using System.IO;

using Harmony;

using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blueprints {
    public static class BlueprintsAssets {
        public static string                  BLUEPRINTS_CREATE_NAME = "New Blueprint";
        public static string                  BLUEPRINTS_CREATE_TOOLNAME = "CreateBlueprintTool";
        public static string                  BLUEPRINTS_CREATE_TOOLTIP = "Create blueprint " + Utilities.GetKeyCodeString(KeyCode.None);
        public static string                  BLUEPRINTS_CREATE_ICON_NAME = "BLUEPRINTS.TOOL.CREATE_BLUEPRINT.ICON";
        public static Sprite                  BLUEPRINTS_CREATE_ICON_SPRITE;
        public static Sprite                  BLUEPRINTS_CREATE_VISUALIZER_SPRITE;
        public static ToolMenu.ToolCollection BLUEPRINTS_CREATE_TOOLCOLLECTION;

        public static string                  BLUEPRINTS_USE_NAME = "Use Blueprint";
        public static string                  BLUEPRINTS_USE_TOOLNAME = "UseBlueprintTool";
        public static string                  BLUEPRINTS_USE_TOOLTIP = "Use blueprint " + Utilities.GetKeyCodeString(KeyCode.None) + "\n\nWhen selecting the tool hold " + Utilities.GetKeyCodeString(KeyCode.LeftShift) + " to reload blueprints";
        public static string                  BLUEPRINTS_USE_ICON_NAME = "BLUEPRINTS.TOOL.USE_BLUEPRINT.ICON";
        public static Sprite                  BLUEPRINTS_USE_ICON_SPRITE;
        public static Sprite                  BLUEPRINTS_USE_VISUALIZER_SPRITE;
        public static ToolMenu.ToolCollection BLUEPRINTS_USE_TOOLCOLLECTION;

        public static string                  BLUEPRINTS_SNAPSHOT_NAME = "Take Snapshot";
        public static string                  BLUEPRINTS_SNAPSHOT_TOOLNAME = "SnapshotTool";
        public static string                  BLUEPRINTS_SNAPSHOT_TOOLTIP = "Take snapshot " + Utilities.GetKeyCodeString(KeyCode.None) + "\n\nCreate a blueprint and quickly place it elsewhere while not cluttering your blueprint collection! \nSnapshots do not persist between games.";
        public static string                  BLUEPRINTS_SNAPSHOT_ICON_NAME = "BLUEPRINTS.TOOL.SNAPSHOT.ICON";
        public static Sprite                  BLUEPRINTS_SNAPSHOT_ICON_SPRITE;
        public static Sprite                  BLUEPRINTS_SNAPSHOT_VISUALIZER_SPRITE;
        public static ToolMenu.ToolCollection BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION;

        public static Color                   BLUEPRINTS_COLOR_VALIDPLACEMENT = Color.white;
        public static Color                   BLUEPRINTS_COLOR_INVALIDPLACEMENT = Color.red;
        public static Color                   BLUEPRINTS_COLOR_NOTECH = new Color32(30, 144, 255, 255);
        public static Color                   BLUEPRINTS_COLOR_BLUEPRINT_DRAG = new Color32(0, 119, 145, 255);

        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_CREATETOOL = KeyCode.None;
        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_USETOOL = KeyCode.None;
        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_USETOOL_RELOAD = KeyCode.LeftShift;
        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_USETOOL_CYCLELEFT = KeyCode.LeftArrow;
        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_USETOOL_CYCLERIGHT = KeyCode.RightArrow;
        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_USETOOL_RENAME = KeyCode.End;
        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_USETOOL_DELETE = KeyCode.Delete;
        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_SNAPSHOTTOOL = KeyCode.None;
        public static KeyCode                 BLUEPRINTS_INPUT_KEYBIND_SNAPSHOTTOOL_DELETE = KeyCode.Delete;

        public static string                  BLUEPRINTS_PATH_CONFIGFOLDER;
        public static string                  BLUEPRINTS_PATH_CONFIGFILE;
        public static string                  BLUEPRINTS_PATH_KEYCODESFILE;

        public static string                  BLUEPRINTS_STRING_CYCLEBLUEPRINTS = "Use " + Utilities.GetKeyCodeString(KeyCode.LeftArrow) + " and " + Utilities.GetKeyCodeString(KeyCode.RightArrow) + " to cycle between blueprints";
        public static string                  BLUEPRINTS_STRING_RENAMEBLUEPRINT = "Press " + Utilities.GetKeyCodeString(KeyCode.End) + " to rename blueprint";
        public static string                  BLUEPRINTS_STRING_DELETEBLUEPRINT = "Press " + Utilities.GetKeyCodeString(KeyCode.Delete) + " to delete selected blueprint";
        public static string                  BLUEPRINTS_STRING_DELETESNAPSHOT = "Press " + Utilities.GetKeyCodeString(KeyCode.Delete) + " to take new snapshot";

        public static bool                    BLUEPRINTS_BOOL_REQUIRECONSTRUCTABLE = true;
        public static bool                    BLUEPRINTS_BOOL_COMPRESBLUEPRINTS = true;

        public static float                   BLUEPRINTS_FXTIME = 0.75F;
    }

    public static class BlueprintsState {
        public static int SelectedBlueprintIndex { get; set; } = 0;
        public static List<Blueprint> LoadedBlueprints { get; } = new List<Blueprint>();
        public static Blueprint SelectedBlueprint { get { return LoadedBlueprints[SelectedBlueprintIndex]; } }

        public static bool InstantBuild { get { return DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild; } }

        private static readonly List<IVisual>          foundationVisuals = new List<IVisual>();
        private static readonly List<IVisual>          dependantVisuals  = new List<IVisual>();
        private static readonly List<ICleanableVisual> cleanableVisuals  = new List<ICleanableVisual>();

        public static readonly Dictionary<int, CellColorPayload>  ColoredCells = new Dictionary<int, CellColorPayload>();

        public static Blueprint CreateBlueprint(Vector2I topLeft, Vector2I bottomRight, FilteredDragTool originTool = null) {
            Blueprint blueprint = new Blueprint(Utilities.GetNewBlueprintLocation("unnamed"));
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

            ClearVisuals();
            foreach (BuildingConfig buildingConfig in blueprint.BuildingConfiguration) {
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

            if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_CREATETOOL) && currentlySelectedCollection != BlueprintsAssets.BLUEPRINTS_CREATE_TOOLCOLLECTION) {
                Traverse.Create(ToolMenu.Instance).Method("ChooseCollection", BlueprintsAssets.BLUEPRINTS_CREATE_TOOLCOLLECTION, true).GetValue();
            }

            else if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_USETOOL) && currentlySelectedCollection != BlueprintsAssets.BLUEPRINTS_USE_TOOLCOLLECTION) {
                Traverse.Create(ToolMenu.Instance).Method("ChooseCollection", BlueprintsAssets.BLUEPRINTS_USE_TOOLCOLLECTION, true).GetValue();
            }

            else if (Input.GetKeyDown(BlueprintsAssets.BLUEPRINTS_INPUT_KEYBIND_SNAPSHOTTOOL) && currentlySelectedCollection != BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION) {
                Traverse.Create(ToolMenu.Instance).Method("ChooseCollection", BlueprintsAssets.BLUEPRINTS_SNAPSHOT_TOOLCOLLECTION, true).GetValue();
            }
        }
    }

    public sealed class Blueprint {
        public string FilePath { get; private set; }
        public string FriendlyName { get; set; }

        public List<BuildingConfig> BuildingConfiguration { get; } = new List<BuildingConfig>();
        public List<Vector2I> DigLocations { get; } = new List<Vector2I>();

        public Blueprint(string filePath) {
            FilePath = filePath;
            InferFriendlyName();
        }

        public bool ReadBinary() {
            if (File.Exists(FilePath)) {
                BuildingConfiguration.Clear();
                DigLocations.Clear();

                try {
                    using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open))) {
                        FriendlyName = reader.ReadString();

                        int buildingCount = reader.ReadInt32();
                        for(int i = 0; i < buildingCount; ++i) {
                            BuildingConfig buildingConfig = new BuildingConfig();
                            if (!buildingConfig.ReadBinary(reader)) {
                                return false;
                            }

                            BuildingConfiguration.Add(buildingConfig);
                        }

                        int digLocationCount = reader.ReadInt32();
                        for (int i = 0; i < digLocationCount; ++i) {
                            DigLocations.Add(new Vector2I(reader.ReadInt32(), reader.ReadInt32()));
                        }
                    }

                    return true;
                }

                catch (System.Exception exception) {
                    Debug.LogError("Error when loading blueprint: " + FilePath + ",\n" + nameof(exception) + ":" + exception.Message);
                }
            }

            return false;
        }

        public bool ReadJSON() {
            if (File.Exists(FilePath)) {
                BuildingConfiguration.Clear();
                DigLocations.Clear();

                try {
                    using (StreamReader reader = File.OpenText(FilePath))
                    using (JsonTextReader jsonReader = new JsonTextReader(reader)) {
                        JObject rootObject = (JObject) JToken.ReadFrom(jsonReader).Root;

                        JToken friendlyNameToken = rootObject.SelectToken("friendlyname");
                        JToken buildingsToken = rootObject.SelectToken("buildings");
                        JToken digCommandsToken = rootObject.SelectToken("digcommands");

                        if (friendlyNameToken != null && friendlyNameToken.Type == JTokenType.String) {
                            FriendlyName = friendlyNameToken.Value<string>();
                        }

                        if (buildingsToken != null) {
                            JArray buildingTokens = buildingsToken.Value<JArray>();

                            if (buildingTokens != null) {
                                foreach (JToken buildingToken in buildingTokens) {
                                    BuildingConfig buildingConfig = new BuildingConfig();

                                    if (buildingConfig.ReadJSON((JObject) buildingToken)) {
                                        BuildingConfiguration.Add(buildingConfig);
                                    }

                                    else if(buildingConfig.ReadJSONLegacy((JObject) buildingToken)) {
                                        BuildingConfiguration.Add(buildingConfig);
                                    }
                                }
                            }
                        }

                        if (digCommandsToken != null) {
                            JArray digCommandTokens = digCommandsToken.Value<JArray>();

                            if (digCommandTokens != null) {
                                foreach (JToken digCommandToken in digCommandTokens) {
                                    JToken xToken = digCommandToken.SelectToken("x");
                                    JToken yToken = digCommandToken.SelectToken("y");

                                    if (xToken != null && xToken.Type == JTokenType.Integer && yToken != null & yToken.Type == JTokenType.Integer) {
                                        DigLocations.Add(new Vector2I(xToken.Value<int>(), yToken.Value<int>()));
                                    }
                                }
                            }
                        }

                        return true;
                    }
                }

                catch (System.Exception exception) {
                    Debug.LogError("Error when loading blueprint: " + FilePath + ",\n" + nameof(exception) + ":" + exception.Message);
                }
            }

            return false;
        }

        public void Write() {
            if (BlueprintsAssets.BLUEPRINTS_BOOL_COMPRESBLUEPRINTS) {
                WriteBinary();
            }

            else {
                WriteJSON();
            }
        }

        public void WriteBinary() {
            using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(FilePath, FileMode.OpenOrCreate))) {
                binaryWriter.Write(FriendlyName);

                binaryWriter.Write(BuildingConfiguration.Count);
                BuildingConfiguration.ForEach(buildingConfig => buildingConfig.WriteBinary(binaryWriter));

                binaryWriter.Write(DigLocations.Count);
                DigLocations.ForEach(digLocation => { binaryWriter.Write(digLocation.x); binaryWriter.Write(digLocation.y); });
            }
        }

        public void WriteJSON() {
            using (TextWriter textWriter = File.CreateText(FilePath))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)) {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("friendlyname");
                jsonWriter.WriteValue(FriendlyName);

                jsonWriter.WritePropertyName("buildings");
                jsonWriter.WriteStartArray();

                foreach (BuildingConfig buildingConfig in BuildingConfiguration) {
                    buildingConfig.WriteJSON(jsonWriter);
                }

                jsonWriter.WriteEndArray();
                jsonWriter.WritePropertyName("digcommands");
                jsonWriter.WriteStartArray();

                foreach (Vector2I digLocation in DigLocations) {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("x");
                    jsonWriter.WriteValue(digLocation.x);
                    jsonWriter.WritePropertyName("y");
                    jsonWriter.WriteValue(digLocation.y);
                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndObject();
            }
        }

        public void DeleteFile() {
            if (File.Exists(FilePath)) {
                File.Delete(FilePath);
            }
        }

        public void Rename(string newFriendlyName) {
            if (newFriendlyName != FriendlyName) {
                DeleteFile();

                string newFileName = newFriendlyName;
                foreach (char invalidCharacter in Path.GetInvalidFileNameChars()) {
                    newFileName = newFileName.Replace(invalidCharacter, '_');
                }

                FilePath = Utilities.GetNewBlueprintLocation(newFileName);
                FriendlyName = newFriendlyName;

                Write();
            }
        }

        public void InferFriendlyName() {
            FileInfo fileInfo = new FileInfo(FilePath);
            FriendlyName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
        }

        public bool IsEmpty() {
            return BuildingConfiguration.Count == 0 && DigLocations.Count == 0;
        }
    }

    public sealed class BuildingConfig : System.IEquatable<BuildingConfig> {
        public Vector2I Offset { get; set; }
        public BuildingDef BuildingDef { get; set; }
        public List<Tag> SelectedElements { get; } = new List<Tag>();
        public Orientation Orientation { get; set; }
        public int Flags { get; set; }

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
            jsonWriter.WritePropertyName("offset");
            jsonWriter.WriteStartObject();
            jsonWriter.WritePropertyName("x");
            jsonWriter.WriteValue(Offset.x);
            jsonWriter.WritePropertyName("y");
            jsonWriter.WriteValue(Offset.y);
            jsonWriter.WriteEndObject();
            jsonWriter.WritePropertyName("buildingdef");
            jsonWriter.WriteValue(BuildingDef.PrefabID);
            jsonWriter.WritePropertyName("selected_elements");
            jsonWriter.WriteStartArray();
            SelectedElements.ForEach(elementTag => jsonWriter.WriteValue(elementTag.GetHash()));
            jsonWriter.WriteEndArray();
            jsonWriter.WritePropertyName("orientation");
            jsonWriter.WriteValue((int) Orientation);
            jsonWriter.WritePropertyName("flags");
            jsonWriter.WriteValue(Flags);
            jsonWriter.WriteEndObject();
        }

        public bool ReadBinary(BinaryReader binaryReader) {
            try {
                Offset = new Vector2I(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                if ((BuildingDef = Assets.GetBuildingDef(binaryReader.ReadString())) == null) {
                    return false;
                }

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

        public bool ReadJSON(JObject rootObject) {
            JToken offsetToken = rootObject.SelectToken("offset");
            JToken buildingDefToken = rootObject.SelectToken("buildingdef");
            JToken selectedElementsToken = rootObject.SelectToken("selected_elements");
            JToken orientationToken = rootObject.SelectToken("orientation");
            JToken flagsToken = rootObject.SelectToken("flags");

            if (offsetToken != null && offsetToken.Type == JTokenType.Object) {
                JToken xToken = offsetToken.SelectToken("x");
                JToken yToken = offsetToken.SelectToken("y");

                if (xToken != null && xToken.Type == JTokenType.Integer && yToken != null & yToken.Type == JTokenType.Integer) {
                    Offset = new Vector2I(xToken.Value<int>(), yToken.Value<int>());
                }

                else {
                    return false;
                }
            }

            else {
                return false;
            }

            if (buildingDefToken != null && buildingDefToken.Type == JTokenType.String) {
                if ((BuildingDef = Assets.GetBuildingDef(buildingDefToken.Value<string>())) == null) {
                    return false;
                }
            }

            else {
                return false;
            }

            if (selectedElementsToken != null && selectedElementsToken.Type == JTokenType.Array) {
                JArray selectedElementTokens = selectedElementsToken.Value<JArray>();

                if (selectedElementTokens == null) {
                    return false;
                }

                else {
                    foreach (JToken selectedElement in selectedElementTokens) {
                        Tag elementTag;

                        if (selectedElement.Type == JTokenType.Integer && ElementLoader.GetElement(elementTag = new Tag(selectedElement.Value<int>())) != null) {
                            SelectedElements.Add(elementTag);
                        }
                    }
                }
            }

            else {
                return false;
            }

            if (orientationToken != null && orientationToken.Type == JTokenType.Integer) {
                Orientation = (Orientation) orientationToken.Value<int>();
            }

            else {
                return false;
            }

            if (flagsToken != null && flagsToken.Type == JTokenType.Integer) {
                Flags = flagsToken.Value<int>();
            }

            else {
                return false;
            }

            return true;
        }

        public bool ReadJSONLegacy(JObject rootObject) {
            JToken offsetToken = rootObject.SelectToken("offset");
            JToken buildingDefToken = rootObject.SelectToken("buildingdef");
            JToken elementToken = rootObject.SelectToken("element");
            JToken orientationToken = rootObject.SelectToken("orientation");
            JToken flagsToken = rootObject.SelectToken("flags");

            if (offsetToken != null && offsetToken.Type == JTokenType.Object) {
                JToken xToken = offsetToken.SelectToken("x");
                JToken yToken = offsetToken.SelectToken("y");

                if (xToken != null && xToken.Type == JTokenType.Integer && yToken != null & yToken.Type == JTokenType.Integer) {
                    Offset = new Vector2I(xToken.Value<int>(), yToken.Value<int>());
                }

                else {
                    return false;
                }
            }

            else {
                return false;
            }

            if (buildingDefToken != null && buildingDefToken.Type == JTokenType.String) {
                if ((BuildingDef = Assets.GetBuildingDef(buildingDefToken.Value<string>())) == null) {
                    return false;
                }
            }

            else {
                return false;
            }

            if (elementToken != null && elementToken.Type == JTokenType.Integer) {
                Tag elementTag;
                if (ElementLoader.GetElement(elementTag = new Tag(elementToken.Value<int>())) == null) {
                    return false;
                }

                else {
                    SelectedElements.Add(elementTag);
                }
            }

            else {
                return false;
            }

            if (orientationToken != null && orientationToken.Type == JTokenType.Integer) {
                Orientation = (Orientation)orientationToken.Value<int>();
            }

            else {
                return false;
            }

            if (flagsToken != null && flagsToken.Type == JTokenType.Integer) {
                Flags = flagsToken.Value<int>();
            }

            else {
                return false;
            }

            return true;
        }

        public bool Equals(BuildingConfig otherBuildingConfig) {
            return Offset == otherBuildingConfig.Offset && BuildingDef == otherBuildingConfig.BuildingDef;
        }
    }

    public struct CellColorPayload {
        public Color Color { get; set; }
        public ObjectLayer TileLayer { get; set; }
        public ObjectLayer ReplacementLayer { get; set; }

        public CellColorPayload(Color color, ObjectLayer tileLayer, ObjectLayer replacementLayer) {
            Color = color;
            TileLayer = tileLayer;
            ReplacementLayer = replacementLayer;
        }
    }
}