using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BepInEx.Logging;
using HarmonyLib;
using OrderController;
using UnityEngine;

namespace factory_clankington
{
    [HarmonyPatch(typeof(ServerOrderControllerBase), "AddNewOrder", new Type[] { typeof(RecipeList.Entry) })]
    public static class Patch_ServerOrderControllerBase_AddNewOrder
    {
        [HarmonyPostfix]
        static void Postfix(ref ServerOrderData __result, RecipeList.Entry _entry)
        {
            FcPlugin.RaiseNewOrderAdded(_entry.m_order.name); 
        }
    }

    [HarmonyPatch(typeof(RoundData), "InitialiseRound")]
    public static class Patch_RoundData_InitialiseRound
    {
        [HarmonyPostfix]
        static void Postfix(ref RoundData __instance)
        {
            string levelName = string.Empty;

            GameObject[] array = UnityEngine.Object.FindObjectsOfType<GameObject>();
            GameObject[] array2 = array;
            foreach (GameObject gameObject in array2)
            {
                if (!(gameObject.name == "FlowManager"))
                {
                    continue;
                }
                ServerCampaignFlowController component = gameObject.GetComponent<ServerCampaignFlowController>();
                if (component != null)
                {
                    LevelConfigBase levelConfig = component.GetLevelConfig();
                    if (levelConfig != null)
                    {
                        levelName = levelConfig.name;
                    }
                }
            }

            FcPlugin.RaiseLevelInitialized(levelName);
        }
    }
}
