// All IInstructions with opcode 3

/*
Unary Arithmetic Instructions

All unary arithmetic instructions will pop the operand from the stack.
Then, then after the operation, the result will be pushed onto the stack. 
All values, input and output, are four-byte integers. 
These instructions will not error check the stack location prior to popping or pushing values.
0011 0000
*/
using System;
using System.Collections.Generic;
using System.IO;
/*
    (Seth Nelson)
    The Neg instruction simply encodes its opcode.
*/
public class Neg : IInstruction
{

    public Neg(){}
    public int Encode()
    {
        return 0 | (0x30 << 24);
    }
}

/*
    (Seth Nelson)
    The Not instruction simply encodes its opcode.
*/
public class Not : IInstruction
{
    public Not(){}
    public int Encode()
    {
        return 0 | (0X31 << 24);
    }
}