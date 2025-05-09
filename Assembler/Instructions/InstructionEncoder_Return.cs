// IInstruction with opcode 6

/*
Return Instruction

The return instruction pops a 4-byte address off the stack + offset and sets the program counter to that value.
The offset is optional. If it is omitted, the offset is set to 0.
If the offset is provided, the stack is moved down by that amount.
The point of the offset is to "free" an entire stack frame rather than 4 bytes at a time.
The stack offset must always be a multiple of four, which is why bits 1:0 are 0b00.
*/

/*
    (Seth Nelson)
    Return encodes the opcode 6 along with a four-divisible offset.
    If no offset is provided, a default value of 0 is encoded.
*/
using System;
using System.Collections.Generic;
using System.IO;
public class Return : IInstruction
{
    private readonly int _offset;
    public Return(string[] args)
    {
        Int32 offset = (args.Length > 1) ? StringTo.Integer(args[1]) : 0;
        offset = (offset > 0) ? offset : 0; //default value of 0 if offset is not specified
        if(offset % 4 != 0) throw new Exception($"{offset}: offset to return is not a multiple of 4.");
        else _offset = offset;
    }
    public int Encode()
    {
        return (6 << 28) | (_offset & 0xFFFFFFC); //where C = (0b1100) to enforce multiple of 4.
    }
}