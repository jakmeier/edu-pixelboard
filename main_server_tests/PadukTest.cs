using PixelBoard.MainServer.Services;
using System.Collections;

namespace main_server_tests;

/// <summary>
/// Test rules and scoring specific to the game Paduk.
/// </summary>
public class PadukTest
{
    public static readonly int[] Teams = { 1, 2, 3, 4, 5 };

    private readonly IBoardService _board;
    private readonly IGameService _game;

    public PadukTest()
    {
        _board = new FakeBoardService();
        _game = new RealTimGoGameService(_board);
    }

    [Theory]
    [ClassData(typeof(PadukTestDataEnumerator))]
    public void TestScore(PadukMove[] moves, int expectedScoreTeam1, int expectedScoreTeam2)
    {
        _game.Start(Teams);
        foreach (PadukMove move in moves)
        {
            _game.MakeMove(move.X, move.Y, move.Team);
            _game.Tick();
        }
        _game.Stop();

        Assert.Equal(expectedScoreTeam1.ToString(), _game.GetTeamInfo(Teams[0])?["Score"]);
        Assert.Equal(expectedScoreTeam2.ToString(), _game.GetTeamInfo(Teams[1])?["Score"]);
    }
}

public class PadukMove
{
    public int X;
    public int Y;
    public int Team;

    public PadukMove(int x, int y, int team)
    {
        X = x;
        Y = y;
        Team = team;
    }
}

public class PadukTestDataEnumerator : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        int team1 = PadukTest.Teams[0];
        int team2 = PadukTest.Teams[1];

        yield return new object[] {
            new PadukMove[] {
                new PadukMove(10, 10, team1),
            },
            1,
            0,
        };

        yield return new object[] {
            new PadukMove[] {
                new PadukMove(10, 10, team1),
                new PadukMove(10, 10, team2),
                new PadukMove(10, 10, team1),
            },
            2,
            1,
        };

        yield return new object[] {
            new PadukMove[] {},
            0,
            0,
        };

        yield return new object[] {
            new PadukMove[] {
                new PadukMove(10, 11, team1),
                new PadukMove(10, 10, team2),
                new PadukMove(10, 11, team2),
                new PadukMove(10, 10, team2),
                new PadukMove(10, 10, team2),
            },
            2,
            7,
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
