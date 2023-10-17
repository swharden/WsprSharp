using FluentAssertions;
using NUnit.Framework;
using System;

namespace WsprSharp.Tests;

internal class ValidationTests
{
    [Test]
    public void Test_Callsign_Validation()
    {
        FluentActions.Invoking(() => Encode.SanitizeCallsign("A1B")).Should().NotThrow();
        FluentActions.Invoking(() => Encode.SanitizeCallsign(null)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeCallsign("")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeCallsign("ASDF")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeCallsign("A12F")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeCallsign("1AB")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeCallsign("AB1")).Should().Throw<ArgumentException>();
    }

    [Test]
    public void Test_Location_Validation()
    {
        FluentActions.Invoking(() => Encode.SanitizeLocation("EL89")).Should().NotThrow();
        FluentActions.Invoking(() => Encode.SanitizeLocation(null)).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeLocation("")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeLocation("EL8")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeLocation("L89")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeLocation("E189")).Should().Throw<ArgumentException>();
        FluentActions.Invoking(() => Encode.SanitizeLocation("EL899")).Should().Throw<ArgumentException>();
    }
}
