SRC = src/main.rs
BIN = machine

all: build

build: $(SRC)
	rustc -o $(BIN) $(SRC)

clean:
	rm -rf $(BIN) target/
