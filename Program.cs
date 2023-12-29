using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RallyServer
{
    internal class Program
    {
        private static Queue<Player> waitingPlayers;

        public static void GameMatching(Player player)
        {
            waitingPlayers.Enqueue(player);
            if (waitingPlayers.Count >= 2)
            {
                var player1 = waitingPlayers.Dequeue();
                var player2 = waitingPlayers.Dequeue();
                if (!player1.Connected)
                    waitingPlayers.Enqueue(player2);
                else if (!player2.Connected)
                    waitingPlayers.Enqueue(player1);
                else
                {
                    var game = new MPGame(player1, player2);
                    Task.Run(() => game.Start());
                }
            }
        }

        static void Main(string[] _)
        {
            waitingPlayers = new Queue<Player>();
            var port = 1366;
            var scoresPort = 1367;
            var ipPoint = new IPEndPoint(IPAddress.Any, port);
            var ipPointScores = new IPEndPoint(IPAddress.Any, scoresPort);
            var socketListener = new Socket(AddressFamily.InterNetwork, 
                SocketType.Stream, ProtocolType.Tcp);
            var socketListenerScores = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);
            socketListener.Bind(ipPoint);
            socketListenerScores.Bind(ipPointScores);
            var quit = false;
            Console.WriteLine($"Server starting ...");
            var task = Task.Run(() =>
            {
                socketListener.Listen(1024);
                Console.WriteLine($"Server started, port: {port}");
                Console.WriteLine($"Press Q to stop server.");
                while (!quit)
                {
                    try
                    {
                        var socket = socketListener.Accept();
                        var player = new Player(socket);
                        if (player.SGame())
                        {

                            //var bot = new Bot();
                            //var game = new MPGame(player, bot);
                            //Task.Run(() => game.Start());
                            var game = new SGame(player);
                            Task.Run( () => game.Start());
                        }
                        else
                            GameMatching(player);
                    }
                    catch { };
                }
            });
            var scoresTask = Task.Run(() =>
            {
                socketListenerScores.Listen(1024);
                while (!quit)
                {
                    try
                    {
                        var socket = socketListenerScores.Accept();
                        var client = new Client(socket);
                        var scores = ScoreTable.GetScores()
                            .OrderByDescending(x => x.Score).ToList();
                        client.SendInt(scores.Count());
                        foreach (var score in scores)
                        {
                            client.SendString(score.Nick);
                            client.SendFloat(score.Score);
                        }
                    }
                    catch { };
                }
            });
            while (!quit)
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Q)
                {
                    Console.WriteLine($"Server stopping ...");
                    quit = true;
                    socketListener.Close();
                    socketListenerScores.Close();
                }
            }
            task.Wait();
            ScoreTable.SaveScores();
            Console.WriteLine($"Server stoped.");
        }
    }
}
