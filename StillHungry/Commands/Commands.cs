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
            Console.Clear();

            Manager.Instance.Scene.ChangeScene(ESceneType.BATTLE_SCENE);
            //Manager.Instance.Scene.ChangeScene(ESceneType.MONSTER_PHASE_SCENE); // 테스트
        }
    }
    #endregion

    #region 상점 및 인벤토리
    public class BuyItemCommand : IExecutable
    {
        private readonly Action mRequestRedrawCallback;

        public BuyItemCommand(Action requestRedrawCallback)
        {
            mRequestRedrawCallback = requestRedrawCallback;
        }

        public void Execute()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("상점 - 아이템 구매");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 구매할 수 있습니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Manager.Instance.Game.PlayerController.Gold} G\n");

            Manager.Instance.UI.PrintStoreItemList();

            Console.Write("\n구매할 아이템의 번호를 입력해주세요 (0: 나가기) >> ");
            string input = Console.ReadLine();
            var storeItems = Manager.Instance.Item.Items;
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
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= inventory.Inventory.Count)
            {
                Item itemToSell = inventory.Inventory.Values.ElementAt(choice - 1);
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
            if (int.TryParse(input, out int choice) && choice > 0 && choice <= player.InventoryController.Inventory.Count)
            {
                Item itemToEquip = player.InventoryController.Inventory.Values.ElementAt(choice - 1);
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

    #region  전투커맨드
    public class BattleStartCommand : IExecutable
    {
        public void Execute()
        {
            Console.Clear();
            Manager.Instance.Battle.isFighting = true;
            Manager.Instance.Scene.ChangeScene(ESceneType.BATTLE_SCENE);
        }
    }

    public class MonsterPhaseCommand : IExecutable
    {
        public void Execute()
        {
            Manager.Instance.Battle.NextMonsterAttack();
        }
    }
    #endregion
}