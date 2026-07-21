using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public enum HandType
    {
        HighCard,
        Pair,
        TwoPair,
        Trips,
        Straight,
        Flush,
        FullHouse,
        Quads,
        StraightFlush
    }

    public class HandRank
    {
        public HandType Type { get; }
        public CardRank HighCard { get; }

        public HandRank(HandType type, CardRank highCard)
        {
            Type = type;
            HighCard = highCard;
        }
    }
}
