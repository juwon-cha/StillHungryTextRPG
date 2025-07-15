using StillHungry.Items;
using StillHungry.Managers;
using StillHungry.Utils;
using System.Runtime.CompilerServices;

namespace StillHungry.UI
{
    public class UIManager
    {
        #region 메인 UI 출력 메서드
        public void ShowTitleScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(@" ____                   _                 ");
            Console.WriteLine(@"/ ___| _ __   __ _ _ __| |_ __ _          ");
            Console.WriteLine(@"\___ \| '_ \ / _` | '__| __/ _` |         ");
            Console.WriteLine(@" ___) | |_) | (_| | |  | || (_| |         ");
            Console.WriteLine(@"|____/| .__/ \__,_|_|   \__\__,_|         ");
            Console.WriteLine(@"|  _ \|_|  _ _ __   __ _  ___  ___  _ __  ");
            Console.WriteLine(@"| | | | | | | '_ \ / _` |/ _ \/ _ \| '_ \ ");
            Console.WriteLine(@"| |_| | |_| | | | | (_| |  __/ (_) | | | |");
            Console.WriteLine(@"|____/ \__,_|_| |_|\__, |\___|\___/|_| |_|");
            Console.WriteLine(@"                   |___/                  ");
            Console.ResetColor();
            DisplayOptions(menuOptions, selectedIndex);
        }

        public void ShowTownScreen(string[] menuOptions, int selectedIndex)
        {
            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.");
            DisplayOptions(menuOptions, selectedIndex);
        }

        public void ShowStatusScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("상태 보기");
            Console.ResetColor();
            Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");

            var player = Manager.Instance.Game.PlayerController;
            Console.WriteLine($"Lv. {player.Level:D2}");
            Console.WriteLine($"{player.Name} ( {StringConverter.ClassTypeToString(player.ClassType)} )");

            string attackStat = $"공격력 : {player.Attack}";
            if (player.BonusAttack > 0) attackStat += $" (+{player.BonusAttack})";
            Console.WriteLine(attackStat);

            string defenseStat = $"방어력 : {player.Defense}";
            if (player.BonusDefense > 0) defenseStat += $" (+{player.BonusDefense})";
            Console.WriteLine(defenseStat);

            Console.WriteLine($"체 력 : {player.HP}");
            Console.WriteLine($"Gold : {player.Gold} G\n");

            DisplayOptions(menuOptions, selectedIndex);
        }

        public void ShowDungeonScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("던전 입장");
            Console.ResetColor();
            Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
            Console.WriteLine("1. 쉬운 던전\t | 방어력 5 이상 권장");
            Console.WriteLine("2. 일반 던전\t | 방어력 11 이상 권장");
            Console.WriteLine("3. 어려운 던전\t | 방어력 17 이상 권장\n");
            DisplayOptions(menuOptions, selectedIndex);
        }

        public void ShowCampsiteScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("휴식하기");
            Console.ResetColor();
            var player = Manager.Instance.Game.PlayerController;
            Console.WriteLine($"500 G 를 내면 체력을 회복할 수 있습니다. (보유 골드 : {player.Gold} G)\n");
            DisplayOptions(menuOptions, selectedIndex);
        }
        #endregion

        #region 캐릭터 정보
        public void ShowNameSettingScreen()
        {
            Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다.");
            Console.Write("원하는 이름을 설정해주세요: ");
        }

        public void ShowClassSettingScreen(string playerName, string[] menuOptions, int selectedIndex)
        {
            Console.WriteLine($"플레이어 이름: {playerName}");
            Console.WriteLine("\n원하는 직업을 선택해주세요.");
            DisplayOptions(menuOptions, selectedIndex);
        }
        #endregion

        #region 아이템 리스트 출력 및 상호작용 UI
        public void ShowStoreScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("상점");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Manager.Instance.Game.PlayerController.Gold} G\n");
            PrintStoreItemList(Manager.Instance.Item.EquipmentItems);
            DisplayOptions(menuOptions, selectedIndex);
        }

        public void SelectStoreScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("상점");
            Console.ResetColor();
            Console.WriteLine("길.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Manager.Instance.Game.PlayerController.Gold} G\n");
            DisplayOptions(menuOptions, selectedIndex);
        }
        public void GuildScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("길드");
            Console.ResetColor();
            Console.WriteLine("길드에서 다양한 활동을 할 수 있습니다.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Manager.Instance.Game.PlayerController.Gold} G\n");
            DisplayOptions(menuOptions, selectedIndex);
        }
        public void ShowInventoryScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("인벤토리");
            Console.ResetColor();
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");
            PrintInventoryList();
            DisplayOptions(menuOptions, selectedIndex);
        }

        public void PrintStoreItemList(Dictionary<int, Item> items)
        {
            Console.WriteLine("[아이템 목록]");
            PrintItemList(items, false);
        }

        public void PrintInventoryList(bool isSellingContext = false)
        {
            Console.WriteLine("[내 아이템]");
            PrintItemList(Manager.Instance.Game.PlayerController.InventoryController.Inventory, isSellingContext);
        }

        private void PrintItemList(Dictionary<int, Item> items, bool isSellingContext)
        {
            // 문자열 포맷 폭 설정
            const int nameWidth = 20;
            const int statWidth = 15;
            const int descWidth = 55;

            // 헤더를 포맷팅 함수를 적용하여 정렬
            Console.WriteLine(
                StringFormatting.PadRightForMixedText("-", 4) +
                StringFormatting.PadRightForMixedText("아이템 이름", nameWidth) + " | " +
                StringFormatting.PadRightForMixedText("능력치", statWidth) + " | " +
                StringFormatting.PadRightForMixedText("설명", descWidth) + " | " +
                "가격"
            );
            Console.WriteLine(new string('-', 120)); // 구분선

            int count = 1;
            foreach (var itemPair in items)
            {
                var item = itemPair.Value;
                string equipped = item.HasEquipped ? "[E] " : "";
                string name = $"{equipped}{item.Name}";

                string statDisplay = "";
                if (item is Weapon weapon)
                {
                    statDisplay = $"공격력 +{weapon.Damage}";
                }
                else if (item is Armor armor)
                {
                    statDisplay = $"방어력 +{armor.Defense}";
                }
                else if (item is ConsumableHP consumable)
                {
                    statDisplay = $"체력 회복량 +{consumable.HPRecovery}";
                }
                else if (item is ConsumableMP consumableMP)
                { 
                    statDisplay = $"마나 회복량 +{consumableMP.MPRecovery}";
                }

                    string priceDisplay;
                if (isSellingContext)
                {
                    priceDisplay = $"{item.SellingPrice} G";
                }
                else
                {
                    priceDisplay = item.HasPurchased ? "구매완료" : $"{item.Price} G";
                }

                // 각 부분을 유틸 함수를 이용해 정렬된 문자열로 만든다.
                string countStr = StringFormatting.PadRightForMixedText($"- {count}", 4);
                string nameStr = StringFormatting.PadRightForMixedText(name, nameWidth);
                string statStr = StringFormatting.PadRightForMixedText(statDisplay, statWidth);
                string descStr = StringFormatting.PadRightForMixedText(item.Description, descWidth);

                // 정렬된 문자열들을 조합하여 최종 출력
                Console.WriteLine($"{countStr}{nameStr} | {statStr} | {descStr} | {priceDisplay}");
                count++;
            }
        }
        #endregion

        private void DisplayOptions(string[] options, int selectedIndex)
        {
            Console.WriteLine();
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"  {options[i]}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("\n(↑, ↓ 방향키로 이동, Enter로 선택)");
        }
    }
}
