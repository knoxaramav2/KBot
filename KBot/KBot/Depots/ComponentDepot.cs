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
    public class ComponentDepot
    {
        private Dictionary<string, Chassis> ChassisDepot;

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

        private void Register(PartType type, Component component)
        {
            switch (type)
            {
                case PartType.Chassis: break;
            }
        }

        private void Build(PartType type, string[] block, string pckg)
        {
            Component cmp = null;

            switch (type)
            {
                case PartType.Chassis:
                    cmp = new Chassis(block, pckg);
                    break;
            }

            if (cmp != null)
            {
                Register(type, cmp);
            }
        }

        public void Load()
        {
            var path = UFile.PartsDir;
            var files = Directory.GetFiles(path, "*.mnf");

            foreach(var file in files)
            {
                var mnfLines = File.ReadAllLines(file);
                string[] block;
                int idx = 0;
                PartType type;
                string pckg = string.Empty;
                while (idx != -1)
                {
                    (type, block, idx) = ParseBlock(ref mnfLines, idx, ref pckg);
                    if (block.Length == 0) { continue; }
                    Build(type, block, pckg);
                }
            }
        }

        public ComponentDepot() 
        {
            Load();
        }
    }

    public static class Depots
    {
        public static ComponentDepot __Components = null;

        public static ComponentDepot Depot => __Components ??= new();

    }

}
