using StillHungry.Controller;
using StillHungry.Items;
using StillHungry.Managers;
using StillHungry.Scene;

namespace StillHungry.Data
{
    #region PlayerStat
    public struct PlayerStat
    {
        public EClassType ClassType;
        public int Level;
        public int HP;
        public int MaxHp;
        public int Mana;
        public int MaxMana; //최대 마나
        public int Attack;
        public int Defense;
        public int TotalExp;
        public int Gold;
        public float CriticalRate;
        public float EvadeRate;
    }

    public class PlayerStatLoader : ILoader<EClassType, PlayerStat>
    {
        // Newtonsoft.Json 라이브러리는 JSON 파일의 키 이름과 역직렬화 대상 클래스(PlayerStatLoader)의 프로퍼티(변수) 이름을 비교하여 자동으로 데이터를 매핑
        // JSON의 키 이름과 일치해야 함
        public List<PlayerStat> Stats = new List<PlayerStat>();

        // JSON 데이터가 PlayerStatLoader 객체의 List들에 모두 채워지면,
        // LoadJson 메서드는 이 객체를 반환한다. 이어서 .MakeData()가 호출된다.

        // MakeData()의 역할은 데이터를 사용하기 좋은 형태로 가공하는 것
        // Dictionary에 stat.ClassType를 키로 저장해두면 데이터를 빠르게 찾을 수 있다.
        // 배열이나 리스트로 저장하면 찾을 때마다 순차적으로 탐색해야 하므로 비효율적이다.
        public Dictionary<EClassType, PlayerStat> MakeData()
        {
            Dictionary<EClassType, PlayerStat> dict = new Dictionary<EClassType, PlayerStat>();

            foreach(PlayerStat stat in Stats)
            {
                dict.Add(stat.ClassType, stat);
            }

            return dict;
        }
    }
    #endregion

    #region Item
    public class ItemData
    {
        public int ID;
        public string Name;
        public EItemType ItemType;
        public int Price;
        public string Description;
    }

    public class WeaponData : ItemData
    {
        public EWeaponType WeaponType;
        public int Damage;
    }

    public class ArmorData : ItemData
    {
        public int Defense;
    }

    public class ConsumableHPData : ItemData
    {
        public int HPRecovery;
    }

    public class ConsumableMPData : ItemData
    {
        public int MPRecovery;
    }
    
    public class EtcItemData : ItemData
    {
        
    }
    public class FoodData : ItemData
    {
        public int Damage;
        public int Defense;
        public float Critical; // 치명타 확률
        public float Evade;  // 회피 확률
        public int Skill;
    }

    public class ItemLoader : ILoader<int, ItemData>
    {
        public List<WeaponData> Weapons = new List<WeaponData>();
        public List<ArmorData> Armors = new List<ArmorData>();
        public List<ConsumableHPData> ConsumableHP = new List<ConsumableHPData>();
        public List<ConsumableMPData> ConsumableMP = new List<ConsumableMPData>();
        public List<FoodData> EtcItems = new List<FoodData>();
        public List<FoodData> Foods = new List<FoodData>();

        public Dictionary<int, ItemData> MakeData()
        {
            Dictionary<int, ItemData> dict = new Dictionary<int, ItemData>();
            foreach (ItemData item in Weapons)
            {
                item.ItemType = EItemType.ITEM_WEAPON;
                dict.Add(item.ID, item);
            }

            foreach (ItemData item in Armors)
            {
                item.ItemType = EItemType.ITEM_ARMOR;
                dict.Add(item.ID, item);
            }

            foreach (ItemData itemData in ConsumableHP)
            {
                itemData.ItemType = EItemType.ITEM_CONSUMABLEHP;
                dict.Add(itemData.ID, itemData);
            }

            foreach (ItemData itemData in ConsumableMP)
            {
                itemData.ItemType = EItemType.ITEM_CONSUMABLEMP;
                dict.Add(itemData.ID, itemData);
            }

            foreach (ItemData itemData in EtcItems)
            {
                itemData.ItemType = EItemType.ITEM_ETC;
                dict.Add(itemData.ID, itemData);
            }

            foreach (ItemData itemData in Foods)
            {
                itemData.ItemType = EItemType.ITEM_FOOD;
                dict.Add(itemData.ID, itemData);
            }

            return dict;
        }
    }
    #endregion

    #region Dungeon
    public struct DungeonData
    {
        public int ID;
        public string Name;
        public int RecommendedDefense;
        public int BaseRewardGold;
    }

    public class DungeonDataLoader : ILoader<int, DungeonData>
    {
        public List<DungeonData> Dungeons = new List<DungeonData>();

        public Dictionary<int, DungeonData> MakeData()
        {
            Dictionary<int, DungeonData> dict = new Dictionary<int, DungeonData>();

            foreach (DungeonData dungeonData in Dungeons)
            {
                dict.Add(dungeonData.ID, dungeonData);
            }

            return dict;
        }
    }
    #endregion

    #region MonsterStat
    public struct MonsterStat
    {
        public int ID;
        public int Level;
        public string Name;
        public int MaxHp;
        public int Attack;
        public int Defense;
        public int ExpReward;
        public int GoldReward;
    }

    public class MonsterStatLoader : ILoader<int, MonsterStat>
    {
        public List<MonsterStat> MonsterStats = new List<MonsterStat>();

        public Dictionary<int, MonsterStat> MakeData()
        {
            Dictionary<int, MonsterStat> dict = new Dictionary<int, MonsterStat>();

            foreach (MonsterStat monster in MonsterStats)
            {
                dict.Add(monster.ID, monster);
            }

            return dict;
        }
    }
    #endregion

    #region 퀘스트
    public class QuestData
    {
        public int ID;
        public string Name;
        public string Speech;
        public string Clear;
        public string Target;
        public int Detail;
        public int PayID;
        public bool isAchievement = false; // 목표 달성 여부
        public bool isClear = false; // 퀘스트 완료 여부
    }

    public class QuestDataLoader : ILoader<int, QuestData>
    {
        public List<QuestData> Quest = new List<QuestData>();
        
        public Dictionary<int, QuestData> MakeData()
        {
            Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();

            foreach (QuestData quest in Quest)
            {
                
                dict.Add(quest.ID, quest);
            }

            return dict;
        }
    }
    #endregion

    #region 스킬
    public struct SkillData
    {
        public int ID;
        public string Name;
        public string Description;
        public int RequiredMP;
        public float DamageMultiplier;
        public float DefenseMultiplier;
        public float CriticalMultiplier;
        public float EvadeMultiplier;
        public bool IsRangeAttack;
    }

    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> Active = new List<SkillData>();

        public Dictionary<int, SkillData> MakeData()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in Active)
            {
                dict.Add(skill.ID, skill);
            }

            return dict;
        }
    }
    #endregion
}
