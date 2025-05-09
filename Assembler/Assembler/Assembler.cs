using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;

public partial class Assembler
{
    public readonly Dictionary<string, int> _labels; // <label, program_count>
    private int _program_counter;
    private List<IInstruction> _instructionList;
    private readonly Dictionary<string, Func<string[], IInstruction>> _instructionMap; // <instruction_name, instructon_ctor>
    public Assembler(string inFile, string outFile)
    {
        _labels = new Dictionary<string, int>();
        _program_counter = 0;
        _instructionList = new List<IInstruction>();
        _instructionMap = InitInstructionMap();

        Pass1(inFile); // fills _labels with addresses of all labels
        Pass2(inFile); // fills _instructionList with IInstruction objects
        if(_program_counter < 4) throw new Exception($"{inFile}: no instructions to assemble.");

        var binFileOut = new BinaryWriter(File.Open(outFile, FileMode.Create));
        binFileOut.Write(0xefbeadde);
        foreach (var inst in _instructionList) 
        {
            binFileOut.Write(inst.Encode());
        }
        
        int paddingNops = 4 - ((_program_counter/4) % 4); //number of padding nops required
        if(0 < paddingNops && paddingNops < 4)
        {
            var nop = new Nop();
            for(int z=0; z < paddingNops; z++)
            {
                binFileOut.Write(nop.Encode());
            }
        }
        binFileOut.Close();
    }
}