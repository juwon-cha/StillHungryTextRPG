using StillHungry.Commands;
using StillHungry.Managers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace StillHungry.Scene
{
    internal class StoryScene : BaseScene
    {
        private bool _hasPlayedStory;
        private bool _skipRequested;
        private CancellationTokenSource _cts;

        private struct StoryLine
        {
            public string Text;
            public int Speed;

            public StoryLine(string text, int speed)
            {
                Text = text;
                Speed = speed;
            }
        }

        public override void Display()
        {
            Update();
            Render();
        }

        public override void Render()
        {
            Console.Clear();

            if (!_hasPlayedStory)
            {
                _skipRequested = false;
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                // 스킵 감지 Task 실행
                Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Enter)
                        {
                            _skipRequested = true;
                            break;
                        }
                    }
                }, token);

                ShowStoryScreen();

                _cts.Cancel(); // 스킵 감지 종료

                Console.WriteLine();
                Console.WriteLine("계속하려면 [Enter] 키를 누르세요...");
                Console.ReadLine();

                _hasPlayedStory = true;

                new ChangeSceneCommand(ESceneType.TOWN_SCENE).Execute();
            }
        }

        public void ShowStoryScreen()
        {
            var story = new List<StoryLine>
            {
                new StoryLine("당신은 21세기 현대 사회를 살아가던, 그저 그런 평범한 사람이었습니다.", 60),
                new StoryLine("평소처럼 일을 마치고, 피곤한 몸을 이끌고 집으로 돌아가던 어느 저녁—", 60),
                new StoryLine("", 500),
                new StoryLine("", 500),
                new StoryLine("쿵!", 1000), // 강한 임팩트
                new StoryLine("", 100),
                new StoryLine("갑작스러운 충격과 함께 시야가 하얘졌고,", 60),
                new StoryLine("의식이 끊기기 직전, 당신의 귓가엔 기묘한 속삭임이 들려왔습니다", 60),
                new StoryLine("", 100),
                new StoryLine("“욿..리쎍켽릁 짦……부탃틊려요”", 140),
                new StoryLine("", 0),

                new StoryLine("그 후 당신은 눈을 뜨게 됩니다.", 35),
                new StoryLine("여긴 익숙하지 않은 풍경.", 35),
                new StoryLine("푸른 숲, 붉은 하늘, 중세풍의 복장을 한 사람들...", 60),
                new StoryLine("", 0),

                new StoryLine("지구와는 닮았지만 분명 다른 이곳은, 또 다른 세계 ‘노움(Noam)’입니다.", 70),
                new StoryLine("당신은 이유도, 목적도 모른 채 이 땅에 떨어졌고,", 60),
                new StoryLine("몇 날 며칠을 방황하며 간신히 살아남았습니다.", 60),
                new StoryLine("이세계는 결코 만만하지 않았고, 혼자 살아남기엔 너무도 낯선 존재였습니다.", 60),
                new StoryLine("", 0),

                new StoryLine("그러던 중 당신은 한 도시의 입구에서 '모험가 길드'라는 간판을 발견합니다.", 60),
                new StoryLine("그곳에선 다양한 이세계인들과 주민들이 모여 정보를 교환하고, 생존을 위한 임무를 나눈다고 합니다.", 60),
                new StoryLine("", 0),

                new StoryLine("당신은 결심합니다.", 60),
                new StoryLine("죽이되던 밥이되던 일단은 이 세계에서 살아남겠다고.", 60),
                new StoryLine("", 0),

                new StoryLine("이제, 당신의 새로운 여정이 시작됩니다...", 90)
            };

            foreach (var line in story)
            {
                if (_skipRequested)
                {
                    Console.WriteLine(line.Text);
                }
                else
                {
                    TypeLine(line.Text, line.Speed);
                }
                Console.WriteLine();
            }
        }

        private void TypeLine(string text, int speed)
        {
            foreach (char c in text)
            {
                if (_skipRequested)
                {
                    Console.Write(text.Substring(text.IndexOf(c)));
                    return;
                }

                Console.Write(c);
                Thread.Sleep(speed);
            }
        }

        protected override void Update()
        {
            // 필요 시 업데이트 처리
        }
    }
}
