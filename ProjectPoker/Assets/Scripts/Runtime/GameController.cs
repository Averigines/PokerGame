using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private List<string> playerNames;
    [SerializeField] private int seats = 3;
    [SerializeField] private PokerGame game = PokerGame.TexasHoldem;
    [SerializeField] private TableView cardViewer;
    private Table table;
    private readonly PokerRound round = new PokerRound();

    private void Start()
    {
        table = new Table(seats, playerNames.Take(seats).Select(name => new Player(name)).ToList(), game);
        cardViewer.InitializeTableSetup(table);
        PlayRound();
    }

    public void PlayRound()
    {
        RoundResult result = round.Play(table);
        cardViewer.DisplayRound(table, result);
        foreach (PlayerHand winner in result.Winners)
            Debug.Log($"Winner: {winner.Player.Name} | {winner.Rank.Type} " +
                $"| Cards: {string.Join(", ", winner.Rank.Cards)}");
    }
}
