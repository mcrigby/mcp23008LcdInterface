using System.Device.Gpio;
using System.Device.I2c;
using Iot.Device.Mcp23xxx;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Iot.Device.CharacterLcd;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public sealed class Mcp23008LcdInterface : LcdInterface
{
    private readonly Mcp23008 mcp23008;
 
    private byte backlight = 0x00;
    
    private Mcp23008LcdInterface(I2cDevice i2cDevice)
    {
        mcp23008 = new(i2cDevice);
    }
    
    private void InitDisplay()
    {
        mcp23008.SetPinMode(Pin_Backlight, PinMode.Output);

        foreach (byte pin in new byte[] { Pin_D4, Pin_D5, Pin_D6, Pin_D7 })
            mcp23008.SetPinMode(pin, PinMode.Output);

        mcp23008.SetPinMode(Pin_RegisterSelect, PinMode.Output);
        mcp23008.SetPinMode(Pin_Enable, PinMode.Output);

        Thread.Sleep(50);
        mcp23008.DigitalWrite(Pin_Enable, PinValue.Low);
        mcp23008.DigitalWrite(Pin_RegisterSelect, PinValue.Low);

        SendCommandAndWait(3);
        SendCommandAndWait(3);
        SendCommandAndWait(3);
        SendCommandAndWait(2);

        BacklightOn = true;
    }

    public override bool EightBitMode => false;

    public override bool BacklightOn
    {
        get => backlight == Value_BacklightOn;
        set
        {
            backlight = value ? Value_BacklightOn : Value_BacklightOff;
            SendCommand(0);
        }
    }

    public override void SendCommandAndWait(byte command)
    {
        SendCommand(command);
        Thread.Sleep(5);
    }

    public override void SendCommand(byte command)
    {
        Write4Bits((byte)(Value_RegisterSelectCommand | (command & 0xF0)));
        Write4Bits((byte)(Value_RegisterSelectCommand | ((command << 4) & 0xF0)));
    }

    private void Write4Bits(byte command)
    {
        byte mappedCommand = MapCommandToDataPins(command);

        mcp23008.WriteByte(Register.GPIO, (byte)(mappedCommand | Value_EnableSet | backlight));
        Thread.Sleep(1);
        mcp23008.WriteByte(Register.GPIO, (byte)((mappedCommand & Value_EnableClear) | backlight));
        Thread.Sleep(1);
    }

    private static byte MapCommandToDataPins(byte command) =>
        (byte)(((command & 0xf0) >> 1) | (command & 0x07));


    public override void SendCommands(ReadOnlySpan<byte> commands)
    {
        ReadOnlySpan<byte> readOnlySpan = commands;
        for (int i = 0; i < readOnlySpan.Length; i++)
        {
            byte command = readOnlySpan[i];
            SendCommand(command);
        }
    }

    public override void SendData(byte value)
    {
        Write4Bits((byte)(Value_RegisterSelectData | (value & 0xF0)));
        Write4Bits((byte)(Value_RegisterSelectData | ((value << 4) & 0xF0)));
    }

    public override void SendData(ReadOnlySpan<byte> values)
    {
        ReadOnlySpan<byte> readOnlySpan = values;
        for (int i = 0; i < readOnlySpan.Length; i++)
        {
            byte value = readOnlySpan[i];
            SendData(value);
        }
    }

    public override void SendData(ReadOnlySpan<char> values)
    {
        ReadOnlySpan<char> readOnlySpan = values;
        for (int i = 0; i < readOnlySpan.Length; i++)
        {
            byte value = (byte)readOnlySpan[i];
            SendData(value);
        }
    }

    public static Mcp23008LcdInterface Create(I2cDevice i2cDevice)
    {
        Mcp23008LcdInterface mcp23008LcdInterface = new(i2cDevice);
        mcp23008LcdInterface.InitDisplay();

        return mcp23008LcdInterface;
    }

    private const byte Pin_RegisterSelect = 1;
    private const byte Pin_Enable = 2;
    private const byte Pin_D7 = 3;
    private const byte Pin_D6 = 4;
    private const byte Pin_D5 = 5;
    private const byte Pin_D4 = 6;
    private const byte Pin_Backlight = 7;

    private const byte Value_BacklightOn = 0x80;
    private const byte Value_BacklightOff = 0x00;

    private const byte Value_RegisterSelectCommand = 0x00;
    private const byte Value_RegisterSelectData = 0x02;

    private const byte Value_EnableSet = 0x04;
    private const byte Value_EnableClear = 0x8b;
}