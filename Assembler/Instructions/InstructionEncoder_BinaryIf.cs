// IInstruction with opcode 8

/*
Binary If Instructions

The binary if instructions peek two values from the stack (right is sp, left is sp+4).
If there are not two values on the stack, then any missing value is defaulted to 0.
The if statement then compares the operands.
If the condition is false, then this instruction is a nop.
If the condition is true, then the if statement has the same effect as a goto statement in that
the program counter of the if instruction is added to the PC relative offset encoded in the instruction.

The condition codes for binary if are encoded as follows:
Code | Mnemonic | Description
000 eq equals (==)
001 ne not equals (!=)
010 lt less than (<)
011 gt greater than (>)
100 le less-than or equal-to (<=)
101 ge greater-than or equal-to (>=)
110 not a legitimate code
111 not a legitimate code
*/
using System;
using System.Collections.Generic;
using System.IO;
public class BinaryIf : IInstruction
{
    private readonly int _condition;
    private readonly int _target;
    
    public BinaryIf(int condition, int offset)
    {
        // Validate condition code (0-5 are valid)
        if (condition < 0 || condition > 5)
            throw new ArgumentOutOfRangeException(nameof(condition), "Condition must be between 0 and 5 inclusive");
            
        _condition = condition;
        _target = offset;
    }
    
    public int Encode()
    {
        // Opcode 8 (binary if) in the highest 4 bits
        int instruction = 8 << 28;
        
        // Condition code in the next 3 bits
        instruction |= (_condition & 0x7) << 25;
        
        // Target offset in the remaining 25 bits (allowing for signed values)
        instruction |= (_target & 0x1FFFFFF);
        
        return instruction;
    }
    
    public static IInstruction Parse(string[] tokens, Dictionary<string, int> labels, int pc)
    {
        if (tokens.Length < 3)
            throw new ArgumentException("Binary if requires a condition and target label");
            
        string condition = tokens[1].ToLower();
        string target = tokens[2];
        
        int conditionCode;
        switch (condition)
        {
            case "eq": conditionCode = 0; break;
            case "ne": conditionCode = 1; break;
            case "lt": conditionCode = 2; break;
            case "gt": conditionCode = 3; break;
            case "le": conditionCode = 4; break;
            case "ge": conditionCode = 5; break;
            default: throw new ArgumentException($"Invalid condition for binary if: {condition}");
        }
        
        int offset = labels.GetValueOrDefault(target, 0) - pc;
        return new BinaryIf(conditionCode, offset);
    }
}