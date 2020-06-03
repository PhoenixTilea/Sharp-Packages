// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SE.Reflection;
using SE;

namespace SE.DataModel
{
    /// <summary>
    /// An expandable compound field to store any type without boxing
    /// </summary>
    public struct FlexField<T> : IDisposable
    {
        private readonly static Type type;
        private readonly static TypeCode typeCode;

        PrimitiveType storage;
        PrimitiveTypeInfo meta;

        /// <summary>
        /// This fields primary value. Returns the value at index 0 if
        /// the field is expanded
        /// </summary>
        public T Value
        {
            get { return GetValue(0); }
            set { SetValue(0, value); }
        }

        /// <summary>
        /// Determines if the field has been expanded
        /// </summary>
        public bool IsArray
        {
            get { return (meta.IsArrayReference || meta.Length > 0); }
        }
        /// <summary>
        /// Returns the length of an expanded field
        /// </summary>
        public int Length
        {
            get
            {
                if (meta.IsArrayReference) return ((T[])GCHandle.FromIntPtr(storage.Segment0_IntPtr).Target).Length;
                else return meta.Length;
            }
        }

        /// <summary>
        /// Accesses the value at given index. Index 0 always points to the
        /// primary value regardless if the field has been expanded
        /// </summary>
        public T this[int index]
        {
            get { return GetValue(index); }
            set { SetValue(index, value); }
        }

        static FlexField()
        {
            type = typeof(T);
            typeCode = Type.GetTypeCode(type);
        }
        public void Dispose()
        {
            Resize(0);
            storage.Segment0_IntPtr = IntPtr.Zero;
        }

        /// <summary>
        /// Accesses the value at given index. Index 0 always points to the
        /// primary value regardless if the field has been expanded
        /// </summary>
        public T GetValue(int index)
        {
            if (meta.IsArrayReference) return (T)((T[])GCHandle.FromIntPtr(storage.Segment0_IntPtr).Target).GetValue(index);
            else if(index == 0 || index < meta.Length)
            {
                switch (typeCode)
                {
                    case TypeCode.Byte: switch (index)
                        {
                            case -1:
                            case 0: return (T)Convert.ChangeType(storage.Segment0_Byte, typeCode);
                            case 1: return (T)Convert.ChangeType(storage.Segment1_Byte, typeCode);
                            case 2: return (T)Convert.ChangeType(storage.Segment2_Byte, typeCode);
                            case 3: return (T)Convert.ChangeType(storage.Segment3_Byte, typeCode);
                        }
                        break;
                    case TypeCode.Int16: switch (index)
                        {
                            case -1:
                            case 0: return (T)Convert.ChangeType(storage.Segment0_Int16, typeCode);
                            case 1: return (T)Convert.ChangeType(storage.Segment1_Int16, typeCode);
                        }
                        break;
                    case TypeCode.UInt16: switch (index)
                        {
                            case -1:
                            case 0: return (T)Convert.ChangeType(storage.Segment0_UInt16, typeCode);
                            case 1: return (T)Convert.ChangeType(storage.Segment1_UInt16, typeCode);
                        }
                        break;
                    case TypeCode.Int32: switch (index)
                        {
                            case -1:
                            case 0: return (T)Convert.ChangeType(storage.Segment0_Int32, typeCode);
                        }
                        break;
                    case TypeCode.UInt32: switch (index)
                        {
                            case -1:
                            case 0: return (T)Convert.ChangeType(storage.Segment0_UInt32, typeCode);
                        }
                        break;
                    default:
                        {
                            if (type.IsValueType) throw new InvalidCastException();
                            else switch (index)
                                {
                                    case -1:
                                    case 0: return storage.Segment0_IntPtr.ToInstance<T>();
                                }
                        }
                        break;
                }
             }
            throw new IndexOutOfRangeException();
        }
        /// <summary>
        /// Accesses the value at given index. Index 0 always points to the
        /// primary value regardless if the field has been expanded
        /// </summary>
        public void SetValue(int index, T value)
        {
            if (meta.IsArrayReference)
            {
                ((T[])GCHandle.FromIntPtr(storage.Segment0_IntPtr).Target).SetValue(value, index);
                return;
            }
            else if(index == 0 || index < meta.Length)
            {
                switch (typeCode)
                {
                    case TypeCode.Byte: switch (index)
                        {
                            case -1:
                            case 0: storage.Segment0_Byte = (byte)Convert.ChangeType(value, typeCode); return;
                            case 1: storage.Segment1_Byte = (byte)Convert.ChangeType(value, typeCode); return;
                            case 2: storage.Segment2_Byte = (byte)Convert.ChangeType(value, typeCode); return;
                            case 3: storage.Segment3_Byte = (byte)Convert.ChangeType(value, typeCode); return;
                        }
                        break;
                    case TypeCode.Int16: switch (index)
                        {
                            case -1:
                            case 0: storage.Segment0_Int16 = (Int16)Convert.ChangeType(value, typeCode); return;
                            case 1: storage.Segment1_Int16 = (Int16)Convert.ChangeType(value, typeCode); return;
                        }
                        break;
                    case TypeCode.UInt16: switch (index)
                        {
                            case -1:
                            case 0: storage.Segment0_UInt16 = (UInt16)Convert.ChangeType(value, typeCode); return;
                            case 1: storage.Segment1_UInt16 = (UInt16)Convert.ChangeType(value, typeCode); return;
                        }
                        break;
                    case TypeCode.Int32: switch (index)
                        {
                            case -1:
                            case 0: storage.Segment0_Int32 = (Int32)Convert.ChangeType(value, typeCode); return;
                        }
                        break;
                    case TypeCode.UInt32: switch (index)
                        {
                            case -1:
                            case 0: storage.Segment0_UInt32 = (UInt32)Convert.ChangeType(value, typeCode); return;
                        }
                        break;
                    default:
                        {
                            if (type.IsValueType) throw new InvalidCastException();
                            else switch (index)
                                {
                                    case -1:
                                    case 0: storage.Segment0_IntPtr = value.ToPointer(); return;
                                }
                        }
                        break;
                }
            }
            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Returns a value to indicate if the index is in local enclosing memory or
        /// located on the heap
        /// </summary>
        /// <returns>True if the index points to local memory, false otherwise</returns>
        public bool IsIndexEmplaced(int index)
        {
            switch (typeCode)
            {
                case TypeCode.Byte: switch (index)
                    {
                        case -1:
                        case 0:
                        case 1:
                        case 2:
                        case 3: return true;
                    }
                    break;
                case TypeCode.Int16:
                case TypeCode.UInt16: switch (index)
                    {
                        case -1:
                        case 0:
                        case 1: return true;
                    }
                    break;
                case TypeCode.Int32:
                case TypeCode.UInt32:
                default: switch (index)
                    {
                        case -1:
                        case 0: return true;
                    }
                    break;
            }
            return false;
        }

        /// <summary>
        /// Changes this fields level of expansion
        /// </summary>
        public void Resize(int length)
        {
            if (Length < length) Grow(length);
            else Shrink(length);
        }
        void ResizeBuffer(int length)
        {
            GCHandle handle = GCHandle.FromIntPtr(storage.Segment0_IntPtr);
            T[] buffer = ((T[])handle.Target);

            Array.Resize(ref buffer, length);

            handle.Free();
            storage.Segment0_IntPtr = (IntPtr)GCHandle.Alloc(buffer, GCHandleType.Normal);
        }

        void Grow(int length)
        {
            if (meta.IsArrayReference) ResizeBuffer(length);
            else switch (length)
                {
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        switch (typeCode)
                        {
                            case TypeCode.Byte: meta.Length = (Int16)length; return;
                            case TypeCode.Int16:
                            case TypeCode.UInt16: switch (length)
                                {
                                    case 1:
                                    case 2: meta.Length = (Int16)length; return;
                                }
                                break;
                            case TypeCode.Int32:
                            case TypeCode.UInt32: switch (length)
                                {
                                    case 1: meta.Length = (Int16)length; return;
                                }
                                break;
                            default:
                                {
                                    if (type.IsValueType) throw new InvalidCastException();
                                    else switch (length)
                                        {
                                            case 1: meta.Length = (Int16)length; return;
                                        }
                                }
                                break;
                        }
                        goto default;

                    default:
                        {
                            T[] buffer = new T[length];
                            switch (typeCode)
                            {
                                case TypeCode.Byte:
                                    {
                                        buffer[0] = (T)Convert.ChangeType(storage.Segment0_Byte, typeCode);
                                        buffer[1] = (T)Convert.ChangeType(storage.Segment1_Byte, typeCode);
                                        buffer[2] = (T)Convert.ChangeType(storage.Segment2_Byte, typeCode);
                                        buffer[3] = (T)Convert.ChangeType(storage.Segment3_Byte, typeCode);
                                    }
                                    break;
                                case TypeCode.Int16:
                                    {
                                        buffer[0] = (T)Convert.ChangeType(storage.Segment0_Int16, typeCode);
                                        buffer[1] = (T)Convert.ChangeType(storage.Segment1_Int16, typeCode);
                                    }
                                    break;
                                case TypeCode.UInt16:
                                    {
                                        buffer[0] = (T)Convert.ChangeType(storage.Segment0_UInt16, typeCode);
                                        buffer[1] = (T)Convert.ChangeType(storage.Segment1_UInt16, typeCode);
                                    }
                                    break;
                                case TypeCode.Int32:
                                    {
                                        buffer[0] = (T)Convert.ChangeType(storage.Segment0_Int32, typeCode);
                                    }
                                    break;
                                case TypeCode.UInt32:
                                    {
                                        buffer[0] = (T)Convert.ChangeType(storage.Segment0_UInt32, typeCode);
                                    }
                                    break;
                                default:
                                    {
                                        if (type.IsValueType) throw new InvalidCastException();
                                        else buffer[0] = storage.Segment0_IntPtr.ToInstance<T>();
                                    }
                                    break;
                            }
                            storage.Segment0_IntPtr = (IntPtr)GCHandle.Alloc(buffer, GCHandleType.Normal);
                            meta.IsArrayReference = true;
                            meta.Length = 0;
                        }
                        break;
                }
        }
        void Shrink(int length)
        {
            if (!meta.IsArrayReference)
            {
                if (length < 0) length = 0;
                meta.Length = (Int16)length;
            }
            else
            {
                GCHandle handle = GCHandle.FromIntPtr(storage.Segment0_IntPtr);
                T[] buffer = ((T[])handle.Target);

                switch (length)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        switch (typeCode)
                        {
                            case TypeCode.Byte: switch (length)
                                {
                                    case 0:
                                    case 1:
                                        {
                                            storage.Segment0_Byte = (byte)Convert.ChangeType(buffer[0], typeCode);
                                            meta.IsArrayReference = false;
                                            meta.Length = (Int16)length;
                                            handle.Free();
                                        }
                                        return;
                                    case 2: storage.Segment1_Byte = (byte)Convert.ChangeType(buffer[1], typeCode); goto case 1;
                                    case 3: storage.Segment2_Byte = (byte)Convert.ChangeType(buffer[2], typeCode); goto case 2;
                                    case 4: storage.Segment3_Byte = (byte)Convert.ChangeType(buffer[3], typeCode); goto case 3;
                                }
                                break;
                            case TypeCode.Int16: switch (length)
                                {
                                    case 0:
                                    case 1:
                                        {
                                            storage.Segment0_Int16 = (Int16)Convert.ChangeType(buffer[0], typeCode);
                                            meta.IsArrayReference = false;
                                            meta.Length = (Int16)length;
                                            handle.Free();
                                        }
                                        return;
                                    case 2: storage.Segment1_Int16 = (Int16)Convert.ChangeType(buffer[1], typeCode); goto case 1;
                                }
                                break;
                            case TypeCode.UInt16: switch (length)
                                {
                                    case 0:
                                    case 1:
                                        {
                                            storage.Segment0_UInt16 = (UInt16)Convert.ChangeType(buffer[0], typeCode);
                                            meta.IsArrayReference = false;
                                            meta.Length = (Int16)length;
                                            handle.Free();
                                        }
                                        return;
                                    case 2: storage.Segment1_UInt16 = (UInt16)Convert.ChangeType(buffer[1], typeCode); goto case 1;
                                }
                                break;
                            case TypeCode.Int32: switch (length)
                                {
                                    case 0:
                                    case 1:
                                        {
                                            storage.Segment0_Int32 = (Int32)Convert.ChangeType(buffer[0], typeCode);
                                            meta.IsArrayReference = false;
                                            meta.Length = (Int16)length;
                                            handle.Free();
                                        }
                                        return;
                                }
                                break;
                            case TypeCode.UInt32: switch (length)
                                {
                                    case 0:
                                    case 1:
                                        {
                                            storage.Segment0_UInt32 = (UInt32)Convert.ChangeType(buffer[0], typeCode);
                                            meta.IsArrayReference = false;
                                            meta.Length = (Int16)length;
                                            handle.Free();
                                        }
                                        return;
                                }
                                break;
                            default:
                                {
                                    if (type.IsValueType) throw new InvalidCastException();
                                    else switch (length)
                                        {
                                            case 0:
                                            case 1:
                                                {
                                                    storage.Segment0_IntPtr = buffer[0].ToPointer();
                                                    meta.IsArrayReference = false;
                                                    meta.Length = (Int16)length;
                                                    handle.Free();
                                                }
                                                return;
                                        }
                                    break;
                                }
                        }
                        goto default;

                    default: ResizeBuffer(length); break;
                }
            }
        }

        /// <summary>
        /// Assigns a number of values to this field. The field gets expanded to fit the amount
        /// of values passed to copy
        /// </summary>
        public void CopyFrom(T[] array, int startIndex)
        {
            Resize(array.Length - startIndex);
            if (meta.IsArrayReference)
                Array.Copy(array, startIndex, GCHandle.FromIntPtr(storage.Segment0_IntPtr).Target as Array, 0, Length);
            else for (int i = startIndex; i < array.Length; i++)
                SetValue(i - startIndex, array[i]);
        }
        /// <summary>
        /// Assigns an amount of values from this field to the provided array
        /// </summary>
        public void CopyTo(T[] array, int startIndex)
        {
            if (meta.IsArrayReference)
                Array.Copy(GCHandle.FromIntPtr(storage.Segment0_IntPtr).Target as Array, 0, array, startIndex, Length);
            else
            {
                int length = Length;
                for (int i = 0; i < length; i++)
                    array[i + startIndex] = GetValue(i);
            }
        }
    }
}
