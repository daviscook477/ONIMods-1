using System.Reflection;
using System.IO;

using KMod;

using Harmony;
using UnityEngine;
using System.Collections.Generic;

namespace VersionDisplayer {
    public static class VersionDisplayer {
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
                HarmonyMethod postfix = new HarmonyMethod(typeof(VersionDisplayer), "ModsScreen_BuildDisplay_Prefix");
                versionDisplayerInstance.Patch(_ModsScreen_OnActivate, null, postfix);
            }

            Debug.Log("Successfully applied version displayer patches!");
        }

        public static void ModsScreen_BuildDisplay_Postfix(Transform ___entryParent) {
            int childCount = ___entryParent.childCount;

            for(int i = 0; i < childCount; ++i) {
                Transform child = ___entryParent.GetChild(i);

                if (child != null && child.GetComponent<HierarchyReferences>() != null) {
                    HierarchyReferences hierarchyReferences = child.GetComponent<HierarchyReferences>();
                    LocText titleLabel = hierarchyReferences.GetReference<LocText>("Label");

                    if (titleLabel != null) {
                        string title = titleLabel.text;
                        Mod mod = Global.Instance.modManager.mods.Find(mod0 => mod0.title == title);

                        if (mod != null) {
                            mod.Unload(Content.DLL);
                            string[] dllFiles = Directory.GetFiles(mod.label.install_path, "*.dll");

                            if (dllFiles.Length > 0) {
                                Assembly assembly = null;
                                int j = 0;

                                while ((assembly = Assembly.LoadFrom(dllFiles[j])) == null) {
                                    if (++j >= dllFiles.Length) {
                                        break;
                                    }
                                }

                                if (assembly != null) {
                                    mod.label.title += " (" + assembly.GetName().Version + ")";
                                }
                            }

                        }
                    }
                }
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
