using Model;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEditorInternal;

namespace Assets.Scripts.Model
{
    public class Table
    {
        private readonly System.Random rnd = new();
        public int Seats { get; }
        public Dictionary<Player, int> Players { get; }

        public Table(int seats, List<Player> players)
        {
            Seats = seats;
            Players = new();
            int currSeat = 1;
            foreach (Player player in players)
            {
                Players.Add(player, currSeat);
                currSeat++;
            }
        }
    }
}
