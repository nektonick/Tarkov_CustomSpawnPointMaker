using System.Reflection;
using Aki.Reflection.Patching;

namespace Lua.CultistsSpawnPatcher
{
    class CulitstSpawnPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(ZoneLeaveControllerClass).GetMethod("IsDayByHour");
        }
        [PatchPrefix]
        private static bool PatchPrefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
