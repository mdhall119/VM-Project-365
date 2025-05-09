// Computer Science 365: VM Project, Machine
// Mike Hall, Seth Nelson, Sarah Pastor, Alan Saucer

/*
    Dear Dr. Marz. Forgive us for not figuring out Return.
*/

// Machine

#[path = "instruction.rs"] mod instruction;
use std::process::exit;
use instruction::Instruction;

pub struct Machine {
    ram: [u8; 4096],
    stack_pointer: usize,
    program_counter: usize,
    last_instruction_index: usize,  // This does not change after reading everything.
}

impl Machine {
    // Constructor
    pub fn new() -> Self {
        Self {
            ram: [0; 4096],
            stack_pointer: 4096,
            program_counter: 0,
            last_instruction_index: 0, // Default to 0
        }
    }


    pub fn run(&mut self)
    {
        // println!("Starting machine execution...");
        // println!("Initial PC: {}, SP: {}", self.program_counter, self.stack_pointer);
        
        // Calculate last_instruction_index based on loaded program
        // Assume all bytes have valid instructions
        // last_instruction_index = next multiple of 4 after last non-zero byte
        for i in (0..self.ram.len()).rev() {
            if self.ram[i] != 0 {
                // Found the last non-zero byte, round up to the next multiple of 4
                self.last_instruction_index = ((i + 1) + 3) & !3;
                break;
            }
        }
        // println!("Last instruction index: {}", self.last_instruction_index);



        // ! 3 Exit Conditions:
        // !    - stack_pointer < last_instruction_index
        // *            Stack has overwritten the instructions, exit gracefully
        // !    - program_counter >= last_instruction_index
        // *            The program_counter can not equal or go above the last_instruction_index
        // *            For example: 3 instructions = I1 = (0,1,2,3), I2 = (4,5,6,7), I3 = (8,9,10,11) and the program_counter will equal 12
        // !    - An Exit instruction is given
        // *            An exit instruction needs to be handled with the proper code
        
        // Main execution loop
        while self.stack_pointer > self.last_instruction_index 
              && self.program_counter < self.last_instruction_index {
            
            // Get the next 4 bytes for the current instruction
            let current_instr_bytes = [
                self.ram[self.program_counter],
                self.ram[self.program_counter + 1],
                self.ram[self.program_counter + 2],
                self.ram[self.program_counter + 3],
            ];
            
            // Decode the instruction
            if let Some(instruction) = Instruction::decode_instruction(&current_instr_bytes) {
                // Execute the instruction
                match instruction {
                    Instruction::Exit => {
                        //println!("Exit instruction encountered. Stopping execution.");
                        exit(current_instr_bytes[0] as i32);
                    },
                    Instruction::Goto(_) | Instruction::BinaryIf(_) | 
                    Instruction::UnaryIf(_) | Instruction::Return(_) | Instruction::Call(_) => {
                        instruction.execute(self);
                    }
                    _ => {
                        // Execute the instruction (which might update PC, SP, etc.)
                        instruction.execute(self);
                        self.pc_increment();
                    }
                }
            } else {
                println!("Unknown instruction at PC={}. Skipping for now.", self.program_counter);
                self.pc_increment();
            }
            
            // println!("After execution: PC={}, SP={}", self.program_counter, self.stack_pointer);
        }
        
        // Report why we stopped
        if self.stack_pointer <= self.last_instruction_index {
            // println!("Machine halted: Stack overflow into instruction area");
        } else if self.program_counter > self.last_instruction_index {
            // println!("Machine halted: End of program reached");
        }
        
        // Print final state
        // println!("Final state - PC: {}, SP: {}", self.program_counter, self.stack_pointer);
    }

    // Load bytes into RAM
    pub fn load_bytes(&mut self, bytes: &[u8]) {
        self.ram[0..bytes.len()].copy_from_slice(bytes);
        self.last_instruction_index = bytes.len();
        //println!("last instruction index = {}", self.last_instruction_index); // !DEBUGGING: make sure index is positioned correctly
    }

    // readonly function to examine memory
    pub fn get_byte(&self, index: usize) -> &u8
    {
        &self.ram[index]
    }

    // Get stack pointer
    pub fn get_stack_pointer(&self) -> usize {
        self.stack_pointer
    }

    pub fn get_program_counter(&self) -> usize
    {
        self.program_counter
    }

    pub fn peek(&self, offset: usize) -> i32
    {
        if offset > 4092
        {
            0
        }
        else
        {
            i32::from_le_bytes([
                self.ram[offset],
                self.ram[offset + 1],
                self.ram[offset + 2],
                self.ram[offset + 3]
            ])
        }
    }

    // (SN) Swap has to live here because it needs direct access to RAM
    pub fn swap(&mut self, from: usize, to: usize)
    {
        //println!("swap {} {}", from, to);
        let temp = self.ram[from];
        self.ram[from] = self.ram[to];
        self.ram[to] = temp;
    }

    // Change stack pointer
    // Parameter: the literal value of where the stack pointer should jump to
    // ex: sp_jump(get_stack_pointer() + 4 as usize)
    pub fn sp_jump(&mut self, value: usize)
    {
        self.stack_pointer = value;
        if self.stack_pointer > 4096
        {
            self.stack_pointer = 4096;
        }
    }

    // Advances stack pointer
    pub fn sp_increment(&mut self)
    {
        self.stack_pointer += 4;
        if self.stack_pointer > 4096
        {
            self.stack_pointer = 4096;
        }
    }

    // Reverses stack pointer
    pub fn sp_decrement(&mut self)
    {
        self.stack_pointer -= 4;
    }

    // Change program counter
    // Parameter: the literal value of where the stack pointer should jump to
    // ex: pc_jump(get_program_counter() + 4 as usize)
    pub fn pc_jump(&mut self, value: usize)
    {
        //println!("\tpc jump to {}", value);
        self.program_counter = value;
    }

    // Advances program counter
    pub fn pc_increment(&mut self)
    {
        self.program_counter += 4;
        if self.program_counter > 4096
        {
            self.program_counter = 4096;
        }
    }

    // * Push and Pop are easier to implement in machine because of direct access to ram/sp/pc
    // machine push
/*
*/
    pub fn stack_push(&mut self, value: i32)
    {
        self.sp_decrement();

        let ext_value = if((value as u32) & 1 << 27) != 0
        {
            (value as u32 | 0xF0000000) as i32
        }
        else
        {
            value
        };

        let bytes: [u8; 4] = ext_value.to_le_bytes();
        for i in 0..4 
        {
            self.ram[self.stack_pointer + i] = bytes[i];
        }
        //println!("\tstack_push: pushed {} to SP {}",value, self.stack_pointer);
    }

    // machine pop
    pub fn stack_pop(&mut self) -> i32
    {
        let mut bytes = [0u8; 4];
        //if self.stack_pointer < 4096 { 
            for i in 0..4
            {
                bytes[i] = self.ram[self.stack_pointer + i];
            }
        //}
        let value = i32::from_le_bytes(bytes);
        self.sp_increment();
        value
    }
}