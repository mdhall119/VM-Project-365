// All IInstructions with opcode 2

/*
Binary Arithmetic Instructions

All binary arithmetic instructions will pop the right value first followed by the left value. 
Then, the arithmetic instruction will be executed on the two values. The result will then be pushed to the stack. 
All values, input and output, are four-byte integers. These instructions will not error check the stack location prior to popping or pushing values.

lsl is a logical left shift for a 32-bit integer. 
A lsr is a logial right shift for a 32-bit integer. 
An asr is an arithmetic right shift for a 32-bit integer.

*/
using System;
using System.Collections.Generic;
using System.IO;
public class Add : IInstruction
{
    public Add(){}
    public int Encode()
    {
        return (0x20 << 24);
    }
}

public class Sub : IInstruction
{
    public Sub(){}
    public int Encode()
    {
        return (0x21 << 24);
    }
}

public class Mul : IInstruction
{
    public Mul(){}
    public int Encode()
    {
        return (0x22 << 24);
    }
}

public class Div : IInstruction
{
    public Div(){}
    public int Encode()
    {
        return (0x23 << 24);
    }
}

public class Rem : IInstruction
{
    public Rem(){}
    public int Encode()
    {
        return (0x24 << 24);
    }
}

public class And : IInstruction
{
    public And(){}
    public int Encode()
    {
        return (0x25 << 24);
    }
}

public class Or : IInstruction
{
    public Or(){}
    public int Encode()
    {
        return (0x26 << 24);

    }
}

public class Xor : IInstruction
{
    public Xor(){}
    public int Encode()
    {
        return (0x27 << 24);

    }
}

public class Lsl : IInstruction
{
    public Lsl(){}
    public int Encode()
    {
        return (0x28 << 24);
    }
}

public class Lsr : IInstruction
{
    public Lsr(){}
    public int Encode()
    {
        return (0x29 << 24);

    }
}

public class Asr : IInstruction
{
    public Asr(){}
    public int Encode()
    {
        return (0x2b << 24);
    }
}