using StillHungry.Commands;
using StillHungry.Data;
using StillHungry.Managers;
using StillHungry.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.Scene
{
    internal class QuestScene : BaseScene
    {
        private readonly string[] mMenuItems;
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public QuestScene()
        {
            mMenuItems = new string[DataManager.QuestDataDict.Count() + 1];
            int i = 0;
            foreach (QuestData quest in DataManager.QuestDataDict.Values)
            {
                mMenuItems[i] = $"{i + 1}. "+ quest.Name;
                i++;
            }
            mMenuItems[i] = "0. 나가기"; // 마지막에 나가기 메뉴 추가
            
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[mMenuItems.Length];
            for(int j = 0; j < mMenuItems.Length - 1; j++)
            {
                mMenuCommands[j] = new QuestSceneCommand(RequestRedraw, i);
            }
            mMenuCommands[mMenuCommands.Length - 1] = new ChangeSceneCommand(ESceneType.TOWN_SCENE);
            
        }
        public override void Display()
        {
            Update();
            ProcessInput(mMenuCommands, mNavigator);
            int i = mNavigator.SelectedIndex;
            Render();
        }

        public override void Render()
        {
            if (!bNeedsRedraw)
            {
                return;
            }

            Console.Clear();
            Manager.Instance.UI.QuestScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {

        }
    }
}
