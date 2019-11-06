using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace ModKeyBinding {
    public enum KeyBindingType {
        Press,
        Hold,
        Release
    };

    public sealed class KeyBinding {
        public KeyBindingType Type { get; set; }
        private readonly List<KeyCode> keyCodes = new List<KeyCode>();
        private string textRepresentation;

        public KeyBinding(KeyBindingType type, string text) {
            Type = type;
            textRepresentation = "";

            string[] keyBindings = text.Split('+');
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

        public KeyBinding(KeyBindingType type, params KeyCode[] keyCodes) {
            Type = type;
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

        private static bool TryParseEnum<T>(string input, out T output) {
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
            if (keyCodes.Count == 0) {
                return false;
            }

            foreach (KeyCode keyCode in keyCodes) {
                switch (Type) {
                    case KeyBindingType.Press: {
                        if (!Input.GetKeyDown(keyCode)) {
                            return false;
                        }

                        break;
                    }
                        
                    case KeyBindingType.Hold: {
                        if (!Input.GetKey(keyCode)) {
                            return false;
                        }

                        break;
                    }

                    default: {
                        if (!Input.GetKeyUp(keyCode)) {
                            return false;
                        }

                        break;
                    }
                }
            }

            return true;
        }

        public string GetStringFormatted() {
            return UI.FormatAsHotkey("[" + textRepresentation.ToUpper() + "]");
        }

        public override string ToString() {
            return textRepresentation;
        }
    }
}
