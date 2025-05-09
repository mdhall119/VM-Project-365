// Computer Science 365: VM Project, Machine
// Mike Hall, Seth Nelson, Sarah Pastor, Alan Saucer

use std::env;
use std::io::prelude::*;
use std::fs::File;
use std::process::exit;

#[path = "machine.rs"] mod machine;
#[path = "instruction.rs"] mod instruction;
use instruction::Instruction;

fn main() {
    // Command line arguments
    let args: Vec<String> = env::args().collect();
    
    // Check if a filename was provided
    if args.len() < 2 || args.len() > 2 {
        println!("Usage: {} <file.v>", args[0]);
        return;
    }
    
    // Use the first argument as the file path
    let file_path = &args[1];
    // println!("File path: {}", file_path);

    // Open the file
    let mut f = match File::open(file_path) {
        Ok(file) => file,
        Err(e) => {
            eprintln!("Error opening file: {}", e);
            return;
        }
    };
    let mut buffer = Vec::new();

    // read the whole file
    match f.read_to_end(&mut buffer) {
        Ok(_bytes_read) => {
        },
        Err(e) => eprintln!("Error reading file: {}", e),
    }

    // Check to see if magic bytes (0xde, 0xad, 0xbe, 0xef) are present
    if buffer.len() < 4 {
        println!("File is too small to contain magic bytes.");
        return;
    } else {
        let magic_bytes = &buffer[0..4];
        if magic_bytes != b"\xde\xad\xbe\xef" {
            println!("ERROR: magic doesn't match: {:?}", magic_bytes);
            exit(1);
            // ERROR: magic doesn't match [52, 10, 54, 10].
        }
    }

    // Remove the magic bytes from the buffer
    let buffer = &buffer[4..];

    // Create a new machine and load the buffer
    let mut m = machine::Machine::new();
    m.load_bytes(buffer);

    //todo: After machine initialization we should probably have the machine run from machine.rs

    m.run();

    // Decode instructions from buffer (for now)
    let mut instructions = Vec::new();
    for chunk in buffer.chunks(4) {
        if let Some(instruction) = Instruction::decode_instruction(chunk) {
            //println!("Running...");
            instructions.push(instruction);
        }
    }

    //println!("Decoded Instructions: {:?}", instructions);

}