using Model;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CardLibrary : MonoBehaviour
{

    [SerializeField] private List<CardEntry> cardSprites;
    private Dictionary<Card, Sprite> spritesToCards;

    public void PopulateCardSprites(List<Card> cards)
    {
        spritesToCards = new();
        foreach (Card card in cards)
        {
            spritesToCards.Add(card, cardSprites.Find(entry => entry.Suit == card.Suit && entry.Rank == card.Rank).Sprite);
        }
    }

    public Sprite GetSprite(Card card)
    {
        return spritesToCards[card];
    }
}
