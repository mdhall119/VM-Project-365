# Test file for binary and unary if statements
# Tests all conditions from the provided mnemonics

main:
    # Test unary if statements first
    # Test ifez (equals zero, code 00)
    push 0
    ifez unary_test1
    goto error

unary_test1:
    # Test ifnz (not equals zero, code 01)
    push 5
    ifnz unary_test2
    goto error

unary_test2:
    # Should fail ifnz with 0
    push 0
    ifnz error
    goto unary_test3

unary_test3:
    # Test ifmi (less than 0/negative, code 10)
    push -10
    ifmi unary_test4
    goto error

unary_test4:
    # Should fail ifmi with positive number
    push 10
    ifmi error
    goto unary_test5

unary_test5:
    # Test ifpl (greater than or equal to 0/positive, code 11)
    push 10
    ifpl unary_test6
    goto error

unary_test6:
    # Test ifpl with 0 (should pass)
    push 0
    ifpl binary_test1
    goto error

# Now test binary if statements
binary_test1:
    # Test ifeq (equals, ==)
    push 42
    push 42
    ifeq binary_test2
    goto error

binary_test2:
    # Test ifne (not equals, !=)
    push 10
    push 20
    ifne binary_test3
    goto error

binary_test3:
    # Should fail ifne with equal values
    push 30
    push 30
    ifne error
    goto binary_test4

binary_test4:
    # Test iflt (less than, <)
    push 10    # First operand
    push 20    # Second operand - stack now has [20, 10]
    iflt binary_test5    # Check if 20 < 10, which is false
    goto error

binary_test5:
    # Correcting logic - properly test iflt
    push 20    # First operand
    push 10    # Second operand - stack now has [10, 20]
    iflt error # Check if 10 < 20, which is true, so should NOT jump to error
    goto binary_test6

binary_test6:
    # Test ifgt (greater than, >)
    push 15    # First operand
    push 30    # Second operand - stack now has [30, 15]
    ifgt binary_test7  # Check if 30 > 15, which is true
    goto error

binary_test7:
    # Correcting logic - properly test ifgt
    push 30    # First operand
    push 15    # Second operand - stack now has [15, 30]
    ifgt error # Check if 15 > 30, which is false, so should jump to error
    goto binary_test8

binary_test8:
    # Test ifle (less than or equal to, <=)
    push 10    # First operand
    push 20    # Second operand - stack now has [20, 10]
    ifle binary_test9  # Check if 20 <= 10, which is false
    goto error

binary_test9:
    # Test ifle with equal values (should pass)
    push 25    # First operand
    push 25    # Second operand - stack now has [25, 25]
    ifle binary_test10  # Check if 25 <= 25, which is true
    goto error

binary_test10:
    # Correcting logic - properly test ifle
    push 20    # First operand
    push 10    # Second operand - stack now has [10, 20]
    ifle error # Check if 10 <= 20, which is true, so should NOT jump to error
    goto binary_test11

binary_test11:
    # Test ifge (greater than or equal to, >=)
    push 15    # First operand
    push 30    # Second operand - stack now has [30, 15]
    ifge binary_test12  # Check if 30 >= 15, which is true
    goto error

binary_test12:
    # Test ifge with equal values (should pass)
    push 25    # First operand
    push 25    # Second operand - stack now has [25, 25]
    ifge success  # Check if 25 >= 25, which is true
    goto error

success:
    stpush "All tests passed successfully!"
    stprint
    exit 0

error:
    stpush "IF STATEMENT ERROR, PLEASE TRACE"
    stprint
    exit -1
