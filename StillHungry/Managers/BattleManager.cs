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
        //public PlayerController PlayerController = new PlayerController();
        public MonsterController MonsterController = new MonsterController();
        public bool isFighting = false;

        public BattleManager() 
        {
            if (MonsterController.ActiveMonsters.Count == 0) MonsterController.SpawnMonstersForBattles(); 
        }
    }
}
