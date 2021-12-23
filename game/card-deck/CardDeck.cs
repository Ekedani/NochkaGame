using System;
using System.Collections.Generic;
using NochkaGame.game.card;

namespace NochkaGame.game
{
    public class CardDeck
    {
        private readonly List<PlayingCard> _deck;

        public CardDeck(DeckType type = DeckType.Medium)
        {
            _deck = new List<PlayingCard>();
            if (type == DeckType.Medium) GenerateMiddleDeck();

            ShuffleDeck();
        }

        public PlayingCard TakeCard()
        {
            var card = _deck[^1];
            _deck.RemoveAt(_deck.Count - 1);
            return card;
        }

        private void GenerateMiddleDeck()
        {
            for (var i = 0; i < 4; i++)
            for (var j = 6; j <= 14; j++)
                _deck.Add(new PlayingCard(j, (CardSuit) i));
        }

        public void MakeCardsVisible()
        {
            foreach (var playingCard in _deck) playingCard.IsVisible = true;
        }

        private void ShuffleDeck()
        {
            var rng = new Random();
            var n = _deck.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                (_deck[k], _deck[n]) = (_deck[n], _deck[k]);
            }
        }
    }
}