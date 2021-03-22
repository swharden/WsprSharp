using System;

namespace WsprSharp
{
    public class WsprTransmission
    {
        public readonly string Callsign;
        public readonly string Location;
        public readonly int Power;
        public readonly byte[] Message;
        public readonly byte[] Levels;

        /// <summary>
        /// Create a new single-packet WSPR transmission
        /// </summary>
        /// <param name="callsign">standard length (6 character max) callsign</param>
        /// <param name="location">4-character grid square</param>
        /// <param name="power">transmission power (dB units)</param>
        public WsprTransmission(string callsign, string location, double power)
        {
            Callsign = Encode.SanitizeCallsign(callsign);
            Location = Encode.SanitizeLocation(location);
            Power = Encode.SanitizePower(power);

            Message = Encode.GetMessageBytes(callsign, location, power);
            byte[] convolved = Encode.Convolve(Message);
            byte[] interleaved = Encode.Interleave(convolved);
            Levels = Encode.IntegrateSyncValues(interleaved);
        }
    }
}
