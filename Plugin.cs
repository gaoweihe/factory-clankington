using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System;
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
public partial class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private bool game_on = false;
    private GameObject timerPanel;
    private InputSimulator inputSimulator = new InputSimulator();
    private VirtualKeyCode key_pickdrop = VirtualKeyCode.VK_X;
    private VirtualKeyCode key_throwchop = VirtualKeyCode.VK_Z;


    //private Dictionary<string, string> operable_object_names = new Dictionary<string, string>
    //{
    //    // operable_object_positions
    //    { "egg_dispenser", "DLC08_dispenser_crate_circus" },
    //    { "chocolate_dispenser", "DLC08_dispenser_crate_circus (1)" },
    //    { "cranberry_dispenser", "DLC08_dispenser_crate_circus (2)" },
    //    { "bun_dispenser", "DLC08_dispenser_crate_circus (3)" },
    //    { "onion_dispenser", "DLC08_dispenser_crate_circus (4)" },
    //    { "sausage_dispenser", "DLC08_dispenser_crate_circus (5)" },
    //    { "flour_dispenser", "DLC08_dispenser_crate_circus (7)" },
    //    { "condiment_dispenser", "DLC08_condiment_dispenser" },
    //    { "condiment_button", "p_dlc08_button_Condiments" },
    //    // countertops
    //    // top right
    //    { "tr_countertop_1", "DLC08_countertop_01_chopping_circus (7)" },
    //    { "tr_countertop_2", "DLC08_countertop_01_standard_circus (24)"},
    //    { "tr_countertop_3", "DLC08_countertop_01_chopping_circus (5)" },
    //};

    private Dictionary<string, GameObject> operable_objects = new Dictionary<string, GameObject> { };
    private Dictionary<string, string> chef_names = new Dictionary<string, string>
    {
        { "player_1", "Player 1" },
        { "player_2", "Player 2" },
    };
    private string controlling_chef_name = "player_2";
    private Dictionary<string, GameObject> chefs = new Dictionary<string, GameObject> { };
    private GameObject controlling_chef;

    //private Dictionary<string, Dictionary<string, Vector3>> operable_object_positions = new Dictionary<string, Dictionary<string, Vector3>>
    //{
    //    {
    //        "EggDispenser", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(30.0f, 0.0f, -10.8f) },
    //            { "ApproachablePosition", new Vector3(30.0f, 0.0f, -11.8f) },
    //            { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
    //        }
    //    },
    //    {
    //        "ChocolateDispenser", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(27.6f, 0.0f, -10.8f) },
    //            { "ApproachablePosition", new Vector3(27.6f, 0.0f, -11.8f) },
    //            { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
    //        }
    //    },
    //    {
    //        "CranberryDispenser", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(26.4f, 0.0f, -10.8f) },
    //            { "ApproachablePosition", new Vector3(26.4f, 0.0f, -11.8f) },
    //            { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
    //        }
    //    },
    //    {
    //        "BunDispenser", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(10.8f, 0.0f, -10.8f) },
    //            { "ApproachablePosition", new Vector3(10.8f, 0.0f, -11.8f) },
    //            { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
    //        }
    //    },
    //    {
    //        "OnionDispenser", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(13.2f, 0.0f, -10.8f) },
    //            { "ApproachablePosition", new Vector3(13.2f, 0.0f, -11.8f) },
    //            { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
    //        }
    //    },
    //    {
    //        "SausageDispenser", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(14.4f, 0.0f, -10.8f) },
    //            { "ApproachablePosition", new Vector3(14.4f, 0.0f, -11.8f) },
    //            { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
    //        }
    //    },
    //    {
    //        "FlourDispenser", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(31.2f, 0.0f, -10.8f) },
    //            { "ApproachablePosition", new Vector3(31.2f, 0.0f, -11.8f) },
    //            { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
    //        }
    //    },
    //    {
    //        "CondimentDispenser", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(20.4f, 0.0f, -10.8f) },
    //            { "ApproachablePosition", new Vector3(20.4f, 0.0f, -11.8f) },
    //            { "EulerAngles", new Vector3(0.0f, 0.0f, 0.0f) },
    //        }
    //    },
    //    {
    //        "CondimentButton", new Dictionary<string, Vector3>
    //        {
    //            { "Position", new Vector3(20.4f, 0.0f, -15.6f) },
    //            { "ApproachablePosition", new Vector3(20.4f, 0.0f, -14.6f) },
    //            { "EulerAngles", new Vector3(0.0f, 180.0f, 0.0f) },
    //        }
    //    }
    //};

    //private ConfigEntry<string> magic;
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo("Plugin awake.");
    }

    private void OnEnable()
    {
        Logger.LogInfo("Plugin enabled.");

        init_config();

        InvokeRepeating("main_loop", 0.0f, 0.2f);
        // InvokeRepeating("assembly_line", 0.0f, 5f);
        InvokeRepeating("execute_ops", 0.0f, 0.2f);
    }

    private void init_config()
    {
        //magic = Config.Bind("General", "Magic", "Hello, world!", "Magic words.");
    }

    private int status = 0;
    private Queue<Action> assembly_line_task_queue = new Queue<Action>();
    private void assembly_line()
    {
        if (assembly_line_task_queue.Count == 0)
        {
            assembly_line_donuts();
            assembly_line_hotdogs();
        }
    }

    void Update()
    {

    }

    void execute_ops()
    {
        if (game_on == true)
        {
            if (assembly_line_task_queue.Count > 0)
            {
                Action next_task = assembly_line_task_queue.Dequeue();
                Logger.LogInfo("Next task: " + next_task.Method.Name);
                next_task?.Invoke();
            }
            else
            {
                assembly_line();
            } 
        }
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
            assembly_line_task_queue.Clear();

            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.Equals("LevelUITimerPanel"))
                {
                    timerPanel = obj;
                    break;
                }
            }

        }

        UnityEngine.UI.Text textComponent = timerPanel.GetComponentInChildren<UnityEngine.UI.Text>();
        if (textComponent.text.Equals("04:29"))
        {
            if (game_on == false)
            {
                game_on = true;
                Logger.LogInfo("Game on.");
                assembly_line(); 
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
        foreach (KeyValuePair<string, FcObject> entry in Maps.factory)
        {
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.Equals(entry.Value.Name))
                {
                    FcObject target_object = entry.Value;
                    target_object.Position = obj.transform.position;
                    target_object.ApproachablePosition = obj.transform.position;
                    // approachable offset 
                    if (target_object.ApproachableDirection[0].Equals("t"))
                    {
                        target_object.ApproachablePosition = new Vector3(target_object.ApproachablePosition.x, target_object.ApproachablePosition.y, target_object.ApproachablePosition.z + 1.0f);
                        target_object.ApproachableEulerAngle = new Vector3(0.0f, 180.0f, 0.0f);
                    }
                    else if (target_object.ApproachableDirection[0].Equals("b"))
                    {
                        target_object.ApproachablePosition = new Vector3(target_object.ApproachablePosition.x, target_object.ApproachablePosition.y, target_object.ApproachablePosition.z - 1.0f);
                        target_object.ApproachableEulerAngle = new Vector3(0.0f, 0.0f, 0.0f);
                    }
                    else if (target_object.ApproachableDirection[0].Equals("l"))
                    {
                        target_object.ApproachablePosition = new Vector3(target_object.ApproachablePosition.x - 1.0f, target_object.ApproachablePosition.y, target_object.ApproachablePosition.z);
                        target_object.ApproachableEulerAngle = new Vector3(0.0f, 90.0f, 0.0f);
                    }
                    else if (target_object.ApproachableDirection[0].Equals("r"))
                    {
                        target_object.ApproachablePosition = new Vector3(target_object.ApproachablePosition.x + 1.0f, target_object.ApproachablePosition.y, target_object.ApproachablePosition.z);
                        target_object.ApproachableEulerAngle = new Vector3(0.0f, 270.0f, 0.0f);
                    }

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

    private void chef_go_to(Vector3 position, Vector3 eulerAngles)
    {
        controlling_chef.transform.position = position;
        controlling_chef.transform.eulerAngles = eulerAngles;
    }

    private void chef_go_to(FcObject destination)
    {
        controlling_chef.transform.position = destination.ApproachablePosition;
        controlling_chef.transform.eulerAngles = destination.ApproachableEulerAngle;
    }

    private void chef_go_to(string destination)
    {
        assembly_line_task_queue.Enqueue(() => chef_go_to(Maps.factory[destination]));
    }

    private void chef_pickdrop()
    {
        assembly_line_task_queue.Enqueue(() => inputSimulator.Keyboard.KeyPress(key_pickdrop));
    }

    private void chef_turnto(string direction)
    {
        Vector3 target_eulerAngles = new Vector3(0, 0, 0);

        if (direction.Equals("t"))
        {
            target_eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
        else if (direction.Equals("b"))
        {
            target_eulerAngles = new Vector3(0.0f, 180.0f, 0.0f);
        }
        else if (direction.Equals("l"))
        {
            target_eulerAngles = new Vector3(0.0f, 270.0f, 0.0f);
        }
        else if (direction.Equals("r"))
        {
            target_eulerAngles = new Vector3(0.0f, 90.0f, 0.0f);
        }

        assembly_line_task_queue.Enqueue(() =>
        {
            controlling_chef.transform.eulerAngles = target_eulerAngles;
        }); 

        }

    private void chef_throwchop()
    {
        assembly_line_task_queue.Enqueue(() => inputSimulator.Keyboard.KeyPress(key_throwchop));
    }

    private void chef_goget(string target)
    {
        chef_go_to(target);
        chef_pickdrop();
    }

    private void chef_puton(string target)
    {
        chef_go_to(target);
        chef_pickdrop();
    }

    private void chef_throwtowards(string direction)
    {
        chef_turnto(direction);
        chef_throwchop();
    }

    private void chef_goget_puton(string get_target, string put_target)
    {
        chef_goget(get_target);
        chef_puton(put_target);
    }

    private void chef_goget_throwtowards(string get_target, string direction)
    {
        chef_goget(get_target);
        chef_throwtowards(direction);
    }

    private void assembly_line_donuts()
    {
        chef_goget_puton("cranberry_dispenser", "tr_countertop_3");
        chef_goget_puton("chocolate_dispenser", "tr_countertop_2");

        chef_goget("flour_dispenser");
        chef_go_to("egg_dispenser");
        chef_throwtowards("l");

        chef_goget_throwtowards("egg_dispenser", "l");

        chef_goget("flour_dispenser");
        chef_go_to("egg_dispenser");
        chef_throwtowards("l");

        chef_goget_throwtowards("egg_dispenser", "l");
    }

    private void assembly_line_hotdogs()
    {
        chef_goget_throwtowards("sausage_dispenser", "r");
        chef_goget_throwtowards("sausage_dispenser", "r");
    }

}
