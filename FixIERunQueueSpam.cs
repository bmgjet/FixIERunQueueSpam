// Reference: 0Harmony
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Harmony;

namespace Oxide.Plugins
{
    [Info("FixIERunQueueSpam", "bmgjet", "1.0.0")]
    [Description("Stops RunQueue Spam")]
    class FixIERunQueueSpam : RustPlugin
    {
        private HarmonyInstance _harmony;
        private void Init()
        {
            _harmony = HarmonyInstance.Create(Name + "PATCH");
            Type[] patchType = { AccessTools.Inner(typeof(FixIERunQueueSpam), "AddQueueNullCheck"), };
            foreach (var t in patchType) { new PatchProcessor(_harmony, t, HarmonyMethod.Merge(t.GetHarmonyMethods())).Patch(); }
        }
        private void Unload() { _harmony.UnpatchAll(Name + "PATCH"); }

        [HarmonyPatch(typeof(ServerMgr), "Update")]
        public static class AddQueueNullCheck
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                List<CodeInstruction> list = instructions.ToList<CodeInstruction>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Ldstr && list[i].operand as string == "Server Exception: IndustrialEntity.RunQueue")
                    {
                        list[i].opcode = OpCodes.Nop;
                        list[i + 1].opcode = OpCodes.Nop;
                        list[i + 2].opcode = OpCodes.Nop;
                        list[i + 3].opcode = OpCodes.Nop;
                        break;
                    }
                }
                return list;
            }
        }
    }
}