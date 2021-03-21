using NUnit.Framework;
using System;
using System.Linq;
using System.Text;

namespace WsprSharp.Tests
{
    public class EncoderTests
    {
        /// <summary>
        /// Return a byte array as a hex string
        /// </summary>
        private string GetHexString(byte[] bytes, string separator = " ") =>
            string.Join(separator, bytes.Select(x => x.ToString("x2")));

        /// <summary>
        /// return a CSV-formatted block of text given a byte array
        /// </summary>
        private string GetSymbolString(byte[] bytes, string separator = ",", int valuesPerLine = 30)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i]);
                sb.Append(separator);
                if (i % valuesPerLine == valuesPerLine - 1)
                    sb.Append("\n");
            }
            return sb.ToString();
        }

        [Test]
        public void Test_OldEcoder_KnownValues()
        {
            var encoder = new OldEncoder();

            // translate callsign, location, and power into a message
            byte[] message = encoder.GetMessageBytes(" W8ZLW", "AA00", 3);
            string knownMessageHex = "f9 72 f2 8f bb 90 c0";
            Assert.AreEqual(knownMessageHex, GetHexString(message));

            // process the message to get data
            byte[] convolvedMessage = encoder.ConvolveMessage(message);
            byte[] dataSymbols = encoder.Interleave(convolvedMessage);
            byte[] knownDataSymbols = new byte[] {
                1,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,1,1,1,0,0,1,1,
                1,0,0,0,1,1,1,0,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,0,0,0,1,0,1,1,
                0,0,0,1,1,0,1,0,1,1,0,0,0,0,0,1,0,0,0,0,0,1,1,1,0,0,1,0,0,1,
                1,1,0,1,1,0,1,0,1,1,1,1,1,0,0,1,1,1,0,1,1,0,1,0,1,0,1,0,0,1,
                0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,1,1,1,0,0,1,
                1,0,0,0,1,0,1,1,1,0,1,0
            };
            Assert.AreEqual(knownDataSymbols, dataSymbols);

            // translate the data into channels
            byte[] channelSymbols = encoder.MergeSyncVector(dataSymbols);
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