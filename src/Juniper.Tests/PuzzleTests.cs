using System;

using NUnit.Framework;

namespace Juniper.Puzzles.Test
{
    [TestFixture]
    public class PuzzleTests
    {
        private readonly int[,] testGrid;
        public PuzzleTests()
        {
            testGrid = new int[,] {
{ 1, 2, 3 },
{ 4, 5, 6 },
{ 7, 8, 9 },
{ 10, 11, 12 },
{ 13, 14, 15 } };
        }


        [TestCase]
        public void DimensionsBad1()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(0, 0));
        }


        [TestCase]
        public void DimensionsBad2()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(0, 2));
        }


        [TestCase]
        public void DimensionsBad3()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(3, 0));
        }

        [TestCase]
        public void DimensionsBad4()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(-4, 0));
        }

        [TestCase]
        public void DimensionsBad5()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(0, -5));
        }

        [TestCase]
        public void DimensionsBad6()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(-6, 2));
        }

        [TestCase]
        public void DimensionsBad7()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(2, -7));
        }


        [TestCase]
        public void DimensionsBad8()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(-8, -8));
        }

        [TestCase]
        public void CheckWidthExplicit()
        {
            var puz = new Puzzle(3, 5);
            Assert.AreEqual(3, puz.Width);
        }

        [TestCase]
        public void CheckHeightExplicit()
        {
            var puz = new Puzzle(3, 5);
            Assert.AreEqual(5, puz.Height);
        }

        [TestCase]
        public void InitToNegOne()
        {
            var puz = new Puzzle(3, 5);
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }
        }

        [TestCase]
        public void InitGridBad()
        {
            Assert.Throws<ArgumentException>(() => new Puzzle(null));
        }

        [TestCase]
        public void CheckWidthImplicit()
        {
            var puz = new Puzzle(testGrid);
            Assert.AreEqual(3, puz.Width);
        }

        [TestCase]
        public void CheckHeightImplicit()
        {
            var puz = new Puzzle(testGrid);
            Assert.AreEqual(5, puz.Height);
        }


        [TestCase]
        public void InitToGrid()
        {
            var puz = new Puzzle(testGrid);
            for (var x = 0; x < puz.Width; ++x)
            {
                for (var y = 0; y < puz.Height; ++y)
            {
                    Assert.AreEqual(testGrid[x, y], puz[x, y]);
                }
            }
        }


        [TestCase]
        public void Comparable()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            var grid = new int[testGrid.GetWidth(), testGrid.GetHeight()];
            Array.Copy(testGrid, grid, testGrid.Length);
            grid[grid.GetWidth() - 1, grid.GetHeight() - 1] = 99;
            var r = new Puzzle(grid);

            Assert.AreEqual(q, p);
            Assert.AreNotEqual(p, r);
            Assert.AreNotEqual(q, r);
        }



        [TestCase]
        public void InBounds1()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsInBounds(-1, -1));
        }



        [TestCase]
        public void InBounds2()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsInBounds(p.Width / 2, -1));
        }

        [TestCase]
        public void InBounds3()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsInBounds(p.Width, -1));
        }

        [TestCase]
        public void InBounds4()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsInBounds(-1, p.Height / 2));
        }

        [TestCase]
        public void InBounds5()
        {
            var p = new Puzzle(testGrid);
            Assert.IsTrue(p.IsInBounds(p.Width / 2, p.Height / 2));
        }

        [TestCase]
        public void InBounds6()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsInBounds(p.Width, p.Height / 2));
        }

        [TestCase]
        public void InBounds7()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsInBounds(-1, p.Height));
        }

        [TestCase]
        public void InBounds8()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsInBounds(p.Width / 2, p.Height));
        }

        [TestCase]
        public void InBounds9()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsInBounds(p.Width, p.Height));
        }


        [TestCase]
        public void InBoundsRect1_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, -3, 2, 3));
        }


        [TestCase]
        public void InBoundsRect2_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, -3, 2, 3));
        }



        [TestCase]
        public void InBoundsRect3_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, -3, 2, 3));
        }


        [TestCase]
        public void InBoundsRect4_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, -3, 2, 3));
        }


        [TestCase]
        public void InBoundsRect5_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, -3, 2, 3));
        }


        [TestCase]
        public void InBoundsRect1_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, -2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect2_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, -2, 2, 3));
        }



        [TestCase]
        public void InBoundsRect3_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, -2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect4_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, -2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect5_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, -2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect1_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, 0, 2, 3));
        }


        [TestCase]
        public void InBoundsRect2_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, 0, 2, 3));
        }


        [TestCase]
        public void InBoundsRect3_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsTrue(p.IsInBounds(0, 0, 2, 3));
        }


        [TestCase]
        public void InBoundsRect4_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, 0, 2, 3));
        }


        [TestCase]
        public void InBoundsRect5_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, 0, 2, 3));
        }


        [TestCase]
        public void InBoundsRect1_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, p.Height - 2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect2_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, p.Height - 2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect3_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, p.Height - 2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect4_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, p.Height - 2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect5_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, p.Height - 2, 2, 3));
        }


        [TestCase]
        public void InBoundsRect1_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, p.Height, 2, 3));
        }


        [TestCase]
        public void InBoundsRect2_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, p.Height, 2, 3));
        }


        [TestCase]
        public void InBoundsRect3_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, p.Height, 2, 3));
        }


        [TestCase]
        public void InBoundsRect4_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, p.Height, 2, 3));
        }


        [TestCase]
        public void InBoundsRect5_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, p.Height, 2, 3));
        }


        [TestCase]
        public void InBoundsShape1_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, -3, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape2_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, -3, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }



        [TestCase]
        public void InBoundsShape3_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, -3, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape4_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, -3, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape5_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, -3, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape1_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, -2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape2_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, -2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }



        [TestCase]
        public void InBoundsShape3_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, -2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape4_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, -2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape5_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, -2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape1_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, 0, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape2_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, 0, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape3_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsTrue(p.IsInBounds(0, 0, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape4_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, 0, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape5_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, 0, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape1_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, p.Height - 2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape2_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, p.Height - 2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape3_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, p.Height - 2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape4_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, p.Height - 2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape5_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, p.Height - 2, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape1_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, p.Height, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape2_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, p.Height, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape3_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, p.Height, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape4_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, p.Height, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShape5_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, p.Height, new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } }));
        }


        [TestCase]
        public void InBoundsShapeBad1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(1, 2, (int[,])null));
        }


        [TestCase]
        public void InBoundsShapeBad2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(1, 2, new int[0, 0]));
        }


        [TestCase]
        public void InBoundsPuzzle1_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, -3, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle2_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, -3, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }



        [TestCase]
        public void InBoundsPuzzle3_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, -3, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle4_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, -3, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle5_1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, -3, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle1_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, -2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle2_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, -2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }



        [TestCase]
        public void InBoundsPuzzle3_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, -2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle4_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, -2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle5_2()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, -2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle1_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, 0, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle2_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, 0, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle3_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsTrue(p.IsInBounds(0, 0, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle4_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, 0, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle5_3()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, 0, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle1_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, p.Height - 2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle2_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, p.Height - 2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle3_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, p.Height - 2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle4_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, p.Height - 2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle5_4()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, p.Height - 2, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle1_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-2, p.Height, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle2_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(-1, p.Height, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle3_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(0, p.Height, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle4_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width - 1, p.Height, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzle5_5()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(p.Width, p.Height, new Puzzle(new int[,] { { 1, -1 }, { 2, 3 }, { 5, -1 } })));
        }


        [TestCase]
        public void InBoundsPuzzleBad1()
        {
            var p = new Puzzle(3, 5);
            Assert.IsFalse(p.IsInBounds(1, 1, (Puzzle)null));
        }


        [TestCase]
        public void IndexerGetParams1()
        {
            var p = new Puzzle(testGrid);
            Assert.AreEqual(Puzzle.EmptyTile, p[-1, -1]);
        }


        [TestCase]
        public void IndexerGetParams2()
        {
            var p = new Puzzle(testGrid);
            Assert.AreEqual(Puzzle.EmptyTile, p[p.Width / 2, -1]);
        }

        [TestCase]
        public void IndexerGetParams3()
        {
            var p = new Puzzle(testGrid);
            Assert.AreEqual(Puzzle.EmptyTile, p[p.Width, -1]);
        }

        [TestCase]
        public void IndexerGetParams4()
        {
            var p = new Puzzle(testGrid);
            Assert.AreEqual(Puzzle.EmptyTile, p[-1, p.Height / 2]);
        }

        [TestCase]
        public void IndexerGetParams5()
        {
            var p = new Puzzle(testGrid);
            Assert.AreNotEqual(Puzzle.EmptyTile, p[p.Width / 2, p.Height / 2]);
        }

        [TestCase]
        public void IndexerGetParams6()
        {
            var p = new Puzzle(testGrid);
            Assert.AreEqual(Puzzle.EmptyTile, p[p.Width, p.Height / 2]);
        }

        [TestCase]
        public void IndexerGetParams7()
        {
            var p = new Puzzle(testGrid);
            Assert.AreEqual(Puzzle.EmptyTile, p[-1, p.Height]);
        }

        [TestCase]
        public void IndexerGetParams8()
        {
            var p = new Puzzle(testGrid);
            Assert.AreEqual(Puzzle.EmptyTile, p[p.Width / 2, p.Height]);
        }

        [TestCase]
        public void IndexerGetParams9()
        {
            var p = new Puzzle(testGrid);
            Assert.AreEqual(Puzzle.EmptyTile, p[p.Width, p.Height]);
        }


        [TestCase]
        public void IndexerSetParams1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[-1, -1] = 99;
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void IndexerSetParams2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[p.Width / 2, -1] = 99;
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void IndexerSetParams3()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[p.Width, -1] = 99;
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void IndexerSetParams4()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[-1, p.Height / 2] = 99;
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void IndexerSetParams5()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[p.Width / 2, p.Height / 2] = 99;
            Assert.AreNotEqual(q, p);
        }


        [TestCase]
        public void IndexerSetParams6()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[p.Width, p.Height / 2] = 99;
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void IndexerSetParams7()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[-1, p.Height] = 99;
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void IndexerSetParams8()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[p.Width / 2, p.Height] = 99;
            Assert.AreEqual(q, p, "8");
        }


        [TestCase]
        public void IndexerSetParams9()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p[p.Width, p.Height] = 99;
            Assert.AreEqual(q, p, "9");
        }


        [TestCase]
        public void ClearRow()
        {
            var puz = new Puzzle(testGrid);
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Clear(Puzzle.RowOrder, 2);
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    if (y == 2)
                    {
                        Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                    else
                    {
                        Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                }
            }
        }

        [TestCase]
        public void ClearColumn()
        {
            var puz = new Puzzle(testGrid);
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Clear(Puzzle.ColumnOrder, 2);
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    if (x == 2)
                    {
                        Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                    else
                    {
                        Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                }
            }
        }


        [TestCase]
        public void Clear()
        {
            var puz = new Puzzle(testGrid);
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Clear();
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }
        }


        [TestCase]
        public void ClearPoint()
        {
            var puz = new Puzzle(testGrid);
            Assert.AreNotEqual(Puzzle.EmptyTile, puz[2, 2]);
            puz.Clear(2, 2);
            Assert.AreEqual(Puzzle.EmptyTile, puz[2, 2]);
        }


        [TestCase]
        public void ClearPointBad1()
        {
            var puz = new Puzzle(testGrid);
            var p2 = new Puzzle(testGrid);
            puz.Clear(-1, -1);
            Assert.AreEqual(p2, puz);
        }


        [TestCase]
        public void ClearPointBad2()
        {
            var puz = new Puzzle(testGrid);
            var p2 = new Puzzle(testGrid);
            puz.Clear(-1, 1);
            Assert.AreEqual(p2, puz);
        }


        [TestCase]
        public void ClearPointBad3()
        {
            var puz = new Puzzle(testGrid);
            var p2 = new Puzzle(testGrid);
            puz.Clear(-1, puz.Height);
            Assert.AreEqual(p2, puz);
        }


        [TestCase]
        public void ClearPointBad4()
        {
            var puz = new Puzzle(testGrid);
            var p2 = new Puzzle(testGrid);
            puz.Clear(1, -1);
            Assert.AreEqual(p2, puz);
        }


        [TestCase]
        public void ClearPointBad5()
        {
            var puz = new Puzzle(testGrid);
            var p2 = new Puzzle(testGrid);
            puz.Clear(1, puz.Height);
            Assert.AreEqual(p2, puz);
        }


        [TestCase]
        public void ClearPointBad6()
        {
            var puz = new Puzzle(testGrid);
            var p2 = new Puzzle(testGrid);
            puz.Clear(puz.Width, -1);
            Assert.AreEqual(p2, puz);
        }


        [TestCase]
        public void ClearPointBad7()
        {
            var puz = new Puzzle(testGrid);
            var p2 = new Puzzle(testGrid);
            puz.Clear(puz.Width, 1);
            Assert.AreEqual(p2, puz);
        }


        [TestCase]
        public void ClearPointBad8()
        {
            var puz = new Puzzle(testGrid);
            var p2 = new Puzzle(testGrid);
            puz.Clear(puz.Width, puz.Height);
            Assert.AreEqual(p2, puz);
        }


        [TestCase]
        public void ClearRect()
        {
            var puz = new Puzzle(testGrid);
            for (var y = 1; y < 3; ++y)
            {
                for (var x = 1; x < 3; ++x)
                {
                    Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Clear(1, 1, 2, 2);
            for (var y = 1; y < 3; ++y)
            {
                for (var x = 1; x < 3; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }
        }


        [TestCase]
        public void ClearRectBad1()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, -2, 2, 2);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearRectBad2()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, 0, 2, 2);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearRectBad3()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, puz.Height, 2, 2);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearRectBad4()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(0, -2, 2, 2);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearRectBad5()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(0, puz.Height, 2, 2);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearRectBad6()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, -2, 2, 2);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearRectBad7()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, 0, 2, 2);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearRectBad8()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, puz.Height, 2, 2);
            Assert.AreEqual(quz, puz);
        }

        [TestCase]
        public void ClearMask()
        {
            var puz = new Puzzle(testGrid);
            var mask = new int[,] { { -1, -1, -1 }, { -1, -1, 1 }, { -1, 2, 3 }, { -1, 5, 7 }, { -1, 11, -1 } };
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Clear(mask);
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    if (mask[y, x] != Puzzle.EmptyTile)
                    {
                        Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                    else
                    {
                        Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                }
            }
        }


        [TestCase]
        public void ClearMaskBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Clear(null);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ClearMaskBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Clear(new int[0, 0]);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ClearMaskBad3()
        {

            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Clear(new int[1, 1] { { 1 } });
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ClearShape()
        {
            var puz = new Puzzle(testGrid);
            var shape = new int[,] { { 1, -1, -1 }, { 2, 3, 4 } };
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Clear(0, 2, shape);
            for (var y = 0; y < puz.Height; ++y)
            {
                for (var x = 0; x < puz.Width; ++x)
                {
                    if (y >= 2 && y < 4 && shape[y - 2, x] != Puzzle.EmptyTile)
                    {
                        Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                    else
                    {
                        Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                }
            }
        }

        [TestCase]
        public void ClearShapeBad1()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearShapeBad2()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, 0, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearShapeBad3()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearShapeBad4()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(0, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearShapeBad5()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(0, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearShapeBad6()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearShapeBad7()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, 0, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearShapeBad8()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }

        [TestCase]
        public void ClearShapeBad9()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Clear(0, 0, (int[,])null);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ClearShapeBad10()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Clear(0, 0, new int[0, 0]);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ClearPuzzle()
        {
            var before = new int[,] { { 1, 2, 3 }, { 23, 29, 31 }, { 5, 7, 11 }, { 13, 17, 19 } };
            var after = new int[,] { { 1, 2, 3 }, { 23, 29, 31 }, { 5, -1, 11 }, { 13, -1, -1 } };
            var shape = new int[,] { { 1, -1 }, { 2, 3 } };
            var p = new Puzzle(after);
            var q = new Puzzle(shape);
            var r = new Puzzle(before);
            r.Clear(1, 2, q);
            Assert.AreEqual(p, r);
        }

        [TestCase]
        public void ClearPuzzleBad1()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearPuzzleBad2()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, 0, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearPuzzleBad3()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(-2, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearPuzzleBad4()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(0, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearPuzzleBad5()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(0, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearPuzzleBad6()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearPuzzleBad7()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, 0, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void ClearPuzzleBad8()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Clear(puz.Width, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }

        [TestCase]
        public void ClearPuzzleBad9()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Clear(0, 0, (Puzzle)null);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void FillGrid()
        {
            var puz = new Puzzle(4, 5);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Fill(3);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreEqual(3, puz[x, y]);
                }
            }
        }


        [TestCase]
        public void FillRow()
        {
            var puz = new Puzzle(4, 5);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Fill(Puzzle.RowOrder, 2, 3);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    if (y == 2)
                    {
                        Assert.AreEqual(3, puz[x, y]);
                    }
                    else
                    {
                        Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                }
            }
        }


        [TestCase]
        public void FillRowBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Fill(Puzzle.RowOrder, -1, 99);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void FillRowBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Fill(Puzzle.RowOrder, p.Height, 99);
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void FillColumn()
        {
            var puz = new Puzzle(4, 5);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Fill(Puzzle.ColumnOrder, 2, 3);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    if (x == 2)
                    {
                        Assert.AreEqual(3, puz[x, y]);
                    }
                    else
                    {
                        Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                }
            }
        }


        [TestCase]
        public void FillColumnBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Fill(Puzzle.ColumnOrder, -1, 99);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void FillColumnBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Fill(Puzzle.ColumnOrder, p.Width, 99);
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void FillRect()
        {
            var puz = new Puzzle(4, 5);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Fill(1, 1, 2, 2, 3);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    if (x >= 1 && x < 3 && y >= 1 && y < 3)
                    {
                        Assert.AreEqual(3, puz[x, y]);
                    }
                    else
                    {
                        Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                    }
                }
            }
        }

        [TestCase]
        public void FillRectBad1()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, -2, 2, 2, 99);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillRectBad2()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, 0, 2, 2, 99);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillRectBad3()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, puz.Height, 2, 2, 99);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillRectBad4()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(0, -2, 2, 2, 99);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillRectBad5()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(0, puz.Height, 2, 2, 99);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillRectBad6()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, -2, 2, 2, 99);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillRectBad7()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, 0, 2, 2, 99);
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillRectBad8()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, puz.Height, 2, 2, 99);
            Assert.AreEqual(quz, puz);
        }

        [TestCase]
        public void FillGridRandom1()
        {
            var puz = new Puzzle(4, 5);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            Random a = new Random(5), b = new Random(5);
            puz.Fill(a);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreEqual(b.Next(), puz[x, y]);
                }
            }
        }


        [TestCase]
        public void FillGridRandomBad()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Fill(null);
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void FillGridRandom2()
        {
            var puz = new Puzzle(4, 5);
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }

            puz.Fill();
            for (var y = 0; y < 5; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    Assert.AreNotEqual(Puzzle.EmptyTile, puz[x, y]);
                }
            }
        }


        [TestCase]
        public void FillShape()
        {
            var puz = new Puzzle(4, 5);
            var shape = new int[,] { { 2, 3, Puzzle.EmptyTile }, { Puzzle.EmptyTile, 5, 6 } };
            puz.Fill(1, 3, shape);
            for (var y = 3; y < 5; ++y)
            {
                for (var x = 1; x < 4; ++x)
                {
                    Assert.AreEqual(shape[y - 3, x - 1], puz[x, y]);
                }
            }
        }


        [TestCase]
        public void FillShapeBad1()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillShapeBad2()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, 0, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillShapeBad3()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillShapeBad4()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(0, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillShapeBad5()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(0, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillShapeBad6()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillShapeBad7()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, 0, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillShapeBad8()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } });
            Assert.AreEqual(quz, puz);
        }

        [TestCase]
        public void FillShapeBad9()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Fill(0, 0, (int[,])null);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void FillShapeBad10()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Fill(0, 0, new int[0, 0]);
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void FillShapeIgnoresEmpties()
        {
            var p = new Puzzle(4, 5);
            var after = new int[,] {
{ 10, 10, 10, 10 },
{ 10, 10, 10, 10 },
{ 10, 10, 10, 10 },
{ 10,  2,  3, 10 },
{ 10, 10,  5,  6 } };
            var q = new Puzzle(after);
            var shape = new int[,] { { 2, 3, Puzzle.EmptyTile }, { Puzzle.EmptyTile, 5, 6 } };
            p.Fill(10);
            p.Fill(1, 3, shape);
            Assert.AreEqual(q, p);
        }



        [TestCase]
        public void FillPuzzle()
        {
            var puz = new Puzzle(4, 5);
            var piece = new Puzzle(new int[,] { { 2, 3, Puzzle.EmptyTile }, { Puzzle.EmptyTile, 5, 6 } });
            puz.Fill(1, 3, piece);
            for (var y = 3; y < 5; ++y)
            {
                for (var x = 1; x < 4; ++x)
                {
                    Assert.AreEqual(piece[x - 1, y - 3], puz[x, y]);
                }
            }
        }


        [TestCase]
        public void FillPuzzleBad1()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillPuzzleBad2()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, 0, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillPuzzleBad3()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(-2, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillPuzzleBad4()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(0, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillPuzzleBad5()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(0, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillPuzzleBad6()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillPuzzleBad7()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, 0, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }


        [TestCase]
        public void FillPuzzleBad8()
        {
            var puz = new Puzzle(testGrid);
            var quz = new Puzzle(testGrid);
            puz.Fill(puz.Width, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 3 } }));
            Assert.AreEqual(quz, puz);
        }

        [TestCase]
        public void FillPuzzleBad9()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);

            p.Fill(0, 0, (Puzzle)null);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void FillPuzzleIgnoresEmpties()
        {

            var p = new Puzzle(4, 5);
            var after = new int[,] {
{ 10, 10, 10, 10 },
{ 10, 10, 10, 10 },
{ 10, 10, 10, 10 },
{ 10,  2,  3, 10 },
{ 10, 10,  5,  6 } };
            var q = new Puzzle(after);
            var shape = new Puzzle(new int[,] { { 2, 3, Puzzle.EmptyTile }, { Puzzle.EmptyTile, 5, 6 } });
            p.Fill(10);
            p.Fill(1, 3, shape);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ShiftColumnsDown()
        {
            var before = new int[,] {
{ 1, 2, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, 7, 8 } };
            var after = new int[,] {
{ 1, Puzzle.EmptyTile, 3 },
{ 4, 2, 5 },
{ 6, 7, 8 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsDown();
            Assert.AreEqual(q, p);
        }



        [TestCase]
        public void ShiftColumnsDown2()
        {
            var before = new int[,] {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, Puzzle.EmptyTile},
                {17, 18, Puzzle.EmptyTile, 20},
                {Puzzle.EmptyTile, 22, 23, 24},
                {25, Puzzle.EmptyTile, 27, 28}};
            var after = new int[,] {
                {Puzzle.EmptyTile, Puzzle.EmptyTile, Puzzle.EmptyTile, Puzzle.EmptyTile},
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, 20},
                {17, 18, 23, 24},
                {25, 22, 27, 28}};
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsDown();
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void ShiftColumnsDown3()
        {
            var before = new int[,] {
{ 1, 2, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, Puzzle.EmptyTile, 8 } };
            var after = new int[,] {
{ 1, Puzzle.EmptyTile, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, 2, 8 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsDown();
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ShiftColumnsRight()
        {
            var before = new int[,] {
{ 1, 2, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, 7, 8 } };
            var after = new int[,] {
{ 1, 2, 3 },
{ Puzzle.EmptyTile, 4, 5 },
{ 6, 7, 8 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsRight();
            Assert.AreEqual(q, p);
        }



        [TestCase]
        public void ShiftColumnsRight2()
        {
            var before = new int[,] {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, Puzzle.EmptyTile},
                {17, 18, Puzzle.EmptyTile, 20},
                {Puzzle.EmptyTile, 22, 23, 24},
                {25, Puzzle.EmptyTile, 27, 28}};
            var after = new int[,]  {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {Puzzle.EmptyTile, 13, 14, 15},
                {Puzzle.EmptyTile, 17, 18, 20},
                {Puzzle.EmptyTile, 22, 23, 24},
                {Puzzle.EmptyTile, 25, 27, 28}};
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsRight();
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void ShiftColumnsRight3()
        {
            var before = new int[,] {
{ 1, 2, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, Puzzle.EmptyTile, 8 } };
            var after = new int[,]{
{ 1, 2, 3 },
{ Puzzle.EmptyTile, 4, 5 },
{ Puzzle.EmptyTile, 6, 8 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsRight();
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ShiftColumnsLeft()
        {
            var before = new int[,] {
{ 1, 2, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, 7, 8 } };
            var after = new int[,] {
{ 1, 2, 3 },
{ 4, 5, Puzzle.EmptyTile },
{ 6, 7, 8 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsLeft();
            Assert.AreEqual(q, p);
        }



        [TestCase]
        public void ShiftColumnsLeft2()
        {
            var before = new int[,] {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, Puzzle.EmptyTile},
                {17, 18, Puzzle.EmptyTile, 20},
                {Puzzle.EmptyTile, 22, 23, 24},
                {25, Puzzle.EmptyTile, 27, 28}};
            var after = new int[,]  {
                {1, 2, 3, 4},
                {5, 6, 7, 8},
                {9, 10, 11, 12},
                {13, 14, 15, Puzzle.EmptyTile},
                {17, 18, 20, Puzzle.EmptyTile},
                {22, 23, 24, Puzzle.EmptyTile},
                {25, 27, 28, Puzzle.EmptyTile}};
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsLeft();
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void ShiftColumnsLeft3()
        {
            var before = new int[,] {
{ 1, 2, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, Puzzle.EmptyTile, 8 } };
            var after = new int[,]{
{ 1, 2, 3 },
{ 4, 5, Puzzle.EmptyTile },
{ 6, 8, Puzzle.EmptyTile } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsLeft();
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void ShiftColumnsUp()
        {
            var before = new int[,] {
{ 1, 2, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, 7, 8 } };
            var after = new int[,] {
{ 1, 2, 3 },
{ 4, 7, 5 },
{ 6, Puzzle.EmptyTile, 8 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsUp();
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void ShiftColumnsUp2()
        {
            var before = new int[,] {
                {1, Puzzle.EmptyTile, 3, 4},
                {5, 6, 7, Puzzle.EmptyTile},
                {9, 10, Puzzle.EmptyTile, 12},
                {Puzzle.EmptyTile, 14, 15, 16},
                {17, 18, 19, 20},
                {21, 22, 23, 24},
                {25, 26, 27, 28}};
            var after = new int[,] {
                {1, 6, 3, 4},
                {5, 10, 7, 12},
                {9, 14, 15, 16},
                {17, 18, 19, 20},
                {21, 22, 23, 24},
                {25, 26, 27, 28},
                {Puzzle.EmptyTile, Puzzle.EmptyTile, Puzzle.EmptyTile, Puzzle.EmptyTile}};
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsUp();
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void ShiftColumnsUp3()
        {
            var before = new int[,] {
{ 1, Puzzle.EmptyTile, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, 2, 8 } };
            var after = new int[,] {
{ 1, 2, 3 },
{ 4, Puzzle.EmptyTile, 5 },
{ 6, Puzzle.EmptyTile, 8 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.ShiftColumnsUp();
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void NullGrid()
        {
            var p = new Puzzle(1, 1);
            Assert.Throws<ArgumentException>(() => p.Grid = null);
        }


        [TestCase]
        public void EmptyGrid()
        {
            var p = new Puzzle(1, 1);
            var grid = new int[0, 0];
            Assert.Throws<ArgumentException>(() => p.Grid = grid);
        }


        [TestCase]
        public void SwapPoint()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(0, 1, 2, 3);
            Assert.AreEqual(p[0, 1], q[2, 3]);
            Assert.AreEqual(p[2, 3], q[0, 1]);
        }

        [TestCase]
        public void SwapPointBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(-1, -1, -2, -2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapPointBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(-1, -1, 2, 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapPointBad3()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(1, 1, -2, -2);
            Assert.AreEqual(q, p);
        }



        [TestCase]
        public void SwapPointBad4()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(p.Width, p.Height, p.Width + 1, p.Height + 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapPointBad5()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(p.Width, p.Height, 2, 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapPointBad6()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(1, 1, p.Width + 1, p.Height + 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapRows()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(Puzzle.RowOrder, 1, 3);
            for (var x = 0; x < p.Width; ++x)
            {
                Assert.AreEqual(p[x, 1], q[x, 3]);
                Assert.AreEqual(p[x, 3], q[x, 1]);
            }
        }

        [TestCase]
        public void SwapRowsBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(Puzzle.RowOrder, -1, 3);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapRowsBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(Puzzle.RowOrder, -1, -3);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapRowsBad3()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(Puzzle.RowOrder, 1, -3);
            Assert.AreEqual(q, p);
        }

        [TestCase]
        public void SwapColumns()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(Puzzle.ColumnOrder, 0, 2);
            for (var y = 0; y < p.Height; ++y)
            {
                Assert.AreEqual(p[0, y], q[2, y]);
                Assert.AreEqual(p[2, y], q[0, y]);
            }
        }

        [TestCase]
        public void SwapColumnsBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(Puzzle.ColumnOrder, -1, 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapColumnsBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(Puzzle.ColumnOrder, -1, -2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapColumnsBad3()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(Puzzle.ColumnOrder, 1, -2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapRect()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            var sx = 0;
            var sy = 0;
            var dx = 0;
            var dy = 2;
            var w = 3;
            var h = 2;
            p.Swap(sx, sy, sx + dx, sy + dy, w, h);
            for (var y = sy; y < sy + h; y++)
            {
                for (var x = sx; x < sx + w; x++)
                {
                    Assert.AreEqual(q[x + dx, y + dy], p[x, y], string.Format("1. At ({0}, {1}) for\n{2}", x, y, p));
                    Assert.AreEqual(q[x, y], p[x + dx, y + dy], string.Format("2. At ({0}, {1}) for\n{2}", x, y, p));
                }
            }
        }


        [TestCase]
        public void SwapRect2()
        {
            var before = new int[,] {
{ 1, 2, 3, 4 },
{ 5, 6, 7, 8 } };
            var after = new int[,] {
{ 3, 4, 1, 2 },
{ 7, 8, 5, 6 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            p.Swap(0, 0, 2, 0, 2, 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapRectBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(-1, -1, 1, 2, 2, 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapRectBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(0, 0, p.Width, 0, 2, 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void SwapRectBad3()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            p.Swap(0, 0, 1, 1, 2, 2);
            Assert.AreEqual(q, p);
        }


        [TestCase]
        public void RectsIntersect1()
        {
            Assert.IsTrue(Puzzle.RectsIntersect(0, 0, 1, 2, 3, 5));
        }


        [TestCase]
        public void RectsIntersect2()
        {
            Assert.IsTrue(Puzzle.RectsIntersect(1, 2, 0, 0, 3, 5));
        }


        [TestCase]
        public void RectsDontIntersect1()
        {
            Assert.IsFalse(Puzzle.RectsIntersect(1, 2, 7, 11, 3, 5));
        }


        [TestCase]
        public void RectsDontIntersect2()
        {
            Assert.IsFalse(Puzzle.RectsIntersect(7, 11, 1, 2, 3, 5));
        }


        [TestCase]
        public void RectsDontIntersect3()
        {
            Assert.IsFalse(Puzzle.RectsIntersect(1, 2, 4, 2, 3, 5));
        }

        [TestCase]
        public void Rotate1()
        {
            var p = new Puzzle(testGrid);
            var q = p.Rotate(Puzzle.Clockwise);
            Assert.AreEqual(p.Width, q.Height);
            Assert.AreEqual(p.Height, q.Width);
        }


        [TestCase]
        public void Rotate2()
        {
            var p = new Puzzle(testGrid);
            var q = p.Rotate(Puzzle.CounterClockwise);
            Assert.AreEqual(p.Width, q.Height);
            Assert.AreEqual(p.Height, q.Width);
        }


        [TestCase]
        public void Rotate3()
        {
            var p = new Puzzle(testGrid);
            var q = p.Rotate(Puzzle.Clockwise).Rotate(Puzzle.Clockwise);
            Assert.AreEqual(p.Width, q.Width);
            Assert.AreEqual(p.Height, q.Height);
        }


        [TestCase]
        public void Rotate4()
        {
            var p = new Puzzle(testGrid);
            var q = p.Rotate(Puzzle.CounterClockwise).Rotate(Puzzle.CounterClockwise);
            Assert.AreEqual(p.Width, q.Width);
            Assert.AreEqual(p.Height, q.Height);
        }


        [TestCase]
        public void Rotate5()
        {
            var p = new Puzzle(testGrid);
            var q = p.Rotate(Puzzle.Clockwise).Rotate(Puzzle.Clockwise).Rotate(Puzzle.Clockwise);
            Assert.AreEqual(p.Width, q.Height);
            Assert.AreEqual(p.Height, q.Width);
        }


        [TestCase]
        public void Rotate6()
        {
            var p = new Puzzle(testGrid);
            var q = p.Rotate(Puzzle.CounterClockwise).Rotate(Puzzle.CounterClockwise).Rotate(Puzzle.CounterClockwise);
            Assert.AreEqual(p.Width, q.Height);
            Assert.AreEqual(p.Height, q.Width);
        }


        [TestCase]
        public void Rotate7()
        {
            var p = new Puzzle(testGrid);
            var q = p.Rotate(Puzzle.Clockwise).Rotate(Puzzle.Clockwise).Rotate(Puzzle.Clockwise).Rotate(Puzzle.Clockwise);
            Assert.AreEqual(p, q);
        }


        [TestCase]
        public void Rotate8()
        {
            var p = new Puzzle(testGrid);
            var q = p.Rotate(Puzzle.CounterClockwise).Rotate(Puzzle.CounterClockwise).Rotate(Puzzle.CounterClockwise).Rotate(Puzzle.CounterClockwise);
            Assert.AreEqual(p, q);
        }


        [TestCase]
        public void Rotate9()
        {
            var before = new int[,] {
{ 1, 2 },
{ 3, 5 } };
            var after = new int[,] {
{ 3, 1 },
{ 5, 2 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            Assert.AreEqual(q, p.Rotate(Puzzle.Clockwise));
        }



        [TestCase]
        public void Rotate10()
        {
            var before = new int[,] {
{ 1, 2 },
{ 3, 5 } };
            var after = new int[,] {
{ 2, 5 },
{ 1, 3 } };
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            Assert.AreEqual(q, p.Rotate(Puzzle.CounterClockwise));
        }


        [TestCase]
        public void Rotate11()
        {
            var before = new int[,] {
{ 1, 2, 3 },
{ 5, 7, 11 } };
            var after = new int[,] {
{ 5, 1 },
{ 7, 2 },
{11, 3}};
            var p = new Puzzle(before);
            var q = new Puzzle(after);
            Assert.AreEqual(q, p.Rotate(Puzzle.Clockwise));
        }


        [TestCase]
        public void Duplicate()
        {
            var p = new Puzzle(testGrid);
            var q = p.Duplicate();
            Assert.AreNotSame(p, q);
            Assert.AreEqual(p, q);
        }


        [TestCase]
        public void IsFullGrid1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsFull());
        }


        [TestCase]
        public void IsFullGrid2()
        {
            var puz = new Puzzle(testGrid);
            puz[0, 0] = -1;
            Assert.IsFalse(puz.IsFull());
        }


        [TestCase]
        public void IsFullGrid3()
        {
            var puz = new Puzzle(11, 13);
            Assert.IsFalse(puz.IsFull());
        }


        [TestCase]
        public void IsFullRow1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsFull(Puzzle.RowOrder, 2));
        }


        [TestCase]
        public void IsFullRow2()
        {
            var puz = new Puzzle(testGrid);
            puz[1, 2] = Puzzle.EmptyTile;
            Assert.IsFalse(puz.IsFull(Puzzle.RowOrder, 2));
        }


        [TestCase]
        public void IsFullRow3()
        {
            var puz = new Puzzle(3, 5);
            Assert.IsFalse(puz.IsFull(Puzzle.RowOrder, 2));
        }

        [TestCase]
        public void IsFullRowBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            Assert.IsFalse(p.IsFull(Puzzle.RowOrder, -1));
        }


        [TestCase]
        public void IsFullRowBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            Assert.IsFalse(p.IsFull(Puzzle.RowOrder, p.Height));
        }



        [TestCase]
        public void IsFullColumn1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsFull(Puzzle.ColumnOrder, 2));
        }


        [TestCase]
        public void IsFullColumn2()
        {
            var puz = new Puzzle(testGrid);
            puz[2, 1] = Puzzle.EmptyTile;
            Assert.IsFalse(puz.IsFull(Puzzle.ColumnOrder, 2));
        }


        [TestCase]
        public void IsFullColumn3()
        {
            var puz = new Puzzle(3, 5);
            Assert.IsFalse(puz.IsFull(Puzzle.ColumnOrder, 2));
        }

        [TestCase]
        public void IsFullColumnBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            Assert.IsFalse(p.IsFull(Puzzle.ColumnOrder, -1));
        }


        [TestCase]
        public void IsFullColumnBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            Assert.IsFalse(p.IsFull(Puzzle.ColumnOrder, p.Width));
        }


        [TestCase]
        public void IsFullRect()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsFull(1, 1, 2, 2));
        }

        [TestCase]
        public void IsFullRectBad1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, -2, 2, 2));
        }


        [TestCase]
        public void IsFullRectBad2()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, 0, 2, 2));
        }


        [TestCase]
        public void IsFullRectBad3()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, puz.Height, 2, 2));
        }


        [TestCase]
        public void IsFullRectBad4()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(0, -2, 2, 2));
        }


        [TestCase]
        public void IsFullRectBad5()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(0, puz.Height, 2, 2));
        }


        [TestCase]
        public void IsFullRectBad6()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, -2, 2, 2));
        }


        [TestCase]
        public void IsFullRectBad7()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, 0, 2, 2));
        }


        [TestCase]
        public void IsFullRectBad8()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, puz.Height, 2, 2));
        }

        [TestCase]
        public void IsFullShape()
        {
            var puz = new Puzzle(testGrid);
            var shape = new int[,] { { 1, 2, -1 }, { -1, 3, 4 } };
            Assert.IsTrue(puz.IsFull(0, 0, shape));
        }


        [TestCase]
        public void IsFullShapeBad1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsFullShapeBad2()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, 0, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsFullShapeBad3()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsFullShapeBad4()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(0, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsFullShapeBad5()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(0, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsFullShapeBad6()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsFullShapeBad7()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, 0, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsFullShapeBad8()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }

        [TestCase]
        public void IsFullShapeBad9()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsFull(0, 0, (int[,])null));
        }


        [TestCase]
        public void IsFullShapeBad10()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsFull(0, 0, new int[0, 0]));
        }


        [TestCase]
        public void IsFullPuzzle()
        {
            var puz = new Puzzle(testGrid);
            var shape = new Puzzle(new int[,] { { 1, 2, -1 }, { -1, 3, 5 } });
            Assert.IsTrue(puz.IsFull(0, 0, shape));
        }


        [TestCase]
        public void IsFullPuzzleBad1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsFullPuzzleBad2()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, 0, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsFullPuzzleBad3()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(-2, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsFullPuzzleBad4()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(0, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsFullPuzzleBad5()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(0, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsFullPuzzleBad6()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsFullPuzzleBad7()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, 0, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsFullPuzzleBad8()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsFull(puz.Width, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }

        [TestCase]
        public void IsFullPuzzleBad9()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsFull(0, 0, (Puzzle)null));
        }



        [TestCase]
        public void IsEmptyGrid1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsEmpty());
        }


        [TestCase]
        public void IsEmptyGrid2()
        {
            var puz = new Puzzle(testGrid);
            puz[0, 0] = -1;
            Assert.IsFalse(puz.IsEmpty());
        }


        [TestCase]
        public void IsEmptyGrid3()
        {
            var puz = new Puzzle(11, 13);
            Assert.IsTrue(puz.IsEmpty());
        }


        [TestCase]
        public void IsEmptyRow1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsEmpty(Puzzle.RowOrder, 2));
        }


        [TestCase]
        public void IsEmptyRow2()
        {
            var puz = new Puzzle(testGrid);
            puz[1, 2] = Puzzle.EmptyTile;
            Assert.IsFalse(puz.IsEmpty(Puzzle.RowOrder, 2));
        }


        [TestCase]
        public void IsEmptyRow3()
        {
            var puz = new Puzzle(3, 5);
            Assert.IsTrue(puz.IsEmpty(Puzzle.RowOrder, 2));
        }

        [TestCase]
        public void IsEmptyRowBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            Assert.IsTrue(p.IsEmpty(Puzzle.RowOrder, -1));
        }


        [TestCase]
        public void IsEmptyRowBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            Assert.IsTrue(p.IsEmpty(Puzzle.RowOrder, p.Height));
        }



        [TestCase]
        public void IsEmptyColumn1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsEmpty(Puzzle.ColumnOrder, 2));
        }


        [TestCase]
        public void IsEmptyColumn2()
        {
            var puz = new Puzzle(testGrid);
            puz[2, 1] = Puzzle.EmptyTile;
            Assert.IsFalse(puz.IsEmpty(Puzzle.ColumnOrder, 2));
        }


        [TestCase]
        public void IsEmptyColumn3()
        {
            var puz = new Puzzle(3, 5);
            Assert.IsTrue(puz.IsEmpty(Puzzle.ColumnOrder, 2));
        }

        [TestCase]
        public void IsEmptyColumnBad1()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            Assert.IsTrue(p.IsEmpty(Puzzle.ColumnOrder, -1));
        }


        [TestCase]
        public void IsEmptyColumnBad2()
        {
            var p = new Puzzle(testGrid);
            var q = new Puzzle(testGrid);
            Assert.IsTrue(p.IsEmpty(Puzzle.ColumnOrder, p.Width));
        }


        [TestCase]
        public void IsEmptyRect1()
        {
            var puz = new Puzzle(3, 5);
            Assert.IsTrue(puz.IsEmpty(1, 1, 2, 2));
        }


        [TestCase]
        public void IsEmptyRect2()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsFalse(puz.IsEmpty(1, 1, 2, 2));
        }


        [TestCase]
        public void IsEmptyRect3()
        {
            var puz = new Puzzle(testGrid);
            puz[1, 1] = Puzzle.EmptyTile;
            Assert.IsFalse(puz.IsEmpty(1, 1, 2, 2));
        }


        [TestCase]
        public void IsEmptyRectBad1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, -2, 2, 2));
        }


        [TestCase]
        public void IsEmptyRectBad2()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, 0, 2, 2));
        }


        [TestCase]
        public void IsEmptyRectBad3()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, puz.Height, 2, 2));
        }


        [TestCase]
        public void IsEmptyRectBad4()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(0, -2, 2, 2));
        }


        [TestCase]
        public void IsEmptyRectBad5()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(0, puz.Height, 2, 2));
        }


        [TestCase]
        public void IsEmptyRectBad6()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, -2, 2, 2));
        }


        [TestCase]
        public void IsEmptyRectBad7()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, 0, 2, 2));
        }


        [TestCase]
        public void IsEmptyRectBad8()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, puz.Height, 2, 2));
        }

        [TestCase]
        public void IsEmptyShape()
        {
            var puz = new Puzzle(testGrid);
            var shape = new int[,] { { 1, 2, -1 }, { -1, 3, 4 } };
            Assert.IsFalse(puz.IsEmpty(0, 0, shape));
        }


        [TestCase]
        public void IsEmptyShapeBad1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsEmptyShapeBad2()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, 0, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsEmptyShapeBad3()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsEmptyShapeBad4()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(0, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsEmptyShapeBad5()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(0, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsEmptyShapeBad6()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, -2, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsEmptyShapeBad7()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, 0, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }


        [TestCase]
        public void IsEmptyShapeBad8()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, puz.Height, new int[2, 2] { { 1, -1 }, { 2, 3 } }));
        }

        [TestCase]
        public void IsEmptyShapeBad9()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsEmpty(0, 0, (int[,])null));
        }


        [TestCase]
        public void IsEmptyShapeBad10()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsEmpty(0, 0, new int[0, 0]));
        }


        [TestCase]
        public void IsEmptyPuzzle()
        {
            var puz = new Puzzle(testGrid);
            var shape = new Puzzle(new int[,] { { 1, 2, -1 }, { -1, 3, 5 } });
            Assert.IsFalse(puz.IsEmpty(0, 0, shape));
        }


        [TestCase]
        public void IsEmptyPuzzleBad1()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsEmptyPuzzleBad2()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, 0, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsEmptyPuzzleBad3()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(-2, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsEmptyPuzzleBad4()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(0, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsEmptyPuzzleBad5()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(0, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsEmptyPuzzleBad6()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, -2, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsEmptyPuzzleBad7()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, 0, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }


        [TestCase]
        public void IsEmptyPuzzleBad8()
        {
            var puz = new Puzzle(testGrid);
            Assert.IsTrue(puz.IsEmpty(puz.Width, puz.Height, new Puzzle(new int[2, 2] { { 1, -1 }, { 2, 2 } })));
        }

        [TestCase]
        public void IsEmptyPuzzleBad9()
        {
            var p = new Puzzle(testGrid);
            Assert.IsFalse(p.IsEmpty(0, 0, (Puzzle)null));
        }

    }
}