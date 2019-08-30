using System;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEngine;

using STRINGS;

namespace WireCutter {
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

        public static bool TryParseEnum<T>(string input, out T output) {
            string inputLower = input.ToLower();
            foreach (T enumeration in Enum.GetValues(typeof(T))) {
                if (enumeration.ToString().ToLower() == inputLower) {
                    output = enumeration;
                    return true;
                }
            }

            output = default;
            return false;
        }

        public static string GetKeyCodeString(KeyCode keyCode) {
            return UI.FormatAsHotkey("[" + keyCode.ToString().ToUpper() + "]");
        }

        public static CellOffset ConnectionsToOffset(UtilityConnections utilityConnections) {
            switch (utilityConnections) {
                case UtilityConnections.Left:
                    return new CellOffset(-1, 0);

                case UtilityConnections.Right:
                    return new CellOffset(1, 0);

                case UtilityConnections.Up:
                    return new CellOffset(0, 1);

                default:
                    return new CellOffset(0, -1);
            }
        }

        public static UtilityConnections OppositeDirection(UtilityConnections utilityConnections) {
            switch (utilityConnections) {
                case UtilityConnections.Left:
                    return UtilityConnections.Right;

                case UtilityConnections.Right:
                    return UtilityConnections.Left;

                case UtilityConnections.Up:
                    return UtilityConnections.Down;

                default:
                    return UtilityConnections.Up;
            }
        }
    }

    public static class IOUtilities {
        public static void CreateDefaultConfig() {
            using (TextWriter textWriter = File.CreateText(WireCutterAssets.WIRECUTTER_PATH_CONFIGFILE))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)) {
                jsonWriter.Formatting = Formatting.Indented;

                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("keybind_wirecutter");
                jsonWriter.WriteValue(KeyCode.None.ToString());
                jsonWriter.WriteEndObject();
            }
        }

        public static void CreateKeycodeHintFile() {
            if(!File.Exists(WireCutterAssets.WIRECUTTER_PATH_KEYCODESFILE)) {
                using (TextWriter textWriter = File.CreateText(WireCutterAssets.WIRECUTTER_PATH_KEYCODESFILE)) {
                    foreach (KeyCode keycode in Enum.GetValues(typeof(KeyCode))) {
                        textWriter.WriteLine(keycode.ToString());
                    }
                }
            }
        }

        public static void ReadConfig() {
            using (StreamReader reader = File.OpenText(WireCutterAssets.WIRECUTTER_PATH_CONFIGFILE))
            using (JsonTextReader jsonReader = new JsonTextReader(reader)) {
                JObject rootObject = (JObject) JToken.ReadFrom(jsonReader).Root;

                JToken kWireToolToken = rootObject.SelectToken("keybind_wirecutter");

                if (kWireToolToken != null && kWireToolToken.Type == JTokenType.String && Utilities.TryParseEnum<KeyCode>(kWireToolToken.Value<string>(), out KeyCode kWireTool)) {
                    WireCutterAssets.WIRECUTTER_INPUT_KEYBIND_TOOL = kWireTool;
                    WireCutterAssets.WIRECUTTER_TOOLTIP = "Disconnect Utilities " + Utilities.GetKeyCodeString(kWireTool);
                }
            }
        }
    }
}
