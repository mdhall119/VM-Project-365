main:
    goto first
    stpush "123"
    goto second
    stpush "456"
    goto third
    stpush "789"


first:
    nop
    nop
    nop

third:
    nop
    nop
    nop
    exit 1

second:
    nop
    nop
    nop 