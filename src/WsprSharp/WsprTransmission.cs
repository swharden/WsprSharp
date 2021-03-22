using System;
using System.Linq;
using System.Text;

namespace WsprSharp
{
    public class WsprTransmission
    {
        public readonly string Callsign;
        public readonly string Location;
        public readonly int Power;
        public readonly byte[] Message;
        public readonly byte[] Levels;
        public readonly bool IsValid;

        /// <summary>
        /// Create a new single-packet WSPR transmission
        /// </summary>
        /// <param name="callsign">standard length (6 character max) callsign</param>
        /// <param name="location">4-character grid square</param>
        /// <param name="power">transmission power (dB units)</param>
        public WsprTransmission(string callsign, string location, double power)
        {
            try
            {
                Callsign = Encode.SanitizeCallsign(callsign);
                Location = Encode.SanitizeLocation(location);
                Power = Encode.SanitizePower(power);
                IsValid = true;
            }
            catch
            {
                IsValid = false;
            }

            if (IsValid)
            {
                Message = Encode.GetMessageBytes(callsign, location, power);
                byte[] convolved = Encode.Convolve(Message);
                byte[] interleaved = Encode.Interleave(convolved);
                Levels = Encode.IntegrateSyncValues(interleaved);
            }
        }

        public string GetMessageString(string separator = " ") =>
            string.Join(separator, Message.Select(x => x.ToString("X2")));

        public string GetLevelsString(string separator = ",", int valuesPerLine = 30)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Levels.Length; i++)
            {
                sb.Append(Levels[i]);
                sb.Append(separator);
                if (i % valuesPerLine == valuesPerLine - 1)
                    sb.Append(Environment.NewLine);
            }
            return sb.ToString().Trim(new char[] { '\r', '\n', ',' });
        }
    }
}
