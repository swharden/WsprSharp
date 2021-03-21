# Original Project (VB)

This Visual Basic .NET Framework Windows Forms project was created by W8ZLW and sent to me (AJ4VD) by email with permission to upload it to GitHub under the MIT license.

```vb
Dim WSPR As clsWSPR = New clsWSPR
WSPR.Encode_Message("W8ZLW", "AA00", 3)
```

```
Message: W8ZLW AA00 3

Source-encoded message (50 bits, hex): F9 72 F2 8F BB 90 C0 

Data Symbols:
1,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,1,1,1,0,0,1,1,
1,0,0,0,1,1,1,0,1,1,1,1,1,0,0,0,1,1,1,1,1,1,1,0,0,0,1,0,1,1,
0,0,0,1,1,0,1,0,1,1,0,0,0,0,0,1,0,0,0,0,0,1,1,1,0,0,1,0,0,1,
1,1,0,1,1,0,1,0,1,1,1,1,1,0,0,1,1,1,0,1,1,0,1,0,1,0,1,0,0,1,
0,0,0,0,0,0,1,1,1,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,1,1,1,0,0,1,
1,0,0,0,1,0,1,1,1,0,1,0

Sync Symbols:
1,1,0,0,0,0,0,0,1,0,0,0,1,1,1,0,0,0,1,0,0,1,0,1,1,1,1,0,0,0,
0,0,0,0,1,0,0,1,0,1,0,0,0,0,0,0,1,0,1,1,0,0,1,1,0,1,0,0,0,1,
1,0,1,0,0,0,0,1,1,0,1,0,1,0,1,0,1,0,0,1,0,0,1,0,1,1,0,0,0,1,
1,0,1,0,1,0,0,0,1,0,0,0,0,0,1,0,0,1,0,0,1,1,1,0,1,1,0,0,1,1,
0,1,0,0,0,1,1,1,0,0,0,0,0,1,0,1,0,0,1,1,0,0,0,0,0,0,0,1,1,0,
1,0,1,1,0,0,0,1,1,0,0,0

Channel Symbols:
3,1,2,2,0,0,0,0,1,0,0,0,3,3,3,2,2,2,1,0,0,1,0,3,3,3,1,0,2,2,
2,0,0,0,3,2,2,1,2,3,2,2,2,0,0,0,3,2,3,3,2,2,3,1,0,1,2,0,2,3,
1,0,1,2,2,0,2,1,3,2,1,0,1,0,1,2,1,0,0,1,0,2,3,2,1,1,2,0,0,3,
3,2,1,2,3,0,2,0,3,2,2,2,2,0,1,2,2,3,0,2,3,1,3,0,3,1,2,0,1,3,
0,1,0,0,0,1,3,3,2,0,0,0,0,1,0,1,0,0,3,3,2,2,0,0,2,2,2,1,1,2,
3,0,1,1,2,0,2,3,3,0,2,0
```