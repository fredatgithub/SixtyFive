/*
 * Copyright (c) 2003 Mike Murphy
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * at your option) any later version. See license.txt for full details.
 *
namespace sim6502 {
public sealed class M6502 {
	private delegate void OpcodeHandler();
	// There is jamming behavior to be implemented
	private bool IsJammed;
	
	public enum Flags : byte
	{
		C = 0x01, Z = 0x02, I = 0x04, D = 0x08, B = 0x10, V = 0x40, N = 0x80
	}

	private const ushort
	// 16-bit register
	// 8-bit registers
	public M6502DASM Disassembler;

	public void Reset() {
		// clear the stack
		// reset the program counter
	public override String ToString() {
	public void Execute(int count) {

	public void Trigger_IRQ_interrupt() {
	public void Trigger_NMI_interrupt() {
		InstallOpcodes();
		// be safe; some devices capture clocks w/negative offsets
		// force explicit Reset() to get CPU going
		// initialize processor status, bit 5 is always set
		
		machine = mem.machine;
	byte MSB(ushort u16) {
	byte LSB(ushort u16) {
	ushort WORD(byte lsb, byte msb) {
	// Processor Status Flag Bits

	// Flag bit setters and getters
	void fset(byte flag, bool value) {
	bool fget(byte flag) {
	bool fC {
	// Zero: set if the result of the last operation was zero
	bool fZ {
	// Irq Disable: set if maskable interrupts are disabled
	bool fI {
	// Decimal Mode: set if decimal mode active
	bool fD {
	// Brk: set if an interrupt caused by a BRK instruction,
	bool fB {
	// Overflow: set if the addition of two-like-signed numbers
	bool fV {
	// Negative: set if bit 7 of the accumulator is set
	bool fN {
	void set_fNZ(byte u8) {
	byte pull() {
	void push(byte data) {
	void clk(int ticks) {
	void interrupt(ushort intr_vector, bool isExternal) {
	void br(bool cond, ushort ea) {

	// Relative: Bxx $aa  (branch instructions only)
	// Zero Page: $aa
	// Zero Page Indexed,X: $aa,X
	// Zero Page Indexed,Y: $aa,Y
	// Absolute: $aaaa
	// Absolute Indexed,X: $aaaa,X
		if (LSB(ea) + X > 0xff) {
	// Absolute Indexed,Y: $aaaa,Y
		if (LSB(ea) + Y > 0xff) {
		return (ushort)(ea + Y);
	// Indexed Indirect: ($aa,X)
	}
	// Indirect Indexed: ($aa),Y
		if (lsb + Y > 0xff) {
		return (ushort)(WORD(lsb, msb) + Y);
	// Indirect Absolute: ($aaaa)    (only used by JMP)
	// aACC = Accumulator
	// ADC: Add with carry
	// AND: Logical and
	// ASL: Arithmetic shift left: C <- [7][6][5][4][3][2][1][0] <- 0
	// BIT: Bit test
	// BRK Force Break  (cause software interrupt)
	// CLC: Clear carry flag
	// CLD: Clear decimal mode
	// CLI: Clear interrupt disable */
	// CLV: Clear overflow flag
	// CMP: Compare accumulator
	// CPX: Compare index X
	// CPY: Compare index Y
	// DEC: Decrement memory
	// DEX: Decrement index x
	// DEY: Decrement index y
	// EOR: Logical exclusive or
	// INC: Increment memory
	// INX: Increment index x
	// INY: Increment index y
	// JMP Jump to address
	// JSR Jump to subroutine
	// LDA: Load accumulator
	// LDX: Load index X
	// LDY: Load index Y
	// LSR: Logic shift right: 0 -> [7][6][5][4][3][2][1][0] -> C
	// NOP: No operation
	// ORA: Logical inclusive or
	// PHA: Push accumulator
	// PHP: Push processor status (flags)
	// PLA: Pull accumuator
	// PLP: Pull processor status (flags)
	// ROL: Rotate left: new C <- [7][6][5][4][3][2][1][0] <- C
		fC = (mem & 0x80) != 0;
	// ROR: Rotate right: C -> [7][6][5][4][3][2][1][0] -> new C
		fC = (mem & 0x01) != 0;
	// RTI: Return from interrupt
	// RTS: Return from subroutine
	// SBC: Subtract with carry (borrow)
		if (fD) {
	// SEC: Set carry flag
	// SED: Set decimal mode
	// SEI: Set interrupt disable
	// STA: Store accumulator
	// STX: Store index X
	// STY: Store index Y
	// TAX: Transfer accumlator to index X
	// TAY: Transfer accumlator to index Y
	// TSX: Transfer stack to index X
	// TXA: Transfer index X to accumlator
	// TXS: Transfer index X to stack
	// TYA: Transfer index Y to accumulator
	private ushort EA;
	void op65() { EA = aZPG();  clk(3); iADC(Mem[EA]); }
	void op24() { EA = aZPG();  clk(3); iBIT(Mem[EA]); }
	void op10() { EA = aREL();  clk(2); br(!fN, EA); /* BPL */ }
	void op00() {    /*aIMP*/   clk(7); iBRK(); }
	void opd8() {    /*aIMP*/   clk(2); iCLD(); }
	void op58() {    /*aIMP*/   clk(2); iCLI(); }
	void opb8() {    /*aIMP*/   clk(2); iCLV(); }
	void opc5() { EA = aZPG();  clk(3); iCMP(Mem[EA]); }
	void ope4() { EA = aZPG();  clk(3); iCPX(Mem[EA]); }
	void opc4() { EA = aZPG();  clk(3); iCPY(Mem[EA]); }
	void opc6() { EA = aZPG();  clk(5); Mem[EA] = iDEC(Mem[EA]); }
	void opca() {    /*aIMP*/   clk(2); iDEX(); }
	void op88() {    /*aIMP*/   clk(2); iDEY(); }
	void op45() { EA = aZPG();  clk(3); iEOR(Mem[EA]); }
	void ope6() { EA = aZPG();  clk(5); Mem[EA] = iINC(Mem[EA]); }
	void ope8() {    /*aIMP*/   clk(2); iINX(); }
	void opc8() {    /*aIMP*/   clk(2); iINY(); }
	void opa5() { EA = aZPG();  clk(3); iLDA(Mem[EA]); }
	void opa6() { EA = aZPG();  clk(3); iLDX(Mem[EA]); }
	void opa4() { EA = aZPG();  clk(3); iLDY(Mem[EA]); }
	void op46() { EA = aZPG();  clk(5); Mem[EA] = iLSR(Mem[EA]); }
	void op4c() { EA = aABS();  clk(3); iJMP(EA); }
	void op20() { EA = aABS();  clk(6); iJSR(EA); }
	void opea() {    /*aIMP*/   clk(2); iNOP(); }
	void op05() { EA = aZPG();  clk(3); iORA(Mem[EA]); }
	void op48() {    /*aIMP*/   clk(3); iPHA(); }
	void op68() {    /*aIMP*/   clk(4); iPLA(); }
	void op08() {    /*aIMP*/   clk(3); iPHP(); }
	void op28() {    /*aIMP*/   clk(4); iPLP(); }
	void op26() { EA = aZPG();  clk(5); Mem[EA] = iROL(Mem[EA]); }
	void op66() { EA = aZPG();  clk(5); Mem[EA] = iROR(Mem[EA]); }
	void op40() {    /*aIMP*/   clk(6); iRTI(); }
	void op60() {    /*aIMP*/   clk(6); iRTS(); }
	void ope5() { EA = aZPG();  clk(3); iSBC(Mem[EA]); }
	void op38() {    /*aIMP*/   clk(2); iSEC(); }
	void opf8() {    /*aIMP*/   clk(2); iSED(); }
	void op78() {    /*aIMP*/   clk(2); iSEI(); }
	void op85() { EA = aZPG();  clk(3); Mem[EA] = iSTA(); }
	void op86() { EA = aZPG();  clk(3); Mem[EA] = iSTX(); }
	void op84() { EA = aZPG();  clk(3); Mem[EA] = iSTY(); }
	void opaa() {    /*aIMP*/   clk(2); iTAX(); }
	void opa8() {    /*aIMP*/   clk(2); iTAY(); }
	void opba() {    /*aIMP*/   clk(2); iTSX(); }
	void op8a() {    /*aIMP*/   clk(2); iTXA(); }
	void op9a() {    /*aIMP*/   clk(2); iTXS(); }
	void op98() {    /*aIMP*/   clk(2); iTYA(); }
	void opXX() {
	private void InstallOpcodes() {