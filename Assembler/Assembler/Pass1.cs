/*
Pass 1 must record the line as well as the instruction location in memory for each label. 
A label can be detected since they always end with a colon ':'. The first instruction 
will be at memory location 0. Since there are also comments, empty lines, and labels, this means 
that the line will not match the memory location.

The job of pass 1 is to encode the labels to the program counter. Labels are not instructions, 
so they do not increment the program counter. For example,

# This is a comment for the entire line
start:

    goto main
main:
    exit 0

done:
In the code above, the start label will encode to 0, whereas the main label will encode to 4, 
since it points to the second instruction, exit. The done label will point to 8, since it is after two instructions.

!!!!
Labels don't execute, they're metadata that lets you reference that location from somewhere else. e.g. as a branch target, or to load data from there. They're not part of the machine code, and CPUs don't know about them. Think of them as zero-width markers that let you refer to this byte-position from elsewhere.

They do not have any implicit association with the following bytes, or the interval until the next label. You can even have multiple labels in the same place if you want. (Compilers may do that when auto-generating labels, like in a simple function whose body reduces to just a loop, they'll

Execution will simply fall through a label, exactly like a C goto label inside a C function. Or a case 'x': label inside a switch - remember you need a break to not fall through to the next case.

Functions (and scopes) are high-level concepts. Labels (to define symbols) are one of the tools that asm provides to make it possible to implement functions. (Along with instructions like call and ret to jump and save a return address.) As opposed to a big pile of spaghetti code that just jumps around between arbitrary points, like gotos within one huge function - apparently this was typical in the bad old days before proponents of "structured programming" pointed out how much easier it was to engineer larger programs in terms of functions and if/else blocks, restricting the way you use jumps in asm to line up with those concepts. "Function" isn't a first-class concept in raw machine code, or in most assembly languages. (MASM has a proc keyword you can use instead of just a label.)
!!!!

Unlike other assemblers, labels will be on their own lines. This should simplify parsing.
Notice that start: is on line 1, but the first memory item is goto main, which is on line 2. 
Therefore, you must keep two counters: one for the line and one for the memory location.

All instructions take 4 bytes, so the memory locations will always be a multiple of 4.

Comments
Comments start with a pound sign '#' and are skipped by your assembler. 
This includes a comment on its own line or after a statement.

Example Comments
#Comment
# Comment
       #     Comment
push 0   # comment
swap -1  # comment

Whitespace
All leading and trailing whitespace must be trimmed before the line is parsed, 
including for the parameters, except where otherwise noted. 
This means that all of the following are the same, valid assembly code:

push 10
   push 10
                push        10
Pseudo Instructions
The stpush pseudoinstruction will complicate pass 1 since it adds instructions, 
so your labels will be moved as more push instructions are added to add strings. 
You will therefore need to keep this in mind as you parse labels on the first pass.
*/

using System;
using System.Collections.Generic;
using System.IO;

public partial class Assembler
{
     
     /*
     The goal of Pass1 is simply to record the PC address of labels
     */
    public void Pass1(string fileName)
    {
        StreamReader sr;
        string line = string.Empty; // Initialize with empty string instead of null
        try
        {
            sr = new StreamReader(fileName);
        }
        catch (Exception e)
        {
            throw new FileNotFoundException(e.Message);
        }

        #pragma warning disable CS8600 // Disble CS8600 warning for nullability
        while((line = sr.ReadLine()) != null)
        #pragma warning restore CS8600
        {
            line = line.Trim();
            if(string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue; //skip empty lines and comments
            string[] data = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if(data[0].ToLower() == "stpush")
            {
                if(data.Length < 2 || !data[1].StartsWith("\"")) throw new Exception("no string to push.");
                string[] pushdata = line.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                if (pushdata[1].Contains('#')) pushdata[1] = pushdata[1].Substring(0, pushdata[1].IndexOf('#')); //only keep part of string before '#' character
                pushdata[1] = pushdata[1].Replace("\\n", "\n").Replace("\\\\", "\\").Replace("\\\"", "\"").Trim('"'); //replace escape sequences and trim the "" from ends
                int stringChunks = (pushdata[1].Length % 3 == 0) ? //three is number of characters per push
                                    pushdata[1].Length/3 : 
                                    pushdata[1].Length/3 + 1;
                _program_counter += stringChunks * 4; //four is size of each push instruction
            }
            else if(data[0].EndsWith(':')) //label flag
            {
                _labels.Add(data[0].TrimEnd(':'), _program_counter);
            }
            else //normal instruction
            {
                _program_counter += 4; 
            }
        }

        sr.Close();
    }
}