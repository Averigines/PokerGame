using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

public sealed class PokerSimulationRunner : MonoBehaviour
{
    [SerializeField] private List<string> playerNames;
    [SerializeField] private int seats = 3;
    [SerializeField] private int handsToPlay = 1000;
    [SerializeField] private int seed = 1;

    private void Start()
    {
        if (handsToPlay < 1) throw new ArgumentOutOfRangeException(nameof(handsToPlay));
        var table = new Table(seats, playerNames.Select(name => new Player(name)).ToList());
        var round = new PokerRound(new Deck(new System.Random(seed)));
        var counts = Enum.GetValues(typeof(HandType)).Cast<HandType>().ToDictionary(type => type, type => 0);
        for (int i = 0; i < handsToPlay; i++)
            foreach (PlayerHand hand in round.Play(table).Hands)
                counts[hand.Rank.Type]++;
        foreach (var count in counts)
            Debug.Log($"{count.Key}: {count.Value} of {handsToPlay * table.Players.Count} hands (seed {seed})");
    }
}
