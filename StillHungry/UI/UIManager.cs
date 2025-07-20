using StillHungry.Controller;
using StillHungry.Data;
using StillHungry.Items;
using StillHungry.Managers;
using StillHungry.Monsters;
using StillHungry.Utils;
using System;
using System.Threading;

namespace StillHungry.UI
{
    // 559 634
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
            if (player.FoodAttack > 0) attackStat += $" (+{player.FoodAttack})";
            Console.WriteLine(attackStat);

            string defenseStat = $"방어력 : {player.Defense}";
            if (player.BonusDefense > 0) defenseStat += $" (+{player.BonusDefense})";
            if (player.FoodDefense > 0) defenseStat += $" (+{player.FoodDefense})";
            Console.WriteLine(defenseStat);

            Console.WriteLine($"체 력 : {player.HP} / {player.MaxHP}");
            Console.WriteLine($"마 나 : {player.Mana} / {player.MaxMana}");

            Console.WriteLine($"Gold : {player.Gold} G");

            string criticalStat = $"치명타 확률 : {player.CriticalChance * 100:F1} %";
            if (player.FoodCriticalChance > 0) criticalStat += $" (+{player.FoodCriticalChance * 100:F1})";
            Console.WriteLine(criticalStat);
            string evasionStat = $"회피 확률 : {player.EvasionChance * 100:F1} %";
            if (player.FoodEvasionChance > 0) evasionStat += $" (+{player.FoodEvasionChance * 100:F1})";
            Console.WriteLine(evasionStat + "\n");

            DisplayOptions(menuOptions, selectedIndex);
        }

        public void ShowDungeonScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("던전 입장\n");
            Console.ResetColor();

            Console.WriteLine("플레이어 상태");
            var player = Manager.Instance.Game.PlayerController;
            Console.WriteLine($"Lv. {player.Level:D2}");
            Console.WriteLine($"{player.Name} ( {StringConverter.ClassTypeToString(player.ClassType)} )");

            string attackStat = $"공격력 : {player.Attack}";
            if (player.BonusAttack > 0) attackStat += $" (+{player.BonusAttack})";
            if (player.FoodAttack > 0) attackStat += $" (+{player.FoodAttack})";
            Console.WriteLine(attackStat);

            string defenseStat = $"방어력 : {player.Defense}";
            if (player.BonusDefense > 0) defenseStat += $" (+{player.BonusDefense})";
            if (player.FoodDefense > 0) defenseStat += $" (+{player.FoodDefense})";
            Console.WriteLine(defenseStat);

            Console.WriteLine($"체 력 : {player.HP} / {player.MaxHP}");
            Console.WriteLine($"마 나 : {player.Mana} / {player.MaxMana}");

            Console.WriteLine($"Gold : {player.Gold} G");

            string criticalStat = $"치명타 확률 : {player.CriticalChance * 100:F1} %";
            if (player.FoodCriticalChance > 0) criticalStat += $" (+{player.FoodCriticalChance * 100:F1})";
            Console.WriteLine(criticalStat);
            string evasionStat = $"회피 확률 : {player.EvasionChance * 100:F1} %";
            if (player.FoodEvasionChance > 0) evasionStat += $" (+{player.FoodEvasionChance * 100:F1})";
            Console.WriteLine(evasionStat);

            //Console.WriteLine($"");
            //Console.WriteLine("이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");
            //Console.WriteLine("- 축축한 동굴\t\t | 방어력  5 이상 권장");
            //Console.WriteLine("- 건조한 풀밭\t\t | 방어력 11 이상 권장");
            //Console.WriteLine("- 돌 산맥\t\t | 방어력 17 이상 권장");
            //Console.WriteLine("- 용암이 흐르는 계곡\t | 방어력 23 이상 권장");
            //Console.WriteLine("- 레드드래곤의 둥지\t | 방어력 29 이상 권장");

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

        public void ShowSaveLoadScreen(string[] menuOptions, int selectedIndex, bool isSavingMode)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(isSavingMode ? "## 저장하기 ##" : "## 불러오기 ##");
            Console.ResetColor();
            Console.WriteLine("데이터를 저장하거나 불러올 슬롯을 선택해주세요.");

            // 모든 슬롯 데이터 불러오기
            DataManager.LoadAllSlotsData();

            Console.WriteLine();
            for (int i = 0; i < menuOptions.Length; i++)
            {
                // menuOptions 배열의 마지막은 보통 '나가기'이므로, 슬롯 번호는 i + 1로 간주
                // "슬롯" 이라는 텍스트를 포함하고 있을 때만 슬롯으로 간주
                if (!menuOptions[i].Contains("슬롯"))
                {
                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"> {menuOptions[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"  {menuOptions[i]}");
                        Console.ResetColor();
                    }
                    continue;
                }

                int slotIndex = i + 1;
                string optionText = menuOptions[i];
                string playerInfo = "";

                // 해당 슬롯에 저장된 데이터가 있는지 확인
                if (DataManager.UserSlots.TryGetValue(slotIndex, out var userData))
                {
                    // 데이터가 있다면, 플레이어 정보를 문자열로 만든다.
                    string className = StringConverter.ClassTypeToString(userData.ClassType);
                    playerInfo = $" - {userData.Name} (Lv.{userData.Level} {className})";
                }
                else
                {
                    // 데이터가 없다면, 비어있음 표시
                    playerInfo = " - (비어 있음)";
                }

                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    // 선택된 메뉴에 플레이어 정보를 합쳐서 출력
                    Console.WriteLine($"> {optionText}{playerInfo}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    // 선택되지 않은 메뉴에 플레이어 정보를 합쳐서 출력
                    Console.WriteLine($"  {optionText}{playerInfo}");
                    Console.ResetColor();
                }
            }
            Console.WriteLine("\n(↑, ↓ 방향키로 이동, Enter로 선택)");
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
        public void EquipmentStoreScreen(string[] menuOptions, int selectedIndex)
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

        public void ConsumableStoreScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("상점");
            Console.ResetColor();
            Console.WriteLine("길.\n");
            Console.WriteLine("[보유 골드]");
            Console.WriteLine($"{Manager.Instance.Game.PlayerController.Gold} G\n");
            PrintStoreItemList(Manager.Instance.Item.ConsumableItems);
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
            DisplayOptions(menuOptions, selectedIndex);
        }
        
        public void PrintStoreItemList(Dictionary<int, Item> items)
        {
            Console.WriteLine("[아이템 목록]");
            PrintItemList(items, false);
        }

        public void PrintInventoryList(bool isSellingContext = false, bool isSellConsumable = false)
        {
            Console.WriteLine("[내 아이템]");
            if (!isSellConsumable)
            {
                PrintItemList(Manager.Instance.Game.PlayerController.InventoryController.EquipInventory, isSellingContext);
            }
            else 
            {
                PrintItemList(Manager.Instance.Game.PlayerController.InventoryController.ConsumableInventory, isSellingContext);
            }

        }

        private void PrintItemList(Dictionary<int, Item> items, bool isSellingContext)
        {
            // 문자열 포맷 폭 설정
            const int nameWidth = 22;
            const int statWidth = 16;
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
                    if (item is ConsumableHP consumable || item is ConsumableMP consumableMP)
                    {
                        priceDisplay = $"{item.SellingPrice}G" + $" x {item.Quantity}";
                    }
                    else
                    {
                        priceDisplay = $"{item.SellingPrice}G";
                    }
                }
                else
                {
                    if (item is ConsumableHP consumable || item is ConsumableMP consumableMP)
                    {
                        priceDisplay = $"{item.Price}G" + $" x {item.Quantity}";
                    }
                    else
                        priceDisplay = item.HasPurchased ? "구매완료" : $"{item.Price}G";
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

        // 요리 프린트
        public void PrintFoodItemList(Dictionary<int, Item> items)
        {
            // 문자열 포맷 폭 설정
            const int nameWidth = 15;
            const int statWidth = 20;
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
                var item = (Food)itemPair.Value;
                string name = $"{item.Name}";

                string statDisplay = "";

                if (item.Damage != 0)
                {
                    statDisplay += $"공격력 +{item.Damage} ";
                }
                if (item.Defense != 0)
                {
                    statDisplay += $"방어력 +{item.Defense} ";
                }
                if (item.Critical != 0)
                {
                    statDisplay += $"크리티컬 확률 +{item.Critical*100}% ";
                }
                if (item.Evade != 0)
                {
                    statDisplay += $"회피 확률 +{item.Evade*100}% ";
                }
                if(item.ID == 504)
                {
                    statDisplay = "???";
                }

                string priceDisplay;
                priceDisplay = $"{item.Price}G";
              

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

        #region 퀘스트 
        public void QuestScreen(string[] menuOptions, int selectedIndex)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("길드 - 퀘스트");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("\"아, 자네구먼! 오늘도 변함없네.\n이렇게 매일 보니 참 반갑기 그지없다네.\n일도 무탈하게 잘 하고 있지? 힘들 땐 언제든 말하라고.\n여기 길드가 자네의 든든한 집이니까 말일세.\"\n");

            DisplayOptions(menuOptions, selectedIndex);
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

        #region 전투
        public void ShowMonsterPhaseScreen(string[] menuOptions, int selectedIndex, Monster attacker, MonsterAction action, PlayerController player)
        {
            Console.WriteLine("\u001b[33mBattle!!\u001b[0m\n");

            bool isAllMonsterDead = true;
            foreach (Monster m in Manager.Instance.Battle.MonsterController.ActiveMonsters)
            {
                if (!m.IsDead)
                {
                    isAllMonsterDead = false;
                    break;
                }
            }

            if (isAllMonsterDead)
            {
                Console.WriteLine("모든 몬스터가 쓰러졌습니다! 전투에서 승리했습니다!");
            }
            else if (attacker == null || action == null || action.Type == EMonsterActionType.NONE)
            {
                // 공격자나 행동 정보가 없을 경우의 예외 처리
                Console.WriteLine("몬스터들이 다음 행동을 준비합니다...");
            }
            else
            {
                // MonsterAction에 담긴 메시지를 출력
                Console.ForegroundColor = ConsoleColor.Yellow;
                if (action.Type == EMonsterActionType.ATTACK)
                {
                    Console.WriteLine($"{attacker.Name}이(가) 공격 자세를 취합니다!");
                }
                else
                {
                    Console.WriteLine($"{attacker.Name}이(가) 방어 자세를 취합니다! (방어력 증가)");
                }
                Console.ResetColor();
                Console.WriteLine();

                // 행동 타입이 '공격'일 경우에만 데미지 정보를 표시
                if (action.Type == EMonsterActionType.ATTACK)
                {
                    Console.WriteLine($"ID: Lv.{attacker.Level} {attacker.Name} 의 공격!");
                    int damage = action.Value;

                    bool evaded = player.TakeDamage(damage);
                    if (evaded)
                    {
                        Console.WriteLine($"{player.Name}님이 공격을 피했습니다!");
                    }
                    else
                    {
                        Console.WriteLine($"{player.Name} 을(를) 맞췄습니다. [데미지 : {damage}]");
                        Console.WriteLine();
                        Console.WriteLine($"Lv.{player.Level} {player.Name}");
                        // 공격 받기 전 체력과 후 체력을 함께 표시
                        Console.Write($"HP {player.HP + damage} -> ");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(player.HP);
                        Console.ResetColor();
                    }
                }
            }

            DisplayOptions(menuOptions, selectedIndex);
        }

        public void ShowSkillSelect(int selectedIndex)
        {
            Console.WriteLine("\u001b[33mSkill Select!!\u001b[0m\n");

            // 게임매니저의 인스턴스를 통해서 몬스터의 정보를 얻어온다.
            foreach (var m in Manager.Instance.Battle.MonsterController.ActiveMonsters)
            {
                // 몬스터의 정보를 출력
                if (m.IsDead)
                {
                    Console.WriteLine(
                    $"\u001b[90mLv.{m.Level} " +
                    $"{m.Name,-5}\tDEAD\u001b[0m");
                }
                else
                {
                    Console.WriteLine(
                    $"Lv.\u001b[33m{m.Level}\u001b[0m " +
                    $"{m.Name,-5}\tHP \u001b[33m{m.CurrentHp}\u001b[0m");
                }
            }

            // 스킬 정보 출력
            List<string> skills = new List<string>();
            foreach (var s in Manager.Instance.Game.PlayerController.ActiveSkills)
            {
                skills.Add($"{s.Name} - MP {s.RequiredMP}({s.Description})");
            }
            skills.Add("돌아가기");
            DisplayOptions(skills.ToArray(), selectedIndex, "\n스킬을 선택해 주세요");

            // 게임매니저의 인스턴스를 통해서 플레이어의 정보를 얻어온다
            Console.WriteLine("\n\n[내정보]");
            var player = Manager.Instance.Game.PlayerController;
            Console.Write($"Lv. \u001b[33m{player.Level}\u001b[0m\t");
            Console.WriteLine($"{player.Name} ({StringConverter.ClassTypeToString(player.ClassType)})");
            Console.WriteLine($"HP : \u001b[33m{player.HP}/{player.MaxHP}\u001b[0m");
        }

        public void ShowAttackSelect(int selectedIndex) 
        {
            Console.WriteLine($"{DataManager.DungeonDataDict[Manager.Instance.Dungeon.CurrentDungeonID].Name}");

            bool isAllMonsterDead = true;
            foreach (Monster m in Manager.Instance.Battle.MonsterController.ActiveMonsters)
            {
                if (!m.IsDead)
                {
                    isAllMonsterDead = false;
                    break;
                }
            }
            ShowAllMonstersDead(isAllMonsterDead);

            Console.WriteLine("\u001b[33mAttack Select!!\u001b[0m");

            // 게임매니저의 인스턴스를 통해서 몬스터의 정보를 얻어온다.
            List<string> monsters = new List<string>();
            foreach (var m in Manager.Instance.Battle.MonsterController.ActiveMonsters)
            {
                // 몬스터의 정보를 출력
                if (m.IsDead)
                {
                    monsters.Add(
                    $"\u001b[90mLv.{m.Level} " +
                    $"{m.Name,-5}\tDEAD\u001b[0m");
                }
                else
                {
                    monsters.Add(
                    $"Lv.\u001b[33m{m.Level}\u001b[0m " +
                    $"{m.Name,-5}\tHP \u001b[33m{m.CurrentHp}\u001b[0m");
                }
            }
            monsters.Add("돌아가기");
            DisplayOptions(monsters.ToArray(), selectedIndex, "\n공격 대상을 선택해 주세요");

            // 게임매니저의 인스턴스를 통해서 플레이어의 정보를 얻어온다
            Console.WriteLine("\n\n[내정보]");
            var player = Manager.Instance.Game.PlayerController;
            Console.Write($"Lv. \u001b[33m{player.Level}\u001b[0m\t");
            Console.WriteLine($"{player.Name} ({StringConverter.ClassTypeToString(player.ClassType)})");
            Console.WriteLine($"HP : \u001b[33m{player.HP}/{player.MaxHP}\u001b[0m");
        }

        public void ShowAllMonstersDead(bool isAllMonsterDead)
        {
            if(isAllMonsterDead)
            {
                Console.WriteLine("모든 몬스터를 처치했습니다! 전투에서 승리했습니다!");

                foreach (var m in Manager.Instance.Battle.MonsterController.ActiveMonsters)
                {
                    // 몬스터의 정보를 출력
                    if (m.IsDead)
                    {
                        Console.WriteLine($"\u001b[90mLv.{m.Level} " + $"{m.Name,-5}\tDEAD\u001b[0m");
                    }
                }

                Console.WriteLine("전투 결과를 보려면 아무 키나 누르세요.");
                Console.ReadKey();

                var battleManager = Manager.Instance.Battle;
                battleManager.EndBattle
                    (
                    isVictory: true,
                    initialHP: battleManager.InitialHP,
                    damageTaken: battleManager.TotalDamageTaken,
                    monsterKillCount: battleManager.MonsterKillCount
                    );
            }
        }

        public void ShowBattleScreen(string[] menuOptions, int selectedIndex)
        {
            Console.WriteLine($"{DataManager.DungeonDataDict[Manager.Instance.Dungeon.CurrentDungeonID].Name}");
            Console.WriteLine("\u001b[33mBattle!!\u001b[0m");

            // 게임매니저의 인스턴스를 통해서 몬스터의 정보를 얻어온다.
            Console.WriteLine();
            foreach (var m in Manager.Instance.Battle.MonsterController.ActiveMonsters) 
            {
                // 몬스터의 점보를 출력
                if (m.IsDead)
                {
                    Console.Write(
                            $"\u001b[90mLv.{m.Level} " +
                            $"{m.Name}\tDEAD\u001b[0m\n");
                }
                else 
                {
                    Console.Write(
                        $"Lv.\u001b[33m{m.Level}\u001b[0m " +
                        $"{m.Name}\tHP \u001b[33m{m.CurrentHp}\u001b[0m\n");
                }
            }

            // 게임매니저의 인스턴스를 통해서 플레이어의 정보를 얻어온다
            Console.WriteLine("\n\n[내정보]");
            var player = Manager.Instance.Game.PlayerController;
            Console.Write($"Lv. \u001b[33m{player.Level}\u001b[0m\t");
            Console.WriteLine($"{player.Name} ({StringConverter.ClassTypeToString(player.ClassType)})");

            Console.WriteLine($"HP : \u001b[33m{player.HP}/{player.MaxHP}\u001b[0m");
            DisplayOptions(menuOptions, selectedIndex);
        }

        public void PlayerTurnScreen(string[] menuOptions, int selectedIndex)
        {
            Console.WriteLine("\u001b[33mPlayer Attack Turn!!\u001b[0m");

            // 게임매니저의 인스턴스를 통해서 플레이어의 정보를 얻어온다
            var player = Manager.Instance.Game.PlayerController;
            Console.WriteLine($"{player.Name} 의 공격!");

            // 몬스터가 받은 데미지 출력
            List<Monster> monsters = Manager.Instance.Battle.MonsterController.ActiveMonsters;
            foreach(Monster monster in monsters)
            {
                if(monster.DamageTaken > 0)
                {
                    if (Manager.Instance.Battle.IsCritical)
                    {
                        Console.WriteLine($"{monster.Name}에게 {monster.DamageTaken}의 치명상을 입혔습니다!");
                    }
                    else
                    {
                        Console.WriteLine($"{monster.Name}에게 {monster.DamageTaken}의 데미지를 입혔습니다.");
                    }
                }
            }

            Console.WriteLine($"");
            DisplayOptions(menuOptions, selectedIndex);
        }

        private void DisplayOptions(string[] options, int selectedIndex, string message)
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
            Console.WriteLine(message);
            Console.WriteLine("(↑, ↓ 방향키로 이동, Enter로 선택)");
        }
        #endregion
    }
}
