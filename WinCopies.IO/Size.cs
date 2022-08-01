/* Copyright © Pierre Sprimont, 2019
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
using static WinCopies.Util.Util;

namespace WinCopies.IO
{
    /// <summary>
    /// Defines the unit of a size.
    /// </summary>
    public enum SizeUnit
    {

        /// <summary>
        /// A size given in bytes.
        /// </summary>
        Byte = 0,

        /// <summary>
        /// A size given in kilobytes.
        /// </summary>
        KiloByte = 1,

        /// <summary>
        /// A size given in megabytes.
        /// </summary>
        MegaByte = 2,

        /// <summary>
        /// A size given in gigabytes.
        /// </summary>
        GigaByte = 3,

        /// <summary>
        /// A size given in terabytes.
        /// </summary>
        TeraByte = 4,

        /// <summary>
        /// A size given in petabytes.
        /// </summary>
        PetaByte = 5,

        /// <summary>
        /// A size given in exabytes.
        /// </summary>
        Exabyte = 6,

        /// <summary>
        /// A size given in zettabytes.
        /// </summary>
        Zettabyte = 7,

        /// <summary>
        /// A size given in yottabytes.
        /// </summary>
        Yottabyte = 8

    }

    /// <summary>
    /// Represent a file size in byte-based unit.
    /// </summary>
    public struct Size
    {

        /// <summary>
        /// The value as numeric.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Returns the size value in given unit.
        /// </summary>
        /// <param name="unit">The unit to which one return the size</param>
        /// <returns>The size as the given unit</returns>
        public double GetValueInUnit(SizeUnit unit) => unit == SizeUnit
                ? Value
                : unit < SizeUnit ? Value * Math.Pow(1024, (SizeUnit - unit)) : Value / Math.Pow(1024, (unit - SizeUnit));

        /// <summary>
        /// The unit of this <see cref="Size"/>.
        /// </summary>
        public SizeUnit SizeUnit { get; }

        // public Speed() { Size = 0; SizeUnit = SizeUnit.Byte; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Size"/> structure without performing value check for the unit given. For value checking initialization of the <see cref="Size"/> structure, see the <see cref="Size.Create(double)"/> function.
        /// </summary>
        /// <param name="value">The value as numeric of the size</param>
        /// <param name="sizeUnit">The unit of the size</param>
        public Size(double value, SizeUnit sizeUnit)

        {

            Value = value;

            SizeUnit = sizeUnit;

        }

        /// <summary>
        /// Create a new <see cref="Size"/> object with the value given in bytes.
        /// </summary>
        /// <param name="valueInBytes">Value in bytes of the size</param>
        /// <returns>A new <see cref="Size"/> object with the value given in bytes</returns>
        public static Size Create(double valueInBytes)

        {

            double size = valueInBytes;

            double _size = valueInBytes;

            SizeUnit sizeUnit = SizeUnit.Byte;

            // if (size == 0) return new Size((long) size, sizeUnit);

            while (sizeUnit != SizeUnit.Yottabyte)

            {

                _size /= 1024;

                if (_size < 1) break;

                size = _size;
                //System.Windows.MessageBox.Show((size < 1).ToString());
                //System.Windows.MessageBox.Show(size.ToString());
                sizeUnit += 1;

            }

            //var s = new Size(0, SizeUnit.Byte);

            //var s0 = s;

            //s0 += s;

            //s0 <<= 3;

            return new Size(size, sizeUnit);

        }

        public override bool Equals(object obj) => IsNumber(obj)
                ? obj.GetType() == typeof(decimal)
                    ? (decimal)GetValueInUnit(SizeUnit.Byte) == (decimal)obj
                    : GetValueInUnit(SizeUnit.Byte) == (double)obj
                : obj is Size ? this == (Size)obj : false;

        public override int GetHashCode() => SizeUnit.GetHashCode() ^ Value.GetHashCode();

        internal string GetDisplaySizeUnit()

        {

            string value = "";

            switch (SizeUnit)

            {

                case SizeUnit.Byte:

                    value = "B";

                    break;

                case SizeUnit.KiloByte:

                    value = "Kb";

                    break;

                case SizeUnit.MegaByte:

                    value = "Mb";

                    break;

                case SizeUnit.GigaByte:

                    value = "Gb";

                    break;

                case SizeUnit.TeraByte:

                    value = "Tb";

                    break;

                case SizeUnit.PetaByte:

                    value = "Pb";

                    break;

                case SizeUnit.Exabyte:

                    value = "Eb";

                    break;

                case SizeUnit.Zettabyte:

                    value = "Zb";

                    break;

                case SizeUnit.Yottabyte:

                    value = "Yb";

                    break;

            }

            return value;

        }

        /// <summary>
        /// Returns a string with the size value and unit.
        /// </summary>
        /// <returns>A string with the size value and unit</returns>
        public override string ToString() => string.Format("{0} {1}", Value, GetDisplaySizeUnit());

        #region Size operators

        #region Equality operators 

        #region Size operators

        /// <summary>
        /// Checks if s1 is less than s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if s1 is less than s2, <see langword="false"/> in the other way</returns>
        public static bool operator <(Size s1, Size s2) => s1.GetValueInUnit(SizeUnit.Byte) < s2.GetValueInUnit(SizeUnit.Byte);

        /// <summary>
        /// Checkes if s1 is greater than s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if s1 is greater than s2, <see langword="false"/> in the other way</returns>
        public static bool operator >(Size s1, Size s2) => s1.GetValueInUnit(SizeUnit.Byte) > s2.GetValueInUnit(SizeUnit.Byte);

        /// <summary>
        /// Checks if s1 is less than or equal to s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if s1 is less than or equal to s2, <see langword="false"/> in the other way</returns>
        public static bool operator <=(Size s1, Size s2) => s1.GetValueInUnit(SizeUnit.Byte) <= s2.GetValueInUnit(SizeUnit.Byte);

        /// <summary>
        /// Checks if s1 is greater than or equal to s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if s1 is greater than or equal to s2, <see langword="false"/> in the other way</returns>
        public static bool operator >=(Size s1, Size s2) => s1.GetValueInUnit(SizeUnit.Byte) >= s2.GetValueInUnit(SizeUnit.Byte);

        /// <summary>
        /// Checks if s1 equals s2.
        /// </summary>
        /// <remarks>This comparison method is based on size value, as like as for other structure types.</remarks>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if s1 is equal to s2, <see langword="false"/> in the other way</returns>
        public static bool operator ==(Size s1, Size s2) => s1.GetValueInUnit(SizeUnit.Byte) == s2.GetValueInUnit(SizeUnit.Byte);

        /// <summary>
        /// Checks if s1 doesn't equal s2.
        /// </summary>
        /// <remarks>This comparison method is based on the size value, as like as for other structure types.</remarks>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns><see langword="true"/> if s1 is not equal to s2, <see langword="false"/> in the other way</returns>
        public static bool operator !=(Size s1, Size s2) => s1.GetValueInUnit(SizeUnit.Byte) != s2.GetValueInUnit(SizeUnit.Byte);

        #endregion



        #region Size, numeric operators

        #region int operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser than i, <see langword="false"/> if s equals or is greather than i.</returns>
        /// <remarks>i must be in byte unit.</remarks>
        public static bool operator <(Size s, int i) => s.GetValueInUnit(SizeUnit.Byte) < i;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather than i, <see langword="false"/> if s equals or is lesser than i.</returns>
        /// <remarks>i must be in byte unit.</remarks>
        public static bool operator >(Size s, int i) => s.GetValueInUnit(SizeUnit.Byte) > i;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser or equals i, <see langword="false"/> if s is greather than i.</returns>
        /// <remarks>i must be in byte unit.</remarks>
        public static bool operator <=(Size s, int i) => s.GetValueInUnit(SizeUnit.Byte) <= i;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather or equals i, <see langword="false"/> if s is lesser than i.</returns>
        /// <remarks>i must be in byte unit.</remarks>
        public static bool operator >=(Size s, int i) => s.GetValueInUnit(SizeUnit.Byte) >= i;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if s equals i, <see langword="false"/> in the other way.</returns>
        /// <remarks>i must be in byte unit.</remarks>
        public static bool operator ==(Size s, int i) => s.GetValueInUnit(SizeUnit.Byte) == i;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="int"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="i">The <see cref="int"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal i, <see langword="false"/> in the other way.</returns>
        /// <remarks>i must be in byte unit.</remarks>
        public static bool operator !=(Size s, int i) => s.GetValueInUnit(SizeUnit.Byte) != i;

        #endregion



        #region long operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser than l, <see langword="false"/> if s equals or is greather than l.</returns>
        /// <remarks>l must be in byte unit.</remarks>
        public static bool operator <(Size s, long l) => s.GetValueInUnit(SizeUnit.Byte) < l;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather than l, <see langword="false"/> if s equals or is lesser than l.</returns>
        /// <remarks>l must be in byte unit.</remarks>
        public static bool operator >(Size s, long l) => s.GetValueInUnit(SizeUnit.Byte) > l;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser or equals l, <see langword="false"/> if s is greather than l.</returns>
        /// <remarks>l must be in byte unit.</remarks>
        public static bool operator <=(Size s, long l) => s.GetValueInUnit(SizeUnit.Byte) <= l;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather or equals l, <see langword="false"/> if s is lesser than l.</returns>
        /// <remarks>l must be in byte unit.</remarks>
        public static bool operator >=(Size s, long l) => s.GetValueInUnit(SizeUnit.Byte) >= l;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if s equals l, <see langword="false"/> in the other way.</returns>
        /// <remarks>l must be in byte unit.</remarks>
        public static bool operator ==(Size s, long l) => s.GetValueInUnit(SizeUnit.Byte) == l;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal l, <see langword="false"/> in the other way.</returns>
        /// <remarks>l must be in byte unit.</remarks>
        public static bool operator !=(Size s, long l) => s.GetValueInUnit(SizeUnit.Byte) != l;

        #endregion



        #region short operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser than @short, <see langword="false"/> if s equals or is greather than @short.</returns>
        /// <remarks>@short must be in byte unit.</remarks>
        public static bool operator <(Size s, short @short) => s.GetValueInUnit(SizeUnit.Byte) < @short;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather than @short, <see langword="false"/> if s equals or is lesser than @short.</returns>
        /// <remarks>@short must be in byte unit.</remarks>
        public static bool operator >(Size s, short @short) => s.GetValueInUnit(SizeUnit.Byte) > @short;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser or equals @short, <see langword="false"/> if s is greather than @short.</returns>
        /// <remarks>@short must be in byte unit.</remarks>
        public static bool operator <=(Size s, short @short) => s.GetValueInUnit(SizeUnit.Byte) <= @short;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="@short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather or equals @short, <see langword="false"/> if s is lesser than @short.</returns>
        /// <remarks>@short must be in byte unit.</remarks>
        public static bool operator >=(Size s, short @short) => s.GetValueInUnit(SizeUnit.Byte) >= @short;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if s equals @short, <see langword="false"/> in the other way.</returns>
        /// <remarks>@short must be in byte unit.</remarks>
        public static bool operator ==(Size s, short @short) => s.GetValueInUnit(SizeUnit.Byte) == @short;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="short"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="short">The <see cref="short"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal @short, <see langword="false"/> in the other way.</returns>
        /// <remarks>@short must be in byte unit.</remarks>
        public static bool operator !=(Size s, short @short) => s.GetValueInUnit(SizeUnit.Byte) != @short;

        #endregion



        #region float operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser than f, <see langword="false"/> if s equals or is greather than f.</returns>
        /// <remarks>f must be in byte unit.</remarks>
        public static bool operator <(Size s, float f) => s.GetValueInUnit(SizeUnit.Byte) < f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather than f, <see langword="false"/> if s equals or is lesser than f.</returns>
        /// <remarks>f must be in byte unit.</remarks>
        public static bool operator >(Size s, float f) => s.GetValueInUnit(SizeUnit.Byte) > f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser or equals f, <see langword="false"/> if s is greather than f.</returns>
        /// <remarks>f must be in byte unit.</remarks>
        public static bool operator <=(Size s, float f) => s.GetValueInUnit(SizeUnit.Byte) <= f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather or equals f, <see langword="false"/> if s is lesser than f.</returns>
        /// <remarks>f must be in byte unit.</remarks>
        public static bool operator >=(Size s, float f) => s.GetValueInUnit(SizeUnit.Byte) >= f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if s equals f, <see langword="false"/> in the other way.</returns>
        /// <remarks>f must be in byte unit.</remarks>
        public static bool operator ==(Size s, float f) => s.GetValueInUnit(SizeUnit.Byte) == f;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="float"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="f">The <see cref="float"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal f, <see langword="false"/> in the other way.</returns>
        /// <remarks>f must be in byte unit.</remarks>
        public static bool operator !=(Size s, float f) => s.GetValueInUnit(SizeUnit.Byte) != f;

        #endregion



        #region double operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser than d, <see langword="false"/> if s equals or is greather than d.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator <(Size s, double d) => s.GetValueInUnit(SizeUnit.Byte) < d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather than d, <see langword="false"/> if s equals or is lesser than d.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator >(Size s, double d) => s.GetValueInUnit(SizeUnit.Byte) > d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser or equals d, <see langword="false"/> if s is greather than d.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator <=(Size s, double d) => s.GetValueInUnit(SizeUnit.Byte) <= d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather or equals d, <see langword="false"/> if s is lesser than d.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator >=(Size s, double d) => s.GetValueInUnit(SizeUnit.Byte) >= d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if s equals d, <see langword="false"/> in the other way.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator ==(Size s, double d) => s.GetValueInUnit(SizeUnit.Byte) == d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="double"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="double"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal d, <see langword="false"/> in the other way.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator !=(Size s, double d) => s.GetValueInUnit(SizeUnit.Byte) != d;

        #endregion



        #region decimal operators

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser than d, <see langword="false"/> if s equals or is greather than d.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator <(Size s, decimal d) => (decimal)s.GetValueInUnit(SizeUnit.Byte) < d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather than d, <see langword="false"/> if s equals or is lesser than d.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator >(Size s, decimal d) => (decimal)s.GetValueInUnit(SizeUnit.Byte) > d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if s is lesser or equals d, <see langword="false"/> if s is greather than d.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator <=(Size s, decimal d) => (decimal)s.GetValueInUnit(SizeUnit.Byte) <= d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if s is greather or equals d, <see langword="false"/> if s is lesser than d.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator >=(Size s, decimal d) => (decimal)s.GetValueInUnit(SizeUnit.Byte) >= d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if s equals d, <see langword="false"/> in the other way.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator ==(Size s, decimal d) => (decimal)s.GetValueInUnit(SizeUnit.Byte) == d;

        /// <summary>
        /// Compares a <see cref="Size"/> to a <see cref="decimal"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <param name="d">The <see cref="decimal"/> to compare.</param>
        /// <returns><see langword="true"/> if s doesn't equal d, <see langword="false"/> in the other way.</returns>
        /// <remarks>d must be in byte unit.</remarks>
        public static bool operator !=(Size s, decimal d) => (decimal)s.GetValueInUnit(SizeUnit.Byte) != d;

        #endregion

        #endregion



        #region Numeric, Size operators

        #region int operators

        public static bool operator <(int i, Size s) => i < s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >(int i, Size s) => i > s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator <=(int i, Size s) => i <= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >=(int i, Size s) => i >= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator ==(int i, Size s) => i == s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator !=(int i, Size s) => i != s.GetValueInUnit(SizeUnit.Byte);

        #endregion



        #region long operators

        /// <summary>
        /// Compares a <see cref="long"/> to a <see cref="Size"/> value.
        /// </summary>
        /// <param name="l">The <see cref="long"/> to compare.</param>
        /// <param name="s">The <see cref="Size"/> to compare.</param>
        /// <returns><see langword="true"/> if l is lesser than s, <see langword="false"/> if l equals or is greather than s.</returns>
        /// <remarks>l must be in byte unit.</remarks>
        public static bool operator <(long l, Size s) => l < s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >(long l, Size s) => l > s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator <=(long l, Size s) => l <= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >=(long l, Size s) => l >= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator ==(long l, Size s) => l == s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator !=(long l, Size s) => l != s.GetValueInUnit(SizeUnit.Byte);

        #endregion 



        #region short operators

        public static bool operator <(short @short, Size s) => @short < s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >(short @short, Size s) => @short > s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator <=(short @short, Size s) => @short <= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >=(short @short, Size s) => @short >= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator ==(short @short, Size s) => @short == s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator !=(short @short, Size s) => @short != s.GetValueInUnit(SizeUnit.Byte);

        #endregion



        #region float operators

        public static bool operator <(float f, Size s) => f < s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >(float f, Size s) => f > s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator <=(float f, Size s) => f <= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >=(float f, Size s) => f >= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator ==(float f, Size s) => f == s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator !=(float f, Size s) => f != s.GetValueInUnit(SizeUnit.Byte);

        #endregion



        #region double operators

        public static bool operator <(double d, Size s) => d < s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >(double d, Size s) => d > s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator <=(double d, Size s) => d <= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >=(double d, Size s) => d >= s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator ==(double d, Size s) => d == s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator !=(double d, Size s) => d != s.GetValueInUnit(SizeUnit.Byte);

        #endregion



        #region decimal operators

        public static bool operator <(decimal d, Size s) => d < (decimal)s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >(decimal d, Size s) => d > (decimal)s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator <=(decimal d, Size s) => d <= (decimal)s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator >=(decimal d, Size s) => d >= (decimal)s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator ==(decimal d, Size s) => d == (decimal)s.GetValueInUnit(SizeUnit.Byte);

        public static bool operator !=(decimal d, Size s) => d != (decimal)s.GetValueInUnit(SizeUnit.Byte);

        #endregion

        #endregion

        #endregion



        #region Arithmetic operators

        #region Size operators

        /// <summary>
        /// Returns a <see cref="Size"/> with the addition of s1 and s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the addition of s1 and s2</returns>
        public static Size operator +(Size s1, Size s2) => Create(s1.GetValueInUnit(SizeUnit.Byte) + s2.GetValueInUnit(SizeUnit.Byte));

        /// <summary>
        /// Returns a <see cref="Size"/> with the substraction of s2 from s1.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the substraction of s2 from s1</returns>
        public static Size operator -(Size s1, Size s2) => Create(s1.GetValueInUnit(SizeUnit.Byte) - s2.GetValueInUnit(SizeUnit.Byte));

        /// <summary>
        /// Returns a <see cref="Size"/> with the multiplication of s1 by s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the multiplication of s1 by s2</returns>
        public static Size operator *(Size s1, Size s2) => Create(s1.GetValueInUnit(SizeUnit.Byte) * s2.GetValueInUnit(SizeUnit.Byte));

        /// <summary>
        /// Returns a <see cref="Size"/> with the division of s1 by s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the division of s1 by s2</returns>
        public static Size operator /(Size s1, Size s2) => Create(s1.GetValueInUnit(SizeUnit.Byte) / s2.GetValueInUnit(SizeUnit.Byte));

        /// <summary>
        /// Returns a <see cref="Size"/> with the remainder of s1 by s2.
        /// </summary>
        /// <param name="s1">Left size</param>
        /// <param name="s2">Right size</param>
        /// <returns>A <see cref="Size"/> with the remainder of s1 by s2</returns>
        public static Size operator %(Size s1, Size s2) => Create(s1.GetValueInUnit(SizeUnit.Byte) % s2.GetValueInUnit(SizeUnit.Byte));

        #endregion



        #region Size, numeric operators

        #region int operators

        public static Size operator +(Size s, int i) => Create(s.GetValueInUnit(SizeUnit.Byte) + i);

        public static Size operator -(Size s, int i) => Create(s.GetValueInUnit(SizeUnit.Byte) - i);

        public static Size operator *(Size s, int i) => Create(s.GetValueInUnit(SizeUnit.Byte) * i);

        public static Size operator /(Size s, int i) => Create(s.GetValueInUnit(SizeUnit.Byte) / i);

        public static Size operator %(Size s, int i) => Create(s.GetValueInUnit(SizeUnit.Byte) % i);

        #endregion 



        #region long operators

        public static Size operator +(Size s, long l) => Create(s.GetValueInUnit(SizeUnit.Byte) + l);

        public static Size operator -(Size s, long l) => Create(s.GetValueInUnit(SizeUnit.Byte) - l);

        public static Size operator *(Size s, long l) => Create(s.GetValueInUnit(SizeUnit.Byte) * l);

        public static Size operator /(Size s, long l) => Create(s.GetValueInUnit(SizeUnit.Byte) / l);

        public static Size operator %(Size s, long l) => Create(s.GetValueInUnit(SizeUnit.Byte) % l);

        #endregion 



        #region short operators

        public static Size operator +(Size s, short @short) => Create(s.GetValueInUnit(SizeUnit.Byte) + @short);

        public static Size operator -(Size s, short @short) => Create(s.GetValueInUnit(SizeUnit.Byte) - @short);

        public static Size operator *(Size s, short @short) => Create(s.GetValueInUnit(SizeUnit.Byte) * @short);

        public static Size operator /(Size s, short @short) => Create(s.GetValueInUnit(SizeUnit.Byte) / @short);

        public static Size operator %(Size s, short @short) => Create(s.GetValueInUnit(SizeUnit.Byte) % @short);

        #endregion 



        #region float operators

        public static Size operator +(Size s, float f) => Create(s.GetValueInUnit(SizeUnit.Byte) + f);

        public static Size operator -(Size s, float f) => Create(s.GetValueInUnit(SizeUnit.Byte) - f);

        public static Size operator *(Size s, float f) => Create(s.GetValueInUnit(SizeUnit.Byte) * f);

        public static Size operator /(Size s, float f) => Create(s.GetValueInUnit(SizeUnit.Byte) / f);

        public static Size operator %(Size s, float f) => Create(s.GetValueInUnit(SizeUnit.Byte) % f);

        #endregion 



        #region double operators

        public static Size operator +(Size s, double d) => Create(s.GetValueInUnit(SizeUnit.Byte) + d);

        public static Size operator -(Size s, double d) => Create(s.GetValueInUnit(SizeUnit.Byte) - d);

        public static Size operator *(Size s, double d) => Create(s.GetValueInUnit(SizeUnit.Byte) * d);

        public static Size operator /(Size s, double d) => Create(s.GetValueInUnit(SizeUnit.Byte) / d);

        public static Size operator %(Size s, double d) => Create(s.GetValueInUnit(SizeUnit.Byte) % d);

        #endregion 

        #endregion



        #region Numeric, Size operators

        #region int operators

        public static Size operator +(int i, Size s) => Create(i + s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator -(int i, Size s) => Create(i - s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator *(int i, Size s) => Create(i * s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator /(int i, Size s) => Create(i / s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator %(int i, Size s) => Create(i % s.GetValueInUnit(SizeUnit.Byte));

        #endregion



        #region long operators

        public static Size operator +(long l, Size s) => Create(l + s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator -(long l, Size s) => Create(l - s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator *(long l, Size s) => Create(l * s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator /(long l, Size s) => Create(l / s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator %(long l, Size s) => Create(l % s.GetValueInUnit(SizeUnit.Byte));

        #endregion



        #region short operators

        public static Size operator +(short @short, Size s) => Create(@short + s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator -(short @short, Size s) => Create(@short - s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator *(short @short, Size s) => Create(@short * s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator /(short @short, Size s) => Create(@short / s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator %(short @short, Size s) => Create(@short % s.GetValueInUnit(SizeUnit.Byte));

        #endregion



        #region float operators

        public static Size operator +(float f, Size s) => Create(f + s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator -(float f, Size s) => Create(f - s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator *(float f, Size s) => Create(f * s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator /(float f, Size s) => Create(f / s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator %(float f, Size s) => Create(f % s.GetValueInUnit(SizeUnit.Byte));

        #endregion



        #region double operators

        public static Size operator +(double d, Size s) => Create(d + s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator -(double d, Size s) => Create(d - s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator *(double d, Size s) => Create(d * s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator /(double d, Size s) => Create(d / s.GetValueInUnit(SizeUnit.Byte));

        public static Size operator %(double d, Size s) => Create(d % s.GetValueInUnit(SizeUnit.Byte));

        #endregion

        #endregion

        #endregion



        #region Size operators

        /// <summary>
        /// Returns the size.
        /// </summary>
        /// <param name="s">Size to return</param>
        /// <returns>The size value</returns> 
        public static Size operator +(Size s) => new Size(+s.Value, s.SizeUnit);

        /// <summary>
        /// Returns the size opposite.
        /// </summary>
        /// <param name="s">Size for which one return the opposite</param>
        /// <returns>The opposite of the size value</returns> 
        public static Size operator -(Size s) => new Size(-s.Value, s.SizeUnit);

        //public static Size operator ~(Size s) => new Size(~s.GetValueInUnit(s.SizeUnit), s.SizeUnit); 

        #endregion



        //public static Size operator <<(Size s, int i) => Create(s.GetValueInUnit(SizeUnit.Byte) << i);

        //public static Size operator >>(Size s, int i) => Create(s.GetValueInUnit(SizeUnit.Byte) >> i);



        #region Cast operators

        #region Size to numeric value

        public static explicit operator Size(int i) => Create(i);

        /// <summary>
        /// Converts a <see cref="long"/> value to a <see cref="Size"/> value.
        /// </summary>
        /// <param name="i">The <see cref="long"/> to convert.</param>
        public static explicit operator Size(long l) => Create(l);

        public static explicit operator Size(short @short) => Create(@short);

        public static explicit operator Size(float f) => Create(f);

        public static explicit operator Size(double d) => Create(d);

        #endregion

        public static explicit operator int(Size s) => (int)s.GetValueInUnit(SizeUnit.Byte);

        /// <summary>
        /// Converts a <see cref="Size"/> value to a <see cref="long"/> value.
        /// </summary>
        /// <param name="s">The <see cref="Size"/> to convert.</param>
        public static explicit operator long(Size s) => (long)s.GetValueInUnit(SizeUnit.Byte);

        public static explicit operator short(Size s) => (short)s.GetValueInUnit(SizeUnit.Byte);

        public static explicit operator float(Size s) => (float)s.GetValueInUnit(SizeUnit.Byte);

        public static explicit operator double(Size s) => s.GetValueInUnit(SizeUnit.Byte);

        public static explicit operator decimal(Size s) => (decimal)s.GetValueInUnit(SizeUnit.Byte);

        #endregion

        #endregion
    }
}
