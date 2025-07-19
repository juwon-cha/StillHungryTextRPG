using StillHungry.Controller;
using StillHungry.Scene;

namespace StillHungry.Utils
{
    public static class StringConverter
    {
        public static string ClassTypeToString(EClassType classType)
        {
            switch ((int)classType)
            {
                case (int)EClassType.WARRIOR:
                    return "전사";

                case (int)EClassType.MAGICIAN:
                    return "마법사";

                case (int)EClassType.ARCHER:
                    return "궁수";

                case (int)EClassType.THIEF:
                    return "도적";

                default:
                    break;
            }

            return null;
        }

        public static string DungeonLevelToString(EDungeonLevel dungeonLevel)
        {
            switch ((int)dungeonLevel)
            {
                case (int)EDungeonLevel.DAMP_CAVE:
                    return "축축한 동굴";

                case (int)EDungeonLevel.DRY_GRASS:
                    return "건조한 풀밭";

                case (int)EDungeonLevel.STONE_MOUNTAIN:
                    return "돌 산맥";

                case (int)EDungeonLevel.LAVA_VALLEY:
                    return "용암이 흐르는 계곡";

                case (int)EDungeonLevel.RED_DRAGON_NEST:
                    return "레드 드래곤의 둥지";

                default:
                    break;
            }

            return null;
        }
    }
}
