using NUnit.Framework;
using System;
using System.Linq;

namespace WsprSharp.Tests
{
    public class EncoderTests
    {
        private string GetHexString(byte[] bytes, string separator = " ") =>
            string.Join(separator, bytes.Select(x => x.ToString("x2")));

        [Test]
        public void Test_Ecode_KnownOutput()
        {
            byte[] expectedMessage = { 0xF9, 0x72, 0xF2, 0x8F, 0xBB, 0x90, 0xC0 };
            byte[] message = WsprEncode.StandardMessage("W8ZLW", "AA00", 3);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Test_OldEcoder_KnownOutput()
        {
            var encoder = new OldEncoder();
            byte[] messageBytes = encoder.Encode_Message(" W8ZLW ", "AA00", 3);
            string message = GetHexString(messageBytes);

            Assert.AreEqual("f9 72 f2 8f bb 90 c0", message);
        }
    }
}