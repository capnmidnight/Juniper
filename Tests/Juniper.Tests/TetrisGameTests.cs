using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

namespace Juniper.Puzzles;

[TestFixture]
public class TetrisGameTests
{
    [TestCase]
    public void TetrisTypeRowClear()
    {
        var before = new int[,] {
            {1, 2, 3, 4},
            {5, -1, -1, 6},
            {7, 8, 9, 10},
            {11, 12, 13, 14}
        };
        var after = new int[,] {
            {-1, -1, -1, -1},
            {1, 2, 3, 4},
            {5, -1, -1, 6},
            {11, 12, 13, 14}
        };

        var p = new Puzzle(after);
        var q = new TetrisGame(before);
        q.TetrisClearRow(2);

        Assert.AreEqual(p, q);
    }


    [TestCase]
    public void TetrisTypeRowClearBad1()
    {
        var before = new int[,] {
            {1, 2, 3, 4},
            {5, -1, -1, 6},
            {7, 8, 9, 10},
            {11, 12, 13, 14}
        };

        var p = new Puzzle(before);
        var q = new TetrisGame(before);
        q.TetrisClearRow(-1);

        Assert.AreEqual(p, q);
    }

    [TestCase]
    public void TetrisTypeRowClearBad2()
    {
        var before = new int[,] {
            {1, 2, 3, 4},
            {5, -1, -1, 6},
            {7, 8, 9, 10},
            {11, 12, 13, 14}
        };

        var p = new Puzzle(before);
        var q = new TetrisGame(before);
        q.TetrisClearRow(q.Height);

        Assert.AreEqual(p, q);
    }
}
