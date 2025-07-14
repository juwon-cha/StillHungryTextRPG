using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.Controller
{
    public class CharacterController
    {
        public int Level { get; protected set; }
        public string Name { get; protected set; }
        public virtual float Attack { get; protected set; }
        public virtual float Defense { get; protected set; }
        public int HP { get; protected set; }
        public int MaxHP { get; protected set; } = 100; // temp: 기본 최대 체력

        public virtual void Init() { }
    }
}
