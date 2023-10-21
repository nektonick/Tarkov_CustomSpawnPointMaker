using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Comfort.Common;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using EFT;
using EFT.UI;
using EFT.Communications;
using Aki.Reflection.Utils;

namespace Lua.CustomSpawnPointMaker
{
    [BepInPlugin("com.Lua.CustomSpawnPointMaker", "Lua-CustomSpawnPointMaker", "2.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private readonly static string folderPath = @"./CustomSpawnPoints";
        private readonly static string filePath = $"{folderPath}/{DateTime.Now.ToString("yyyy-MM-dd")}.json";
        private MethodInfo displayMessageNotification;

        internal static ConfigEntry<KeyboardShortcut> KeybindCSPMaker { get; set; }
        private void Start()
        {
            KeybindCSPMaker = Config.Bind<KeyboardShortcut>("Keybind", String.Empty, new KeyboardShortcut(KeyCode.Insert), new ConfigDescription("Insert current player's position into json file as SpawnPoint format\nSaved path: %SPT%/CustomSpawnPoints/"));
        }

        void Update()
        {
            if (KeybindCSPMaker.Value.IsDown())
            {
                var localPlayer = GetLocalPlayerFromWorld();
                if (localPlayer == null)
                {
                    if (GameObject.Find("ErrorScreen"))
                    {
                        PreloaderUI.Instance.CloseErrorScreen();
                    }
                    PreloaderUI.Instance.ShowErrorScreen("Lua-CustomSpawnPointMaker", "You need a spawnpoint outside of the map? :/");
                    return;
                }

                if (displayMessageNotification == null)
                {
                    displayMessageNotification = PatchConstants.EftTypes.Single(x => x.GetMethod("DisplayMessageNotification") != null).GetMethod("DisplayMessageNotification");
                }

                var fileLen = 0;
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    File.CreateText(filePath).Close();
                }
                else if (!File.Exists(filePath))
                {
                    File.CreateText(filePath).Close();
                }
                else
                {
                    fileLen = File.ReadAllLines(filePath).Length;
                }
                var position = localPlayer.Transform.position;
                var rotation = localPlayer.Transform.rotation.eulerAngles.y;
                var txt = $"Lua-CustomSpawnPointMaker:\n--------------------\nPosition: [{position.x}, {position.y}, {position.z}]\nRotation: {rotation}\nLocation: {localPlayer.Location}";
                Logger.LogInfo(txt);
                if (displayMessageNotification != null)
                {
                    displayMessageNotification.Invoke(null, new object[] { txt, ENotificationDurationType.Long, ENotificationIconType.Default, Color.magenta});
                }
                var customSpawnPoint = new JObject();
                customSpawnPoint.Add("Comment", localPlayer.Location);
                customSpawnPoint.Add("Position", new JArray(position.x, position.y, position.z));
                customSpawnPoint.Add("Rotation", rotation);
                customSpawnPoint.Add("Sides", new JArray(""));
                customSpawnPoint.Add("Categories", new JArray(""));
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
        private Player GetLocalPlayerFromWorld()
        {
            // If the GameWorld instance is null or has no RegisteredPlayers, it most likely means we're not in a raid
            var gameWorld = Singleton<GameWorld>.Instance;
            if (gameWorld == null || gameWorld.RegisteredPlayers == null)
            {
                return null;
            }

            // One of the RegisterdPlayers will have the IsYourPlayer flag set, which will be our own Player instance
            return gameWorld.RegisteredPlayers.Find(p => p.IsYourPlayer) as Player;
        }
    }
}
