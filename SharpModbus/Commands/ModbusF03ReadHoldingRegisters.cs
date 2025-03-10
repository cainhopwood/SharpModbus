﻿using System;

namespace SharpModbus
{
    public class ModbusF03ReadHoldingRegisters : IModbusCommand
    {
        private readonly byte slave;
        private byte stationid;
        private readonly ushort address;
        private readonly ushort count;

        public byte Code { get { return 3; } }
        public byte Slave { get { return slave; } }
        public ushort Address { get { return address; } }
        public ushort Count { get { return count; } }
        public int RequestLength { get { return 6; } }
        public int ResponseLength { get { return 3 + ModbusHelper.BytesForWords(count); } }
        public byte StationId 
        { 
            get { return stationid; } 
            set { stationid = value; } 
        }

        public ModbusF03ReadHoldingRegisters(byte slave, ushort address, ushort count)
        {
            this.slave = slave;
            this.stationid = slave; // normally same as slave, but can be overridden if required to avoid exceptions.
            this.address = address;
            this.count = count;
        }

        public void FillRequest(byte[] request, int offset)
        {
            request[offset + 0] = slave;
            request[offset + 1] = 3;
            request[offset + 2] = ModbusHelper.High(address);
            request[offset + 3] = ModbusHelper.Low(address);
            request[offset + 4] = ModbusHelper.High(count);
            request[offset + 5] = ModbusHelper.Low(count);
        }

        public object ParseResponse(byte[] response, int offset)
        {
            var bytes = ModbusHelper.BytesForWords(count);
            Tools.AssertEqual(response[offset + 0], stationid, "Slave mismatch got {0} expected {1}");
            Tools.AssertEqual(response[offset + 1], 3, "Function mismatch got {0} expected {1}");
            Tools.AssertEqual(response[offset + 2], bytes, "Bytes mismatch got {0} expected {1}");
            return ModbusHelper.DecodeWords(response, offset + 3, count);
        }

        public object ApplyTo(IModbusModel model)
        {
            return model.getWOs(slave, address, count);
        }

        public void FillResponse(byte[] response, int offset, object value)
        {
            var bytes = ModbusHelper.BytesForWords(count);
            response[offset + 0] = slave;
            response[offset + 1] = 3;
            response[offset + 2] = bytes;
            var data = ModbusHelper.EncodeWords((ushort[])value);
            ModbusHelper.Copy(data, 0, response, offset + 3, bytes);
        }

        public override string ToString()
        {
            return string.Format("[ModbusF03ReadHoldingRegisters Slave={0}, Address={1}, Count={2}]", slave, address, count);
        }
    }
}