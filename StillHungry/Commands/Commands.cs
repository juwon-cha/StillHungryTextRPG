using StillHungry.Controller;
using StillHungry.Data;
using StillHungry.Items;
using StillHungry.Managers;
using StillHungry.Monsters;
using StillHungry.Scene;
using StillHungry.Skills;
using StillHungry.Utils;

namespace StillHungry.Commands
{
    #region 게임 라이프사이클
    public class NewGameCommand : IExecutable
    {
        public void Execute()
        {
            Manager.Instance.Scene.ChangeScene(ESceneType.CHARACTER_SETTING_SCENE);
        }
    }

    // TitleScene에서 "불러오기" 메뉴를 눌렀을 때 호출될 커맨드
    public class LoadGameCommand : IExecutable
    {
        public void Execute()
        {
            // SceneManager를 통해 SaveLoadScene 인스턴스를 가져옴
            var saveLoadScene = Manager.Instance.Scene.GetScene(ESceneType.SAVELOAD_SCENE) as SaveLoadScene;
            // "불러오기" 모드로 설정
            saveLoadScene?.SetMode(false);
            // 씬 전환
            Manager.Instance.Scene.ChangeScene(ESceneType.SAVELOAD_SCENE);
        }
    }

    // TownScene에서 "저장하기" 메뉴를 눌렀을 때 호출될 커맨드
    public class SaveGameCommand : IExecutable
    {
        public void Execute()
        {
            var saveLoadScene = Manager.Instance.Scene.GetScene(ESceneType.SAVELOAD_SCENE) as SaveLoadScene;
            // "저장하기" 모드로 설정
            saveLoadScene?.SetMode(true);
            Manager.Instance.Scene.ChangeScene(ESceneType.SAVELOAD_SCENE);
        }
    }

    public class SaveSlotCommand : IExecutable
    {
        private int slotIndex;

        public SaveSlotCommand(int slot)
        {
            slotIndex = slot;
        }

        public void Execute()
        {
            // 현재 슬롯에 데이터가 있는지 확인
            if (DataManager.UserSlots.ContainsKey(slotIndex))
            {
                Console.Clear();
                Console.WriteLine($"슬롯 {slotIndex}에 이미 데이터가 있습니다. 덮어쓰시겠습니까? (y/n)");
                string input = Console.ReadLine()?.ToLower();
                if (input != "y")
                {
                    // 사용자가 y를 입력하지 않으면 저장 작업을 중단하고 씬을 다시 그림
                    Manager.Instance.Scene.CurrentScene.RequestRedraw();
                    return;
                }
            }

            // 게임 저장
            Manager.Instance.Game.SaveGame(slotIndex);

            Console.WriteLine($"슬롯 {slotIndex}에 게임을 저장했습니다.\n");
            Console.WriteLine("아무 키나 누르면 마을로 돌아갑니다...");
            Console.ReadKey();
            // 저장 후 마을로 돌아감
            Manager.Instance.Scene.ChangeScene(ESceneType.TOWN_SCENE);
        }
    }

    public class LoadSlotCommand : IExecutable
    {
        private int slotIndex;

        public LoadSlotCommand(int slot)
        {
            slotIndex = slot;
        }

        public void Execute()
        {
            // 지정된 슬롯에서 유저 데이터 불러오기 시도
            if (DataManager.LoadUserDataFromSlot(slotIndex))
            {
                // 성공 시, GameManager에 플레이어 데이터 로드를 요청
                Manager.Instance.Game.LoadPlayerData();
                Console.WriteLine($"슬롯 {slotIndex}의 데이터를 불러왔습니다!");
                Console.WriteLine("아무 키나 누르면 게임을 시작합니다...");
                Console.ReadKey();
                // 게임 시작 (TownScene으로 이동)
                Manager.Instance.Scene.ChangeScene(ESceneType.TOWN_SCENE);
            }
            else
            {
                // 데이터가 없는 슬롯일 경우
                Console.WriteLine("해당 슬롯에 데이터가 없습니다.");
                Console.WriteLine("아무 키나 누르면 다시 선택합니다...");
                Console.ReadKey();
                // 현재 씬(SaveLoadScene)을 다시 그리도록 요청
                Manager.Instance.Scene.CurrentScene.RequestRedraw();
            }
        }
    }

    public class ExitGameCommand : IExecutable
    {
        public void Execute()
        {
            Manager.Instance.Game.ExitGame();
        }
    }
    #endregion

    #region 씬 전환
    public class ChangeSceneCommand : IExecutable
    {
        private readonly ESceneType mSceneType;

        public ChangeSceneCommand(ESceneType sceneType)
        {
            mSceneType = sceneType;
        }

        public void Execute()
        {
            Manager.Instance.Scene.ChangeScene(mSceneType);
        }
    }
    #endregion



    #region 캐릭터 설정
    public class InputNameCommand : IExecutable
    {
        private readonly Action<CharacterSettingScene.ESettingState> mSetStateCallback;

        public InputNameCommand(Action<CharacterSettingScene.ESettingState> setStateCallback)
        {
            mSetStateCallback = setStateCallback;
        }

        public void Execute()
        {
            Console.CursorVisible = true;
            string name = null;
            bool nameEntered = false;

            while (!nameEntered)
            {
                Console.Clear();
                Manager.Instance.UI.ShowNameSettingScreen();
                name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("이름을 입력해야 합니다.");
                    Thread.Sleep(1000);
                    continue;
                }

                Console.WriteLine("\n1. 저장");
                Console.WriteLine("2. 취소");
                Console.Write(">> ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    Manager.Instance.Game.PlayerController.SetPlayerName(name);
                    mSetStateCallback?.Invoke(CharacterSettingScene.ESettingState.SelectClass); // 직업 선택으로 넘어감
                    nameEntered = true;
                }
                else if (choice == "2")
                {
                    Console.WriteLine("이름 입력을 취소했습니다. 다시 입력해주세요.");
                    Thread.Sleep(1000);
                    continue;
                }
                else
                {
                    Console.WriteLine("잘못된 선택입니다. 다시 입력해주세요.");
                    Thread.Sleep(1000);
                    continue;
                }
            }
            Console.CursorVisible = false;
        }
    }

    public class SelectClassCommand : IExecutable
    {
        private readonly EClassType mClassType;
        private readonly Func<string> mGetPlayerName;
        private readonly Action<CharacterSettingScene.ESettingState> mSetStateCallback;

        public SelectClassCommand(EClassType classType, Func<string> getPlayerName, Action<CharacterSettingScene.ESettingState> setStateCallback)
        {
            mClassType = classType;
            mGetPlayerName = getPlayerName;
            mSetStateCallback = setStateCallback;
        }

        public void Execute()
        {
            string name = mGetPlayerName();
            string className = StringConverter.ClassTypeToString(mClassType);

            Console.Clear();
            Manager.Instance.UI.ShowClassSettingScreen(name, new string[] { $"선택된 직업: {className}" }, 0); // 선택된 직업 표시
            Console.WriteLine("\n1. 저장");
            Console.WriteLine("2. 취소");
            Console.Write(">> ");
            Console.CursorVisible = true;
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                // 게임 매니저에서 플레이어 정보 세팅
                Manager.Instance.Game.SetPlayerData(name, mClassType);

                Console.WriteLine($"\n{name}({className})님, 노아의 안식처로 여정을 시작합니다!");
                Thread.Sleep(2000);
                Manager.Instance.Scene.ChangeScene(ESceneType.STORY_SCENE);
            }
            else if (choice == "2")
            {
                Console.WriteLine("직업 선택을 취소했습니다. 다시 선택해주세요.");
                Thread.Sleep(1000);
                mSetStateCallback?.Invoke(CharacterSettingScene.ESettingState.SelectClass); // 직업 선택 상태로 유지
            }
            else
            {
                Console.WriteLine("잘못된 선택입니다. 다시 선택해주세요.");
                Thread.Sleep(1000);
                mSetStateCallback?.Invoke(CharacterSettingScene.ESettingState.SelectClass); // 직업 선택 상태로 유지
            }
            Console.CursorVisible = false;
        }
    }
    #endregion

    #region 휴식과 던전
    public class RestCommand : IExecutable
    {
        const int REST_COST = 500; // 휴식 비용

        private readonly Action mRequestRedrawCallback;

        public RestCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            var player = Manager.Instance.Game.PlayerController;
            ERestResult result = player.RestAtCampsite(REST_COST);
            string message = result switch
            {
                ERestResult.NOT_ENOUGH_GOLD => "Gold가 부족합니다.",
                ERestResult.FULL_HP => "체력이 이미 가득 찼습니다.",
                ERestResult.SUCCESS => "체력이 모두 회복되었습니다.",
                _ => "알 수 없는 오류가 발생했습니다."
            };
            Console.WriteLine(message);
            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }

    public class EnterDungeonCommand : IExecutable
    {
        private readonly int dungeonID;
        private readonly Action mRequestRedrawCallback;

        public EnterDungeonCommand(int ID, Action requestRedrawCallback)
        {
            dungeonID = ID;
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            // 던전 정보 설정
            var dungeon = Manager.Instance.Dungeon;
            dungeon.CurrentDungeonID = dungeonID;
            dungeon.TryEnterDungeon(dungeonID);

            // 던전 입장 시 몬스터 소환
            var battle = Manager.Instance.Battle;
            battle.SpawnMonsters();
            battle.InitialHP = Manager.Instance.Game.PlayerController.HP; // 던전 입장 시 플레이어의 초기 체력 저장
            battle.MonsterKillCount = 0; // 던전 입장 시 몬스터 킬 카운트 초기화

            // 공격 선택 커맨드 생성
            AttackSelectScene scene = Manager.Instance.Scene.GetScene(ESceneType.ATTACK_SELECT_SCENE) as AttackSelectScene;
            scene.GenerateAttackSelectCommands();

            if (Manager.Instance.Game.PlayerController.HP <= 0)
            {
                Console.WriteLine("던전에 입장할 힘이 남아 있지 않습니다.");
                Thread.Sleep(1000);
                mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
                return;
            }
            Console.Clear();

            Manager.Instance.Scene.ChangeScene(ESceneType.BATTLE_SCENE);
        }
    }
    #endregion

    #region 상점 및 인벤토리
    // 장비 구매
    public class BuyEquipmentCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public BuyEquipmentCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }


        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("장비 상점 - 아이템 구매");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 구매할 수 있습니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Manager.Instance.Game.PlayerController.Gold} G\n");

            Manager.Instance.UI.PrintStoreItemList(Manager.Instance.Item.EquipmentItems);

            Console.Write("\n구매할 아이템의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();
            var storeItems = Manager.Instance.Item.EquipmentItems;
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= storeItems.Count)
            {
                Item itemToBuy = storeItems.Values.ElementAt(choice - 1);
                if (itemToBuy != null)
                {
                    EPurchaseResult result = Manager.Instance.Game.PlayerController.BuyItem(itemToBuy);
                    string message = result switch
                    {
                        EPurchaseResult.SUCCESS => "구매를 완료했습니다.",
                        EPurchaseResult.NOT_ENOUGH_GOLD => "Gold가 부족합니다.",
                        EPurchaseResult.ALREADY_PURCHASED => "이미 구매한 아이템입니다.",
                        _ => "구매에 실패했습니다."
                    };
                    Console.WriteLine(message);
                }
            }
            else if (choice != 0)
            {
                Console.WriteLine("잘못된 입력입니다.");
            }

            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }
    // 소모품 구매
    public class BuyConsumableCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public BuyConsumableCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }


        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("소모품 상점 - 아이템 구매");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 구매할 수 있습니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Manager.Instance.Game.PlayerController.Gold} G\n");

            Manager.Instance.UI.PrintStoreItemList(Manager.Instance.Item.ConsumableItems);

            Console.Write("\n구매할 아이템의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();

            var storeItems = Manager.Instance.Item.ConsumableItems;
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= storeItems.Count)
            {
                // 구매할 아이템의 갯수 입력
                Console.Write("\n\n구매할 아이템의 갯수를 입력해주세요 (0: 나가기) >> ");
                string input1 = Console.ReadLine();
                if(int.TryParse(input1, out int count) && count > 0)
                {
                    Item itemToBuy = storeItems.Values.ElementAt(choice - 1);
                    if (itemToBuy != null)
                    {
                        EPurchaseResult result = Manager.Instance.Game.PlayerController.BuyItem(itemToBuy, count);
                        string message = result switch
                        {
                            EPurchaseResult.SUCCESS => $"{count}개를 구매했습니다.",
                            EPurchaseResult.NOT_ENOUGH_GOLD => "Gold가 부족합니다.",
                            _ => "구매에 실패했습니다."
                        };
                        itemToBuy.HasPurchased = false;
                        Console.WriteLine(message);
                    }
                }
                else if (count != 0)
                {
                    Console.WriteLine("잘못된 입력입니다.");
                }

            }
            else if (choice != 0)
            {
                Console.WriteLine("잘못된 입력입니다.");
            }

            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }
    // 장비 판매
    public class SellItemCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public SellItemCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("상점 - 아이템 판매");
            Console.ResetColor();
            Console.WriteLine("가지고 있는 아이템을 판매할 수 있습니다.\n");

            Manager.Instance.UI.PrintInventoryList(true);

            Console.Write("\n판매할 아이템의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();
            var inventory = Manager.Instance.Game.PlayerController.InventoryController;
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= inventory.EquipInventory.Count)
            {
                Item itemToSell = inventory.EquipInventory.Values.ElementAt(choice - 1);
                if (itemToSell != null)
                {
                    ESellResult result = Manager.Instance.Game.PlayerController.SellItem(itemToSell);
                    string message = result == ESellResult.SUCCESS ? "판매를 완료했습니다." : "판매에 실패했습니다.";
                    Console.WriteLine(message);
                }
            }
            else if (choice != 0)
            {
                Console.WriteLine("잘못된 입력입니다.");
            }
            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }
    // 소모품 판매
    public class SellConsumableCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public SellConsumableCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("상점 - 아이템 판매");
            Console.ResetColor();
            Console.WriteLine("가지고 있는 아이템을 판매할 수 있습니다.\n");

            Manager.Instance.UI.PrintInventoryList(true, true);

            Console.Write("\n판매할 아이템의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();
            var inventory = Manager.Instance.Game.PlayerController.InventoryController;
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= inventory.ConsumableInventory.Count)
            {
                Item itemToSell = inventory.ConsumableInventory.Values.ElementAt(choice - 1);
                if (itemToSell != null)
                {
                    ESellResult result = Manager.Instance.Game.PlayerController.SellItem(itemToSell);
                    string message = result == ESellResult.SUCCESS ? "판매를 완료했습니다." : "판매에 실패했습니다.";
                    Console.WriteLine(message);
                    itemToSell.HasEquipped = false;
                }
            }
            else if (choice != 0)
            {
                Console.WriteLine("잘못된 입력입니다.");
            }
            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }

    public class BuyFoodCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public BuyFoodCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;

        }


        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("요리 상점 - 식사");
            Console.ResetColor();
            Console.WriteLine("해금된 요리를 구매하실 수 있습니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Manager.Instance.Game.PlayerController.Gold} G\n");


            Manager.Instance.UI.PrintFoodItemList(Manager.Instance.Game.PlayerController.InventoryController.FoodInventory);

            Console.Write("\n구매할 요리의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();
            var storeItems = Manager.Instance.Game.PlayerController.InventoryController.FoodInventory;
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= storeItems.Count)
            {
                Item itemToBuy = storeItems.Values.ElementAt(choice - 1);
                if (itemToBuy != null)
                {
                    EPurchaseResult result = Manager.Instance.Game.PlayerController.BuyFood(itemToBuy);
                    string message = result switch
                    {
                        EPurchaseResult.SUCCESS => "구매를 완료했습니다.",
                        EPurchaseResult.NOT_ENOUGH_GOLD => "Gold가 부족합니다.",
                        EPurchaseResult.ALREADY_PURCHASED => $"이미 {Manager.Instance.Game.PlayerController.EatFood.Name}을 먹었습니다.",
                        _ => "구매에 실패했습니다."
                    };
                    if (result == EPurchaseResult.SUCCESS)
                    {
                        Manager.Instance.Game.PlayerController.EatFood = (Food)itemToBuy;
                        Manager.Instance.Game.PlayerController.RecalculateFoodStats();
                    }
                    itemToBuy.HasPurchased = false;
                    Console.WriteLine(message);
                }
            }
            else if (choice != 0)
            {
                Console.WriteLine("잘못된 입력입니다.");
            }

            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }

    // 장비 창고
    public class EquipManageCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public EquipManageCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("인벤토리 - 장착 관리");
            Console.ResetColor();
            Console.WriteLine("보유 중인 아이템을 장착하거나 해제할 수 있습니다.\n");

            Manager.Instance.UI.PrintInventoryList();

            Console.Write("\n장착/해제할 아이템의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();
            var player = Manager.Instance.Game.PlayerController;
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= player.InventoryController.EquipInventory.Count)
            {
                Item itemToEquip = player.InventoryController.EquipInventory.Values.ElementAt(choice - 1);
                if (itemToEquip != null)
                {
                    player.EquipItem(itemToEquip);
                    Console.WriteLine($"{itemToEquip.Name}을(를) {(itemToEquip.HasEquipped ? "장착" : "해제")}했습니다.");
                }
            }
            else if (choice != 0)
            {
                Console.WriteLine("잘못된 입력입니다.");
            }

            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }


    //소모품 창고
    public class ConsumableManageCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public ConsumableManageCommand(Action requestRedrawCallback, bool isInventory = true)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("인벤토리 - 사용 하기");
            Console.ResetColor();
            Console.WriteLine("보유 중인 아이템을 사용할 수 있습니다.\n");

            Manager.Instance.UI.PrintInventoryList(false, true);

            Console.Write("\n사용할 아이템의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();
            var player = Manager.Instance.Game.PlayerController;
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= player.InventoryController.ConsumableInventory.Count)
            {
                Item itemToEquip = player.InventoryController.ConsumableInventory.Values.ElementAt(choice - 1);
                if (itemToEquip != null)
                {
                    //player.EquipItem(itemToEquip);
                    Console.WriteLine($"{itemToEquip.Name}을(를) 사용했습니다.");
                    if (itemToEquip.ID >= 200 && itemToEquip.ID < 300)
                        player.UseHPRecovery((ConsumableHP)itemToEquip); // 아이템 사용 시 HP 회복
                    else if (itemToEquip.ID >= 300 && itemToEquip.ID < 400)
                        player.UseMPRecovery((ConsumableMP)itemToEquip); // 아이템 사용 시 MP 회복
                    else
                        Console.WriteLine("해당 아이템은 사용할 수 없습니다.");
                }
            }
            else if (choice != 0)
            {
                Console.WriteLine("잘못된 입력입니다.");
            }
            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }
    //기타 창고
    public class EtcManageCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public EtcManageCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("인벤토리 - 기타 아이템");
            Console.ResetColor();
            Console.WriteLine("보유 중인 아이템을 확인할 수 있습니다.\n");

            Manager.Instance.UI.PrintInventoryList(false, true);

            Console.Write("\n사용할 아이템의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();
            var player = Manager.Instance.Game.PlayerController;
            if (int.TryParse(input, out int choice) && choice == 0)
            {
                Console.WriteLine("창고를 나갑니다.");
            }
            else if (choice != 0)
            {
                Console.WriteLine("잘못된 입력입니다.");
            }
            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }
    #endregion

    #region 퀘스트
    public class QuestSceneCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;
        private readonly QuestData focusQuest;

        public QuestSceneCommand(Action requestRedrawCallback, int key)
        {
            mRequestRedrawCallback = requestRedrawCallback;
            focusQuest = DataManager.QuestDataDict[key];
        }


        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"퀘스트 - {focusQuest.Name}");
            Console.ResetColor();
            Console.WriteLine();

            // (퀘스트ID = 1 || 이전 퀘스트가 클리어 된 경우) && (현재 퀘스트를 안받은 경우 || 현재 퀘스트랑 들어온 퀘스트랑 똑같을 때 || 현재 퀘스트가 클리어 된 상황)
            if ((focusQuest.ID == 1 || DataManager.QuestDataDict[focusQuest.ID - 1].isClear) && (Manager.Instance.Game.PlayerController.LiveQuest == null || Manager.Instance.Game.PlayerController.LiveQuest == focusQuest || focusQuest.isClear))
            {
                // 퀘스트가 클리어된 경우
                if (focusQuest.isClear)
                {
                    Console.WriteLine("\n이미 클리어한 퀘스트라네");
                    Thread.Sleep(1000);
                    mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
                    return;
                }
                // 퀘스트 목표를 달성한 경우
                else if (Manager.Instance.Game.PlayerController.LiveQuest == focusQuest)
                {
                    focusQuest.isAchievement = true;
                    if (focusQuest.isAchievement)
                    {
                        focusQuest.isClear = true; // 퀘스트 완료 처리
                        Console.WriteLine(focusQuest.Clear);

                        Console.WriteLine();
                        Console.WriteLine($"해금 - {Manager.Instance.Item.FoodItems[focusQuest.PayID].Name}");
                        Manager.Instance.Game.PlayerController.InventoryController.FoodInventory.Add(focusQuest.PayID, Manager.Instance.Item.FoodItems[focusQuest.PayID]);


                        Console.Write("\n0. 나가기 ");
                        Console.Write("\n번호를 입력해주세요 >> ");
                        string input = Console.ReadLine();

                        Manager.Instance.Game.PlayerController.LiveQuest = null; // 퀘스트 완료 후 라이브 퀘스트 초기화
                        Thread.Sleep(1000);
                        mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
                        return;
                    }
                }

                Console.WriteLine(focusQuest.Speech);
                Console.WriteLine();
                Console.WriteLine("목표");
                Console.WriteLine($"{focusQuest.Target} {Manager.Instance.Game.PlayerController.currentQuestKillCount} / {focusQuest.Detail}");
                // 똑같은 퀘스트를 들어 왔을 때
                if (Manager.Instance.Game.PlayerController.LiveQuest == focusQuest)
                {

                    Console.Write("1. 퀘스트 완료\n0. 나가기 ");
                    Console.Write("\n번호를 입력해주세요 >> ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int choice) && choice == 1)
                    {
                        focusQuest.isAchievement = true;
                    }

                }
                else
                {
                    Console.Write("\n1. 수락하기\n2. 거절하기 ");
                    Console.Write("\n번호를 입력해주세요 >> ");
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out int choice) && choice == 1)
                    {
                        Console.WriteLine($"고맙네 역시 자네 뿐이야.");
                        Manager.Instance.Game.PlayerController.LiveQuest = focusQuest;
                    }
                    else if (choice == 2)
                    {
                        Console.WriteLine("생각이 바뀌면 다시 찾아오게.");
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다.");
                    }
                }
            }
            // 이전 퀘스트를 완료하지 않은 경우
            else if (!DataManager.QuestDataDict[focusQuest.ID - 1].isClear)
            {
                Console.WriteLine("\n이전 퀘스트 먼저 깨고 오게나.");
            }

            Thread.Sleep(1000);
            mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
        }
    }
    #endregion

    #region  전투커맨드
    public class BattleStartCommand : IExecutable
    {
        private readonly int mDungeonID;
        public BattleStartCommand(int dungeonID)
        {
            mDungeonID = dungeonID;
        }

        public void Execute()
        {
            // 전투 시작 시 던전 레벨 세팅
            Manager.Instance.Battle.StartBattle(mDungeonID);

            Manager.Instance.Scene.ChangeScene(ESceneType.ATTACK_SELECT_SCENE);
        }
    }

    public class AttackSelectCommand : IExecutable
    {
        private readonly int mSelectedIndex;

        public AttackSelectCommand(int index)
        {
            mSelectedIndex = index;
        }

        public void Execute()
        {
            Monster monster = Manager.Instance.Battle.MonsterController.GetMonsterFromList(mSelectedIndex);
            Manager.Instance.Battle.AttackMonster(monster);
            if(!Manager.Instance.Battle.IsFighting)
            {
                // 전투 종료 후 던전 입구로 돌아가기
                Manager.Instance.Scene.ChangeScene(ESceneType.DUNGEON_SCENE);
                return;
            }

            Manager.Instance.Scene.ChangeScene(ESceneType.PLAYER_ATTACK_SCENE);
        }
    }

    public class MonsterPhaseCommand : IExecutable
    {
        public void Execute()
        {
            Manager.Instance.Battle.StartMonsterPhase();
        }
    }

    public class SkillSelectCommand : IExecutable
    {
        public void Execute()
        {
            SkillSelectScene scene = Manager.Instance.Scene.GetScene(ESceneType.SKILL_SELECT_SCENE) as SkillSelectScene;
            scene.GenerateSkillSelectCommands(); // 공격 선택 커맨드 생성

            Manager.Instance.Scene.ChangeScene(ESceneType.SKILL_SELECT_SCENE);
        }
    }

    public class UseSkillCommand : IExecutable
    {
        private readonly Skill mSkill;

        public UseSkillCommand(Skill skill)
        {
            mSkill = skill;
        }

        public void Execute()
        {
            var scene = Manager.Instance.Scene;

            bool isRange = Manager.Instance.Game.PlayerController.UseSkill(mSkill);

            // 범위 공격
            if (isRange)
            {
                // 플레이어 공격 씬으로 전환해서 몬스터 공격
                scene.ChangeScene(ESceneType.PLAYER_ATTACK_SCENE);
            }
            else // 단일 공격
            {
                // 몬스터 공격 선택 씬으로 전환해서 단일 스킬 적용 몬스터 선택

                scene.ChangeScene(ESceneType.ATTACK_SELECT_SCENE);
            }
        }
    }
    #endregion
}