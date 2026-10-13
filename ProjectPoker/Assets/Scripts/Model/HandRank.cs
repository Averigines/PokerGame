using System;
using System.Collections.Generic;


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

    public class HandRank : IComparable<HandRank>
    {
        public HandType Type { get; }
        public IReadOnlyList<CardRank> HighCards { get; } // All 5 qualifying cards from most relevant to least relevant (e.g. 2 pair: Higher pair --> lower pair --> kicker)

        public HandRank(HandType type, CardRank[] highCards)
        {
            if (highCards == null) throw new ArgumentNullException(nameof(highCards));
            if (highCards.Length != 5)
                throw new ArgumentException("Exactly five qualifying ranks are required.", nameof(highCards));
            Type = type;
            HighCards = Array.AsReadOnly((CardRank[])highCards.Clone());
        }

        public int CompareTo(HandRank other)
        {
            if (other == null) return 1;
            int comparison = Type.CompareTo(other.Type);
            if (comparison != 0) return comparison;
            for (int i = 0; i < HighCards.Count; i++)
            {
                comparison = HighCards[i].CompareTo(other.HighCards[i]);
                if (comparison != 0) return comparison;
            }
            return 0;
        }
    }
}
