using NUnit.Framework;
using System;

namespace WsprSharp.Tests;
internal class QuickstartTests
{
    [Test]
    public void Test_Quickstart_Demo()
    {
        WsprTransmission wspr = new("AJ4VD", "EL89", 3);

        Console.WriteLine("Encoded Bytes:");
        Console.WriteLine(string.Join(" ", wspr.Message));

        Console.WriteLine("Transmission Frequency:");
        Console.WriteLine(string.Join(" ", wspr.Levels));
    }
}
