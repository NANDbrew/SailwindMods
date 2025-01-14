﻿using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.MonoBehaviourScripts;
using UnityEngine;

namespace TweaksAndFixes.Patches
{
    internal static class QuitNoSavePatches
    {
        private static GameObject quitWithoutSavingButton;
        private static GameObject quitButton;
        private static Vector3 originalQuitPos;

        [HarmonyPatch(typeof(Sun), "Start")]
        private static class StartPatch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                StartMenu startMenu = GameObject.FindObjectOfType<StartMenu>();
                if (startMenu)
                {
                    GameObject settingsUi = (GameObject)Traverse.Create(startMenu).Field("confirmQuitUI").GetValue();
                    foreach (Transform child in settingsUi.transform)
                    {
                        if (child.name == "button quit")
                        {
                            quitButton = child.gameObject;
                            originalQuitPos = child.localPosition;

                            quitWithoutSavingButton = GameObject.Instantiate(child.gameObject);
                            quitWithoutSavingButton.name = "button quit no save";
                            quitWithoutSavingButton.transform.parent = settingsUi.transform;
                            quitWithoutSavingButton.transform.localPosition = new Vector3(-0.486f, -0.311f, 0.036f);
                            quitWithoutSavingButton.transform.localRotation = Quaternion.Euler(180, 1.366038e-05f, 180);
                            quitWithoutSavingButton.transform.localScale = Vector3.one;
                            GameObject gameObject = quitWithoutSavingButton.GetComponentInChildren<StartMenuButton>().gameObject;
                            GameObject.Destroy(gameObject.GetComponent<StartMenuButton>());
                            GPQuitNoSaveButton quitNoSaveButton = gameObject.AddComponent<GPQuitNoSaveButton>();
                            quitNoSaveButton.startMenu = startMenu;
                            TextMesh textMesh = quitWithoutSavingButton.GetComponentInChildren<TextMesh>();
                            textMesh.text = "Quit No Save";
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(StartMenu), "EnableQuitConfirmMenu")]
        private static class EnableQuitConfirmMenuPatch
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                quitWithoutSavingButton.SetActive(GameState.playing);
                quitButton.SetActive(false);
                quitButton.transform.localPosition = GameState.playing ? new Vector3(0.486f, -0.311f, 0.036f) : originalQuitPos;
                quitButton.SetActive(true);
            }
        }
    }
}
