using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

[CreateAssetMenu(menuName = "Poker/Card Sprite Library")]
public sealed class CardSpriteLibrary : ScriptableObject
{
    [SerializeField] private List<CardSpriteEntry> cardSprites;
    private Dictionary<Card, Sprite> spritesByCard;

    private void OnEnable() => spritesByCard = null;
    private void OnValidate() => spritesByCard = null;

    private void BuildLookup()
    {
        var lookup = new Dictionary<Card, Sprite>();
        foreach (CardSpriteEntry entry in cardSprites)
        {
            if (entry == null || entry.Sprite == null)
                throw new InvalidOperationException("Every card entry needs a sprite.");
            var card = new Card(entry.Suit, entry.Rank);
            if (lookup.ContainsKey(card))
                throw new InvalidOperationException($"Duplicate artwork entry: {card}.");
            lookup.Add(card, entry.Sprite);
        }
        spritesByCard = lookup;
    }

    public Sprite GetSprite(Card card)
    {
        if (spritesByCard == null) BuildLookup();
        if (!spritesByCard.TryGetValue(card, out Sprite sprite))
            throw new InvalidOperationException($"No artwork assigned for {card}.");
        return sprite;
    }
}
