using System.Collections.Generic;
using UnityEngine;

namespace factory_clankington;

public partial class FcPlugin
{
    [System.Serializable]
    private class FcObject
    {
        public string Alias { get; set; }
        public string Name { get; set; }
        public List<string> InteractableDirection { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 InteractablePosition { get; set; }
        public Vector3 InteractableEulerAngle { get; set; }

        public FcObject(string alias, string name, List<string> approachable_direction)
        {
            Alias = alias;
            Name = name;
            InteractableDirection = approachable_direction;
            Position = new Vector3(0, 0, 0);
            InteractablePosition = new Vector3(0, 0, 0);
            InteractableEulerAngle = new Vector3(0, 0, 0);
        }
    }

    [System.Serializable]
    private class Maps
    {
        public static Dictionary<string, FcObject> factory = new Dictionary<string, FcObject>
        {
            // dispensers
            { 
                "bun_dispenser", 
                new FcObject 
                ( 
                    "bun_dispenser", 
                    "DLC08_dispenser_crate_circus (3)", 
                    new List<string> { "b" }
                ) 
            },
            { 
                "chocolate_dispenser", 
                new FcObject 
                ( 
                    "chocolate_dispenser", 
                    "DLC08_dispenser_crate_circus (1)", 
                    new List<string> { "b" } 
                ) 
            },
            { 
                "condiment_button", 
                new FcObject 
                ( 
                    "condiment_button", 
                    "p_dlc08_button_Condiments", 
                    new List<string> { "t" }
                ) 
            },
            { 
                "condiment_dispenser", 
                new FcObject 
                ( 
                    "condiment_dispenser", 
                    "DLC08_condiment_dispenser", 
                    new List<string> { "b" }
                ) 
            },
            { 
                "cranberry_dispenser", 
                new FcObject 
                ( 
                    "cranberry_dispenser", 
                    "DLC08_dispenser_crate_circus (2)", 
                    new List<string> { "b" }
                ) 
            },
            { 
                "egg_dispenser", 
                new FcObject 
                ( 
                    "egg_dispenser", 
                    "DLC08_dispenser_crate_circus", 
                    new List<string> { "b" }
                ) 
            },
            { 
                "flour_dispenser", 
                new FcObject 
                ( 
                    "flour_dispenser", 
                    "DLC08_dispenser_crate_circus (7)", 
                    new List<string> { "b" }
                ) 
            },
            { 
                "onion_dispenser", 
                new FcObject 
                ( 
                    "onion_dispenser", 
                    "DLC08_dispenser_crate_circus (4)", 
                    new List<string> { "b" }
                ) 
            },
            { 
                "sausage_dispenser", 
                new FcObject 
                ( 
                    "sausage_dispenser", 
                    "DLC08_dispenser_crate_circus (5)", 
                    new List<string> { "b" }
                ) 
            }, 

            // cook stations
            {
                "tl_stove_1",
                new FcObject
                (
                    "tl_stove_1",
                    "workstation_cooker_01 (3)",
                    new List<string> { "b" }
                )
            },
            {
                "tl_stove_2",
                new FcObject
                (
                    "tl_stove_2",
                    "workstation_cooker_01 (2)",
                    new List<string> { "b" }
                )
            },
            {
                "bl_stove_1",
                new FcObject
                (
                    "bl_stove_1",
                    "workstation_cooker_01 (4)",
                    new List<string> { "t" }
                )
            },
            {
                "bl_stove_2",
                new FcObject
                (
                    "bl_stove_2",
                    "workstation_cooker_01 (1)",
                    new List<string> { "t" }
                )
            },
            
            // countertops
            {
                "tr_countertop_1",
                new FcObject
                (
                    "tr_countertop_1",
                    "DLC08_countertop_01_chopping_circus (7)",
                    new List<string> { "r" }
                )
            },
            {
                "tr_countertop_2",
                new FcObject
                (
                    "tr_countertop_2",
                    "DLC08_countertop_01_standard_circus (24)",
                    new List<string> { "r" }
                )
            },
            {
                "tr_countertop_3",
                new FcObject
                (
                    "tr_countertop_3",
                    "DLC08_countertop_01_chopping_circus (5)",
                    new List<string> { "r" }
                )
            },
            {
                "tl_countertop_1",
                new FcObject
                (
                    "tl_countertop_1",
                    "DLC08_countertop_01_chopping_circus (6)",
                    new List<string> { "l" }
                )
            },
            {
                "tl_countertop_2",
                new FcObject
                (
                    "tl_countertop_2",
                    "DLC08_countertop_01_chopping_circus (19)",
                    new List<string> { "l" }
                )
            },
            {
                "tl_countertop_3",
                new FcObject
                (
                    "tl_countertop_3",
                    "DLC08_countertop_01_chopping_circus (4)",
                    new List<string> { "l" }
                )
            },
        };
    }

}
