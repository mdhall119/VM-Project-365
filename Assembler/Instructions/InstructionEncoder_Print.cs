// IInstruction with opcode 13

/*
1101 Stack Relative Offset fmt
print[h|o|b] [dec|hex]
Print Instruction

The print instruction will print the four-byte integer value given by the *(sp + offset).
The offset can be given as a decimal or hexadecimal number and is a signed, four-byte integer.
If the offset is not given to the instruction, it will be encoded as 0.
This instruction will not bounds check the offset.
After the print instruction prints the integer, it will print a newline character.

The print instruction may end with an h (hex), o (octal), or b (binary),
such as printh, printo, or printb. By default, print prints in base 10.
The format is encoded in the lower two bits (1:0) according to the following.
fmt
Format
    00 Decimal (base 10)
    01 Hex (base 16)
    10 Binary (base 2)
    11 Octal (base 8)
Output
    Hex will be prefixed with 0x.
    Binary will be prefixed with 0b.
    Octal will be prefixed with 0o.
    Decimal will not be prefixed.
    No leading zeroes will be printed.
*/
using System;
using System.Collections.Generic;
using System.IO;

/*
    (Mike Hall)
    Takes mode character, offsetStr (if given)
*/
public class Print : IInstruction
{
    private readonly int _mode;  // Format: 0 = Decimal, 1 = Hex, 2 = Binary, 3 = Octal
    private readonly int _offset; // Offset for the stack pointer

    public Print(char mode, string offsetStr)
    {
        switch(mode)
        {
            case 'd': _mode = 0; break;
            case 'h': _mode = 1; break;
            case 'b': _mode = 2; break;
            case 'o': _mode = 3; break;
            default: _mode = 0; break;
        }
        
        if (!string.IsNullOrEmpty(offsetStr))
        {
            if(offsetStr.Length > 1 && (offsetStr[0] == '0' && (offsetStr[1] == 'x' || offsetStr[1] == 'X')))
            {
                // Fix Convert.ToInt32 call to use correct overload
                _offset = Convert.ToInt32(offsetStr.Substring(2), 16);
            }
            else
            {
                _offset = int.Parse(offsetStr);
            }
        }
        else
        {
            _offset = 0;  // Default value if offsetStr is null
        }
    }

    public int Encode()
    {
        // 1101           Offset in bits 27-24    Format in bits 1-0
        return (13 << 28) | (_mode + ( _offset & 0xFFFFFFC));
    }
}
