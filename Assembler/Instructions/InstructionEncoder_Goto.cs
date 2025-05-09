// IInstruction with opcode 7

/*
Goto Instruction

The goto instruction takes a label operand.
This operand is encoded as a signed value in the instruction itself.
When executed, the current value of the PC, which is the goto instruction, will be added to the offset,
and then the program counter will be moved to that new location to execute the next instruction.

The relative offset will be sign-extended to be four bytes.
Since the relative offset is signed, a negative value will move the PC backwards, and a positive value will move the PC forwards.
*/

// _value = (arg != null && labels.TryGetValue(arg, out int val)) ? val : //first check if arg is a label
//          (arg != null) ? StringTo.Integer(arg) : 0; //if not then parse it
using System;
using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;

public class Goto : IInstruction
{
    private readonly int _targetAddress;
    private readonly int _currentPC;
    
    public Goto(string arg, Dictionary<string, int> labels, int currentPC)
    {
        _currentPC = currentPC;
        _targetAddress = (arg != null && labels.TryGetValue(arg, out int val)) ? val : 
                         (arg != null) ? StringTo.Integer(arg) : 0;
    }
    
    public int Encode()
    {
        // Calculate the relative offset
        int relativeOffset = _targetAddress - _currentPC;
        return (0x7 << 28) | (relativeOffset & 0x0FFFFFFF); // Apply the 28-bit mask
    }
}