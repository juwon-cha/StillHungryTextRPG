using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{


    internal class GuildScene : BaseScene
    {
        private readonly string[] mMenuItems = { "장비 상점", "소모품 상점", "요리 상점  ","퀘스트 보드","휴식하기", "나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public GuildScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new ChangeSceneCommand(ESceneType.STORE_SCENE),
                new ChangeSceneCommand(ESceneType.CONSUMABLE_STORE_SCENE),
                new BuyFoodCommand(RequestRedraw), // 이거 요리 상점
                new ChangeSceneCommand(ESceneType.QUEST_SCENE),
                new ChangeSceneCommand(ESceneType.CAMPSITE_SCENE),
                new ChangeSceneCommand(ESceneType.TOWN_SCENE),
                
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
            Manager.Instance.UI.GuildScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }
    }
}
