using System;

public struct Bool8
{
    byte _value;

    public bool this[int index]
    {
        get
        {
            if ((uint)index >= 8)
            {
                throw new IndexOutOfRangeException();
            }

            return (_value & (1 << index)) != 0;
        }
        set
        {
            if ((uint)index >= 8)
            {
                throw new IndexOutOfRangeException();
            }
            if (value)
            {
                _value |= (byte)(1 << index);
            }
            else
            {
                _value &= (byte)~(1 << index);

            }
        }
    }

    public byte ByteValue
    {
        get => _value;
    }

    public Bool8(byte val)
    {
        _value = val;
    }
}
