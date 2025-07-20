using StillHungry.Commands;
using StillHungry.Data;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public enum EDungeonLevel
    {
        DAMP_CAVE,
        DRY_GRASS,
        STONE_MOUNTAIN,
        LAVA_VALLEY,
        RED_DRAGON_NEST
    }

    public class DungeonScene : BaseScene
    {
        private readonly string[] mMenuItems; //= {"축축한 동굴", "건조한 풀밭", "돌 산맥", "용암이 흐르는 계곡", "레드 드래곤의 둥지", "나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public DungeonScene()
        {
            mMenuItems = new string[DataManager.DungeonDataDict.Count() + 1];
            int i = 0;
            foreach (DungeonData quest in DataManager.DungeonDataDict.Values)
            {
                mMenuItems[i] = quest.Name;
                i++;
            }
            mMenuItems[i] = "나가기"; // 마지막에 나가기 메뉴 추가

            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[mMenuItems.Length];
            for (int j = 0; j < mMenuItems.Length - 1; j++)
            {
                mMenuCommands[j] = new EnterDungeonCommand(j + 1, RequestRedraw);
            }
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands[mMenuCommands.Length - 1] = new ChangeSceneCommand(ESceneType.TOWN_SCENE);
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
            Manager.Instance.UI.ShowDungeonScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }
    }
}