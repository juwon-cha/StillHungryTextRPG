using StillHungry.Commands;
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
        private readonly string[] mMenuItems = {"1. 축축한동굴", "2. 건조한 풀밭",
            "3. 돌 산맥", "4. 용암이 흐르는 계곡", "5. 레드드래곤의 둥지", "0. 나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public DungeonScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new EnterDungeonCommand(EDungeonLevel.DAMP_CAVE, RequestRedraw),
                new EnterDungeonCommand(EDungeonLevel.DRY_GRASS, RequestRedraw),
                new EnterDungeonCommand(EDungeonLevel.STONE_MOUNTAIN, RequestRedraw),
                new EnterDungeonCommand(EDungeonLevel.LAVA_VALLEY, RequestRedraw),
                new EnterDungeonCommand(EDungeonLevel.RED_DRAGON_NEST, RequestRedraw),
                new ChangeSceneCommand(ESceneType.TOWN_SCENE)
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
            Manager.Instance.UI.ShowDungeonScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
        }
    }
}