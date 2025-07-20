using StillHungry.Commands;
using StillHungry.Controller;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public class CharacterSettingScene : BaseScene
    {
        public enum ESettingState { EnterName, SelectClass }
        private ESettingState mCurrentState = ESettingState.EnterName;

        private readonly string[] mMenuItems = { "전사", "마법사", "궁수", "도적" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public CharacterSettingScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new SelectClassCommand(EClassType.WARRIOR, () => Manager.Instance.Game.PlayerController.Name, SetSettingState),
                new SelectClassCommand(EClassType.MAGICIAN, () => Manager.Instance.Game.PlayerController.Name, SetSettingState),
                new SelectClassCommand(EClassType.ARCHER, () => Manager.Instance.Game.PlayerController.Name, SetSettingState),
                new SelectClassCommand(EClassType.THIEF, () => Manager.Instance.Game.PlayerController.Name, SetSettingState)
            };
        }

        public override void Display()
        {
            Render();

            // 상태에 따라 다른 입력 처리 로직 호출
            if (mCurrentState == ESettingState.EnterName)
            {
                // InputNameCommand에 상태 변경 콜백과 전달
                new InputNameCommand(SetSettingState).Execute();
            }
            else
            {
                ProcessInput(mMenuCommands, mNavigator);
            }
        }

        public override void Render()
        {
            if (!bNeedsRedraw) return;

            Console.Clear();
            if (mCurrentState == ESettingState.EnterName)
            {
                Manager.Instance.UI.ShowNameSettingScreen();
            }
            else
            {
                Manager.Instance.UI.ShowClassSettingScreen(Manager.Instance.Game.PlayerController.Name, mMenuItems, mNavigator.SelectedIndex);
            }
            bNeedsRedraw = false;
        }

        // 씬의 상태를 설정할 수 있는 메서드(이름 선택, 직업 선택)
        public void SetSettingState(ESettingState state)
        {
            mCurrentState = state;
            bNeedsRedraw = true; // 상태 변경 시 화면 갱신 요청
        }
    }
}
