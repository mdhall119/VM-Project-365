// The Assembler Dictionary matches strings to functions
// The goal is to create a dictionary that maps a string to it's corresponding encoding function
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;

public partial class Assembler
{
    private Dictionary<string, Func<string[], IInstruction>> InitInstructionMap()
    {
        return new Dictionary<string,  Func<string[], IInstruction>>(StringComparer.OrdinalIgnoreCase)
        {
            {"exit", exit => new Exit((exit.Length > 1 && Int32.TryParse(exit[1], out int exitCode)) ? exitCode : 0)},
            {"swap", swap => new Swap(swap)},
            {"nop", none => new Nop()},
            {"input", input => new Input()},
            {"stinput", stinput => new StInput(stinput) },
            {"debug", debug => new Debug(debug.Length > 1 ? debug[1] : "")},
            {"pop", pop => new Pop(pop) }, 
            {"add", add => new Add() },
            {"sub", sub => new Sub() },
            {"mul", mul => new Mul() },
            {"div", div => new Div() },
            {"rem", rem => new Rem() },
            {"and", and => new And() },
            {"or" , or  => new Or () },
            {"xor", xor => new Xor() },
            {"lsl", lsl => new Lsl() },
            {"lsr", lsr => new Lsr() },
            {"asr", asr => new Asr() }, 
            {"neg", neg => new Neg() },
            {"not", not => new Not() },
            {"stprint", stprint => new StPrint(stprint) },
            {"call", call => new Call(call.Length > 1 ? _labels.GetValueOrDefault(call[1], 0) - _program_counter : 0) },
            {"return", ret => new Return(ret) },
            {"goto", _goto => new Goto(_goto.Length > 1 ? _goto[1] : "", _labels, _program_counter) },

            // Binary if conditions
            {"ifeq", ifeq => new BinaryIf(0, _labels.GetValueOrDefault(ifeq[1], 0) - _program_counter)},
            {"ifne", ifne => new BinaryIf(1, _labels.GetValueOrDefault(ifne[1], 0) - _program_counter)},
            {"iflt", iflt => new BinaryIf(2, _labels.GetValueOrDefault(iflt[1], 0) - _program_counter)},
            {"ifgt", ifgt => new BinaryIf(3, _labels.GetValueOrDefault(ifgt[1], 0) - _program_counter)},
            {"ifle", ifle => new BinaryIf(4, _labels.GetValueOrDefault(ifle[1], 0) - _program_counter)},
            {"ifge", ifge => new BinaryIf(5, _labels.GetValueOrDefault(ifge[1], 0) - _program_counter)},
            
            // Unary if conditions
            {"ifez", ifez => new UnaryIf(0, _labels.GetValueOrDefault(ifez[1], 0) - _program_counter)},
            {"ifnz", ifnz => new UnaryIf(1, _labels.GetValueOrDefault(ifnz[1], 0) - _program_counter)},
            {"ifmi", ifmi => new UnaryIf(2, _labels.GetValueOrDefault(ifmi[1], 0) - _program_counter)},
            {"ifpl", ifpl => new UnaryIf(3, _labels.GetValueOrDefault(ifpl[1], 0) - _program_counter)},
            {"dup", dup => new Dup(dup.Length > 1 ? (dup[1].StartsWith("0x")?Convert.ToInt32(dup[1].Substring(2), 16):Convert.ToInt32(dup[1])):0)},
            // {"dup", dup => new Dup(Convert.ToInt32(dup[1]))},
            {"print", print => new Print(print[0].Length > 5 ? print[0][5] : 'd', print.Length > 1 ? print[1] : "")},
            {"dump", dump => new Dump() },
            {"push", push => new Push(push.Length > 1 ? push[1] : "", _labels)}
        };
    }
}