/* 
 * Code in this file started as VB code code translated to C#
 * using https://converter.telerik.com/ and is being refactored 
 * into idiomatic C# by Scott Harden
 * 
 */
using System;
using System.Linq;

public class OldEncoder
{
    public const byte WSPR_SYMBOL_COUNT = 162;
    public const byte WSPR_BIT_COUNT = 162;
    public const byte WSPR_MESSAGE_SIZE = 11;

    public byte[] symbols = new byte[162];

    private readonly byte[] SyncVector = new byte[] {
        1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 0, 0,
        0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0,
        0, 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0,
        0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1,
        0, 0, 1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0,
        0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0 };

    /// <summary>
    /// Encode a message into a 7-byte array
    /// </summary>
    public byte[] GetMessageBytes(string callsign, string location, byte power)
    {
        UInt32 intA;
        UInt32 intB;

        char[] callChars = callsign.ToCharArray();

        intA = GetCharacterCode(callChars[0]);

        intA = intA * 36 + GetCharacterCode(callChars[1]);
        intA = intA * 10 + GetCharacterCode(callChars[2]);
        intA = (uint)(intA * 27 + (GetCharacterCode(callChars[3]) - 10));
        intA = (uint)(intA * 27 + (GetCharacterCode(callChars[4]) - 10));
        intA = (uint)(intA * 27 + (GetCharacterCode(callChars[5]) - 10));

        char[] locChars = location.ToCharArray();
        intB = (uint)((179 - 10 * (locChars[0] - 'A') - (locChars[2] - '0')) * 180 +
                          10 * (locChars[1] - 'A') + (locChars[3] - '0'));
        intB = (intB * 128) + power + 64;

        byte[] messageBytes = new byte[7];
        messageBytes[3] = ((byte)((intA & 0xF) << 4));
        intA >>= 4;
        messageBytes[2] = ((byte)(intA & 0xFF));
        intA >>= 8;
        messageBytes[1] = ((byte)(intA & 0xFF));
        intA >>= 8;
        messageBytes[0] = ((byte)(intA & 0xFF));
        messageBytes[6] = ((byte)((intB & 0x3) << 6));
        intB >>= 2;
        messageBytes[5] = ((byte)(intB & 0xFF));
        intB >>= 8;
        messageBytes[4] = ((byte)(intB & 0xFF));
        intB >>= 8;
        messageBytes[3] = (byte)(messageBytes[3] | (intB & 0xF));
        return messageBytes;
    }

    public byte[] ConvolveMessage(byte[] messageBytes)
    {
        // the input message is only 7 bytes but convolution expects 11 bytes
        byte[] c = new byte[WSPR_MESSAGE_SIZE];
        Array.Copy(messageBytes, 0, c, 0, messageBytes.Length);

        byte[] s = new byte[162];
        UInt32 reg_0 = 0;
        UInt32 reg_1 = 0;
        byte input_bit;
        byte parity_bit;
        byte bit_count = 0;
        byte i, j, k;
        byte bit_size = 162;

        for (i = 0; i < WSPR_MESSAGE_SIZE; i++)
        {
            for (j = 0; j < 8; j++)
            {
                // Set input bit according the MSB of current element
                input_bit = (byte)((((c[i] << j) & 0x80) == 0x80) ? 1 : 0);

                // Shift both registers and put in the new input bit
                reg_0 <<= 1;
                reg_1 <<= 1;
                reg_0 |= input_bit;
                reg_1 |= input_bit;

                // AND Register 0 with feedback taps, calculate parity
                UInt32 reg_temp = reg_0 & 0xf2d05351;
                parity_bit = 0;
                for (k = 0; k < 32; k++)
                {
                    parity_bit = (byte)(parity_bit ^ (reg_temp & 0x01));
                    reg_temp >>= 1;
                }
                s[bit_count] = parity_bit;
                bit_count++;

                // AND Register 1 with feedback taps, calculate parity
                reg_temp = reg_1 & 0xe4613c47;
                parity_bit = 0;
                for (k = 0; k < 32; k++)
                {
                    parity_bit = (byte)(parity_bit ^ (reg_temp & 0x01));
                    reg_temp >>= 1;
                }

                s[bit_count] = parity_bit;
                bit_count++;
                if (bit_count >= bit_size)
                {
                    break;
                }
            }
        }
        return s;
    }

    public byte[] Interleave(byte[] s)
    {
        byte[] d = new byte[162];

        byte rev, index_temp, i, j, k;

        i = 0;

        for (j = 0; j < 255; j++)
        {
            // Bit reverse the index
            index_temp = j;
            rev = 0;

            for (k = 0; k < 8; k++)
            {
                if ((index_temp & 0x01) > 0)
                {
                    rev = (byte)(rev | (1 << (7 - k)));
                }
                index_temp = (byte)(index_temp >> 1);
            }

            if (rev < WSPR_BIT_COUNT)
            {
                d[rev] = s[i];
                i++;
            }

            if (i >= WSPR_BIT_COUNT)
            {
                break;
            }
        }

        return d;
    }

    public byte[] MergeSyncVector(byte[] bytes)
    {
        byte[] standardSyncBytes =
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

        byte[] merged = new byte[162];
        for (int i = 0; i < 162; i++)
            merged[i] = (byte)(standardSyncBytes[i] + (2 * bytes[i]));

        return merged;
    }

    public byte GetCharacterCode(char c)
    {
        bool isdigit;
        byte result = 255;

        isdigit = false;

        if (c >= '0')
        {
            if (c <= '9')
                isdigit = true;
        }

        if (isdigit == true)
        {
            result = (byte)(c - 48);
        }
        else
        {
            if (c == ' ')
                result = 36;

            if (c >= 'A')
            {
                if (c <= 'Z')
                    result = (byte)(c - 55);
            }
        }

        return result;
    }
}
