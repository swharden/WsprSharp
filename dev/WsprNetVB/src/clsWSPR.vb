Public Class clsWSPR
    Public Const WSPR_SYMBOL_COUNT As Byte = 162
    Public Const WSPR_BIT_COUNT As Byte = 162
    Public Const WSPR_MESSAGE_SIZE As Byte = 11

    'Valiation of WSPR Message Variables
    Public InValidMessage As Boolean = False
    Public InValidReason As String = ""

    'Symbol Arrays
    Public symbols(162) As Byte 'This will contain the resulting Channel Symbols

    Dim Sync_Vector As Byte() = {1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 1, 0, 0,
                                    1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0, 0,
                                    0, 0, 0, 0, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 0, 1, 1, 0, 1,
                                    0, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 0, 1, 0,
                                    1, 1, 0, 0, 0, 1, 1, 0, 1, 0, 1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1,
                                    0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 1, 1, 0, 1, 0, 0, 0, 1,
                                    1, 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0, 0,
                                    1, 1, 0, 1, 0, 1, 1, 0, 0, 0, 1, 1, 0, 0, 0}



    Public Sub Encode_Message(ByRef callsign As String, location As String, dbm As Byte)
        Dim callsign_ As String = callsign
        Dim location_ As String = location
        Dim dbm_ As Byte = dbm

        'Ensure that the message text conforms to standards
        message_prep(callsign_, location_, dbm_)
        If InValidMessage = True Then
            MsgBox(InValidReason)
            Exit Sub
        End If

        'Bit packing
        Dim c(11) As Byte
        bit_packing(c, callsign_, location_, dbm_)

        'Convolutional Encoding (162 symbols for WSPR)
        Dim s(162) As Byte
        convolve(c, s)

        'Interleaving
        Dim d(162) As Byte
        interleave(c, s, d)

        'Merge with sync vector
        merge_sync_vector(s, symbols)



        'Print out Sync symbols as formated by the command line wsprcode.exe 
        Dim Message As String = "Message:" & callsign_ & " " & location_ & " " & dbm_.ToString
        Message = Message & vbNewLine & vbNewLine

        'Hex encoded message
        Dim HexMessage As String = "Source-encoded message (50 bits, hex): "
        For i = 0 To 6
            HexMessage = HexMessage & Conversion.Hex(c(i)) & " "
        Next
        HexMessage = HexMessage & vbNewLine & vbNewLine

        'Print out Data Symbols
        Dim DataSymbols As String = "Data Symbols:" & vbNewLine
        Dim CRCount As Integer = 1
        For i = 0 To WSPR_SYMBOL_COUNT - 1
            DataSymbols = DataSymbols & d(i) & ","
            CRCount += 1
            If CRCount > 30 Then
                CRCount = 1
                DataSymbols = DataSymbols & vbNewLine
            End If
        Next
        DataSymbols = Mid(DataSymbols, 1, DataSymbols.Length - 1)
        DataSymbols = DataSymbols & vbNewLine & vbNewLine

        'Print out Sync Symbols
        Dim SyncSymbols As String = "Sync Symbols:" & vbNewLine
        CRCount = 1
        For i = 0 To WSPR_SYMBOL_COUNT - 1
            SyncSymbols = SyncSymbols & Sync_Vector(i) & ","
            CRCount += 1
            If CRCount > 30 Then
                CRCount = 1
                SyncSymbols = SyncSymbols & vbNewLine
            End If
        Next
        SyncSymbols = Mid(SyncSymbols, 1, SyncSymbols.Length - 1)
        SyncSymbols = SyncSymbols & vbNewLine & vbNewLine

        'Print out Channel Symbols
        Dim ChannelSymbols = "Channel Symbols:" & vbNewLine
        CRCount = 1
        For i = 0 To WSPR_SYMBOL_COUNT - 1
            ChannelSymbols = ChannelSymbols & symbols(i) & ","
            CRCount += 1
            If CRCount > 30 Then
                CRCount = 1
                ChannelSymbols = ChannelSymbols & vbNewLine
            End If
        Next
        ChannelSymbols = Mid(ChannelSymbols, 1, ChannelSymbols.Length - 1)

        MsgBox(Message & HexMessage & DataSymbols & SyncSymbols & ChannelSymbols)

    End Sub

    Public Sub message_prep(ByRef callsign_ As String, ByRef location_ As String, ByRef dbm_ As Byte)
        'Set Validation Variables to default
        InValidMessage = False
        InValidReason = ""

        'If only the 2nd character is a digit, then pad with a space
        'if this happens, then the callsign will be truncated if it is 
        'longer than 5 characters
        If IsNumeric(callsign_.Chars(2)) = False Then
            'Not a digit shift call string to right
            callsign_ = " " & callsign_
            callsign_ = Mid(callsign_, 1, 6)
        End If

        'Now the Third character in the callsign must be a digit
        If IsNumeric(callsign_.Chars(2)) = False Then
            Mid(callsign_, 3, 1) = "0"
            InValidMessage = True
            InValidReason = "Third character in the callsign must be a digit"
            Exit Sub
        End If

        'Grid Locator validation
        location_ = location_.ToUpper 'All CAPS
        location_ = Mid(location_, 1, 4) 'Only 4 Positions

        'First Two letters must be A thru R
        If location_.Chars(0) < "A" Or location_.Chars(0) > "R" Then
            InValidMessage = True
            InValidReason = "First two characters in locator must be 'A' thru 'R'"
        End If

        'Power Level Validation
        'Only certain increments are allowed
        If dbm_ > 60 Then
            dbm_ = 60
        End If

        Dim valid_dbm As Byte() = {0, 3, 7, 10, 13, 17, 20, 23, 27, 30, 33, 37, 40,
                                   43, 47, 50, 53, 57, 60}

        For i = 1 To valid_dbm.Length - 1
            If dbm_ < valid_dbm(i) And dbm_ >= valid_dbm(i - 1) Then
                dbm_ = valid_dbm(i - 1)
                Exit For
            End If
        Next
    End Sub

    Public Sub bit_packing(ByRef c() As Byte, ByRef callsign_ As String, location_ As String, ByRef dbm_ As Byte)
        Dim n As UInt32 = 0
        Dim m As UInt32 = 0

        n = code(callsign_.Chars(0))

        n = n * 36 + code(callsign_.Chars(1))
        n = n * 10 + code(callsign_.Chars(2))
        n = n * 27 + (code(callsign_.Chars(3)) - 10)
        n = n * 27 + (code(callsign_.Chars(4)) - 10)
        n = n * 27 + (code(callsign_.Chars(5)) - 10)

        m = ((179 - 10 * (Asc(location_.Chars(0)) - Asc("A")) - (Asc(location_.Chars(2)) - Asc("0"))) * 180) + (10 * (Asc(location_.Chars(1)) - Asc("A"))) + (Asc(location_.Chars(3)) - Asc("0"))
        m = (m * 128) + dbm_ + 64

        'Callsign Is 28 bits, locator/power Is 22 bits.
        ' A little less work to start with the least-significant bits
        c(3) = ((n And &HF) << 4)
        n = n >> 4
        c(2) = (n And &HFF)
        n = n >> 8
        c(1) = (n And &HFF)
        n = n >> 8
        c(0) = (n And &HFF)

        c(6) = ((m And &H3) << 6)
        m = m >> 2
        c(5) = (m And &HFF)
        m = m >> 8
        c(4) = (m And &HFF)
        m = m >> 8
        c(3) = c(3) Or (m And &HF)
        c(7) = 0
        c(8) = 0
        c(9) = 0
        c(10) = 0
    End Sub

    Public Sub convolve(ByRef c() As Byte, ByRef s() As Byte)
        Dim message_size As Byte = WSPR_MESSAGE_SIZE
        Dim bit_size As Byte = 162
        Dim reg_0 As UInt32 = 0
        Dim reg_1 As UInt32 = 0
        Dim reg_temp As UInt32 = 0
        Dim input_bit As Byte = 0
        Dim parity_bit As Byte = 0
        Dim bit_count As Byte = 0

        For i = 0 To message_size - 1
            For j = 0 To 7
                'Set input bit according the MSB of current element
                input_bit = If(((c(i) << j) And &H80) = &H80, 1, 0)

                ' Shift both registers and put in the new input bit
                reg_0 = reg_0 << 1
                reg_1 = reg_1 << 1
                reg_0 = reg_0 Or input_bit
                reg_1 = reg_1 Or input_bit

                ' AND Register 0 with feedback taps, calculate parity
                reg_temp = reg_0 And &HF2D05351
                parity_bit = 0
                For k = 0 To 31
                    parity_bit = parity_bit Xor (reg_temp And &H1)
                    reg_temp = reg_temp >> 1
                Next k
                s(bit_count) = parity_bit
                bit_count = bit_count + 1

                If bit_count >= bit_size Then
                    GoTo NextI
                End If


                'AND Register 1 with feedback taps, calculate parity
                reg_temp = reg_1 And &HE4613C47
                parity_bit = 0
                For k = 0 To 31
                    parity_bit = parity_bit Xor (reg_temp And &H1)
                    reg_temp = reg_temp >> 1
                Next k

                s(bit_count) = parity_bit
                bit_count = bit_count + 1

                If bit_count >= bit_size Then
                    GoTo NextI
                End If
            Next j
NextI:
        Next i
    End Sub

    Public Sub interleave(ByRef c() As Byte, ByRef s() As Byte, ByRef d() As Byte)

        Dim rev As Byte = 0
        Dim index_temp As Integer = 0

        Dim i As Integer = 0

        For j = 0 To 254
            'Bit reverse the index
            index_temp = j
            rev = 0

            For k = 0 To 7
                If index_temp And &H1 Then
                    rev = rev Or (1 << (7 - k))
                End If
                index_temp = index_temp >> 1
            Next k

            If rev < 162 Then
                d(rev) = s(i)
                i = i + 1
            End If

            If i >= 162 Then
                GoTo NextJ
            End If

NextJ:
        Next j

        For i = 0 To 162 - 1
            s(i) = d(i)
        Next i

    End Sub

    Public Sub merge_sync_vector(ByRef s() As Byte, ByRef symbols() As Byte)



        'merge_sync_vector
        For i = 0 To WSPR_SYMBOL_COUNT - 1
            symbols(i) = Sync_Vector(i) + (2 * s(i))
        Next i
    End Sub

    Public Function code(c_ As Char) As Byte
        Dim isdigit As Boolean
        Dim result As Byte = 255

        isdigit = False

        If c_ >= "0" Then
            If c_ <= "9" Then
                isdigit = True
            End If
        End If

        If isdigit = True Then
            result = Asc(c_) - 48
        Else
            If c_ = " " Then
                result = 36
            End If

            If c_ >= "A" Then
                If c_ <= "Z" Then
                    result = Asc(c_) - 55
                End If
            End If
        End If

        Return result
    End Function


End Class
