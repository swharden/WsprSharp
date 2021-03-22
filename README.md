# WsprSharp

[![Build Status](https://dev.azure.com/swharden/swharden/_apis/build/status/swharden.WsprSharp?branchName=main)](https://dev.azure.com/swharden/swharden/_build/latest?definitionId=17&branchName=main)

WsprSharp is a .NET Standard library for encoding/decoding messages using the [WSPR protocol](https://en.wikipedia.org/wiki/WSPR_(amateur_radio_software))

<div align="center">
<img src="dev/graphics/wspr-spectrogram.png">
</div>

## Quickstart

```cs
using WsprSharp;
var wspr = new WsprTransmission("AJ4VD", "EL89", 3);

Console.WriteLine("Message:");
Console.WriteLine(string.Join(" ", wspr.Message));

Console.WriteLine("Frequencies:");
Console.WriteLine(string.Join(" ", wspr.Levels));
```

```
Message:
71 59 134 235 146 112 192

Frequencies:
1 3 2 0 2 2 2 2 3 0 2 0 3 1 3 2 0 0 1 2 2 3 0 1 1 3 3 0 0 0 2 0 2
0 1 0 0 3 0 3 0 2 0 0 0 2 1 2 1 3 2 2 3 3 2 1 2 0 2 3 1 0 1 2 2 0
2 1 1 0 1 0 1 0 1 0 1 2 2 1 0 2 1 2 1 3 0 0 0 3 1 2 3 0 1 0 2 2 1
0 2 0 2 2 1 2 0 3 2 2 3 1 1 2 3 3 2 0 1 3 2 1 0 2 2 3 1 3 0 0 2 0
2 1 0 3 2 0 1 3 0 2 2 0 2 0 2 3 1 2 3 0 1 3 0 2 2 3 3 0 0 0
```

## WSPR Inspector

WSPR Inspector is a small Windows application designed to visualize how changes to the WSPR inputs affect the message and frequency of the transmission.

![](dev/graphics/wspr-inspector.png)

### Download

Download the latest `WsprInspector.exe` from the [Releases page](https://github.com/swharden/WsprSharp/releases)

## Authors

WsprSharp began as a Visual Basic project for .NET Framework written by [Jeff J Weinmann](jwein.acs@gmail.com) (W8ZLW). [Scott W Harden](https://swharden.com/) (AJ4VD) uploaded it to GitHub (with permission) under an MIT license, added tests, and translated it to a C# library targeting .NET Standard.