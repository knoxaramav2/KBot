using KBot.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KBot.State
{
    public class GameState
    {
        private static GameState __State = null;
        public static GameState State => __State ??= new();


        public string SaveName { get; set; }

        private GameState() 
        { 
            
        }

        public void Load()
        {

        }

        public void Save()
        {
            var rand = new Random();
            var val = rand.Next(1000);
            var path = Path.Combine(UFile.SavesDir, SaveName);
            using StreamWriter writer = new(path);
            var txt = "Test " + val.ToString();
            writer.WriteLine(txt);
            Debug.WriteLine(txt);
        }
    }
}
