﻿using System;

namespace SharpModbus
{
    public class ModbusF05WriteCoil : IModbusCommand
    {
        private readonly byte slave;
        private byte stationid;
        private readonly ushort address;
        private readonly bool value;

        public byte Code { get { return 5; } }
        public byte Slave { get { return slave; } }
        public ushort Address { get { return address; } }
        public bool Value { get { return value; } }
        public int RequestLength { get { return 6; } }
        public int ResponseLength { get { return 6; } }
        public byte StationId 
        { 
            get { return stationid; } 
            set { stationid = value; } 
        }

        public ModbusF05WriteCoil(byte slave, ushort address, bool state)
        {
            this.slave = slave;
            this.stationid = slave; // normally same as slave, but can be overridden if required to avoid exceptions.
            this.address = address;
            this.value = state;
        }

        public void FillRequest(byte[] request, int offset)
        {
            request[offset + 0] = slave;
            request[offset + 1] = 5;
            request[offset + 2] = ModbusHelper.High(address);
            request[offset + 3] = ModbusHelper.Low(address);
            request[offset + 4] = ModbusHelper.EncodeBool(value);
            request[offset + 5] = 0;
        }

        public object ParseResponse(byte[] response, int offset)
        {
            Tools.AssertEqual(response[offset + 0], stationid, "Slave mismatch got {0} expected {1}");
            Tools.AssertEqual(response[offset + 1], 5, "Function mismatch got {0} expected {1}");
            Tools.AssertEqual(ModbusHelper.GetUShort(response, offset + 2), address, "Address mismatch got {0} expected {1}");
            Tools.AssertEqual(response[offset + 4], ModbusHelper.EncodeBool(value), "Value mismatch got {0} expected {1}");
            Tools.AssertEqual(response[offset + 5], 0, "Pad mismatch {0} expected:{1}");
            return null;
        }

        public object ApplyTo(IModbusModel model)
        {
            model.setDO(slave, address, value);
            return null;
        }

        public void FillResponse(byte[] response, int offset, object value)
        {
            FillRequest(response, offset);
        }

        public override string ToString()
        {
            return string.Format("[ModbusF05WriteCoil Slave={0}, Address={1}, Value={2}]", slave, address, value);
        }
    }
}
