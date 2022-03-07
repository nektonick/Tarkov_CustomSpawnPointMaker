using UnityEngine;
using Aki.Common.Utils;

namespace Lua.CustomSpawnPointMaker
{
    public class Program
    {
        private static GameObject HookObject;

        static void Main(string[] args)
        {
            Log.Info("Loading: Lua-CustomSpawnPointMaker...");
            HookObject = new GameObject();
            HookObject.AddComponent<CSPGenerator>();
            Object.DontDestroyOnLoad(HookObject);
        }
    }
}
