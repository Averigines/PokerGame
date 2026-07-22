using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class HandEvaluator
{
    public static HandRank EvaluateHand(List<Card> cards)
    {
        CardRank[] highCards = new CardRank[5];

        cards.Sort((a, b) => b.Rank.CompareTo(a.Rank));
        // Debug.Log($"Sorted Cards: {String.Join(", ", cards)}");

        if (HasStraightFlush(cards, highCards))
        {
            return new HandRank(HandType.StraightFlush, highCards);
        }
        if (HasQuads(cards, highCards))
        {
            return new HandRank(HandType.Quads, highCards);
        }
        if (HasFullHouse(cards, highCards))
        {
            return new HandRank(HandType.FullHouse, highCards);
        }
        if (HasFlush(cards, highCards))
        {
            return new HandRank(HandType.Flush, highCards);
        }
        if (HasStraight(cards, highCards))
        {
            return new HandRank(HandType.Straight, highCards);
        }
        if (HasTrips(cards, highCards))
        {
            return new HandRank(HandType.Trips, highCards);
        }
        if (HasTwoPair(cards, highCards))
        {
            return new HandRank(HandType.TwoPair, highCards);
        }
        if (HasPair(cards, highCards))
        {
            return new HandRank(HandType.Pair, highCards);
        }
        highCards[0] = cards[0].Rank;
        highCards[1] = cards[1].Rank;
        highCards[2] = cards[2].Rank;
        highCards[3] = cards[3].Rank;
        highCards[4] = cards[4].Rank;
        return new HandRank(HandType.HighCard, highCards);
    }

    public static bool HasStraightFlush(List<Card> cards, CardRank[] highCards)
    {
        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            CardRank lowestInChain = CardRank.None;

            List <CardRank> ranks = cards
                .Where(card => card.Suit == suit)
                .Select(card => card.Rank)
                .OrderByDescending(rank => rank)
                .ToList();

            if (ranks.Count < 5) continue;

            int chain = 1;

            for (int i = 1; i < ranks.Count; i++)
            {
                if (ranks[i] + 1 == ranks[i - 1])
                {
                    chain++;

                    if (chain == 5)
                    {
                        lowestInChain = ranks[i];
                        highCards[0] = lowestInChain + 4;
                        highCards[1] = lowestInChain + 3;
                        highCards[2] = lowestInChain + 2;
                        highCards[3] = lowestInChain + 1;
                        highCards[4] = lowestInChain;
                        return true;
                    }
                }
                else chain = 1;
            }

            if (chain == 4 && ranks.Contains(CardRank.Ace) && ranks.Contains(CardRank.Two))
            {
                highCards[0] = CardRank.Five;
                highCards[1] = CardRank.Four;
                highCards[2] = CardRank.Three;
                highCards[3] = CardRank.Two;
                highCards[4] = CardRank.Ace;
                return true;
            }
        }
        return false;
    }

    public static bool HasQuads(List<Card> cards, CardRank[] highCards)
    {
        int sameRank = 1;

        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank == cards[i - 1].Rank)
            {
                sameRank++;
                if (sameRank == 4)
                {
                    CardRank quadRank = cards[i].Rank;
                    CardRank kicker = cards
                        .First(card => card.Rank != quadRank)
                        .Rank;

                    highCards[0] = quadRank;
                    highCards[1] = quadRank;
                    highCards[2] = quadRank;
                    highCards[3] = quadRank;
                    highCards[4] = kicker;
                    return true;
                }
            }
            else sameRank = 1;
        }
        return false;
    }

    public static bool HasFullHouse(List<Card> cards, CardRank[] highCards)
    {
        int sameRank = 1;

        CardRank tripsRank = CardRank.None;

        // Find highest trips
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank == cards[i - 1].Rank)
            {
                sameRank++;
                if (sameRank == 3)
                {
                    tripsRank = cards[i].Rank;
                    break;
                }
            }
            else sameRank = 1;
        }

        // If no trips are present, full house is impossible, so return false immediately.
        if (tripsRank == CardRank.None) return false;

        // Find highest pair with different rank than prev. found trips.
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank == cards[i - 1].Rank && cards[i].Rank != tripsRank)
            {
                CardRank pairRank = cards[i].Rank;
                highCards[0] = tripsRank;
                highCards[1] = tripsRank;
                highCards[2] = tripsRank;
                highCards[3] = pairRank;
                highCards[4] = pairRank;
                return true;
            }
        }
        return false;
    }

    public static bool HasFlush(List<Card> cards, CardRank[] highCards)
    {
        foreach (CardSuit suit in Enum.GetValues(typeof(CardSuit)))
        {
            List<CardRank> ranks = cards
                .Where(card => card.Suit == suit)
                .Select(card => card.Rank)
                .OrderByDescending(rank => rank)
                .ToList();

            if (ranks.Count < 5) continue;

            highCards[0] = ranks[0];
            highCards[1] = ranks[1];
            highCards[2] = ranks[2];
            highCards[3] = ranks[3];
            highCards[4] = ranks[4];
            return true;
        }
        return false;
    }

    public static bool HasStraight(List<Card> cards, CardRank[] highCards)
    {
        int chain = 1;

        // Check if we have five continous cards where the cardrank is 1 lower than the previous card
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank + 1 == cards[i - 1].Rank)
            {
                chain++;
                if (chain == 5)
                {
                    CardRank lowestInChain = cards[i].Rank;
                    highCards[0] = lowestInChain + 4;
                    highCards[1] = lowestInChain + 3;
                    highCards[2] = lowestInChain + 2;
                    highCards[3] = lowestInChain + 1;
                    highCards[4] = lowestInChain;
                    return true;
                }
            }
            else if (cards[i].Rank != cards[i - 1].Rank) chain = 1;
        }

        // If we now have a chain of 4, we need to consider the low ace since ace only counts as high card in the cardrank enum.
        if (chain == 4 && cards[0].Rank == CardRank.Ace && cards[^1].Rank == CardRank.Two)
        {
            // In this case, we know the straight is 5-4-3-2-A, so we can hardcode the cardranks
            highCards[0] = CardRank.Five;
            highCards[1] = CardRank.Four;
            highCards[2] = CardRank.Three;
            highCards[3] = CardRank.Two;
            highCards[4] = CardRank.Ace;
            return true;
        }
        return false;
    }

    public static bool HasTrips(List<Card> cards, CardRank[] highCards)
    {
        int sameRank = 1;

        // Find highest trips
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank == cards[i - 1].Rank)
            {
                sameRank++;
                if (sameRank == 3)
                {
                    CardRank tripsRank = cards[i].Rank;
                    CardRank kicker1 = cards
                        .First(card => card.Rank != tripsRank)
                        .Rank;
                    CardRank kicker2 = cards
                        .Where(card => card.Rank != tripsRank)
                        .Skip(1)
                        .First()
                        .Rank;
                    highCards[0] = tripsRank;
                    highCards[1] = tripsRank;
                    highCards[2] = tripsRank;
                    highCards[3] = kicker1;
                    highCards[4] = kicker2;
                    return true;
                }
            }
            else sameRank = 1;
        }
        return false;
    }

    public static bool HasTwoPair(List<Card> cards, CardRank[] highCards)
    {
        CardRank firstPairRank = CardRank.None;

        // Find highest pair
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank == cards[i - 1].Rank)
            {
                firstPairRank = cards[i].Rank;
                break;
            }
        }

        // If no pair is present, two pair is impossible, so return false immediately.
        if (firstPairRank == CardRank.None) return false;

        // Find highest pair with different rank than prev. found pair.
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank == cards[i - 1].Rank && cards[i].Rank != firstPairRank)
            {
                CardRank secondPairRank = cards[i].Rank;
                CardRank kicker = cards
                    .First(card => card.Rank != firstPairRank && card.Rank != secondPairRank)
                    .Rank;
                highCards[0] = firstPairRank;
                highCards[1] = firstPairRank;
                highCards[2] = secondPairRank;
                highCards[3] = secondPairRank;
                highCards[4] = kicker;
                return true;
            }
        }
        return false;
    }

    public static bool HasPair(List<Card> cards, CardRank[] highCards)
    {
        // Find highest pair
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank == cards[i - 1].Rank)
            {
                CardRank pairRank = cards[i].Rank;
                CardRank kicker1 = cards
                    .First(card => card.Rank != pairRank)
                    .Rank;
                CardRank kicker2 = cards
                    .Where(card => card.Rank != pairRank)
                    .Skip(1)
                    .First()
                    .Rank;
                CardRank kicker3 = cards
                    .Where(card => card.Rank != pairRank)
                    .Skip(2)
                    .First()
                    .Rank;
                highCards[0] = pairRank;
                highCards[1] = pairRank;
                highCards[2] = kicker1;
                highCards[3] = kicker2;
                highCards[4] = kicker3;
                return true;
            }
        }
        return false;
    }

    public static Player DetermineWinner(List<Player> players)
    {
        return null;
    }
}