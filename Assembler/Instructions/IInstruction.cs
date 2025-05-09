using System;
using System.Globalization;
using System.Collections.Generic;
using System.IO;

public interface IInstruction 
{
    int Encode();
}

/*
    (Seth Nelson)
    StringTo is a helper class for all IInstructions to parse a string into a helpful value.
*/
public static class StringTo
{
    /*
        (Seth Nelson)
        Integer is how StringTo converts a string into an integer.
        It is designed to parse string representations of integers as base 2, 10, and 16 (bin, dec, hex)
        Parameter: a string
        Return: an integer on successful parse of a string into an integer, otherwise, -1 as default.
    */
    public static int Integer(string s)
    {
        if(string.IsNullOrWhiteSpace(s)) return -1;
        
        if(s.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return int.TryParse(s.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int f) ? f : -1;
        }
        else if (s.StartsWith("0b", StringComparison.OrdinalIgnoreCase))
        {
            // Binary parsing needs custom implementation since NumberStyles.BinaryNumber doesn't exist
            string binaryDigits = s.Substring(2);
            try {
                // Use Convert.ToInt32 with base 2 instead
                return Convert.ToInt32(binaryDigits, 2);
            }
            catch {
                return -1;
            }
        }
        else
        {
            return int.TryParse(s, out int x) ? x : -1;
        }
    }
}