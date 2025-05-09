main:
    # Test pop without arguments
    push 11
    push 22
    pop
    pop
    
    # Test pop with explicit offsets
    push 33
    push 44
    push 55
    pop 4
    pop 8
    pop
    
    # Test large pop offset
    push 66
    push 77
    push 88
    # Pop multiple values at once
    pop 12
    
    exit
