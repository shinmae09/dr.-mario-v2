using DrMarioPlayer.Converter;
using DrMarioPlayer.Model;
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
        private static Tile[,] gameBoard = new Tile[Config.Environment.WIDTH, Config.Environment.HEIGHT];
        private static Partner[,] partnerBoard = new Partner[Config.Environment.WIDTH, Config.Environment.HEIGHT];
        private static Pill previousPill;
        private static Pill currentPill;
        private static Pill nextPill;
        private static bool goodToGo = true;
        private static bool testRun;

        private static async Task Main(string[] args)
        {
            InitializeLogger();
            InitializeTestRun();

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
                        goodToGo = true;
                        Task.Run(() => UpdateGameState(message));
                        Log.Information("Server message : " + message);
                    }
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
            currentPill = new Pill(arbiterMessage.ActivePill);
            nextPill = new Pill(arbiterMessage.NextPill);

            if ((board[3, 13] != null && board[4, 13] != null) || IsNextPill())
            {
                previousPill = currentPill;
                for (int row = 0; row < gameBoard.GetLength(0); row++)
                {
                    for (int col = 0; col < gameBoard.GetLength(1); col++)
                    {
                        Tile tile = TileConverter.Convert(board[row, col]);
                        if (tile != gameBoard[row, col])
                        {
                            if (!IsSpawnPoint(row, col))
                            {
                                Log.Information("Something went wrong");
                            }

                            gameBoard[row, col] = tile;
                            partnerBoard[row, col] = Partner.NONE;
                        }
                    }
                }

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
            gameBoard[3, 13] = Tile.NONE;
            gameBoard[4, 13] = Tile.NONE;

            Searcher searcher = new Searcher(gameBoard);
            searcher.Search(currentPill);

            var boardScores = new ConcurrentDictionary<Tuple<Tile[,], Partner[,]>, double>();
            var firstPillGameBoards = Simulator.LockPills(gameBoard, partnerBoard, searcher.LockedPills);

            if (currentPill.Color1 != Color.WHITE)
            {
                Parallel.ForEach(firstPillGameBoards.Values, (board) =>
                {
                    Searcher nextPillSearcher = new Searcher(board.Item1);
                    nextPillSearcher.Search(nextPill.Clone());

                    var nextPillGameBoards = Simulator.LockPills(board.Item1, board.Item2, nextPillSearcher.LockedPills);
                    ConcurrentBag<double> scores = new ConcurrentBag<double>();

                    Parallel.ForEach(nextPillGameBoards.Values, (board2) =>
                    {
                        double score = Evaluator.Evaluate(board2.Item1);
                        scores.Add(score);
                    });

                    boardScores.TryAdd(board, scores.Max());
                });
            }
            else
            {
                Parallel.ForEach(firstPillGameBoards.Values, (board) =>
                {
                    double score = Evaluator.Evaluate(board.Item1);
                    boardScores.TryAdd(board, score);
                });
            }

            var maxScore = boardScores.Values.Max();
            var bestBoards = boardScores.Where(x => x.Value == maxScore);
            var bestBoard = bestBoards.FirstOrDefault().Key;
            
            if (bestBoards.Count() > 1)
            {
                var tempBoardScores = new ConcurrentDictionary<Tuple<Tile[,], Partner[,]>, double>();
                Parallel.ForEach(bestBoards, (board) =>
                {
                    double score = Evaluator.Evaluate(board.Key.Item1);
                    tempBoardScores.TryAdd(board.Key, score);
                    Console.WriteLine(score);
                });
                
                bestBoard = tempBoardScores.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
            }

            var bestPill = firstPillGameBoards.Where(x => x.Value == bestBoard).FirstOrDefault().Key;

            gameBoard = bestBoard.Item1;
            partnerBoard = bestBoard.Item2;

            List<Move> moves = searcher.GetMoveSet(bestPill.Position.X, bestPill.Position.Y, bestPill.Orientation);
            string movesToBeSent = string.Empty;
            var simpMoves = new List<string>();
            if (!testRun)
            {
                for (int i = moves.Count - 1; i >= 0; i--)
                {
                    switch (moves[i])
                    {
                        case Move.DOWN: movesToBeSent += "D"; break;
                        case Move.RIGHT: movesToBeSent += "R"; break;
                        case Move.LEFT: movesToBeSent += "L"; break;
                        case Move.ROTATE_90: movesToBeSent += "F"; break;
                    }

                    bool isLastMoveDown = movesToBeSent.Last() == 'D';
                    if (isLastMoveDown || movesToBeSent.Length == 3 || i == 0)
                    {
                        while (!goodToGo)
                        {
                            Task.Delay(10).GetAwaiter().GetResult();
                        }

                        goodToGo = false;

                        _client.SendMessage(socketConnection, movesToBeSent);
                        simpMoves.Add(movesToBeSent);

                        if (!isLastMoveDown)
                        {
                            Task.Delay(100).GetAwaiter().GetResult();
                            goodToGo = true;
                        }

                        movesToBeSent = string.Empty;
                    }
                }
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

        private static bool IsNextPill()
        {
            return previousPill.Color1 != currentPill.Color1 &&
                previousPill.Color2 != currentPill.Color2;
        }

        private static bool IsSpawnPoint(int x, int y)
        {
            return (x == 3 && y == 13)
                || (x == 4 && y == 13);
        }
        
        private static void InitializeLogger()
        {
            if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DrMario", "log4.txt")))
            {
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DrMario", "log4.txt"));
            }

            var log = new LoggerConfiguration().WriteTo.File(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DrMario", "log4.txt"))
                .CreateLogger();

            Log.Logger = log;
        }

        private static void InitializeTestRun()
        {
            try
            {
                string testMessage = "" +
                    "{'Player':" +
                        "{'Coordinates':[" +
                            "[null,null,null,null,null,null,null,null,null,null,null,null,null,null]," +
                            "[null,null,null,null,null,null,null,null,null,null,null,null,null,null]," +
                            "[null,null,null,null,null,null,null,null,null,null,null,null,null,null]," +
                            "[null,null,null,null,null,null,null,null,null,null,null,null,null,'Blue']," +
                            "[null,null,null,null,null,null,null,null,null,null,null,null,null,'Blue']," +
                            "[null,null,null,null,null,null,null,null,null,null,null,null,null,null]," +
                            "[null,null,null,null,null,null,null,null,null,null,null,null,null,null]," +
                            "[null,null,null,null,null,null,null,null,null,null,null,null,null,null]]," +
                        "'Score':0," +
                        "'Status':null," +
                        "'VirusRemaining':20," +
                        "'ActivePill':'BBPill'," +
                        "'NextPill':'GYPill'}}";
                testRun = true;
                UpdateGameState(testMessage);
                UpdateGameState(testMessage);
                testRun = false;
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
        }
    }
}
