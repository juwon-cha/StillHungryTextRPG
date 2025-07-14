using StillHungry.Data;
using StillHungry.Scene;

namespace StillHungry.Managers
{
    public class DungeonResult
    {
        public bool IsClear { get; }
        public int InitialHP { get; }
        public int DamageTaken { get; }
        public int RewardGold { get; }

        public DungeonResult(bool isClear, int initialHP, int damageTaken, int rewardGold)
        {
            IsClear = isClear;
            InitialHP = initialHP;
            DamageTaken = damageTaken;
            RewardGold = rewardGold;
        }
    }

    class DungeonManager
    {
        public DungeonResult TryEnterDungeon(EDungeonLevel level)
        {
            Random rand = new Random();
            var player = Manager.Instance.Game.PlayerController;

            // json 던전 데이터 로드
            if (!DataManager.DungeonDataDict.TryGetValue(level.ToString(), out DungeonData dungeonData))
            {
                // 데이터가 없는 예외적인 경우
                return new DungeonResult(false, player.HP, 0, 0);
            }

            // 방어력 체크
            int initialHP = player.HP;
            if (player.Defense < dungeonData.RecommendedDefense)
            {
                if (rand.Next(0, 100) < 40) // 40% 확률로 실패
                {
                    // 실패 시, 체력의 절반을 잃음
                    int damage = player.HP / 2;

                    // 체력이 1 남았을 때는 damage를 1로 설정해서 플레이어 사망 처리
                    if(player.HP == 1)
                    {
                        damage = 1;
                    }

                    player.TakeDamage(damage);

                    return new DungeonResult(false, initialHP, damage, 0);
                }
            }

            // 던전 클리어
            // 기본 체력 감소(20 ~ 35)
            // 권장 방어력 +- 에 따라 종료시 체력 소모 반영
            float baseDamage = rand.Next(20, 36);
            int finalDamage = (int)(baseDamage + (player.Defense - dungeonData.RecommendedDefense));

            player.TakeDamage(finalDamage);

            // 보상
            // 공격력  ~ 공격력 * 2 의 % 만큼 추가 보상 획득 가능
            int reward = dungeonData.BaseRewardGold;
            int bonusReward = (int)(reward * (rand.Next((int)player.Attack, (int)(player.Attack * 2f)) / 100.0f));
            int totalReward = reward + bonusReward;

            player.EarnGold(totalReward);
            player.LevelUp();

            return new DungeonResult(true, initialHP, finalDamage, totalReward);
        }
    }
}
