using System;

namespace WsprSharp
{
    public static class WsprEncode
    {
        /// <summary>
        /// Create a standard WSPR packet: callsign + 4-character locator + power level
        /// </summary>
        /// <param name="callsign">callsign (max ??? characters)</param>
        /// <param name="locator">4 character grid square</param>
        /// <param name="txPower">transmission power (dBm)</param>
        public static byte[] StandardMessage(string callsign, string locator, double txPower)
        {
            byte[] message = { 0xF9, 0x72, 0xF2, 0x8F, 0xBB, 0x90, 0xC0 };
            return message;
        }

        /// <summary>
        /// Use a two-transmission sequence to create a packet with a compound callsign.
        /// Transmission 1: compound callsign + power level
        /// transmission 2: hashed callsign + 6-character locator + and power level
        /// </summary>
        public static void CompoundCallsign(string callsign, string locator, double txPower)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Use a two-transmission sequence to create a packet with a six-character locator.
        /// Transmission 1: standard callsign + 4-character locator + power level
        /// Transmission 2: hashed callsign + 6-character locator + power level.
        /// </summary>
        public static void ExtendedLocator(string callsign, string locator, double txPower)
        {
            throw new NotImplementedException();
        }
    }
}
