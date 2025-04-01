using BepInEx;
using BepInEx.Logging;
using System.Collections.Generic;
using UnityEngine;

using WindowsInput;
using WindowsInput.Native;

namespace factory_clankington;

// plugin info
public static class PluginInfo
{
    public const string PLUGIN_GUID = "org.tangopia.factory_clankington";
    public const string PLUGIN_NAME = "Factory Clankington";
    public const string PLUGIN_VERSION = "0.0.1.10003";
}

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private bool game_on = false;
    private GameObject timerPanel;
    private InputSimulator inputSimulator = new InputSimulator();

    private Dictionary<string, string> operable_objects_names = new Dictionary<string, string>
    {
        { "egg_dispenser", "DLC08_dispenser_crate_circus" },
        { "chocolate_dispenser", "DLC08_dispenser_crate_circus (1)" },
        { "cranberry_dispenser", "DLC08_dispenser_crate_circus (2)" },
        { "bun_dispenser", "DLC08_dispenser_crate_circus (3)" },
        { "onion_dispenser", "DLC08_dispenser_crate_circus (4)" },
        { "sausage_dispenser", "DLC08_dispenser_crate_circus (5)" },
        { "flour_dispenser", "DLC08_dispenser_crate_circus (7)" },
        { "condiment_dispenser", "DLC08_condiment_dispenser" },
        { "condiment_button", "p_dlc08_button_Condiments" },
    };
    private Dictionary<string, GameObject> operable_objects = new Dictionary<string, GameObject> { };
    private Dictionary<string, string> chef_names = new Dictionary<string, string>
    {
        { "player_1", "Player 1" },
        { "player_2", "Player 2" },
    };
    private string controlling_chef_name = "player_2";
    private Dictionary<string, GameObject> chefs = new Dictionary<string, GameObject> { };
    private GameObject controlling_chef;

    private Dictionary<string, Dictionary<string, Vector3>> dispensers = new Dictionary<string, Dictionary<string, Vector3>>
    {
        { "EggDispenser", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(30.0f, 0.0f, -10.8f) },
                { "ApproachablePosition", new Vector3(30.0f, 0.0f, -11.8f) },
                { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) }, 
            }
        },
        { "ChocolateDispenser", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(27.6f, 0.0f, -10.8f) },
                { "ApproachablePosition", new Vector3(27.6f, 0.0f, -11.8f) },
                { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
            }
        },
        { "CranberryDispenser", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(26.4f, 0.0f, -10.8f) },
                { "ApproachablePosition", new Vector3(26.4f, 0.0f, -11.8f) },
                { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
            }
        },
        { "BunDispenser", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(10.8f, 0.0f, -10.8f) },
                { "ApproachablePosition", new Vector3(10.8f, 0.0f, -11.8f) },
                { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
            }
        },
        { "OnionDispenser", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(13.2f, 0.0f, -10.8f) },
                { "ApproachablePosition", new Vector3(13.2f, 0.0f, -11.8f) },
                { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
            }
        },
        { "SausageDispenser", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(14.4f, 0.0f, -10.8f) },
                { "ApproachablePosition", new Vector3(14.4f, 0.0f, -11.8f) },
                { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
            }
        },
        { "FlourDispenser", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(31.2f, 0.0f, -10.8f) },
                { "ApproachablePosition", new Vector3(31.2f, 0.0f, -11.8f) },
                { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
            }
        },
        { "CondimentDispenser", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(20.4f, 0.0f, -10.8f) },
                { "ApproachablePosition", new Vector3(20.4f, 0.0f, -11.8f) },
                { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
            }
        },
        { "CondimentButton", new Dictionary<string, Vector3>
            {
                { "Position", new Vector3(20.4f, 0.0f, -15.6f) },
                { "ApproachablePosition", new Vector3(20.4f, 0.0f, -14.6f) },
                { "EulerAngles", new Vector3(0.0f, 180.0f, 0.0f) },
            }
        }
    };


    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

        InvokeRepeating("main_loop", 0.0f, 0.2f);
        InvokeRepeating("assembly_line", 0.0f, 1f);
    }

    private int status = 0;
    private void assembly_line()
    {
        chef_go_to("CranberryDispenser");

        chef_grab();

        chef_go_to("ChocolateDispenser");
    }
    
    private void main_loop()
    {
        if (timerPanel == null)
        {
            if (game_on == true)
            {
                Logger.LogInfo("Game off.");
            }
            game_on = false;

            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.Equals("LevelUITimerPanel"))
                {
                    timerPanel = obj;
                }
            }

            return;
        }

        UnityEngine.UI.Text textComponent = timerPanel.GetComponentInChildren<UnityEngine.UI.Text>();
        if (textComponent.text.Equals("04:29"))
        {
            if (game_on == false)
            {
                game_on = true;
                Logger.LogInfo("Game on.");

                InitializeObjects();
            }
        }
        else if (textComponent.text.Equals("00:00"))
        {
            if (game_on == true)
            {
                game_on = false;
                Logger.LogInfo("Game off.");
            }
        }
    }

    private void InitializeObjects()
    {
        foreach (KeyValuePair<string, string> entry in operable_objects_names)
        {
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.Equals(entry.Value))
                {
                    operable_objects.Add(entry.Key, obj);
                    break;
                }
            }
        }

        chefs.Clear();
        foreach (KeyValuePair<string, string> entry in chef_names)
        {
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.Equals(entry.Value))
                {
                    chefs.Add(entry.Key, obj);
                    break;
                }
            }
        }
        controlling_chef = chefs[controlling_chef_name];
    }

    private void chef_go_to(GameObject destination_object)
    {
        Vector3 destination = destination_object.transform.position;
        controlling_chef.transform.position = destination;
    }

    private void chef_go_to(Vector3 destination)
    {
        controlling_chef.transform.position = destination;
    }

    private void chef_go_to(Dictionary<string, Vector3> destination)
    {
        controlling_chef.transform.position = destination["ApproachablePosition"];
        controlling_chef.transform.eulerAngles = destination["EulerAngles"];
    }

    private void chef_go_to(string destination)
    {
        chef_go_to(dispensers[destination]);
    }

    private void chef_grab()
    {
        inputSimulator.Keyboard.KeyPress(VirtualKeyCode.OEM_PERIOD);
    }

}
