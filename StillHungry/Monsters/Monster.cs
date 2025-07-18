using StillHungry.Data;
using StillHungry.Managers;

namespace StillHungry.Monsters
{
    public class Monster
    {
        public int ID { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }
        public float AttackPower { get; set; }
        public float DefensePower { get; set; }
        public int ExpReward { get; set; } // 몬스터 처치 시 주는 경험치
        public int GoldReward { get; set; } // 몬스터 처치 시 주는 경험치
        public bool IsDefending { get; set; } = false;
        public bool IsDead { get; set; }

        public Monster(int id)
        {
            Init(id);
        }

        private void Init(int id)
        {
            MonsterStat monsterData;
            if (DataManager.MonsterStatDict.TryGetValue(id, out monsterData))
            {
                ID = monsterData.ID;
                Level = monsterData.Level;
                Name = monsterData.Name;
                MaxHp = monsterData.MaxHp;
                CurrentHp = MaxHp; // 초기 체력은 최대 체력으로 설정
                AttackPower = monsterData.Attack;
                DefensePower = monsterData.Defense;
                ExpReward = monsterData.ExpReward;
                GoldReward = monsterData.GoldReward;
            }
            else
            {
                throw new KeyNotFoundException($"Monster with ID {id} not found.");
            }
        }

        public static Monster SpawnMonster(int monsterId)
        {
            Monster monster = null;
            MonsterStat monsterStat;

            // json 데이터에서 몬스터 정보 가져옴
            if (DataManager.MonsterStatDict.TryGetValue(monsterId, out monsterStat))
            {
                monster = new Monster(monsterId);
            }
            else
            {
                throw new KeyNotFoundException($"Monster with ID {monsterId} not found.");
            }

            return monster;
        }
    }
}
