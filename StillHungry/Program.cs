using StillHungry.Managers;

namespace StillHungry
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                //as 
                Manager.Instance.Scene.DisplayScene();

            }
        }
    }
}
