// Computer Science 365: VM Project, Machine
// Mike Hall, Seth Nelson, Sarah Pastor, Alan Saucer

// Instructions
use std::io::{stdin, Write};
use std::u32;
use std::convert::TryInto;

//#[derive(Debug, Clone)]
#[allow(dead_code)] // call me a tattletale, this is some AI shit Alan probably put here to silence the errors.
pub enum Instruction {
    // OPCODE 0: Miscellaneous
    Exit,
    Swap([u8; 4]),
    Nop,
    Input,
    StInput([u8; 4]),
    Debug,
    // OPCODE 1: Pop
    Pop(u32),
    // OPCODE 2: Binary arithmetic
    Add,
    Sub,
    Mul,
    Div,
    Rem,
    And,
    Or,
    Xor,
    Lsl,
    Lsr,
    Asr,
    // OPCODE 3: Unary arithmetic
    Neg,
    Not,
    // OPCODE 4: String print
    StPrint([u8; 4]),
    // OPCODE 5: Call
    Call([u8; 4]),
    // OPCODE 6: Return
    Return([u8; 4]),
    // OPCODE 7: Unconditional goto
    Goto([u8; 4]),
    BinaryIf([u8; 4]),
    UnaryIf([u8; 4]),
    Dup(i32),
    // OPCODE 13: Print
    Print(i32,u8),
    // OPCODE 14: Dump
    Dump,
    // OPCODE 15: Push
    Push(Option<i32>)
}

#[allow(dead_code)]
impl Instruction {
    // Decode 4-bytes into an Instruction
    pub fn decode_instruction(bytes: &[u8]) -> Option<Instruction> {
        if bytes.len() < 4 {
            return None; // Not enough bytes
        }

        // Extract the upper 4 bits of the last byte, the opcode
        let opcode = bytes[3] >> 4;
        // println!("Bytes: {}", helper::convert_hex(bytes));
        // println!("Opcode: {:#04x}", opcode);

        match opcode {
            0x0 => {
                // Exit instruction
                let operation = bytes[3] & 0x0F; // (SN) get lower order nibble
                match operation
                {
                    0x0 => Some(Instruction::Exit),
                    0x1 => Some(Instruction::Swap(bytes[0..4].try_into().unwrap())),
                    0x2 => Some(Instruction::Nop),
                    0x4 => Some(Instruction::Input),
                    0x5 => Some(Instruction::StInput(bytes[0..4].try_into().unwrap())),
                    0xF => Some(Instruction::Debug),
                    _ => None,
                }
            }
            0x1 => {
                // Pop instruction
                let offset = u32::from_le_bytes([bytes[0], bytes[1], bytes[2], bytes[3] & 0x0F]);
                // println!("Pop value: {}", offset);
                Some(Instruction::Pop(offset))
            }
            0x2 => {
                //Binary Arithmetic
                let operation = bytes[3] & 0x0F; // (SN) get lower order nibble
                match operation
                {
                    0x0 => Some(Instruction::Add),
                    0x1 => Some(Instruction::Sub),
                    0x2 => Some(Instruction::Mul),
                    0x3 => Some(Instruction::Div),
                    0x4 => Some(Instruction::Rem),
                    0x5 => Some(Instruction::And),
                    0x6 => Some(Instruction::Or ),
                    0x7 => Some(Instruction::Xor),
                    0x8 => Some(Instruction::Lsl),
                    0x9 => Some(Instruction::Lsr),
                    0xb => Some(Instruction::Asr),
                    _ => None,
                }
            } 
            0x3 => { // (SN) No argument, gets value from stack and pushes to stack
                let operation = bytes[3] & 0x0F; // (SN) get lower order nibble
                match operation
                {
                    0x0 => Some(Instruction::Neg),
                    0x1 => Some(Instruction::Not),
                    _ => None,
                }
            }
            0x4 => { // (SN) Take 4 bytes as your argument, you need bits 2-27 for the 'Stack Relative Offset'
                Some(Instruction::StPrint(bytes[0..4].try_into().unwrap()))
            }
            0x5 => { // (SN) Take 4 bytes as your argument, you need bits 2-27 for the 'PC-Relative Offset'
                Some(Instruction::Call(bytes[0..4].try_into().unwrap()))
            }
            0x6 => { // (SN) Take 4 bytes as your argument, you need bits 2-27 for the 'Stack Offset'
                Some(Instruction::Return(bytes[0..4].try_into().unwrap()))
            }
            0x7 => { // (SN) Take 4 bytes as your argument, you need bits 2-27 for the 'PC-Relative Offset'
                Some(Instruction::Goto(bytes[0..4].try_into().unwrap()))
            }
            0x8 => { 
                Some(Instruction::BinaryIf(bytes[0..4].try_into().unwrap()))
            },
            0x9 => { // (AS) Unary if condition
                Some(Instruction::UnaryIf(bytes[0..4].try_into().unwrap()))
            },
            0xC => { // (SN) Take 4 bytes as your argument, you need bits 2-27 for the 'Stack Relative Offset'
            let offset = i32::from_le_bytes([bytes[0], bytes[1], bytes[2], bytes[3] & 0x0F]);
            Some(Instruction::Dup(offset))

            }
            0xD => { // (SN) Take 4 bytes as your argument, bits 2-27 'Stack Relative Offset' bits 01 'format'

                let word = u32::from_le_bytes([bytes[0], bytes[1], bytes[2], bytes[3]]);
                let print_bit = (word & 0b11) as u8;
                let offset = ((word & !0b11) & 0x0FFF_FFFF) as i32;
                //println!("Offset: {}, Print Bit: {}",offset,print_bit);
                Some(Instruction::Print(offset, print_bit))
            }
            0xE => { // (SN) No arguments
                Some(Instruction::Dump)
            }
            0xF => {
                // Push instruction
                let value = i32::from_le_bytes([bytes[0], bytes[1], bytes[2], bytes[3] & 0x0F]);
                // println!("Push value: {}", value);
                Some(Instruction::Push(Some(value)))
            }
            
            _ => None, // Unknown opcode
        }
    }

    // Executes instruction
    pub fn execute(&self, machine: &mut crate::machine::Machine) {
        //println!("Executing: {:?}", self);
        match self {
            // OPCODE 0: Miscellaneous instructions
            Instruction::Exit => {},
            // OPCODE 0: Miscellaneous instructions
            Instruction::Swap(bytes) => {
                let i = u32::from_le_bytes(*bytes);
                let from_raw = (i >> 12) & 0xFFF; //get bits 12-23
                let from: i32 = if (from_raw & 1 << 11) != 0 // if 11th bit set, then negative
                {
                    ((from_raw | 0xFFFFF000) << 2) as i32 //sign extend if negative
                } 
                else 
                {
                    (from_raw << 2) as i32
                };
                let to_raw = i & 0xFFF;
                let to: i32 = if (to_raw & 1 << 11) != 0 
                {
                    ((to_raw | 0xFFFFF000) << 2) as i32
                } 
                else 
                {
                    (to_raw << 2) as i32
                };
                
                let sp = machine.get_stack_pointer() as i32;
                // println!("swap sp = {}", sp);
                // println!("swap raw from={}, to={}", from_raw, to_raw);
                // println!("swap adjusted from={}, to={}", from, to);
                // println!("swap final addresses {} {}", (sp + from) as usize, (sp + to) as usize);
                machine.swap((sp + from) as usize, (sp + to) as usize);
            },
            // OPCODE 0: Miscellaneous instructions
            Instruction::Nop => {},
            Instruction::Input => {
                let mut ipt: String = String::new();
                let _ = stdin().read_line(&mut ipt);
                let trimmed = ipt.trim();
                
                let value = if trimmed.starts_with("0x") || trimmed.starts_with("0X") 
                {
                    i32::from_str_radix(&trimmed[2..], 16).unwrap_or(0)
                } 
                else if trimmed.starts_with("0b") || trimmed.starts_with("0B") 
                {
                    i32::from_str_radix(&trimmed[2..], 2).unwrap_or(0)
                } 
                else 
                {
                    trimmed.parse::<i32>().unwrap_or(0)
                };
                
                machine.stack_push(value);
            },
            Instruction::StInput(bytes) => {
                let max_chars = (u32::from_le_bytes(*bytes) & 0x00FFFFFF) as usize; //maximum string length

                //get input from user
                let mut ipt: String = String::new();
                let _ = stdin().read_line(&mut ipt);
                let trimmed_str = ipt.trim();

                // if input is empty or just whitespace, push 0
                if trimmed_str.is_empty() { machine.stack_push(0); return; } 

                // clean out whitespace and get important info about string
                let my_string: Vec<char> = trimmed_str.chars().collect();
                let len = usize::min(my_string.len(), max_chars);
                let mut remainder = len % 3;

                let mut first_push: u32 = 0;
                match remainder //the first push (end of string) holds the remainder
                {
                    0 => 
                    {
                        first_push |= (my_string[len - 1] as u32) << 16;
                        first_push |= (my_string[len - 2] as u32) << 8;
                        first_push |= (my_string[len - 3] as u32) << 0;
                        machine.stack_push(first_push as i32);
                        remainder = 3;
                    },

                    1 =>
                    {
                        // 1 is default padding value
                        first_push |= 1 << 16;
                        first_push |= 1 << 8;
                        first_push |= (my_string[len - 1] as u32) << 0;
                        machine.stack_push(first_push as i32);
                    },

                    2 =>
                    {
                        first_push |= 1 << 16;
                        first_push |= (my_string[len - 1] as u32) << 8;
                        first_push |= (my_string[len - 2] as u32) << 0;
                        machine.stack_push(first_push as i32);
                    },
                
                    _ => return,
                }

                //println!("First Push: {:08b} {:08b} {:08b} {:08b}", (first_push >> 24) & 0xFF, (first_push >> 16) & 0xFF, (first_push >> 8) & 0xFF, first_push & 0xFF);
                let mut index = (len as i32 - remainder as i32) as usize; //assign here because remainder may need modified in earlier match
                //println!("len {} - remainder {} = index {}", len, remainder, index);
                loop //assign remaining 3-byte sets
                {
                    if index == 0 {break;}
                    let mut push_val: u32 = 0;
                    push_val |= (my_string[index - 1] as u32) << 16;
                    push_val |= (my_string[index - 2] as u32) << 8;
                    push_val |= (my_string[index - 3] as u32) << 0;
                    push_val |= 0b0001 << 24; // continue flag
                    machine.stack_push(push_val as i32);
                    index -= 3;
                }
            },
            // OPCODE 0: Miscellaneous instructions
            Instruction::Debug => {},
            // OPCODE 1: Pop instructions
            Instruction::Pop(offset) => {
                if machine.get_stack_pointer() < 4096
                {
                    let new_sp = machine.get_stack_pointer() + *offset as usize;
                    //println!("current SP: {}, target SP: {}", machine.get_stack_pointer(), new_sp);
                    if new_sp < 4096
                    {
                        machine.sp_jump(new_sp);
                    }
                    else 
                    {
                        machine.sp_jump(4096);
                    }
                }
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Add => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left + right);
                //println!("\tADD: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Sub => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left - right);
                //println!("\tSUB: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Mul => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left * right);
                //println!("\tMUL: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Div => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left / right);
                //println!("\tDIV: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Rem => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left % right);
                //println!("\tREM: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::And => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left & right);
                //println!("\tAND: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Or => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left | right);
                //println!("\tOR: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Xor => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left ^ right);
                //println!("\tXOR: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Lsl => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left << right);
                //println!("\tLSL: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Lsr => {
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                machine.stack_push(left >> right);
                //println!("\tLSR: result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 2: Binary arithmetic
            Instruction::Asr => { // ! LOW CERTAINTY THAT THIS IS HOW ASR WORKS
                let right = machine.stack_pop();
                let left = machine.stack_pop();
                let nth_bit = 1 << 27;
                if left & nth_bit == 0
                {
                    machine.stack_push(left >> right);
                }
                else
                {
                    machine.stack_push(left >> right | 1 << 28);    
                }
                //println!("ASR result: {}", machine.peek(machine.get_stack_pointer()));
            },
            // OPCODE 3: Unary arithmetic
            Instruction::Neg => {
                let mut stack_val = machine.stack_pop();
                //println!("\tNEG: Popped Val: {}",stack_val);
                stack_val *= -1;
                //println!("\tNEG: Negated Val: {}",stack_val);

                machine.stack_push(stack_val);
            },
            // OPCODE 3: Unary arithmetic
            Instruction::Not => {
                let mut stack_val = machine.stack_pop();
                //println!("\tNOT: Popped Val: {}",stack_val);
                //println!("\tNOT: Popped Hex Val: {:08x}",stack_val);

                stack_val = !stack_val;
                //println!("\tNOT: Negated Val: {}",stack_val);
                //println!("\tNOT: Negated Hex Val: {:08x}",stack_val);

                machine.stack_push(stack_val);
            },
            // OPCODE 4: String print
            Instruction::StPrint(bytes) => {
                let raw = u32::from_le_bytes(*bytes) & 0x0FFFFFFC;
                let sro = if (raw & (1 << 27)) != 0
                {
                    (raw | 0xF0000000) as i32
                }
                else
                {
                    raw as i32
                };

                let mut index = machine.get_stack_pointer() as i32 + sro;
                //println!("STPRINT index: {}", index);
                let mut b: u8;
                loop 
                {
                    if index > 4095 {break;} // out of memory

                    b = *machine.get_byte(index as usize);
                    index += 1;
                    //println!("{:08b}", b);
                    match b
                    {
                        0 => break,
                        1 => continue,
                        _ => 
                        {
                            print!("{}", b as char)
                        }
                    }
                }
                std::io::stdout().flush().unwrap();
            },
            // OPCODE 5: Call instructions
            Instruction::Call(bytes) => {
                let raw = u32::from_le_bytes(*bytes) & 0x0FFFFFFC;
                //println!("\tCall raw: {}", raw);
                let pc_ro = if (raw & 1 << 27) != 0
                {
                    (raw | 0xF0000000) as i32
                }
                else
                {
                    raw as i32
                };
                //if pc_ro == 0 {pc_ro = machine.get_program_counter() as i32;}
                //println!("\tCALL: PC {}", machine.get_program_counter() + 4);
                machine.stack_push((machine.get_program_counter() + 4) as i32);
                machine.pc_jump( (machine.get_program_counter() as i32 + pc_ro) as usize);
                //println!("\tCALL: pushed instruction: {} to SP {}", (machine.get_program_counter() + 4), machine.get_stack_pointer());
            },
            // OPCODE 6: Return instructions
            Instruction::Return(bytes) => {
                let raw = u32::from_le_bytes(*bytes) & 0x0FFFFFFC;
                let sro = if (raw & (1 << 27)) != 0
                {
                    (raw | 0xF0000000) as i32
                }
                else
                {
                    raw as i32
                };
                //let ret_addr = machine.peek((machine.get_stack_pointer() as i32 + sro) as usize);
                // println!("\tRETURN sro: {} SP: {}", sro, machine.get_stack_pointer() - 4);
                //machine.sp_jump((4096 - sro) as usize);
                machine.sp_jump((machine.get_stack_pointer() as i32 + sro) as usize);
                //machine.sp_jump(sro as usize);
                // println!("\tRETURN SP: {}", machine.get_stack_pointer());
                let ret_addr = machine.stack_pop();
                // println!("\tRETURN: popped {} from {}", ret_addr as usize, machine.get_stack_pointer() - 4);
                machine.pc_jump(ret_addr as usize);
            },
            // OPCODE 7: Unconditional goto
            Instruction::Goto(bytes) => {
                let raw = u32::from_le_bytes(*bytes) & 0x0FFFFFFC;
                let pc_ro = if (raw & 1 << 27) != 0
                {
                    (raw | 0xF0000000) as i32
                }
                else
                {
                    raw as i32
                };
                let target = machine.get_program_counter() as i32 + pc_ro;
                machine.pc_jump(target as usize);
                //println!("\tGOTO: pc_ro: {} target: {}", pc_ro, target);
            },
            Instruction::BinaryIf(bytes) => {
                let condition = (bytes[3] >> 1) & 0x7;
                let raw = u32::from_le_bytes(*bytes) & 0x00FF_FFFC;
                let pc_ro = if (raw & 1 << 23) != 0
                {
                    (raw | 0xFF00_0000) as i32
                }
                else
                {
                    raw as i32
                };
                let jump_target = machine.get_program_counter() as i32 + pc_ro;
                let right = machine.peek(machine.get_stack_pointer());
                let left = machine.peek(machine.get_stack_pointer() + 4);                
                // println!("Bif: jump attempt to {} on condition {}", jump_target, condition);
                // println!("left: {} right: {}", left, right);
                let full_send = match condition
                {
                    0 => left == right,
                    1 => left != right,
                    2 => left <  right,
                    3 => left >  right,
                    4 => left <= right,
                    5 => left >= right,
                    _ => {machine.pc_increment(); return;}
                };

                if full_send
                {
                    machine.pc_jump(jump_target as usize);
                }
                else
                {
                    machine.pc_increment();
                }
            },
            Instruction::UnaryIf(bytes) => {
                let condition = (bytes[3] >> 1) & 0x3;
                let raw = u32::from_le_bytes(*bytes) & 0x00FFFFFC;
                let pc_ro = if (raw & 1 << 23) != 0
                {
                    (raw | 0xFF00_0000) as i32
                }
                else
                {
                    raw as i32
                };
                let jump_target = machine.get_program_counter() as i32 + pc_ro;
                let val = machine.peek(machine.get_stack_pointer());
                
                // println!("Uif: jump attempt to {} on condition {}", jump_target, condition);
                // println!("val: {}", val);
                let full_send = match condition
                {
                    0 => val == 0,
                    1 => val != 0,
                    2 => val <  0,
                    3 => val >  0,
                    _ => {machine.pc_increment(); return;}
                };
                if full_send
                {
                    machine.pc_jump(jump_target as usize);
                }
                else
                {
                    machine.pc_increment();
                }
            },
            /* THERE ARE NO INSTRUCTIONS FOR OPCODE 10 OR 11 */
            // OPCODE 12: Dup instructions
            // Mike - April 30th
            
            Instruction::Dup (offset) => {
                //println!("\tDUP: Provided offset: {}",offset);

                let offset_usize = *offset as usize;
                let peeked_val = machine.peek(machine.get_stack_pointer() + offset_usize);
                machine.stack_push(peeked_val);

                //println!("\tDUP: Pushing peeked val: {}",peeked_val);

            },
            // OPCODE 13: Print instructions

            Instruction::Print (offset, print_bit) => {
                let offset_usize = *offset as usize;
                let offset_val = machine.peek(machine.get_stack_pointer() + offset_usize);
                //println!("\tPRINT: Offset Num {}", offset);
                //println!("\tPRINT: Val peeked: {} | Print bit: {}",offset_val,print_bit);
                // Decimal
                match *print_bit {
                    0 => println!("{}", offset_val),                // Decimal
                    1 => println!("0x{:x}", offset_val),            // Hex
                    2 => println!("0b{:b}", offset_val),            // Binary
                    3 => println!("0o{:o}", offset_val),            // Octal
                    _ => println!("\tPRINT: Invalid format: {}", print_bit) 
                    // Note, ^ this should mathematically never ocur 
                }
            },
            // OPCODE 14: Dump instructions
            Instruction::Dump => {
                let stack_val = machine.get_stack_pointer();
                // println!("\tDUMP: stack_val = {}",stack_val);
                if stack_val >= 4096{
                    // println!("\tDUMP: Nothing on the stack to display, Performing as NOP");
                    // functionally a NOP
                }else{
                    // println!("\tDUMP: Executing...");
                    for i in (stack_val..4096).step_by(4){
                    // println!("\tDUMP: {:x}",machine.peek(i));
                        
                        println!("{:04x}: {:08x}",i,machine.peek(i));
                    }
                }
            },
            // OPCODE 15: Push instructions
            Instruction::Push(value) => {
                let v = value.unwrap_or(0);
                //println!("\tPUSH: Pushing value {}", v);
                machine.stack_push(v);
            },
        }
    }
}