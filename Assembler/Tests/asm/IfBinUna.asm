
# Binary if instructions take a condition and a label
ifeq start
ifne end
iflt start
ifgt end
ifle start
ifge end

# Unary if instructions take a condition and a label
ifez start
ifnz end
ifpl start
ifmi end


start:
    exit

end:
    exit