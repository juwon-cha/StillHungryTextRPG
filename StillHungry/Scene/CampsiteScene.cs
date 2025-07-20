using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public enum ERestResult
    {
        SUCCESS,
        NOT_ENOUGH_GOLD,
        FULL_HP
    }

    public class CampsiteScene : BaseScene
    {
        private readonly string[] mMenuItems = { "휴식하기", "나가기" };
        private readonly IExecutable[] mMenuCommands;
        private readonly MenuNavigator mNavigator;

        public CampsiteScene()
        {
            mNavigator = new MenuNavigator(mMenuItems.Length);
            mMenuCommands = new IExecutable[]
            {
                new RestCommand(RequestRedraw),
                new ChangeSceneCommand(ESceneType.GUILD_SCENE)
            };
        }

        public override void Display()
        {
            ProcessInput(mMenuCommands, mNavigator);
            Render();
        }

        public override void Render()
        {
            if (!bNeedsRedraw)
            {
                return;
            }

            Console.Clear();
            Manager.Instance.UI.ShowCampsiteScreen(mMenuItems, mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }
    }
}