using StillHungry.Managers;
using StillHungry.Monsters;

namespace StillHungry.Controller
{
    public class MonsterController
    {
        private Random mRand = new Random();

        public Dictionary<int, Monster> ActiveMonsters = new Dictionary<int, Monster>();

        // 전투를 위한 몬스터 스폰
        public void SpawnMonstersForBattles()
        {
            ActiveMonsters.Clear();
            // 1~3마리의 몬스터를 생성
            int numberOfMonsters = mRand.Next(1, 4); 

            for (int i = 1; i <= numberOfMonsters; i++)
            {
                // TODO: 랜덤 몬스터 스폰
                ActiveMonsters.Add(i, Monster.SpawnMonster(i));
            }
            Console.WriteLine($"\n{numberOfMonsters}마리의 몬스터가 나타났습니다!");
        }

        public void TakeDamage(int monsterId, int damage)
        {
            Monster targetMonster = null;
            if (ActiveMonsters.TryGetValue(monsterId, out targetMonster) && targetMonster.CurrentHp > 0)
            {
                // 방어력을 고려한 데미지 계산
                int finalDamage = damage - (int)targetMonster.DefensePower;
                if (finalDamage < 1) finalDamage = 1; // 최소 1의 데미지는 보장

                targetMonster.CurrentHp -= finalDamage;

                Console.WriteLine($"{targetMonster.Name}에게 {finalDamage}의 데미지를 입혔습니다! (남은 체력: {targetMonster.CurrentHp})\n");

                if (targetMonster.CurrentHp <= 0)
                {
                    OnMonsterDeath(targetMonster);
                }
            }
        }

        // 몬스터 턴 실행
        public void ExecuteMonsterTurn()
        {
            if (ActiveMonsters.Count == 0)
            {
                // 모든 몬스터가 이미 처치된 경우 턴을 종료
                return;
            }

            Console.WriteLine("\n[몬스터 턴]");
            
            // TODO: 활성화된 모든 몬스터의 턴 처리?
            foreach (var monster in ActiveMonsters)
            {
                if (monster.Value.CurrentHp > 0) // 몬스터가 살아있는 경우에만 행동
                {
                    ProcessMonsterTrun(monster.Key);
                }
            }
        }

        /// 몬스터가 확률에 따라 공격 또는 방어를 결정하고 수행
        private void ProcessMonsterTrun(int id)
        {
            Monster monster = GetMonsterFromID(id);

            // 이전 턴의 방어 상태 초기화
            if (monster.mbIsDefending)
            {
                monster.DefensePower /= 2; // 방어 시 증가했던 방어력을 원상 복구
                monster.mbIsDefending = false;
            }

            int actionChance = mRand.Next(1, 101); // 1에서 100 사이의 난수 생성

            if (actionChance <= 70) // 70% 확률로 공격
            {
                ExecuteAttack(monster);
            }
            else // 30% 확률로 방어
            {
                ExecuteDefense(monster);
            }
        }

        private void ExecuteAttack(Monster monster)
        {
            Console.WriteLine($"{monster.Name}이(가) 플레이어를 공격합니다!");

            Manager.Instance.Game.PlayerController.TakeDamage((int)monster.AttackPower);

            Console.WriteLine($"플레이어는 {monster.AttackPower}의 데미지를 입었습니다. (시뮬레이션)");
        }

        private void ExecuteDefense(Monster monster)
        {
            monster.mbIsDefending = true;
            // 방어 시 다음 공격에 대한 방어력을 2배로 증가
            monster.DefensePower *= 2; 

            Console.WriteLine($"{monster.Name}이(가) 방어 자세를 취합니다! (다음 공격에 대한 방어력이 증가합니다)");
        }

        private void OnMonsterDeath(Monster monster)
        {
            Console.WriteLine($"{monster.Name}을(를) 처치했습니다!");
            ActiveMonsters.Remove(monster.ID);

            // TODO: 경험치 획득, 아이템 드랍 로직 추가

            Manager.Instance.Battle.monsterKillCount++;

            // 모든 몬스터를 처치했는지 확인
            if (ActiveMonsters.Count == 0)
            {
                Console.WriteLine("모든 몬스터를 처치했습니다! 전투에서 승리했습니다!");
                
                Thread.Sleep(1000); // 승리 메시지 후 잠시 대기

                var battleManager = Manager.Instance.Battle;
                battleManager.EndBattle
                    (
                    isVictory: true,
                    initialHP: battleManager.initialHP,
                    damageTaken: battleManager.totalDamageTaken,
                    monsterKillCount: battleManager.monsterKillCount
                    );

                // 전투 종료 후 던전 씬으로 전환
                //Manager.Instance.Scene.ChangeScene(ESceneType.DUNGEON_SCENE);
            }
        }

        public Monster GetMonsterFromID(int monsterId)
        {
            if(ActiveMonsters.TryGetValue(monsterId, out Monster monster))
            {
                return monster;
            }

            return null; // 해당 ID의 몬스터가 없으면 null을 반환합니다.
        }
    }
}
