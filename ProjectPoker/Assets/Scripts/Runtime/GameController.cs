using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

public sealed class GameController : MonoBehaviour
{
    [SerializeField] private List<string> playerNames;
    [SerializeField] private int seats = 3;
    [SerializeField] private TableView cardViewer;
    private Table table;
    private readonly PokerRound round = new PokerRound();

    private void Start()
    {
        table = new Table(seats, playerNames.Select(name => new Player(name)).ToList());
        cardViewer.InitializeTableSetup(table.Seats);
        PlayRound();
    }

    public void PlayRound()
    {
        RoundResult result = round.Play(table);
        cardViewer.DisplayRound(table, result);
        Debug.Log($"Winner(s): {string.Join(", ", result.Winners.Select(hand => hand.Player.Name))}");
    }
}
