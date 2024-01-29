using KBot.Components;
using KBot.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace KBot.Depots
{
    internal class FabDepot : IDepot<Bot>
    {
        private static FabDepot __Depots = null;
        public static FabDepot Depots => __Depots ??= new();

        private Dictionary<string, Bot> BotDepot;
        private ComponentDepot CompDepot;

        private void ReadManifest(PartType type, KV[] block, string pckg)
        {
            Component cmp = null;
            Component curr = null;

            string name = string.Empty;
            string id = string.Empty;

            foreach (var kv in block)
            {
                var key = kv.Key;
                var val = kv.Value;

                switch (key)
                {
                    case DataKW.NAME: name = val; break;
                    case DataKW.ID: id = val; break;
                    case DataKW.SUB:
                        {
                            var rcmp = ComponentDepot.Depots.Get(val);
                            if (rcmp == null) { Debug.WriteLine($"Unable to load part: {val}"); continue; }
                            curr = rcmp;
                            break;
                        }
                    case DataKW.BASE:
                        {
                            var rcmp = ComponentDepot.Depots.Get(val);
                            if (rcmp == null) { Debug.WriteLine($"Unable to load part: {val}"); continue; }

                            var idx = kv.Indices;

                            if (idx.Length == 0)
                            {
                                if (cmp != null) { Debug.WriteLine($"Base ref {cmp.ID} -> {rcmp.ID}"); continue; }
                                cmp = rcmp;
                                curr = cmp;
                            }
                            else
                            {
                                if (cmp == null) { Debug.WriteLine($"Missing base"); continue; }
                                if (!curr.GetSlot(idx, out var slot)) 
                                    { Debug.WriteLine($"Missing slot: {curr.ID}.{idx} -> {rcmp.ID}"); continue; }
                                if (!slot.SetPart(curr, rcmp)) 
                                    { Debug.WriteLine($"Failed to set part: {curr.ID}.{idx} -> {rcmp.ID}"); continue; }
                            }

                            break;
                        }
                }
            }

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id))
            {
                Debug.WriteLine($"Invalid identifiers: NAME={name}, ID={id}");
            }

            var bot = new Bot(name, id, pckg, cmp);
            BotDepot.Add(id, bot);
        }

        public void Load()
        {
            var path = UFile.PreFabDir;
            var files = Directory.GetFiles(path, "*.prf");

            foreach (var file in files)
            {
                DataFile dataFile = new(file);
                dataFile.Load();
                foreach(var dt in dataFile.Data)
                {
                    if (dt.Type == BlockType.DEF)
                    {
                        ReadManifest(DataKW.GetPartTypeId(dt.BlockName), dt.Data.ToArray(), dataFile.Package);
                    }
                }
            }
        }

        public Bot Get(string id)
        {
            id = id.ToUpper();
            { if (BotDepot.TryGetValue(id, out var ret)) return ret.DeepCopy(); }

            return null;
        }

        public FabDepot()
        {
            BotDepot = new();
            CompDepot = ComponentDepot.Depots;

            Load();
        }

    }
}
