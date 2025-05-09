// All IInstructions with opcode 0

/*
    (Seth Nelson)
    Exit: Issues the exit command. An optional exit code may be provided ranging from 0-256.
*/
using System;
using System.Collections.Generic;
using System.IO;
public class Exit : IInstruction
{
    private readonly int _code;
    public Exit(int code = 0)
    {
        _code = code;
    }
    public int Encode()
    {
        return 0 | (_code & 0xFF); //last 8 bits available for exit code
    }
}

/*
Swap Instruction

The swap instruction will swap the four-byte values at the memory address given by the stack pointer (sp+from) and (sp+to). 
In other words, swap(sp+from, sp+to). This instruction does NOT check to make sure (sp+from) and (sp+to) are value. 
It doesn’t matter which one is from and which is to. The from and to fields are both 12 bits and are signed values. 
If from is not specified, it defaults to 4, and if to is not specified, it defaults to 0.

The from and to fields are encoded using 12 bits. 
However, since the offset can only be a multiple of four, the offset is shifted two places to the right before being encoded into the 12-bit field. 
Therefore, when decoded, the machine will read these as 14-bit values, with the lower two bits cleared to 0.

The from and to fields may be specified as either decimal (base 10) or hexadecimal (base 16). Hexadecimal will be prefixed by 0x. 
*/

/*
    (Seth Nelson)
    Swap issues the swap instruction with the from and to addresses encoded.
    The from and to arguments must be in the order of <from> <to>
*/
public class Swap : IInstruction
{
    private int _from;
    private int _to;
    public Swap(string[] data)
    {
        // assign temp values with defaults if no args given
        Int32 from = (data.Length > 1) ? StringTo.Integer(data[1]) : 4;
        Int32 to   = (data.Length > 2) ? StringTo.Integer(data[2]) : 0;

        // ensure both values are multiples of 4, otherwise, throw exception and print error.
        if(from % 4 != 0) throw new Exception($"{from}: swap from value is not a multiple of 4.");
        else _from = from;

        if(to % 4 != 0) throw new Exception($"{to}: swap to value is not a multiple of 4.");
        else _to = to;
    }
    public int Encode()
    {
        return (0x01 << 24) | (((_from >> 2) & 0xFFF) << 12) | ((_to >> 2) & 0xFFF);
    }
}


/*
    (Seth Nelson)
    The NOP instruction simply encodes the opcode '0x02'
*/
public class Nop : IInstruction
{
    public Nop()
    {
        Encode();
    }
    public int Encode()
    {
        return (0 << 28) | (2 << 24);
    }
}

/*
Input Instruction

The input instruction will input an integer value from the console.
When the four-byte integer is received, it will be pushed to the stack.
The input will be interpreted as base 10 unless a prefix is supplied.
If the 0x or 0X prefix is supplied, the input will be interpreted as base 16.
If the 0b or 0B prefix is supplied, the input will be interpreted as base 2.
*/

/*
    (Seth Nelson)
    The Input instruction simply encodes the input opcode
*/
public class Input : IInstruction
{
    public Input(){}
    public int Encode()
    {
        return (0 << 28) | (4 << 24);
    }
}

/*
String Input Instruction

The stinput instruction will read a line as a string from the console (stdin). 
The maximum number of characters that can be pushed to the stack is given by a 24-bit unsigned size encoded in the instruction. 
If no number is given, the 24 bits will all be encoded with 1s (i.e., 0x00FF_FFFF). 
If more characters are given than the maximum size, they will be discarded.

If the maximum size is given, it may be specified in either decimal or hex. Hex must be prefixed with 0x.

All leading and trailing whitespace characters will be removed from the input string (not the final, pushed string). 
Leading and trailing whitespace does not count towards the maximum since it is removed before the string is pushed. 
If no string is given (e.g, the string is nothing but whitespace or is just enter), then 0 is pushed to the stack.
The string input instruction pushes a string to the stack just like String Push Instruction does. 
*/

/*
    (Seth Nelson)
    The StInput instruction encodes the opcode along with an offset field.
    If no offset is provided, a default maximum value of 0x00FF_FFFF is used.
*/
public class StInput : IInstruction
{
    private readonly int _maxChars;
    public StInput(string[] size)
    {
        Int32 chars = (size.Length > 1) ? StringTo.Integer(size[1]) : 0x00FF_FFFF;
        _maxChars = chars;
    }
    public int Encode()
    {
        return (0x05 << 24) | (_maxChars & 0x00FF_FFFF);
    }
}

/*
Debug Instruction

The debug instruction is used to tell the machine to output debugging information. The bits [23:0] may be used for any purposes. Your assembler will take an optional value in base 10 or base 16 (if prefixed with 0x) and encode it into this field. If the value is not specified, the field will be encoded with all zeroes.
Testing Machine Debug
	Debugging information will not be used for grading. It is used mainly for you to test your machine. However, in order to do so, your assembler must encode it. 
*/
public class Debug : IInstruction
{
    private readonly int _debug;
    
    public Debug(string strNum)
    {
        if (!string.IsNullOrWhiteSpace(strNum))
        {
            if(strNum.Length > 1 && (strNum[1] == 'x' || strNum[1] == 'X'))
            {
                // Use the simpler overload that just takes a base parameter (16 for hex)
                _debug = Convert.ToInt32(strNum.Substring(2), 16);
            }
            else
            {
                _debug = int.Parse(strNum);
            }
        }
        else
        {
            _debug = 0;
        }
    }
    
    public int Encode()
    {
        return (0x0 << 28) | (0xF << 24) | (_debug & 0xFFFFFF);
    }
}