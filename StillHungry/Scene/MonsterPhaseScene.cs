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
    class MonsterPhaseScene : BaseScene
    {
        private readonly string[] mMenuItems = { "0. 다음" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public MonsterPhaseScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new MonsterPhaseCommand()
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

            // BattleManager로부터 현재 공격 정보를 가져와 UI에 전달
            var attacker = Manager.Instance.Battle.CurrentAttacker;
            var lastAction = Manager.Instance.Battle.LastAction;
            var player = Manager.Instance.Game.PlayerController;

            // UI 매니저에 필요한 정보를 모두 넘겨 화면을 그리도록 요청
            Manager.Instance.UI.ShowMonsterPhaseScreen(mMenuItems, mNavigator.SelectedIndex, attacker, lastAction, player);

            bNeedsRedraw = false;
        }

        protected override void Update()
        {
            
        }
    }
}
