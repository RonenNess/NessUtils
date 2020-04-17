
namespace Ness.Utils
{
    /// <summary>
    /// A buffer of bytes that you can easily push values into, built in a way to be serialized by client side.
    /// </summary>
    public class BinaryBuffer
    {
        // the actual buffer
        public byte[] _buff;

        /// <summary>
        /// Offset in buffer.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Create the binary buffer from buff or null.
        /// </summary>
        public BinaryBuffer(byte[] buff)
        {
            _buff = buff;
            Offset = 0;
        }

        /// <summary>
        /// Reset offset to 0.
        /// </summary>
        public void Reset()
        {
            Offset = 0;
        }

        /// <summary>
        /// Set buffer and offset.
        /// </summary>
        public void SetBuffer(byte[] buff, int offset)
        {
            Offset = offset;
            _buff = buff;
        }

        /// <summary>
        /// Push short value to buffer.
        /// </summary>
        public void PushShort(short val)
        {
            _buff[Offset++] = (byte)(val & 0xff);
            _buff[Offset++] = (byte)((val >> 8) & 0xff);
        }

        /// <summary>
        /// Push ushort value to buffer.
        /// </summary>
        public void PushUShort(ushort val)
        {
            _buff[Offset++] = (byte)(val & 0xff);
            _buff[Offset++] = (byte)((val >> 8) & 0xff);
        }

        /// <summary>
        /// Push string value to buffer.
        /// If encodeSize is true, will also push string size.
        /// If shortString is true, will push string size as byte.
        /// </summary>
        public void PushString(string txt, bool encodeSize, bool shortString, int sizeLimit = 0)
        {
            // handle null strings
            txt = txt ?? string.Empty;

            // add size limit
            if (sizeLimit != 0 && txt.Length > sizeLimit)
            {
                txt = txt.Substring(0, sizeLimit);
            }

            // encode string size as short
            if (encodeSize)
            {
                if (shortString) PushByte((byte)txt.Length); else PushUShort((ushort)txt.Length);
            }

            // encode text itself
            foreach (var chr in txt)
            {
                _buff[Offset++] = (byte)chr;
            }
        }

        /// <summary>
        /// Push int value to buffer.
        /// </summary>
        public void PushInt(int val)
        {
            _buff[Offset++] = (byte)(val & 0xff);
            _buff[Offset++] = (byte)((val >> 8) & 0xff);
            _buff[Offset++] = (byte)((val >> 16) & 0xff);
            _buff[Offset++] = (byte)((val >> 24) & 0xff);
        }

        /// <summary>
        /// Push uint value to buffer.
        /// </summary>
        public void PushUInt(uint val)
        {
            _buff[Offset++] = (byte)(val & 0xff);
            _buff[Offset++] = (byte)((val >> 8) & 0xff);
            _buff[Offset++] = (byte)((val >> 16) & 0xff);
            _buff[Offset++] = (byte)((val >> 24) & 0xff);
        }

        /// <summary>
        /// Push ulong value to buffer.
        /// </summary>
        public void PushULong(ulong val)
        {
            _buff[Offset++] = (byte)(val & 0xff);
            _buff[Offset++] = (byte)((val >> 8) & 0xff);
            _buff[Offset++] = (byte)((val >> 16) & 0xff);
            _buff[Offset++] = (byte)((val >> 24) & 0xff);
            _buff[Offset++] = (byte)((val >> 32) & 0xff);
            _buff[Offset++] = (byte)((val >> 40) & 0xff);
            _buff[Offset++] = (byte)((val >> 48) & 0xff);
            _buff[Offset++] = (byte)((val >> 56) & 0xff);
        }

        /// <summary>
        /// Push byte value to buffer.
        /// </summary>
        public void PushByte(byte val)
        {
            _buff[Offset++] = val;
        }

        /// <summary>
        /// Return space left in buffer.
        /// </summary>
        public int SpaceLeft
        {
            get { return _buff.Length - Offset; }
        }

        /// <summary>
        /// Set a single byte manually without affecting offset.
        /// </summary>
        public void SetByte(int offset, byte value)
        {
            _buff[offset] = value;
        }
    }
}
