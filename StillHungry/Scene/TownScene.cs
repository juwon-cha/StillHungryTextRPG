using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public class TownScene : BaseScene
    {
        private readonly string[] mMenuItems = { "1. 상태 보기", "2. 인벤토리", "3. 길드", "4. 던전 입장","5. 저장하기", "0. 게임 종료" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public TownScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new ChangeSceneCommand(ESceneType.STATUS_SCENE),
                new ChangeSceneCommand(ESceneType.INVENTORY_SCENE),
                new ChangeSceneCommand(ESceneType.GUILD_SCENE),
                new ChangeSceneCommand(ESceneType.DUNGEON_SCENE),
                new SaveGameCommand(),
                new ExitGameCommand()
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
            Manager.Instance.UI.ShowTownScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
        }
    }
}