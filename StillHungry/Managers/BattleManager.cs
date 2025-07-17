using StillHungry.Controller;
using StillHungry.Monsters;
using StillHungry.Scene;
using System.Numerics;

namespace StillHungry.Managers
{
    public class BattleManager
    {
        public int monsterKillCount = 0;

        public MonsterController MonsterController = new MonsterController();
        public bool IsFighting = false;
        public int SelectedMonsterID { get; set; } = 0;

        public int InitialHP; //전투 시작 시의 플레이어 체력
        public int TotalDamageTaken = 0; //전투 중 받은 누적 피해

        private int mCurrentMonsterIndex; // 몬스터 인덱스

        // UI에 표시할 몬스터의 공격 결과 정보
        public Monster CurrentAttacker { get; private set; }
        public MonsterAction LastAction { get; private set; }

        public void SpawnMonsters() 
        {
            MonsterController.SpawnMonstersForBattles(); 
        }

        public void StartBattle() //전투 시작 시 호출될 함수
        {
            IsFighting = true;
            InitialHP = Manager.Instance.Game.PlayerController.HP;
            TotalDamageTaken = 0;
        }

        public void EndBattle(bool isVictory, int initialHP, int damageTaken, int monsterKillCount)
        {
            var player = Manager.Instance.Game.PlayerController;
            IsFighting = false;

            Console.Clear();
            Console.WriteLine("Battle!! - Result\n");

            if (isVictory)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Victory\n");
                Console.ResetColor();

                Console.WriteLine($"던전에서 몬스터 {monsterKillCount}마리를 잡았습니다.");
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {initialHP} -> {player.HP}\n");

                Console.WriteLine("던전 입구로 돌아가려면 아무 키나 누르세요.");
                Console.ReadKey();

                Manager.Instance.Scene.ChangeScene(ESceneType.DUNGEON_SCENE);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You Lose\n");
                Console.ResetColor();

                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {initialHP} -> {player.HP}\n");
                Console.WriteLine("던전 입구로 돌아가려면 아무 키나 누르세요.");
                Console.ReadKey();

                Manager.Instance.Scene.ChangeScene(ESceneType.DUNGEON_SCENE);
            }
        }

        public void StartMonsterPhase()
        {
            Console.Clear(); // 이전 결과 지우기

            // 다음 살아있는 몬스터 찾기
            for (int i = mCurrentMonsterIndex; i < MonsterController.ActiveMonsters.Count; i++)
            {
                var monster = MonsterController.ActiveMonsters[i];
                if (!monster.IsDead)
                {
                    mCurrentMonsterIndex = ++i;
                    CurrentAttacker = monster;

                    // MonsterController에 행동 결정 및 실행 요청
                    LastAction = MonsterController.DecideAndExecuteAction(monster);

                    // 행동 결과에 따라 플레이어에게 데미지 적용
                    if (LastAction.Type == EMonsterActionType.ATTACK)
                    {
                        Manager.Instance.Game.PlayerController.TakeDamage(LastAction.Value);
                    }

                    // 씬 갱신
                    if (Manager.Instance.Scene.CurrentScene is MonsterPhaseScene scene)
                    {
                        scene.RequestRedraw();
                    }

                    return; // 한 몬스터의 행동 후 정지
                }
            }

            // 모든 몬스터의 턴이 끝났을 경우
            EndMonsterPhase();
        }

        private void EndMonsterPhase()
        {
            // 모든 몬스터의 턴이 끝났으므로 인덱스 및 공격 몬스터 초기화
            mCurrentMonsterIndex = 0;
            CurrentAttacker = null;
            LastAction = null;

            Console.WriteLine("\n모든 몬스터의 턴이 끝났습니다. 당신의 차례입니다.");
            Console.WriteLine("계속 하려면 아무 키나 누르세요.");
            Console.ReadKey();

            // 플레이어 공격 턴으로 전환
            Manager.Instance.Scene.ChangeScene(ESceneType.ATTACK_SELECT_SCENE);
        }

        public void MonsterAttack(int monsterID, int damage) 
        {
            MonsterController.TakeDamage(monsterID, damage);
        }
    }
}
