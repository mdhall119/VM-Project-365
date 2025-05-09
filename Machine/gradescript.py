#!/usr/bin/env python3

# pip install colorama

import os
import sys
import glob
import argparse
import subprocess
import filecmp
import colorama
from colorama import Fore, Style

# Initialize colorama for cross-platform support
colorama.init()

class Colors:
    def __init__(self, use_colors=True):
        self.use_colors = use_colors
        if not use_colors:
            # No-op functions if colors are disabled
            self.blue = self.red = self.green = self.yellow = lambda text: text

    def blue(self, text):
        return f"{Fore.BLUE}{text}{Style.RESET_ALL}" if self.use_colors else text

    def red(self, text):
        return f"{Fore.RED}{text}{Style.RESET_ALL}" if self.use_colors else text

    def green(self, text):
        return f"{Fore.GREEN}{text}{Style.RESET_ALL}" if self.use_colors else text

    def yellow(self, text):
        return f"{Fore.YELLOW}{text}{Style.RESET_ALL}" if self.use_colors else text

def parse_args():
    parser = argparse.ArgumentParser(description='Run tests for the machine')
    parser.add_argument('--no-colors', action='store_true', help='Disable colored output')
    return parser.parse_args()

def run_tests(colors):
    # Define directories
    input_dir = "Tests/input"
    v_dir = "Tests/v"
    output_dir = "Tests/output"
    our_dir = "Tests/our"
    machine_bin = "./machine"

    # I just assume that windows used exes
    # Adjust binary name for Windows
    # if sys.platform == 'win32':
    #     machine_bin = ".\\machine.exe"
    
    # Make sure our output directory exists
    os.makedirs(our_dir, exist_ok=True)
    
    # Clear the our directory but keep the .keep file
    for file in os.listdir(our_dir):
        if file != ".keep" and os.path.isfile(os.path.join(our_dir, file)):
            os.remove(os.path.join(our_dir, file))
    
    passed = 0
    failed = 0
    
    # Run all tests
    for input_file in glob.glob(os.path.join(v_dir, "*.v")):
        # Get the base filename without extension
        base_filename = os.path.basename(input_file)[:-2]
        
        # Check for base named input file (e.g., abs.txt)
        base_in_file = os.path.join(input_dir, f"{base_filename}.txt")
        if os.path.isfile(base_in_file):
            base_out_file = os.path.join(our_dir, f"{base_filename}.txt")
            ref_out_file = os.path.join(output_dir, f"{base_filename}.txt")
            
            print(colors.blue(f"Running test: {base_filename}"))
            
            # Run the machine binary with input redirection
            with open(base_in_file, 'r') as infile, open(base_out_file, 'w') as outfile:
                subprocess.run([machine_bin, input_file], stdin=infile, stdout=outfile, text=True)
            
            # Compare with reference output
            if os.path.isfile(ref_out_file) and filecmp.cmp(base_out_file, ref_out_file):
                print(colors.green(f"Test passed: {base_filename}"))
                passed += 1
            else:
                print(colors.red(f"Test failed: {base_filename}"))
                failed += 1
        
        # Check for numbered input files (e.g., abs-01.txt, abs-02.txt)
        numbered_files = glob.glob(os.path.join(input_dir, f"{base_filename}-*.txt"))
        for numbered_in_file in numbered_files:
            # Extract the suffix (e.g., "-01")
            suffix = os.path.basename(numbered_in_file)[:-4].replace(base_filename, '')
            numbered_out_file = os.path.join(our_dir, f"{base_filename}{suffix}.txt")
            ref_out_file = os.path.join(output_dir, f"{base_filename}{suffix}.txt")
            
            print(colors.blue(f"Running test: {base_filename}{suffix}"))
            
            # Run the machine binary with input redirection
            with open(numbered_in_file, 'r') as infile, open(numbered_out_file, 'w') as outfile:
                subprocess.run([machine_bin, input_file], stdin=infile, stdout=outfile, text=True)
            
            # Compare with reference output
            if os.path.isfile(ref_out_file) and filecmp.cmp(numbered_out_file, ref_out_file):
                print(colors.green(f"Test passed: {base_filename}{suffix}"))
                passed += 1
            else:
                print(colors.red(f"Test failed: {base_filename}{suffix}"))
                failed += 1
        
        # If no input files found, run without input
        if not os.path.isfile(base_in_file) and not numbered_files:
            out_file = os.path.join(our_dir, f"{base_filename}.txt")
            ref_out_file = os.path.join(output_dir, f"{base_filename}.txt")
            
            print(colors.blue(f"Running test: {base_filename} (no input)"))
            
            # Run the machine binary without input
            with open(out_file, 'w') as outfile:
                subprocess.run([machine_bin, input_file], stdout=outfile, text=True)
            
            # Compare with reference output
            if os.path.isfile(ref_out_file) and filecmp.cmp(out_file, ref_out_file):
                print(colors.green(f"Test passed: {base_filename}"))
                passed += 1
            else:
                print(colors.red(f"Test failed: {base_filename}"))
                failed += 1
    
    # Print totals
    print()
    print(f"{passed} correct out of {passed + failed}")
    
    return passed, failed

def main():
    args = parse_args()
    colors = Colors(not args.no_colors)
        
    passed, failed = run_tests(colors)
    
    # Return non-zero exit code if any tests failed
    if failed > 0:
        sys.exit(1)

if __name__ == "__main__":
    main()