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
        public int MaxHp;
        public int Attack;
        public int Defense;
        public int TotalExp;
        public int Gold;
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

    public class ItemLoader : ILoader<int, ItemData>
    {
        public List<WeaponData> Weapons = new List<WeaponData>();
        public List<ArmorData> Armors = new List<ArmorData>();

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
}
