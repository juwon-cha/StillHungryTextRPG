using StillHungry.Commands;
using StillHungry.UI;
using StillHungry.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.Scene
{
    internal class SelectStoreScene : BaseScene
    {
        private readonly string[] mMenuItems = { "1. 장비 상점", "2. 소모품 상점","3. 재료 상점" ,"0. 나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public SelectStoreScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new ChangeSceneCommand(ESceneType.STORE_SCENE)
                //new ChangeSceneCommand(ESceneType.CONSUMABLE_STORE_SCENE),
                //new ChangeSceneCommand(ESceneType.MATERIAL_STORE_SCENE),
                //new ChangeSceneCommand(ESceneType.TOWN_SCENE)
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
