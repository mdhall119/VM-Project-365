// IInstruction with opcode 5
/*
Call Instruction

The call instruction takes a mandatory label. 
The assembler will generate a PC-relative offset. 
An offset of 0 means to call back to the call instruction itself.
The call instruction will first push the program counter of the next instruction and then execute a goto to the PC-relative offset.
The PC offset must always be a multiple of four, which is why bits 1:0 are 0b00.

*/
using System;
using System.Collections.Generic;
using System.IO;
public class Call : IInstruction
{
    private readonly int _label;
    public Call(int label)
    {
        _label = label & ~0x3;
    }
    public int Encode()
    { 
        // Opcode 5 in bits 31-27, PC-relative offset in lower bits
        // Ensure the offset is a multiple of 4 by clearing bits 1:0
        return (5 << 28) | (_label & 0xFFFFFFF);
    }
}