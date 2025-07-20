
using StillHungry.Commands;
using StillHungry.Managers;
using StillHungry.Skills;
using StillHungry.UI;

namespace StillHungry.Scene
{
    public class SkillSelectScene : BaseScene
    {
        private List<IExecutable> mMenuCommands = new List<IExecutable>();
        private MenuNavigator mNavigator;

        public SkillSelectScene()
        {

        }

        // 플레이어 스킬 개수에 따라 커맨드 생성
        public void GenerateSkillSelectCommands()
        {
            mMenuCommands.Clear();

            var skills = Manager.Instance.Game.PlayerController.ActiveSkills;
            foreach(Skill s in skills)
            {
                mMenuCommands.Add(new UseSkillCommand(s));
            }

            mMenuCommands.Add(new ChangeSceneCommand(ESceneType.BATTLE_SCENE));
            mNavigator = new MenuNavigator(skills.Count + 1);
        }

        public override void Display()
        {
            ProcessInput(mMenuCommands.ToArray(), mNavigator);
            Render();
        }

        public override void Render()
        {
            if (!bNeedsRedraw)
            {
                return;
            }
            Console.Clear();
            Manager.Instance.UI.ShowSkillSelect(mNavigator.SelectedIndex);
            bNeedsRedraw = false;
        }
    }
}
