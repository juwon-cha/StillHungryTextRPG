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
        private Dictionary<EDungeonLevel, List<int>> dungeonMonsterMap = new Dictionary<EDungeonLevel, List<int>>()
        {
            { EDungeonLevel.DAMP_CAVE, new List<int> { 1, 2, 3 } },          // 축축한 동굴 (박쥐, 슬라임, 미믹)
            { EDungeonLevel.DRY_GRASS, new List<int> { 4, 5, 6 } },          // 건조한 풀밭 (풀푸푸, 꽃벌, 나무지기)
            { EDungeonLevel.STONE_MOUNTAIN, new List<int> { 7, 8, 9 } },     // 돌 산맥 (스톤골렘, 바위새, 딱딱이)
            { EDungeonLevel.LAVA_VALLEY, new List<int> { 10, 11, 12 } },     // 용암이 흐르는 계곡 (베이비피닉스, 용암슬라임, 용암기사)
            { EDungeonLevel.RED_DRAGON_NEST, new List<int> { 100 } }         // 레드드래곤의 둥지 (레드드래곤 보스)
        };
        public DungeonResult DungeonResult { get; private set; }

        public void TryEnterDungeon(EDungeonLevel level)
        {

            var player = Manager.Instance.Game.PlayerController;

            // json 던전 데이터 로드
            if (!DataManager.DungeonDataDict.TryGetValue(level.ToString(), out DungeonData dungeonData))
            {
                // 데이터가 없는 예외적인 경우
                DungeonResult = new DungeonResult(false, 0);
                return;
            }

            //if (!dungeonMonsterMap.TryGetValue(level, out List<int> monsterIds))
            //{
            //    DungeonResult = new DungeonResult(false, 0);
            //    return;
            //}

            //// 몬스터 정보 리스트 생성
            //List<MonsterStat> monsters = monsterIds
            //    .Select(id => DataManager.MonsterStatDict[id])
            //    .ToList();



            DungeonResult = new DungeonResult(false, dungeonData.BaseRewardGold);
        }
    }
    //public enum EDungeonLevel
    //{
    //    EASY,
    //    NORMAL,
    //    HARD,
    //    DAMP_CAVE,
    //    DRY_GRASS,
    //    STONE_MOUNTAIN,
    //    LAVA_VALLEY,
    //    RED_DRAGON_NEST
    //}
}