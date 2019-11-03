using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace ModKeyBinding {
    public sealed class KeyBinding {
        private readonly List<KeyCode> keyCodes = new List<KeyCode>();
        private string textRepresentation;

        public KeyBinding(string text) {
            string[] keyBindings = text.Split('+');
            textRepresentation = "";

            foreach (string keyBinding in keyBindings) {
                if (TryParseEnum<KeyCode>(keyBinding, out KeyCode keyCode)) {
                    if (keyCode != KeyCode.None) {
                        keyCodes.Add(keyCode);
                        textRepresentation += keyCode + "+";
                    }
                }
            }

            if (keyCodes.Count == 0) {
                textRepresentation = "NONE";
            }

            else {
                textRepresentation = textRepresentation.TrimEnd('+');
            }
        }

        public KeyBinding(params KeyCode[] keyCodes) {
            textRepresentation = "";

            foreach (KeyCode keyCode in keyCodes) {
                if (keyCode != KeyCode.None) {
                    this.keyCodes.Add(keyCode);
                    textRepresentation += keyCode + "+";
                }

            }

            if (this.keyCodes.Count == 0) {
                textRepresentation = "NONE";
            }

            else {
                textRepresentation = textRepresentation.TrimEnd('+');
            }
        }

        internal static bool TryParseEnum<T>(string input, out T output) {
            string inputLower = input.ToLower();
            foreach (T enumeration in System.Enum.GetValues(typeof(T))) {
                if (enumeration.ToString().ToLower() == inputLower) {
                    output = enumeration;
                    return true;
                }
            }

            output = default;
            return false;
        }


        public void AssignIfEmpty(KeyCode keyCode) {
            if (keyCodes.Count == 0) {
                keyCodes.Add(keyCode);
                textRepresentation = keyCode.ToString();
            }
        }

        public bool IsActive() {
            return keyCodes.Count > 0 && keyCodes.TrueForAll(x => Input.GetKey(x));

        }

        public string GetStringFormatted() {
            return UI.FormatAsHotkey("[" + textRepresentation + "]");
        }

        public override string ToString() {
            return textRepresentation;
        }
    }
}
