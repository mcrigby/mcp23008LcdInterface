# Introduction 
MCP23008 LCD Interface

The MCP23008 is used on the AdaFruit LCD Character Backpack. Use this LCDInterface
with the HD44780 class to use the AdaFruit LCD Character Backpack in dotnet.

# Getting Started
using System.Device.I2c;
using System.Drawing;
using Iot.Device.CharacterLcd;

I2cConnectionSettings i2CConnectionSettings = new(1, 0x20);
I2cDevice i2CDevice = I2cDevice.Create(i2CConnectionSettings);
Hd44780 hd44780 = new(new Size(20, 4), Mcp23008LcdInterface.Create(i2CDevice));

hd44780.BacklightOn = true;
hd44780.Clear();
hd44780.BlinkingCursorVisible = true;
hd44780.UnderlineCursorVisible = true;
hd44780.Increment = true;
hd44780.DisplayOn = true;
hd44780.Write("Hello World!");
hd44780.SetCursorPosition(0, 1);
hd44780.Write("Line 2");
