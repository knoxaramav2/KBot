using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.Components
{
    public interface IBot
    {

    }

    internal class Bot : IBot
    {
        string Name;
        string ID;
        string Package;
        Component Base;

        private Bot() { }

        public Bot(string name, string id, string package, Component baseComponent)
        {
            Name = name;
            ID = id;
            Package = package;
            Base = baseComponent;
        }

        public virtual Bot DeepCopy()
        {
            Bot copy = new Bot
            {
                Name = Name,
                ID = ID,
                Package = Package,
                Base = Base.DeepCopy()
            };

            return copy;
        }

    }
}
