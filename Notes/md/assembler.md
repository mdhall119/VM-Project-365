# Project: Assembler (C#)

## Overview

You will be writing a **two-pass** assembler for the COSC365 virtual machine (vMach). The first pass will record the labels and the second pass will encode the instructions.

> You must write this assembler in C#.

The virtual machine you are writing the assembler for is a **stack based** virtual machine, much like the Java virtual machine and .NET virtual machine. It also mimics the x87 floating point unit.

Labels are **case-sensitive**, but instructions are **case-insensitive**.

## Pass 1

Pass 1 must record the line as well as the instruction location in memory for each label. A label can be detected since they always end with a colon ':'. The first instruction will be at memory location 0. Since there are also comments, empty lines, and labels, this means that the line will not match the memory location.

The job of pass 1 is to encode the labels to the program counter. Labels are not instructions, so they do not increment the program counter. For example,

```plaintext
# This is a comment for the entire line
start:

    goto main
main:
    exit 0

done:
```

In the code above, the start label will encode to 0, whereas the main label will encode to 4, since it points to the second instruction, exit. The done label will point to 8, since it is after two instructions.

> Unlike other assemblers, labels will be on their own lines. This should simplify parsing.

Notice that `start:` is on line 1, but the first memory item is `goto main`, which is on line 2. Therefore, you must keep two counters: one for the line and one for the memory location.

> All instructions take 4 bytes, so the memory locations will always be a multiple of 4.

### Comments

Comments start with a pound sign '#' and are skipped by your assembler. This includes a comment on its own line or after a statement.

Example Comments

```plaintext
#Comment
# Comment
       #     Comment
push 0   # comment
swap -1  # comment
```

### Whitespace

All leading and trailing whitespace must be trimmed before the line is parsed, including for the parameters, except where otherwise noted. This means that all of the following are the same, valid assembly code:

```plaintext
push 10
   push 10
                push        10
```

### Pseudo Instructions

The `stpush` pseudoinstruction will complicate pass 1 since it adds instructions, so your labels will be moved as more `push` instructions are added to add strings. You will therefore need to keep this in mind as you parse labels on the first pass.

## Pass 2

The second pass will encode the instructions. This is where you will use object-oriented programming. First, implement an instruction interface as given below.

```plaintext
public interface IInstruction {
    int Encode();
}
```

You will then need to create a class for each instruction. The `Encode()` instruction will give the 4-byte representation of the instruction. This will make it so you can store all of the instructions in a `List<IInstruction>` and then after parsing and converting, you can write to a binary file by simply calling `Encode()` on each list element.

You can then use this interface to implement your instructions. For example, the dup instruction is given below.

```csharp
public class Dup : IInstruction {
    private readonly int _offset;
    public Dup(int offset) {
        _offset = offset & ~3;
    }
    public int Encode() {
        return (0b1100 << 28) | _offset;
    }
}
```

You can then call this virtualized method to write the output file.

```plaintext
foreach (var inst in _instructionList) {
    binFileOut.Write(inst.Encode());
}
```

### Memory Labels

Since any instruction that manipulates the PC has an offset that is PC-relative, you need to be careful with any pseudoinstruction that expands to multiple instructions, since this PC doesn't just increment by 4 for each valid line in the assembly file. For example, `stpush "Hello"` is going to expand into two push instructions, so the PC after this line will be moved up by 8.

## Output File Format

The first four bytes of the output file will be 0xde, 0xad, 0xbe, 0xef. In other words, the first byte must be 0xde, the second 0xad, and so forth. Afterward, each of the four-byte instructions will be stored in **little-endian** format.

Double check by looking at the size of the output file. If there are 10 instructions, the file should be 44 bytes. The magic occupies 4 bytes, and the 10 instructions occupy 40 bytes, making the file 44 bytes. Again, labels and comments do not affect the output size since they are not encoded in the output file.

> Your assembler will give an error if there are 0 instructions to assemble. Do not create a file with just the magic header.
>
> Your assembler will add the `nop` instruction to the bottom of the file to pad out a multiple of 4 instructions. Do not include the magic header as part of your calculations. |

## Testing

There are several `.asm` source code files and `.v` assembled files you can test on the Hydra/Tesla machines.

```plaintext
~> cd /home/smarz1/courses/cosc365/project/tests
/home/smarz1/courses/cosc365/project/tests>
```

You can assemble the `.asm` files using your assembler to see if it produces the exact same `.v` file.

There are also two executables: `assemble` and `machine`, which are the assembler and machine, respectively you can test with.

### Checking Differences

You may use the `diff` command to check to see if your assembler produces a different file than mine.

```plaintext
~> diff myavg.v /home/smarz1/courses/cosc365/project/tests/avg.v
```

If the two files are the same, nothing will be output. Otherwise, if the files are different, you will see:

Binary files `myavg.v` and `/home/smarz1/courses/cosc365/project/tests/avg.v` differ

> `myavg.v` is just an example. You should test all of the `.asm` files in the tests directory and some of your own.

## Submit

Submit your assembler as a `.zip` file.

### ZIP Format

Your ZIP file **must** have the format that all of your files will be in a directory that is your netid. For example, if my netID is xyz123, my ZIP file will extract as:

```plaintext
> unzip file.zip
Archive:  file.zip
  inflating: xyz123/Main.cs
  inflating: xyz123/IInstruction.cs
  inflating: xyz123/Instructions.cs
```

Don't worry about the number of files, but you should at least have three: (1) for the class that contains `Main`, (2) for the class that contains the `IInstruction` interface, and (3) for the classes that contain the implementation of `IInstruction`.

> If your ZIP file does not extract into a new directory which is your netID, your project will be penalized 25%.
>
> Don't worry about the **name** of the ZIP file. Canvas will change the name anyway, but it will always have the extension `.zip`. The only thing you need to worry about is how the zip file extracts.
