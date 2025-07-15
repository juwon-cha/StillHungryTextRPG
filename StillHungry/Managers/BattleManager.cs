using StillHungry.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.Managers
{
    public class BattleManager
    {
        public int monsterKillCount = 0;

        //public PlayerController PlayerController = new PlayerController();
        public MonsterController MonsterController = new MonsterController();
        public bool isFighting = false;

        public int initialHP; //전투 시작 시의 플레이어 체력
        public int totalDamageTaken = 0; //전투 중 받은 누적 피해
        
        public BattleManager() 
        {
            if (MonsterController.ActiveMonsters.Count == 0) 
                MonsterController.SpawnMonstersForBattles(); 
        }

        public void startBattle() //전투 시작 시 호출될 함수
        {
            isFighting = true;
            initialHP = Manager.Instance.Game.PlayerController.HP;
            totalDamageTaken = 0;
        }
        public void EndBattle(bool isVictory, int initialHP, int damageTaken, int monsterKillCount)
        {
            var player = Manager.Instance.Game.PlayerController;

            Console.Clear();
            Console.WriteLine("Battle!! - Result\n");

            if (isVictory)
            {
                Console.WriteLine("Victory\n");
                Console.WriteLine($"던전에서 몬스터 {monsterKillCount}마리를 잡았습니다.");
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {initialHP} -> {player.HP}\n");
                Console.WriteLine("0. 다음");
                Console.Write("\n>>");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    isFighting = false;
                    Manager.Instance.Scene.ChangeScene(ESceneType.TOWN_SCENE);
                }
            }
            else
            {
                Console.WriteLine("You Lose\n");
                Console.WriteLine($"Lv.{player.Level} {player.Name}");
                Console.WriteLine($"HP {initialHP} -> {player.HP}\n");
                Console.WriteLine("0. 다음");
                Console.Write("\n>>");
                string input = Console.ReadLine();

                if (input == "0")
                {
                    Console.WriteLine("\n게임 종료");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            }
        }
    }
}

