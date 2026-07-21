using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Player
    {
        public string Name { get; }
        public List<Card> HoleCards { get; }
        public HandRank HandRank { get; private set; }

        public Player(string name)
        {
            Name = name;
            HoleCards = new();
        }

        public void AddHoleCard(Card card)
        {
            HoleCards.Add(card);
        }

        public void RemoveHoleCards()
        {
            HoleCards.Clear();
        }

        public void AddHandRank(HandRank handRank)
        {
            HandRank = handRank;
        }
    }
}
