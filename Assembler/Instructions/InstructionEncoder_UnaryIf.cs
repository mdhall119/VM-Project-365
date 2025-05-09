// IInstruction with opcode 9

/*
Unary If Instructions

The unary if statement will peek one value at the stack pointer.
There will be no check to make sure a value exists on the stack.
If the condition is false, then this instruction is a nop. Otherwise, if the condition is true,
then this instruction has the same effect as a goto statement.
The PC relative offset is sign-extended to four bytes and is then added to the program counter of the if statement.

The condition codes for unary if are encoded as follows:

Code | Mnemonic | Description
00 ez equals zero (x == 0)
01 nz not equals zero (x != 0)
10 mi less than 0 (negative)
11 pl greater than or equal to 0 (positive)
*/
using System;
using System.Collections.Generic;
using System.IO;
public class UnaryIf : IInstruction
{
    private readonly int _condition;
    private readonly int _target;
    
    public UnaryIf(int condition, int offset)
    {
        // Validate condition code (0-3 are valid)
        if (condition < 0 || condition > 3)
            throw new ArgumentOutOfRangeException(nameof(condition), "Condition must be between 0 and 3 inclusive");
            
        _condition = condition;
        _target = offset;
    }
    
    public int Encode()
    {
        // Opcode 9 (unary if) in the highest 4 bits
        int instruction = 9 << 28;
        
        // Bit 27 must be 0, condition code in bits 26-25
        instruction |= (_condition & 0x3) << 25;
        
        // Target offset in the remaining 25 bits (allowing for signed values)
        instruction |= (_target & 0x1FFFFFF);
        
        return instruction;
    }
    
    public static IInstruction Parse(string[] tokens, Dictionary<string, int> labels, int pc)
    {
        if (tokens.Length < 3)
            throw new ArgumentException("Unary if requires a condition and target label");
            
        string condition = tokens[1].ToLower();
        string target = tokens[2];
        
        int conditionCode;
        switch (condition)
        {
            case "ez": conditionCode = 0; break;
            case "nz": conditionCode = 1; break;
            case "mi": conditionCode = 2; break;
            case "pl": conditionCode = 3; break;
            default: throw new ArgumentException($"Invalid condition for unary if: {condition}");
        }
        
        int offset = labels.GetValueOrDefault(target, 0) - pc;
        return new UnaryIf(conditionCode, offset);
    }
}
