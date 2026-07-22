using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Model
{
    public class Deck
    {
        private readonly System.Random rnd = new();
        private List<Card> Cards;

        public Deck()
        {
            Cards = Create();
        }

        private static List<Card> Create()
        {
            List<Card> cards = new();
            foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)).Cast<CardSuit>().Where(suit => suit != CardSuit.None))
            {
                foreach (CardRank rank in Enum.GetValues(typeof(CardRank)).Cast<CardRank>().Where(rank => rank != CardRank.None))
                {
                    cards.Add(new Card(suit, rank));
                }
            }

            // Debug.Log($"Created a deck with {cards.Count} cards in this order:\n{string.Join("\n", cards)}");
            return cards;
        }

        public void Reset()
        {
            Cards = Create();
        }

        public void Shuffle()
        {
            // Debug.Log($"Card order before shuffling the deck with {cards.Count} cards:\n{string.Join("\n", cards)}");
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                var temp = Cards[i];
                Cards[i] = Cards[j];
                Cards[j] = temp;
            }

            // Debug.Log($"Shuffled the deck with {cards.Count} cards in this order:\n{string.Join("\n", cards)}");
        }

        public Card Draw()
        {
            Card card = Cards[0];
            // Debug.Log($"Drew the top card of the deck: {card}");
            Cards.RemoveAt(0);
            return card;
        }

        public List<Card> GetCards()
        {
            return Cards;
        }
    }
}
