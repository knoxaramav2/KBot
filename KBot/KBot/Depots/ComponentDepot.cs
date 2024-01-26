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

        private Dictionary<string, Chassis> ChassisDepot;
        private Dictionary<string, CPU> CpuDepot;
        private Dictionary<string, Mem> MemDepot;
        private Dictionary<string, Motor> MotorDepot;
        private Dictionary<string, PowerCell> PowerDepot;
        private Dictionary<string, Utility> UtilDepot;
        private Dictionary<string, Weapon> WeaponDepot;
        private Dictionary<string, MotherBoard> MoboDepot;

        private (PartType type, string[], int) ParseBlock(ref string[] list, int idx, ref string package)
        {
            PartType type = PartType.Misc;
            var ret = new List<string>();

            for(; idx < list.Length; ++idx)
            {
                var ln = list[idx].Trim();
                if (string.IsNullOrEmpty(ln) || ln.StartsWith('#')) 
                    continue;
                var trms = ln.Split(new char[] { ' ', '=' }, 2).Select(x => x.Trim()).ToList();
                
                if (trms.Count != 2)
                {
                    Debug.WriteLine($"WRN: Invalid MSF line:\n\t{ln}");
                    continue;
                }

                var key = trms[0].ToUpper();
                var value = trms[1];

                if (key == "PCKG") { package = value; }
                else if (key == "DEF") 
                {
                    if (type != PartType.Misc) { break; }
                    value = value.ToUpper();
                    switch (value)
                    {
                        case "CHASSIS": type = PartType.Chassis; break;
                        case "CPU": type = PartType.CPU; break;
                        case "MEM": type = PartType.Mem; break;
                        case "MOTOR": type = PartType.Motor; break;
                        case "POWER": type = PartType.Power; break;
                        case "UTIL": type = PartType.Util; break;
                        case "WEAPON": type = PartType.Weapon; break;
                        case "MOBO": type = PartType.Mobo; break;
                        default:
                            Debug.WriteLine($"WRN: Invalid MNF part type\n\t{ln}");
                            break;
                    }
                }
                else { ret.Add(ln); }
            }

            if (idx == list.Length || type == PartType.Misc) { idx = -1; }

            return (type, ret.ToArray(), idx);
        }

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

        public Component Get<K>(string id)
        {
            Component ret = default;
            id = id.ToUpper();

            switch (typeof(K))
            {
                case Type t when t == typeof(Chassis): ret = ChassisDepot.GetValueOrDefault(id); break;
                case Type t when t == typeof(PowerCell): ret = PowerDepot.GetValueOrDefault(id); break;
                case Type t when t == typeof(Motor): ret = MotorDepot.GetValueOrDefault(id); break;
                case Type t when t == typeof(Weapon): ret = WeaponDepot.GetValueOrDefault(id); break;
                case Type t when t == typeof(CPU): ret = CpuDepot.GetValueOrDefault(id); break;
                case Type t when t == typeof(Mem): ret = MemDepot.GetValueOrDefault(id); break;
                case Type t when t == typeof(Utility): ret = UtilDepot.GetValueOrDefault(id); break;
                case Type t when t == typeof(MotherBoard): ret = MoboDepot.GetValueOrDefault(id); break;
            }

            return ret;
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
