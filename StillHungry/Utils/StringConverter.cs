using StillHungry.Controller;

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
    }
}
