using Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class TableView : MonoBehaviour
{
    private readonly List<Image[]> playerHands = new List<Image[]>();
    private readonly List<Image> boardCards = new List<Image>();
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject boardPrefab;
    [SerializeField] private GameObject playerHandPrefab;
    [SerializeField] private CardSpriteLibrary cardLibrary;

    [Header("Table Layout")]
    private float horizontalRadius;
    private float verticalRadius;
    [SerializeField] private float startingAngle;

    public void InitializeTableSetup(int seats)
    {
        RectTransform table = GetComponent<RectTransform>();
        horizontalRadius = table.rect.width * 0.4f;
        verticalRadius = table.rect.height * 0.4f;

        if (playerHands.Count != 0) throw new System.InvalidOperationException("Table is already initialized.");
        for (int i = 0; i < seats; i++)
        {
            GameObject playerHand = Instantiate(playerHandPrefab, transform);
            playerHand.name = $"Player Hand {i + 1}";
            PositionPlayerHand(playerHand, i, seats);

            GameObject firstCard = Instantiate(cardPrefab, playerHand.transform);
            firstCard.GetComponent<Image>().enabled = false;
            GameObject secondCard = Instantiate(cardPrefab, playerHand.transform);
            secondCard.GetComponent<Image>().enabled = false;

            playerHands.Add(new[] { firstCard.GetComponent<Image>(), secondCard.GetComponent<Image>() });
        }

        GameObject board = Instantiate(boardPrefab, transform);
        board.name = "Board";

        for (int i = 0; i < 5; i++)
        {
            GameObject boardCard = Instantiate(cardPrefab, board.transform);
            boardCard.GetComponent<Image>().enabled = false;
            boardCards.Add(boardCard.GetComponent<Image>());
        }
    }
    public void DisplayPlayerCard(Card card, int seatPos, int cardPos)
    {
        Sprite sprite = cardLibrary.GetSprite(card);

        Image image = playerHands[seatPos - 1][cardPos - 1];
        image.sprite = sprite;
        image.enabled = true;
    }

    public void DisplayBoardCard(Card card, int cardPos)
    {
        Sprite sprite = cardLibrary.GetSprite(card);

        Image image = boardCards[cardPos - 1];
        image.sprite = sprite;
        image.enabled = true;
    }

    public void DisplayRound(Table table, RoundResult result)
    {
        foreach (PlayerHand hand in result.Hands)
            for (int card = 0; card < hand.HoleCards.Count; card++)
                DisplayPlayerCard(hand.HoleCards[card], table.Players[hand.Player], card + 1);
        for (int card = 0; card < result.Board.Count; card++)
            DisplayBoardCard(result.Board[card], card + 1);
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
