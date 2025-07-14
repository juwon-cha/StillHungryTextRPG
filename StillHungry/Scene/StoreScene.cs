using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public enum EPurchaseResult
    {
        SUCCESS,
        NOT_ENOUGH_GOLD,
        ALREADY_PURCHASED,
        NONE,
    }

    public enum ESellResult
    {
        SUCCESS,
        NOT_IN_INVENTORY
    }

    public class StoreScene : BaseScene
    {
        private readonly string[] mMenuItems = { "1. 아이템 구매", "2. 아이템 판매", "0. 나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public StoreScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new BuyItemCommand(RequestRedraw),
                new SellItemCommand(RequestRedraw),
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
            Manager.Instance.UI.ShowStoreScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
        }
    }
}