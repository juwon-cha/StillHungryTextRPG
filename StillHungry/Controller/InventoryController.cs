using StillHungry.Items;

namespace StillHungry.Controller
{
    class InventoryController
    {
        public Dictionary<int, Item> Inventory = new Dictionary<int, Item>();
        public Dictionary<int, Item> ConsumableInventory = new Dictionary<int, Item>();


        public void AddItem(Item item)
        {
            if(item == null)
            {
                return;
            }

            if (item.ID < 200)
            {
                Inventory.Add(item.ID, item);
            }
            else if (item.ID < 400)
            {
                // 이미 있으면 수량 증가, 없으면 추가
                if (ConsumableInventory.TryGetValue(item.ID, out var existItem))
                {
                    existItem.Quantity += 1;
                }
                else
                {
                    item.Quantity = 1;
                    ConsumableInventory.Add(item.ID, item);
                }
            }

                
        }

        public bool RemoveItem(Item item)
        {
            if(item == null)
            {
                return false;
            }

            // 키 존재해서 성공적으로 제거하면 true 반환, 아니라면  false 반환
            if (item.ID < 200)
            {
                return Inventory.Remove(item.ID);
            }
            else if (item.ID < 400)
            {
                if (ConsumableInventory.TryGetValue(item.ID, out var existItem))
                {
                    existItem.Quantity -= 1;
                    if (existItem.Quantity <= 0)
                    {
                        return ConsumableInventory.Remove(item.ID);
                    }
                    return true;
                }
            }
            return false;
        }

        // 상점에 배치된 리스트의 번호로 인벤토리에 있는 아이템 가져옴
        // 상점에 배치된 아이템 번호와 아이템 아이디는 다름
        public Item GetItemFromInventory(int listNumber)
        {
            // listNumber는 1부터 시작하므로, 0부터 시작하는 인덱스로 변환
            int index = listNumber - 1;

            // 인덱스가 유효한 범위 내에 있는지 확인
            if (index < 0 || index >= Inventory.Count)
            {
                return null; // 잘못된 번호일 경우 null 반환
            }

            // 딕셔너리의 값들을 순서대로 가져와서 해당 인덱스의 아이템을 반환
            // LINQ의 ElementAt()을 사용하여 N번째 요소를 찾음
            return Inventory.Values.ElementAt(index);
        }

        public void ClearInventory()
        {
            Inventory.Clear();
            ConsumableInventory.Clear();
        }
    }
}
