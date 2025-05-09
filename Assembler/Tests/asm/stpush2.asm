# stpush is the only pseudo instruction, but it should expand to one
# push for each 3 characters. It also needs to support three escapes \\, \n, and \"
stpush "Hello\\ \"World\"\n"
stpush "Quite the verbose statement"
stpush " "
stpush "Quite the weir" input"\\style\t"

