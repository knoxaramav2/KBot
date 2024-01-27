using KBot.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.Depots
{
    public interface IDepot<T>
    {
        public void Load();
        public T Get(string id);
    }
}
