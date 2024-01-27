using KBot.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Security.Cryptography;

namespace KBot.Util
{
    public enum BlockType
    {
        None,
        DEF
    }

    public class DataKW
    {
        private static string[] BlockKW = { DEF };

        //Common
        public const string PACKAGE = "PCKG";
        public const string DEF = "DEF";

        //Meta
        public const string DISPLAYNAME = "DSPNAME";

        //Components
        public const string CHASSIS = "CHASSIS";
        public const string MOBO = "MOBO";
        public const string CPU = "CPU";
        public const string MOTOR = "MOTOR";
        public const string MEM = "MEM";
        public const string UTILITY = "UTILITY";
        public const string POWERCELL = "POWERCELL";
        public const string WEAPON = "WEAPON";

        public const string COST = "COST";
        public const string NAME = "NAME";
        public const string DESC = "DESC";
        public const string ID = "ID";
        public const string SPRITE = "SPRITE";
        public const string DIM = "DIM";
        public const string HEALTH = "HEALTH";
        public const string ATTACH = "ATTACH";

        //Fab
        public const string FAB = "FAB";
        public const string PREFAB = "PREFAB";
        public const string BASE = "BASE";
        public const string SUB = "SUB";

        public static bool Equals(string v1, string v2)
        {
            return v1.ToUpper() == v2.ToUpper();
        }

        public static bool IsBlockKW(string kw)
        {
            var ukw = kw.ToUpper();
            return BlockKW.Any(x => x == ukw);
        }

        public static BlockType GetBlockTypeId(string kw)
        {
            switch (kw.ToUpper())
            {
                case DEF: return BlockType.DEF;
                default: return BlockType.None;
            }
        }

        public static PartType GetPartTypeId(string kw)
        {
            switch(kw.ToUpper())
            {
                case CHASSIS: return PartType.Chassis;
                case MOBO: return PartType.Mobo;
                case CPU: return PartType.CPU;
                case MOTOR: return PartType.Motor;
                case MEM: return PartType.Mem;
                case UTILITY: return PartType.Util;
                case POWERCELL: return PartType.Power;
                case WEAPON: return PartType.Weapon;

                case PREFAB: return PartType.PreFab;

                default: return PartType.Misc;
            }
        }
    }

    public class KV
    {
        private string __key;
        private string __value;
        
        public string Key
        {
            get { return __key; }
            private set { __key = value.ToUpper().Trim(); } 
        }

        public string Value 
        { 
            get { return __value; }
            private set { __value = value.Trim(); }
        }

        public int[] Indices { get; private set; } = null;

        public KV(string kvString)
        {
            var trms = kvString.Split(new char[] { ' ', '=' }, 2).Select(x => x.Trim()).ToList();
            Key = trms[0];
            if (trms.Count == 2) { Value = trms[1]; }
        }

        public KV(string key, string value)
        {
            var ks = key.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            Indices = Array.ConvertAll(ks[1..ks.Length], x => int.Parse(x));

            Key = ks[0];
            Value = value;
        }

        public static KV[] ParseKV(string kvString) 
        {
            var ret = new List<KV>();
            var trms = kvString.Split(new char[] { ' ', '=' }, 2).Select(x => x.Trim()).ToList();
            if (trms.Count == 1) { return Array.Empty<KV>(); }

            var key = trms[0];
            var values = trms[1].Split(";");

            foreach(var val in values)
            {
                ret.Add(new KV(key, val));
            }

            return ret.ToArray();
        }
    }

    public struct DataBlock
    {
        public BlockType Type { get; private set; } = BlockType.None;
        public string BlockName { get; private set; }
        //public DataBlock[] SubBlocks { get; private set; } = default;
        public List<KV> Data = default;

        public DataBlock(KV header, List<KV> data)
        {
            Type = DataKW.GetBlockTypeId(header.Key);
            BlockName = header.Value;
            Data = data;
        }
    }

    public class DataFile
    {
        private string __Package;
        public string Package { get { return __Package; } protected set { __Package = value.ToUpper(); } }
        public string FilePath { get; protected set; }
        public List<DataBlock> Data { get; private set; }

        public DataFile(string filePath)
        {
            Data = null;
            FilePath = filePath;
        }

        public bool Load()
        {
            if (Data != null) { return false; }
            Data = new();

            if (!File.Exists(FilePath)) { return false; }
            var lines = File.ReadAllLines(FilePath);
            var kvList = lines.Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x) && !x.StartsWith('#'))
                .SelectMany(x => KV.ParseKV(x))
                .ToList();
            var pckg = kvList.FindAll(x => DataKW.Equals(x.Key, DataKW.PACKAGE));
            kvList.RemoveAll(x => pckg.Contains(x));
            if (pckg.Count != 1)
            {
                throw new Exception($"ERR for file :{FilePath}\n\tMust have exactly one package definition");
            }
            Package = pckg[0].Value;

            var blockIdxs = kvList.Select((elt, idx) => new { elt.Key, idx })
                .Where(ki => DataKW.IsBlockKW(ki.Key)).ToArray();
            
            for(var i = 0; i < blockIdxs.Length; ++i)
            {
                var sIdx = blockIdxs[i].idx;
                var eIdx = i+1==blockIdxs.Length ? kvList.Count-1: blockIdxs[i+1].idx;
                var def = kvList.ElementAt(sIdx);
                var kvSubList = kvList.GetRange(sIdx+1, eIdx);
                var block = new DataBlock(def, kvSubList);
                Data.Add(block);
            }

            return true;
        }
    }

    public static class UFile
    {
        public static string BaseDir { get => Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName; }
        public static string StdAssetDir { get => Path.Join(BaseDir, "Content"); }
        public static string GameDataDir { get => Path.Join(BaseDir, "GameData"); }
        public static string PartsDir { get => Path.Join(GameDataDir, "Parts"); }
        public static string SavesDir { get => Path.Join(GameDataDir, "Saves"); }
        public static string PreFabDir { get => Path.Join(GameDataDir, "Prefab"); }
        public static string ModDir { get => Path.Join(GameDataDir, "Mod"); }
    }

    public static class DataFileUtil
    {
        public static (string, string, string)[] ParseKV(string[] data)
        {
            List<(string, string, string)> ret = new();

            foreach (var line in data)
            {
                var trms = line.Split(new char[] { ' ', '=' }, 2).Select(x => x.Trim()).ToList();
                if (trms.Count != 2) { continue; }
                var key = trms[0].ToUpper();
                var val = trms[1];
                ret.Add((key, val, line));
            }

            return ret.ToArray();
        }
    }
    
}
