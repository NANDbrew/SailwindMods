﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweaksAndFixes.Scripts;
using UnityEngine;

namespace TweaksAndFixes.Patches
{
    internal static class PortPatches
    {
		public static MissionSorting missionSorting = MissionSorting.PricePerKM;

        [HarmonyPatch(typeof(Port), "GenerateMissions")]
        public static class GenerateMissionsPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(Port __instance, int page, ref Mission[] ___missions, ref int ___currentMissionCount, ref Port[] ___destinationPorts, GameObject[] ___producedGoodPrefabs)
            {
                if (!Main.enabled) return true;
				___missions = new Mission[5];
				___currentMissionCount = 0;
				int num = page * ___missions.Length;
				List<Mission> list = new List<Mission>();
				foreach (Port port in ___destinationPorts)
				{
					foreach (GameObject gameObject in ___producedGoodPrefabs)
					{
						int prefabIndex = gameObject.GetComponent<SaveablePrefab>().prefabIndex;
						if (port.island.GetDemand(prefabIndex) > 0)
						{
							Good component = gameObject.GetComponent<Good>();
							bool flag = component.nativeRegion != port.region || (port.hubPort && !__instance.hubPort);
							if (component.requiredRepLevel > PlayerReputation.GetRepLevel(port.region))
							{
								flag = false;
							}
							if (Mission.GetDistance(__instance, port) > PlayerReputation.GetMaxDistance(__instance.region))
							{
								flag = false;
							}
							if (flag)
							{
								int demand = port.island.GetDemand(prefabIndex);
								
								int totalPrice = (int)AccessTools.Method(typeof(Port), "GetTotalPrice").Invoke(__instance, new object[] { prefabIndex, port, demand });
								int dueDay = (int)AccessTools.Method(typeof(Port), "GetDueDay").Invoke(__instance, new object[] { port, component });
								Mission item = new Mission(__instance, port, gameObject, demand, totalPrice, 1f, 0, dueDay);
								list.Add(item);
							}
						}
					}
				}
				list.Sort(SortMissions);
				___currentMissionCount = list.Count;
				for (int k = 0; k < ___missions.Length; k++)
				{
					if (k + num < list.Count)
					{
						___missions[k] = list[k + num];
					}
				}
				return false;
            }

			private static int SortMissions(Mission s2, Mission s1)
            {
                switch (missionSorting)
				{
					case MissionSorting.PricePerKM:
						{
							return s1.pricePerKm.CompareTo(s2.pricePerKm);
						}
					case MissionSorting.TotalPrice:
						{
							return s1.totalPrice.CompareTo(s2.totalPrice);
						}
					case MissionSorting.GoodCount:
						{
							return s1.goodCount.CompareTo(s2.goodCount);
						}
					case MissionSorting.Distance:
						{
							return s1.distance.CompareTo(s2.distance);
						}
					default:
						{
							return s1.pricePerKm.CompareTo(s2.pricePerKm);
						}
                }
            }
        }
    }
}