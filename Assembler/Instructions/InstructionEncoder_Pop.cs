using System;
using System.Collections.Generic;

// All IInstructions with opcode 1
/*
Pop Instruction

The pop instruction will move the stack pointer down by the number given, which is an unsigned value and a multiple of four bytes (sp = sp + offset). 
If the stack pointer is already at the bottom of the memory allocated, this instruction has no effect. 
If the offset is not given, it is by default 4. 
If the offset places the stack pointer past the end of the memory space, the stack pointer will be reset to the end of the memory space (e.g., length(memory)).

*/
public class Pop : IInstruction
{
    private readonly uint _offset;
    public Pop(string[] offset)
    {
        if (offset == null || offset.Length <= 1 || string.IsNullOrWhiteSpace(offset[1]))
        {
            _offset = 4; // Default offset
            return;
        }
        
        string offsetValue = offset[1];
        uint parsedOffset;
        
        // Handle hexadecimal values (starting with 0x)
        if (offsetValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            if (uint.TryParse(offsetValue.Substring(2), System.Globalization.NumberStyles.HexNumber, null, out parsedOffset))
            {
                if (parsedOffset % 4 == 0)
                {
                    _offset = parsedOffset;
                    return;
                }
            }
        }
        // Handle decimal values
        else if (uint.TryParse(offsetValue, out parsedOffset) && parsedOffset % 4 == 0)
        {
            _offset = parsedOffset;
            return;
        }
        
        throw new ArgumentException("Pop offset must be a multiple of 4.");
    }
    
    public int Encode()
    {
        // Opcode 1 in the upper 4 bits and the offset (28 bits) in the lower part.
        return (1 << 28) | ((int)_offset & 0x0FFFFFFF);
    }
}
