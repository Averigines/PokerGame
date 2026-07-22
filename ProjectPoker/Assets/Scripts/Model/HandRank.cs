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
        public CardRank[] HighCards { get; } // All 5 qualifying cards from most relevant to least relevant (e.g. 2 pair: Higher pair --> lower pair --> kicker)

        public HandRank(HandType type, CardRank[] highCards)
        {
            Type = type;
            HighCards = highCards;
        }
    }
}
