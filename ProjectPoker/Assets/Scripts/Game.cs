using Model;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Game : MonoBehaviour
{
    private Deck deck;
    private Card card;
    private List<Player> players;
    private List<Card> board;

    [SerializeField]
    private List<string> playerNames;

    // Testing
    private bool finishTestRuns = false;
    private int handsToPlay = 100;
    private int handsPlayed = 0;
    private int playersForTest = 8;
    private Dictionary<HandType, int> handRankCounts;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Testing
        handRankCounts = new()
        {
            {HandType.HighCard, 0},
            {HandType.Pair, 0},
            {HandType.TwoPair, 0},
            {HandType.Trips, 0},
            {HandType.Straight, 0},
            {HandType.Flush, 0},
            {HandType.FullHouse, 0},
            {HandType.Quads, 0},
            {HandType.StraightFlush, 0},
        };
        for (int i = 1; i <= handsToPlay; i++)
        {
            RunTestHand(playersForTest);
            foreach (Player player in players)
            {
                handRankCounts[player.HandRank.Type]++;
            }
        }

        Debug.Log($"We had this many high cards on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.HighCard]}");
        Debug.Log($"We had this many pairs on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.Pair]}");
        Debug.Log($"We had this many two pairs on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.TwoPair]}");
        Debug.Log($"We had this many trips on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.Trips]}");
        Debug.Log($"We had this many straights on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.Straight]}");
        Debug.Log($"We had this many flushes on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.Flush]}");
        Debug.Log($"We had this many full houses on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.FullHouse]}");
        Debug.Log($"We had this many quads on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.Quads]}");
        Debug.Log($"We had this many straight flushes on a total of {handsPlayed * playersForTest} hands: {handRankCounts[HandType.StraightFlush]}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DetermineHands()
    {
        List<Card> combinedCards;

        foreach (Player player in players)
        {
            combinedCards = new();
            combinedCards.AddRange(player.HoleCards);
            combinedCards.AddRange(board);
            player.AddHandRank(HandEvaluator.EvaluateHand(combinedCards));

            Debug.Log($"Player {player.Name} has {player.HandRank.Type} with those cards: {string.Join(", ", player.HandRank.HighCards)}");
        }
    }

    private void RunTestHand(int numPlayers)
    {
        deck = new Deck();
        deck.Shuffle();
        players = new();
        board = new();

        for (int i = 0; i < numPlayers; i++)
        {
            players.Add(new Player(playerNames[i]));
        }

        Debug.Log($"Starting Test Run with {numPlayers} players.");

        // Give each player two cards
        foreach (Player player in players)
        {
            card = deck.Draw();
            player.AddHoleCard(card);
            // Debug.Log($"Player {player.Name} got card {card}");
        }
        foreach (Player player in players)
        {
            card = deck.Draw();
            player.AddHoleCard(card);
            // Debug.Log($"Player {player.Name} got card {card}");
        }

        // Burn card
        card = deck.Draw();
        // Debug.Log($"First Burn card: {card}");

        // Give Flop
        for (int i = 0; i < 3; i++)
        {
            card = deck.Draw();
            board.Add(card);
        }
        // Debug.Log($"Flop is {string.Join(", ", board)}");

        // Burn card
        card = deck.Draw();
        // Debug.Log($"Second Burn card: {card}");

        // Give Turn
        card = deck.Draw();
        board.Add(card);
        // Debug.Log($"Turn is {card}");

        // Burn card
        card = deck.Draw();
        // Debug.Log($"Third Burn card: {card}");

        // Give River
        card = deck.Draw();
        board.Add(card);
        // Debug.Log($"River is {card}");

        foreach (Player player in players)
        {
            Debug.Log($"{player.Name}'s hole cards are {string.Join(", ", player.HoleCards)}");
        }
        Debug.Log($"Board is {string.Join(", ", board)}");

        handsPlayed++;
        DetermineHands();
    }
}
