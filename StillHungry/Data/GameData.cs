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
        public List<PlayerStat> Stats = new List<PlayerStat>();

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
        public EDungeonLevel Level;
        public int RecommendedDefense;
        public int BaseRewardGold;
    }

    public class DungeonDataLoader : ILoader<string, DungeonData>
    {
        public List<DungeonData> Dungeons = new List<DungeonData>();

        public Dictionary<string, DungeonData> MakeData()
        {
            Dictionary<string, DungeonData> dict = new Dictionary<string, DungeonData>();

            foreach (DungeonData dungeonData in Dungeons)
            {
                dict.Add(dungeonData.Level.ToString(), dungeonData);
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
