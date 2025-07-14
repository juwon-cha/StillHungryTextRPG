using StillHungry.Controller;
using StillHungry.Data;
using StillHungry.Items;

namespace StillHungry.Managers
{
    // 게임 매니저에서 플레이어 관리
    class GameManager
    {
        enum SlotID { ID = 1 }

        public PlayerController PlayerController = new PlayerController();

        // 플레이어 데이터 설정
        public void SetPlayerData(string playerName, EClassType classType)
        {
            PlayerController.Init(playerName, classType);
        }

        public void SaveGame()
        {
            UserData data = CreateSaveData();

            DataManager.SaveUserData(data, "SaveData/UserData");
        }

        public UserData CreateSaveData()
        {
            // TODO: 저장/로드 슬롯 시스템 구현
            // 최대 3개 까지 게임 상황을 저장/로드할 수 있는 슬롯 시스템
            // 저장/로드 슬롯을 선택할 수 있는 씬 추가

            UserData userData = new UserData();
            userData.SlotID = (int)SlotID.ID;
            userData.ClassType = PlayerController.ClassType;
            userData.Level = PlayerController.Level;
            userData.Name = PlayerController.Name;
            userData.HP = PlayerController.HP;
            userData.Attack = PlayerController.BaseAttack;
            userData.Defense = PlayerController.BaseDefense;
            userData.Gold = PlayerController.Gold;

            // 유저가 소유하고 있는 아이템 정보(아이디, 장착여부) 저장
            var inventory = PlayerController.InventoryController.Inventory;
            if (inventory.Count != 0)
            {
                userData.Items = new List<UserItemData>();

                foreach (KeyValuePair<int, Item> item in inventory)
                {
                    userData.Items.Add(new UserItemData(item.Value.ID, item.Value.HasEquipped));
                }
            }

            return userData;
        }

        public bool LoadGame()
        {
            DataManager.LoadUserData();
            if (DataManager.SaveDataDict == null || DataManager.SaveDataDict.Count == 0)
            {
                return false; // 저장된 데이터 없음
            }

            // TODO: 슬롯 시스템 구현하면서 수정 필요
            UserData data;
            if(DataManager.SaveDataDict.TryGetValue((int)SlotID.ID, out data))
            {
                PlayerController.LoadPlayerSettings(data);
                return true;
            }

            return false;
        }

        public void ExitGame()
        {
            // TODO: 게임 종료 전 필요한 모든 정리 작업 수행
            // 예를 들어, 마지막 데이터 저장 등
            Console.WriteLine("\n게임 종료");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }
    }
}
