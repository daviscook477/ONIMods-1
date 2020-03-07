using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace ModKeyBinding {


    public enum KeyBindingType {
        Press,
        Hold,
        Release
    };

    public interface IBinding {
        bool IsActive();
    }

    public sealed class MouseWheelBinding : IBinding {
        public bool Up { get; set; }

        public MouseWheelBinding(bool up) {
            Up = up;
        }

        public bool IsActive() {
            return Up ? Input.mouseScrollDelta.y > 0 : Input.mouseScrollDelta.y < 0;
        }

        public override string ToString() {
            return Up ? "MOUSEWHEELUP" : "MOUSEWHEELDOWN";
        }
    }

    public sealed class KeyBinding : IBinding {
        public KeyCode KeyCode { get; set; }
        public KeyBindingType Type { get; set; }

        public KeyBinding(KeyCode keyCode, KeyBindingType type) {
            KeyCode = keyCode;
            Type = type;
        }

        public bool IsActive() {
            switch (Type) {
                case KeyBindingType.Press: {
                    if (!Input.GetKeyDown(KeyCode)) {
                        return false;
                    }

                    break;
                }

                case KeyBindingType.Hold: {
                    if (!Input.GetKey(KeyCode)) {
                        return false;
                    }

                    break;
                }

                default: {
                    if (!Input.GetKeyUp(KeyCode)) {
                        return false;
                    }

                    break;
                }
            }

            return true;
        }
    }

    public sealed class ModKeyBinding {
        private readonly List<IBinding> bindings = new List<IBinding>();
        private string textRepresentation;

        public ModKeyBinding(string text) {
            textRepresentation = "";

            string[] keyBindings = text.Split('+');
            foreach (string keyBinding in keyBindings) {
                bool key = false;

                if (TryParseEnum<KeyCode>(keyBinding, out KeyCode keyCode)) {
                    if (keyCode != KeyCode.None) {
                        bindings.Add(new KeyBinding(keyCode, KeyBindingType.Press));
                        textRepresentation += keyCode + "+";
                        key = true;
                    }
                }

                if (!key) {
                    string lKeyBinding = keyBinding.ToLower();

                    if (lKeyBinding == "mousewheelup" || lKeyBinding == "mousewheeldown") {
                        bindings.Add(new MouseWheelBinding(lKeyBinding == "mousewheelup"));
                    }
                }
            }

            if (bindings.Count == 0) {
                textRepresentation = "NONE";
            }

            else {
                textRepresentation = textRepresentation.TrimEnd('+');
            }
        }

        public ModKeyBinding(KeyCode keyCode) : this(keyCode.ToString()) {  }

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
            AssignIfEmpty(new KeyBinding(keyCode, KeyBindingType.Press));
        }

        public void AssignIfEmpty(IBinding binding) {
            if (bindings.Count == 0) {
                bindings.Add(binding);
                textRepresentation = binding.ToString();
            }
        }

        public bool IsActive() {
            if (bindings.Count == 0) {
                return false;
            }

            foreach (IBinding binding in bindings) {
                if (!binding.IsActive()) {
                    return false;
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
