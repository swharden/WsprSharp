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
        public readonly string MessageString;

        public readonly byte[] Levels;
        public readonly string LevelsString = string.Empty;
        public bool IsValid => string.IsNullOrEmpty(ErrorMessage);
        public readonly string ErrorMessage;
        public readonly string ErrorMessageDetails = string.Empty;

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
                ErrorMessage = string.Empty;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                ErrorMessageDetails = ex.ToString();
                Message = Array.Empty<byte>();
                Levels = Array.Empty<byte>();
                return;
            }

            Message = Encode.GetMessageBytes(callsign, location, power);
            MessageString = string.Join(" ", Message.Select(x => x.ToString("X2")));
            byte[] convolved = Encode.Convolve(Message);
            byte[] interleaved = Encode.Interleave(convolved);
            Levels = Encode.IntegrateSyncValues(interleaved);
            LevelsString = GetLevelsString(", ");
        }

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
