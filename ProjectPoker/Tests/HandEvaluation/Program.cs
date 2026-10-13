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
        Run(nameof(RecognizesAllHandCategories), RecognizesAllHandCategories);
        Run(nameof(EvaluationPreservesInputOrder), EvaluationPreservesInputOrder);
        Run(nameof(EvaluationDoesNotDependOnInputOrder), EvaluationDoesNotDependOnInputOrder);
        Run(nameof(AceLowStraightFlushIsFiveHigh), AceLowStraightFlushIsFiveHigh);
        Run(nameof(FlushUsesTheFiveHighestSuitedCards), FlushUsesTheFiveHighestSuitedCards);
        Run(nameof(ThirdPairProvidesTheTwoPairKicker), ThirdPairProvidesTheTwoPairKicker);
        Run(nameof(TwoTripletsUseTheHigherTripletForTheFullHouse), TwoTripletsUseTheHigherTripletForTheFullHouse);
        Run(nameof(EvaluationRecognizesUnsortedStraight), EvaluationRecognizesUnsortedStraight);
        Run(nameof(HigherKickerWinsWhenPairsMatch), HigherKickerWinsWhenPairsMatch);
        Run(nameof(TiedHandsReturnBothWinners), TiedHandsReturnBothWinners);
        Run(nameof(RejectsTooFewCards), RejectsTooFewCards);
        Run(nameof(RejectsInvalidCardRanks), RejectsInvalidCardRanks);
        Run(nameof(RejectsDuplicateCards), RejectsDuplicateCards);

        Run(nameof(RoundsDealUniqueCardsAndPreservePreviousResults), RoundsDealUniqueCardsAndPreservePreviousResults);
        Run(nameof(DeckContainsEveryCardAndRejectsAnEmptyDraw), DeckContainsEveryCardAndRejectsAnEmptyDraw);
        Console.WriteLine("All hand evaluation regression checks passed.");
    }

    static void RecognizesAllHandCategories()
    {
        // Suits are explicit so the test hands can be checked against a real deck.
        AssertHandType(HandType.HighCard, new List<Card>
        {
            new Card(Clubs, Ace), new Card(Diamonds, Jack), new Card(Spades, Nine),
            new Card(Hearts, Seven), new Card(Clubs, Four)
        });

        AssertHandType(HandType.Pair, new List<Card>
        {
            new Card(Clubs, Nine), new Card(Diamonds, Ace), new Card(Spades, Nine),
            new Card(Hearts, Seven), new Card(Clubs, Four)
        });

        AssertHandType(HandType.TwoPair, new List<Card>
        {
            new Card(Clubs, Nine), new Card(Diamonds, Ace), new Card(Spades, Nine),
            new Card(Hearts, Ace), new Card(Clubs, Four)
        });

        AssertHandType(HandType.Trips, new List<Card>
        {
            new Card(Clubs, Nine), new Card(Diamonds, Ace), new Card(Spades, Nine),
            new Card(Hearts, Seven), new Card(Diamonds, Nine)
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
        Card[] originalOrder = cards.ToArray();

        HandEvaluator.EvaluateHand(cards);

        Assert(cards.SequenceEqual(originalOrder), "Evaluation must preserve the caller's card order.");
    }

    static void EvaluationDoesNotDependOnInputOrder()
    {
        List<Card> cards = TwoTripletCards();
        HandRank originalResult = HandEvaluator.EvaluateHand(cards);
        cards.Reverse();

        HandRank reversedResult = HandEvaluator.EvaluateHand(cards);

        Assert(reversedResult.Type == originalResult.Type, "Reversing cards must preserve the hand category.");
        AssertRanks(reversedResult.HighCards, originalResult.HighCards.ToArray());
    }

    static void AceLowStraightFlushIsFiveHigh()
    {
        HandRank result = HandEvaluator.EvaluateHand(AceLowStraightFlushCards());

        Assert(result.Type == HandType.StraightFlush, "A-2-3-4-5 of hearts must be a straight flush.");
        AssertRanks(result.HighCards, Five, Four, Three, Two, Ace);
    }

    static void FlushUsesTheFiveHighestSuitedCards()
    {
        HandRank result = HandEvaluator.EvaluateHand(FlushCards());

        Assert(result.Type == HandType.Flush, "Seven clubs without a straight must be a flush.");
        AssertRanks(result.HighCards, Ace, Jack, Nine, Seven, Four);
    }

    static void ThirdPairProvidesTheTwoPairKicker()
    {
        var cards = new List<Card>
        {
            new Card(Clubs, Ace), new Card(Diamonds, Ace),
            new Card(Spades, King), new Card(Hearts, King),
            new Card(Clubs, Queen), new Card(Diamonds, Queen), new Card(Spades, Two)
        };

        HandRank result = HandEvaluator.EvaluateHand(cards);

        Assert(result.Type == HandType.TwoPair, "Three pairs must use the highest two pairs.");
        AssertRanks(result.HighCards, Ace, Ace, King, King, Queen);
    }

    static void TwoTripletsUseTheHigherTripletForTheFullHouse()
    {
        HandRank result = HandEvaluator.EvaluateHand(TwoTripletCards());

        Assert(result.Type == HandType.FullHouse, "Two triplets must form a full house.");
        AssertRanks(result.HighCards, Ace, Ace, Ace, Nine, Nine);
    }

    static void EvaluationRecognizesUnsortedStraight()
    {
        var cards = new List<Card>
        {
            new Card(Clubs, Three), new Card(Diamonds, Ace), new Card(Spades, Two),
            new Card(Hearts, Five), new Card(Clubs, Four)
        };


        HandRank result = HandEvaluator.EvaluateHand(cards);

        Assert(result.Type == HandType.Straight, "Evaluation must recognize an unsorted ace-low straight.");
        AssertRanks(result.HighCards, Five, Four, Three, Two, Ace);
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
            new Card(Hearts, Five), new Card(Clubs, CardRank.None)
        };

        AssertInvalidHand(cards, "CardRank.None must be rejected.");
    }

    static void RejectsDuplicateCards()
    {
        var cards = new List<Card>
        {
            new Card(Clubs, Ace), new Card(Clubs, Ace),
            new Card(Hearts, Two), new Card(Hearts, Three), new Card(Hearts, Four)
        };

        AssertInvalidHand(cards, "The same ace of clubs cannot appear twice.");
    }

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
            new Card(Hearts, secondKicker), new Card(Clubs, Four)
        };
        var player = new Player(name);
        return new PlayerHand(player, cards.Take(2).ToList(), HandEvaluator.EvaluateHand(cards));
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
            HandRank expected = HandEvaluator.EvaluateHand(hand.HoleCards.Concat(first.Board).ToList());
            Assert(hand.Rank.CompareTo(expected) == 0, "Each result must rank its own hole cards and board.");
        }

        round.Play(table);

        Assert(first.Board.SequenceEqual(originalBoard), "Playing another round must preserve the previous board.");
        Assert(first.Hands.SelectMany(hand => hand.HoleCards).SequenceEqual(originalHoleCards),
            "Playing another round must preserve the previous hole cards.");
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
    static void AssertHandType(HandType expected, List<Card> cards)
    {
        HandRank result = HandEvaluator.EvaluateHand(cards);
        Assert(result.Type == expected, $"Expected {expected}, but got {result.Type}. Cards: {string.Join(", ", cards)}");
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
            HandEvaluator.EvaluateHand(cards);
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
}
