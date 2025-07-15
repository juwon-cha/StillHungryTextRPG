using StillHungry.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StillHungry.UI
{
    internal class QuestUI
    {
        private static QuestUI mInstance;

        public static QuestUI Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new QuestUI();
                }

                return mInstance;
            }


        }
    }
}
