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
    internal class PlayerAttackScene : BaseScene
    {
        private readonly List<string> mMenuItems = new List<string>();
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public PlayerAttackScene()
        {
            mMenuItems.Add("0. 돌아가기");
            mNavigator = new MenuNavigator(mMenuItems.Count);
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
            Manager.Instance.UI.PlayerTurnScreen(mMenuItems.ToArray(), mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
        }
    }
}
