using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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
            mMenuItems.Add("몬스터 턴으로 넘기기");
            mNavigator = new MenuNavigator(mMenuItems.Count);
            mMenuCommands = new IExecutable[]
            {
                new ChangeSceneCommand(ESceneType.MONSTER_PHASE_SCENE)
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
            Manager.Instance.UI.PlayerTurnScreen(mMenuItems.ToArray(), mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }        
    }
}
