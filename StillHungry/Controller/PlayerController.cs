using StillHungry.Data;
using StillHungry.Items;
using StillHungry.Managers;
using StillHungry.Scene;
using System;


namespace StillHungry.Controller
{
    public enum EClassType
    {
        NONE,
        WARRIOR,
        MAGICIAN,
        ARCHER,
        THIEF,
    }

    public class PlayerController : CharacterController
    {
        public EClassType ClassType { get; private set; }

        // 기본 능력치!!
        public float BaseAttack { get; private set; }
        public float BaseDefense { get; private set; }
        public int Mana {  get; private set; }
        public int MaxMana { get; private set; } = 100;
        public float BaseCriticalChance { get; private set; } // 치명타 확률
        public float BaseEvasionChance { get; private set; } // 회피 확률

        // 장비로 인한 추가 능력치
        public float BonusAttack { get; private set; }
        public float BonusDefense { get; private set; }

        // 최종 능력치는 기본 능력치와 추가 능력치의 합
        // 외부에서는 이 프로퍼티를 통해 최종 값만 읽을 수 있음
        public override float Attack => BaseAttack + BonusAttack;
        public override float Defense => BaseDefense + BonusDefense;
        public float CriticalChance => BaseCriticalChance;
        public float EvasionChance => BaseEvasionChance;
        public int CurrentMana => Mana;
        public int MaximumMana => MaxMana;

        public int Gold { get; private set; }
        public InventoryController InventoryController { get; private set; } = new InventoryController();

        // 새 게임 시작할 때 플레이어 데이터 설정
        public virtual void Init(string name, EClassType classType)
        {
            PlayerStat stat;
            // json 데이터 로드
            if(DataManager.PlayerStatDict.TryGetValue(classType, out stat))
            {
                ClassType = stat.ClassType;
                Level = stat.Level;
                Name = name;
                BaseAttack = stat.Attack;
                BaseDefense = stat.Defense;
                HP = stat.MaxHp;
                MaxHP = stat.MaxHp;
                Gold = stat.Gold;
                BaseCriticalChance = stat.CriticalRate;
                BaseEvasionChance = stat.EvadeRate;
                MaxMana = stat.MaxMana;
                Mana = stat.MaxMana;
                
            }
            else
            {
                throw new KeyNotFoundException("Initialization Failed: Failed to load PlayerStat");
            }
        }

        public void SetClassType(EClassType classType)
        {
            ClassType = classType;
        }

        public void SetPlayerName(string name)
        {
            Name = name;
        }

        // 로드한 저장 데이터를 플레이어 데이터에 설정
        public void SetPlayerSettingsFromLoadData(UserData userData)
        {
            ClassType = userData.ClassType;
            Level = userData.Level;
            Name = userData.Name;
            BaseAttack = userData.Attack;
            BaseDefense = userData.Defense;
            HP = userData.HP;
            Mana = userData.Mana;
            Gold = userData.Gold;
            BaseCriticalChance = userData.CriticalRate;
            BaseEvasionChance = userData.EvadeRate;

            // 인벤토리 초기화 후 저장 데이터 세팅
            InventoryController.ClearInventory();

            // 로드한 유저 데이터로부터 아이템 세팅
            if(userData.Items != null)
            {
                foreach (UserItemData savedItem in userData.Items)
                {
                    // 아이템 정보를 아이템 매니저에서 가져오기
                    Item item = Manager.Instance.Item.GetItemFromID(savedItem.ID);
                    if(item != null)
                    {
                        // 상점 구매기록 복원
                        item.HasPurchased = true;

                        // 아이템 장착 여부 복원
                        item.HasEquipped = savedItem.HasEquipped;

                        // 인벤토리에 복원된 상태의 아이템 추가
                        InventoryController.AddItem(item);

                        // 아이템 장착에 따른 능력치 조정
                        RecalculateStats();
                    }
                }
            }
        }

        public void EquipItem(Item item)
        {
            if (item.HasEquipped)
            {
                // 장착된 아이템 선택해서 해당 아이템 해제
                ToggleEquipItem(item);

                return;
            }

            foreach (KeyValuePair<int, Item> pair in InventoryController.EquipInventory)
            {
                // 중복 장착 처리
                // 장착된 아이템이 있는지 확인 후 아이템 타입을 판별해서 같은 타입이면 장착 상태 바꿈
                if (pair.Value.HasEquipped && item.ItemType == pair.Value.ItemType)
                {
                    ToggleEquipItem(item);
                    ToggleEquipItem(pair.Value);
                    return;
                }
            }

            // 처음 장착
            ToggleEquipItem(item);
        }

        public void ToggleEquipItem(Item item)
        {
            if(item == null)
            {
                return;
            }

            // 아이템 장착 상태 반전
            if(item.HasEquipped)
            {
                item.HasEquipped = !item.HasEquipped;
            }

            // 모든 장비의 보너스 능력치를 다시 계산하여 플레이어에게 적용
            RecalculateStats();
        }

        public void RecalculateStats()
        {
            // 보너스 스탯을 0으로 초기화
            // 장착 해제된 아이템의 능력치는 사라짐
            BonusAttack = 0;
            BonusDefense = 0;

            // 인벤토리의 모든 아이템을 확인
            foreach (var item in InventoryController.EquipInventory.Values)
            {
                // 장착된 아이템일 경우에만 보너스 스탯을 더함
                if (item.HasEquipped)
                {
                    if (item is Weapon weapon)
                    {
                        BonusAttack += weapon.Damage;
                    }
                    else if (item is Armor armor)
                    {
                        BonusDefense += armor.Defense;
                    }
                }
            }
        }

        public EPurchaseResult BuyItem(Item item)
        {
            if (item == null)
            {
                return EPurchaseResult.NONE;
            }

            //골드 부족
            if (Gold < item.Price)
            {
                return EPurchaseResult.NOT_ENOUGH_GOLD;
            }

            // 이미 구매한 경우
            if (item.HasPurchased)
            {
                return EPurchaseResult.ALREADY_PURCHASED;
            }

            // 플레이어 골드 소비
            Gold -= item.Price;

            // 인벤토리에 아이템 추가
            InventoryController.AddItem(item);

            item.HasPurchased = true;
            item.HasSold = false;

            return EPurchaseResult.SUCCESS;
        }

        public ESellResult SellItem(Item item)
        {
            if (item == null)
            {
                return ESellResult.NOT_IN_INVENTORY;
            }

            // 인벤토리에서 제거
            if (!InventoryController.RemoveItem(item))
            {
                return ESellResult.NOT_IN_INVENTORY;
            }

            // 제거 성공한 경우 플레이어 골드 증가
            Gold += item.SellingPrice;

            item.HasSold = true;
            item.HasPurchased = false;

            // 장착된 아이템 판매 후 능력치 조정 및 장착 해제
            ToggleEquipItem(item);

            return ESellResult.SUCCESS;
        }

        public ERestResult RestAtCampsite(int cost)
        {
            if (Gold < cost)
            {
                return ERestResult.NOT_ENOUGH_GOLD; // 골드 부족으로 실패
            }

            // 현재 체력이 최대 체력인지 확인
            if (HP >= MaxHP)
            {
                return ERestResult.FULL_HP; // 이미 최대 체력이면 회복 안됨
            }

            // 골드를 차감하고 체력을 최대 체력으로 회복
            Gold -= cost;
            HP = MaxHP;

            return ERestResult.SUCCESS;
        }

        public void TakeDamage(int damage)
        {
            if(damage > 0)
            {
                HP -= damage;
                if(HP < 0)
                {
                    HP = 0;
                }
            }
        }

        public void EarnGold(int goldAmount)
        {
            if(goldAmount > 0)
            {
                Gold += goldAmount;
            }
        }

        public void LevelUp()
        {
            // TEMP
            ++Level;

            // 기본 능력치 상승
            BaseAttack += 0.5f;
            BaseDefense += 1f;
        }

        public bool UseMana(int amount)
        {
            if(amount <= 0)
            {
                return false;
            }
            if(Mana >= amount)
            {
                Mana -= amount;
                return true;
            }
            return false;
        }

        public void UseSkill()
        {

        }

        public void Defend()
        {

        }

        public void UseHPRecovery(ConsumableHP consumableHP)
        {
            if (InventoryController.ConsumableInventory.ContainsKey(consumableHP.ID))
            {
                // 아이템 사용
                HP += consumableHP.HPRecovery;
                if (HP > MaxHP)
                {
                    HP = MaxHP; // 최대 체력 초과 방지
                }
                // 인벤토리에서 아이템 제거
                InventoryController.RemoveItem(consumableHP);

                Console.WriteLine($"{consumableHP.Name}을 사용하여 체력을 {consumableHP.HPRecovery}회복했습니다.");
            }
            else
            {
                Console.WriteLine("해당 아이템을 사용할 수 없습니다.");
            }
        }

        public void UseMPRecovery(ConsumableMP consumableMP)
        {
            if (InventoryController.ConsumableInventory.ContainsKey(consumableMP.ID))
            {
                Console.WriteLine($"현재 마나 : {Mana}, 최대 마나 : {MaxMana}");
                // 아이템 사용
                Mana += consumableMP.MPRecovery;
                if (Mana > MaxMana)
                {
                    Mana = MaxMana; // 최대 체력 초과 방지
                }
                // 인벤토리에서 아이템 제거
                InventoryController.RemoveItem(consumableMP);

                Console.WriteLine($"{consumableMP.Name}을 사용하여 마나를 {consumableMP.MPRecovery}회복했습니다.\n 현재 마나 : {Mana}");
            }
            else
            {
                Console.WriteLine("해당 아이템을 사용할 수 없습니다.");
            }
        }



        public bool IsDead()
        {
            return false;
        }
    }
}
