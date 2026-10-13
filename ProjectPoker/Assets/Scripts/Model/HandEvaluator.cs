using System;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    public static class HandEvaluator
    {
        public static HandRank EvaluateHoldemHand(IReadOnlyList<Card> holeCards, IReadOnlyList<Card> board)
        {
            if (holeCards == null) throw new ArgumentNullException(nameof(holeCards));
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (holeCards.Count != 2) throw new ArgumentException("Texas Hold'em requires two hole cards.", nameof(holeCards));
            if (board.Count != 5) throw new ArgumentException("The board must contain five cards.", nameof(board));

            var seen = new HashSet<Card>();

            foreach (Card card in holeCards
                .Concat(board))
                if (card.Rank < CardRank.Two || card.Rank > CardRank.Ace ||
                    card.Suit < CardSuit.Clubs || card.Suit > CardSuit.Hearts || !seen.Add(card))
                    throw new ArgumentException("Cards must be valid and unique.");

            var combinedCards = new List<Card>(holeCards
                .Concat(board));
            return EvaluateHand(combinedCards);
        }

        public static HandRank EvaluateOmahaHand(IReadOnlyList<Card> holeCards, IReadOnlyList<Card> board)
        {
            if (holeCards == null) throw new ArgumentNullException(nameof(holeCards));
            if (board == null) throw new ArgumentNullException(nameof(board));
            if (holeCards.Count != 4) throw new ArgumentException("Omaha requires four hole cards.", nameof(holeCards));
            if (board.Count != 5) throw new ArgumentException("The board must contain five cards.", nameof(board));

            var seen = new HashSet<Card>();

            foreach (Card card in holeCards
                .Concat(board))
                if (card.Rank < CardRank.Two || card.Rank > CardRank.Ace ||
                    card.Suit < CardSuit.Clubs || card.Suit > CardSuit.Hearts || !seen.Add(card))
                    throw new ArgumentException("Cards must be valid and unique.");

            HandRank best = null;

            for (int h1 = 0; h1 < holeCards.Count - 1; h1++)
                for (int h2 = h1 + 1; h2 < holeCards.Count; h2++)
                    for (int b1 = 0; b1 < board.Count - 2; b1++)
                        for (int b2 = b1 + 1; b2 < board.Count - 1; b2++)
                            for (int b3 = b2 + 1; b3 < board.Count; b3++)
                            {
                                HandRank candidate = EvaluateHand(new List<Card>
                                {
                                    holeCards[h1], holeCards[h2], board[b1], board[b2], board[b3]
                                });

                                if (best == null || candidate.CompareTo(best) > 0) best = candidate;
                            }

            return best;
        }

        private static HandRank EvaluateHand(List<Card> cards)
        {
            cards = cards
                .OrderByDescending(card => card.Rank)
                .ToList();

            List<Card> qualifyingCards;

            qualifyingCards = FindStraightFlush(cards);
            if (qualifyingCards != null) return new HandRank(HandType.StraightFlush, qualifyingCards);

            qualifyingCards = FindQuads(cards);
            if (qualifyingCards != null) return new HandRank(HandType.Quads, qualifyingCards);

            qualifyingCards = FindFullHouse(cards);
            if (qualifyingCards != null) return new HandRank(HandType.FullHouse, qualifyingCards);

            qualifyingCards = FindFlush(cards);
            if (qualifyingCards != null) return new HandRank(HandType.Flush, qualifyingCards);

            qualifyingCards = FindStraight(cards);
            if (qualifyingCards != null) return new HandRank(HandType.Straight, qualifyingCards);

            qualifyingCards = FindTrips(cards);
            if (qualifyingCards != null) return new HandRank(HandType.Trips, qualifyingCards);

            qualifyingCards = FindTwoPair(cards);
            if (qualifyingCards != null) return new HandRank(HandType.TwoPair, qualifyingCards);

            qualifyingCards = FindPair(cards);
            if (qualifyingCards != null) return new HandRank(HandType.Pair, qualifyingCards);

            return new HandRank(HandType.HighCard, cards
                .Take(5)
                .ToList());
        }

        // Input is sorted by descending rank. Return five cards in tie-break order, or null.
        private static List<Card> FindStraightFlush(List<Card> cards)
        {
            foreach (var suit in cards
                .GroupBy(card => card.Suit))
            {
                if (suit.Count() < 5) continue;

                List<Card> straight = FindStraight(suit.ToList());
                if (straight != null) return straight;
            }

            return null;
        }

        private static List<Card> FindQuads(List<Card> cards)
        {
            var quads = cards
                .GroupBy(card => card.Rank)
                .FirstOrDefault(group => group.Count() == 4);
            if (quads == null) return null;

            return quads
                .Concat(cards
                    .Where(card => card.Rank != quads.Key)
                    .Take(1))
                .ToList();
        }

        private static List<Card> FindFullHouse(List<Card> cards)
        {
            var groups = cards
                .GroupBy(card => card.Rank)
                .ToList();
            var trips = groups
                .FirstOrDefault(group => group.Count() >= 3);
            if (trips == null) return null;

            var pair = groups
                .FirstOrDefault(group => group.Key != trips.Key && group.Count() >= 2);
            if (pair == null) return null;

            return trips
                .Take(3)
                .Concat(pair
                    .Take(2))
                .ToList();
        }

        private static List<Card> FindFlush(List<Card> cards)
        {
            var suit = cards
                .GroupBy(card => card.Suit)
                .FirstOrDefault(group => group.Count() >= 5);

            return suit?
                .Take(5)
                .ToList();
        }

        private static List<Card> FindStraight(List<Card> cards)
        {
            // Keep one actual card per rank so duplicate ranks cannot interrupt a straight.
            List<Card> distinct = cards
                .GroupBy(card => card.Rank)
                .Select(group => group.First())
                .ToList();
            var chain = new List<Card>();

            foreach (Card card in distinct)
            {
                if (chain.Count > 0 && chain[chain.Count - 1].Rank != card.Rank + 1) chain.Clear();
                chain.Add(card);
                if (chain.Count == 5) return chain;
            }

            // An ace-low straight places its ace last for rank comparison.
            if (chain.Count == 4 && chain[0].Rank == CardRank.Five &&
                distinct[0].Rank == CardRank.Ace)
            {
                chain.Add(distinct[0]);
                return chain;
            }

            return null;
        }

        private static List<Card> FindTrips(List<Card> cards)
        {
            var trips = cards
                .GroupBy(card => card.Rank)
                .FirstOrDefault(group => group.Count() >= 3);
            if (trips == null) return null;

            return trips
                .Take(3)
                .Concat(cards
                    .Where(card => card.Rank != trips.Key)
                    .Take(2))
                .ToList();
        }

        private static List<Card> FindTwoPair(List<Card> cards)
        {
            var pairs = cards
                .GroupBy(card => card.Rank)
                .Where(group => group.Count() >= 2)
                .Take(2)
                .ToList();
            if (pairs.Count < 2) return null;

            return pairs[0]
                .Take(2)
                .Concat(pairs[1]
                    .Take(2))
                .Concat(cards
                    .Where(card => card.Rank != pairs[0].Key && card.Rank != pairs[1].Key)
                    .Take(1))
                .ToList();
        }

        private static List<Card> FindPair(List<Card> cards)
        {
            var pair = cards
                .GroupBy(card => card.Rank)
                .FirstOrDefault(group => group.Count() >= 2);
            if (pair == null) return null;

            return pair
                .Take(2)
                .Concat(cards
                    .Where(card => card.Rank != pair.Key)
                    .Take(3))
                .ToList();
        }

        public static PlayerHand DetermineWinner(IReadOnlyList<PlayerHand> hands)
        {
            List<PlayerHand> winners = DetermineWinners(hands);
            return winners.Count == 1 ? winners[0] : null;
        }

        // All tied winners must share the pot. The single-winner API returns null for ties.
        public static List<PlayerHand> DetermineWinners(IReadOnlyList<PlayerHand> hands)
        {
            if (hands == null) throw new ArgumentNullException(nameof(hands));
            var winners = new List<PlayerHand>();

            foreach (PlayerHand hand in hands)
            {
                if (hand == null) throw new ArgumentException("Every entry must contain an evaluated hand.", nameof(hands));

                int comparison = winners.Count == 0 ? 1 : hand.Rank.CompareTo(winners[0].Rank);
                if (comparison > 0) winners.Clear();
                if (comparison >= 0) winners.Add(hand);
            }

            return winners;
        }
    }
}
