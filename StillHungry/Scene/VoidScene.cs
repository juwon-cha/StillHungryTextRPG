using StillHungry.Commands;
using StillHungry.Scene;
using StillHungry.UI;
using System.ComponentModel.Design;

namespace StillHungry.Managers
{
    internal class VoidScene : BaseScene
    {
        private List<IExecutable> mMenuCommands = new List<IExecutable>();
        private readonly MenuNavigator mNavigator;
        public VoidScene(List<IExecutable> val1, MenuNavigator val2)
        {
            mMenuCommands = val1;
            mNavigator = val2;
        }
        public override void Display()
        {
            ProcessInput(mMenuCommands.ToArray(), mNavigator);
            Render();
        }
        public override void Render()
        {
            return;
        }
    }
}