PROJECT=Main.csproj
GROUP_NAME=vmas3

all: build

build:
	@if command -v mcs &> /dev/null; then \
		mcs -langversion:7.2 -out:assembler.exe $$(find . -name "*.cs"); \
	else \
		dotnet build $(PROJECT); \
	fi

clean:
	@if command -v dotnet &> /dev/null; then \
		dotnet clean $(PROJECT); \
	else \
		rm -rf bin obj assembler.exe; \
	fi

grade:
	./gradescript

zip:
	./zipper $(GROUP_NAME)
