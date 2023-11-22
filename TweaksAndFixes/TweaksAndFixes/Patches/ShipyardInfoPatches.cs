using HarmonyLib;
using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.MonoBehaviourScripts;
using UnityEngine;
using UnityModManagerNet;
using Log = UnityModManagerNet.UnityModManager.Logger;

namespace TweaksAndFixes.Patches
{
    internal static class ShipyardInfoPatches
    {
        private static int category;

        private class SailInfo : MonoBehaviour
        {
            public float GetSailMass(Sail sail)
            {
                float num = sail.GetRealSailPower() * 20f;
                float num2;
                if (sail.category == SailCategory.junk || sail.category == SailCategory.gaff)
                {
                    num2 = sail.GetRealSailPower() * 20f;
                }
                else if (sail.category == SailCategory.staysail)
                {
                    num2 = 0f;
                }
                else
                {
                    num2 = sail.GetRealSailPower() * 10f;
                }
                return num + num2;
            }
        }

        [HarmonyPatch(typeof(ShipyardUI), "ChangeMenuCategory")]
        private static class ShipyardChangeMenu
        {
            [HarmonyPostfix]
            private static void Postfix(ShipyardUI __instance, object[] __args)
            {
                if (Main.enabled)
                {

                    if (__args.Length > 0)
                    {
                        category = (int)__args[0];
                    }
                    __instance.UpdateDescriptionText();
                }
            }
        }

        [HarmonyPatch(typeof(ShipyardUI), "UpdateDescriptionText")]
        private static class ShipyardUIStartPatch
        {
            [HarmonyPostfix]
            public static void Postfix(TextMesh ___descText)
            {
                if (Main.enabled)
                {
                    Sail currentSail = GameState.currentShipyard.sailInstaller.GetCurrentSail();

                    if ((bool)currentSail)
                    {
                        SailInfo sailInfo = new SailInfo();
                        string mass = Mathf.RoundToInt(sailInfo.GetSailMass(currentSail)).ToString();

                        List<string> texts = new List<string>(___descText.text.Split('\n'));
                        texts.Insert(2, "weight: " + mass);
                        ___descText.text = string.Join("\n", texts);
                    }

                    if (category > -1)
                    {
                        BoatPartsOrder currentOrder = GameState.currentShipyard.partsInstaller.GetCurrentOrder();
                        BoatCustomParts currentParts = GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatCustomParts>();
                        int numLines = 0;
                        string text = "Weight: ";

                        for (int i = 0; i < currentParts.availableParts.Count; i++)
                        {
                            int thisPartMass = Mathf.RoundToInt((float)currentParts.availableParts[i].partOptions[currentOrder.orderedOptions[i]].mass);
                            if (currentParts.availableParts[i].partOptions[currentOrder.orderedOptions[i]].optionName.Contains("stay"))
                            {
                                currentParts.availableParts[i].category = 2;
                            }
                            if (currentParts.availableParts[i].category == category && thisPartMass > 0)
                            {
                                text = text + "\n" + currentParts.availableParts[i].partOptions[currentOrder.orderedOptions[i]].optionName + ": " + thisPartMass;
                                numLines++;
                            }
                        }
                        if (numLines > 0)
                        {
                            ___descText.GetComponent<TextMesh>().characterSize = numLines > 5 ? 0.2f - (0.015f * (numLines % 5)) : 0.2f;

                            ___descText.text = text;
                        }
                    }
                }
            }
        }
/*
        [HarmonyPatch(typeof(ShipyardUI), "RefreshPartsPanel")]
        private static class ShipyardPartCategoryPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ShipyardUI __instance, ref int category, ref bool freshStart)
            {
                BoatCustomParts component = GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatCustomParts>();
                BoatPartsOrder currentOrder = GameState.currentShipyard.partsInstaller.GetCurrentOrder();
                if (component)
                {
                    int num = 0;
                    for (int i = 0; i < __instance.GetPrivateField<TextMesh[]>("partOptionsTexts").Length; i++)
                    {
                        for (int j = num; j < component.availableParts.Count; j++)
                        {
                            if (component.availableParts[j].partOptions[j].optionName.Contains("stay"))
                            {
                                component.availableParts[j].category = 1;
                            }
                            num++;
                        }
                    }
                }
                return true;
            }
        }*/
        /*        [HarmonyPatch(typeof(ShipyardUI), "LateUpdate")]
                private static class ShipyardUIMenuPatch
                {
                    [HarmonyPrefix]
                    public static bool Prefix(ShipyardUI __instance)
                    {
                        if (!Main.enabled) return true;

                        __instance.InvokePrivateMethod("UpdateDescriptionText");
                        return true;
                    }
                }*/

/*        [HarmonyPatch(typeof(ShipyardUIOrderText), "SetTotalLine")]
        private static class ShipyardUIOrderTextPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ShipyardUIOrderText __instance)
            {
                if (Main.enabled)
                {
                    GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatMass>().UpdatePartsMass();
                    BoatPartsOrder currentOrder = GameState.currentShipyard.partsInstaller.GetCurrentOrder();
                    BoatCustomParts currentParts = GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatCustomParts>();



                    float partsMass = 0f;
                    for (int i = 0; i < currentParts.availableParts.Count; i++)
                    {
                        partsMass += Mathf.RoundToInt((float)currentParts.availableParts[i].partOptions[currentOrder.orderedOptions[i]].mass);
                    }


                    int oldMass = Mathf.RoundToInt(GameState.currentShipyard.GetCurrentBoat().GetComponent<BoatMass>().GetPrivateField<float>("partsMass"));
                    int newMass = Mathf.RoundToInt(partsMass);
                    //shipyardOrderText.AddLine("hull mass: " + Mathf.RoundToInt(this.GetCurrentBoat().GetComponent<BoatMass>().selfMass));
                    //___orderText.text = GameState.currentShipyard.GetCurrentOrderText() + newText;
                    __instance.AddLine("---------------------");
                    __instance.AddLine("Parts weight: " + oldMass + " -> " + newMass);
                }
            }
        }*/
    }
}
