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
        // Five qualifying cards in tie-break order; an ace-low straight ends with the ace.
        public IReadOnlyList<Card> Cards { get; }

        public HandRank(HandType type, IList<Card> cards)
        {
            if (cards == null) throw new ArgumentNullException(nameof(cards));
            if (cards.Count != 5)
                throw new ArgumentException("Exactly five qualifying cards are required.", nameof(cards));
            Type = type;
            Cards = new List<Card>(cards).AsReadOnly();
        }

        public int CompareTo(HandRank other)
        {
            if (other == null) return 1;
            int comparison = Type.CompareTo(other.Type);
            if (comparison != 0) return comparison;
            for (int i = 0; i < Cards.Count; i++)
            {
                comparison = Cards[i].Rank.CompareTo(other.Cards[i].Rank);
                if (comparison != 0) return comparison;
            }
            return 0;
        }
    }
}
