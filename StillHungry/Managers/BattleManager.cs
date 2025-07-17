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
            monsterKillCount = 0; //전투 시작될때 다시 0으로 출력
            mCurrentMonsterIndex = 0;
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
                
                GetPlayerExp();
                
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
                        var player = Manager.Instance.Game.PlayerController;
                        Random rand = new Random();

                        if (rand.NextDouble() < player.EvasionChance)
                        {
                            Console.WriteLine("공격을 회피했습니다!");
                        }
                        else
                        {
                            player.TakeDamage(LastAction.Value);
                            Console.WriteLine($"{LastAction.Value}의 피해를 입었습니다!");
                        }
                        // 씬 갱신
                        if (Manager.Instance.Scene.CurrentScene is MonsterPhaseScene scene)
                        {
                            scene.RequestRedraw();
                        }

                        return; // 한 몬스터의 행동 후 정지
                    }
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


        #region 박용규 추가 메소드
        // 몬스터에게 데미지를 주는 메소드
        public void AttackEnemy(int monsterId) 
        {
            // 몬스터 컨트롤러에 정의 되어있는 TakeDamage를 그대로 재사용
            Manager.Instance.Battle.MonsterController.TakeDamage(monsterId, (int)Manager.Instance.Game.PlayerController.Attack);
        }
        // 플레이어의 경험치 획득 처리 메소드
        public void GetPlayerExp()
        {
            int rewardExp = 0;
            foreach (var m in Manager.Instance.Battle.MonsterController.ActiveMonsters) 
            {
                // 몬스터별 획득 경험치를 불러와서 합산
                rewardExp += m.ExpReward;
            }
            Console.WriteLine($"경험치를 \u001b[33m{rewardExp}\u001b[0m 획득 했습니다.\n");
            // 플레이어 컨트롤러에 있는 경험치 컨트롤 메서드에 값을 넘겨줌
            Manager.Instance.Game.PlayerController.SetExp(rewardExp);
        }
        #endregion
    }
}
