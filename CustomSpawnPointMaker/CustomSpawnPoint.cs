using System;
using System.IO;
using Comfort.Common;
using Newtonsoft.Json.Linq;
using EFT;
using UnityEngine;
using Aki.Common.Utils;

namespace Lua.CustomSpawnPointMaker
{
    public class CSPGenerator : MonoBehaviour
    {
        private readonly static string folderPath = @"./user/mods/Lua-CustomSpawnPointMaker/CustomSpawnPoints";
        private readonly static string filePath = $"{folderPath}/{DateTime.Now.ToString("yyyy-MM-dd")}.json";

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                var localPlayer = GetLocalPlayerFromWorld();
                if (localPlayer == null)
                {
                    return;
                }

                var fileLen = 0;
                if (! Directory.Exists(folderPath) )
                {
                    Directory.CreateDirectory(folderPath);
                    File.CreateText(filePath).Close();
                }
                else if (! File.Exists(filePath) )
                {
                    File.CreateText(filePath).Close();
                }
                else
                {
                    fileLen = File.ReadAllLines(filePath).Length;
                }
                var position = localPlayer.Transform.position;
                var rotation = localPlayer.Transform.rotation.eulerAngles.y;
                Log.Info($"Custom Spawn Point @ Position: [{position.x}, {position.y}, {position.z}] | Rotation: {rotation} | Location: {localPlayer.Location}");
                var customSpawnPoint = new JObject();
                customSpawnPoint.Add("Comment", localPlayer.Location);
                customSpawnPoint.Add("Position", new JArray(position.x, position.y, position.z) );
                customSpawnPoint.Add("Rotation", rotation);
                customSpawnPoint.Add("Sides", new JArray("") );
                customSpawnPoint.Add("Categories", new JArray("") );
                customSpawnPoint.Add("Infiltration", "");
                customSpawnPoint.Add("DelayToCanSpawnSec", 3);
                customSpawnPoint.Add("ColliderRadius", 0.0);
                File.AppendAllText(filePath, (fileLen > 1 ? $",{Environment.NewLine}{Environment.NewLine}" : "") + customSpawnPoint.ToString());
            }
        }

        /// <summary>
        /// Gets the current <see cref="Player"/> instance if it's available
        /// </summary>
        /// <returns>Local <see cref="Player"/> instance; returns null if the game is not in raid</returns>
        private static Player GetLocalPlayerFromWorld()
        {
            // If the GameWorld instance is null or has no RegisteredPlayers, it most likely means we're not in a raid
            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.RegisteredPlayers == null)
            {
                return null;
            }

            // One of the RegisterdPlayers will have the IsYourPlayer flag set, which will be our own Player instance
            return gameWorld.RegisteredPlayers.Find(p => p.IsYourPlayer);
        }
    }
}