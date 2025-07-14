using StillHungry.Data;
using StillHungry.Managers;
using StillHungry.Controller;

namespace StillHungry.Items
{
    public enum EItemType
    {
        ITEM_NONE = 0,
        ITEM_WEAPON,
        ITEM_ARMOR,
        ITEM_CONSUMABLE
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
                case EItemType.ITEM_CONSUMABLE:
                    item = new Consumable(itemId);
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

    public class Consumable : Item
    {
        public int HealAmount { get; private set; }
        public Consumable(int id) : base(EItemType.ITEM_CONSUMABLE)
        {
            Init(id);
        }
        private void Init(int id)
        {
            ItemData itemData = null;
            DataManager.ItemDict.TryGetValue(id, out itemData);
            if (itemData.ItemType != EItemType.ITEM_CONSUMABLE)
            {
                return;
            }
            ConsumableData data = (ConsumableData)itemData;
            ID = data.ID;
            Name = data.Name;
            HealAmount = data.Recovery;
            Price = data.Price;
            Description = data.Description;
            HasPurchased = false;
            SellingPrice = (int)(Price * SELLING_PRICE_PERCENTAGE);
        }
    }
}
