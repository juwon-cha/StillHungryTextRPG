using StillHungry.Data;
using StillHungry.Managers;

namespace StillHungry.Monsters
{
    public class Monster
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int MaxHp { get; set; }
        public int CurrentHp { get; set; }
        public float AttackPower { get; set; }
        public float DefensePower { get; set; }
        public bool mbIsDefending { get; set; } = false;

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
                Name = monsterData.Name;
                MaxHp = monsterData.MaxHp;
                CurrentHp = MaxHp; // 초기 체력은 최대 체력으로 설정
                AttackPower = monsterData.Attack;
                DefensePower = monsterData.Defense;
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

            // json 데이터에서 아이템 정보 가져옴
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
