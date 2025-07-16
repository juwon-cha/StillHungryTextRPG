using StillHungry.Data;
using StillHungry.Managers;

namespace StillHungry.Items
{
    public enum EItemType
    {
        ITEM_NONE = 0,
        ITEM_WEAPON,
        ITEM_ARMOR,
        ITEM_CONSUMABLEHP,
        ITEM_CONSUMABLEMP,
        ITEM_ETC,
        ITEM_FOOD
    }

    public enum EWeaponType
    {
        WEAPON_NONE = 0,
        WEAPON_SWORD,
        WEAPON_BOW,
        WEAPON_WAND,
        WEAPON_DAGGER
    };

    public class Item
    {
        public EItemType ItemType { get; private set; }
        public int ID { get; protected set; }
        public string Name { get; protected set; }
        public int Price { get; protected set; }
        public string Description { get; protected set; }

        protected const float SELLING_PRICE_PERCENTAGE = 0.85f;
        public int SellingPrice { get; protected set; }
        public bool HasSold { get; set; }
        public bool HasEquipped { get; set; }
        public bool HasPurchased { get; set; }

        public int Quantity { get; set; } // 소모품 아이템의 개수
        public Item(EItemType type)
        {
            ItemType = type;
        }

        public static Item MakeItem(int itemId)
        {
            Item item = null;
            ItemData itemData = null;

            // json 데이터에서 아이템 정보 가져옴
            DataManager.ItemDict.TryGetValue(itemId, out itemData);
            if (itemData == null)
            {
                return null;
            }

            switch (itemData.ItemType)
            {
                case EItemType.ITEM_WEAPON:
                    item = new Weapon(itemId);
                    break;
                case EItemType.ITEM_ARMOR:
                    item = new Armor(itemId);
                    break;
                case EItemType.ITEM_CONSUMABLEHP:
                    item = new ConsumableHP(itemId);
                    break;
                case EItemType.ITEM_CONSUMABLEMP:
                    item = new ConsumableMP(itemId);
                    break;
                case EItemType.ITEM_FOOD:
                    item = new Food(itemId);
                    break;
                case EItemType.ITEM_NONE:
                    break;
            }

            return item;
        }
    }

    public class Weapon : Item
    {
        public EWeaponType WeaponType { get; private set; }
        public int Damage { get; private set; }

        public Weapon(int id) : base(EItemType.ITEM_WEAPON)
        {
            Init(id);
        }

        private void Init(int id)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(id, out itemData);
            if (itemData.ItemType != EItemType.ITEM_WEAPON)
            {
                return;
            }

            WeaponData data = (WeaponData)itemData;

            ID = data.ID;
            Name = data.Name;
            WeaponType = data.WeaponType;
            Damage = data.Damage;
            Price = data.Price;
            Description = data.Description;

            HasPurchased = false;
            SellingPrice = (int)(Price * SELLING_PRICE_PERCENTAGE);
        }
    }

    public class Armor : Item
    {
        public int Defense { get; private set; }

        public Armor(int id) : base(EItemType.ITEM_ARMOR)
        {
            Init(id);
        }

        private void Init(int id)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(id, out itemData);
            if (itemData.ItemType != EItemType.ITEM_ARMOR)
            {
                return;
            }

            ArmorData data = (ArmorData)itemData;

            ID = data.ID;
            Name = data.Name;
            Defense = data.Defense;
            Price = data.Price;
            Description = data.Description;

            HasPurchased = false;
            SellingPrice = (int)(Price * SELLING_PRICE_PERCENTAGE);
        }
    }

    public class ConsumableHP : Item
    {
        public int HPRecovery { get; private set; }
        public ConsumableHP(int id) : base(EItemType.ITEM_CONSUMABLEHP)
        {
            Init(id);
        }
        private void Init(int id)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(id, out itemData);
            if (itemData.ItemType != EItemType.ITEM_CONSUMABLEHP)
            {
                return;
            }
            ConsumableHPData data = (ConsumableHPData)itemData;
            ID = data.ID;
            Name = data.Name;
            HPRecovery = data.HPRecovery;
            Price = data.Price;
            Description = data.Description;
            HasPurchased = false;
            SellingPrice = (int)(Price * SELLING_PRICE_PERCENTAGE);
        }
    }

    public class ConsumableMP : Item
    {
        public int MPRecovery { get; private set; }
        public ConsumableMP(int id) : base(EItemType.ITEM_CONSUMABLEMP)
        {
            Init(id);
        }
        private void Init(int id)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(id, out itemData);
            if (itemData.ItemType != EItemType.ITEM_CONSUMABLEMP)
            {
                return;
            }
            ConsumableMPData data = (ConsumableMPData)itemData;
            ID = data.ID;
            Name = data.Name;
            MPRecovery = data.MPRecovery;
            Price = data.Price;
            Description = data.Description;
            HasPurchased = false;
            SellingPrice = (int)(Price * SELLING_PRICE_PERCENTAGE);
        }
    }

    public class EtcItems : Item
    {
        public int Skill { get; private set; }


        public EtcItems(int id) : base(EItemType.ITEM_ETC)
        {
            Init(id);
        }
        private void Init(int id)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(id, out itemData);
            if (itemData.ItemType != EItemType.ITEM_ETC)
            {
                return;
            }
            FoodData data = (FoodData)itemData; // 고쳐야함 // FoodData로 변경 필요
            ID = data.ID;
            Name = data.Name;
            Price = data.Price;
            Description = data.Description;
            HasPurchased = false;
            SellingPrice = (int)(Price * SELLING_PRICE_PERCENTAGE);

        }
    }
    public class Food : Item
    {
        public int Damage { get; private set; }
        public int Defense { get; private set; }
        public float Critical { get; private set; } // 치명타 확률
        public float Evade { get; private set; }   // 회피 확률
        public int Skill { get; private set; }


        public Food(int id) : base(EItemType.ITEM_FOOD)
        {
            Init(id);
        }
        private void Init(int id)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(id, out itemData);
            if (itemData.ItemType != EItemType.ITEM_FOOD)
            {
                return;
            }
            FoodData data = (FoodData)itemData; // 고쳐야함 // FoodData로 변경 필요
            ID = data.ID;
            Name = data.Name;
            Price = data.Price;
            Description = data.Description;
            HasPurchased = false;
            SellingPrice = (int)(Price * SELLING_PRICE_PERCENTAGE);
        }
    }

}
