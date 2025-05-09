// IInstruction with opcode 4

/*
The print instruction will print characters byte-by-byte *(sp + offset) until it hits a 0 byte (null terminator) or it hits the bottom of the stack.
The offset can be given as a decimal or hexadecimal number and is a signed, four-byte integer.
If the offset is not given to the instruction, it will be encoded as 0.
This instruction will not bounds check the offset.
Since a full four-byte value can’t be pushed to the stack, strings may have a byte of value 1. This byte must be ignored. 
*/
using System;
using System.Collections.Generic;
using System.IO;
public class StPrint : IInstruction
{
    private readonly int _offset;
    public StPrint(string[] args)
    {
        Int32 offset = (args.Length > 1) ?  StringTo.Integer(args[1]) : 0;
        _offset = (offset > 0) ? offset : 0; //default of 0 if no argument given
    }
    public int Encode()
    {
        return (4 << 28) | (_offset & 0xFFFFFFF);
    }
}