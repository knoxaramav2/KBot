﻿using KBot.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot.State
{


    public class PlayerState
    {
        public string Name = string.Empty;
        public int Funds = 500;

        public List<Component> PartInventory;
        public List<Bot> BotInventory;

        public PlayerState() {
            PartInventory = new();
            BotInventory = new();

            if (Config.DevMode)
            {
                BotInventory.Add(Depots.FabDepot.Depots.Get("DEVBOT"));
            }
        }
    }
}
