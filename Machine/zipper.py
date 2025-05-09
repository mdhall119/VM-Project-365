#!/usr/bin/env python3
import sys
import os
import shutil
import zipfile
import argparse

IGNORED_FILES = ['.DS_STORE', 'helper.rs']

def ignore_files(directory, contents):
    return [item for item in contents if item.lower() in [name.lower() for name in IGNORED_FILES]]

def lab_mode(netid):
    # Lab mode: zip current directory excluding specific items, placing files under folder netid
    print(f"Lab Mode:\nZip the current dir into folder {netid} within {netid}.zip")
    zip_filename = f"{netid}.zip"
    current_dir = os.getcwd()
    script_path = os.path.abspath(__file__)
    zip_abs_path = os.path.abspath(os.path.join(current_dir, zip_filename))
    with zipfile.ZipFile(zip_filename, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(current_dir):
            # Exclude .git*, target, machine, and the netid folder
            dirs[:] = [d for d in dirs if d != netid and not (d.lower().startswith('.git') or d in {'target', 'machine'})]
            for file in files:
                if file.lower().startswith('.git'):
                    continue
                file_path = os.path.join(root, file)
                # Only exclude the zip file itself, not the zipper.py script
                if os.path.abspath(file_path) == zip_abs_path:
                    continue
                print(f"  adding: {file_path}")
                # Prefix the relative path with netid folder
                arcname = os.path.join(netid, os.path.relpath(file_path, current_dir))
                zipf.write(file_path, arcname)

def normal_mode(netid):
    # Normal mode: get it ready for submission
    print(f"Creating {netid}.zip")
    shutil.copy("Cargo.toml", netid)

    # Copy src directory preserving the structure and ignore .DS_Store
    src_dest = os.path.join(netid, "src")
    shutil.copytree("src", src_dest, ignore=ignore_files)

    # Zip the new directory
    zip_filename = f"{netid}.zip"
    with zipfile.ZipFile(zip_filename, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for root, dirs, files in os.walk(netid):
            for file in files:
                if file.lower() in [name.lower() for name in IGNORED_FILES]:
                    continue
                file_path = os.path.join(root, file)
                print(f"  adding: {file_path}")
                arcname = os.path.relpath(file_path, start=netid)
                zipf.write(file_path, arcname=os.path.join(netid, arcname))

def main():
    # Parse command-line arguments
    parser = argparse.ArgumentParser(description="Zip machine utility")
    parser.add_argument("netid", nargs="?", help="netid for submission")
    parser.add_argument("-l", "--lab", action="store_true", help="Enable lab mode")
    args = parser.parse_args()

    netid = args.netid
    if not netid:
        if os.path.isfile("netid.txt"):
            with open("netid.txt", "r") as f:
                netid = f.read().strip()
        else:
            netid = input("Enter your netid: ").strip()

    if not netid:
        print("Error: netid cannot be empty.")
        sys.exit(1)

    # Create a new directory if needed
    os.makedirs(netid, exist_ok=True)

    # Choose mode based on flag
    if args.lab:
        lab_mode(netid)
    else:
        normal_mode(netid)

    # Clean up
    shutil.rmtree(netid)
    print(f"Created {netid}.zip successfully.")

if __name__ == "__main__":
    main()
