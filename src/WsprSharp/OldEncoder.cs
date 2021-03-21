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

    public byte[] symbols = new byte[163];

    private readonly byte[] Sync_Vector = new byte[] {
        1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 0, 0,
        0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0,
        0, 1, 1, 0, 1, 0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0, 1, 1, 0,
        0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1,
        0, 0, 1, 1, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0,
        0, 0, 1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0 };

    public byte[] Encode_Message(string callsign, string location, int txPower)
    {
        byte dbm = (byte)txPower;

        string callsign_ = callsign;
        string location_ = location;
        byte dbm_ = dbm;

        // Bit packing
        byte[] c = new byte[12];
        bit_packing(ref c, ref callsign_, location_, ref dbm_);

        // Convolutional Encoding (162 symbols for WSPR)
        byte[] s = new byte[163];
        convolve(ref c, ref s);

        // Interleaving
        byte[] d = new byte[163];
        interleave(ref c, ref s, ref d);
        byte[] messageBytes = c.Take(7).ToArray();

        // Merge with sync vector
        for (var i = 0; i <= WSPR_SYMBOL_COUNT - 1; i++)
            symbols[i] = (byte)(Sync_Vector[i] + (2 * s[i]));

        // Print out Sync symbols as formated by the command line wsprcode.exe 
        string Message = "Message:" + callsign_ + " " + location_ + " " + dbm_.ToString();
        Message = Message + Environment.NewLine + Environment.NewLine;

        // Hex encoded message
        string HexMessage = "Source-encoded message (50 bits, hex): ";
        for (var i = 0; i <= 6; i++)
            HexMessage = HexMessage + c[i].ToString("X2") + " ";
        HexMessage = HexMessage + Environment.NewLine + Environment.NewLine;

        // Print out Data Symbols
        string DataSymbols = "Data Symbols:" + Environment.NewLine;
        int CRCount = 1;
        for (var i = 0; i <= WSPR_SYMBOL_COUNT - 1; i++)
        {
            DataSymbols = DataSymbols + d[i] + ",";
            CRCount += 1;
            if (CRCount > 30)
            {
                CRCount = 1;
                DataSymbols = DataSymbols + Environment.NewLine;
            }
        }

        // TODO: confirm this works as expected
        //DataSymbols = Strings.Mid(DataSymbols, 1, DataSymbols.Length - 1);
        DataSymbols = DataSymbols.Substring(1, DataSymbols.Length - 1);
        DataSymbols = DataSymbols + Environment.NewLine + Environment.NewLine;

        // Print out Sync Symbols
        string SyncSymbols = "Sync Symbols:" + Environment.NewLine;
        CRCount = 1;
        for (var i = 0; i <= WSPR_SYMBOL_COUNT - 1; i++)
        {
            SyncSymbols = SyncSymbols + Sync_Vector[i] + ",";
            CRCount += 1;
            if (CRCount > 30)
            {
                CRCount = 1;
                SyncSymbols = SyncSymbols + Environment.NewLine;
            }
        }

        // TODO: confirm this works as expected
        //SyncSymbols = Strings.Mid(SyncSymbols, 1, SyncSymbols.Length - 1);
        SyncSymbols = SyncSymbols.Substring(1, SyncSymbols.Length - 1);
        SyncSymbols = SyncSymbols + Environment.NewLine + Environment.NewLine;

        // Print out Channel Symbols
        var ChannelSymbols = "Channel Symbols:" + Environment.NewLine;
        CRCount = 1;
        for (var i = 0; i <= WSPR_SYMBOL_COUNT - 1; i++)
        {
            ChannelSymbols = ChannelSymbols + symbols[i] + ",";
            CRCount += 1;
            if (CRCount > 30)
            {
                CRCount = 1;
                ChannelSymbols = ChannelSymbols + Environment.NewLine;
            }
        }

        // TODO: confirm this works as expected
        //ChannelSymbols = Strings.Mid(ChannelSymbols, 1, ChannelSymbols.Length - 1);
        ChannelSymbols = ChannelSymbols.Substring(1, ChannelSymbols.Length - 1);

        //Debug.WriteLine(Message + HexMessage + DataSymbols + SyncSymbols + ChannelSymbols);

        return messageBytes;
    }

    public void bit_packing(ref byte[] c, ref string callsign_, string location_, ref byte dbm_)
    {
        UInt32 n = 0;
        UInt32 m = 0;

        char[] callChars = callsign_.ToCharArray();

        n = code(callChars[0]);

        n = n * 36 + code(callChars[1]);
        n = n * 10 + code(callChars[2]);
        n = (uint)(n * 27 + (code(callChars[3]) - 10));
        n = (uint)(n * 27 + (code(callChars[4]) - 10));
        n = (uint)(n * 27 + (code(callChars[5]) - 10));

        char[] locChars = location_.ToCharArray();
        m = (uint)((179 - 10 * (locChars[0] - 'A') - (locChars[2] - '0')) * 180 +
                          10 * (locChars[1] - 'A') + (locChars[3] - '0'));
        m = (m * 128) + dbm_ + 64;

        // Callsign Is 28 bits, locator/power Is 22 bits.
        // A little less work to start with the least-significant bits
        c[3] = ((byte)((n & 0xF) << 4));
        n = n >> 4;
        c[2] = ((byte)(n & 0xFF));
        n = n >> 8;
        c[1] = ((byte)(n & 0xFF));
        n = n >> 8;
        c[0] = ((byte)(n & 0xFF));

        c[6] = ((byte)((m & 0x3) << 6));
        m = m >> 2;
        c[5] = ((byte)(m & 0xFF));
        m = m >> 8;
        c[4] = ((byte)(m & 0xFF));
        m = m >> 8;
        c[3] = (byte)(c[3] | (m & 0xF));
        c[7] = 0;
        c[8] = 0;
        c[9] = 0;
        c[10] = 0;
    }

    public void convolve(ref byte[] c, ref byte[] s)
    {
        byte message_size = WSPR_MESSAGE_SIZE;
        byte bit_size = 162;
        UInt32 reg_0 = 0;
        UInt32 reg_1 = 0;
        UInt32 reg_temp = 0;
        byte input_bit = 0;
        byte parity_bit = 0;
        byte bit_count = 0;

        for (var i = 0; i <= message_size - 1; i++)
        {
            for (var j = 0; j <= 7; j++)
            {
                // Set input bit according the MSB of current element
                input_bit = (byte)(((c[i] << j) & 0x80) == 0x80 ? 1 : 0);

                // Shift both registers and put in the new input bit
                reg_0 = reg_0 << 1;
                reg_1 = reg_1 << 1;
                reg_0 = reg_0 | input_bit;
                reg_1 = reg_1 | input_bit;

                // AND Register 0 with feedback taps, calculate parity
                reg_temp = reg_0 & 0xF2D05351;
                parity_bit = 0;
                for (var k = 0; k <= 31; k++)
                {
                    parity_bit = (byte)(parity_bit ^ (reg_temp & 0x1));
                    reg_temp = reg_temp >> 1;
                }
                s[bit_count] = parity_bit;
                bit_count = (byte)(bit_count + 1);

                if (bit_count >= bit_size)
                    goto NextI;


                // AND Register 1 with feedback taps, calculate parity
                reg_temp = reg_1 & 0xE4613C47;
                parity_bit = 0;
                for (var k = 0; k <= 31; k++)
                {
                    parity_bit = (byte)(parity_bit ^ (reg_temp & 0x1));
                    reg_temp = reg_temp >> 1;
                }

                s[bit_count] = parity_bit;
                bit_count = (byte)(bit_count + 1);

                if (bit_count >= bit_size)
                    goto NextI;
            }

        NextI:
            ;
        }
    }

    public void interleave(ref byte[] c, ref byte[] s, ref byte[] d)
    {
        byte rev;
        int index_temp;

        int i = 0;

        for (var j = 0; j <= 254; j++)
        {
            // Bit reverse the index
            index_temp = j;
            rev = 0;

            for (var k = 0; k <= 7; k++)
            {
                if ((index_temp & 0x1) > 0)
                    rev = (byte)(rev | (1 << (7 - k)));
                index_temp = index_temp >> 1;
            }

            if (rev < 162)
            {
                d[rev] = s[i];
                i = i + 1;
            }

            if (i >= 162)
                goto NextJ;

            NextJ:
            ;
        }

        for (i = 0; i <= 162 - 1; i++)
            s[i] = d[i];
    }

    public byte code(char c_)
    {
        bool isdigit;
        byte result = 255;

        isdigit = false;

        if (c_ >= '0')
        {
            if (c_ <= '9')
                isdigit = true;
        }

        if (isdigit == true)
            result = (byte)(c_ - 48);
        else
        {
            if (c_ == ' ')
                result = 36;

            if (c_ >= 'A')
            {
                if (c_ <= 'Z')
                    result = (byte)(c_ - 55);
            }
        }

        return result;
    }
}
