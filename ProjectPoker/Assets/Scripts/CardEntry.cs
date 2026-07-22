using Model;
using System;
using UnityEngine;

[Serializable]
public class CardEntry
{
    [SerializeField] private CardSuit suit;
    [SerializeField] private CardRank rank;
    [SerializeField] private Sprite sprite;

    public CardSuit Suit => suit;
    public CardRank Rank => rank;
    public Sprite Sprite => sprite;
}