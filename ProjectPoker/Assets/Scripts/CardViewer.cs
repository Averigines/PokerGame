using Assets.Scripts.Model;
using Model;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardViewer : MonoBehaviour
{
    private List<GameObject> cards;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private GameObject playerHandPrefab;
    [SerializeField] private CardLibrary cardLibrary;

    [Header("Table Layout")]
    private float horizontalRadius;
    private float verticalRadius;
    [SerializeField] private float startingAngle;

    public void InitializeTableSetup(int seats)
    {
        RectTransform table = GetComponent<RectTransform>();
        horizontalRadius = table.rect.width * 0.4f;
        verticalRadius = table.rect.height * 0.4f;

        cards = new();
        for (int i = 0; i < seats; i++)
        {
            GameObject playerHand = Instantiate(playerHandPrefab, transform);
            playerHand.name = $"Player Hand {i + 1}";
            PositionPlayerHand(playerHand, i, seats);

            GameObject firstCard = Instantiate(cardPrefab, playerHand.transform);
            firstCard.GetComponent<Image>().enabled = false;
            GameObject secondCard = Instantiate(cardPrefab, playerHand.transform);
            secondCard.GetComponent<Image>().enabled = false;

            cards.Add(firstCard);
            cards.Add(secondCard);
        }

        GameObject board = Instantiate(boardPrefab, transform);
        board.name = "Board";

        for (int i = 0; i < 5; i++)
        {
            GameObject boardCard = Instantiate(cardPrefab, board.transform);
            boardCard.GetComponent<Image>().enabled = false;
            cards.Add(boardCard);
        }
    }
    public void DisplayPlayerCard(Card card, int seatPos, int cardPos)
    {
        Sprite sprite = cardLibrary.GetSprite(card);

        GameObject cardObj = cards[2 * seatPos + cardPos - 3];

        Image image = cardObj.GetComponent<Image>();
        image.sprite = sprite;
        image.enabled = true;
    }

    public void DisplayBoardCard(Card card, int cardPos)
    {
        Sprite sprite = cardLibrary.GetSprite(card);

        GameObject cardObj = cards[cards.Count - 1 - (5 - cardPos)];

        Image image = cardObj.GetComponent<Image>();
        image.sprite = sprite;
        image.enabled = true;
    }

    private void PositionPlayerHand(GameObject playerHand, int seatIndex, int seatCount)
    {
        RectTransform rectTransform = playerHand.GetComponent<RectTransform>();

        float angleStep = 360f / seatCount;
        float angle = startingAngle - angleStep * seatIndex;
        float radians = angle * Mathf.Deg2Rad;

        rectTransform.anchoredPosition = new Vector2(
            Mathf.Cos(radians) * horizontalRadius,
            Mathf.Sin(radians) * verticalRadius
        );

        rectTransform.localRotation = Quaternion.identity;
    }
}
