using System;
using System.Collections.Generic;

namespace Model
{
    public sealed class Deck
    {
        private readonly Random random;
        private readonly List<Card> cards = new List<Card>();
        public IReadOnlyList<Card> Cards => cards.AsReadOnly();

        public Deck(Random random = null)
        {
            this.random = random ?? new Random();
            Reset();
        }

        public void Reset()
        {
            cards.Clear();
            for (CardSuit suit = CardSuit.Clubs; suit <= CardSuit.Hearts; suit++)
                for (CardRank rank = CardRank.Two; rank <= CardRank.Ace; rank++)
                    cards.Add(new Card(suit, rank));
        }

        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                Card temporary = cards[i];
                cards[i] = cards[j];
                cards[j] = temporary;
            }
        }

        public Card Draw()
        {
            if (cards.Count == 0) throw new InvalidOperationException("The deck is empty.");
            int top = cards.Count - 1;
            Card card = cards[top];
            cards.RemoveAt(top);
            return card;
        }
    }
}
