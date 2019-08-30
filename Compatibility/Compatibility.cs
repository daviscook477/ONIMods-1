using System;
using System.Collections.Generic;
using System.Reflection;

using Harmony;

namespace Compatibility {
    public static class Compatibility {
        private static void Patch(HarmonyInstance harmonyInstance) {
            if (harmonyInstance.HasAnyPatches("mod.mayall.compatibility.patches")) {
                return;
            }

            HarmonyInstance compatibilityInstance = HarmonyInstance.Create("mod.mayall.compatibility.patches");

            ConstructorInfo _KButtonEvent_Constructor = typeof(KButtonEvent).GetConstructor(new Type[] { typeof(KInputController), typeof(InputEventType), typeof(bool[]) });
            if (_KButtonEvent_Constructor == null) {
                Debug.LogError("Failed to apply compatibility patches! Expect stuff to be broken. \nError: Could not find KButtonEvent constructor to patch!");
            }

            else {
                HarmonyMethod postfix = new HarmonyMethod(typeof(Compatibility), "KButtonEvent_Constructor_Postfix");
                compatibilityInstance.Patch(_KButtonEvent_Constructor, null, postfix);
            }

            MethodInfo _GameInputMapping_FindEntry = typeof(GameInputMapping).GetMethod("FindEntry");
            if (_GameInputMapping_FindEntry == null) {
                Debug.LogError("Failed to apply compatibility patches! Expect stuff to be broken. \nError: Could not find GameInputMapping::FindEntry to patch!");
            }

            else {
                HarmonyMethod prefix = new HarmonyMethod(typeof(Compatibility), "GameInputMapping_FindEntry_Prefix");
                compatibilityInstance.Patch(_GameInputMapping_FindEntry, prefix);
            }

            MethodInfo _GameInputMapping_CompareActionKeyCodes = typeof(GameInputMapping).GetMethod("CompareActionKeyCodes");
            if (_GameInputMapping_CompareActionKeyCodes == null) {
                Debug.LogError("Failed to apply compatibility patches! Expect stuff to be broken. \nError: Could not find GameInputMapping::CompareActionKeyCodes to patch!");
            }

            else {
                HarmonyMethod prefix = new HarmonyMethod(typeof(Compatibility), "GameInputMapping_CompareActionKeyCodes_Prefix");
                compatibilityInstance.Patch(_GameInputMapping_CompareActionKeyCodes, prefix);
            }

            Debug.Log("Successfully applied compatibility patches!");
        }

        public static void KButtonEvent_Constructor_Postfix(ref bool[] ___mIsAction) {
            ___mIsAction = new List<bool>(___mIsAction) { false }.ToArray();
        }

        public static bool GameInputMapping_FindEntry_Prefix(Action mAction, ref BindingEntry __result) {
            foreach (BindingEntry keyBinding in GameInputMapping.KeyBindings) {
                if (keyBinding.mAction == mAction) {
                    __result = keyBinding;
                    return false;
                }
            }

            __result = new BindingEntry(null, GamepadButton.NumButtons, KKeyCode.None, Modifier.None, Action.NumActions, false, false);
            return false;
        }

        public static bool GameInputMapping_CompareActionKeyCodes_Prefix(Action a, Action b, ref bool __result) {
            BindingEntry entry1 = GameInputMapping.FindEntry(a);
            BindingEntry entry2 = GameInputMapping.FindEntry(b);

            if (!(entry1.mAction == Action.NumActions || entry2.mAction == Action.NumActions) && entry1.mKeyCode == entry2.mKeyCode) {
                __result = entry1.mModifier == entry2.mModifier;
                return false;
            }

            __result = false;
            return false;
        }

        [HarmonyPatch]
        public static class Compatibility_Hook {
            public static void DummyMethod() { }

            public static MethodBase TargetMethod(HarmonyInstance harmonyInstance) {
                Patch(harmonyInstance);
                return typeof(Compatibility_Hook).GetMethod("DummyMethod");
            }

            public static void Prefix() { }
            public static void Postfix() { }
        }
    }
}
