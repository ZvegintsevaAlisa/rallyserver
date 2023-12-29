//using System;
//using System.Drawing;
//using System.Threading;
//using System.Threading.Tasks;

//namespace RallyServer
//{
//    internal class Bot : Player
//    {
//        private Size carSize;
//        private bool gameOver;
//        private readonly Car[] cars;
//        private readonly int playerSpeed = 5;


//        public new bool Connected => true;

//        public Bot() : base(null)
//        {
//            carSize  = new Size(55, 100);
//            cars = new Car[]
//            {
//                new Car
//                {
//                    DrawBounds = new RectangleF(new PointF(0, 0), carSize),
//                    Location = CarLocation.Left
//                },
//                new Car
//                {
//                    DrawBounds = new RectangleF(new PointF(0, 0), carSize),
//                    Location = CarLocation.Right
//                }
//            };
//            var roadSize = new Size(370, 530);
//            var x = (roadSize.Width - carSize.Width) / 2;
//            var y = roadSize.Height - carSize.Height - 10;
//            bounds = new RectangleF(new PointF(x, y), carSize);
//        }

//        public override string GetNick()
//        {
//            return "Bot";
//        }

//        public override PointF? GetPosition() => bounds.Location;

//        public override bool SetCarPosition(int index, PointF position)
//        {
//            cars[index].SetPosition(position.X, position.Y);
//            return true;
//        }

//        public override bool SetCarColor(int _1, int _2) => true;

//        public override bool Start()
//        {
//            Task.Run(() =>
//            {
//                DateTime lastTime = DateTime.Now;
//                while (!gameOver)
//                {
//                    var time = DateTime.Now;
//                    var deltaTime = (time - lastTime)
//                        .Duration().Ticks / 200000.0f;
//                    lastTime = time;

//                    foreach (var car in cars)
//                    {
//                        if (car.CollisionBounds.X < bounds.X + carSize.Width &&
//                            car.CollisionBounds.X + carSize.Width > bounds.X &&
//                            bounds.Y - car.CollisionBounds.Bottom < 100)
//                        {
//                            if (car.Location == CarLocation.Left)
//                            {
//                                if (bounds.X + carSize.Width < 320)
//                                    bounds.X += playerSpeed * deltaTime;
//                            }
//                            else
//                            {
//                                if (bounds.X > 34)
//                                    bounds.X -= playerSpeed * deltaTime;
//                            }
//                        }
//                    }

//                    Thread.Sleep(10);
//                }
//            });
//            return true;
//        }

//        public override bool SetOpponentPosition(PointF _) => true;

//        public override bool SetPosition(float _) => true;

//        public override bool SetResult(bool _) => true;

//        public override bool SetScore(float _) => true;

//        public override bool Disconnect()
//        {
//            gameOver = true;
//            return true;
//        }
//    }
//}
