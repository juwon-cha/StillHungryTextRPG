using StillHungry.Commands;
using StillHungry.Data;
using StillHungry.Items;

namespace StillHungry.Managers
{
    class ItemManager
    {
        public Dictionary<int, Item> EquipmentItems = new Dictionary<int, Item>();   // 장착 아이템 
        public Dictionary<int, Item> ConsumableItems = new Dictionary<int, Item>();  // 소모품 아이템
        public Dictionary<int, Item> EtcItems = new Dictionary<int, Item>();         // 기타 아이템 

        public void Init()
        {
            // 아이템 추가
            foreach(int itemId in DataManager.ItemDict.Keys)
            {
                Item item = Item.MakeItem(itemId);
                if(item != null)
                {
                    Add(item);
                }
            }
        }

        public void Add(Item item)
        {
            if (item.ID < 200)
            {
                EquipmentItems.Add(item.ID, item);
            }
            else if(item.ID < 400)
            {
                ConsumableItems.Add(item.ID, item);
            }
            else if(item.ID < 500)
            {
                EtcItems.Add(item.ID, item);
            }

        }

        // 아이템 아이디로 아이템 가져옴
        public Item GetItemFromID(int itemId)
        {
            Item item = null;
            if (itemId < 200)
                EquipmentItems.TryGetValue(itemId, out item);
            else if (itemId < 400)
                ConsumableItems.TryGetValue(itemId, out item);
            else if (itemId < 500)
                EtcItems.TryGetValue(itemId, out item);

            return item;
        }

        // 상점에 배치된 리스트의 번호로 아이템 가져옴
        // 상점에 배치된 아이템 번호와 아이템 아이디는 다름
        public Item GetItemFromStoreList(int listNumber)
        {
            // listNumber는 1부터 시작하므로, 0부터 시작하는 인덱스로 변환
            int index = listNumber - 1;

            // 인덱스가 유효한 범위 내에 있는지 확인
            if (index < 0 || index >= EquipmentItems.Count)
            {
                return null; // 잘못된 번호일 경우 null 반환
            }

            // 딕셔너리의 값들을 순서대로 가져와서 해당 인덱스의 아이템을 반환
            // LINQ의 ElementAt()을 사용하여 N번째 요소를 찾음
            return EquipmentItems.Values.ElementAt(index);
        }
    }
}
