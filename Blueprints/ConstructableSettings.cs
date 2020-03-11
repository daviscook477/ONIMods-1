using KSerialization;
using Harmony;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection.Emit;

namespace Blueprints {
    /// <summary>
    /// Component that is attached to buildings under construction.
    /// Under construction means both while it is simply an outline that has been placed,
    /// or while a duplicant is in the middle of building the thing.
    /// 
    /// The component stores a save reference to an original source building that is used
    /// as the source of the settings for this new building. In more general terms, this
    /// component is used to copy settings from an old building to the new building this
    /// component is attached to.
    /// </summary>
    [SerializationConfig(MemberSerialization.OptIn)]
    public sealed class ConstructableSettings : KMonoBehaviour, ISaveLoadable {
        [Serialize]
        public GameObject settingsSource;

        /// <summary>
        /// Simple method used in the transpiler patch on `Constructable` that copies
        /// settings from this component onto a completed building.
        /// </summary>
        /// <param name="gameObject">The completed building</param>
        /// <param name="__instance">The constructable that was finished</param>
        public static void CopySettings(GameObject gameObject, Constructable __instance) {
            ConstructableSettings settings = __instance.GetComponent<ConstructableSettings>();
            // Since this method hooks into all construction completions, many of them will not have any settings to transfer.
            if (settings == null)
                return;
            CopyUtilities.CopySettingsWithDelegate(gameObject, settings.settingsSource);
        }

        [HarmonyPatch(typeof(Constructable))]
        [HarmonyPatch("FinishConstruction")]
        public static class Constructable_FinishConstruction_Patch {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                for (int i = 0; i < codes.Count; i++)
                {
                    CodeInstruction instr = codes[i];
                    // Search for the call to BuildingDef.Build that returns the GameObject we need to modify
                    if ((instr.opcode.Equals(OpCodes.Call) || instr.opcode.Equals(OpCodes.Calli) || instr.opcode.Equals(OpCodes.Callvirt))
                        && (bool)instr.operand?.Equals(typeof(BuildingDef).GetMethod("Build"))) {
                        // Insert at one above the index in order to be directly after the call instruction
                        int start = i + 1;
                        codes.Insert(start + 0, new CodeInstruction(OpCodes.Dup));
                        codes.Insert(start + 1, new CodeInstruction(OpCodes.Ldarg_0));
                        codes.Insert(start + 2, new CodeInstruction(OpCodes.Call, typeof(ConstructableSettings).GetMethod("CopySettings")));
                        break;
                    }
                }

                return codes;
            }
        }
    }
}
