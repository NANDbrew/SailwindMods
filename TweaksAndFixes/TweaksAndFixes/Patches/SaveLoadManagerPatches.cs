﻿/*using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TweaksAndFixes.Patches
{
    internal static class SaveLoadManagerPatches
    {
        [HarmonyPatch(typeof(SaveLoadManager), "SaveModData")]
        private static class SaveModDataPatch
        {
            [HarmonyPrefix]
            public static void Postfix()
            {
                if (Main.enabled)
                {
                    Main.saveContainer.Save();
                }
                AddPrefabPatch.indexCounter = 0;
                SaveablePrefabPatches.PrepareSaveDataPatch.saveablePrefabs = new Dictionary<SaveablePrefab, int>();
            }
        }

        [HarmonyPatch(typeof(SaveLoadManager), "LoadGame")]
        private static class LoadGamePatch
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                SaveablePrefabPatches.LoadPatch.indexCounter = 0;
                SaveablePrefabPatches.LoadPatch.saveablePrefabs = new Dictionary<int, SaveablePrefab>();
                SaveContainer saveContainer = new SaveContainer();
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream fileStream = File.Open(SaveSlots.GetCurrentSavePath(), FileMode.Open);
                saveContainer = (SaveContainer)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
                if (saveContainer.modData != null)
                {
                    GameState.modData = saveContainer.modData;
                }
                Main.saveContainer = ModSaveContainer.Load<TAFSaveContainer>(Main.mod);
            }

            [HarmonyPostfix]
            public static void Postfix()
            {
                SaveablePrefabPatches.LoadPatch.AssignHookItems();
            }
        }

        [HarmonyPatch(typeof(SaveLoadManager), "DoSaveGame")]
        private static class DoSaveGamePatch
        {
            [HarmonyPrefix]
            public static void Prefix()
            {
                SaveablePrefabPatches.PrepareSaveDataPatch.indexCounter = 0;
                Main.saveContainer.HookItems = new List<HookItemSaveable>();
                Main.saveContainer.InInventoryItems = new List<InInventorySaveable>();
            }
        }

        [HarmonyPatch(typeof(SaveLoadManager), "AddPrefab")]
        private static class AddPrefabPatch
        {
            [HarmonyPrefix]
            public static void Prefix(SaveablePrefab pref)
            {
                indexCounter++;
                SaveablePrefabPatches.PrepareSaveDataPatch.saveablePrefabs.Add(pref, indexCounter);
            }

            public static int indexCounter;
        }

        [HarmonyPatch(typeof(SaveLoadManager), "RemovePrefab")]
        private static class RemovePrefabPatch
        {
            [HarmonyPrefix]
            public static void Prefix(SaveablePrefab pref)
            {
                SaveablePrefabPatches.PrepareSaveDataPatch.saveablePrefabs.Remove(pref);
            }
        }
    }
}
*/