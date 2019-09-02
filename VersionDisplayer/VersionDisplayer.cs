using System.Reflection;
using System.IO;

using KMod;

using Harmony;

namespace VersionDisplayer {
    public static class VersionDisplayer {
        private static bool NamesPatched = false;

        private static void Patch(HarmonyInstance harmonyInstance) {
            if (harmonyInstance.HasAnyPatches("mod.mayall.versiondisplayer.patches")) {
                return;
            }

            HarmonyInstance versionDisplayerInstance = HarmonyInstance.Create("mod.mayall.versiondisplayer.patches");

            MethodInfo _ModsScreen_OnActivate = typeof(ModsScreen).GetMethod("OnActivate", BindingFlags.NonPublic | BindingFlags.Instance);
            if (_ModsScreen_OnActivate == null) {
                Debug.LogError("Failed to apply version displayer patches! \nError: Could not find ModsScreen::OnActivate to patch!");
            }

            else {
                HarmonyMethod prefix = new HarmonyMethod(typeof(VersionDisplayer), "ModsScreen_OnActivate_Prefix");
                versionDisplayerInstance.Patch(_ModsScreen_OnActivate, prefix);
            }

            Debug.Log("Successfully applied version displayer patches!");
        }

        public static void ModsScreen_OnActivate_Prefix() {
            if (!NamesPatched) {
                foreach (Mod mod in Global.Instance.modManager.mods) {
                    string[] dllFiles = Directory.GetFiles(mod.label.install_path, "*.dll");

                    if (dllFiles.Length > 0) {
                        Assembly assembly = null;
                        int i = 0;

                        while ((assembly = Assembly.LoadFrom(dllFiles[i])) == null) {
                            if (++i >= dllFiles.Length) {
                                break;
                            }
                        }

                        if (assembly != null) {
                            mod.label.title += " (" + assembly.GetName().Version + ")";
                        }
                    }
                }

                NamesPatched = true;
            }
        }

        [HarmonyPatch]
        public static class VersionDisplayer_Hook {
            public static void DummyMethod() { }

            public static MethodBase TargetMethod(HarmonyInstance harmonyInstance) {
                Patch(harmonyInstance);
                return typeof(VersionDisplayer_Hook).GetMethod("DummyMethod");
            }

            public static void Prefix() { }
            public static void Postfix() { }
        }
    }
}
