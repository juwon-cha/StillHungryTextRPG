using StillHungry.Commands;
using StillHungry.Controller;
using StillHungry.Managers;
using StillHungry.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StillHungry.Scene.CharacterSettingScene;

namespace StillHungry.Scene
{
    internal class BattleScene : BaseScene
    {
        private readonly string[] mMenuItems = { "1. 공격\t\t\t(공사중)", "2. 요리재료로 만들기\t\t(공사중)", "3. 너 내 동료가 되라\t\t(공사중)", "0. 도망가기\t\t\t(공사중)" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public BattleScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new BattleStartCommand()
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
            Manager.Instance.UI.ShowBattleScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
        }
    }
}
