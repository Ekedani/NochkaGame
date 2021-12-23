using System;
using System.Collections.Generic;
using NochkaGame.game.card;

namespace NochkaGame.game
{
    public class Player
    {
        public readonly List<PlayingCard> PlayerHand;
        private bool _isSecond;

        public Player(bool isSecond)
        {
            this._isSecond = isSecond;
            PlayerHand = new List<PlayingCard>();
        }

        public Move AssumeMoves(Table startTable)
        {
            var possibleMoves = new List<Move>();
            for (int i = 0; i < PlayerHand.Count; i++)
            {
                if (startTable.AvailableMoves[(int) PlayerHand[i].Suit, PlayerHand[i].Value - 6])
                {
                    var place = new Tuple<int, int>((int) PlayerHand[i].Suit, PlayerHand[i].Value - 6);
                    possibleMoves.Add(new Move(place, _isSecond, i));
                }
            }

            if (possibleMoves.Count == 0)
            {
                for (int i = 0; i < startTable.Cards.GetLength(0); i++)
                {
                    for (int j = 0; j < startTable.Cards.GetLength(1); j++)
                    {
                        if (startTable.AvailableMoves[i, j] && startTable.Cards[i, j] != null)
                        {
                            var place = new Tuple<int, int>(i, j);
                            possibleMoves.Add(new Move(place, _isSecond));
                        }
                    }
                }
            }

            var bestMove = possibleMoves[0];
            var minEvaluation = float.MaxValue;
            foreach (var possibleMove in possibleMoves)
            {
                var curEvaluation = EvaluateMove(possibleMove, startTable);
                if (curEvaluation < minEvaluation)
                {
                    bestMove = possibleMove;
                    minEvaluation = curEvaluation;
                }
            }
            return bestMove;
        }

        public float EvaluateMove(Move move, Table startTable)
        {
            var newHand = new List<PlayingCard>(PlayerHand);
            var newTable = startTable.AssumeMove(move, newHand);
            var profitableCards = 0;
            foreach (var playingCard in newHand)
            {
                if (newTable.AvailableMoves[(int) playingCard.Suit, playingCard.Value - 6])
                {
                    profitableCards++;
                }
            }

            if (profitableCards == 0) return 100 * newHand.Count;
            var profitablePercent = (float)profitableCards / newHand.Count;
            var evaluation = newHand.Count / profitablePercent;
            return evaluation;
        }
    }
}