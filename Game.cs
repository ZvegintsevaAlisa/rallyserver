using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using static RallyServer.Car;

namespace RallyServer
{
    internal class SGame
    {
        private static readonly Random random = new Random();
        private readonly Player player;

        public SGame(Player player)
        {
            this.player = player;
        }

        private void ChangeAIcars(Car car)
        {
            var x = 0.0f;
            var y = random.Next(100, 400) * -1;
            if (car.Location == Car.CarLocation.Left)
                x = random.Next(34, 178);
            else if (car.Location == Car.CarLocation.Right)
                x = random.Next(233, 320);
            car.Color = random.Next(1, 3);
            car.SetPosition(x, y);
        }

        public void Start()
        {
            try
            {
                var score = 0.0f;
                var nick = "";
                var task = Task.Run(() => nick = player.GetNick());
                task.Wait();
                var roadSize = new Size(370, 530);
                var carSize = new Size(55, 100);
                var gameOver = false;
                var roadSpeed = 4.0f;
                var trafficSpeed = 3.0f;
                var cars = new Car[]
                {
                    new Car(CarLocation.Left),
                    new Car(CarLocation.Right)
                };

                for (int i = 0; i < cars.Length; i++)
                    ChangeAIcars(cars[i]);

                var lastTime = DateTime.Now;
                var playerY = 0.0f;

                for (int i = 0; i < cars.Length; i++)
                {
                    var location = cars[i].DrawBounds.Location;
                    var color = cars[i].Color;
                    player.SetCarPosition(i, location);
                    player.SetCarColor(i, color);
                }

                player.Start();


                while (!gameOver)
                {
                    var time = DateTime.Now;
                    var deltaTime = (time - lastTime)
                        .Duration().Ticks / 200000.0f;
                    lastTime = time;

                    score += deltaTime;
                    player.SetScore(score);

                    var player1Position = player.GetPosition();

                    playerY += roadSpeed * deltaTime;
                    player.SetPosition(playerY);
 

                    foreach (var car in cars)
                        car.MoveDown(trafficSpeed * deltaTime);

                    for (int i = 0; i < cars.Length; i++)
                    {
                        if (cars[i].DrawBounds.Top > roadSize.Height)
                        {
                            ChangeAIcars(cars[i]);
                            player.SetCarColor(i, cars[i].Color);
                        }
                    }

                    for (int i = 0; i < cars.Length; i++)
                    {
                        var location = cars[i].DrawBounds.Location;
                        player.SetCarPosition(i, location);
                    }

                    foreach (var car in cars)
                    {
                        if (player.CollisionBounds.IntersectsWith(car.CollisionBounds))
                        {
                            gameOver = true;
                            ScoreTable.Add(player.Nick, score);
                            //Console.WriteLine(player.Nick + score);
                            player.SetResult();
                            player.Disconnect();
                        }
                       
                    }

                    roadSpeed += 1 / 150.0f;
                    trafficSpeed += 2 / 150.0f;
                    Thread.Sleep(10);
                }

            }
            catch { }
        }
    }

    internal class MPGame
    {
        private static readonly Random random = new Random();
        private readonly Player player1;
        private readonly Player player2;

        public MPGame(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }

        private void ChangeAIcars(Car car)
        {
            var x = 0.0f;
            var y = random.Next(100, 400) * -1;
            if (car.Location == Car.CarLocation.Left)
                x = random.Next(34, 178);
            else if (car.Location == Car.CarLocation.Right)
                x = random.Next(233, 320);
            car.Color = random.Next(1, 3);
            car.SetPosition(x, y);
        }

        public void Start()
        {
            try
            {
                var score = 0.0f;
                var nick1 = "";
                var nick2 = "";
                var task1 = Task.Run(() => nick1 = player1.GetNick());
                var task2 = Task.Run(() => nick2 = player2.GetNick());
                task1.Wait();
                task2.Wait();
                var winner = (Player)null;
                var loser = (Player)null;

                var roadSize = new Size(370, 530);
                var carSize = new Size(55, 100);
                var gameOver = false;
                var roadSpeed = 4.0f;
                var trafficSpeed = 3.0f;
                var cars = new Car[]
                {
                    new Car(CarLocation.Left),
                    new Car(CarLocation.Right)
                };

                for (int i = 0; i < cars.Length; i++)
                    ChangeAIcars(cars[i]);

                var lastTime = DateTime.Now;
                var playerY = 0.0f;

                for (int i = 0; i < cars.Length; i++)
                {
                    var location = cars[i].DrawBounds.Location;
                    var color = cars[i].Color;
                    player1.SetCarPosition(i, location);
                    player2.SetCarPosition(i, location);
                    player1.SetCarColor(i, color);
                    player2.SetCarColor(i, color);
                }

                player1.Start();
                player2.Start();

                while (!gameOver)
                {
                    var time = DateTime.Now;
                    var deltaTime = (time - lastTime)
                        .Duration().Ticks / 200000.0f;
                    lastTime = time;

                    score += deltaTime;
                    player1.SetScore(score);
                    player2.SetScore(score);

                    var player1Position = player1.GetPosition();
                    var player2Position = player2.GetPosition();
                    player2.SetOpponentPosition((PointF)player1Position);
                    player1.SetOpponentPosition((PointF)player2Position);

                    playerY += roadSpeed * deltaTime;
                    player1.SetPosition(playerY);
                    player2.SetPosition(playerY);

                    foreach (var car in cars)
                        car.MoveDown(trafficSpeed * deltaTime);

                    for (int i = 0; i < cars.Length; i++)
                    {
                        if (cars[i].DrawBounds.Top > roadSize.Height)
                        {
                            ChangeAIcars(cars[i]);
                            player1.SetCarColor(i, cars[i].Color);
                            player2.SetCarColor(i, cars[i].Color);
                        }
                    }

                    for (int i = 0; i < cars.Length; i++)
                    {
                        var location = cars[i].DrawBounds.Location;
                        player1.SetCarPosition(i, location);
                        player2.SetCarPosition(i, location);
                    }

                    foreach (var car in cars)
                    {
                        if (player1.CollisionBounds.IntersectsWith(car.CollisionBounds))
                        {
                            loser = player1;
                            winner = player2;
                            gameOver = true;
                        }
                        else if (player2.CollisionBounds.IntersectsWith(car.CollisionBounds))
                        {
                            loser = player2;
                            winner = player1;
                            gameOver = true;
                        }
                    }

                    roadSpeed += 1 / 300.0f;
                    trafficSpeed += 2 / 300.0f;
                    Thread.Sleep(10);
                }

                if (loser != null)
                {
                    loser.SetResultMP(false);
                    ScoreTable.Add(loser.Nick, score);
                    loser.Disconnect();
                }
                if (winner != null)
                {
                    winner.SetResultMP(true);
                    ScoreTable.Add(winner.Nick, score);
                    winner.Disconnect();
                }

            }
            catch { }
        }
    }
    //internal class Game
    //{
    //    private static readonly Random random = new Random();
    //    private readonly Player player1;
    //    private readonly Player player2;

    //    public Game(Player player1, Player player2) 
    //    {
    //        this.player1 = player1;
    //        this.player2 = player2;
    //    }

    //    private void ChangeAIcars(Car car)
    //    {
    //        var x = 0.0f;
    //        var y = random.Next(100, 400) * -1;
    //        if (car.Location == Car.CarLocation.Left)
    //            x = random.Next(34, 178);
    //        else if (car.Location == Car.CarLocation.Right)
    //            x = random.Next(233, 320);
    //        car.Color = random.Next(1, 3);
    //        car.SetPosition(x, y);
    //    }

    //    public void Start()
    //    {
    //        try
    //        {
    //            var score = 0.0f;
    //            var nick1 = "";
    //            var nick2 = "";
    //            var task1 = Task.Run(() => nick1 = player1.GetNick());
    //            var task2 = Task.Run(() => nick2 = player2.GetNick());
    //            task1.Wait();
    //            task2.Wait();
    //            var winner = (Player)null;
    //            var loser = (Player)null;

    //            var roadSize = new Size(370, 530);
    //            var carSize = new Size(55, 100);
    //            var gameOver = false;
    //            var roadSpeed = 4.0f;
    //            var trafficSpeed = 3.0f;
    //            var cars = new Car[]
    //            {
    //                new Car(CarLocation.Left),
    //                new Car(CarLocation.Right)
    //            };

    //            for (int i = 0; i < cars.Length; i++)
    //                ChangeAIcars(cars[i]);

    //            var lastTime = DateTime.Now;
    //            var playerY = 0.0f;

    //            for (int i = 0; i < cars.Length; i++)
    //            {
    //                var location = cars[i].DrawBounds.Location;
    //                var color = cars[i].Color;
    //                player1.SetCarPosition(i, location);
    //                player2.SetCarPosition(i, location);
    //                player1.SetCarColor(i, color);
    //                player2.SetCarColor(i, color);
    //            }

    //            player1.Start();
    //            player2.Start();

    //            while (!gameOver)
    //            {
    //                var time = DateTime.Now;
    //                var deltaTime = (time - lastTime)
    //                    .Duration().Ticks / 200000.0f;
    //                lastTime = time;

    //                score += deltaTime;
    //                player1.SetScore(score);
    //                player2.SetScore(score);

    //                var player1Position = player1.GetPosition();
    //                var player2Position = player2.GetPosition();
    //                player2.SetOpponentPosition((PointF)player1Position);
    //                player1.SetOpponentPosition((PointF)player2Position);

    //                playerY += roadSpeed * deltaTime;
    //                player1.SetPosition(playerY);
    //                player2.SetPosition(playerY);

    //                foreach (var car in cars)
    //                    car.MoveDown(trafficSpeed * deltaTime);

    //                for (int i = 0; i < cars.Length; i++)
    //                {
    //                    if (cars[i].DrawBounds.Top > roadSize.Height)
    //                    {
    //                        ChangeAIcars(cars[i]);
    //                        player1.SetCarColor(i, cars[i].Color);
    //                        player2.SetCarColor(i, cars[i].Color);
    //                    }
    //                }

    //                for (int i = 0; i < cars.Length; i++)
    //                {
    //                    var location = cars[i].DrawBounds.Location;
    //                    player1.SetCarPosition(i, location);
    //                    player2.SetCarPosition(i, location);
    //                }

    //                foreach (var car in cars)
    //                {
    //                    if (player1.CollisionBounds.IntersectsWith(car.CollisionBounds))
    //                    {
    //                        loser = player1;
    //                        winner = player2;
    //                        gameOver = true;
    //                    }
    //                    else if (player2.CollisionBounds.IntersectsWith(car.CollisionBounds))
    //                    {
    //                        loser = player2;
    //                        winner = player1;
    //                        gameOver = true;
    //                    }
    //                }

    //                roadSpeed += 1 / 300.0f;
    //                trafficSpeed += 2 / 300.0f;
    //                Thread.Sleep(10);
    //            }

    //            if (loser != null)
    //            {
    //                loser.SetResult(false);
    //                loser.Disconnect();
    //            }
    //            if (winner != null)
    //            {
    //                winner.SetResult(true);
    //                ScoreTable.Add(winner.Nick, score);
    //                winner.Disconnect();
    //            }

    //        }
    //        catch { }
    //    }
    //}
}
