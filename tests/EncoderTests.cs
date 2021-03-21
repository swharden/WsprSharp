using NUnit.Framework;
using System;
using System.Linq;
using System.Text;

namespace WsprSharp.Tests
{
    public class EncoderTests
    {
        [Test]
        public void Test_OldEcoder_KnownValues()
        {
            var encoder = new OldEncoder();
            byte[] message = encoder.GetMessageBytes(" W8ZLW", "AA00", 3);
            byte[] convolvedMessage = encoder.Convolve(message);
            byte[] dataSymbols = encoder.Interleave(convolvedMessage);
            byte[] channelSymbols = encoder.IntegrateSyncValues(dataSymbols);
            byte[] knownChannelSymbols = new byte[]
            {
                3,1,2,2,0,0,0,0,1,0,0,0,3,3,3,2,2,2,1,0,0,1,0,3,3,3,1,0,2,2,
                2,0,0,0,3,2,2,1,2,3,2,2,2,0,0,0,3,2,3,3,2,2,3,1,0,1,2,0,2,3,
                1,0,1,2,2,0,2,1,3,2,1,0,1,0,1,2,1,0,0,1,0,2,3,2,1,1,2,0,0,3,
                3,2,1,2,3,0,2,0,3,2,2,2,2,0,1,2,2,3,0,2,3,1,3,0,3,1,2,0,1,3,
                0,1,0,0,0,1,3,3,2,0,0,0,0,1,0,1,0,0,3,3,2,2,0,0,2,2,2,1,1,2,
                3,0,1,1,2,0,2,3,3,0,2,0
            };
            Assert.AreEqual(knownChannelSymbols, channelSymbols);
        }
    }
}