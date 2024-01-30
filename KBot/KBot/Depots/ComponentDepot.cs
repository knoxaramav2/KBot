using KBot.Components;
using KBot.State;
using KBot.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace KBot.Depots
{
    public class ComponentDepot : IDepot<Component>
    {
        private static ComponentDepot __Depots = null;
        public static ComponentDepot Depots => __Depots ??= new();

        private readonly Dictionary<string, Chassis> ChassisDepot;
        private readonly Dictionary<string, CPU> CpuDepot;
        private readonly Dictionary<string, Mem> MemDepot;
        private readonly Dictionary<string, Motor> MotorDepot;
        private readonly Dictionary<string, PowerCell> PowerDepot;
        private readonly Dictionary<string, Utility> UtilDepot;
        private readonly Dictionary<string, Weapon> WeaponDepot;
        private readonly Dictionary<string, MotherBoard> MoboDepot;

        private void Register(Component component)
        {
            var id = component.ID;
            var type = component.Type;

            Debug.WriteLine($"REGISTER {type} : {id}");

            switch (type)
            {
                case PartType.Chassis: ChassisDepot.Add(id, (Chassis)component); break;
                case PartType.CPU: CpuDepot.Add(id, (CPU)component); break;
                case PartType.Mem: MemDepot.Add(id, (Mem)component); break;
                case PartType.Motor: MotorDepot.Add(id, (Motor)component); break;
                case PartType.Power: PowerDepot.Add(id, (PowerCell)component); break;
                case PartType.Util: UtilDepot.Add(id, (Utility)component); break;
                case PartType.Weapon: WeaponDepot.Add(id, (Weapon)component); break;
                case PartType.Mobo: MoboDepot.Add(id, (MotherBoard)component); break;
            }
        }

        private void Build(PartType type, KV[] block, string pckg)
        {
            Component cmp = null;

            switch (type)
            {
                case PartType.Chassis: cmp = new Chassis(block, pckg); break;
                case PartType.CPU: cmp = new CPU(block, pckg); break;
                case PartType.Mem: cmp = new Mem(block, pckg); break;
                case PartType.Motor: cmp = new Motor(block, pckg); break;
                case PartType.Power: cmp = new PowerCell(block, pckg); break;
                case PartType.Util: cmp = new Utility(block, pckg); break;
                case PartType.Weapon: cmp = new Weapon(block, pckg); break;
                case PartType.Mobo: cmp = new MotherBoard(block, pckg); break;
            }

            if (cmp != null) { Register(cmp); }
        }

        public void Load()
        {
            var path = UFile.PartsDir;
            var files = Directory.GetFiles(path, "*.mnf");

            foreach (var file in files)
            {
                DataFile dataFile = new(file);
                dataFile.Load();
                foreach(var dt in dataFile.Data)
                {
                    if (dt.Type == BlockType.DEF)
                    {
                        Build(DataKW.GetPartTypeId(dt.BlockName), dt.Data.ToArray(), dataFile.Package);
                    }
                }
            }
        }

        public Component Get(string id)
        {
            id = id.ToUpper();
            { if (ChassisDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }
            { if (PowerDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }
            { if (MotorDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }
            { if (WeaponDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }
            { if (CpuDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }
            { if (MemDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }
            { if (UtilDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }
            { if (MoboDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }

            return null;
        }

        public ComponentDepot() 
        {
            ChassisDepot = new();
            CpuDepot = new();
            MemDepot = new();
            MotorDepot = new();
            PowerDepot = new();
            UtilDepot = new();
            WeaponDepot = new();
            MoboDepot = new();

            Load();
        }
    }

}
