using StillHungry.Commands;
using StillHungry.Data;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public class SaveLoadScene : BaseScene
    {
        private string[] mMenuItems = new string[4];
        private IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;
        private bool mbIsSavingMode; // false: Load, true: Save

        public SaveLoadScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            // 메뉴 아이템 기본 텍스트 초기화
            mMenuItems[0] = "[슬롯 1]";
            mMenuItems[1] = "[슬롯 2]";
            mMenuItems[2] = "[슬롯 3]";
            mMenuItems[3] = "나가기";
        }

        // 외부에서 호출하여 저장/불러오기 모드를 설정하는 메서드
        public void SetMode(bool isSavingMode)
        {
            mbIsSavingMode = isSavingMode;
            UpdateCommands();
            // 모드가 변경되었으므로 화면을 다시 그려야 함을 표시
            bNeedsRedraw = true;
        }

        // 모드에 따라 커맨드를 업데이트하는 내부 메서드
        private void UpdateCommands()
        {
            if (mbIsSavingMode)
            {
                // "저장하기" 모드일 때의 커맨드
                mMenuCommands = new IExecutable[]
                {
                    new SaveSlotCommand(1), // 슬롯 1 저장
                    new SaveSlotCommand(2), // 슬롯 2 저장
                    new SaveSlotCommand(3), // 슬롯 3 저장
                    new ChangeSceneCommand(ESceneType.TOWN_SCENE) // 마을로 돌아가기
                };
            }
            else
            {
                // "불러오기" 모드일 때의 커맨드
                mMenuCommands = new IExecutable[]
                {
                    new LoadSlotCommand(1), // 슬롯 1 불러오기
                    new LoadSlotCommand(2), // 슬롯 2 불러오기
                    new LoadSlotCommand(3), // 슬롯 3 불러오기
                    new ChangeSceneCommand(ESceneType.TITLE_SCENE) // 타이틀로 돌아가기
                };
            }

            // 네비게이터가 알고 있는 메뉴 개수도 업데이트
            //mNavigator.SetMenuCount(mMenuCommands.Length);
        }

        public override void Display()
        {
            Update();
            ProcessInput(mMenuCommands, mNavigator);
            Render();
        }

        public override void Render()
        {
            if (!bNeedsRedraw) return;

            Console.Clear();
            // UIManager를 통해 화면을 그림
            Manager.Instance.UI.ShowSaveLoadScreen(mMenuItems, mNavigator.SelectedIndex, mbIsSavingMode);
            bNeedsRedraw = false;
        }

        protected override void Update()
        {
        }
    }
}