using Model;
using System;
using System.Collections.Generic;
using UnityEngine;
public static class HandEvaluator
{
    public static HandRank EvaluateHand(List<Card> cards)
    {
        CardRank highCard;
        cards.Sort((a, b) => b.Rank.CompareTo(a.Rank));
        Debug.Log($"Sorted Cards: {String.Join(", ", cards)}");

        if (HasStraightFlush(cards, out highCard))
        {
            return new HandRank(HandType.StraightFlush, highCard);
        }
        if (HasQuads(cards, out highCard))
        {
            return new HandRank(HandType.Quads, highCard);
        }
        if (HasFullHouse(cards, out highCard))
        {
            return new HandRank(HandType.FullHouse, highCard);
        }
        if (HasFlush(cards, out highCard))
        {
            return new HandRank(HandType.Flush, highCard);
        }
        if (HasStraight(cards, out highCard))
        {
            return new HandRank(HandType.Straight, highCard);
        }
        if (HasTrips(cards, out highCard))
        {
            return new HandRank(HandType.Trips, highCard);
        }
        if (HasTwoPair(cards, out highCard))
        {
            return new HandRank(HandType.TwoPair, highCard);
        }
        if (HasPair(cards, out highCard))
        {
            return new HandRank(HandType.Pair, highCard);
        }
        return new HandRank(HandType.HighCard, cards[0].Rank);
    }

    public static bool HasStraightFlush(List<Card> cards, out CardRank highCard)
    {
        highCard = default;
        return false;
    }

    public static bool HasQuads(List<Card> cards, out CardRank highCard)
    {
        int sameRank = 1;
        for (int i = 1; i < cards.Count; i++)
        {
            if (cards[i].Rank == cards[i - 1].Rank)
            {
                sameRank++;
                if (sameRank == 4)
                {
                    highCard = cards[i].Rank;
                    return true;
                }
            }
            else sameRank = 1;
        }
        highCard = default;
        return false;
    }

    public static bool HasFullHouse(List<Card> cards, out CardRank highCard)
    {
        highCard = default;
        return false;
    }

    public static bool HasFlush(List<Card> cards, out CardRank highCard)
    {
        highCard = default;
        return false;
    }

    public static bool HasStraight(List<Card> cards, out CardRank highCard)
    {
        highCard = default;
        return false;
    }

    public static bool HasTrips(List<Card> cards, out CardRank highCard)
    {
        highCard = default;
        return false;
    }

    public static bool HasTwoPair(List<Card> cards, out CardRank highCard)
    {
        highCard = default;
        return false;
    }

    public static bool HasPair(List<Card> cards, out CardRank highCard)
    {
        highCard = default;
        return false;
    }
}