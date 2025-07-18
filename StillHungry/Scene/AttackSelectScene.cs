using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.Monsters;
using StillHungry.UI;
using System;

namespace StillHungry.Scene
{
    // 이 씬의 선택지들은 동적으로 만들어지는 몬스터 List에 따라 달라짐. 몬스터는 던전을 입장할 때마다 랜덤 스폰됨
    internal class AttackSelectScene : BaseScene
    {
        private List<IExecutable> mMenuCommands = new List<IExecutable>();
        private MenuNavigator mNavigator;

        public AttackSelectScene()
        {
            
        }

        // 몬스터 List 길이 만큼 공격 선택 커맨드 생성
        public void GenerateAttackSelectCommands()
        {
            mMenuCommands.Clear();

            int monsterCount = Manager.Instance.Battle.MonsterController.ActiveMonsters.Count;
            mNavigator = new MenuNavigator(monsterCount + 1);

            for (int i = 0; i < monsterCount; ++i)
            {
                // 죽은 상태의 몬스터를 표시
                if (Manager.Instance.Battle.MonsterController.ActiveMonsters[i].IsDead)
                {
                    // 아무것도 안하는 보이드씬을 생성해서 메뉴에 추가
                    VoidScene voidScene = new VoidScene(mMenuCommands, mNavigator);
                    mMenuCommands.Add(new ChangeSceneCommand(ESceneType.VOID_SCENE));
                }
                else
                {
                    mMenuCommands.Add(new AttackSelectCommand(mNavigator.SelectedIndex));
                }
            }
            // 죽은 몬스터는 선택메뉴에서 위로 올라가게 만들기 **** TODO ****

            mMenuCommands.Add(new ChangeSceneCommand(ESceneType.BATTLE_SCENE));
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
            Manager.Instance.UI.ShowAttackSelect(mNavigator.SelectedIndex);
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
                // 선택한 인덱스 몬스터 List에서 유효한 인덱스인지 확인
                if (navigator.SelectedIndex < Manager.Instance.Battle.MonsterController.ActiveMonsters.Count)
                {
                    // 배틀매니저의 선택된몬스터 ID를 선택된 몬스터의 인덱스값으로 설정
                    Manager.Instance.Battle.SelectedMonsterID = navigator.SelectedIndex;
                }

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
