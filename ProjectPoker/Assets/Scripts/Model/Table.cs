using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model
{
    public sealed class Table
    {
        public int Seats { get; }
        public PokerGame Game { get; }
        public int HoleCardCount => Game == PokerGame.PotLimitOmaha ? 4 : 2;
        private readonly Dictionary<Player, int> players = new Dictionary<Player, int>();
        public IReadOnlyDictionary<Player, int> Players { get; }

        public Table(int seats, IList<Player> players, PokerGame game = PokerGame.TexasHoldem)
        {
            if (!Enum.IsDefined(typeof(PokerGame), game))
                throw new ArgumentOutOfRangeException(nameof(game));
            if (players == null) throw new ArgumentNullException(nameof(players));
            if (seats < 1 || seats > 10 || players.Count < 1 || players.Count > seats)
                throw new ArgumentException("A table needs one to ten seats and enough seats for all players.");
            Seats = seats;
            Game = game;
            foreach (Player player in players)
            {
                if (player == null) throw new ArgumentException("Players cannot be null.", nameof(players));
                this.players.Add(player, this.players.Count + 1);
            }
            Players = new ReadOnlyDictionary<Player, int>(this.players);
        }
    }
}
