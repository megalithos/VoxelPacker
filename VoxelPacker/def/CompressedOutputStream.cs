using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace VoxelPacker
{
    public struct CompressedOutputStream
    {
        public CompressedOutputStream(byte[] buffer, int bufferOffset, int bufferSizeInBytes = int.MaxValue)
        {
            m_Buffer = buffer;
            m_BufferOffset = bufferOffset;
            m_CurrentBitIndex = 0;
            m_CurrentByteIndex = bufferOffset;
            m_BitBuffer = 0;
            m_BufferSize = bufferSizeInBytes;
        }

        public void WriteRawBits(uint value, int numbits)
        {
            WriteRawBitsInternal(value, numbits);
            FlushBits();
        }

        unsafe public void WriteRawBytes(byte* value, int count)
        {
            for (int i = 0; i < count; i++)
                WriteRawBits(value[i], 8);
        }

        public unsafe void WritePackedUInt(uint value)
        {
            int writtenBytesCount = BitPackUtility.WriteVarU64(out ulong output, value);

            WriteRawBitsInternal((ulong)output, writtenBytesCount * 8);

            FlushBits();
        }

        public void WritePackedUIntDelta(uint value, uint baseline)
        {
            int diff = (int)(baseline - value);
            uint interleaved = (uint)((diff >> 31) ^ (diff << 1));
            WritePackedUInt(interleaved);
        }

        public int Flush()
        {
            while (m_CurrentBitIndex > 0)
            {
                m_Buffer[m_CurrentByteIndex++] = (byte)m_BitBuffer;
                m_CurrentBitIndex -= 8;
                m_BitBuffer >>= 8;
            }
            m_CurrentBitIndex = 0;
            return m_CurrentByteIndex - m_BufferOffset;
        }

        void WriteRawBitsInternal(ulong value, int numbits)
        {
            int bitsLeftToWrite = numbits;
            ulong unwrittenBuffer = value;
            while (bitsLeftToWrite > 0)
            {
                int bitWriteAmount = Math.Min(bitsLeftToWrite, 8);
                int newBitIndex = m_CurrentBitIndex + bitWriteAmount;
                if (m_CurrentByteIndex + Math.Ceiling((newBitIndex) / 8.0f) > m_BufferSize)
                {
                    throw new Exception("out of buffer space");
                }
                m_BitBuffer |= (unwrittenBuffer & (ulong)((1 << bitWriteAmount) - 1)) << m_CurrentBitIndex;
                unwrittenBuffer >>= bitWriteAmount;
                bitsLeftToWrite -= bitWriteAmount;
                m_CurrentBitIndex = newBitIndex;
            }
        }

        void FlushBits()
        {
            while (m_CurrentBitIndex >= 8)
            {
                if (m_CurrentByteIndex < 0 || m_CurrentByteIndex >= m_Buffer.Length)
                {
                    throw new IndexOutOfRangeException($"Index: {m_CurrentByteIndex} was out of range for length: {m_Buffer.Length}");
                }

                m_Buffer[m_CurrentByteIndex++] = (byte)m_BitBuffer;
                m_CurrentBitIndex -= 8;
                m_BitBuffer >>= 8;
            }
        }

        public byte[] m_Buffer;
        public int m_BufferOffset;
        public ulong m_BitBuffer;
        public int m_CurrentBitIndex;
        public int m_CurrentByteIndex;
        public int m_BufferSize;
    }
}