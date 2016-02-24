/*
 * Assembler.cs
 *
 * Line assembler
 *
 * Copyright (c) 2004 Dan Boris
 *
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * at your option) any later version. See license.txt for full details.
 *
 */

using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace sim6502
{
	public class Assembler
	{
		private Hashtable OpcodeModes;		// Lookup of instruction/mode to opcode
		private Hashtable Instructions;		// Valid instructions
		private Hashtable AdmodeLen;		// Number of bytes in each addressing mode
		
		public String ErrMsg;				// Error message generated by assembler

		public Assembler()
		{	
			// Create tables
			OpcodeModes = new Hashtable();
			Instructions = new Hashtable();
			LoadOpcodes();

			// Setup number of bytes needed for each addressing mode
			AdmodeLen = new Hashtable();
			AdmodeLen.Add("IMP",1);
			AdmodeLen.Add("ACC",1);
			AdmodeLen.Add("IMM",2);
			AdmodeLen.Add("REL",2);
			AdmodeLen.Add("ZP",2);
			AdmodeLen.Add("ZPX",2);
			AdmodeLen.Add("ZPY",2);
			AdmodeLen.Add("INX",2);
			AdmodeLen.Add("INY",2);
			AdmodeLen.Add("ABS",3);
			AdmodeLen.Add("ABX",3);
			AdmodeLen.Add("ABY",3);
		}

		public int AsmLine(String line,Byte[] b,ushort addr)
		{
			string inst = "";
			string operand;
			string admode;
			byte opcode = 0;
			int n,offset;
			int value = 0;

			// Cleanup line
			line = line.Trim().ToUpper();

			// Find Instructions
			if (line.Length >= 3) inst = line.Substring(0,3);
			
			// Verify that instruction is valid
			if (!Instructions.ContainsKey(inst)) 
			{
				ErrMsg = "Invalid instruction.";
				return -1;
			}

			// Operand
			if (line.Trim().Length > 3) 
				operand = line.Substring(4).Trim();
			else	
				operand = "";
			
			// Determine address mode
			admode = ParseInstruction(operand,ref value);
			if (admode == "") return -1;
	
			// Find opcode
			if (!FindOpcode(ref opcode,ref admode,inst)) 
			{
				ErrMsg = "Invalid addressing mode for this instruction.";
				return -1;
			}
			
			b[0] = opcode;

			// Get number of bytes for this addressing mode
			n = Convert.ToInt32(AdmodeLen[admode]);

			// Handle branches
			if (admode == "REL") 
			{
				offset = value - (int)addr - 2;
				if (offset > 128 || offset < -128) 
				{
					ErrMsg = "Branch out of range.";
					return -1;
				}
				b[1] = (byte)offset;
			}
			else
			{
				if (n == 3) 
				{
					b[2] = (byte)(value / 256);
					b[1] = (byte)(value - (b[2] * 256));
				}
				if (n == 2) 
				{
					b[1] = (byte)value;
				}
			}

			return n;
		}

		
		/*-------------------------------------------------------------------------
		 * FindOpcode(ref byte opcode, reg string admode, string inst)
		 *	Function: Determines opcode for an instruction, also checks for alternate
		 *			  addressing modes.
		 *  Parameters: admode = Address mode determined through parsing
		 *				inst = Instruction to lookup
		 *  Returns (bool): true = found opcode/false = could not find opcode
		 *		     opcode: opcode value
		 *			 admone: Changed for alternate addressing modes
		 *-------------------------------------------------------------------------*/
		private bool FindOpcode(ref byte opcode,ref string admode,string inst) 
		{
			
			// Check for instruction/addressmode combo
			if (OpcodeModes.ContainsKey(inst+":"+admode)) 
			{
				opcode = Convert.ToByte(OpcodeModes[inst+":"+admode]);
				return true;
			}

			// Handle alternate modes
			switch (admode) 
			{	
				case "ABS":
					// Check for relative
					if (OpcodeModes.ContainsKey (inst+":REL")) 
					{
						opcode = Convert.ToByte(OpcodeModes[inst+":REL"]);
						admode = "REL";
						return true;
					}
					break;
				case "ZP":
					// Check for absolute
					if (OpcodeModes.ContainsKey(inst+":ABS")) 
					{
						opcode = Convert.ToByte(OpcodeModes[inst+":ABS"]);
						admode = "ABS";
						return true;
					}
				
					// Check for relative
					if (OpcodeModes.ContainsKey (inst+":REL")) 
					{
						opcode = Convert.ToByte(OpcodeModes[inst+":REL"]);
						admode = "REL";
						return true;
					}
					break;

				case "ZPX":
					// Check for absolute X
					if (OpcodeModes.ContainsKey(inst+":ABX")) 
					{
						opcode = Convert.ToByte(OpcodeModes[inst+":ABX"]);
						admode = "ABX";
						return true;
					}
					break;

				case "ZPY":
					// Check for absolute Y
					if (OpcodeModes.ContainsKey(inst+":ABY")) 
					{
						opcode = Convert.ToByte(OpcodeModes[inst+":ABY"]);
						admode = "ABX";
						return true;
					}
					break;

			}
			return false;

		}
		
		/*-----------------------------------------------------------------------
		 * ParseNumber(string op)
		 *	Function: Parses a number 
		 *  Parameters: op = number to parse
		 *  Returns (int): value of number (-1 = failure)
		 *---------------------------------------------------------------------- */
		private int ParseNumber(string op) 
		{	
		    int frombase;
			
			// Determine base
			switch (op.Substring(0,1))
			{
				case "$":		//Hex
					frombase = 16;
					break;
				case "O":		//Octal
					frombase = 8;
					break;
				case "%":		//Binary
					frombase = 2;
					break;
				default:		//Decimal
					frombase = 10;
					break;
			}
			
			// Attempt conversion
			try 
			{
				if (frombase == 10) 
					return(Convert.ToUInt16(op,frombase));
				else
					return(Convert.ToUInt16(op.Substring(1),frombase));	
			}
			catch {
				// Could not do conversion
				return(-1);	
			}
		}
		
		

		/*-----------------------------------------------------------------------
		 * ParseInstruction(string op, int value)
		 *	Function: Parses instruction to determine addressing mode and operand value
		 *  Parameters: op = instruction to parse
		 *  Returns (string): addressing mode
		 *			  value : operand value
		 *---------------------------------------------------------------------- */
		private string ParseInstruction(string op,ref int value) 
		{
			int n;

			// Implied
			if (op == "") return("IMP");

			// Accumulator
			if (op == "A") return ("ACC");

			// Immediate
			if (op.StartsWith("#")) 
			{
				value = ParseNumber(op.Substring(1));

				// Check for errors
				if (value == -1) 
				{
					ErrMsg = "Syntax error.";
					return("");
				}

				if (value > 0xFF) 
				{
					ErrMsg = "Value to large for this addressing mode.";
					return("");
				}
				return("IMM");
			}
			
			// Indirect X
			if ((n = op.IndexOf(",X)")) > -1)
			{
				value = ParseNumber(op.Substring(1,n-1));

				// Check for errors
				if (value == -1) 
				{
					ErrMsg = "Syntax error.";
					return("");
				}

				if (value > 0xFF) 
				{
					ErrMsg = "Address to large for this addressing mode.";
					return("");
				}
				return("INX");
			}

			// Indirect Y
			if ((n = op.IndexOf("),Y")) > -1)
			{
				value = ParseNumber(op.Substring(1,n-1));

				// Check for errors
				if (value == -1) 
				{
					ErrMsg = "Syntax error.";
					return("");
				}

				if (value > 0xFF) 
				{
					ErrMsg = "Address to large for this addressing mode.";
					return("");
				}

				return("INY");
			}

			// Absolute or zero page X
			if (( n = op.IndexOf(",X")) > -1) 
			{
				value = ParseNumber(op.Substring(0,n));
				if (value == -1) 
				{
					ErrMsg = "Syntax error.";
					return("");
				}
				if (value < 0x100) 
					return("ZPX");
				else
					return("ABX");
			}
			
			// Absolute or zero page Y
			if ((n = op.IndexOf(",Y")) > -1) 
			{
				value = ParseNumber(op.Substring(0,n));
				if (value == -1) 
				{
					ErrMsg = "Syntax error.";
					return("");
				}
				if (value < 0x100) 
					return("ZPY");
				else
					return("ABY");
			}

			// Indirect
			if ((n = op.IndexOf(")")) > -1) 
			{
				value = ParseNumber(op.Substring(1,n-1));
				if (value == -1) 
				{
					ErrMsg = "Syntax error.";
					return("");
				}
				return("IND");
			}
			
			//Zero page or Absolute
			value = ParseNumber(op);
			if (value == -1) 
			{
				ErrMsg = "Syntax error.";
				return("");
			}
			
			if (value < 0x100) 
				return("ZP");
			else
				return("ABS");
		}
		
		/*---------------------------------------------------------------------------
		 * LoadOpcodes()
		 *	Function: Loads opcode translation table
		 *---------------------------------------------------------------------------*/

		private void LoadOpcodes() 
		{
			StreamReader sr;
			String s,k;
			Byte op;
			
			OpcodeModes.Clear();

			// Open opcodes file and create reader
			sr = new StreamReader("opcodes.txt");
			
			while ((s = sr.ReadLine()) != null)
			{
				if (s != "") 
				{
					// Create key "instruction:addmode"
					k = s.Substring(0,3) + ":" + s.Substring(4,3);

					// Get opcode
					op = Convert.ToByte(s.Substring(8,2),16);

					// Add to lookup table
					OpcodeModes.Add(k.Trim(),op);

					// Add instruction to instruction list
					if (!Instructions.ContainsKey(s.Substring(0,3)))
					{
						Instructions.Add(s.Substring(0,3),0);
					}
				}	
			}
			sr.Close();
		}

	}
}