// IInstruction with opcode 15

/*
Push Instruction

The push instruction will push a four-byte, signed integer onto the stack.
The stack pointer will move up (to lower memory address) by four bytes. 
The value pushed can be in base 10, base 16, or a memory label.
If no argument is given to push, then the value 0 is pushed to the stack.
*/
using System;
using System.Collections.Generic;
using System.IO;
public class Push : IInstruction
{
    private readonly int _value;
    public Push(string arg, Dictionary<string, int> labels)
    {
        _value = string.IsNullOrEmpty(arg)
            ? 0 // default push value
            : (
                // check if arg is a label
                labels.TryGetValue(arg, out int val)
                    ? val  // if arg is a label, use its value
                    : StringTo.Integer(arg) // if not then parse it
              );
    }
    public Push(int i)
    {
        _value = i;
    }
    public int Encode()
    {
        return (0b1111 << 28) | (_value & 0xFFFFFFF); // opcode + least significant bits
    }
}
