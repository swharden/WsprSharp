using NUnit.Framework;

namespace WsprSharp.Tests
{
    public class EncoderTests
    {
        [Test]
        public void Test_Ecode_KnownOutput()
        {
            byte[] expectedMessage = { 0xF9, 0x72, 0xF2, 0x8F, 0xBB, 0x90, 0xC0 };
            byte[] message = WsprEncode.StandardMessage("W8ZLW", "AA00", 3);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}