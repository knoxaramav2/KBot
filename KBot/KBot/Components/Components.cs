using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace KBot.Components
{
    public enum PartType
    {
        Chassis, Power, Motor, Weapon, CPU, Mem, Util, Mobo,

        PreFab,

        Misc
    }

    public class Slot
    {
        public Component Part { get; set; }
        public PartType[] AllowedTypes { get; set; }
        public Point Offset { get; set; }
        public bool SetPart(Component parent, Component part)
        {
            if (AllowedTypes.Contains(PartType.Misc) || AllowedTypes.Any(x => x == part.Type))
            {
                part.Parent = parent;
                Part = part;
                return true;
            }

            return false;
        }

        public Slot(Component part, PartType[] types, Point offset)
        {
            Part = part;
            AllowedTypes = types;
            Offset = offset;
        }
    }

    public class Component
    {
        public string Package { get;  set; }
        public string ID { get;  set; }
        public string DisplayName { get;  set; }
        public string Description { get;  set; }
        public int Cost { get;  set; }
        public PartType Type { get;  set; }
        public Point Size { get;  set; }
        public string SpriteName { get; set; }
        [JsonIgnore]
        public Texture2D Sprite { get;  set; }
        public List<Slot> SubComponents { get;  set; }
        public Component Parent { get;  set; }
        public int Health;

        public Component() { }

        public Component(
            Component parent,
            string id, string package, int cost, int health,
            PartType ptype, Point size, 
            string spriteName,
            Slot[] attachPoints
            ) 
        {
            Parent = parent;
            Package = package;
            ID = id;
            Cost = cost;
            Health = health;
            Type = ptype; 
            Size = size;
            SpriteName = spriteName;
            Sprite = Providers.Sprites.Get(spriteName);
            SubComponents = attachPoints.ToList();
        }

        public void RectifyLoadState() 
        { 
            Sprite ??= Providers.Sprites.Get(SpriteName);
            foreach(var sb in SubComponents) { sb.Part?.RectifyLoadState(); }
        }

        //public enum AttachErr
        //{
        //    OK, Occupied, Invalid
        //}

        //public AttachErr Attach(Component component, int idx, bool replace=false)
        //{
        //    if (idx < 0 || idx >= SubComponents.Count){
        //        throw new System.Exception("Out of bounds");
        //    }

        //    if (SubComponents[idx].Part != null && !replace) { return AttachErr.Occupied; }
        //    if (SubComponents[idx].AllowedTypes.Any(x => x == component.Type) ||
        //         SubComponents[idx].AllowedTypes.Contains(PartType.Misc))
        //        { return AttachErr.Invalid; }
        //    component.Parent = this;
        //    SubComponents[idx] = new Slot(component, SubComponents[idx].AllowedTypes, new Point());

        //    return AttachErr.OK; 
        //}

        protected (int, int) IntPair(string value)
        {
            var dv = value.Split(',');
            if (dv.Length != 2 || !int.TryParse(dv[0], out int w) || !int.TryParse(dv[1], out int h))
                return (0, 0);
            return (w, h);
        }

        public virtual KV[] Load(KV[] data)
        {
            List<KV> remainder = new();

            foreach(KV kv in data)
            {
                switch(kv.Key)
                {
                    case DataKW.COST: { if (int.TryParse(kv.Value, out int v)) Cost = v; } break;
                    case DataKW.HEALTH: { if (int.TryParse(kv.Value, out int v)) Health = v; } break;
                    case DataKW.NAME: DisplayName = kv.Value; break;
                    case DataKW.DESC: Description = kv.Value; break;
                    case DataKW.ID: ID = kv.Value; break;
                    case DataKW.SPRITE:
                        SpriteName = kv.Value;
                        Sprite = Providers.Sprites.Get(kv.Value); 
                        break;
                    case DataKW.DIM:
                        {
                            var wh = IntPair(kv.Value);
                            Size = new Point(wh.Item1, wh.Item2);
                        }
                        break;
                    case DataKW.ATTACH:
                        {
                            var sVals = kv.Value.Split(':');
                            var offset = IntPair(sVals[0]);
                            var types = sVals[1].Split('|');
                            var typeCodes = types.Select(x => DataKW.GetPartTypeId(x)).ToArray();
                            SubComponents.Add(new Slot(null, typeCodes, new Point(offset.Item1, offset.Item2)));
                        }
                        break;
                }
            }

            return remainder.ToArray();
        }
    
        public bool GetSlot(int[] index, out Slot slot)
        {
            if (index.Length == 0 || index[0] >= SubComponents.Count) 
                { slot = null; return false; }
            var tmpSlot = SubComponents[index[0]];
            index = index[1 .. index.Length];

            if (index.Length > 0 && tmpSlot.Part != null) 
                { return tmpSlot.Part.GetSlot(index, out slot); }

            slot = tmpSlot;
            return true;
        }

        public virtual Component DeepCopy()
        {
            var clone = new Component
            {
                Package = Package,
                ID = ID,
                DisplayName = DisplayName,
                Description = Description,
                Cost = Cost,
                Type = Type,
                Size = new Point(Size.X, Size.Y),
                SpriteName = SpriteName,
                Sprite = Sprite,
                SubComponents = SubComponents
                .Select(x => new Slot(x.Part?.DeepCopy(), x.AllowedTypes, x.Offset))
                .ToList(),
                Parent = Parent,
                Health = Health
            };

            return clone;
        }
    }

    public class Chassis : Component
    {
        public Chassis(Component parent, string id, string package, int cost, int health,
            Point size, string spriteName, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Chassis, size, spriteName, attachPoints)
        {
        }

        public override KV[] Load(KV[] data)
        {
            var remData = base.Load(data);
            var ret = new List<KV>();

            return ret.ToArray();
        }

        public Chassis(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Chassis, new Point(), string.Empty, Array.Empty<Slot>())
        {
            Package = package;
            Load(data);
        }

        public override Component DeepCopy()
        {
            var clone = base.DeepCopy();


            return clone;
        }
    }

    public class Motor : Component
    {
        public Motor(Component parent, string id, string package, int cost, int health, 
            Point size, string spriteName, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Motor, size, spriteName, attachPoints)
        {
        }

        public Motor(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Motor, new Point(), string.Empty, Array.Empty<Slot>())
        {
            Package = package;
            Load(data);
        }

        public override Component DeepCopy()
        {
            var clone = base.DeepCopy();


            return clone;
        }
    }

    public class PowerCell : Component
    {
        public PowerCell(Component parent, string id, string package, int cost, int health, 
            Point size, string spriteName, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Power, size, spriteName, attachPoints)
        {
        }

        public PowerCell(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Power, new Point(), string.Empty, Array.Empty<Slot>())
        {
            Package = package;
            Load(data);
        }

        public override Component DeepCopy()
        {
            var clone = base.DeepCopy();


            return clone;
        }
    }

    public class Utility : Component
    {
        public Utility(Component parent, string id, string package, int cost, int health, 
            Point size, string spriteName, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Util, size, spriteName, attachPoints)
        {
        }

        public Utility(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Util, new Point(), string.Empty, Array.Empty<Slot>())
        {
            Package = package;
            Load(data);
        }

        public override Component DeepCopy()
        {
            var clone = base.DeepCopy();


            return clone;
        }
    }

    public class Weapon : Component
    {
        public Weapon(Component parent, string id, string package, int cost, int health, 
            Point size, string spriteName, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Weapon, size, spriteName, attachPoints)
        {
        }

        public Weapon(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Weapon, new Point(), string.Empty, Array.Empty<Slot>())
        {
            Package = package;
            Load(data);
        }

        public override Component DeepCopy()
        {
            var clone = base.DeepCopy();


            return clone;
        }
    }

    public class CPU : Component
    {
        public CPU(Component parent, string id, string package, int cost, int health, 
            Point size, string spriteName, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.CPU, size, spriteName, attachPoints)
        {
        }

        public CPU(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.CPU, new Point(), string.Empty, Array.Empty<Slot>())
        {
            Package = package;
            Load(data);
        }

        public override Component DeepCopy()
        {
            var clone = base.DeepCopy();


            return clone;
        }
    }

    public class Mem : Component
    {
        public Mem(Component parent, string id, string package, int cost, int health, 
            Point size, string spriteName, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Mem, size, spriteName, attachPoints)
        {
        }

        public Mem(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Mem, new Point(), string.Empty, Array.Empty<Slot>())
        {
            Package = package;
            Load(data);
        }

        public override Component DeepCopy()
        {
            var clone = base.DeepCopy();


            return clone;
        }
    }

    public class MotherBoard : Component
    {
        public MotherBoard(Component parent, string id, string package, int cost, int health,
            Point size, string spriteName, Slot[] attachPoints)
            : base(parent, id, package, cost, health, PartType.Mobo, size, spriteName, attachPoints)
        {
        }

        public MotherBoard(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Mobo, new Point(), string.Empty, Array.Empty<Slot>())
        {
            Package = package;
            Load(data);
        }

        public override Component DeepCopy()
        {
            var clone = base.DeepCopy();


            return clone;
        }
    }
}
