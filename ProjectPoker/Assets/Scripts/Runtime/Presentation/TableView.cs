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

    public void InitializeTableSetup(Table pokerTable)
    {
        if (pokerTable == null) throw new System.ArgumentNullException(nameof(pokerTable));
        int seats = pokerTable.Seats;
        RectTransform table = GetComponent<RectTransform>();
        horizontalRadius = table.rect.width * 0.4f;
        verticalRadius = table.rect.height * 0.4f;

        if (playerHands.Count != 0) throw new System.InvalidOperationException("Table is already initialized.");
        for (int i = 0; i < seats; i++)
        {
            GameObject playerHand = Instantiate(playerHandPrefab, transform);
            playerHand.name = $"Player Hand {i + 1}";
            PositionPlayerHand(playerHand, i, seats);

            var cards = new Image[pokerTable.HoleCardCount];
            for (int card = 0; card < cards.Length; card++)
            {
                cards[card] = Instantiate(cardPrefab, playerHand.transform).GetComponent<Image>();
                cards[card].enabled = false;
            }
            playerHands.Add(cards);
            foreach (var player in pokerTable.Players)
                if (player.Value == i + 1)
                    CreatePlayerName(playerHand, player.Key.Name, cards);
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
    private void CreatePlayerName(GameObject playerHand, string playerName, Image[] cards)
    {
        var labelObject = new GameObject("Player Name", typeof(RectTransform), typeof(LayoutElement), typeof(Text));
        labelObject.transform.SetParent(playerHand.transform, false);
        labelObject.GetComponent<LayoutElement>().ignoreLayout = true;
        RectTransform rect = labelObject.GetComponent<RectTransform>();
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        float cardHeight = cards[0].rectTransform.sizeDelta.y;
        float handWidth = cards[0].rectTransform.sizeDelta.x * cards.Length;
        rect.sizeDelta = new Vector2(handWidth, 32f);
        rect.anchoredPosition = new Vector2(0f, -cardHeight * 0.5f - 20f);
        Text label = labelObject.GetComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 22;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.raycastTarget = false;
        label.text = playerName;
        label.supportRichText = false;
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 12;
        label.resizeTextMaxSize = 22;
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
