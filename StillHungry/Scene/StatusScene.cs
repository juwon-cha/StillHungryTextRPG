using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public class StatusScene : BaseScene
    {
        private readonly string[] mMenuItems = { "나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public StatusScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new ChangeSceneCommand(ESceneType.TOWN_SCENE)
            };
        }

        public override void Display()
        {
            ProcessInput(mMenuCommands, mNavigator);
            Render();
        }
        public override void Render()
        {
            if (!bNeedsRedraw)
            {
                return;
            }

            Console.Clear();
            Manager.Instance.UI.ShowStatusScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }
    }
}