using System;

namespace NochkaGame.game
{
    public class Move
    {
        public int CardNum;

        public Move(Tuple<int, int> place, bool isSecondPlayer, int cardNum = -1)
        {
            Place = place;
            IsSecondPlayer = isSecondPlayer;
            CardNum = cardNum;
        }

        public Tuple<int, int> Place { get; set; }
        public bool IsSecondPlayer { get; set; }
    }
}