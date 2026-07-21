using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model
{
    public class Deck
    {
        private readonly System.Random rnd = new();
        private List<Card> cards;

        public Deck()
        {
            cards = Create();
        }

        private static List<Card> Create()
        {
            List<Card> cards = new();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)))
                {
                    cards.Add(new Card(suit, rank));
                }
            }

            Debug.Log($"Created a deck with {cards.Count} cards in this order:\n{string.Join("\n", cards)}");
            return cards;
        }

        public void Shuffle()
        {
            Debug.Log($"Card order before shuffling the deck with {cards.Count} cards:\n{string.Join("\n", cards)}");
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                var temp = cards[i];
                cards[i] = cards[j];
                cards[j] = temp;
            }

            Debug.Log($"Shuffled the deck with {cards.Count} cards in this order:\n{string.Join("\n", cards)}");
        }

        public Card Draw()
        {
            Card card = cards[0];
            Debug.Log($"Drew the top card of the deck: {card}");
            cards.RemoveAt(0);
            return card;
        }
    }
}
