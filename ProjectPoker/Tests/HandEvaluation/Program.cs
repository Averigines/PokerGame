using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using static Model.CardRank;
using static Model.CardSuit;

static class Program
{
    static void Main()
    {
        RunGeneralTests();
        RunHoldemTests();
        RunOmahaTests();
        Console.WriteLine("All hand evaluation regression checks passed.");
    }

    #region General

    static void RunGeneralTests()
    {
        Console.WriteLine("General");
        Run(nameof(HandRankOwnsFiveCardsAndIgnoresSuitsInTies), HandRankOwnsFiveCardsAndIgnoresSuitsInTies);
        Run(nameof(DeckContainsEveryCardAndRejectsAnEmptyDraw), DeckContainsEveryCardAndRejectsAnEmptyDraw);
        Run(nameof(HigherKickerWinsWhenPairsMatch), HigherKickerWinsWhenPairsMatch);
        Run(nameof(TiedHandsReturnBothWinners), TiedHandsReturnBothWinners);
    }

    static void HandRankOwnsFiveCardsAndIgnoresSuitsInTies()
    {
        var cards = new List<Card> { new Card(Clubs, Ace), new Card(Clubs, Jack),
            new Card(Clubs, Nine), new Card(Clubs, Seven), new Card(Clubs, Four) };
        Card[] snapshot = cards.ToArray();
        var rank = new HandRank(HandType.Flush, cards);
        var otherSuit = new HandRank(HandType.Flush,
            cards.Select(card => new Card(Hearts, card.Rank)).ToList());
        Assert(rank.CompareTo(otherSuit) == 0, "Suits must not break a rank tie.");
        cards.Clear();
        Assert(rank.Cards.SequenceEqual(snapshot), "HandRank must copy the supplied cards.");
        foreach (IList<Card> invalid in new IList<Card>[] { null, cards, snapshot.Take(4).ToList() })
        {
            bool rejected = false;
            try { new HandRank(HandType.Flush, invalid); }
            catch (ArgumentException) { rejected = true; }
            Assert(rejected, "HandRank must require exactly five cards.");
        }
    }

    static void DeckContainsEveryCardAndRejectsAnEmptyDraw()
    {
        var deck = new Deck(new Random(42));
        Assert(deck.Cards.Count == 52, "A fresh deck must contain 52 cards.");
        Assert(deck.Cards.Distinct().Count() == 52, "Every card in the deck must be unique.");
        var beforeShuffle = new HashSet<Card>(deck.Cards);

        deck.Shuffle();

        Assert(beforeShuffle.SetEquals(deck.Cards), "Shuffling must preserve all cards.");
        for (int i = 0; i < 52; i++) deck.Draw();
        try
        {
            deck.Draw();
        }
        catch (InvalidOperationException)
        {
            deck.Reset();
            Assert(deck.Cards.Count == 52, "Reset must restore a complete deck.");
            return;
        }
        throw new Exception("Drawing from an empty deck must fail.");
    }

    static void HigherKickerWinsWhenPairsMatch()
    {
        PlayerHand kingKicker = PlayerWithPairAndKicker("King kicker", King);
        PlayerHand queenKicker = PlayerWithPairAndKicker("Queen kicker", Queen);
        var players = new List<PlayerHand> { queenKicker, kingKicker };

        PlayerHand winner = HandEvaluator.DetermineWinner(players);

        Assert(winner == kingKicker, "With matching nines and ace kickers, king must beat queen.");
    }

    static void TiedHandsReturnBothWinners()
    {
        PlayerHand firstPlayer = PlayerWithPairAndKicker("First player", King);
        PlayerHand secondPlayer = PlayerWithPairAndKicker("Second player", King);
        var players = new List<PlayerHand> { firstPlayer, secondPlayer };

        List<PlayerHand> winners = HandEvaluator.DetermineWinners(players);
        PlayerHand singleWinner = HandEvaluator.DetermineWinner(players);

        Assert(winners.SequenceEqual(players), "Both players must be returned when their hands tie.");
        Assert(singleWinner == null, "A tied hand must not have a single winner.");
    }

    #endregion

    #region Holdem

    static void RunHoldemTests()
    {
        Console.WriteLine("Holdem");
        Run(nameof(FlushQualifyingCardsKeepTheirDetectedSuit), FlushQualifyingCardsKeepTheirDetectedSuit);
        Run(nameof(RecognizesAllHandCategories), RecognizesAllHandCategories);
        Run(nameof(EvaluationPreservesInputOrder), EvaluationPreservesInputOrder);
        Run(nameof(EvaluationDoesNotDependOnInputOrder), EvaluationDoesNotDependOnInputOrder);
        Run(nameof(AceLowStraightFlushIsFiveHigh), AceLowStraightFlushIsFiveHigh);
        Run(nameof(FlushUsesTheFiveHighestSuitedCards), FlushUsesTheFiveHighestSuitedCards);
        Run(nameof(ThirdPairProvidesTheTwoPairKicker), ThirdPairProvidesTheTwoPairKicker);
        Run(nameof(TwoTripletsUseTheHigherTripletForTheFullHouse), TwoTripletsUseTheHigherTripletForTheFullHouse);
        Run(nameof(EvaluationRecognizesUnsortedStraight), EvaluationRecognizesUnsortedStraight);
        Run(nameof(RejectsTooFewCards), RejectsTooFewCards);
        Run(nameof(RejectsInvalidCardRanks), RejectsInvalidCardRanks);
        Run(nameof(RejectsDuplicateCards), RejectsDuplicateCards);
        Run(nameof(RoundsDealUniqueCardsAndPreservePreviousResults), RoundsDealUniqueCardsAndPreservePreviousResults);
    }

    static void FlushQualifyingCardsKeepTheirDetectedSuit()
    {
        var suited = new List<Card> { new Card(Hearts, Ace), new Card(Hearts, Jack),
            new Card(Hearts, Nine), new Card(Hearts, Seven), new Card(Hearts, Four) };
        var cards = new List<Card> { new Card(Clubs, Ace), new Card(Diamonds, Jack) };
        cards.AddRange(suited);
        HandRank flush = EvaluateHoldemCards(cards);
        Assert(flush.Type == HandType.Flush && flush.Cards.SequenceEqual(suited),
            "Matching off-suit ranks must not replace the detected flush cards.");

        suited = new List<Card> { new Card(Hearts, Five), new Card(Hearts, Four),
            new Card(Hearts, Three), new Card(Hearts, Two), new Card(Hearts, Ace) };
        cards = new List<Card> { new Card(Clubs, Ace), new Card(Diamonds, Five) };
        cards.AddRange(suited);
        HandRank straightFlush = EvaluateHoldemCards(cards);
        Assert(straightFlush.Type == HandType.StraightFlush && straightFlush.Cards.SequenceEqual(suited),
            "An ace-low straight flush must keep its suit and place the actual ace last.");
    }

    static void RecognizesAllHandCategories()
    {
        // Suits are explicit so the test hands can be checked against a real deck.
        AssertHandType(HandType.HighCard, new List<Card>
        {
            new Card(Clubs, Ace), new Card(Diamonds, Jack), new Card(Spades, Nine),
            new Card(Hearts, Seven), new Card(Clubs, Four), new Card(Diamonds, Two), new Card(Spades, Three)
        });

        AssertHandType(HandType.Pair, new List<Card>
        {
            new Card(Clubs, Nine), new Card(Diamonds, Ace), new Card(Spades, Nine),
            new Card(Hearts, Seven), new Card(Clubs, Four), new Card(Diamonds, Two), new Card(Spades, Three)
        });

        AssertHandType(HandType.TwoPair, new List<Card>
        {
            new Card(Clubs, Nine), new Card(Diamonds, Ace), new Card(Spades, Nine),
            new Card(Hearts, Ace), new Card(Clubs, Four), new Card(Diamonds, Two), new Card(Spades, Three)
        });

        AssertHandType(HandType.Trips, new List<Card>
        {
            new Card(Clubs, Nine), new Card(Diamonds, Ace), new Card(Spades, Nine),
            new Card(Hearts, Seven), new Card(Diamonds, Nine), new Card(Diamonds, Two), new Card(Spades, Three)
        });

        // A duplicate five must not interrupt the ace-low straight.
        AssertHandType(HandType.Straight, new List<Card>
        {
            new Card(Clubs, Ace), new Card(Diamonds, Two), new Card(Spades, Five),
            new Card(Hearts, Three), new Card(Clubs, Four),
            new Card(Diamonds, Five), new Card(Spades, Nine)
        });

        AssertHandType(HandType.Flush, FlushCards());
        AssertHandType(HandType.FullHouse, TwoTripletCards());

        AssertHandType(HandType.Quads, new List<Card>
        {
            new Card(Clubs, Nine), new Card(Diamonds, Nine), new Card(Spades, Nine),
            new Card(Hearts, Nine), new Card(Clubs, Ace),
            new Card(Diamonds, Three), new Card(Spades, Two)
        });

        AssertHandType(HandType.StraightFlush, AceLowStraightFlushCards());
    }

    static void EvaluationPreservesInputOrder()
    {
        List<Card> cards = TwoTripletCards();
        List<Card> holeCards = cards.Take(2).ToList();
        List<Card> board = cards.Skip(2).ToList();
        Card[] originalHoleCards = holeCards.ToArray();
        Card[] originalBoard = board.ToArray();

        HandEvaluator.EvaluateHoldemHand(holeCards, board);

        Assert(holeCards.SequenceEqual(originalHoleCards) && board.SequenceEqual(originalBoard),
            "Evaluation must preserve the caller's hole-card and board order.");
    }

    static void EvaluationDoesNotDependOnInputOrder()
    {
        List<Card> cards = TwoTripletCards();
        HandRank originalResult = EvaluateHoldemCards(cards);
        cards.Reverse();

        HandRank reversedResult = EvaluateHoldemCards(cards);

        Assert(reversedResult.Type == originalResult.Type, "Reversing cards must preserve the hand category.");
        AssertRanks(reversedResult.Cards.Select(card => card.Rank).ToArray(), originalResult.Cards.Select(card => card.Rank).ToArray());
    }

    static void AceLowStraightFlushIsFiveHigh()
    {
        HandRank result = EvaluateHoldemCards(AceLowStraightFlushCards());

        Assert(result.Type == HandType.StraightFlush, "A-2-3-4-5 of hearts must be a straight flush.");
        AssertRanks(result.Cards.Select(card => card.Rank).ToArray(), Five, Four, Three, Two, Ace);
    }

    static void FlushUsesTheFiveHighestSuitedCards()
    {
        HandRank result = EvaluateHoldemCards(FlushCards());

        Assert(result.Type == HandType.Flush, "Seven clubs without a straight must be a flush.");
        AssertRanks(result.Cards.Select(card => card.Rank).ToArray(), Ace, Jack, Nine, Seven, Four);
    }

    static void ThirdPairProvidesTheTwoPairKicker()
    {
        var cards = new List<Card>
        {
            new Card(Clubs, Ace), new Card(Diamonds, Ace),
            new Card(Spades, King), new Card(Hearts, King),
            new Card(Clubs, Queen), new Card(Diamonds, Queen), new Card(Spades, Two)
        };

        HandRank result = EvaluateHoldemCards(cards);
        AssertQualifyingCards(result, cards);

        Assert(result.Type == HandType.TwoPair, "Three pairs must use the highest two pairs.");
        AssertRanks(result.Cards.Select(card => card.Rank).ToArray(), Ace, Ace, King, King, Queen);
    }

    static void TwoTripletsUseTheHigherTripletForTheFullHouse()
    {
        HandRank result = EvaluateHoldemCards(TwoTripletCards());

        Assert(result.Type == HandType.FullHouse, "Two triplets must form a full house.");
        AssertRanks(result.Cards.Select(card => card.Rank).ToArray(), Ace, Ace, Ace, Nine, Nine);
    }

    static void EvaluationRecognizesUnsortedStraight()
    {
        var cards = new List<Card>
        {
            new Card(Clubs, Three), new Card(Diamonds, Ace), new Card(Spades, Two),
            new Card(Hearts, Five), new Card(Clubs, Four), new Card(Diamonds, Eight), new Card(Spades, Ten)
        };


        HandRank result = EvaluateHoldemCards(cards);
        AssertQualifyingCards(result, cards);

        Assert(result.Type == HandType.Straight, "Evaluation must recognize an unsorted ace-low straight.");
        AssertRanks(result.Cards.Select(card => card.Rank).ToArray(), Five, Four, Three, Two, Ace);
    }

    static void RejectsTooFewCards()
    {
        var cards = new List<Card> { new Card(Clubs, Two), new Card(Diamonds, Three) };

        AssertInvalidHand(cards, "A two-card hand must be rejected.");
    }

    static void RejectsInvalidCardRanks()
    {
        var cards = new List<Card>
        {
            new Card(Clubs, Two), new Card(Diamonds, Three), new Card(Spades, Four),
            new Card(Hearts, Five), new Card(Clubs, CardRank.None), new Card(Diamonds, Seven), new Card(Spades, Eight)
        };

        AssertInvalidHand(cards, "CardRank.None must be rejected.");
    }

    static void RejectsDuplicateCards()
    {
        var cards = new List<Card>
        {
            new Card(Clubs, Ace), new Card(Clubs, Ace),
            new Card(Hearts, Two), new Card(Hearts, Three), new Card(Hearts, Four), new Card(Diamonds, Seven), new Card(Spades, Eight)
        };

        AssertInvalidHand(cards, "The same ace of clubs cannot appear twice.");
    }

    static void RoundsDealUniqueCardsAndPreservePreviousResults()
    {
        var players = new List<Player> { new Player("Alice"), new Player("Bob"), new Player("Carol") };
        var table = new Table(3, players);
        var deck = new Deck(new Random(42));
        var round = new PokerRound(deck);

        RoundResult first = round.Play(table);
        Card[] originalBoard = first.Board.ToArray();
        Card[] originalHoleCards = first.Hands.SelectMany(hand => hand.HoleCards).ToArray();
        Card[] dealtCards = originalBoard.Concat(originalHoleCards).ToArray();

        Assert(first.Board.Count == 5, "A round must deal five board cards.");
        Assert(first.Hands.Count == 3, "Every player must receive a hand.");
        Assert(first.Hands.All(hand => hand.HoleCards.Count == 2), "Every player must receive two hole cards.");
        Assert(dealtCards.Distinct().Count() == 11, "All dealt cards must be unique.");
        Assert(deck.Cards.Count == 38, "The deck must consume eleven dealt cards and three burns.");
        Assert(first.Winners.Count >= 1, "A completed round must have winners.");
        foreach (PlayerHand hand in first.Hands)
        {
            HandRank expected = HandEvaluator.EvaluateHoldemHand(hand.HoleCards, first.Board);
            AssertQualifyingCards(hand.Rank, hand.HoleCards.Concat(first.Board).ToList());
            Assert(hand.Rank.CompareTo(expected) == 0, "Each result must rank its own hole cards and board.");
        }

        round.Play(table);

        Assert(first.Board.SequenceEqual(originalBoard), "Playing another round must preserve the previous board.");
        Assert(first.Hands.SelectMany(hand => hand.HoleCards).SequenceEqual(originalHoleCards),
            "Playing another round must preserve the previous hole cards.");
    }

    #endregion

    #region Omaha

    static void RunOmahaTests()
    {
        Console.WriteLine("Omaha");
        Run(nameof(OmahaUsesExactlyTwoHoleCards), OmahaUsesExactlyTwoHoleCards);
        Run(nameof(OmahaFindsBestCombinationAndTies), OmahaFindsBestCombinationAndTies);
        Run(nameof(OmahaDealsTenPlayers), OmahaDealsTenPlayers);
        Run(nameof(OmahaRejectsInvalidInputs), OmahaRejectsInvalidInputs);
    }

    static void OmahaUsesExactlyTwoHoleCards()
    {
        var board = new List<Card> { new Card(Hearts, Ace), new Card(Hearts, King),
            new Card(Hearts, Queen), new Card(Hearts, Jack), new Card(Hearts, Ten) };
        var hole = new List<Card> { new Card(Hearts, Nine), new Card(Clubs, Two),
            new Card(Diamonds, Four), new Card(Spades, Six) };
        Assert(HandEvaluator.EvaluateOmahaHand(hole, board).Type == HandType.HighCard,
            "Omaha cannot play the board or make a flush with only one suited hole card.");

        hole = new List<Card> { new Card(Clubs, Ace), new Card(Diamonds, Ace),
            new Card(Spades, Ace), new Card(Clubs, Two) };
        board = new List<Card> { new Card(Clubs, King), new Card(Diamonds, Queen),
            new Card(Spades, Jack), new Card(Hearts, Nine), new Card(Clubs, Seven) };
        Assert(HandEvaluator.EvaluateOmahaHand(hole, board).Type == HandType.Pair,
            "Three hole-card aces must not count as trips.");
    }

    static void OmahaFindsBestCombinationAndTies()
    {
        var board = new List<Card> { new Card(Clubs, Two), new Card(Hearts, Queen),
            new Card(Hearts, Jack), new Card(Hearts, Ten), new Card(Diamonds, Three) };
        var hole = new List<Card> { new Card(Spades, Four), new Card(Hearts, Ace),
            new Card(Clubs, Five), new Card(Hearts, King) };
        HandRank rank = HandEvaluator.EvaluateOmahaHand(hole, board);
        Assert(rank.Type == HandType.StraightFlush && rank.Cards[0].Rank == Ace,
            "The best combination may use nonadjacent hole and board cards.");
        AssertQualifyingCards(rank, hole.Concat(board).ToList());
        Assert(rank.Cards.Count(card => hole.Contains(card)) == 2 &&
            rank.Cards.Count(card => board.Contains(card)) == 3,
            "Omaha qualifying cards must use exactly two hole cards and three board cards.");
        hole.Reverse();
        board.Reverse();
        Assert(rank.CompareTo(HandEvaluator.EvaluateOmahaHand(hole, board)) == 0,
            "Omaha evaluation must be independent of input order.");

        board = new List<Card> { new Card(Clubs, Queen), new Card(Diamonds, Jack),
            new Card(Spades, Ten), new Card(Hearts, Two), new Card(Clubs, Three) };
        var first = new List<Card> { new Card(Clubs, Ace), new Card(Clubs, King),
            new Card(Clubs, Four), new Card(Clubs, Five) };
        var second = new List<Card> { new Card(Diamonds, Ace), new Card(Diamonds, King),
            new Card(Diamonds, Six), new Card(Diamonds, Seven) };
        var hands = new List<PlayerHand> {
            new PlayerHand(new Player("Alice"), first, HandEvaluator.EvaluateOmahaHand(first, board)),
            new PlayerHand(new Player("Bob"), second, HandEvaluator.EvaluateOmahaHand(second, board)) };
        Assert(HandEvaluator.DetermineWinners(hands).Count == 2, "Equal Omaha straights must tie.");
    }

    static void OmahaDealsTenPlayers()
    {
        var table = new Table(10, Enumerable.Range(1, 10).Select(i => new Player(i.ToString())).ToList(),
            PokerGame.PotLimitOmaha);
        var deck = new Deck(new Random(42));
        var round = new PokerRound(deck);
        RoundResult result = round.Play(table);
        Assert(table.Game == PokerGame.PotLimitOmaha && table.HoleCardCount == 4,
            "The table must own the selected game.");
        Assert(result.Hands.Count == 10 && result.Hands.All(h => h.HoleCards.Count == 4),
            "Every Omaha player must receive four cards.");
        Assert(result.Board.Count == 5 && result.Board.Concat(result.Hands.SelectMany(h => h.HoleCards))
            .Distinct().Count() == 45 && deck.Cards.Count == 4,
            "Ten-player Omaha must consume 45 unique dealt cards and three burns.");
        Card[] snapshot = result.Hands.SelectMany(h => h.HoleCards).ToArray();
        round.Play(table);
        Assert(result.Hands.SelectMany(h => h.HoleCards).SequenceEqual(snapshot),
            "Previous Omaha results must remain valid.");
        Assert(new Table(1, new List<Player> { new Player("Default") }).Game == PokerGame.TexasHoldem,
            "Existing tables must default to Hold'em.");
    }

    static void OmahaRejectsInvalidInputs()
    {
        var hole = new List<Card> { new Card(Clubs, Ace), new Card(Diamonds, King),
            new Card(Spades, Queen), new Card(Hearts, Jack) };
        var board = new List<Card> { new Card(Clubs, Ten), new Card(Diamonds, Nine), new Card(Spades, Eight), new Card(Hearts, Two), new Card(Clubs, Three) };
        Assert(HandEvaluator.EvaluateOmahaHand(hole, board).Type == HandType.Straight,
            "Omaha must evaluate a complete five-card board.");
        Action[] invalid = {
            () => HandEvaluator.EvaluateOmahaHand(null, board),
            () => HandEvaluator.EvaluateOmahaHand(hole, null),
            () => HandEvaluator.EvaluateOmahaHand(hole.Take(3).ToList(), board),
            () => HandEvaluator.EvaluateOmahaHand(hole, board.Take(2).ToList()),
            () => HandEvaluator.EvaluateOmahaHand(hole, new List<Card> { hole[0], board[1], board[2], board[3], board[4] }),
            () => new Table(1, new List<Player> { new Player("Invalid") }, (PokerGame)99)
        };
        foreach (Action action in invalid)
        {
            bool rejected = false;
            try { action(); } catch (ArgumentException) { rejected = true; }
            Assert(rejected, "Invalid Omaha inputs must be rejected.");
        }
    }

    #endregion

    #region Shared helpers

    static List<Card> AceLowStraightFlushCards() => new List<Card>
    {
        new Card(Hearts, Ace), new Card(Hearts, Two), new Card(Hearts, Five),
        new Card(Hearts, Three), new Card(Hearts, Four),
        new Card(Hearts, Nine), new Card(Hearts, Seven)
    };

    static List<Card> FlushCards() => new List<Card>
    {
        new Card(Clubs, Ace), new Card(Clubs, Jack), new Card(Clubs, Nine),
        new Card(Clubs, Seven), new Card(Clubs, Four),
        new Card(Clubs, Three), new Card(Clubs, Two)
    };

    static List<Card> TwoTripletCards() => new List<Card>
    {
        new Card(Clubs, Nine), new Card(Diamonds, Ace), new Card(Spades, Nine),
        new Card(Hearts, Ace), new Card(Diamonds, Nine),
        new Card(Clubs, Ace), new Card(Spades, Two)
    };

    static PlayerHand PlayerWithPairAndKicker(string name, CardRank secondKicker)
    {
        var cards = new List<Card>
        {
            new Card(Clubs, Nine), new Card(Diamonds, Nine), new Card(Spades, Ace),
            new Card(Hearts, secondKicker), new Card(Clubs, Four), new Card(Diamonds, Two), new Card(Spades, Three)
        };
        var player = new Player(name);
        return new PlayerHand(player, cards.Take(2).ToList(), EvaluateHoldemCards(cards));
    }

    static HandRank EvaluateHoldemCards(List<Card> cards) =>
        HandEvaluator.EvaluateHoldemHand(cards.Take(2).ToList(), cards.Skip(2).ToList());

    static void AssertHandType(HandType expected, List<Card> cards)
    {
        HandRank result = EvaluateHoldemCards(cards);
        AssertQualifyingCards(result, cards);
        Assert(result.Type == expected, $"Expected {expected}, but got {result.Type}. Cards: {string.Join(", ", cards)}");
    }

    static void AssertQualifyingCards(HandRank rank, List<Card> dealtCards)
    {
        Assert(rank.Cards.Count == 5 && rank.Cards.Distinct().Count() == 5,
            "An evaluated rank must contain five distinct qualifying cards.");
        Assert(rank.Cards.All(dealtCards.Contains), "Qualifying cards must come from the dealt cards.");

        if (rank.Type == HandType.Flush || rank.Type == HandType.StraightFlush)
            Assert(rank.Cards.Select(card => card.Suit).Distinct().Count() == 1,
                "Flush qualifying cards must share a suit.");
    }

    static void AssertRanks(IReadOnlyList<CardRank> actual, params CardRank[] expected)
    {
        Assert(actual.SequenceEqual(expected),
            $"Expected ranks [{string.Join(", ", expected)}], but got [{string.Join(", ", actual)}].");
    }

    static void AssertInvalidHand(List<Card> cards, string failureMessage)
    {
        try
        {
            EvaluateHoldemCards(cards);
        }
        catch (ArgumentException)
        {
            return;
        }

        throw new Exception(failureMessage);
    }

    static void Assert(bool condition, string failureMessage)
    {
        if (!condition)
        {
            throw new Exception(failureMessage);
        }
    }

    static void Run(string scenarioName, Action scenario)
    {
        try
        {
            scenario();
            Console.WriteLine($"PASS: {scenarioName}");
        }
        catch (Exception exception)
        {
            throw new Exception($"FAIL: {scenarioName}. {exception.Message}", exception);
        }
    }

    #endregion
}
