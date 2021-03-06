/*
 * Machine.cs 
 *
 * Handles the virtual machine being simulated
 *
 * Copyright (c) 2004 Dan Boris
 * Copyright � 2018 Neil McNeight
 *  
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * at your option) any later version. See license.txt for full details.
 *
 */

using System;
using System.Drawing;
using System.IO;
using System.Collections;

namespace SixtyFive
{
    /// <summary>
    /// 
    /// </summary>
    public class Machine
    {
        #region CPU
        public M6502 cpu;
        public AddressSpace mem;
        public ArrayList Devices;
        #endregion

        #region Execution control
        public bool stopped = false;
        public BreakPoints breakpoint;
        #endregion

        public Trace trace;

        /// <summary>
        /// Machine object constructor
        /// </summary>
        public Machine()
        {
            // Setup address spave
            mem = new AddressSpace
            {
                machine = this
            };

            // Setup CPU
            cpu = new M6502(mem);
            breakpoint = new BreakPoints();
            trace = new Trace();
            mem.breakpoint = breakpoint;

            Devices = new ArrayList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int interrupt()
        {
            return 0;
        }

        /// <summary>
        /// Run 1 instruction
        /// </summary>
        public void stepcpu()
        {

            // Handle tracing
            if (trace.traceon)
                trace.Write(cpu.Disassembler.Disassemble(cpu.PC));

            // Handle breakpoints
            if (breakpoint.CheckAddrBreak(cpu.PC))
            {
                stopped = true;
                return;
            }

            cpu.Execute(1);
        }

        /// <summary>
        /// Reset the machine
        /// </summary>
        public void Reset()
        {
            cpu.Reset();
            stopped = false;
        }

        #region Device handling
        public void AddDevice(IDevice dev)
        {
            Devices.Add(dev);
        }

        public void RemoveAllDevices()
        {
            Devices.Clear();
        }
        #endregion
    }
}
