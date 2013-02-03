
MCS=mcs

RESOURCES+= -resource:Tables/z80.table,sass.Tables.z80.table

all: Assembler

Assembler:
	$(MCS) $(RESOURCES) *.cs

clean:
	rm Assembler.exe
