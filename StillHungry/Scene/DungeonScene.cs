using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public enum EDungeonLevel
    {
        EASY,
        NORMAL,
        HARD
    }

    public class DungeonScene : BaseScene
    {
        private readonly string[] mMenuItems = { "1. 쉬운 던전", "2. 일반 던전", "3. 어려운 던전", "0. 나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public DungeonScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new EnterDungeonCommand(EDungeonLevel.EASY, RequestRedraw),
                new EnterDungeonCommand(EDungeonLevel.NORMAL, RequestRedraw),
                new EnterDungeonCommand(EDungeonLevel.HARD, RequestRedraw),
                new ChangeSceneCommand(ESceneType.TOWN_SCENE)
            };
        }

        public override void Display()
        {
            Update();
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
            Manager.Instance.UI.ShowDungeonScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
        }
    }
}