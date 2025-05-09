# Complex Goto Test
# This test exercises multiple goto patterns and label references

main:
    input
    input
    add
    print
    goto func1
    input
    print
    exit

func1:
    input
    print
    goto func2

func4:
    nop
    nop
    nop
    nop
    nop
    nop
    goto func5

func2:
    goto func3

func3:
    goto func4

func5:
    input
    input
    mul
    print
    exit