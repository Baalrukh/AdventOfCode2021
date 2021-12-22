using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2021 {
    public class Day21 : Exercise {
        public long ExecutePart1(string[] lines) {
            var positions = lines.Select(x => x.Last() - '0' - 1).ToList();

            int[] scores = new int[2];

            int diceRollCount = 0;
            int dice = 1;
            int player = 1;
            while (scores[player] < 1000)
            {
                player = 1 - player;
                int moves;
                switch (dice)
                {
                    case 99:
                        moves = 99 + 100 + 1;
                        break;
                    case 100:
                        moves = 100 + 1 + 2;
                        break;
                    default:
                        moves = dice * 3 + 3;
                        break;
                }

                dice = (dice + 3) % 100;
                var newPosition = (positions[player] + moves) % 10;
                positions[player] = newPosition;
                scores[player] += newPosition + 1;
                diceRollCount += 3;
            }

            return scores.Min() * diceRollCount;
        }

        private static readonly Dictionary<int, int> RollsResult = new Dictionary<int, int>()
        {
            {3, 1},
            {4, 3},
            {5, 6},
            {6, 7},
            {7, 6},
            {8, 3},
            {9, 1},
        };

        private const int WinningScore = 21;

        private struct PlayerStatus
        {
            public readonly int Position;
            public readonly int Score;

            public PlayerStatus(int position, int score)
            {
                Position = position;
                Score = score;
            }

            public PlayerStatus Advance(int amount)
            {
                var position = Position + amount;
                if (position > 10)
                {
                    position -= 10;
                }

                return new PlayerStatus(position, Score + position);
            }

            public override string ToString()
            {
                return $"P {Position}, S {Score}";
            }
        }

        private struct GameStatus
        {
            public readonly PlayerStatus PlayerStatus0;
            public readonly PlayerStatus PlayerStatus1;

            public GameStatus(int positions0, int positions1)
            {
                PlayerStatus0 = new PlayerStatus(positions0, 0);
                PlayerStatus1 = new PlayerStatus(positions1, 0);
            }

            private GameStatus(PlayerStatus playerStatus0, PlayerStatus playerStatus1)
            {
                PlayerStatus0 = playerStatus0;
                PlayerStatus1 = playerStatus1;
            }

            public PlayerStatus GetPlayerStatus(int player)
            {
                return player == 0 ? PlayerStatus0 : PlayerStatus1;
            }

            public GameStatus Advance(int player, int amount)
            {
                PlayerStatus p0;
                PlayerStatus p1;
                if (player == 0)
                {
                    p0 = PlayerStatus0.Advance(amount);
                    p1 = PlayerStatus1;
                }
                else
                {
                    p0 = PlayerStatus0;
                    p1 = PlayerStatus1.Advance(amount);
                }

                return new GameStatus(p0, p1);
            }

            public override string ToString()
            {
                return $"{PlayerStatus0} | {PlayerStatus1}";
            }
        }

        public long ExecutePart2(string[] lines)
        {
            Dictionary<int, int> vals = new Dictionary<int, int>();
            for (int i = 1; i <= 3; i++)
            {
                for (int j = 1; j <= 3; j++)
                {
                    for (int k = 1; k <= 3; k++)
                    {
                        int sum = i + j + k;
                        if (vals.ContainsKey(sum))
                        {
                            vals[sum]++;
                        }
                        else
                        {
                            vals.Add(sum, 1);
                        }
                    }

                }
            }


            var positions = lines.Select(x => x.Last() - '0').ToList();

            GameStatus gameStatus = new GameStatus(positions[0], positions[1]);

            var (player0Wins, player1Wins) = CountWinningGames(gameStatus, 1, 0);
            return Math.Max(player0Wins, player1Wins);
        }

        private (long player0Wins, long player1Wins) CountWinningGames(GameStatus gameStatus, long occurenceCount, int currentPlayer)
        {
            long[] playerWins = new long[2];

            foreach (var pair in RollsResult)
            {
                var newGameStatus = gameStatus.Advance(currentPlayer, pair.Key);
                long newOccurrenceCount = occurenceCount * pair.Value;

                if (newGameStatus.GetPlayerStatus(currentPlayer).Score >= WinningScore)
                {
                    playerWins[currentPlayer] += newOccurrenceCount;
                }
                else
                {
                    var newCurrentPlayer = 1 - currentPlayer;
                    var countWinningGames = CountWinningGames(newGameStatus, newOccurrenceCount, newCurrentPlayer);
                    playerWins[0] += countWinningGames.player0Wins;
                    playerWins[1] += countWinningGames.player1Wins;
                }
            }

            return (playerWins[0], playerWins[1]);
        }
    }
}
