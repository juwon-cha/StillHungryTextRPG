using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public class InventoryScene : BaseScene
    {
        private readonly string[] mMenuItems = { "장비 창고","소모품 창고","기타 창고", "나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public InventoryScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new EquipManageCommand(RequestRedraw),// 장비 창고
                new ConsumableManageCommand(RequestRedraw), // 소모품 창고
                new EtcManageCommand(RequestRedraw), // 이거 기타창고임  
                new ChangeSceneCommand(ESceneType.TOWN_SCENE) // 나가기
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
            Manager.Instance.UI.ShowInventoryScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }
    }
}