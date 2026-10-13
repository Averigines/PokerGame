using System;
using System.Collections.Generic;

namespace Model
{
    public sealed class PokerRound
    {
        private readonly Deck deck;

        public PokerRound(Deck deck = null)
        {
            this.deck = deck ?? new Deck();
        }

        public RoundResult Play(Table table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            deck.Reset();
            deck.Shuffle();
            var holeCards = new Dictionary<Player, List<Card>>();
            foreach (Player player in table.Players.Keys)
                holeCards.Add(player, new List<Card>());

            // Deal one card to every player on each pass.
            for (int card = 0; card < table.HoleCardCount; card++)
                foreach (List<Card> hand in holeCards.Values)
                    hand.Add(deck.Draw());

            var board = new List<Card>();
            deck.Draw(); // Burn before the flop.
            for (int card = 0; card < 3; card++) board.Add(deck.Draw());
            deck.Draw(); // Burn before the turn.
            board.Add(deck.Draw());
            deck.Draw(); // Burn before the river.
            board.Add(deck.Draw());

            var hands = new List<PlayerHand>();
            foreach (var entry in holeCards)
            {
                HandRank rank;
                if (table.Game == PokerGame.PotLimitOmaha)
                    rank = HandEvaluator.EvaluateOmahaHand(entry.Value, board);
                else
                {
                    rank = HandEvaluator.EvaluateHoldemHand(entry.Value, board);
                }
                hands.Add(new PlayerHand(entry.Key, entry.Value, rank));
            }
            return new RoundResult(board, hands);
        }
    }
}
