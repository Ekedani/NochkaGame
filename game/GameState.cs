using System;
using System.Linq;

namespace NochkaGame.game
{
    public class GameState
    {
        private bool _isFirstMove = true;

        public GameState()
        {
            FirstPlayer = new Player(false);
            SecondPlayer = new Player(true);
            GameTable = new Table();
            var deck = new CardDeck();
            for (var i = 0; i < 4; i++)
            for (var j = 1; j < 8; j++)
                GameTable.Cards[i, j] = deck.TakeCard();

            deck.MakeCardsVisible();
            for (var i = 0; i < 8; i++)
                if (i % 2 == 0)
                    FirstPlayer.PlayerHand.Add(deck.TakeCard());
                else
                    SecondPlayer.PlayerHand.Add(deck.TakeCard());

            for (var i = 0; i < 4; i++)
            for (var j = 0; j < 9; j++)
                GameTable.AvailableMoves[i, j] = true;
        }

        public Player FirstPlayer { get; set; }
        public Player SecondPlayer { get; set; }
        public Table GameTable { get; set; }

        public void MakeMove(Move move)
        {
            if (_isFirstMove)
            {
                GameTable.AvailableMoves = new bool[4, 9];
                _isFirstMove = false;
            }

            var player = move.IsSecondPlayer ? SecondPlayer : FirstPlayer;
            Table.MakeAvailable(move, GameTable);
            if (move.CardNum != -1)
            {
                if (GameTable.Cards[move.Place.Item1, move.Place.Item2] != null)
                {
                    var tmp = player.PlayerHand[move.CardNum];
                    player.PlayerHand[move.CardNum] = GameTable.Cards[move.Place.Item1, move.Place.Item2];
                    player.PlayerHand[move.CardNum].IsVisible = true;
                    GameTable.Cards[move.Place.Item1, move.Place.Item2] = tmp;
                }
                else
                {
                    GameTable.Cards[move.Place.Item1, move.Place.Item2] = player.PlayerHand[move.CardNum];
                    player.PlayerHand.RemoveAt(move.CardNum);
                }

                GameTable.AvailableMoves[move.Place.Item1, move.Place.Item2] = false;
            }
            else
            {
                player.PlayerHand.Add(GameTable.Cards[move.Place.Item1, move.Place.Item2].Clone());
                player.PlayerHand.Last().IsVisible = true;
                GameTable.Cards[move.Place.Item1, move.Place.Item2] = null;
            }
        }

        public bool IsTerminal()
        {
            return FirstPlayer.PlayerHand.Count == 0 || SecondPlayer.PlayerHand.Count == 0;
        }

        public GameState Clone()
        {
            var newState = new GameState
            {
                FirstPlayer = FirstPlayer.Clone(),
                SecondPlayer = SecondPlayer.Clone(),
                _isFirstMove = _isFirstMove,
                GameTable = GameTable.Clone()
            };
            return newState;
        }
    }
}