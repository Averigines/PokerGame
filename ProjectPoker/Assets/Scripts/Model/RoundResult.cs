using System;
using System.Collections.Generic;

namespace Model
{
    public sealed class PlayerHand
    {
        public Player Player { get; }
        public IReadOnlyList<Card> HoleCards { get; }
        public HandRank Rank { get; }

        public PlayerHand(Player player, IList<Card> holeCards, HandRank rank)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            HoleCards = new List<Card>(holeCards).AsReadOnly();
            Rank = rank ?? throw new ArgumentNullException(nameof(rank));
        }
    }

    public sealed class RoundResult
    {
        public IReadOnlyList<Card> Board { get; }
        public IReadOnlyList<PlayerHand> Hands { get; }
        public IReadOnlyList<PlayerHand> Winners { get; }

        public RoundResult(IList<Card> board, IList<PlayerHand> hands)
        {
            Board = new List<Card>(board).AsReadOnly();
            Hands = new List<PlayerHand>(hands).AsReadOnly();
            Winners = HandEvaluator.DetermineWinners(Hands).AsReadOnly();
        }
    }
}
