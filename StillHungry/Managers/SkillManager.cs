using StillHungry.Items;
using StillHungry.Skills;

namespace StillHungry.Managers
{
    class SkillManager
    {
        public Dictionary<int, Skill> ActiveSkills = new Dictionary<int, Skill>();

        public void Init()
        {
            // 아이템 추가
            foreach (int skillId in DataManager.SkillDict.Keys)
            {
                Skill skill = Skill.MakeSkill(skillId);
                if (skill != null)
                {
                    Add(skill);
                }
            }
        }

        public void Add(Skill skill)
        {
            ActiveSkills.Add(skill.ID, skill);
        }

        // 스킬 아이디로 스킬 가져옴
        public Skill GetSkillFromID(int skillId)
        {
            Skill skill = null;

            ActiveSkills.TryGetValue(skillId, out skill);

            return skill;
        }
    }
}
