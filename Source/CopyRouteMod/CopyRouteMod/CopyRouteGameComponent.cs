using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sweet_Transit;
using KeyState = STMG.Utility.KeyState;

namespace CopyRouteMod
{
    public class CopyRouteGameComponent : IGameComponent, IUpdateable
    {
        public void Initialize()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (Main.keys.GetKey(Keys.D, KeyState.Pressed))
            {
                Console.WriteLine("D pressed!");
            }
        }

        public bool Enabled => true;
        public int UpdateOrder => 0;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
    }
}