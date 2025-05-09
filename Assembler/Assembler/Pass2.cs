// Pass2.cs file
// VM Assembler Project: Computer Science 365
// Alan Saucer, Mike Hall, Sarah Pastor, Seth Nelson

using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using System.Text;

public partial class Assembler
{
    public void Pass2(string fileName)
    {
        // open fileName for Pass2
        StreamReader sr;
        // initialize with empty string instead of null
        string line = string.Empty; 
        _program_counter = 0;
        try
        {
            sr = new StreamReader(fileName);
        }
        catch (Exception e)
        {
            throw new FileNotFoundException(e.Message);
        }

        while (true)
        {
            var maybeLine = sr.ReadLine();
            if (maybeLine == null) break; 
            line = maybeLine;  

            // remove extra whitespaces and comments
            line = line.Trim();
            line = line.Split('#')[0];
            if(string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue; //skip empty lines and comments
            string[] data = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            // execute stpush function
            if (data[0].ToLower() == "stpush")
            {
                // remove any remaining comments or "quotes"
                if(data.Length < 2 || !data[1].StartsWith("\"")) throw new Exception("no string to push.");
                string[] pushString = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries); 
                
                //Console.WriteLine($"original string: |{pushString[1]}|");

                int secondQuote = -1;
                string s = pushString[1].Substring(1);
        
                // handle double backslash \\ escape sequences
                for (int i = s.Length - 1; i >= 0; i--) 
                {
                    if (s[i] == '"') 
                    {
                        // count the number of backslashes before this quote
                        int backslashCount = 0;
                        int j = i - 1;
                        while (j >= 0 && s[j] == '\\') 
                        {
                            backslashCount++;
                            j--;
                        }
                        // if even num of backslashes, quote is NOT escaped
                        if (backslashCount % 2 == 0) secondQuote = i+1;
                    }
                }
                
                // extract the string between the first and second unescaped quotes
                pushString[1] = pushString[1].Substring(0, secondQuote + 1);
                // handle general backslash escape sequences (\n, \\, \")
                pushString[1] = pushString[1].Replace("\\n", "\n").Replace("\\\\", "\\").Replace("\\\"", "\"").Trim('"'); //clean out escape sequences and trim '"' from ends
                // convert the chars to ASCII bytes
                var bs = Encoding.ASCII.GetBytes(pushString[1]); //convert to ASCII

                // determine pushes needed
                int len = bs.Length;
                int remainder = len % 3;
                int pushes = (len + 2) / 3;
                int a, b, c, code;

                //Console.WriteLine($"length = {len}");
                //Console.WriteLine($"Remainder = {remainder}");

                // last push has 3 characters
                if(remainder == 0)
                {
                    a = bs[len - 3];
                    b = bs[len - 2];
                    c = bs[len - 1];
                    remainder = 3;
                }
                // last push has 2 characters
                else if (remainder == 2)
                {
                    a = bs[len - 2];
                    b = bs[len - 1];
                    c = 1;
                } 
                // last push has 1 character
                else
                {
                    a = bs[len - 1];
                    b = c = 1;
                }
                // first push is encoded with exit
                code = (0 << 24) | (c << 16) | (b << 8) | a; 

                //Console.WriteLine($"push 0x{code:x8} #  {(char)a}{(char)b}{(char)c}");

                // add instruction
                _instructionList.Add(new Push(code));

                // build and add the remaining pushes
                for(int i=len-remainder-1; i >= 0; i-=3)
                {
                    a = (i-2 >= 0) ? bs[i-2] : 1;
                    b = (i-1 >= 0) ? bs[i-1] : 1;
                    c = bs[i];  
                    code = (0b0001 << 24) | (c << 16) | (b << 8) | a; //all subsequent encoded with continue
                    
                    //Console.WriteLine($"push 0x{code:x8} #  {(char)a}{(char)b}{(char)c}"); 
                    
                    a = b = c = 0;
                    _instructionList.Add(new Push(code));
                }
                // update the program counter
                _program_counter += 4 * pushes;
                
                //Console.WriteLine($"{pushes} push instructions for this stpush");
            }
            // handle program labels
            else if(data[0].EndsWith(':'))
            {
                continue;
            }

            // handles "print", "printh", "printb", and "printo"
            else if (data[0].StartsWith("print") && _instructionMap.TryGetValue("print", out var makeInstruction)) // Handle print variations
            {
                // Console.WriteLine($"\n{data[0]} starts with print");
                // Console.WriteLine($"found print instruction: Sending data. . .");
               
                IInstruction instruction = makeInstruction(data);
                _instructionList.Add(instruction);
                _program_counter += 4;

            }
            // handles remaining instructions
            else if (_instructionMap.TryGetValue(data[0], out makeInstruction))
            {
                IInstruction instruction = makeInstruction(data);
                _instructionList.Add(instruction);
                _program_counter += 4;
            }
            // catch invalid instructions
            else
            {
                throw new Exception($"Unknown instruction: {data[0]}");
            }
        }
        // close file
        sr.Close();
    }
}