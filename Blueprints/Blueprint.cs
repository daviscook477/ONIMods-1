using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Blueprints {
    public sealed class BlueprintFolder {
        public string Name { get; set; }
        public Blueprint SelectedBlueprint => contents[selectedBlueprintIndex];
        public int BlueprintCount => contents.Count;

        private int selectedBlueprintIndex = 0;
        public int SelectedBlueprintIndex {
            get {
                return selectedBlueprintIndex;
            }

            set {
                selectedBlueprintIndex = Mathf.Clamp(value, 0, contents.Count - 1);
            }
        }

        private readonly List<Blueprint> contents = new List<Blueprint>();

        public BlueprintFolder(string name) {
            Name = name;
        }

        public void AddBlueprint(Blueprint blueprint) {
            contents.Add(blueprint);
            SelectedBlueprintIndex = contents.Count - 1;
        }

        public void DeleteBlueprint(Blueprint blueprint, bool deleteIfEmpty = true) {
            contents.Remove(blueprint);
            selectedBlueprintIndex = Mathf.Clamp(SelectedBlueprintIndex, 0, contents.Count - 1);

            if (deleteIfEmpty && BlueprintCount == 0) {
                BlueprintsState.LoadedBlueprints.Remove(this);
                BlueprintsState.SelectedBlueprintFolderIndex = Mathf.Clamp(BlueprintsState.SelectedBlueprintFolderIndex, 0, BlueprintsState.LoadedBlueprints.Count - 1);

                if (Name != "") {
                    string path = Path.Combine(Utilities.GetBlueprintDirectory(), Name);

                    if (Directory.Exists(path)) {
                        Directory.Delete(path);
                    }
                }
            }
        }

        public void DeleteBlueprint(int index, bool deleteIfEmpty = true) {
            DeleteBlueprint(contents[index], deleteIfEmpty);
        }

        public bool NextBlueprint() {
            if (contents.Count == 0) {
                return false;
            }

            if (++selectedBlueprintIndex >= BlueprintCount) {
                selectedBlueprintIndex = 0;
            }

            return true;
        }

        public bool PreviousBlueprint() {
            if (contents.Count == 0) {
                return false;
            }

            if (--selectedBlueprintIndex < 0) {
                selectedBlueprintIndex = BlueprintCount - 1;
            }

            return true;
        }
    }

    public sealed class Blueprint {
        public string FriendlyName { get; set; } = "unnamed";
        public string FilePath { get; private set; }
        public string Folder { get; private set; } = "";

        public List<BuildingConfig> BuildingConfiguration { get; } = new List<BuildingConfig>();
        public List<Vector2I> DigLocations { get; } = new List<Vector2I>();

        public Blueprint(string fileLocation) {
            int blueprintsDirectoryLength = Utilities.GetBlueprintDirectory().Length + 1;
            int folderLength = fileLocation.Length - (blueprintsDirectoryLength + Path.GetFileName(fileLocation).Length + 1);

            FilePath = fileLocation;
            Folder = fileLocation.Substring(blueprintsDirectoryLength, Mathf.Max(0, folderLength)).ToLower();
        }

        public Blueprint(string friendlyName, string folder) {
            FriendlyName = friendlyName;
            Folder = SanitizeFolder(folder).ToLower();

            InferFileLocation();
            InferFriendlyName();
        }

        private string SanitizeFolder(string folder) {
            if (folder == "") {
                return "";
            }

            folder = folder.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            string returnString = "";
            
            string[] folderSections = folder.Split(Path.DirectorySeparatorChar);
            foreach (string folderSection in folderSections) {
                if (folderSection.Trim().Length > 0) {
                    returnString += SanitizeFile(folderSection) + Path.DirectorySeparatorChar;
                }
            }

            return returnString.TrimEnd(Path.DirectorySeparatorChar).ToLowerInvariant();
        }

        private string SanitizeFile(string file) {
            string returnString = "";

            for(int i = 0; i < file.Length; ++i) {
                char character = file[i];
                returnString += BlueprintsAssets.BLUEPRINTS_FILE_DISALLOWEDCHARACTERS.Contains(character) ? '_' : character;
            }

            return returnString.Trim().ToLowerInvariant();
        }

        public bool ReadBinary() {
            if (File.Exists(FilePath)) {
                BuildingConfiguration.Clear();
                DigLocations.Clear();

                try {
                    using (BinaryReader reader = new BinaryReader(File.Open(FilePath, FileMode.Open))) {
                        FriendlyName = reader.ReadString();

                        int buildingCount = reader.ReadInt32();
                        for (int i = 0; i < buildingCount; ++i) {
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
                    Debug.Log("Error when loading blueprint: " + FilePath + ",\n" + nameof(exception) + ": " + exception.Message);
                }
            }

            return false;
        }

        public void ReadJSON() {
            if (File.Exists(FilePath)) {
                BuildingConfiguration.Clear();
                DigLocations.Clear();

                try {
                    using StreamReader reader = File.OpenText(FilePath);
                    using JsonTextReader jsonReader = new JsonTextReader(reader);

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
                                buildingConfig.ReadJSON((JObject) buildingToken);

                                BuildingConfiguration.Add(buildingConfig);
                            }
                        }
                    }

                    if (digCommandsToken != null) {
                        JArray digCommandTokens = digCommandsToken.Value<JArray>();

                        if (digCommandTokens != null) {
                            foreach (JToken digCommandToken in digCommandTokens) {
                                JToken xToken = digCommandToken.SelectToken("x");
                                JToken yToken = digCommandToken.SelectToken("y");

                                if (xToken != null && xToken.Type == JTokenType.Integer || yToken != null && yToken.Type == JTokenType.Integer) {
                                    DigLocations.Add(new Vector2I(xToken == null ? 0 : xToken.Value<int>(), yToken == null ? 0 : yToken.Value<int>()));
                                }

                                else if (xToken == null && yToken == null) {
                                    DigLocations.Add(new Vector2I(0, 0));
                                }
                            }
                        }
                    }
                }

                catch (System.Exception exception) {
                    Debug.Log("Error when loading blueprint: " + FilePath + ",\n" + nameof(exception) + ":" + exception.Message);
                }
            }
        }

        public void Write() {
            string folder = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }

            if (BlueprintsAssets.BLUEPRINTS_CONFIG_COMPRESBLUEPRINTS) {
                WriteBinary();
            }

            else {
                WriteJSON();
            }
        }

        public void WriteBinary() {
            using BinaryWriter binaryWriter = new BinaryWriter(File.Open(FilePath, FileMode.OpenOrCreate));

            binaryWriter.Write(FriendlyName);

            binaryWriter.Write(BuildingConfiguration.Count);
            BuildingConfiguration.ForEach(buildingConfig => buildingConfig.WriteBinary(binaryWriter));

            binaryWriter.Write(DigLocations.Count);
            DigLocations.ForEach(digLocation => { binaryWriter.Write(digLocation.x); binaryWriter.Write(digLocation.y); });
        }

        public void WriteJSON() {
            using TextWriter textWriter = File.CreateText(FilePath);
            using JsonTextWriter jsonWriter = new JsonTextWriter(textWriter) {
                Formatting = Formatting.Indented
            };

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("friendlyname");
            jsonWriter.WriteValue(FriendlyName);

            if (BuildingConfiguration.Count > 0) {
                jsonWriter.WritePropertyName("buildings");
                jsonWriter.WriteStartArray();

                foreach (BuildingConfig buildingConfig in BuildingConfiguration) {
                    buildingConfig.WriteJSON(jsonWriter);
                }

                jsonWriter.WriteEndArray();
            }

            if (DigLocations.Count > 0) {
                jsonWriter.WritePropertyName("digcommands");
                jsonWriter.WriteStartArray();

                foreach (Vector2I digLocation in DigLocations) {
                    jsonWriter.WriteStartObject();

                    if (digLocation.x != 0) {
                        jsonWriter.WritePropertyName("x");
                        jsonWriter.WriteValue(digLocation.x);
                    }

                    if (digLocation.y != 0) {
                        jsonWriter.WritePropertyName("y");
                        jsonWriter.WriteValue(digLocation.y);
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();
            }

            jsonWriter.WriteEndObject();
        }

        public void DeleteFile() {
            if (File.Exists(FilePath)) {
                File.Delete(FilePath);
            }
        }

        public void SetFolder(string newFolder, bool rewrite = true) {
            if (rewrite) {
                DeleteFile();
            }

            for (int i = 0; i < BlueprintsState.LoadedBlueprints.Count; ++i) {
                if (BlueprintsState.LoadedBlueprints[i].Name == Folder) {
                    BlueprintsState.SelectedFolder.DeleteBlueprint(this);
                    break;
                }
            }

            Folder = SanitizeFolder(newFolder).ToLower();
            InferFileLocation();

            Utilities.PlaceIntoFolder(this);
            if (rewrite) {
                Write();
            }
        }

        public void Rename(string newFriendlyName, bool rewrite = true) {
            if (rewrite) {
                DeleteFile();
            }

            FriendlyName = newFriendlyName;
            InferFileLocation();

            if (rewrite) {
                Write();
            }
        }

        public void InferFileLocation() {
            FilePath = GetFileLocation(-1);
            int index = 0;

            while (File.Exists(FilePath)) {
                FilePath = GetFileLocation(index);
                ++index;
            }
        }

        private string GetFileLocation(int index) {
            string sanitizedFriendlyName = SanitizeFile(FriendlyName);

            if (index == -1) {
                return Path.Combine(Path.Combine(Utilities.GetBlueprintDirectory(), Folder), sanitizedFriendlyName + ".blueprint");
            }

            return Path.Combine(Path.Combine(Utilities.GetBlueprintDirectory(), Folder), sanitizedFriendlyName + "-" + index + ".blueprint");
        }

        public void InferFriendlyName() {
            FileInfo fileInfo = new FileInfo(FilePath);
            FriendlyName = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
        }

        public bool IsEmpty() {
            return BuildingConfiguration.Count == 0 && DigLocations.Count == 0;
        }
    }
}
