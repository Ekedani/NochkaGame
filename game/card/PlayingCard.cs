using System.Transactions;

namespace NochkaGame.game.card
{
    public class PlayingCard
    {
        public int Value { get; }
        public CardSuit Suit { get; }

        public CardColour Colour => ((int) Suit) < 2 ? CardColour.Red : CardColour.Black;

        public bool IsVisible;

        public PlayingCard(int value, CardSuit suit, bool isVisible=false)
        {
            Value = value;
            Suit = suit;
            IsVisible = isVisible;
        }

        public PlayingCard Clone()
        {
            return new PlayingCard(this.Value, this.Suit, this.IsVisible);
        }
        public override string ToString()
        {
            string card = "";
            if (Value <= 10)
            {
                card += Value.ToString();
            }
            else
            {
                switch (Value)
                {
                    case(11):
                        card += "J";
                        break;
                    case(12):
                        card += "Q";
                        break;
                    case(13):
                        card += "K";
                        break;
                    case(14):
                        card += "A";
                        break;
                }
            }
            switch (Suit)
            {
                case(CardSuit.Clubs):
                    card += '♣';
                    break;
                case(CardSuit.Diamonds):
                    card += '♦';
                    break;
                case (CardSuit.Hearts):
                    card += '♥';
                    break;
                case (CardSuit.Spades):
                    card += '♠';
                    break;
            }
            return card;
        }
    }
}