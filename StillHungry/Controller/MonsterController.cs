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
        }
    }
}
