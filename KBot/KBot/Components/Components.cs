using KBot.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Xml.Serialization;

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
        public PartType[] AllowedTypes { get; private set; }
        public bool SetPart(Component part)
        {
            if (AllowedTypes.Contains(PartType.Misc) || AllowedTypes.Any(x => x == part.Type))
            {
                Part = part;
                return true;
            }

            return false;
        }

        public Slot(Component part, PartType[] types)
        {
            Part = part;
            AllowedTypes = types;
        }
    }

    public class Component
    {
        public string Package { get; protected set; }
        public string ID { get; protected set; }
        public string DisplayName { get; protected set; }
        public string Description { get; protected set; }
        public int Cost { get; protected set; }
        public PartType Type { get; protected set; }
        public Point Size { get; protected set; }
        public Texture2D Sprite { get; protected set; }
        public List<Slot> SubComponents { get; protected set; }
        public Component Parent { get; protected set; }
        public int Health;

        private Component() { }

        public Component(
            Component parent,
            string id, string package, int cost, int health,
            PartType ptype, Point size, 
            Texture2D sprite,
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
            Sprite = sprite;
            SubComponents = attachPoints.ToList();
        }

        public enum AttachErr
        {
            OK, Occupied, Invalid
        }

        public AttachErr Attach(Component component, int idx, bool replace=false)
        {
            if (idx < 0 || idx >= SubComponents.Count){
                throw new System.Exception("Out of bounds");
            }

            if (SubComponents[idx].Part != null && !replace) { return AttachErr.Occupied; }
            if (SubComponents[idx].AllowedTypes.Any(x => x == component.Type) ||
                 SubComponents[idx].AllowedTypes.Contains(PartType.Misc))
                { return AttachErr.Invalid; }
            SubComponents[idx] = new Slot(component, SubComponents[idx].AllowedTypes);

            return AttachErr.OK; 
        }

        protected (int, int) PartDim(string value)
        {
            var dv = value.Split(',');
            if (dv.Length != 2 || !int.TryParse(dv[0], out int w) || !int.TryParse(dv[0], out int h))
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
                    case DataKW.SPRITE: Sprite = Providers.Sprites.Get(kv.Value); break;
                    case DataKW.DIM:
                        {
                            var wh = PartDim(kv.Value);
                            Size = new Point(wh.Item1, wh.Item2);
                        }
                        break;
                    case DataKW.ATTACH:
                        {
                            var sVals = kv.Value.Split(':');
                            var wh = PartDim(sVals[0]);
                            var types = sVals[1].Split('|');
                            var typeCodes = types.Select(x => DataKW.GetPartTypeId(x)).ToArray();
                            SubComponents.Add(new Slot(null, typeCodes));
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
            var clone = new Component();

            clone.Package = Package;
            clone.ID = ID;
            clone.DisplayName = DisplayName;
            clone.Description = Description;
            clone.Cost = Cost;
            clone.Type = Type;
            clone.Size = new Point(Size.X, Size.Y);
            clone.Sprite = Sprite;
            clone.SubComponents = SubComponents
                .Select(x => new Slot((Component)x.Part?.DeepCopy(), x.AllowedTypes))
                .ToList();
            clone.Parent = Parent;
            clone.Health = Health;

            return clone;
        }
    }

    public class Chassis : Component
    {
        public Chassis(Component parent, string id, string package, int cost, int health,
            Point size, Texture2D sprite, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Chassis, size, sprite, attachPoints)
        {
        }

        public override KV[] Load(KV[] data)
        {
            var remData = base.Load(data);
            var ret = new List<KV>();

            return ret.ToArray();
        }

        public Chassis(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Chassis, new Point(), null, Array.Empty<Slot>())
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
            Point size, Texture2D sprite, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Motor, size, sprite, attachPoints)
        {
        }

        public Motor(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Motor, new Point(), null, Array.Empty<Slot>())
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
            Point size, Texture2D sprite, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Power, size, sprite, attachPoints)
        {
        }

        public PowerCell(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Power, new Point(), null, Array.Empty<Slot>())
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
            Point size, Texture2D sprite, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Util, size, sprite, attachPoints)
        {
        }

        public Utility(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Util, new Point(), null, Array.Empty<Slot>())
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
            Point size, Texture2D sprite, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Weapon, size, sprite, attachPoints)
        {
        }

        public Weapon(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Weapon, new Point(), null, Array.Empty<Slot>())
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
            Point size, Texture2D sprite, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.CPU, size, sprite, attachPoints)
        {
        }

        public CPU(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.CPU, new Point(), null, Array.Empty<Slot>())
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
            Point size, Texture2D sprite, Slot[] attachPoints) 
            : base(parent, id, package, cost, health, PartType.Mem, size, sprite, attachPoints)
        {
        }

        public Mem(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Mem, new Point(), null, Array.Empty<Slot>())
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
            Point size, Texture2D sprite, Slot[] attachPoints)
            : base(parent, id, package, cost, health, PartType.Mobo, size, sprite, attachPoints)
        {
        }

        public MotherBoard(KV[] data, string package) : base(null, string.Empty, string.Empty, 0, 0,
            PartType.Mobo, new Point(), null, Array.Empty<Slot>())
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
