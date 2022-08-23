using BepInEx;

namespace Lua.CultistsSpawnPatcher
{
    [BepInPlugin("com.Lua.CultistsSpawnPatcher", "Lua-CultistsSpawnPatcher", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new CulitstSpawnPatch().Enable();
        }
    }
}
