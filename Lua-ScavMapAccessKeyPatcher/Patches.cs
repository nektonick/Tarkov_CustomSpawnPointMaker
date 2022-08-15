using EFT;
using EFT.UI;
using UnityEngine;
using System;
using System.Reflection;
using Newtonsoft.Json;
using Aki.Common.Http;
using Aki.Reflection.Patching;

namespace Lua.ScavMapAccessKeyPatcher
{
    class Patches : ModulePatch
    {
        private static string[] itemKey;
        private static ESideType side;
        private static Locations mapConfig;

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_39", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        [PatchPrefix]
        private static void PrefixPatch(RaidSettings ___raidSettings_0)
        {
            if (mapConfig == null)
            {
                var json = RequestHandler.GetJson("/Lua/ScavMapAccessKeyPatcher/config");
                mapConfig = JsonConvert.DeserializeObject<Locations>(json);
            }

            side = ___raidSettings_0.Side;
            itemKey = ___raidSettings_0.SelectedLocation.AccessKeys;
            if (___raidSettings_0.Side != ESideType.Pmc)
            {
                string map = ___raidSettings_0.SelectedLocation.Id.ToLower();
                string scavAccessKey = GetMapAccessKey(map);
                string[] accessKeys = scavAccessKey == null || scavAccessKey.Length == 0 ? Array.Empty<string>() : new string[] { scavAccessKey };

                ___raidSettings_0.Side = ESideType.Pmc;
                ___raidSettings_0.SelectedLocation.AccessKeys = accessKeys;

                if (map == "laboratory")
                {
                    if (GameObject.Find("ErrorScreen"))
                    {
                        PreloaderUI.Instance.CloseErrorScreen();
                    }
                    PreloaderUI.Instance.ShowErrorScreen("Lua-ScavMapAccessKeyPatcher", "NO EXIT WARNING!\n==========================================\nLaboratory is not meant to be played as Scav,\nYou will lost your gears and loots.");
                }
            }
        }
        [PatchPostfix]
        private static void PostfixPatch(RaidSettings ___raidSettings_0)
        {
            ___raidSettings_0.Side = side;
            ___raidSettings_0.SelectedLocation.AccessKeys = itemKey;
        }
        public static string GetMapAccessKey(string map)
        {
            string accessKey = string.Empty;

            switch(map)
            {
                case "bigmap":
                    {
                        accessKey = mapConfig.bigmap;
                        break;
                    }
                case "factory4_day":
                    {
                        accessKey = mapConfig.factory4_day;
                        break;
                    }
                case "factory4_night":
                    {
                        accessKey = mapConfig.factory4_night;
                        break;
                    }
                case "interchange":
                    {
                        accessKey = mapConfig.interchange;
                        break;
                    }
                case "laboratory":
                    {
                        accessKey = mapConfig.laboratory;
                        break;
                    }
                case "lighthouse":
                    {
                        accessKey = mapConfig.lighthouse;
                        break;
                    }
                case "rezervbase":
                    {
                        accessKey = mapConfig.rezervbase;
                        break;
                    }
                case "shoreline":
                    {
                        accessKey = mapConfig.shoreline;
                        break;
                    }
                case "woods":
                    {
                        accessKey = mapConfig.woods;
                        break;
                    }
            }

            return accessKey;
        }
        public class Locations
        {
            public string bigmap { get; set; }
            public string factory4_day { get; set; }
            public string factory4_night { get; set; }
            public string interchange { get; set; }
            public string laboratory { get; set; }
            public string lighthouse { get; set; }
            public string rezervbase { get; set; }
            public string shoreline { get; set; }
            public string woods { get; set; }
        }
    }
}
