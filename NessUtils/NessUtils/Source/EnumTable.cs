using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace Ness.Utils
{
    /// <summary>
    /// Implement dictionary-like API, but with an array with enum values.
    /// Used for when you want to use a dictionary with enum, but with better performance.
    /// Note: enum values must start with 0 and be sequential!
    /// </summary>
    public class EnumTable<KT, VT> : IXmlSerializable, ICloneable where KT : struct
    {
        /// <summary>
        /// Iteration pair type.
        /// </summary>
        public struct IterationPair
        {
            public KT Key;
            public VT Value;
        }

        // values and keys arrays
        VT[] _values;
        KT[] _keys;

        // if true, it means its a simple value type
        bool _isSimpleValue;

        /// <summary>
        /// Get table length.
        /// </summary>
        public int Length { get { return _values.Length; } }

        /// <summary>
        /// Get keys array.
        /// </summary>
        public KT[] Keys { get { return _keys; } }

        /// <summary>
        /// Get values array.
        /// </summary>
        public VT[] Values { get { return _values; } }

        /// <summary>
        /// Clone this table.
        /// </summary>
        public object Clone()
        {
            var ret = new EnumTable<KT, VT>();
            for (var i = 0; i < _values.Length; ++i)
            {
                if (_values[i] is ICloneable)
                {
                    ret._values[i] = (VT)((_values[i] as ICloneable).Clone());
                }
                ret._values[i] = _values[i];
            }
            return ret;
        }

        /// <summary>
        /// Create the enum table.
        /// </summary>
        public EnumTable()
        {
            _keys = Enum.GetValues(typeof(KT)).Cast<KT>().ToArray();
            _values = new VT[_keys.Length];
            _isSimpleValue =
                typeof(VT) == typeof(string) ||
                typeof(VT) == typeof(sbyte) ||
                typeof(VT) == typeof(byte) ||
                typeof(VT) == typeof(short) ||
                typeof(VT) == typeof(ushort) ||
                typeof(VT) == typeof(int) ||
                typeof(VT) == typeof(uint) ||
                typeof(VT) == typeof(long) ||
                typeof(VT) == typeof(ulong) ||
                typeof(VT) == typeof(float) ||
                typeof(VT) == typeof(double) ||
                typeof(VT) == typeof(decimal);
        }

        /// <summary>
        /// Implement [] operator.
        /// </summary>
        public VT this[KT key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        /// <summary>
        /// Implement [] operator.
        /// </summary>
        public VT this[int key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }

        /// <summary>
        /// Set value from int.
        /// </summary>
        public void SetValue(int key, VT value)
        {
            _values[key] = value;
        }

        /// <summary>
        /// Set value from enum.
        /// </summary>
        public void SetValue(KT key, VT value)
        {
            _values[Convert.ToInt32(key)] =  value;
        }

        /// <summary>
        /// Get value from int.
        /// </summary>
        public VT GetValue(int key)
        {
            return _values[key];
        }

        /// <summary>
        /// Get value from enum.
        /// </summary>
        public VT GetValue(KT key)
        {
            return _values[Convert.ToInt32(key)];
        }

        /// <summary>
        /// Get iterator.
        /// </summary>
        public IEnumerator<IterationPair> GetEnumerator()
        {
            for (var i = 0; i < _values.Length; ++i)
            {
                yield return new IterationPair() { Key = _keys[i], Value = _values[i] };
            }
        }

        #region IXmlSerializable Members

        /// <summary>
        /// Get schema - irrelevant for us.
        /// </summary>
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Serialize table.
        /// </summary>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer valueSerializer = new XmlSerializer(typeof(VT));

            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            var isString = typeof(VT) == typeof(string);
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                // get element content and break if done
                reader.MoveToContent();
                if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
                {
                    break;
                }

                // read key
                KT key = (KT)Enum.Parse(typeof(KT), reader.Name);

                // read value
                if (_isSimpleValue)
                {
                    reader.ReadStartElement();
                    var val = reader.ReadContentAsString();
                    reader.ReadEndElement();
                    if (isString)
                    {
                        SetValue(key, (VT)(object)val);
                    }
                    else
                    {
                        object casted = null;
                        if (typeof(VT) == typeof(byte))
                        {
                            casted = byte.Parse(val);
                        }
                        else if (typeof(VT) == typeof(short))
                        {
                            casted = short.Parse(val);
                        }
                        else if (typeof(VT) == typeof(int))
                        {
                            casted = int.Parse(val);
                        }
                        else if (typeof(VT) == typeof(long))
                        {
                            casted = long.Parse(val);
                        }
                        else if (typeof(VT) == typeof(float))
                        {
                            casted = float.Parse(val);
                        }
                        else if (typeof(VT) == typeof(double))
                        {
                            casted = double.Parse(val);
                        }
                        else if (typeof(VT) == typeof(sbyte))
                        {
                            casted = sbyte.Parse(val);
                        }
                        else if (typeof(VT) == typeof(ushort))
                        {
                            casted = ushort.Parse(val);
                        }
                        else if (typeof(VT) == typeof(uint))
                        {
                            casted = uint.Parse(val);
                        }
                        else if (typeof(VT) == typeof(ulong))
                        {
                            casted = ulong.Parse(val);
                        }
                        VT value = (VT)(casted);
                        SetValue(key, value);
                    }
                }
                else
                {
                    reader.ReadStartElement();
                    VT value = (VT)valueSerializer.Deserialize(reader);
                    SetValue(key, value);
                    reader.ReadEndElement();
                }
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// Deserialize table.
        /// </summary>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(KT));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(VT));

            for (var i = 0; i < Length; ++i)
            {
                writer.WriteStartElement(_keys[i].ToString());

                VT value = _values[i];
                if (_isSimpleValue)
                {
                    writer.WriteString(value.ToString());
                }
                else
                {
                    valueSerializer.Serialize(writer, value);
                }

                writer.WriteEndElement();
            }
        }
        #endregion
    }

    /// <summary>
    /// Bonus utility to get enum values.
    /// </summary>
    public static class EnumUtil
    {
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
