using StillHungry.Controller;
using StillHungry.Items;
using StillHungry.Managers;
using StillHungry.Scene;
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

    public class LoadGameCommand : IExecutable
    {
        private readonly TitleScene mScene;

        // 불러오기 실패 시 화면을 다시 그려야 하므로, TitleScene에 신호를 보낼 방법이 필요
        public LoadGameCommand(TitleScene scene)
        {
            mScene = scene;
        }

        public void Execute()
        {
            if (Manager.Instance.Game.LoadGame())
            {
                Console.WriteLine("\n게임 데이터를 성공적으로 불러왔습니다.");
                Console.WriteLine("마을로 이동합니다...");
                Thread.Sleep(1500);
                Manager.Instance.Scene.ChangeScene(ESceneType.TOWN_SCENE);
            }
            else
            {
                Console.WriteLine("\n저장된 게임 데이터가 없습니다.");
                Thread.Sleep(1500);
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
                Manager.Instance.Game.SetPlayerData(name, mClassType);
                Console.WriteLine($"\n{name}({className})님, 스파르타 마을로 여정을 시작합니다!");
                Thread.Sleep(2000);
                Manager.Instance.Scene.ChangeScene(ESceneType.TOWN_SCENE);
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
        private readonly EDungeonLevel mDungeonLevel;
        private readonly Action mRequestRedrawCallback;

        public EnterDungeonCommand(EDungeonLevel level, Action requestRedrawCallback)
        {
            mDungeonLevel = level;
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            if (Manager.Instance.Game.PlayerController.HP <= 0)
            {
                Console.WriteLine("던전에 입장할 힘이 남아 있지 않습니다.");
                Thread.Sleep(1000);
                mRequestRedrawCallback?.Invoke(); // 화면 갱신 요청
                return;
            }
            DungeonResult result = Manager.Instance.Dungeon.TryEnterDungeon(mDungeonLevel);
            Console.Clear();
            Console.WriteLine(result.IsClear ? "던전 클리어!" : "던전 공략 실패...");
            Console.WriteLine("마을로 돌아갑니다.");
            Thread.Sleep(2000);
            Manager.Instance.Scene.ChangeScene(ESceneType.TOWN_SCENE);
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
    #endregion

    public class ConsumableManageCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public ConsumableManageCommand(Action requestRedrawCallback)
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
                    player.EquipItem(itemToEquip);
                    Console.WriteLine($"{itemToEquip.Name}을(를) 사용했습니다.");
                    if (itemToEquip.ID >= 200 && itemToEquip.ID < 300)
                        player.UseHPRecovery((ConsumableHP)itemToEquip); // 아이템 사용 시 HP 회복
                   // else (itemToEquip.ID >= 300 && itemToEquip.ID < 400)
                       // player.UseMPRecovery((ConsumableMP)itemToEquip); // 아이템 사용 시 MP 회복
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
}