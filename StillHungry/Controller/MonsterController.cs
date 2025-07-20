using StillHungry.Managers;
using StillHungry.Monsters;
using StillHungry.Scene;

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

            var dungeonLevel = Manager.Instance.Dungeon.CurrentDungeonID;

            int minMonsterLevel = 0;
            int maxMonsterLevel = 0;

            // 던전 레벨에 따라 몬스터 레벨 범위 설정
            if (dungeonLevel >= 1 && dungeonLevel <= 4)
            {
                minMonsterLevel = 1 + (dungeonLevel - 1) * 3;
                maxMonsterLevel = minMonsterLevel + 2;
            }
            else if (dungeonLevel == 5)
            {
                minMonsterLevel = 99;
                maxMonsterLevel = 100;
            }
            else
            {
                // 예외 처리: 범위 밖의 던전 레벨
                minMonsterLevel = 1;
                maxMonsterLevel = 3;
            }

            // 레벨 범위에 해당하는 모든 몬스터의 ID 리스트를 만듦
            List<int> possibleMonsterIds = DataManager.MonsterStatDict.Values
                .Where(m => m.Level >= minMonsterLevel && m.Level <= maxMonsterLevel)
                .Select(m => m.ID)
                .ToList();

            if (possibleMonsterIds.Count == 0)
            {
                return;
            }

            // TODO: 레드 드래곤이 있는 던전은 항상 1마리 스폰하도록 처리
            if (possibleMonsterIds[0] == 100)
            {
                ActiveMonsters.Add(Monster.SpawnMonster(100)); // 레드 드래곤은 항상 1마리 스폰
                return;
            }

            // 1~4마리의 몬스터를 랜덤하게 결정하고, 중복을 허용하여 스폰
            int numberOfMonsters = mRand.Next(1, 5); // 1, 2, 3, 4 중 하나의 숫자를 반환
            for (int i = 0; i < numberOfMonsters; i++)
            {
                // 몬스터 후보 리스트에서 무작위로 하나의 ID를 선택
                int randomIndex = mRand.Next(0, possibleMonsterIds.Count);
                int randomMonsterId = possibleMonsterIds[randomIndex];
                ActiveMonsters.Add(Monster.SpawnMonster(randomMonsterId));
            }
        }

        public void TakeDamage(Monster targetMonster, int damage, bool isCritical)
        {
            if (targetMonster == null && targetMonster.IsDead)
            {
                return;
            }

            int finalDamage = damage - (int)targetMonster.DefensePower;
            if (finalDamage < 1)
            {
                finalDamage = 1;
            }

            targetMonster.CurrentHp -= finalDamage;
            targetMonster.DamageTaken += finalDamage;

            if (targetMonster.CurrentHp <= 0)
            {
                OnMonsterDeath(targetMonster);
            }
        }

        // 몬스터 턴에서 공격 또는 방어 자세 선택
        // 공격이면 BattleManager의 StartMonsterPhase메서드에서 플레이어 바로 공격
        // 방어라면 방어력 올라가고 턴 종료 -> 다음 플레이어가 몬스터 공격 시 몬스터가 받는 데미지 반감
        // 몬스터 턴이 다시 시작되면 방어 상태 초기화
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
            Manager.Instance.Game.PlayerController.QuestKillCountUp(monster.Name);

            // TODO: 경험치 획득, 아이템 드랍 로직 추가?

            Manager.Instance.Battle.MonsterKillCount++;
            bool isAllMonsterDead = true;
            foreach (Monster m in ActiveMonsters) 
            {
                if (!m.IsDead) {
                    isAllMonsterDead = false;
                    break;
                }
            }
        }

        public Monster GetMonsterFromList(int index)
        {
            if (index < 0 || index >= ActiveMonsters.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");
            }

            return ActiveMonsters.ElementAt(index);
        }

        public Monster GetMonsterFromID(int monsterId)
        {
            return ActiveMonsters.FirstOrDefault(m => m.ID == monsterId); 
            // 해당 ID의 몬스터가 없으면 null을 반환
        }

    }
}
