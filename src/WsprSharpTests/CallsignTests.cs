using FluentAssertions;
using NUnit.Framework;

namespace WsprSharp.Tests;

public class CallsignTests
{
    [Test]
    public void Test_Callsign_Formatting()
    {
        Encode.SanitizeCallsign("ab0xyz").Should().BeEquivalentTo("AB0XYZ");
        Encode.SanitizeCallsign("a0xyz").Should().BeEquivalentTo(" A0XYZ");
        Encode.SanitizeCallsign("a0xy").Should().BeEquivalentTo(" A0XY ");
    }
}