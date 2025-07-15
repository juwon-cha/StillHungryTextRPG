using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.Scene
{
    internal class QuestScene : BaseScene
    {
        public override void Display()
        {
            Update();
            // ProcessInput(mMenuCommands, mNavigator);
            Render();
        }

        protected override void Update()
        {

        }

        public override void Render()
        {
            if (!bNeedsRedraw)
            {
                return;
            }
        }
    }
}
