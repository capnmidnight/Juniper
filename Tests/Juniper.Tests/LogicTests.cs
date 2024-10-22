using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

using static Juniper.Logic.LogicConstructor;

namespace Juniper.Logic;

[TestFixture]
public class LogicTests
{
    [TestCase]
    public void ItemIntTrue()
    {
        var expr = Expr(5);
        Assert.IsTrue(expr.Evaluate((v) => v > 4));
    }

    [TestCase]
    public void ItemIntFalse()
    {
        var expr = Expr(2);
        Assert.IsFalse(expr.Evaluate((v) => v > 4));
    }

    [TestCase]
    public void ItemStringTrue()
    {
        var expr = Expr("Hello, world");
        Assert.IsTrue(expr.Evaluate((v) => v is object));
    }

    [TestCase]
    public void ItemStringFalse()
    {
        var expr = Expr<string>(null);
        Assert.IsFalse(expr.Evaluate((v) => v is object));
    }

    [TestCase]
    public void And2IntTrue()
    {
        var expr1 = Expr(5);
        var expr2 = Expr(7);
        var expr = And(expr1, expr2);
        Assert.IsTrue(expr.Evaluate((v) => v > 4));
    }

    [TestCase]
    public void And2IntFalse()
    {
        var expr1 = Expr(5);
        var expr2 = Expr(3);
        var expr = And(expr1, expr2);
        Assert.IsFalse(expr.Evaluate((v) => v > 4));
    }

    [TestCase]
    public void Complex()
    {
        var expr = Or(
            And(1, 2, 3),
            Not(And(4, 5, 6)),
            Not(7));

        Assert.IsTrue(expr.Evaluate(i => i < 4));
    }
}
