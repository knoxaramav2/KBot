using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.State
{
    public static class Config
    {
        public static bool DevMode { get; set; }

        private static void ParseCLI(string[] args)
        {
            foreach(var arg in args)
            {
                var trms = arg.Split('=');
                if (trms.Length > 2 ) 
                { 
                    Debug.WriteLine($"Invalid argument: {arg}");
                    continue;
                }

                var key = trms[0];
                var val = trms.Length == 2 ? trms[1] : string.Empty;

                switch (key.ToUpper())
                {
                    case "--DEV": DevMode = true; break;
                }
            }
        }

        public static void Init(string[] args)
        {
            DevMode = false;


            ParseCLI(args);
        }
    }
}
