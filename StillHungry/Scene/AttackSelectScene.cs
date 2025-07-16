using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    internal class AttackSelectScene : BaseScene
    {
        private readonly List<string> mMenuItems = new List<string>();
        private readonly List<IExecutable> mMenuCommands = new List<IExecutable>();
        private readonly MenuNavigator mNavigator;

        public AttackSelectScene()
        {
            int idx = 0;
            foreach (var m in Manager.Instance.Battle.MonsterController.ActiveMonsters)
            {
                mMenuItems.Add($"");
                mMenuCommands.Add(new PlayerTurnCommand());
                idx++;
            }
            mMenuCommands.Add(new EnterDungeonCommand());
            mMenuItems.Add($"");
            mNavigator = new MenuNavigator(mMenuItems.Count);
        }
        public override void Display()
        {
            Update();
            ProcessInput(mMenuCommands.ToArray(), mNavigator);
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
        // 플레이어가 공격할 몬스터의 ID값을 동적으로 얻어오기 위한 메소드(기존의 코드를 재사용)
        private void ProcessInput(IExecutable[] menuCommands, MenuNavigator navigator)
        {
            if (!Console.KeyAvailable) return;
            
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.Enter)
            {
                // 배틀매니저의 선택된몬스터 ID를 선택된 몬스터의 인덱스값으로 설정
                Manager.Instance.Battle.selectedMonsterID = navigator.SelectedIndex;
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
    }
 }
