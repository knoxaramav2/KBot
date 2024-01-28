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

    public class Bot : IBot
    {
        public string Name;
        public string ID;
        public string Package;
        public Component Base;

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
