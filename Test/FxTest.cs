namespace Test;

public class FxTest
{
    [Throws(typeof(ArgumentNullException))]
    [Throws(typeof(FormatException))]
    [Throws(typeof(OverflowException))]
    public void Foo()
    {
        var x = int.Parse("f");
    }

    [Throws(typeof(ArgumentNullException))]
    [Throws(typeof(OverflowException))]
    public void Foo2()
    {
        try
        {
            var x = int.Parse("f");
        }
        catch (FormatException)
        {

        }
    }

    [Throws(typeof(Exception))]
    public void Foo3()
    {
        Foo();
    }
}