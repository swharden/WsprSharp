using NUnit.Framework;

namespace WsprSharp.Tests;

public class EncodeTests
{
    [Test]
    public void Test_OldEcoder_KnownValues()
    {
        byte[] message = Encode.GetMessageBytes(" W8ZLW", "AA00", 3);
        byte[] convolved = Encode.Convolve(message);
        byte[] interleaved = Encode.Interleave(convolved);
        byte[] levels = Encode.IntegrateSyncValues(interleaved);

        byte[] expectedLevels = new byte[]
        {
            3,1,2,2,0,0,0,0,1,0,0,0,3,3,3,2,2,2,1,0,0,1,0,3,3,3,1,0,2,2,
            2,0,0,0,3,2,2,1,2,3,2,2,2,0,0,0,3,2,3,3,2,2,3,1,0,1,2,0,2,3,
            1,0,1,2,2,0,2,1,3,2,1,0,1,0,1,2,1,0,0,1,0,2,3,2,1,1,2,0,0,3,
            3,2,1,2,3,0,2,0,3,2,2,2,2,0,1,2,2,3,0,2,3,1,3,0,3,1,2,0,1,3,
            0,1,0,0,0,1,3,3,2,0,0,0,0,1,0,1,0,0,3,3,2,2,0,0,2,2,2,1,1,2,
            3,0,1,1,2,0,2,3,3,0,2,0
        };

        Assert.AreEqual(expectedLevels, levels);
    }

    [Test]
    public void Test_WsprTransmission_KnownValues()
    {
        WsprTransmission wspr = new("W8ZLW", "AA00", 3.1);

        byte[] expectedLevels = new byte[]
        {
            3,1,2,2,0,0,0,0,1,0,0,0,3,3,3,2,2,2,1,0,0,1,0,3,3,3,1,0,2,2,
            2,0,0,0,3,2,2,1,2,3,2,2,2,0,0,0,3,2,3,3,2,2,3,1,0,1,2,0,2,3,
            1,0,1,2,2,0,2,1,3,2,1,0,1,0,1,2,1,0,0,1,0,2,3,2,1,1,2,0,0,3,
            3,2,1,2,3,0,2,0,3,2,2,2,2,0,1,2,2,3,0,2,3,1,3,0,3,1,2,0,1,3,
            0,1,0,0,0,1,3,3,2,0,0,0,0,1,0,1,0,0,3,3,2,2,0,0,2,2,2,1,1,2,
            3,0,1,1,2,0,2,3,3,0,2,0
        };

        Assert.AreEqual(expectedLevels, wspr.Levels);
    }
}