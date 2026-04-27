using ukrepeaterlib;

namespace ukrepeaterlib_tests;

public class FrequencyFormattingTests
{
    [Theory]
    [InlineData(145600000L, "145.600")]
    [InlineData(145687500L, "145.6875")]
    [InlineData(433075000L, "433.075")]
    public void ToMHzString_PreservesExpectedPrecision(long frequencyHz, string expected)
    {
        Assert.Equal(expected, frequencyHz.ToMHzString());
    }
}
