using ModKeyBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using UnityEngine;

namespace Pliers {
    public static class Utilities {
        public static Sprite CreateSpriteDXT5(Stream inputStream, int width, int height) {
            byte[] buffer = new byte[inputStream.Length - 128];
            inputStream.Seek(128, SeekOrigin.Current);
            inputStream.Read(buffer, 0, buffer.Length);

            Texture2D texture = new Texture2D(width, height, TextureFormat.DXT5, false);
            texture.LoadRawTextureData(buffer);
            texture.Apply(false, true);
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5F, 0.5F));
        }

        public static CellOffset ConnectionsToOffset(UtilityConnections utilityConnections) {
            return utilityConnections switch {
                UtilityConnections.Left => new CellOffset(-1, 0),
                UtilityConnections.Right => new CellOffset(1, 0),
                UtilityConnections.Up => new CellOffset(0, 1),
                _ => new CellOffset(0, -1)
            };
        }

        public static UtilityConnections OppositeDirection(UtilityConnections utilityConnections) {
            return utilityConnections switch {
                UtilityConnections.Left => UtilityConnections.Right,
                UtilityConnections.Right => UtilityConnections.Left,
                UtilityConnections.Up => UtilityConnections.Down,
                _ => UtilityConnections.Up,
            };
        }
    }

    public static class IOUtilities {
        public static void WriteConfig() {
            if (!Directory.Exists(PliersAssets.PLIERS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(PliersAssets.PLIERS_PATH_CONFIGFOLDER);
            }

            using TextWriter textWriter = File.CreateText(PliersAssets.PLIERS_PATH_CONFIGFILE);
            using JsonTextWriter jsonWriter = new JsonTextWriter(textWriter) {
                Formatting = Formatting.Indented
            };

            jsonWriter.WriteStartObject();

            jsonWriter.WritePropertyName("keybind_wirecutter");
            jsonWriter.WriteValue(PliersAssets.PLIERS_KEYBIND_TOOL.ToString());

            jsonWriter.WriteEndObject();
        }

        public static void CreateKeycodeHintFile() {
            if (!Directory.Exists(PliersAssets.PLIERS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(PliersAssets.PLIERS_PATH_CONFIGFOLDER);
            }

            if (!File.Exists(PliersAssets.PLIERS_PATH_KEYCODESFILE)) {
                using TextWriter textWriter = File.CreateText(PliersAssets.PLIERS_PATH_KEYCODESFILE);

                foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode))) {
                    textWriter.WriteLine(keycode.ToString());
                }
            }
        }

        public static void ReadConfig() {
            if (!Directory.Exists(PliersAssets.PLIERS_PATH_CONFIGFOLDER)) {
                Directory.CreateDirectory(PliersAssets.PLIERS_PATH_CONFIGFOLDER);
                return;
            }

            using StreamReader reader = File.OpenText(PliersAssets.PLIERS_PATH_CONFIGFILE);
            using JsonTextReader jsonReader = new JsonTextReader(reader);

            JObject rootObject = (JObject) JToken.ReadFrom(jsonReader).Root;
            JToken kWireToolToken = rootObject.SelectToken("keybind_wirecutter");

            if (kWireToolToken != null && kWireToolToken.Type == JTokenType.String) {
                PliersAssets.PLIERS_KEYBIND_TOOL = new KeyBinding(KeyBindingType.Press, kWireToolToken.Value<string>());
                PliersAssets.PLIERS_KEYBIND_TOOL.AssignIfEmpty(KeyCode.None);
            }
        }
    }
}
