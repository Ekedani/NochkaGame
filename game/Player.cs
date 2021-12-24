using System;
using System.Collections.Generic;
using NochkaGame.game.card;

namespace NochkaGame.game
{
    public class Player
    {
        public readonly List<PlayingCard> PlayerHand;
        private readonly bool _isSecond;

        public Player(bool isSecond)
        {
            _isSecond = isSecond;
            PlayerHand = new List<PlayingCard>();
        }

        public Move AssumeMoves(Table startTable)
        {
            var possibleMoves = new List<Move>();
            for (var i = 0; i < PlayerHand.Count; i++)
                if (startTable.AvailableMoves[(int) PlayerHand[i].Suit, PlayerHand[i].Value - 6])
                {
                    var place = new Tuple<int, int>((int) PlayerHand[i].Suit, PlayerHand[i].Value - 6);
                    possibleMoves.Add(new Move(place, _isSecond, i));
                }

            if (possibleMoves.Count == 0)
                for (var i = 0; i < startTable.Cards.GetLength(0); i++)
                for (var j = 0; j < startTable.Cards.GetLength(1); j++)
                    if (startTable.AvailableMoves[i, j] && startTable.Cards[i, j] != null)
                    {
                        var place = new Tuple<int, int>(i, j);
                        possibleMoves.Add(new Move(place, _isSecond));
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
                if (newTable.AvailableMoves[(int) playingCard.Suit, playingCard.Value - 6])
                    profitableCards++;

            if (profitableCards == 0) return 100 * newHand.Count;
            var profitablePercent = (float) profitableCards / newHand.Count;
            var evaluation = newHand.Count / profitablePercent;
            return evaluation;
        }

        public bool HasCard(PlayingCard card)
        {
            var result = false;
            foreach (var playingCard in PlayerHand)
                if (playingCard.Suit == card.Suit && playingCard.Value == card.Value)
                {
                    result = true;
                    break;
                }

            return result;
        }

        public bool HasMoves(Table gameTable)
        {
            var result = false;
            foreach (var playingCard in PlayerHand)
                if (gameTable.AvailableMoves[(int) playingCard.Suit, playingCard.Value - 6])
                    result = true;

            return result;
        }

        public bool KnowOpponentCards(Table gameTable)
        {
            var result = true;
            for (var i = 0; i < gameTable.Cards.GetLength(0); i++)
            {
                for (var j = 0; j < gameTable.Cards.GetLength(1); j++)
                {
                    result &= gameTable.Cards[i, j] == null || gameTable.Cards[i, j].IsVisible;
                    if (!result) break;
                }
            }

            return result;
        }
        
        public Player Clone()
        {
            var newPlayer = new Player(_isSecond);
            foreach (var playingCard in PlayerHand)
            {
                newPlayer.PlayerHand.Add(playingCard.Clone());
            }
            return newPlayer;
        }
    }
}