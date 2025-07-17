using StillHungry.Controller;
using StillHungry.Managers;
using StillHungry.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// 삭제 예정
namespace StillHungry
{
    public struct PassiveSkillStat
    {
        public string Name;
        public string ID;
        public string Description;
    }

    internal class PassiveSkill
    {
        // 경험치 증가 ID : 1
        public void ExpIncrease()
        {
            // 경험치 증가 로직 구현
            // 예시: 플레이어의 경험치 증가
        }

        // 속공 ID : 2
        public void QuickStrike()
        {
            // 속공 로직 구현
            // 예시: 플레이어의 공격 속도 증가
        }

        // 불굴 ID : 3
        bool isUseess = false; // 사용 불가능한 상태
        public void Indomitable()
        {
            // 플레이어 현재 체력
            if(!isUseess)
            {
                Manager.Instance.Game.PlayerController.HP = 1;
                isUseess = true; // 스킬 사용 후 사용 불가능 상태로 변경
            }
        }

        // 더블어택 ID : 4
        public void DoubleAttack()
        {
            // 더블 어택 로직 구현
            // 예시: 플레이어가 공격 시 두 번 공격하는 효과
        }

        // 확률적 즉사 ID : 5
        public void InstantDeath()
        {
            // 확률적 즉사 로직 구현
            // 예시: 플레이어의 공격이 적을 즉사시킬 확률 증가
        }
    }
}
