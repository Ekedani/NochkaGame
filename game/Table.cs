using System.Collections.Generic;
using NochkaGame.game.card;

namespace NochkaGame.game
{
    public class Table
    {
        public PlayingCard[,] Cards { get; set; }
        public bool[,] AvailableMoves { get; set; }

        public Table()
        {
            Cards = new PlayingCard[4, 9];
            AvailableMoves = new bool[4, 9];
        }

        private Table(Table previousTable)
        {
            Cards = previousTable.Cards.Clone() as PlayingCard[,];
            AvailableMoves = previousTable.AvailableMoves.Clone() as bool[,];
        }

        public static void MakeAvailable(Move move, Table table)
        {
            var lowerI = move.Place.Item1 > 0 ? move.Place.Item1 - 1 : 0;
            var lowerJ = move.Place.Item2 > 0 ? move.Place.Item2 - 1 : 0;
            var higherI = move.Place.Item1 + 1;
            var higherJ = move.Place.Item2 + 1;
            for (var i = lowerI; i <= higherI && i < table.Cards.GetLength(0); i++)
            {
                for (var j = lowerJ; j <= higherJ && j < table.Cards.GetLength(1); j++)
                {
                    if (table.Cards[i, j] == null || !table.Cards[i, j].IsVisible)
                    {
                        table.AvailableMoves[i, j] = true;
                    }
                }
            }
        }

        public Table AssumeMove(Move move, List<PlayingCard> playerHand)
        {
            var newTable = new Table(this);
            MakeAvailable(move, newTable);
            if (move.CardNum != -1)
            {
                if (newTable.Cards[move.Place.Item1, move.Place.Item2] != null)
                {
                    var tmp = playerHand[move.CardNum];
                    playerHand[move.CardNum] = newTable.Cards[move.Place.Item1, move.Place.Item2];
                    newTable.Cards[move.Place.Item1, move.Place.Item2] = tmp;
                }
                else
                {
                    newTable.Cards[move.Place.Item1, move.Place.Item2] = playerHand[move.CardNum];
                    playerHand.RemoveAt(move.CardNum);
                }

                newTable.AvailableMoves[move.Place.Item1, move.Place.Item2] = false;
            }
            else
            {
                playerHand.Add(newTable.Cards[move.Place.Item1, move.Place.Item2].Clone());
                newTable.Cards[move.Place.Item1, move.Place.Item2] = null;
            }

            return newTable;
        }
    }
}