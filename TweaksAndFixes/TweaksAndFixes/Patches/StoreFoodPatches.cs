using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TweaksAndFixes.Patches
{
    internal static class StoreFoodPatches
    {
        private static readonly Dictionary<string, float> crateSizes = new Dictionary<string, float>()
        {
            {"firewood", 12f },
            {"fishing hooks", 20f },
            {"white tobacco", 6f },
            {"black tobacco", 6f },
            {"brown tobacco", 6f },
            {"green tobacco", 6f },
        };

        [HarmonyPatch(typeof(ShipItem), "OnItemClick")]
        private static class OnItemClickPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ShipItem __instance, ShipItem heldItem, ref bool __result)
            {
                if (!Main.enabled) return true;
                if (__instance is ShipItemCrate itemCrate)
                {
                    var thisPrefabIndex = heldItem.gameObject.GetComponent<SaveablePrefab>().prefabIndex;
                    var crateItemPrefabIndex = itemCrate.GetContainedPrefab().GetComponent<SaveablePrefab>().prefabIndex;

                    Good crateGood = __instance.GetPrivateField<Good>("goodC");
                    if (crateGood && crateGood.GetMissionIndex() > -1) return true;
                    if (thisPrefabIndex == crateItemPrefabIndex)
                    {
                        float maxAmount = 99;
                        if (crateGood)
                        {
                            maxAmount = PrefabsDirectory.instance.directory[itemCrate.GetPrivateField<Good>("goodC").GetComponent<SaveablePrefab>().prefabIndex].GetComponent<ShipItemCrate>().amount;
                        }
                        else crateSizes.TryGetValue(itemCrate.name, out maxAmount);

                        if (itemCrate.smokedFood && (heldItem.amount < 1f || heldItem.amount > 1.5f))
                        {
                            return true;
                        }
                        if (itemCrate.amount < maxAmount)
                        {
                            itemCrate.amount++;
                            heldItem.DestroyItem();
                            UISoundPlayer.instance.PlayUISound(UISounds.itemPickup, 0.8f, 0.5f);

                            return false;
                        }
                    }
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ShipItemCrate), "UpdateLookText")]
        private static class UpdateLookTextPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ShipItemCrate __instance)
            {
                if (!Main.enabled) return true;
                Good crateGood = __instance.GetPrivateField<Good>("goodC");
                if (crateGood && crateGood.GetMissionIndex() > -1) return true;
                if (__instance.sold)
                {
                    float maxAmount = 99;
                    if (crateGood) maxAmount = PrefabsDirectory.instance.directory[__instance.GetPrivateField<Good>("goodC").GetComponent<SaveablePrefab>().prefabIndex].GetComponent<ShipItemCrate>().amount;
                    else crateSizes.TryGetValue(__instance.name, out maxAmount);
                    string text = __instance.name;
                    if (__instance.smokedFood) text += " (smoked)";

                    text += "\n" + __instance.amount + " / " + maxAmount;
                    __instance.lookText = text;
                    return false;
                }
                return true;
            }
        }
    }
}
