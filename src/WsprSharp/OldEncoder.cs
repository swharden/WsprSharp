using System;
using System.Linq;

public class OldEncoder
{
    /// <summary>
    /// Number of bits in a WSPR transmission
    /// </summary>
    public const byte WSPR_BIT_COUNT = 162;

    /// <summary>
    /// Total number of bytes in a WSPR transmission
    /// </summary>
    public const byte WSPR_MESSAGE_SIZE = 11;

    /// <summary>
    /// Convert station information into a 7-byte array 
    /// using JTEncode's WSPR bit packing algorythm
    /// </summary>
    /// <param name="callsign">callsign of any length (will get padded automatically)</param>
    /// <param name="location">4-character location (grid square identifier)</param>
    /// <param name="power">power level in dB (will get rounded automatically)</param>
    /// <returns></returns>
    public byte[] GetMessageBytes(string callsign, string location, double power)
    {
        // sanitize inputs and perform error checking
        callsign = SanitizeCallsign(callsign);
        location = SanitizeLocation(location);
        byte powerByte = SanitizePower(power);

        // pack callsign data into a 32-bit integer
        char[] callChars = callsign.ToCharArray();
        uint intA;
        intA = WsprCode(callChars[0]);
        intA = intA * 36 + WsprCode(callChars[1]);
        intA = intA * 10 + WsprCode(callChars[2]);
        intA = (uint)(intA * 27 + (WsprCode(callChars[3]) - 10));
        intA = (uint)(intA * 27 + (WsprCode(callChars[4]) - 10));
        intA = (uint)(intA * 27 + (WsprCode(callChars[5]) - 10));

        // pack location and power into a 32-bit integer
        char[] locChars = location.ToCharArray();
        uint intB;
        intB = (uint)(
        (
            179 - 
            10 * (locChars[0] - 'A') - (locChars[2] - '0')) * 180 +
            10 * (locChars[1] - 'A') + (locChars[3] - '0')
        );
        intB = (intB * 128) + powerByte + 64;

        // translate the two integers into a 7-byte array
        byte[] bytes = new byte[7];
        bytes[3] = ((byte)((intA & 0xF) << 4));
        intA >>= 4;
        bytes[2] = ((byte)(intA & 0xFF));
        intA >>= 8;
        bytes[1] = ((byte)(intA & 0xFF));
        intA >>= 8;
        bytes[0] = ((byte)(intA & 0xFF));
        bytes[6] = ((byte)((intB & 0x3) << 6));
        intB >>= 2;
        bytes[5] = ((byte)(intB & 0xFF));
        intB >>= 8;
        bytes[4] = ((byte)(intB & 0xFF));
        intB >>= 8;
        bytes[3] = (byte)(bytes[3] | (intB & 0xF));
        return bytes;
    }

    /// <summary>
    /// Clean-up a callsign in preparation for WSPR encoding.
    /// Throw an exception if it is not in an expected format.
    /// </summary>
    public string SanitizeCallsign(string callsign)
    {
        if (callsign is null)
            throw new ArgumentException("callsign can not be null");

        // All characters must be uppercase
        callsign = callsign.ToUpper();

        // If only the 2nd character is a digit, then left-pad with a space.
        bool secondCharacterIsNumeric = char.IsNumber(callsign.ToCharArray()[1]);
        if (secondCharacterIsNumeric)
            callsign = " " + callsign;

        // Right-pad with several spaces to ensure callsign has at least 6 characters
        callsign += "      ";

        // Limit the callsign to exactly 6 characters.
        callsign = callsign.Substring(0, 6);

        return callsign;
    }

    /// <summary>
    /// Clean-up a 4-character location in preparation for WSPR encoding.
    /// Throw an exception if it is not in an expected format.
    /// </summary>
    public string SanitizeLocation(string location)
    {
        if (location is null)
            throw new ArgumentException("location can not be null");

        // All characters must be uppercase
        location = location.ToUpper();

        // Location must be exactly four characters long
        if (location.Length != 4)
            throw new ArgumentException("Location must be exactly four characters long");

        // First two characters must be A thru R
        foreach (char letter in location.ToCharArray().Take(2))
            if (letter < 'A' || letter > 'R')
                throw new ArgumentException("First two location characters must be A-R");

        // Last two characters must be 0-9
        foreach (char letter in location.ToCharArray().Skip(2).Take(2))
            if (letter < '0' || letter > '9')
                throw new ArgumentException("Last two location characters must be 0-9");

        return location;
    }

    /// <summary>
    /// Sanitize a power level in preparation for WSPR encoding.
    /// Only certain power levels are supported by the WSPR protocol.
    /// </summary>
    public byte SanitizePower(double power)
    {
        if (double.IsNaN(power) || double.IsInfinity(power))
            throw new ArgumentException("power must me finite");

        byte powerLevel = 0;
        byte[] validPowerLevels = { 0, 3, 7, 10, 13, 17, 20, 23, 27, 30, 33, 37, 40, 43, 47, 50, 53, 57, 60 };
        foreach (byte validPowerLevel in validPowerLevels)
        {
            if (power >= validPowerLevel)
                powerLevel = validPowerLevel;
        }

        return powerLevel;
    }

    /// <summary>
    /// Convolve a byte array using JTEncode's convolution method
    /// </summary>
    public byte[] Convolve(byte[] data, int bitCount = WSPR_BIT_COUNT, int messageSize = WSPR_MESSAGE_SIZE)
    {
        byte[] paddedInput = new byte[messageSize];
        Array.Copy(data, 0, paddedInput, 0, data.Length);

        byte[] output = new byte[bitCount];
        UInt32 reg0 = 0;
        UInt32 reg1 = 0;
        byte inputBit;
        byte parityBit;
        byte bitIndex = 0;
        byte i, j, k;

        for (i = 0; i < WSPR_MESSAGE_SIZE; i++)
        {
            for (j = 0; j < 8; j++)
            {
                // Set input bit according the MSB of current element
                inputBit = (byte)((((paddedInput[i] << j) & 0x80) == 0x80) ? 1 : 0);

                // Shift both registers and put in the new input bit
                reg0 <<= 1;
                reg1 <<= 1;
                reg0 |= inputBit;
                reg1 |= inputBit;

                // AND Register 0 with feedback taps, calculate parity
                UInt32 regTemp = reg0 & 0xf2d05351;
                parityBit = 0;
                for (k = 0; k < 32; k++)
                {
                    parityBit = (byte)(parityBit ^ (regTemp & 0x01));
                    regTemp >>= 1;
                }
                output[bitIndex] = parityBit;
                bitIndex++;

                // AND Register 1 with feedback taps, calculate parity
                regTemp = reg1 & 0xe4613c47;
                parityBit = 0;
                for (k = 0; k < 32; k++)
                {
                    parityBit = (byte)(parityBit ^ (regTemp & 0x01));
                    regTemp >>= 1;
                }

                output[bitIndex] = parityBit;
                bitIndex++;
                if (bitIndex >= bitCount)
                    break;
            }
        }

        return output;
    }

    /// <summary>
    /// Interleave a byte array according to JTEncode's WSPR standard
    /// </summary>
    public byte[] Interleave(byte[] data)
    {
        byte[] d = new byte[WSPR_BIT_COUNT];
        byte rev, j2, j, k;
        byte i = 0;

        for (j = 0; j < 255; j++)
        {
            j2 = j;
            rev = 0;

            for (k = 0; k < 8; k++)
            {
                if ((j2 & 0x01) > 0)
                    rev = (byte)(rev | (1 << (7 - k)));
                j2 >>= 1;
            }

            if (rev < WSPR_BIT_COUNT)
            {
                d[rev] = data[i];
                i++;
            }

            if (i >= WSPR_BIT_COUNT)
                break;
        }

        return d;
    }

    /// <summary>
    /// Combine data with a standard synchronization array that has good auto-correlation properties.
    /// </summary>
    public byte[] IntegrateSyncValues(byte[] data)
    {
        byte[] sync =
        {
             1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0,
             1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0,
             0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1,
             0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0,
             1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1,
             0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 0, 1,
             1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0,
             1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0
        };

        if (data.Length != sync.Length)
            throw new ArgumentException($"Input data must be the same size as the sync array ({sync.Length} elements)");

        return Enumerable
            .Range(0, WSPR_BIT_COUNT)
            .Select(i => data[i] * 2 + sync[i])
            .Select(x => (byte)x)
            .ToArray();
    }

    /// <summary>
    /// Encode a character as a number according to JTEncode's standard
    /// </summary>
    public byte WsprCode(char c)
    {
        if (char.IsDigit(c))
            return (byte)(c - 48);
        else if (c == ' ')
            return 36;
        else if (c >= 'A' && c <= 'Z')
            return (byte)(c - 55);
        else
            throw new InvalidOperationException($"character {c} is not allowed");
    }
}
