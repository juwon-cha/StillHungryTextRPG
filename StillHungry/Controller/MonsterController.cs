using StillHungry.Managers;
using StillHungry.Monsters;

namespace StillHungry.Controller
{
    public enum EMonsterActionType
    {   
        NONE, 
        ATTACK, 
        DEFEND 
    }

    public class MonsterAction
    {
        public EMonsterActionType Type { get; set; }
        public int Value { get; set; } // 공격 시: 데미지, 방어 시: 방어력
    }

    public class MonsterController
    {
        private Random mRand = new Random();

        public List<Monster> ActiveMonsters = new List<Monster>();

        // 전투를 위한 몬스터 스폰
        public void SpawnMonstersForBattles()
        {
            ActiveMonsters.Clear();
            // 1~3마리의 몬스터를 생성
            int numberOfMonsters = mRand.Next(1, 4); 

            for (int i = 1; i <= numberOfMonsters; i++)
            {
                // TODO: 랜덤 몬스터 스폰
                ActiveMonsters.Add(Monster.SpawnMonster(i));
            }
            Console.WriteLine($"\n{numberOfMonsters}마리의 몬스터가 나타났습니다!");
        }

        public void TakeDamage(int monsterIndex, int damage)
        {
            if (monsterIndex < 0 || monsterIndex >= ActiveMonsters.Count)
            {
                return;
            }

            Monster targetMonster = ActiveMonsters[monsterIndex];
            if (targetMonster.IsDead)
            {
                return;
            }

            int finalDamage = damage - (int)targetMonster.DefensePower;
            if (finalDamage < 1)
            {
                finalDamage = 1;
            }

            targetMonster.CurrentHp -= finalDamage;

            Console.WriteLine($"{targetMonster.Name}에게 {finalDamage}의 데미지를 입혔습니다!");

            if (targetMonster.CurrentHp <= 0)
            {
                OnMonsterDeath(targetMonster);
            }
        }

        public MonsterAction DecideAndExecuteAction(Monster monster)
        {
            if (monster.IsDead) return new MonsterAction { Type = EMonsterActionType.NONE };

            // 이전 턴의 방어 상태 초기화
            if (monster.IsDefending)
            {
                monster.DefensePower /= 2;
                monster.IsDefending = false;
            }

            int actionChance = mRand.Next(1, 101); // 1~100

            // 70% 확률로 공격
            if (actionChance <= 70)
            {
                // 공격 실행 및 결과 반환
                // 데미지는 BattleManager에서 플레이어에게 직접 적용
                return new MonsterAction { Type = EMonsterActionType.ATTACK, Value = (int)monster.AttackPower};
            }
            // 30% 확률로 방어
            else
            {
                // 방어 실행 및 결과 반환
                monster.IsDefending = true;
                monster.DefensePower *= 2;
                return new MonsterAction { Type = EMonsterActionType.DEFEND, Value = (int)monster.DefensePower};
            }
        }

        private void OnMonsterDeath(Monster monster)
        {
            monster.IsDead = true;
            monster.CurrentHp = 0;
            Console.WriteLine($"{monster.Name}을(를) 처치했습니다!");

            ActiveMonsters.Remove(monster);

            // TODO: 경험치 획득, 아이템 드랍 로직 추가

            Manager.Instance.Battle.monsterKillCount++;
            // a
            // 모든 몬스터를 처치했는지 확인
            if (ActiveMonsters.Count == 0)
            {
                Console.WriteLine("모든 몬스터를 처치했습니다! 전투에서 승리했습니다!");
                
                Thread.Sleep(1000); // 승리 메시지 후 잠시 대기

                var battleManager = Manager.Instance.Battle;
                battleManager.EndBattle
                    (
                    isVictory: true,
                    initialHP: battleManager.initialHP,
                    damageTaken: battleManager.totalDamageTaken,
                    monsterKillCount: battleManager.monsterKillCount
                    );

                // 전투 종료 후 던전 씬으로 전환
                //Manager.Instance.Scene.ChangeScene(ESceneType.DUNGEON_SCENE);
            }
            // a
        }


        //public Monster GetMonsterFromID(int monsterId)
        //{
        //    if(ActiveMonsters.TryGetValue(monsterId, out Monster monster))
        //    {
        //        return monster;
        //    }

        //    return null; // 해당 ID의 몬스터가 없으면 null을 반환합니다.

        //}
    }
}
