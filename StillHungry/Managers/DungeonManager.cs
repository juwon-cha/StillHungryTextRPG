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

        public DungeonResult(bool isClear, int rewardGold)
        {
            IsClear = isClear;
            RewardGold = rewardGold;
        }
    }

    class DungeonManager
    {
        public DungeonResult DungeonResult { get; private set; }

        public void TryEnterDungeon(EDungeonLevel level)
        {
            Random rand = new Random();
            var player = Manager.Instance.Game.PlayerController;

            // json 던전 데이터 로드
            if (!DataManager.DungeonDataDict.TryGetValue(level.ToString(), out DungeonData dungeonData))
            {
                // 데이터가 없는 예외적인 경우
                DungeonResult = new DungeonResult(false, 0);
            }

            DungeonResult = new DungeonResult(false, dungeonData.BaseRewardGold);
        }
    }
}
