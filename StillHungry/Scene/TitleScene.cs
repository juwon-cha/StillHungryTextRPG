using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public class TitleScene : BaseScene
    {
        private readonly string[] mMenuItems = { "1. New Game", "2. Load Game", "0. Exit" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public TitleScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new NewGameCommand(),
                new LoadGameCommand(),
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
            // UI 그릴 때 네비게이터가 가진 현재 인덱스 정보를 넘겨줌
            Manager.Instance.UI.ShowTitleScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
            // 애니메이션이나 상태 업데이트가 필요하다면 여기에 구현
        }
    }
}
