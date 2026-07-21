using Model;
using System.Collections.Generic;
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
    private int handsPlayed = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RunTestHand(9);
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

            //Testing

            if (player.HandRank.Type == HandType.Quads)
            {
                Debug.Log($"Player {player.Name} has Quads.");
                Debug.Log($"{handsPlayed} hands were played.");
                finishTestRuns = true;
            }
        }
        if (!finishTestRuns)
        {
            RunTestHand(9);
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
            Debug.Log($"Player {player.Name} got card {card}");
        }
        foreach (Player player in players)
        {
            card = deck.Draw();
            player.AddHoleCard(card);
            Debug.Log($"Player {player.Name} got card {card}");
        }

        // Burn card
        card = deck.Draw();
        Debug.Log($"First Burn card: {card}");

        // Give Flop
        for (int i = 0; i < 3; i++)
        {
            card = deck.Draw();
            board.Add(card);
        }
        Debug.Log($"Flop is {string.Join(", ", board)}");

        // Burn card
        card = deck.Draw();
        Debug.Log($"Second Burn card: {card}");

        // Give Turn
        card = deck.Draw();
        board.Add(card);
        Debug.Log($"Turn is {card}");

        // Burn card
        card = deck.Draw();
        Debug.Log($"Third Burn card: {card}");

        // Give River
        card = deck.Draw();
        board.Add(card);
        Debug.Log($"River is {card}");

        Debug.Log($"Board is {string.Join(", ", board)}");
        foreach (Player player in players)
        {
            Debug.Log($"{player.Name}'s hole cards are {string.Join(", ", player.HoleCards)}");
        }

        handsPlayed++;
        DetermineHands();
    }
}
