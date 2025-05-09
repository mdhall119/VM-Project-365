// IInstruction with opcode 12
/*
Dup Instruction

The dup instruction peeks a value from the stack and pushes it. 
The value peeked is given by *(sp + sro).
The sro is the stack-relative offset.
Since the last two bits are not encoded, the value must be a multiple of four.
This is obvious since all values on the stack are four bytes.
For example, dup 0 would duplicate the value at the stack pointer and push it.
Whereas, dup 4 would peek the next value from the stack pointer *(sp + 4) and push it.
*/
using System;
using System.Collections.Generic;
using System.IO;
public class Dup : IInstruction {
    private readonly int _offset;
    public Dup(int offset) {
        _offset = offset & ~3;
    }
    public int Encode() {
        int offsetBits = _offset & 0x0FFFFFFF;
        return (0b1100 << 28) | offsetBits;
    }
}