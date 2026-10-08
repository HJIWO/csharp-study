using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PressureMap.Common
{
    public class Frame
    {
        public int FrameNo { get; }
        public DateTime Timestamp { get; }
        public ushort[,] Values { get; }
        public Frame(int frameNo, DateTime timestamp, ushort[,] values)
        {
            FrameNo = frameNo;
            Timestamp = timestamp;
            Values = values;
        }

        public const int ByteSize = 4 + 8 + Config.CellCount * 2;   // 524

        public byte[] ToBytes()
        {
            using (var ms = new MemoryStream(ByteSize))
            using (var writer = new BinaryWriter(ms))
            {
                writer.Write(FrameNo);
                writer.Write(Timestamp.ToBinary());
                for (int r = 0; r < Config.Rows; r++)
                    for (int c = 0; c < Config.Cols; c++)
                        writer.Write(Values[r, c]);
                return ms.ToArray();
            }
        }
        public static Frame FromBytes(byte[] data)
        {
            if (data.Length != ByteSize)
                throw new ArgumentException("프레임은 " + ByteSize + "바이트여야 합니다.");

            using (var ms = new MemoryStream(data))
            using (var reader = new BinaryReader(ms))
            {
                int frameNo = reader.ReadInt32();
                DateTime timestamp = DateTime.FromBinary(reader.ReadInt64());
                var values = new ushort[Config.Rows, Config.Cols];
                for (int r = 0; r < Config.Rows; r++)
                    for (int c = 0; c < Config.Cols; c++)
                        values[r, c] = reader.ReadUInt16();
                return new Frame(frameNo, timestamp, values);
            }
        }

    }

}
