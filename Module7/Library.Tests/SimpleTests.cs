namespace Library.Tests;

public class SimpleTests
{
    [Fact]
    public void Pow_ReturnsCorrectSquareNumber()
    {
        var result = Math.Pow(2, 2);
        
        Assert.Equal(4, result);
    }
}