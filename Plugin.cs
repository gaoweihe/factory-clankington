using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

using WindowsInput;
using WindowsInput.Native;
using HarmonyLib;

namespace factory_clankington;

// plugin info
public static class PluginInfo
{
    public const string PLUGIN_GUID = "org.tangopia.factory_clankington";
    public const string PLUGIN_NAME = "Factory Clankington";
    public const string PLUGIN_VERSION = "0.0.1.10003";
}

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public partial class FcPlugin : BaseUnityPlugin
{
    public static event Action<List<string>> OnNewOrderAdded;
    public static event Action<string> OnNewLevelInitialized;

    public static void RaiseNewOrderAdded(List<string> orderName)
    {
        OnNewOrderAdded?.Invoke(orderName);
    }

    public static void RaiseLevelInitialized(string levelName)
    {
        OnNewLevelInitialized?.Invoke(levelName);
    }

    private void HandleNewOrderAdded(List<string> orderName)
    {
        active_orders = orderName;
        foreach (string order in active_orders)
        {
            Logger.LogInfo($"Active Order: {order}");
        }
    }

    private void HandleNewLevelInitialized(string levelName)
    {
        current_level_name = levelName;
        Logger.LogInfo($"New level initialized: {levelName}");
    }

    private List<string> active_orders;
    private List<string> present_ingredients;

    private List<GameObject> pots;
    private List<GameObject> pans; 

    internal static new ManualLogSource Logger;

    private bool game_on = false;
    private GameObject timerPanel;
    private InputSimulator inputSimulator = new InputSimulator();
    private VirtualKeyCode key_pickdrop = VirtualKeyCode.VK_X;
    private VirtualKeyCode key_throwchop = VirtualKeyCode.VK_Z;

    private string current_level_name; 

    private Dictionary<string, string> chef_names = new Dictionary<string, string>
    {
        { "player_1", "Player 1" },
        { "player_2", "Player 2" },
    };
    private string controlling_chef_name = "player_2";
    private Dictionary<string, GameObject> chefs = new Dictionary<string, GameObject> { };
    private GameObject controlling_chef;

    private ConfigEntry<bool> configPluginEnabled;
    private ConfigEntry<string> configLevelName;

    private static Harmony _harmony;

    //private ConfigEntry<string> magic;
    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo("FcPlugin awake.");

        try
        {
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Harmony patching failed: {ex}");
        }
        
        // events
        FcPlugin.OnNewOrderAdded += HandleNewOrderAdded;
        FcPlugin.OnNewLevelInitialized += HandleNewLevelInitialized;
    }

    private void OnEnable()
    {
        Logger.LogInfo("FcPlugin enabled.");

        init_config();

        InvokeRepeating("main_loop", 0.0f, 0.2f);
    }

    private void OnDestroy()
    {
        _harmony.UnpatchSelf();
    }

    private void init_config()
    {
        configPluginEnabled = Config.Bind("General",
                                 "PluginEnabled",
                                 true,
                                 "Plugin Enabled. ");

        configLevelName = Config.Bind("General",
                                 "LevelName",
                                 "Day_3_4_2P",
                                 "Level Name. ");
    }

    private Queue<Action> assembly_line_task_queue = new Queue<Action>();
    private void assembly_line()
    {
        if (assembly_line_task_queue.Count == 0)
        {
            assembly_line_donuts();
            assembly_line_sausages();
            assembly_line_onions();
            assembly_line_buns();
            assembly_line_sausages();
            assembly_line_buns();
        }
    }

    void Update()
    {

    }

    private bool execute_ops_result = true; 
    void execute_tasks()
    {
        if (game_on == true)
        {
            if (assembly_line_task_queue.Count > 0)
            {
                Action next_task = assembly_line_task_queue.Peek();
                //Logger.LogInfo("Next task: " + next_task.Method.Name);
                next_task?.Invoke();

                if (execute_ops_result == true)
                {
                    assembly_line_task_queue.Dequeue();
                }
            }
            else
            {
                assembly_line();
            } 
        }
    }

    private void OnGameOff()
    {
        assembly_line_task_queue.Clear();
        timerPanel = null;
        Logger.LogInfo("Game off.");
    }

    private void OnGameOn() 
    {
        Logger.LogInfo("Game on.");
        init_objects();
    }

    private void main_loop()
    {
        if (configPluginEnabled.Value == false)
        {
            return;
        }

        if (timerPanel == null)
        {
            if (game_on == true)
            {
                game_on = false;
                OnGameOff();
            }

            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.Equals("LevelUITimerPanel"))
                {
                    timerPanel = obj;
                    break;
                }
            }

        }
        else
        {
            UnityEngine.UI.Text textComponent = timerPanel.GetComponentInChildren<UnityEngine.UI.Text>();
            if (textComponent.text.Equals("04:29"))
            {
                if (game_on == false)
                {
                    game_on = true;
                    OnGameOn();
                }
            }
            else if (textComponent.text.Equals("00:00"))
            {
                if (game_on == true)
                {
                    game_on = false;
                    OnGameOff();
                }
            }

            if (game_on == true)
            {
                execute_tasks(); 
            }
        }
        
    }

    private void init_objects()
    {
        // work stations       
        foreach (KeyValuePair<string, FcObject> entry in Maps.factory)
        {
            foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (obj.name.Equals(entry.Value.Name))
                {
                    FcObject target_object = entry.Value;
                    target_object.Position = obj.transform.position;
                    target_object.InteractablePosition = obj.transform.position;
                    // approachable offset 
                    if (target_object.InteractableDirection[0].Equals("t"))
                    {
                        target_object.InteractablePosition = new Vector3(
                            target_object.InteractablePosition.x,
                            target_object.InteractablePosition.y,
                            target_object.InteractablePosition.z + 1.0f
                            );
                        target_object.InteractableEulerAngle = new Vector3(0.0f, 180.0f, 0.0f);
                    }
                    else if (target_object.InteractableDirection[0].Equals("b"))
                    {
                        target_object.InteractablePosition = new Vector3(
                            target_object.InteractablePosition.x,
                            target_object.InteractablePosition.y,
                            target_object.InteractablePosition.z - 1.0f
                            );
                        target_object.InteractableEulerAngle = new Vector3(0.0f, 0.0f, 0.0f);
                    }
                    else if (target_object.InteractableDirection[0].Equals("l"))
                    {
                        target_object.InteractablePosition = new Vector3(
                            target_object.InteractablePosition.x - 1.0f,
                            target_object.InteractablePosition.y,
                            target_object.InteractablePosition.z
                            );
                        target_object.InteractableEulerAngle = new Vector3(0.0f, 90.0f, 0.0f);
                    }
                    else if (target_object.InteractableDirection[0].Equals("r"))
                    {
                        target_object.InteractablePosition = new Vector3(
                            target_object.InteractablePosition.x + 1.0f,
                            target_object.InteractablePosition.y,
                            target_object.InteractablePosition.z
                            );
                        target_object.InteractableEulerAngle = new Vector3(0.0f, 270.0f, 0.0f);
                    }

                    break;
                }
            }
        }

        // chefs 
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

        // utensils
        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
        {
            if (obj.name.Equals("DLC08_utensil_pot_01"))
            {
                pots.Add(obj);
                Logger.LogInfo($"Pot: {obj.name}");
            }
            //else if (obj.name.Equals("DLC08_utensil_pan_01"))
            //{
            //    pans.Add(obj);
            //}
        }
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
        controlling_chef.transform.position = destination.InteractablePosition;
        controlling_chef.transform.eulerAngles = destination.InteractableEulerAngle;
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

    private GameObject get_object_by_alias(string alias)
    {
        string object_name = Maps.factory[alias].Name;
        GameObject target = GameObject.Find(object_name);
        return target; 
    }

    private string get_objectName_by_alias(string alias)
    {
        string object_name = Maps.factory[alias].Name;
        return object_name;
    }

    private bool check_countertop_clear(AttachStation target_attachStation)
    {
        Transform attachPoint = target_attachStation.m_attachPoint; 
        int childCount = attachPoint?.transform.childCount ?? 0;
        if (childCount == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool check_countertop_clear(string target_alias)
    {
        string objectName = get_objectName_by_alias(target_alias);
        AttachStation target_attachStation = GameObject.Find(objectName).GetComponent<AttachStation>();

        bool result = check_countertop_clear(target_attachStation);
        return result; 
    }

    // TODO: implement
    private void chef_wait_countertop(string target_alias, string target_status)
    {
        assembly_line_task_queue.Enqueue(() => {

            if (target_status.Equals("empty"))
            {
                bool empty = check_countertop_clear(target_alias);
                if (empty == true)
                {
                    execute_ops_result = true;
                }
                else
                {
                    execute_ops_result = false;
                }
            }
            else if (target_status.Equals("engaged"))
            {
                bool empty = check_countertop_clear(target_alias);
                if (empty == false)
                {
                    // TODO: check engaged object type
                    execute_ops_result = true;
                }
                else
                {
                    execute_ops_result = false;
                }
            }
            else
            {
                execute_ops_result = false;
            }

        });
    }

    // TODO: implement
    private bool check_pot_ready(AttachStation target_attachStation)
    {
        Transform attachPoint = target_attachStation.m_attachPoint;
        int childCount = attachPoint?.transform.childCount ?? 0;
        if (childCount == 0)
        {
            return false;
        }

        GameObject pot_gameobject = attachPoint.transform.GetChild(0).gameObject;
        GameObject soup_gameobject = pot_gameobject.transform.Find("Content/Soup").gameObject;
        bool is_soup_active = soup_gameobject.activeSelf;
        bool is_pot_empty = !is_soup_active;

        return is_pot_empty; 
    }

    // TODO: implement
    private bool check_pot_ready(string target_alias)
    {
        string objectName = get_objectName_by_alias(target_alias);
        AttachStation target_attachStation = GameObject.Find(objectName).GetComponent<AttachStation>();
        bool result = check_pot_ready(target_attachStation);
        return result;
    }

    private bool check_pot_engaged(string target_alias)
    {
        return false; 
    }

    private void chef_wait_pot(string target_alias, string target_status)
    {
        assembly_line_task_queue.Enqueue(() => {

            if (target_status.Equals("empty"))
            {
                bool empty = check_countertop_clear(target_alias);
                if (empty == true)
                {
                    execute_ops_result = true;
                }
                else
                {
                    execute_ops_result = false;
                }
            }
            else if (target_status.Equals("engaged"))
            {
                bool empty = check_countertop_clear(target_alias);
                if (empty == false)
                {
                    // TODO: check engaged object type
                    execute_ops_result = true;
                }
                else
                {
                    execute_ops_result = false;
                }
            }
            else
            {
                execute_ops_result = false;
            }

        });
    }
    
    private void assembly_line_donuts()
    {
        chef_wait_countertop("tr_countertop_3", "empty");
        chef_goget_puton("cranberry_dispenser", "tr_countertop_3");
        chef_wait_countertop("tr_countertop_2", "empty");
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

    private void assembly_line_sausages()
    {
        chef_wait_countertop("tl_countertop_1", "empty");
        check_pot_ready("tl_stove_1");
        chef_goget_throwtowards("sausage_dispenser", "r");
        
        chef_wait_countertop("tl_countertop_1", "empty");
        check_pot_ready("tl_stove_2");
        chef_goget_throwtowards("sausage_dispenser", "r");
    }

    private void assembly_line_onions()
    {
        chef_wait_countertop("tl_countertop_1", "empty");
        chef_goget_puton("onion_dispenser", "tl_countertop_1");
        chef_throwchop();

        chef_wait_countertop("tl_countertop_1", "empty");
        chef_goget_puton("onion_dispenser", "tl_countertop_1");
        chef_throwchop();
    }

    private void assembly_line_buns()
    {
        chef_wait_countertop("tl_countertop_1", "empty");
        chef_goget_puton("bun_dispenser", "tl_countertop_1");
        chef_throwchop();

        chef_wait_countertop("tl_countertop_1", "empty");
        chef_goget_puton("bun_dispenser", "tl_countertop_1");
        chef_throwchop();
    }

    private void assembly_line_serve()
    {

    }

    private void assembly_line_serve(int number_orders)
    {
        for (int i = 0; i < number_orders; i++)
        {
            assembly_line_serve();
        }
    }
}
