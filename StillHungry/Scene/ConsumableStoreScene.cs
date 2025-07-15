using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.Scene
{
    internal class ConsumableStoreScene : BaseScene
    {
        private readonly string[] mMenuItems = { "1. 아이템 구매", "2. 아이템 판매", "0. 나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public ConsumableStoreScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new BuyConsumableCommand(RequestRedraw), // 소모품 상점
                new SellConsumableCommand(RequestRedraw),      // 아이템 판매
                new ChangeSceneCommand(ESceneType.GUILD_SCENE)
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
            Manager.Instance.UI.ConsumableStoreScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {

        }
    }
}
