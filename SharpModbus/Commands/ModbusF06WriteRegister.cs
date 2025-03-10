﻿using System;

namespace SharpModbus
{
    public class ModbusF06WriteRegister : IModbusCommand
    {
        private readonly byte slave;
        private byte stationid;
        private readonly ushort address;
        private readonly ushort value;

        public byte Code { get { return 6; } }
        public byte Slave { get { return slave; } }
        public ushort Address { get { return address; } }
        public ushort Value { get { return value; } }
        public int RequestLength { get { return 6; } }
        public int ResponseLength { get { return 6; } }
        public byte StationId 
        { 
            get { return stationid; } 
            set { stationid = value; } 
        }

        public ModbusF06WriteRegister(byte slave, ushort address, ushort value)
        {
            this.slave = slave;
            this.stationid = slave; // normally same as slave, but can be overridden if required to avoid exceptions.
            this.address = address;
            this.value = value;
        }

        public void FillRequest(byte[] request, int offset)
        {
            request[offset + 0] = slave;
            request[offset + 1] = 6;
            request[offset + 2] = ModbusHelper.High(address);
            request[offset + 3] = ModbusHelper.Low(address);
            request[offset + 4] = ModbusHelper.High(value);
            request[offset + 5] = ModbusHelper.Low(value);
        }

        public object ParseResponse(byte[] response, int offset)
        {
            Tools.AssertEqual(response[offset + 0], stationid, "Slave mismatch got {0} expected {1}");
            Tools.AssertEqual(response[offset + 1], 6, "Function mismatch got {0} expected {1}");
            Tools.AssertEqual(ModbusHelper.GetUShort(response, offset + 2), address, "Address mismatch got {0} expected {1}");
            Tools.AssertEqual(ModbusHelper.GetUShort(response, offset + 4), value, "Value mismatch got {0} expected {1}");
            return null;
        }

        public object ApplyTo(IModbusModel model)
        {
            model.setWO(slave, address, value);
            return null;
        }

        public void FillResponse(byte[] response, int offset, object value)
        {
            FillRequest(response, offset);
        }

        public override string ToString()
        {
            return string.Format("[ModbusF06WriteRegister Slave={0}, Address={1}, Value={2}]", slave, address, value);
        }
    }
}