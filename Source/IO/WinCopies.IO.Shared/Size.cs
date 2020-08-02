/* Copyright © Pierre Sprimont, 2020
 *
 * This file is part of the WinCopies Framework.
 *
 * The WinCopies Framework is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * The WinCopies Framework is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with the WinCopies Framework.  If not, see <https://www.gnu.org/licenses/>. */

using System;
using System.Diagnostics.CodeAnalysis;
using WinCopies.Util;
using static WinCopies.Util.Util;

namespace WinCopies.IO
{
    /// <summary>
    /// Defines the unit of a size.
    /// </summary>
    public enum ByteUnit : ushort
    {
        /// <summary>
        /// A size given in bytes.
        /// </summary>
        Byte = 0,

        /// <summary>
        /// A size given in kibibytes.
        /// </summary>
        KibiByte = 1,

        /// <summary>
        /// A size given in mebibytes.
        /// </summary>
        MebiByte = 2,

        /// <summary>
        /// A size given in gibibytes.
        /// </summary>
        GibiByte = 3,

        /// <summary>
        /// A size given in tebibytes.
        /// </summary>
        TebiByte = 4,

        /// <summary>
        /// A size given in pebibytes.
        /// </summary>
        PebiByte = 5,

        /// <summary>
        /// A size given in exbibytes.
        /// </summary>
        ExbiByte = 6,

        /// <summary>
        /// A size given in zebibytes.
        /// </summary>
        ZebiByte = 7,

        /// <summary>
        /// A size given in yobibytes.
        /// </summary>
        YobiByte = 8
    }

    /// <summary>
    /// Represent a file size in byte-based unit.
    /// </summary>
    public struct Size : IComparable<Size>, IComparable<byte>, IComparable<ushort>, IComparable<uint>, IComparable<ulong>, IComparable<sbyte>, IComparable<short>, IComparable<int>, IComparable<long>
    {
        /// <summary>
        /// The numeric value in bytes.
        /// </summary>
        public WinCopies.Util.CheckedUInt64 ValueInBytes { get; }

        public float GetFloatValueInUnit(in ByteUnit unit) => unit == ByteUnit.Byte
                ? (float)ValueInBytes
                : (float)ValueInBytes / Util.Math.Pow(1024f, (float)unit);

        public double GetDoubleValueInUnit(in ByteUnit unit) => unit == ByteUnit.Byte
                ? (double)ValueInBytes
                : (double)ValueInBytes / System.Math.Pow(1024d, (double)unit);

        public decimal GetDecimalValueInUnit(in ByteUnit unit) => unit == ByteUnit.Byte
                ? (decimal)ValueInBytes
                : (decimal)ValueInBytes / Util.Math.Pow(1024m, (decimal)unit);

        public float GetFloatValueInUnit() => GetFloatValueInUnit(Unit);

        public double GetDoubleValueInUnit() => GetDoubleValueInUnit(Unit);

        public decimal GetDecimalValueInUnit() => GetDecimalValueInUnit(Unit);

        //public static ulong GetValueInUnit(ulong valueInBytes, ByteUnit unit) => valueInBytes / Math.Pow(1024, (ulong)unit);

        private ByteUnit? _unit;

        /// <summary>
        /// The unit of this <see cref="Size"/>.
        /// </summary>
        public ByteUnit Unit
        {
            get
            {
                if (_unit is null)
                {
                    ushort unit;

                    ushort newUnit = 0;

                    //float value;

                    var newValue = (float)ValueInBytes;

                    do
                    {
                        unit = newUnit;

                        newUnit++;

                        //value = newValue;

                        newValue /= 1024;

                    } while (newValue > 1 && Enum.IsDefined(typeof(ByteUnit), newUnit));

                    _unit = (ByteUnit)unit;
                }

                return _unit.Value;
            }
        }

        // public Speed() { Size = 0; Unit = Unit.Byte; }

        public Size(in ulong valueInBytes)
        {
            ValueInBytes = new CheckedUInt64(valueInBytes);

            _unit = null;
        }

        public Size(in CheckedUInt64 valueInBytes)
        {
            ValueInBytes = valueInBytes;

            _unit = null;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size size) return size == this;

            if (IsNumber(obj))
            {
                if (obj is sbyte sb) return sb == this;

                if (obj is byte b) return b == this;

                if (obj is short s) return s == this;

                if (obj is ushort us) return us == this;

                if (obj is int i) return i == this;

                if (obj is uint ui) return ui == this;

                if (obj is long l) return l == this;

                if (obj is ulong ul) return ul == this;

                if (obj is float f) return f == this;

                if (obj is decimal d) return d == this;

                if (obj is double _d) return _d == this;
            }

            return false;
        }

        public override int GetHashCode() => ValueInBytes.GetHashCode();

        public static string GetDisplaySizeUnit(in ByteUnit unit)
#if NETFRAMEWORK
        {
            switch (unit)
            {
                case ByteUnit.Byte:

                    return "B";

                case ByteUnit.KibiByte:

                    return "KiB";

                case ByteUnit.MebiByte:

                    return "MiB";

                case ByteUnit.GibiByte:

                    return "GiB";

                case ByteUnit.TebiByte:

                    return "TiB";

                case ByteUnit.PebiByte:

                    return "PiB";

                case ByteUnit.ExbiByte:

                    return "EiB";

                case ByteUnit.ZebiByte:

                    return "ZiB";

                case ByteUnit.YobiByte:

                    return "YiB";

                default:

                    throw new ArgumentOutOfRangeException(nameof(unit), unit, $"{nameof(unit)} must be a value defined in the {nameof(ByteUnit)} enumeration.");
            }
        }
            
#else

            => unit switch
            {
                ByteUnit.Byte => "B",
                ByteUnit.KibiByte => "KiB",
                ByteUnit.MebiByte => "MiB",
                ByteUnit.GibiByte => "GiB",
                ByteUnit.TebiByte => "TiB",
                ByteUnit.PebiByte => "PiB",
                ByteUnit.ExbiByte => "EiB",
                ByteUnit.ZebiByte => "ZiB",
                ByteUnit.YobiByte => "YiB",
                _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, $"{nameof(unit)} must be a value defined in the {nameof(ByteUnit)} enumeration."),
            };

#endif

        /// <summary>
        /// Returns a string with the size value and unit.
        /// </summary>
        /// <returns>A string with the size value and unit</returns>
        public override string ToString() => $"{GetFloatValueInUnit(Unit)} {GetDisplaySizeUnit(Unit)}";

        public int CompareTo(
#if NETCORE
            [AllowNull]
#endif
        Size other) =>
#if NETCORE
            other == null ? ValueInBytes.IsNaN ? 0 : 1 :
#endif
            ValueInBytes.CompareTo(other.ValueInBytes);

        public int CompareTo(
#if NETCORE
            [AllowNull]
        #endif
        long other) => ValueInBytes.CompareTo(other);

        public int CompareTo(
#if NETCORE
            [AllowNull]
        #endif
        int other) => ValueInBytes.CompareTo(other);

        public int CompareTo(
#if NETCORE
            [AllowNull]
        #endif
        short other) => ValueInBytes.CompareTo(other);

        public int CompareTo(
#if NETCORE
            [AllowNull]
        #endif
        sbyte other) => ValueInBytes.CompareTo(other);

        public int CompareTo(
#if NETCORE
            [AllowNull]
        #endif
        ulong other) => ValueInBytes.CompareTo(other);

        public int CompareTo(
#if NETCORE
            [AllowNull]
        #endif
        uint other) => ValueInBytes.CompareTo(other);

        public int CompareTo(
#if NETCORE
            [AllowNull]
        #endif
        ushort other) => ValueInBytes.CompareTo(other);

        public int CompareTo(
#if NETCORE
            [AllowNull]
        #endif
        byte other) => ValueInBytes.CompareTo(other);

        #region Size operators

        #region Equality operators 

        #region Size operators

        /// <summary>
        /// Checks if <paramref name="s1"/> is less than <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if <paramref name="s1"/> is less than <paramref name="s2"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator <(in Size s1, in Size s2) => s1.ValueInBytes < s2.ValueInBytes;

        /// <summary>
        /// Checks if <paramref name="s1"/> is greater than <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if <paramref name="s1"/> is greater than <paramref name="s2"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator >(in Size s1, in Size s2) => s1.ValueInBytes > s2.ValueInBytes;

        /// <summary>
        /// Checks if <paramref name="s1"/> is less than or equal to <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if <paramref name="s1"/> is less than or equal to <paramref name="s2"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator <=(in Size s1, in Size s2) => !(s1 > s2);

        /// <summary>
        /// Checks if <paramref name="s1"/> is greater than or equal to <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if <paramref name="s1"/> is greater than or equal to <paramref name="s2"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator >=(in Size s1, in Size s2) => !(s1 < s2);

        /// <summary>
        /// Checks if <paramref name="s1"/> equals <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if <paramref name="s1"/> is equal to <paramref name="s2"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator ==(in Size s1, in Size s2) => s1.ValueInBytes == s2.ValueInBytes;

        /// <summary>
        /// Checks if <paramref name="s1"/> doesn't equal s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if <paramref name="s1"/> is not equal to <paramref name="s2"/>, otherwise <see langword="false"/>.</returns>
        public static bool operator !=(in Size s1, in Size s2) => !(s1 == s2);

        #endregion



        #region Size, numeric operators

        #region sbyte operators

        public static bool operator <(in Size s, in sbyte b) => b <= 0 ? false : s.ValueInBytes < (ulong)b;

        public static bool operator >(in Size s, in sbyte b) => b < 0 || (b == 0 && s.ValueInBytes != 0) ? true : s.ValueInBytes > (ulong)b;

        public static bool operator <=(in Size s, in sbyte b) => !(s > b);

        public static bool operator >=(in Size s, in sbyte b) => !(s < b);

        public static bool operator ==(in Size s, in sbyte b) => b < 0 || (b == 0 && s.ValueInBytes != 0) ? false : s.ValueInBytes == (ulong)b;

        public static bool operator !=(in Size s, in sbyte b) => !(s == b);

        #endregion



        #region byte operators

        public static bool operator <(in Size s, in byte b) => s.ValueInBytes < (ulong)b;

        public static bool operator >(in Size s, in byte b) => s.ValueInBytes > (ulong)b;

        public static bool operator <=(in Size s, in byte b) => s.ValueInBytes <= (ulong)b;

        public static bool operator >=(in Size s, in byte b) => s.ValueInBytes >= (ulong)b;

        public static bool operator ==(in Size s, in byte b) => s.ValueInBytes == (ulong)b;

        public static bool operator !=(in Size s, in byte b) => s.ValueInBytes != (ulong)b;

        #endregion



        #region short operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less than <paramref name="short"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="short"/> must be in byte unit.</remarks>
        public static bool operator <(in Size s, in short @short) => @short <= 0 ? false : s.ValueInBytes < (ulong)@short;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater than <paramref name="short"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="short"/> must be in byte unit.</remarks>
        public static bool operator >(in Size s, in short @short) => @short < 0 || (@short == 0 && s.ValueInBytes != 0) ? true : s.ValueInBytes > (ulong)@short;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less or equals <paramref name="short"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="short"/> must be in byte unit.</remarks>
        public static bool operator <=(in Size s, in short @short) => !(s > @short);

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater or equals <paramref name="short"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="short"/> must be in byte unit.</remarks>
        public static bool operator >=(in Size s, in short @short) => !(s < @short);

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> equals <paramref name="short"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="short"/> must be in byte unit.</remarks>
        public static bool operator ==(in Size s, in short @short) => @short < 0 || (@short == 0 && s.ValueInBytes != 0) ? false : s.ValueInBytes == (ulong)@short;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal <paramref name="short"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="short"/> must be in byte unit.</remarks>
        public static bool operator !=(in Size s, in short @short) => !(s == @short);

        #endregion



        #region ushort operators

        public static bool operator <(in Size s, in ushort @short) => s.ValueInBytes < (ulong)@short;

        public static bool operator >(in Size s, in ushort @short) => s.ValueInBytes > (ulong)@short;

        public static bool operator <=(in Size s, in ushort @short) => s.ValueInBytes <= (ulong)@short;

        public static bool operator >=(in Size s, in ushort @short) => s.ValueInBytes >= (ulong)@short;

        public static bool operator ==(in Size s, in ushort @short) => s.ValueInBytes == (ulong)@short;

        public static bool operator !=(in Size s, in ushort @short) => s.ValueInBytes != (ulong)@short;

        #endregion



        #region int operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less than <paramref name="i"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="i"/> must be in byte unit.</remarks>
        public static bool operator <(in Size s, in int i) => i <= 0 ? false : s.ValueInBytes < (ulong)i;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater than <paramref name="i"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="i"/> must be in byte unit.</remarks>
        public static bool operator >(in Size s, in int i) => i < 0 || (i == 0 && s.ValueInBytes != 0) ? true : s.ValueInBytes > (ulong)i;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less or equals i, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="i"/> must be in byte unit.</remarks>
        public static bool operator <=(in Size s, in int i) => !(s > i);

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater or equals i, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="i"/> must be in byte unit.</remarks>
        public static bool operator >=(in Size s, in int i) => !(s < i);

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> equals i, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="i"/> must be in byte unit.</remarks>
        public static bool operator ==(in Size s, in int i) => i < 0 || (i == 0 && s.ValueInBytes != 0) ? false : s.ValueInBytes == (ulong)i;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> doesn't equal <paramref name="i"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="i"/> must be in byte unit.</remarks>
        public static bool operator !=(in Size s, in int i) => !(s == i);

        #endregion



        #region uint operators

        public static bool operator <(in Size s, in uint i) => s.ValueInBytes < (ulong)i;

        public static bool operator >(in Size s, in uint i) => s.ValueInBytes > (ulong)i;

        public static bool operator <=(in Size s, in uint i) => s.ValueInBytes <= (ulong)i;

        public static bool operator >=(in Size s, in uint i) => s.ValueInBytes >= (ulong)i;

        public static bool operator ==(in Size s, in uint i) => s.ValueInBytes == (ulong)i;

        public static bool operator !=(in Size s, in uint i) => s.ValueInBytes != (ulong)i;

        #endregion



        #region long operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less than <paramref name="l"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="l"/> must be in byte unit.</remarks>
        public static bool operator <(in Size s, in long l) => l <= 0 ? false : s.ValueInBytes < (ulong)l;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater than <paramref name="l"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="l"/> must be in byte unit.</remarks>
        public static bool operator >(in Size s, in long l) => l < 0 || (l == 0 && s.ValueInBytes != 0) ? true : s.ValueInBytes > (ulong)l;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less or equals <paramref name="l"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="l"/> must be in byte unit.</remarks>
        public static bool operator <=(in Size s, in long l) => !(s > l);

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater or equals <paramref name="l"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="l"/> must be in byte unit.</remarks>
        public static bool operator >=(in Size s, in long l) => !(s < l);

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> equals <paramref name="l"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="l"/> must be in byte unit.</remarks>
        public static bool operator ==(in Size s, in long l) => l < 0 || (l == 0 && s.ValueInBytes != 0) ? false : s.ValueInBytes == (ulong)l;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal <paramref name="l"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="l"/> must be in byte unit.</remarks>
        public static bool operator !=(in Size s, in long l) => !(s == l);

        #endregion



        #region ulong operators

        public static bool operator <(in Size s, in ulong l) => s.ValueInBytes < l;

        public static bool operator >(in Size s, in ulong l) => s.ValueInBytes > l;

        public static bool operator <=(in Size s, in ulong l) => s.ValueInBytes <= l;

        public static bool operator >=(in Size s, in ulong l) => s.ValueInBytes >= l;

        public static bool operator ==(in Size s, in ulong l) => s.ValueInBytes == l;

        public static bool operator !=(in Size s, in ulong l) => s.ValueInBytes != l;

        #endregion



        #region float operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less than <paramref name="f"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="f"/> must be in byte unit.</remarks>
        public static bool operator <(in Size s, in float f) => (float)s.ValueInBytes < f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater than <paramref name="f"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="f"/> must be in byte unit.</remarks>
        public static bool operator >(in Size s, in float f) => (float)s.ValueInBytes > f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less or equals <paramref name="f"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="f"/> must be in byte unit.</remarks>
        public static bool operator <=(in Size s, in float f) => (float)s.ValueInBytes <= f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater or equals <paramref name="f"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="f"/> must be in byte unit.</remarks>
        public static bool operator >=(in Size s, in float f) => (float)s.ValueInBytes >= f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> equals <paramref name="f"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="f"/> must be in byte unit.</remarks>
        public static bool operator ==(in Size s, in float f) => (float)s.ValueInBytes == f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal <paramref name="f"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="f"/> must be in byte unit.</remarks>
        public static bool operator !=(in Size s, in float f) => (float)s.ValueInBytes != f;

        #endregion



        #region double operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less than <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator <(in Size s, in double d) => (double)s.ValueInBytes < d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater than <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator >(in Size s, in double d) => (double)s.ValueInBytes > d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less or equals <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator <=(in Size s, in double d) => (double)s.ValueInBytes <= d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater or equals <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator >=(in Size s, in double d) => (double)s.ValueInBytes >= d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> equals <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator ==(in Size s, in double d) => (double)s.ValueInBytes == d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator !=(in Size s, in double d) => (double)s.ValueInBytes != d;

        #endregion



        #region decimal operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less than <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator <(in Size s, in decimal d) => (decimal)s.ValueInBytes < d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater than <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator >(in Size s, in decimal d) => (decimal)s.ValueInBytes > d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is less or equals <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator <=(in Size s, in decimal d) => (decimal)s.ValueInBytes <= d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> is greater or equals <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator >=(in Size s, in decimal d) => (decimal)s.ValueInBytes >= d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if <paramref name="s"/> equals <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator ==(in Size s, in decimal d) => (decimal)s.ValueInBytes == d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal <paramref name="d"/>, otherwise <see langword="false"/>.</returns>
        /// <remarks><paramref name="d"/> must be in byte unit.</remarks>
        public static bool operator !=(in Size s, in decimal d) => (decimal)s.ValueInBytes != d;

        #endregion

        #endregion



        #region Numeric, Size operators

        #region sbyte operators

        public static bool operator <(in sbyte b, in Size s) => b < 0 || (b == 0 && s.ValueInBytes != 0) ? true : (ulong)b < s.ValueInBytes;

        public static bool operator >(in sbyte b, in Size s) => b <= 0 ? false : (ulong)b > s.ValueInBytes;

        public static bool operator <=(in sbyte b, in Size s) => !(b > s);

        public static bool operator >=(in sbyte b, in Size s) => !(b < s);

        public static bool operator ==(in sbyte b, in Size s) => b < 0 || (b == 0 && s.ValueInBytes != 0) ? false : (ulong)b == s.ValueInBytes;

        public static bool operator !=(in sbyte b, in Size s) => !(b == s);

        #endregion



        #region byte operators

        public static bool operator <(in byte b, in Size s) => (ulong)b < s.ValueInBytes;

        public static bool operator >(in byte b, in Size s) => (ulong)b > s.ValueInBytes;

        public static bool operator <=(in byte b, in Size s) => (ulong)b <= s.ValueInBytes;

        public static bool operator >=(in byte b, in Size s) => (ulong)b >= s.ValueInBytes;

        public static bool operator ==(in byte b, in Size s) => (ulong)b == s.ValueInBytes;

        public static bool operator !=(in byte b, in Size s) => (ulong)b != s.ValueInBytes;

        #endregion



        #region short operators

        public static bool operator <(in short @short, in Size s) => @short < 0 || (@short == 0 && s.ValueInBytes != 0) ? true : (ulong)@short < s.ValueInBytes;

        public static bool operator >(in short @short, in Size s) => @short <= 0 ? false : (ulong)@short > s.ValueInBytes;

        public static bool operator <=(in short @short, in Size s) => !(@short > s);

        public static bool operator >=(in short @short, in Size s) => !(@short < s);

        public static bool operator ==(in short @short, in Size s) => @short < 0 || (@short == 0 && s.ValueInBytes != 0) ? false : (ulong)@short == s.ValueInBytes;

        public static bool operator !=(in short @short, in Size s) => !(@short == s);

        #endregion



        #region ushort operators

        public static bool operator <(in ushort @short, in Size s) => (ulong)@short < s.ValueInBytes;

        public static bool operator >(in ushort @short, in Size s) => (ulong)@short > s.ValueInBytes;

        public static bool operator <=(in ushort @short, in Size s) => (ulong)@short <= s.ValueInBytes;

        public static bool operator >=(in ushort @short, in Size s) => (ulong)@short >= s.ValueInBytes;

        public static bool operator ==(in ushort @short, in Size s) => (ulong)@short == s.ValueInBytes;

        public static bool operator !=(in ushort @short, in Size s) => (ulong)@short != s.ValueInBytes;

        #endregion



        #region int operators

        public static bool operator <(in int i, in Size s) => i < 0 || (i == 0 && s.ValueInBytes != 0) ? true : (ulong)i < s.ValueInBytes;

        public static bool operator >(in int i, in Size s) => i <= 0 ? false : (ulong)i > s.ValueInBytes;

        public static bool operator <=(in int i, in Size s) => !(i > s);

        public static bool operator >=(in int i, in Size s) => !(i < s);

        public static bool operator ==(in int i, in Size s) => i < 0 || (i == 0 && s.ValueInBytes != 0) ? false : (ulong)i == s.ValueInBytes;

        public static bool operator !=(in int i, in Size s) => !(i == s);

        #endregion



        #region uint operators

        public static bool operator <(in uint i, in Size s) => (ulong)i < s.ValueInBytes;

        public static bool operator >(in uint i, in Size s) => (ulong)i > s.ValueInBytes;

        public static bool operator <=(in uint i, in Size s) => (ulong)i <= s.ValueInBytes;

        public static bool operator >=(in uint i, in Size s) => (ulong)i >= s.ValueInBytes;

        public static bool operator ==(in uint i, in Size s) => (ulong)i == s.ValueInBytes;

        public static bool operator !=(in uint i, in Size s) => (ulong)i != s.ValueInBytes;

        #endregion



        #region long operators

        public static bool operator <(in long l, in Size s) => l < 0 || (l == 0 && s.ValueInBytes != 0) ? true : (ulong)l < s.ValueInBytes;

        public static bool operator >(in long l, in Size s) => l <= 0 ? false : (ulong)l > s.ValueInBytes;

        public static bool operator <=(in long l, in Size s) => !(l > s);

        public static bool operator >=(in long l, in Size s) => !(l < s);

        public static bool operator ==(in long l, in Size s) => l < 0 || (l == 0 && s.ValueInBytes != 0) ? false : (ulong)l == s.ValueInBytes;

        public static bool operator !=(in long l, in Size s) => !(l == s);

        #endregion



        #region ulong operators

        public static bool operator <(in ulong l, in Size s) => l < s.ValueInBytes;

        public static bool operator >(in ulong l, in Size s) => l > s.ValueInBytes;

        public static bool operator <=(in ulong l, in Size s) => l <= s.ValueInBytes;

        public static bool operator >=(in ulong l, in Size s) => l >= s.ValueInBytes;

        public static bool operator ==(in ulong l, in Size s) => l == s.ValueInBytes;

        public static bool operator !=(in ulong l, in Size s) => l != s.ValueInBytes;

        #endregion



        #region float operators

        public static bool operator <(in float f, in Size s) => f < (float)s.ValueInBytes;

        public static bool operator >(in float f, in Size s) => f > (float)s.ValueInBytes;

        public static bool operator <=(in float f, in Size s) => f <= (float)s.ValueInBytes;

        public static bool operator >=(in float f, in Size s) => f >= (float)s.ValueInBytes;

        public static bool operator ==(in float f, in Size s) => f == (float)s.ValueInBytes;

        public static bool operator !=(in float f, in Size s) => f != (float)s.ValueInBytes;

        #endregion



        #region double operators

        public static bool operator <(in double d, in Size s) => d < (double)s.ValueInBytes;

        public static bool operator >(in double d, in Size s) => d > (double)s.ValueInBytes;

        public static bool operator <=(in double d, in Size s) => d <= (double)s.ValueInBytes;

        public static bool operator >=(in double d, in Size s) => d >= (double)s.ValueInBytes;

        public static bool operator ==(in double d, in Size s) => d == (double)s.ValueInBytes;

        public static bool operator !=(in double d, in Size s) => d != (double)s.ValueInBytes;

        #endregion



        #region decimal operators

        public static bool operator <(in decimal d, in Size s) => d < (decimal)s.ValueInBytes;

        public static bool operator >(in decimal d, in Size s) => d > (decimal)s.ValueInBytes;

        public static bool operator <=(in decimal d, in Size s) => d <= (decimal)s.ValueInBytes;

        public static bool operator >=(in decimal d, in Size s) => d >= (decimal)s.ValueInBytes;

        public static bool operator ==(in decimal d, in Size s) => d == (decimal)s.ValueInBytes;

        public static bool operator !=(in decimal d, in Size s) => d != (decimal)s.ValueInBytes;

        #endregion

        #endregion

        #endregion



        #region Arithmetic operators

        // todo: modulos?

        #region Size operators

        /// <summary>
        /// Returns a <see cref="Size"/> with the addition of <paramref name="s1"/> and <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the addition of <paramref name="s1"/> and <paramref name="s2"/>.</returns>
        public static Size operator +(in Size s1, in Size s2) => new Size(s1.ValueInBytes + s2.ValueInBytes);

        /// <summary>
        /// Returns a <see cref="Size"/> with the substraction of <paramref name="s2"/> by <paramref name="s1"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the substraction of <paramref name="s2"/> by <paramref name="s1"/>.</returns>
        public static Size operator -(in Size s1, in Size s2) => new Size(s1.ValueInBytes.Value - s2.ValueInBytes.Value);

        /// <summary>
        /// Returns a <see cref="Size"/> with the multiplication of <paramref name="s1"/> by <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the multiplication of <paramref name="s1"/> by <paramref name="s2"/></returns>
        public static Size operator *(in Size s1, in Size s2) => new Size(s1.ValueInBytes * s2.ValueInBytes);

        /// <summary>
        /// Returns a <see cref="Size"/> with the division of <paramref name="s1"/> by <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the division of <paramref name="s1"/> by <paramref name="s2"/></returns>
        public static Size operator /(in Size s1, in Size s2) => new Size(s1.ValueInBytes.Value / s2.ValueInBytes.Value);

        /// <summary>
        /// Returns a <see cref="Size"/> with the remainder of <paramref name="s1"/> by <paramref name="s2"/>.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the remainder of <paramref name="s1"/> by <paramref name="s2"/></returns>
        public static Size operator %(in Size s1, in Size s2) => new Size(s1.ValueInBytes.Value % s2.ValueInBytes.Value);

        #endregion



        #region Size, numeric operators

        #region sbyte operators

        public static Size operator +(in Size s, in sbyte b) => b < 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be equal or greater than 0.") : b == 0 ? s : new Size(s.ValueInBytes + (ulong)b);

        public static Size operator -(in Size s, in sbyte b) => b < 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be equal or greater than 0.") : b == 0 ? s : new Size(s.ValueInBytes.Value - (ulong)b);

        public static Size operator *(in Size s, in sbyte b) => b < 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be equal or greater than 0.") : b == 0 ? new Size(0) : new Size(s.ValueInBytes * (ulong)b);

        public static Size operator /(in Size s, in sbyte b) => b <= 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be greater than 0.") : new Size(s.ValueInBytes.Value / (ulong)b);

        // public static Size operator %(in Size s, in sbyte b) => new Size(s.ValueInBytes % (ulong)b);

        #endregion



        #region byte operators

        public static Size operator +(in Size s, in byte b) => new Size(s.ValueInBytes + (ulong)b);

        public static Size operator -(in Size s, in byte b) => new Size(s.ValueInBytes.Value - (ulong)b);

        public static Size operator *(in Size s, in byte b) => new Size(s.ValueInBytes * (ulong)b);

        public static Size operator /(in Size s, in byte b) => new Size(s.ValueInBytes.Value / (ulong)b);

        public static Size operator %(in Size s, in byte b) => new Size(s.ValueInBytes.Value % (ulong)b);

        #endregion



        #region short operators

        public static Size operator +(in Size s, in short @short) => @short < 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be equal or greater than 0.") : @short == 0 ? s : new Size(s.ValueInBytes + (ulong)@short);

        public static Size operator -(in Size s, in short @short) => @short < 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be equal or greater than 0.") : @short == 0 ? s : new Size(s.ValueInBytes.Value - (ulong)@short);

        public static Size operator *(in Size s, in short @short) => @short < 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be equal or greater than 0.") : @short == 0 ? new Size(0) : new Size(s.ValueInBytes * (ulong)@short);

        public static Size operator /(in Size s, in short @short) => @short <= 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be greater than 0.") : new Size(s.ValueInBytes.Value / (ulong)@short);

        // public static Size operator %(in Size s, in short @short) => new Size(s.ValueInBytes % (ulong)@short);

        #endregion



        #region ushort operators

        public static Size operator +(in Size s, in ushort @short) => new Size(s.ValueInBytes + (ulong)@short);

        public static Size operator -(in Size s, in ushort @short) => new Size(s.ValueInBytes.Value - (ulong)@short);

        public static Size operator *(in Size s, in ushort @short) => new Size(s.ValueInBytes * (ulong)@short);

        public static Size operator /(in Size s, in ushort @short) => new Size(s.ValueInBytes.Value / (ulong)@short);

        public static Size operator %(in Size s, in ushort @short) => new Size(s.ValueInBytes.Value % (ulong)@short);

        #endregion



        #region int operators

        public static Size operator +(in Size s, in int i) => i < 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be equal or greater than 0.") : i == 0 ? s : new Size(s.ValueInBytes + (ulong)i);

        public static Size operator -(in Size s, in int i) => i < 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be equal or greater than 0.") : i == 0 ? s : new Size(s.ValueInBytes.Value - (ulong)i);

        public static Size operator *(in Size s, in int i) => i < 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be equal or greater than 0.") : i == 0 ? new Size(0) : new Size(s.ValueInBytes * (ulong)i);

        public static Size operator /(in Size s, in int i) => i <= 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be greater than 0.") : new Size(s.ValueInBytes.Value / (ulong)i);

        // public static Size operator %(in Size s, in int i) => new Size(s.ValueInBytes % (ulong)i);

        #endregion



        #region uint operators

        public static Size operator +(in Size s, in uint i) => new Size(s.ValueInBytes + (ulong)i);

        public static Size operator -(in Size s, in uint i) => new Size(s.ValueInBytes.Value - (ulong)i);

        public static Size operator *(in Size s, in uint i) => new Size(s.ValueInBytes * (ulong)i);

        public static Size operator /(in Size s, in uint i) => new Size(s.ValueInBytes.Value / (ulong)i);

        public static Size operator %(in Size s, in uint i) => new Size(s.ValueInBytes.Value % (ulong)i);

        #endregion



        #region long operators

        public static Size operator +(in Size s, in long l) => l < 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be equal or greater than 0.") : l == 0 ? s : new Size(s.ValueInBytes + (ulong)l);

        public static Size operator -(in Size s, in long l) => l < 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be equal or greater than 0.") : l == 0 ? s : new Size(s.ValueInBytes.Value - (ulong)l);

        public static Size operator *(in Size s, in long l) => l < 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be equal or greater than 0.") : l == 0 ? new Size(0) : new Size(s.ValueInBytes * (ulong)l);

        public static Size operator /(in Size s, in long l) => l <= 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be greater than 0.") : new Size(s.ValueInBytes.Value / (ulong)l);

        // public static Size operator %(in Size s, in long l) => new Size(s.ValueInBytes % (ulong)l);

        #endregion



        #region ulong operators

        public static Size operator +(in Size s, in ulong l) => new Size(s.ValueInBytes + l);

        public static Size operator -(in Size s, in ulong l) => new Size(s.ValueInBytes.Value - l);

        public static Size operator *(in Size s, in ulong l) => new Size(s.ValueInBytes * l);

        public static Size operator /(in Size s, in ulong l) => new Size(s.ValueInBytes.Value / l);

        public static Size operator %(in Size s, in ulong l) => new Size(s.ValueInBytes.Value % l);

        #endregion

        #endregion



        #region Numeric, Size operators

        #region sbyte operators

        public static Size operator +(in sbyte b, in Size s) => b < 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be equal or greater than 0.") : b == 0 ? s : new Size((ulong)b + s.ValueInBytes);

        public static Size operator -(in sbyte b, in Size s) => b < 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be equal or greater than 0.") : b == 0 ? s : new Size((ulong)b - s.ValueInBytes.Value);

        public static Size operator *(in sbyte b, in Size s) => b < 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be equal or greater than 0.") : b == 0 ? new Size(0) : new Size((ulong)b * s.ValueInBytes);

        public static Size operator /(in sbyte b, in Size s) => b <= 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be greater than 0.") : b == 0 ? s : new Size((ulong)b / s.ValueInBytes.Value);

        // public static Size operator %(in sbyte b, in Size s) => new Size((ulong)b % s.ValueInBytes);

        #endregion



        #region byte operators

        public static Size operator +(in byte b, in Size s) => new Size((ulong)b + s.ValueInBytes);

        public static Size operator -(in byte b, in Size s) => new Size((ulong)b - s.ValueInBytes.Value);

        public static Size operator *(in byte b, in Size s) => new Size((ulong)b * s.ValueInBytes);

        public static Size operator /(in byte b, in Size s) => new Size((ulong)b / s.ValueInBytes.Value);

        public static Size operator %(in byte b, in Size s) => new Size((ulong)b % s.ValueInBytes.Value);

        #endregion



        #region short operators

        public static Size operator +(in short @short, in Size s) => @short < 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be equal or greater than 0.") : @short == 0 ? s : new Size((ulong)@short + s.ValueInBytes);

        public static Size operator -(in short @short, in Size s) => @short < 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be equal or greater than 0.") : @short == 0 ? s : new Size((ulong)@short - s.ValueInBytes.Value);

        public static Size operator *(in short @short, in Size s) => @short < 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be equal or greater than 0.") : @short == 0 ? new Size(0) : new Size((ulong)@short * s.ValueInBytes);

        public static Size operator /(in short @short, in Size s) => @short <= 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be greater than 0.") : @short == 0 ? s : new Size((ulong)@short / s.ValueInBytes.Value);

        // public static Size operator %(in sbyte b, in Size s) => new Size((ulong)b % s.ValueInBytes);

        #endregion



        #region ushort operators

        public static Size operator +(in ushort @short, in Size s) => new Size((ulong)@short + s.ValueInBytes);

        public static Size operator -(in ushort @short, in Size s) => new Size((ulong)@short - s.ValueInBytes.Value);

        public static Size operator *(in ushort @short, in Size s) => new Size((ulong)@short * s.ValueInBytes);

        public static Size operator /(in ushort @short, in Size s) => new Size((ulong)@short / s.ValueInBytes.Value);

        public static Size operator %(in ushort @short, in Size s) => new Size((ulong)@short % s.ValueInBytes.Value);

        #endregion



        #region int operators

        public static Size operator +(in int i, in Size s) => i < 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be equal or greater than 0.") : i == 0 ? s : new Size((ulong)i + s.ValueInBytes);

        public static Size operator -(in int i, in Size s) => i < 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be equal or greater than 0.") : i == 0 ? s : new Size((ulong)i - s.ValueInBytes.Value);

        public static Size operator *(in int i, in Size s) => i < 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be equal or greater than 0.") : i == 0 ? new Size(0) : new Size((ulong)i * s.ValueInBytes);

        public static Size operator /(in int i, in Size s) => i <= 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be greater than 0.") : i == 0 ? s : new Size((ulong)i / s.ValueInBytes.Value);

        // public static Size operator %(in sbyte b, in Size s) => new Size((ulong)b % s.ValueInBytes);

        #endregion



        #region uint operators

        public static Size operator +(in uint i, in Size s) => new Size((ulong)i + s.ValueInBytes);

        public static Size operator -(in uint i, in Size s) => new Size((ulong)i - s.ValueInBytes.Value);

        public static Size operator *(in uint i, in Size s) => new Size((ulong)i * s.ValueInBytes);

        public static Size operator /(in uint i, in Size s) => new Size((ulong)i / s.ValueInBytes.Value);

        public static Size operator %(in uint i, in Size s) => new Size((ulong)i % s.ValueInBytes.Value);

        #endregion



        #region long operators

        public static Size operator +(in long l, in Size s) => l < 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be equal or greater than 0.") : l == 0 ? s : new Size((ulong)l + s.ValueInBytes);

        public static Size operator -(in long l, in Size s) => l < 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be equal or greater than 0.") : l == 0 ? s : new Size((ulong)l - s.ValueInBytes.Value);

        public static Size operator *(in long l, in Size s) => l < 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be equal or greater than 0.") : l == 0 ? new Size(0) : new Size((ulong)l * s.ValueInBytes);

        public static Size operator /(in long l, in Size s) => l <= 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be greater than 0.") : l == 0 ? s : new Size((ulong)l / s.ValueInBytes.Value);

        // public static Size operator %(in sbyte b, in Size s) => new Size((ulong)b % s.ValueInBytes);

        #endregion



        #region ulong operators

        public static Size operator +(in ulong l, in Size s) => new Size(l + s.ValueInBytes);

        public static Size operator -(in ulong l, in Size s) => new Size(l - s.ValueInBytes.Value);

        public static Size operator *(in ulong l, in Size s) => new Size(l * s.ValueInBytes);

        public static Size operator /(in ulong l, in Size s) => new Size(l / s.ValueInBytes.Value);

        public static Size operator %(in ulong l, in Size s) => new Size(l % s.ValueInBytes.Value);

        #endregion

        #endregion

        #endregion



        //#region Size operators

        ///// <summary>
        ///// Returns the size.
        ///// </summary>
        ///// <param name="s">Size to return</param>
        ///// <returns>The size value</returns> 
        //public static Size operator +(Size s) => new Size(+s.ValueInBytes, s.Unit);

        ///// <summary>
        ///// Returns the size opposite.
        ///// </summary>
        ///// <param name="s">Size for which one return the opposite</param>
        ///// <returns>The opposite of the size value</returns> 
        //public static Size operator -(Size s) => new Size(-s.ValueInBytes, s.Unit);

        //public static Size operator ~(Size s) => new Size(~s.GetValueInUnit(s.Unit), s.Unit); 

        //#endregion



        //public static Size operator <<(Size s, int i) => Create(s.GetValueInUnit(Unit.Byte) << i);

        //public static Size operator >>(Size s, int i) => Create(s.GetValueInUnit(Unit.Byte) >> i);



        #region Cast operators

        #region Numeric value to Size

        public static explicit operator Size(sbyte b) => b < 0 ? throw new ArgumentOutOfRangeException(nameof(b), b, $"{nameof(b)} must be equal or greater than 0.") : b == 0 ? new Size(0UL) : new Size((ulong)b);

        public static explicit operator Size(byte b) => new Size((ulong)b);

        public static explicit operator Size(short @short) => @short < 0 ? throw new ArgumentOutOfRangeException(nameof(@short), @short, $"{nameof(@short)} must be equal or greater than 0.") : @short == 0 ? new Size(0UL) : new Size((ulong)@short);

        public static explicit operator Size(ushort @short) => new Size(@short);

        public static explicit operator Size(int i) => i < 0 ? throw new ArgumentOutOfRangeException(nameof(i), i, $"{nameof(i)} must be equal or greater than 0.") : i == 0 ? new Size(0UL) : new Size((ulong)i);

        public static explicit operator Size(uint i) => new Size(i);

        ///// <summary>
        ///// Converts a <see cref="long"/> value to a <see cref="Size"/> value.
        ///// </summary>
        ///// <param name="i">The <see cref="long"/> to convert.</param>
        public static explicit operator Size(long l) => l < 0 ? throw new ArgumentOutOfRangeException(nameof(l), l, $"{nameof(l)} must be equal or greater than 0.") : l == 0 ? new Size(0UL) : new Size((ulong)l);

        public static explicit operator Size(ulong l) => new Size(l);

        #endregion

        #region Size to numeric value

        public static explicit operator sbyte(Size s) => (sbyte)s.ValueInBytes;

        public static explicit operator byte(Size s) => (byte)s.ValueInBytes;

        public static explicit operator short(Size s) => (short)s.ValueInBytes;

        public static explicit operator ushort(Size s) => (ushort)s.ValueInBytes;

        public static explicit operator int(Size s) => (int)s.ValueInBytes;

        public static explicit operator uint(Size s) => (uint)s.ValueInBytes;

        ///// <summary>
        ///// Converts a <see cref="Size"/> value to a <see cref="long"/> value.
        ///// </summary>
        ///// <param name="s">The <see cref="Size"/> to convert.</param>
        public static explicit operator long(Size s) => (long)s.ValueInBytes;

        public static explicit operator ulong(Size s) => (ulong)s.ValueInBytes;

        public static explicit operator float(Size s) => (float)s.ValueInBytes;

        public static explicit operator double(Size s) => (double)s.ValueInBytes;

        public static explicit operator decimal(Size s) => (decimal)s.ValueInBytes;

        #endregion

        #endregion

        #endregion
    }
}
