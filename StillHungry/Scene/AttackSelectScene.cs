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
    internal class AttackSelectScene : BaseScene
    {
        private readonly List<string> mMenuItems = new List<string>();
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public AttackSelectScene()
        {
            int idx = 0;
            foreach (var m in Manager.Instance.Battle.MonsterController.ActiveMonsters)
            {
                mMenuItems.Add($"");
                    idx++;
            }
            mMenuItems.Add($"");
            mNavigator = new MenuNavigator(mMenuItems.Count);

            mMenuCommands = new IExecutable[]
            {
                new PlayerTurnCommand(),
                new PlayerTurnCommand(),
                new PlayerTurnCommand(),
                new EnterDungeonCommand(),
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
            Manager.Instance.UI.ShowAttackSelect(mMenuItems.ToArray(), mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
        }
    }
 }
