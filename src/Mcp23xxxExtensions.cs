using System.Device.Gpio;
using Iot.Device.Mcp23xxx;
using Mcp23xxx_Base = Iot.Device.Mcp23xxx.Mcp23xxx;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Iot.Device.CharacterLcd;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class Mcp23xxxExtensions
{
    public static void SetPinMode(this Mcp23xxx_Base mcp23Xxx, byte pin, PinMode pinMode)
    {
        mcp23Xxx.PinWrite(Register.IODIR, pin, pinMode == PinMode.Output ? PinValue.Low : PinValue.High);
        mcp23Xxx.PinWrite(Register.GPPU, pin, pinMode == PinMode.InputPullUp ? PinValue.High : PinValue.Low);
    }

    public static void DigitalWrite(this Mcp23xxx_Base mcp23Xxx, byte pin, PinValue pinValue) =>
        mcp23Xxx.PinWrite(Register.GPIO, pin, pinValue);

    public static PinValue DigitalRead(this Mcp23xxx_Base mcp23Xxx, byte pin) =>
        mcp23Xxx.PinRead(Register.GPIO, pin);

    public static PinValue PinRead(this Mcp23xxx_Base mcp23Xxx, Register register, byte pin)
    {
        byte mask = (byte)(1 << (pin & 0x07));
        byte value = (byte)(mcp23Xxx.ReadByte(register) & mask);

        return value == 0 ? PinValue.Low : PinValue.High;
    }

    public static void PinWrite(this Mcp23xxx_Base mcp23Xxx, Register register, byte pin, PinValue pinValue)
    {
        byte mask = (byte)(1 << (pin & 0x07));
        byte value = mcp23Xxx.ReadByte(register);

        if (pinValue == PinValue.Low)
            value &= (byte)~mask;
        else
            value |= mask;
        
        mcp23Xxx.WriteByte(register, value);
    }
}