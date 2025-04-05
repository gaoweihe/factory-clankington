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
        static void Postfix(ref ServerOrderControllerBase __instance, ref ServerOrderData __result, RecipeList.Entry _entry)
        {
            List<ServerOrderData> active_orders = AccessTools.Field(__instance.GetType(), "m_activeOrders").GetValue(__instance) as List<ServerOrderData>;
            List<string> order_name_list = new List<string>();
            foreach (ServerOrderData curr_order in active_orders)
            {
                order_name_list.AddItem(curr_order.RecipeListEntry.m_order.name);
                
            }
            FcPlugin.RaiseNewOrderAdded(order_name_list);
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
