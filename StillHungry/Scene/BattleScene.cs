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
        private readonly string[] mMenuItems = { 
            "싸우다", "스킬", "아이템", "던전 나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public BattleScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new BattleStartCommand(),
                new SkillSelectCommand(),
                new ConsumableManageCommand(RequestRedraw),
                new ChangeSceneCommand(ESceneType.DUNGEON_SCENE),
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
