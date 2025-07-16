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

        // 새로운 캐릭터 생성 시 데이터 설정
        public void SetPlayerData(string playerName, EClassType classType)
        {
            PlayerController.Init(playerName, classType);
            // DataManager의 현재 유저 정보도 생성된 캐릭터로 설정
            DataManager.CurrentUser = CreateSaveData();
        }

        public void SaveGame(int slotIndex)
        {
            DataManager.CurrentUser = Manager.Instance.Game.CreateSaveData();
            DataManager.SaveCurrentUserData(slotIndex);
        }

        // 저장할 유저 데이터 생성
        public UserData CreateSaveData()
        {
            UserData userData = new UserData();
            userData.SlotID = (int)SlotID.ID;
            userData.ClassType = PlayerController.ClassType;
            userData.Level = PlayerController.Level;
            userData.Name = PlayerController.Name;
            userData.HP = PlayerController.HP;
            userData.Mana = PlayerController.Mana;
            userData.Attack = PlayerController.BaseAttack;
            userData.Defense = PlayerController.BaseDefense;
            userData.Gold = PlayerController.Gold;
            userData.CriticalRate = PlayerController.CriticalChance;
            userData.EvadeRate = PlayerController.EvasionChance;

            // 유저가 소유하고 있는 아이템 정보(아이디, 장착여부) 저장
            var inventory = PlayerController.InventoryController.EquipInventory;
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

        // 슬롯에서 불러온 데이터로 플레이어 정보 설정
        public void LoadPlayerData()
        {
            if (DataManager.CurrentUser != null)
            {
                PlayerController.SetPlayerSettingsFromLoadData(DataManager.CurrentUser);
            }
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
