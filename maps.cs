using System.Collections.Generic;
using UnityEngine;

namespace factory_clankington;

public partial class Plugin
{

    private class FcObject
    {
        public string Alias { get; set; }
        public string Name { get; set; }
        public List<string> ApproachableDirection { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 ApproachablePosition { get; set; }
        public Vector3 ApproachableEulerAngle { get; set; }
    }
    private class Maps
    {
        public static Dictionary<string, FcObject> factory = new Dictionary<string, FcObject>
        {
            // dispensers
            { 
                "bun_dispenser", 
                new FcObject 
                { 
                    Alias = "bun_dispenser", 
                    Name = "DLC08_dispenser_crate_circus (3)", 
                    ApproachableDirection = new List<string> { "b" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            },
            { 
                "chocolate_dispenser", 
                new FcObject 
                { 
                    Alias = "chocolate_dispenser", 
                    Name = "DLC08_dispenser_crate_circus (1)", 
                    ApproachableDirection = new List<string> { "b" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            },
            { 
                "condiment_button", 
                new FcObject 
                { 
                    Alias = "condiment_button", 
                    Name = "p_dlc08_button_Condiments", 
                    ApproachableDirection = new List<string> { "t" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            },
            { 
                "condiment_dispenser", 
                new FcObject 
                { 
                    Alias = "condiment_dispenser", 
                    Name = "DLC08_condiment_dispenser", 
                    ApproachableDirection = new List<string> { "b" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            },
            { 
                "cranberry_dispenser", 
                new FcObject 
                { 
                    Alias = "cranberry_dispenser", 
                    Name = "DLC08_dispenser_crate_circus (2)", 
                    ApproachableDirection = new List<string> { "b" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            },
            { 
                "egg_dispenser", 
                new FcObject 
                { 
                    Alias = "egg_dispenser", 
                    Name = "DLC08_dispenser_crate_circus", 
                    ApproachableDirection = new List<string> { "b" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            },
            { 
                "flour_dispenser", 
                new FcObject 
                { 
                    Alias = "flour_dispenser", 
                    Name = "DLC08_dispenser_crate_circus (7)", 
                    ApproachableDirection = new List<string> { "b" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            },
            { 
                "onion_dispenser", 
                new FcObject 
                { 
                    Alias = "onion_dispenser", 
                    Name = "DLC08_dispenser_crate_circus (4)", 
                    ApproachableDirection = new List<string> { "b" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            },
            { 
                "sausage_dispenser", 
                new FcObject 
                { 
                    Alias = "sausage_dispenser", 
                    Name = "DLC08_dispenser_crate_circus (5)", 
                    ApproachableDirection = new List<string> { "b" }, 
                    Position = new Vector3(0, 0, 0), 
                    ApproachablePosition = new Vector3(0, 0, 0), 
                    ApproachableEulerAngle = new Vector3(0, 0, 0) 
                } 
            }, 
            
            // countertops
            {
                "tr_countertop_1",
                new FcObject
                {
                    Alias = "tr_countertop_1",
                    Name = "DLC08_countertop_01_chopping_circus (7)",
                    ApproachableDirection = new List<string> { "r" },
                    Position = new Vector3(0, 0, 0),
                    ApproachablePosition = new Vector3(0, 0, 0),
                    ApproachableEulerAngle = new Vector3(0, 0, 0)
                }
            },
            {
                "tr_countertop_2",
                new FcObject
                {
                    Alias = "tr_countertop_2",
                    Name = "DLC08_countertop_01_standard_circus (24)",
                    ApproachableDirection = new List<string> { "r" },
                    Position = new Vector3(0, 0, 0),
                    ApproachablePosition = new Vector3(0, 0, 0),
                    ApproachableEulerAngle = new Vector3(0, 0, 0)
                }
            },
            {
                "tr_countertop_3",
                new FcObject
                {
                    Alias = "tr_countertop_3",
                    Name = "DLC08_countertop_01_chopping_circus (5)",
                    ApproachableDirection = new List<string> { "r" },
                    Position = new Vector3(0, 0, 0),
                    ApproachablePosition = new Vector3(0, 0, 0),
                    ApproachableEulerAngle = new Vector3(0, 0, 0)
                }
            },
            {
                "tl_countertop_1",
                new FcObject
                {
                    Alias = "tl_countertop_1",
                    Name = "DLC08_countertop_01_chopping_circus (6)",
                    ApproachableDirection = new List<string> { "l" },
                    Position = new Vector3(0, 0, 0),
                    ApproachablePosition = new Vector3(0, 0, 0),
                    ApproachableEulerAngle = new Vector3(0, 0, 0)
                }
            },
            {
                "tl_countertop_2",
                new FcObject
                {
                    Alias = "tl_countertop_2",
                    Name = "DLC08_countertop_01_chopping_circus (19)",
                    ApproachableDirection = new List<string> { "l" },
                    Position = new Vector3(0, 0, 0),
                    ApproachablePosition = new Vector3(0, 0, 0),
                    ApproachableEulerAngle = new Vector3(0, 0, 0)
                }
            },
            {
                "tl_countertop_3",
                new FcObject
                {
                    Alias = "tl_countertop_3",
                    Name = "DLC08_countertop_01_chopping_circus (4)",
                    ApproachableDirection = new List<string> { "l" },
                    Position = new Vector3(0, 0, 0),
                    ApproachablePosition = new Vector3(0, 0, 0),
                    ApproachableEulerAngle = new Vector3(0, 0, 0)
                }
            },
            
        };
    }

}
