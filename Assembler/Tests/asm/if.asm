# Test file for if and uif instructions with numeric output
# Output: 1 = test passed, 0 = test failed

# ----- Binary If Tests -----
# Test each binary condition (eq, ne, lt, gt, le, ge)

# Test 1: equal condition (should jump)
push 10
push 10
ifeq equal_case
push 0  # Test failed if we get here
goto next_test1
equal_case:
    push 1  # Test passed

next_test1:
    # Test 2: less than condition (should jump)
    push 5
    push 10
    iflt less_than_case
    push 0

    goto next_test2
    less_than_case:
        push 2  # Test ID 2 passed

next_test2:
    # Test 3: greater than condition (should jump)
    push 15
    push 10
    ifgt greater_than_case
    push 0

    goto next_test3
    greater_than_case:
    push 3  # Test ID 3 passed

next_test3:
    # Test 4: not equal condition (should jump)
    push 10
    push 5
    ifne not_equal_case
    push 0

    goto next_test4
    not_equal_case:
    push 4  # Test ID 4 passed

next_test4:
    # Test 5: less than or equal (should jump)
    push 10
    push 10
    ifle less_equal_case
    push 0

    goto next_test5
    less_equal_case:
        push 5  # Test ID 5 passed

next_test5:
    # Test 6: greater than or equal (should jump)
    push 10
    push 5
    ifge greater_equal_case
    push 0

    goto unary_tests
    greater_equal_case:
        push 6  # Test ID 6 passed

# ----- Unary If Tests -----
unary_tests:
    # Test 7: equals zero (should jump)
    push 0
    ifez equals_zero_case
    push 0

    goto next_utest1
equals_zero_case:
    push 7  # Test ID 7 passed

next_utest1:
    # Test 8: not equals zero (should jump)
    push 42
    ifnz not_zero_case
    push 0

    goto next_utest2
not_zero_case:
    push 8  # Test ID 8 passed

next_utest2:
    # Test 9: negative (should jump)
    push -5
    ifmi negative_case
    push 0

    goto next_utest3
    negative_case:
        push 9  # Test ID 9 passed

next_utest3:
    # Test 10: positive (should jump)
    push 5
    ifpl positive_case
    push 0

    goto end_tests
    positive_case:
        push 10  # Test ID 10 passed

# Test conditions that should NOT jump
end_tests:
    # Test 11: not equal condition that shouldn't jump
    push 10
    push 10
    ifne should_not_jump1
    push 0

    goto test2
should_not_jump1:
    push 11  # Test ID 11 failed

test2:
    # Test 12: zero test that shouldn't jump
    push 10
    ifez should_not_jump2
    push 12  # Test ID 12 passed

    goto exit
should_not_jump2:
    push 0

exit:
    exit 0
