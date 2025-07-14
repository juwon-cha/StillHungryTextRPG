namespace StillHungry.UI
{
    public class MenuNavigator
    {
        public int SelectedIndex { get; private set; }
        private readonly int mMenuCount;

        public MenuNavigator(int menuCount)
        {
            mMenuCount = menuCount;
            SelectedIndex = 0;
        }

        // 키 입력을 받아 인덱스를 변경하고, 변경 여부를 반환
        public bool Navigate(ConsoleKey key)
        {
            int oldIndex = SelectedIndex;
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    SelectedIndex--;
                    if (SelectedIndex < 0) SelectedIndex = mMenuCount - 1;
                    break;
                case ConsoleKey.DownArrow:
                    SelectedIndex++;
                    if (SelectedIndex > mMenuCount - 1) SelectedIndex = 0;
                    break;
            }
            return oldIndex != SelectedIndex;
        }
    }
}
