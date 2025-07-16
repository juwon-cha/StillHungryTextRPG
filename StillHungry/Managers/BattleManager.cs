using StillHungry.Controller;
using StillHungry.Monsters;
using StillHungry.Scene;
using System.Numerics;

namespace StillHungry.Managers
{
    public class BattleManager
    {
        public int monsterKillCount = 0;

        //public PlayerController PlayerController = new PlayerController();
        public MonsterController MonsterController = new MonsterController();
        public bool isFighting = false;


        public int initialHP; //전투 시작 시의 플레이어 체력
        public int totalDamageTaken = 0; //전투 중 받은 누적 피해
        

        private int mCurrentMonsterIndex = -1; // 몬스터 인덱스

        // UI에 표시할 몬스터의 공격 결과 정보
        public Monster CurrentAttacker { get; private set; }
        public MonsterAction LastAction { get; private set; }


        public BattleManager() 
        {
            if (MonsterController.ActiveMonsters.Count == 0) 
                MonsterController.SpawnMonstersForBattles(); 
        }

        public void startBattle() //전투 시작 시 호출될 함수
        {
            isFighting = true;
            initialHP = Manager.Instance.Game.PlayerController.HP;
            totalDamageTaken = 0;
        }


        // 수정
        public void EndBattle(bool isVictory, int initialHP, int damageTaken, int monsterKillCount)
        {
            
            var player = Manager.Instance.Game.PlayerController;

            Console.Clear();
            Console.WriteLine("Battle!! - Result\n");

            if (isVictory)
            {
                Console.WriteLine("Victory\n");
                Console.WriteLine($"던전에서 몬스터 {monsterKillCount}마리를 잡았습니다.");
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {initialHP} -> {player.HP}\n");
                Console.WriteLine("0. 다음");
                Console.Write("\n>>");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    isFighting = false;
                    Manager.Instance.Scene.ChangeScene(ESceneType.TOWN_SCENE);
                }
            }
            else
            {
                Console.WriteLine("You Lose\n");
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {initialHP} -> {player.HP}\n");
                Console.WriteLine("0. 다음");
                Console.Write("\n>>");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    Console.WriteLine("\n게임 종료");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            }
        }
        // 수정





        public void StartMonsterPhase()
        {
            mCurrentMonsterIndex = -1;
            Console.Clear();
            Console.WriteLine("\n[몬스터의 턴]");
            Thread.Sleep(1000);
            NextMonsterAttack();
        }

        public void NextMonsterAttack()
        {
            // 다음 살아있는 몬스터 찾기
            for (int i = mCurrentMonsterIndex + 1; i < MonsterController.ActiveMonsters.Count; i++)
            {
                var monster = MonsterController.ActiveMonsters[i];
                if (!monster.IsDead)
                {
                    mCurrentMonsterIndex = i;
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
            Console.WriteLine("\n모든 몬스터의 턴이 끝났습니다. 당신의 차례입니다.");
            // TODO: 플레이어 사망 체크
            //if (PlayerController.IsDead())
            //{ 
            //    Manager.Instance.Scene.ChangeScene(ESceneType.DUNGEON_SCENE);
            //}

            Thread.Sleep(1000); // 잠시 메시지를 보여줌

            // TODO: 플레이어 공격 턴으로 전환
            //Manager.Instance.Scene.ChangeScene();
        }

        public void MonsterAttack(int monsterID, int damage) 
        {
            MonsterController.TakeDamage(monsterID, damage);
        }
    }
}

