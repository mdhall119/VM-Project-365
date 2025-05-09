// IInstruction with opcode 14

/*
Dump Instruction

The dump instruction will print the address of (relative to the stack) and the value of each value on the stack.
If there are no values on the stack, this instruction has the same effect as nop.
Otherwise, each address will be output using four hex digits, followed by a colon and a space,
followed by the value as an eight-digit hex value.
No prefixes will be used, and all digits will be lowercase.
*/
using System;
using System.Collections.Generic;
using System.IO;
public class Dump : IInstruction
{
    public Dump(){}
    public int Encode()
    {
        return (0xe << 28);
    }
}