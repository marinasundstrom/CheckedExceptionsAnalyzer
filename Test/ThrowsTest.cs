namespace Test;

public class ThrowsTest
{
    [Throws(typeof(InvalidOperationException))]
    public void Foo()
    {
        throw new InvalidOperationException();
    }

    public void Foo2()
    {
        try
        {
            throw new NullReferenceException("Data source is null.");
        }
        catch (NullReferenceException exc)
        {
            throw new InvalidCastException();
        }
        catch (FormatException exc)
        {
            throw;
        }
    }

    public void Foo3()
    {
        try
        {
            Foo();
        }
        catch
        {
            throw;
        }
    }

    public void Foo4()
    {
        try
        {
            Foo();
        }
        catch (InvalidOperationException exc)
        {
            throw new InvalidCastException();
        }
        catch
        {
            throw;
        }
    }
}