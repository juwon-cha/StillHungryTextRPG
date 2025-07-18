using StillHungry.Data;
using StillHungry.Managers;
using StillHungry.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.Skills
{
    public class Skill
    {
        public int ID;
        public string Name;
        public string Description;
        public int RequiredMP;
        public float DamageMultiplier;
        public float DefenseMultiplier;
        public float CriticalMultiplier;
        public float EvadeMultiplier;
        public bool IsRangeAttack;

        public Skill(int id)
        {
            Init(id);
        }

        private void Init(int id)
        {
            SkillData skillData;
            if (DataManager.SkillDict.TryGetValue(id, out skillData))
            {
                ID = skillData.ID;
                Name = skillData.Name;
                Description = skillData.Description;
                RequiredMP = skillData.RequiredMP;
                DamageMultiplier = skillData.DamageMultiplier;
                DefenseMultiplier = skillData.DefenseMultiplier;
                CriticalMultiplier = skillData.CriticalMultiplier;
                EvadeMultiplier = skillData.EvadeMultiplier;
                IsRangeAttack = skillData.IsRangeAttack;
            }
        }

        static public Skill MakeSkill(int skillID)
        {
            Skill skill = null;
            SkillData skillData;

            // json 데이터에서 몬스터 정보 가져옴
            if (DataManager.SkillDict.TryGetValue(skillID, out skillData))
            {
                skill = new Skill(skillID);
            }
            else
            {
                throw new KeyNotFoundException($"Skill with ID {skillID} not found.");
            }

            return skill;
        }
    }
}
