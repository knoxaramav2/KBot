using KBot.State;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KBot
{
    public class ControlSystem
    {
        private List<Entity> Entities;
        private Controllable Player;

        public ControlSystem()
        {
            Player = GameState.State.Avatar;
            Entities = new()
            {
                Player
            };
        }

        public void Update(KeyboardState kbst, MouseState mst)
        {
            Player?.ActionIO(kbst, mst);
            foreach(var entity in Entities) { entity.Update(); }
        }

        public void Draw()
        {
            foreach (var entity in Entities) { entity.Draw(); }
        }

        public void AddEntity(Entity entity)
        {
            if (Player == null)
            {
                if (entity is Controllable controllable)
                {
                    Player = controllable;
                }

                Entities.Add(entity);
            }
        }
    }
}
