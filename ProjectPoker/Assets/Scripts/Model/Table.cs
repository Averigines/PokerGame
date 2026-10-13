using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model
{
    public sealed class Table
    {
        public int Seats { get; }
        public IReadOnlyDictionary<Player, int> Players { get; }

        public Table(int seats, IList<Player> players)
        {
            if (players == null) throw new ArgumentNullException(nameof(players));
            if (seats < 1 || seats > 10 || players.Count < 1 || players.Count > seats)
                throw new ArgumentException("A table needs one to ten seats and enough seats for all players.");
            Seats = seats;
            var assignments = new Dictionary<Player, int>();
            foreach (Player player in players)
            {
                if (player == null) throw new ArgumentException("Players cannot be null.", nameof(players));
                assignments.Add(player, assignments.Count + 1);
            }
            Players = new ReadOnlyDictionary<Player, int>(assignments);
        }
    }
}
