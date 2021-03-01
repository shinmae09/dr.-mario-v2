using DrMarioPlayer.Converter;
using DrMarioPlayer.Model;
using DrMarioPlayer.Utility;
using HackathonClient;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace DrMarioPlayer
{
    internal class Program
    {
        private static readonly IClient _client = new ClientService();
        private static TcpClient socketConnection;
        private static bool firstRun = true;
        private static Tile[,] globalGameBoard = new Tile[Config.Environment.WIDTH, Config.Environment.HEIGHT];
        private static Pill previousPill;
        private static Pill currentPill;
        private static Pill nextPill;

        private static async Task Main(string[] args)
        {
            InitializeLogger();

            socketConnection = await _client.ConnectToServer();
            if (!socketConnection.Connected)
            {
                Log.Information("Not connected to server.");
            }
            Log.Information("Connected to server.");

            while (socketConnection.Connected)
            {
                try
                {
                    
                    string message = await _client.ReceiveMessage(socketConnection);
                    if (!String.IsNullOrEmpty(message))
                    {
                        //Log.Information("Server message : " + message);
                        Task.Run(() => UpdateGameState(message));
                    }
                    

                    //Task.Delay(2000).GetAwaiter().GetResult();
                    //_client.SendMessage(socketConnection, "FFF");

                    //var mes = Console.ReadLine();
                    //_client.SendMessage(socketConnection, mes);

                }
                catch (Exception e)
                {
                    Log.Error(e.StackTrace);
                }
            }
        }

        private static void UpdateGameState(string message)
        {
            Message arbiterMessage = JsonConvert.DeserializeObject<Message>(message);
            var board = arbiterMessage.Coordinates;

            if (board[3, 13] != null && board[4, 13] != null)
            {
                Log.Information("Server message : " + message);
                bool thereIsChange = false;
                Parallel.For(0, globalGameBoard.GetLength(0), (row) =>
                {
                    Parallel.For(0, globalGameBoard.GetLength(1), (col) =>
                    {
                        Tile tile = TileConverter.Convert(board[row, col], row, col);
                        if (!TileHelper.IsTileSimilar(tile, globalGameBoard[row, col]))
                        {
                            if (!firstRun)
                            {
                                if ((row == 3 && col == 13) || (row == 4 && col == 13))
                                {

                                }
                                else
                                {
                                    if (!thereIsChange)
                                    {
                                        Log.Information($"Current: {globalGameBoard.LogGameBoard()}");
                                    }
                                    thereIsChange = true;
                                    Log.Information($"x: {row} y: {col}");
                                }
                            }
                            globalGameBoard[row, col] = tile;
                        }
                    });
                });

                previousPill = currentPill;
                currentPill = new Pill(arbiterMessage.ActivePill);
                nextPill = new Pill(arbiterMessage.NextPill);

            
                try
                {
                    SearchForBestMove(currentPill, nextPill);
                }
                catch (Exception e)
                {
                    Log.Information("ERROR");
                    Log.Error(e.StackTrace);
                }
            }
        }

        private static void SearchForBestMove(Pill currentPill, Pill nextPill)
        {
            Tile[,] gameBoard = (Tile[,]) globalGameBoard.Clone();
            gameBoard[3, 13] = null;
            gameBoard[4, 13] = null;

            Searcher searcher = new Searcher(gameBoard);
            searcher.Search(currentPill);

            var boardScores = new ConcurrentDictionary<Tile[,], double>();
            var firstPillGameBoards = Simulator.LockPills(gameBoard, searcher.LockedPills);


            Parallel.ForEach(firstPillGameBoards.Values, (tempBoard) =>
            //foreach (var tempBoard in firstPillGameBoards.Values)
            {
                Searcher nextPillSearcher = new Searcher(tempBoard);
                nextPillSearcher.Search(nextPill.Clone());

                var nextPillGameBoards = Simulator.LockPills(tempBoard, nextPillSearcher.LockedPills);

                try
                {
                    ConcurrentBag<double> scores = new ConcurrentBag<double>();
                    Parallel.ForEach(nextPillGameBoards.Values, (tempBoard2) =>
                    //foreach (var tempBoard2 in nextPillGameBoards.Values)
                    {
                        double score = Evaluator.Evaluate(tempBoard2);
                        scores.Add(score);
                    });

                    boardScores.TryAdd(tempBoard, scores.Max());
                }
                catch (Exception e)
                {
                    Log.Information("ERROR");
                    Log.Error(e.StackTrace);
                }
            });
            var bestBoard = boardScores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            var bestPill = firstPillGameBoards.Where(x => x.Value == bestBoard).FirstOrDefault().Key;

            /*
            double maxScore = Double.MinValue;
            Pill bestPill = null;
            foreach (var b in firstPillGameBoards)
            {
                double score = Evaluator.Evaluate(b.Value);
                if (score > maxScore)
                {
                    maxScore = score;
                    bestPill = b.Key;
                }
                boardScores.Add(b.Value, score);
            }
            */
            globalGameBoard = bestBoard;
            var moves = searcher.GetMoveSet(bestPill.Position.X, bestPill.Position.Y, bestPill.Orientation);
            var simpMoves = new List<string>();
            for (int i = moves.Count - 1; i >= 0; i--)
            {
                string mes = "";

                switch (moves[i])
                {
                    case Move.DOWN: mes = "D"; break;
                    case Move.RIGHT: mes = "R"; break;
                    case Move.LEFT: mes = "L"; break;
                    case Move.ROTATE_90: mes = "F"; break;
                    case Move.ROTATE_180: mes = "FF"; break;
                    case Move.ROTATE_270: mes = "FFF"; break;
                }

                if (firstRun)
                {
                    Task.Delay(50).GetAwaiter().GetResult();
                    firstRun = false;
                }
                
                _client.SendMessage(socketConnection, mes);
                simpMoves.Add(mes);
                //Console.WriteLine("My Move: " + mes);
            }

            

            if (bestPill.Orientation == Orientation.HORIZONTAL)
            {
                Console.WriteLine($"{Enum.GetName(typeof(Color), bestPill.Color1)} {Enum.GetName(typeof(Color), bestPill.Color2)}");
            }
            else if (bestPill.Orientation == Orientation.REVERSE_HORIZONTAL)
            {
                Console.WriteLine($"{Enum.GetName(typeof(Color), bestPill.Color2)} {Enum.GetName(typeof(Color), bestPill.Color1)}");
            }
            else if (bestPill.Orientation == Orientation.REVERSE_VERTICAL)
            {
                Console.WriteLine($"{Enum.GetName(typeof(Color), bestPill.Color1)}");
                Console.WriteLine($"{Enum.GetName(typeof(Color), bestPill.Color2)}");
            }
            else
            {
                Console.WriteLine($"{Enum.GetName(typeof(Color), bestPill.Color2)}");
                Console.WriteLine($"{Enum.GetName(typeof(Color), bestPill.Color1)}");
            }
            Console.WriteLine($"X: {bestPill.Position.X}   Y: {bestPill.Position.Y}");
            Console.WriteLine(String.Join(" ", simpMoves));
            Console.WriteLine("-----------------");
        }

        private static void InitializeLogger()
        {
            var log = new LoggerConfiguration().WriteTo.File(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DrMario", "log.txt"))
                .CreateLogger();

            Log.Logger = log;
        }
    }
}
