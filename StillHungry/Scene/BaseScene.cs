using StillHungry.Commands;
using StillHungry.UI;
using StillHungry.Utils;

namespace StillHungry.Scene
{
    public abstract class BaseScene
    {
        internal bool bNeedsRedraw = true; // 화면을 처음에 그릴 필요가 있음

        // 자식 씬들이 반드시 각자 구현해야 하는 부분들
        public abstract void Display();
        protected abstract void Update(); // 애니메이션이나 상태 업데이트가 필요하다면 여기에 구현
        public abstract void Render(); // UI 출력

        // 입력 처리 메소드
        protected void ProcessInput(IExecutable[] menuCommands, MenuNavigator navigator)
        {
            if (!Console.KeyAvailable) return;

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                // Enter가 눌리면, 네비게이터의 현재 인덱스에 맞는 커맨드 실행
                menuCommands[navigator.SelectedIndex].Execute();
            }
            else
            {
                // 다른 키(방향키)가 눌리면 네비게이터가 인덱스를 변경
                if (navigator.Navigate(keyInfo.Key))
                {
                    bNeedsRedraw = true; // 인덱스가 변경되었으므로 화면 갱신 요청
                }
            }
        }

        public void RequestRedraw()
        {
            bNeedsRedraw = true;
        }
    }
}