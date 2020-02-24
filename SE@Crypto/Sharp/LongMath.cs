// Copyright (C) 2017 Schroedinger Entertainment
// Distributed under the Schroedinger Entertainment EULA (See EULA.md for details)

using System;
using System.Collections.Generic;

namespace SE.Crypto
{
    /// <summary>
    /// Big Integer Math Utilities
    /// </summary>
    public static class LongMath
    {
        struct UInt128
	    {
	        public UInt64 LowPart;
            public UInt64 HighPart;
	    }

        /// <summary>
		/// Sets the value of a to zero
		/// </summary>
		public static void AssignZero(UInt64[] a, int size)
		{
			for(int i = 0; i < size; i++)
				a[i] = 0;
		}
        /// <summary>
        /// Sets the value of a to zero
        /// </summary>
        public static void AssignZero(UInt64[] a, int index, int size)
        {
            for (int i = 0; i < size; i++)
                a[i + index] = 0;
        }
        /// <summary>
        /// Assigns b to a
        /// </summary>
        public static void Assign(UInt64[] a, UInt64[] b, int size)
        {
            for (int i = 0; i < size; i++)
                a[i] = b[i];
        }
		/// <summary>
		/// Assigns b to a
		/// </summary>
		public static void Assign(UInt64[] a, int aIndex, UInt64[] b, int bIndex, int size)
		{
			for(int i = 0; i < size; i++)
                a[i + aIndex] = b[i + bIndex];
		}

		/// <summary>
		/// Determines if a is zero
		/// </summary>
		public static bool IsZero(UInt64[] a, int size)
		{
			for(int i = 0; i < size; i++)
				if(a[i] != 0) return false;

			return true;
		}
        /// <summary>
        /// Determines if a is zero
        /// </summary>
        public static bool IsZero(UInt64[] a, int index, int size)
        {
            for (int i = 0; i < size; i++)
                if (a[i + index] != 0) return false;

            return true;
        }
		/// <summary>
		/// Determines if a is even
		/// </summary>
		public static bool IsEven(UInt64[] a, int size)
		{
			return ((a[0] & 1) == 0);
		}
		/// <summary>
		/// Compares two long values for equality
		/// </summary>
		public static Int32 Compare(UInt64[] a, UInt64[] b, int size)
		{
			for(int i = (size - 1); i >= 0; i--)
			{
				if(a[i] > b[i]) return 1;
				if(a[i] < b[i]) return -1;
			}
			return 0;
		}
        /// <summary>
        /// Compares two long values for equality
        /// </summary>
        public static Int32 Compare(UInt64[] a, int aIndex, UInt64[] b, int bIndex, int size)
        {
            for (int i = (size - 1); i >= 0; i--)
            {
                if (a[i + aIndex] > b[i + bIndex]) return 1;
                if (a[i + aIndex] < b[i + bIndex]) return -1;
            }
            return 0;
        }

		/// <summary>
		/// Determines the number of digits used
		/// </summary>
		public static int GetDigits(UInt64[] a, int size)
		{
			int result = (size - 1);
			while(result >= 0 && a[result] == 0)
				result--;

			return (result + 1);
		}
        /// <summary>
        /// Determines the number of digits used
        /// </summary>
        public static int GetDigits(UInt64[] a, int index, int size)
        {
            int result = (size - 1);
            while (result >= 0 && a[result + index] == 0)
                result--;

            return (result + 1);
        }
        /// <summary>
        /// Determines the highest bit set
        /// </summary>
        public static int GetBits(UInt64[] a, int size)
        {
            int digits = GetDigits(a, size);
            if (digits == 0)
                return 0;

            int result = 0;
            UInt64 digit = a[digits - 1];
            for (; digit != 0; result++)
                digit >>= 1;

            return ((digits - 1) * 64 + result);
        }
		/// <summary>
		/// Determines the highest bit set
		/// </summary>
		public static int GetBits(UInt64[] a, int index, int size)
		{
			int digits = GetDigits(a, size);
			if(digits == 0)
				return 0;

			int result = 0;
            UInt64 digit = a[digits - 1 + index];
			for(; digit != 0; result++)
				digit >>= 1;
    
			return ((digits - 1) * 64 + result);
		}

		/// <summary>
		/// Gets the long bit at index
		/// </summary>
		public static bool GetBitLong(UInt64[] a, int index, int size)
		{
			return ((a[index / 64] & ((UInt64)1 << (index % 64))) != 0);
		}

		/// <summary>
		/// Assigns sum of b and c to a
		/// </summary>
		public static UInt64 Add(UInt64[] a, UInt64[] b, UInt64[] c, int size)
		{
			UInt64 carry = 0;
			for(int i = 0; i < size; i++)
			{
				UInt64 tmp = (b[i] + c[i] + carry);
				if(tmp != b[i]) carry = ((tmp < b[i]) ? 1u : 0u);
				a[i] = tmp;
			}
			return carry;
		}
        /// <summary>
        /// Assigns sum of b and c to a
        /// </summary>
        public static UInt64 Add(UInt64[] a, int aIndex, UInt64[] b, int bIndex, UInt64[] c, int cIndex, int size)
        {
            UInt64 carry = 0;
            for (int i = 0; i < size; i++)
            {
                UInt64 tmp = (b[i + bIndex] + c[i + cIndex] + carry);
                if (tmp != b[i + bIndex]) carry = ((tmp < b[i + bIndex]) ? 1u : 0u);
                a[i + aIndex] = tmp;
            }
            return carry;
        }
        /// <summary>
        /// Assigns difference of b and c to a
        /// </summary>
        public static UInt64 Sub(UInt64[] a, UInt64[] b, UInt64[] c, int size)
        {
            UInt64 carry = 0;
            for (int i = 0; i < size; i++)
            {
                UInt64 tmp = b[i] - c[i] - carry;
                if (tmp != b[i]) carry = ((tmp > b[i]) ? 1u : 0u);
                a[i] = tmp;
            }
            return carry;
        }
		/// <summary>
		/// Assigns difference of b and c to a
		/// </summary>
        public static UInt64 Sub(UInt64[] a, int aIndex, UInt64[] b, int bIndex, UInt64[] c, int cIndex, int size)
		{
			UInt64 carry = 0;
			for(int i = 0; i < size; i++)
			{
                UInt64 tmp = b[i + bIndex] - c[i + cIndex] - carry;
                if (tmp != b[i + bIndex]) carry = ((tmp > b[i + bIndex]) ? 1u : 0u);
                a[i + aIndex] = tmp;
			}
			return carry;
		}

		/// <summary>
		/// Assigns b to a shifted left n bits
		/// </summary>
		public static UInt64 LeftShift(UInt64[] a, UInt64[] b, int n, int size)
		{
			UInt64 carry = 0;
			for(int i = 0; i < size; i++)
			{
				UInt64 tmp = b[i];
				a[i] = ((tmp << n) | carry);
				carry = (tmp >> (64 - n));
			}
			return carry;
		}
        /// <summary>
        /// Assigns b to a shifted left n bits
        /// </summary>
        public static UInt64 LeftShift(UInt64[] a, int aIndex, UInt64[] b, int bIndex, int n, int size)
        {
            UInt64 carry = 0;
            for (int i = 0; i < size; i++)
            {
                UInt64 tmp = b[i + bIndex];
                a[i + aIndex] = ((tmp << n) | carry);
                carry = (tmp >> (64 - n));
            }
            return carry;
        }
        /// <summary>
        /// Assigns b to a shifted right n bits
        /// </summary>
        public static UInt64 RightShift(UInt64[] a, UInt64[] b, int n, int size)
        {
            UInt64 carry = 0;
            for (int i = (size - 1); i >= 0; i--)
            {
                UInt64 tmp = b[i];
                a[i] = ((tmp >> n) | carry);
                carry = (tmp << (64 - n));
            }
            return carry;
        }
		/// <summary>
		/// Assigns b to a shifted right n bits
		/// </summary>
        public static UInt64 RightShift(UInt64[] a, int aIndex, UInt64[] b, int bIndex, int n, int size)
		{
			UInt64 carry = 0;
			for(int i = (size - 1); i >= 0; i--)
			{
                UInt64 tmp = b[i + bIndex];
                a[i + aIndex] = ((tmp >> n) | carry);
				carry = (tmp << (64 - n));
			}
			return carry;
		}

		private static UInt128 BitwiseAdd(UInt128 a, UInt128 b)
		{
			UInt128 result;
			result.LowPart = (a.LowPart + b.LowPart);
			result.HighPart = (a.HighPart + b.HighPart + ((result.LowPart < a.LowPart) ? 1u : 0u));
			return result;
		}
		private static UInt128 BitwiseMult(UInt64 a, UInt64 b)
		{
			UInt64 a0 = (a & 0xfffffffful);
			UInt64 a1 = (a >> 32);
			UInt64 b0 = (b & 0xfffffffful);
			UInt64 b1 = (b >> 32);
			UInt64 t0 = (a0 * b0);
			UInt64 t1 = (a0 * b1);
			UInt64 t2 = (a1 * b0);
			UInt64 t3 = (a1 * b1);
			t2 += (t0 >> 32);
			t2 += t1;

			if(t2 < t1) t3 += 0x100000000ul;
			UInt128 result = new UInt128()
			{ 
				LowPart = ((t0 & 0xfffffffful) | (t2 << 32)), 
				HighPart = (t3 + (t2 >> 32))
			};
			return result;
		}

		/// <summary>
		/// Assigns product of b and c to a. 
        /// a must be double word size
		/// </summary>
		public static void Mult(UInt64[] a, UInt64[] b, UInt64[] c, int size)
		{
			UInt128 result = new UInt128();
			for(int i = 0; i < ((size * 2) - 1); i++)
			{
				UInt64 carry = 0;
				int min = (i < size ? 0 : (i + 1) - size);
				for(int j = min; j <= i && j < size; j++)
				{
					UInt128 tmp = BitwiseMult(b[j], c[i - j]);
					result = BitwiseAdd(result, tmp);
					carry += ((result.HighPart < tmp.HighPart) ? 1u : 0u);
				}
				a[i] = result.LowPart;
				result.LowPart = result.HighPart;
				result.HighPart = carry;
			}
			a[((size * 2) - 1)] = result.LowPart;
		}
		/// <summary>
		/// Assigns division of c and d to a and remainder to b
		/// </summary>
		public static void Div(UInt64[] a, UInt64[] b, UInt64[] c, UInt64[] d, int size)
		{
			UInt64[] one = new UInt64[size];
            one[0] = 1;

			int divBits = GetBits(c, size);

            AssignZero(a, size);
            AssignZero(b, size);
			for(; divBits > 0; divBits--)
			{
                LeftShift(a, a, 1, size);
                LeftShift(b, b, 1, size);

                if (GetBitLong(c, divBits - 1, size))
                    Add(b, b, one, size);

                if (Compare(b, d, size) >= 0)
				{
                    Sub(b, b, d, size);
                    Add(a, a, one, size);
				}
			}
		}

		/// <summary>
		/// Assigns sum of b and c modulo mod to a
		/// </summary>
		public static void ModAdd(UInt64[] a, UInt64[] b, UInt64[] c, UInt64[] mod, int size)
		{
            UInt64 carry = Add(a, b, c, size);
            if (carry != 0 || Compare(a, mod, size) >= 0)
                Sub(a, a, mod, size);
		}
		/// <summary>
		/// Assigns difference of b and c modulo mod to a
		/// </summary>
		public static void ModSub(UInt64[] a, UInt64[] b, UInt64[] c, UInt64[] mod, int size)
		{
            UInt64 carry = Sub(a, b, c, size);
            if (carry != 0) Add(a, a, mod, size);
		}
		/// <summary>
		/// Assigns product of b and c modulo mod to a
		/// </summary>
		public static void ModMult(UInt64[] a, UInt64[] b, UInt64[] c, UInt64[] mod, int size)
		{
			UInt64[] prod = new UInt64[size * 2];
			UInt64[] modProd = new UInt64[size * 2];
            Mult(prod, b, c, size);

            int prodBits = GetBits(prod, size, size);
            int modBits = GetBits(mod, size);

			if(prodBits != 0) prodBits += size * 64;
            else prodBits = GetBits(prod, size);
			if(prodBits < modBits)
			{
                Assign(a, prod, size);
				return;
			}

            AssignZero(modProd, size);
            AssignZero(modProd, size, size);
			int digitShift = (prodBits - modBits) / 64;
			int bitShift = (prodBits - modBits) % 64;

            if (bitShift != 0) modProd[digitShift + size] = LeftShift(modProd, digitShift, mod, 0, bitShift, size);
            else Assign(modProd, digitShift, mod, 0, size);
            AssignZero(a, size);
			a[0] = 1;

            while (prodBits > size * 64 || Compare(modProd, mod, size) >= 0)
			{
                int equality = Compare(modProd, size, prod, size, size);
                if (equality < 0 || (equality == 0 && Compare(modProd, prod, size) <= 0))
				{
                    if (Sub(prod, prod, modProd, size) != 0) Sub(prod, size, prod, size, a, 0, size);
                    Sub(prod, size, prod, size, modProd, size, size);
				}
				UInt64 carry = (modProd[size] & 1) << 63;
                RightShift(modProd, size, modProd, size, 1, size);
                RightShift(modProd, modProd, 1, size);
				modProd[size - 1] |= carry;
				prodBits--;
			}

            Assign(a, prod, size);
		}
		/// <summary>
        /// Assigns inverse of b modulo mod to a if existing
		/// </summary>
		public static void ModInv(UInt64[] a, UInt64[] b, UInt64[] mod, int size)
		{
			UInt64[] tmp = new UInt64[size];
			UInt64[] m = new UInt64[size];
			UInt64[] x = new UInt64[size];
			UInt64[] y = new UInt64[size];
			
			if(IsZero(b, size))
			{
				AssignZero(a, size);
				return;
			}

			Assign(tmp, b, size);
			Assign(m, mod, size);
			AssignZero(x, size);
			AssignZero(y, size);
			x[0] = 1;
    
			int equality;
			while((equality = Compare(tmp, m, size)) != 0)
			{
				UInt64 carry = 0;
				if(IsEven(tmp, size))
				{
					RightShift(tmp, tmp, 1, size);
					if(!IsEven(x, size)) carry = Add(x, x, mod, size);
					RightShift(x, x, 1, size);

					if(carry != 0)
						x[size-1] |= 0x8000000000000000ul;
				}
				else if(IsEven(m, size))
				{
					RightShift(m, m, 1, size);
					if(!IsEven(y, size)) carry = Add(y, y, mod, size);
					RightShift(y, y, 1, size);

					if(carry != 0)
						y[size-1] |= 0x8000000000000000ul;
				}
				else if(equality > 0)
				{
                    Sub(tmp, tmp, m, size);
                    RightShift(tmp, tmp, 1, size);

                    if (Compare(x, y, size) < 0)
                        Add(x, x, mod, size);

                    Sub(x, x, y, size);
                    if (!IsEven(x, size)) carry = Add(x, x, mod, size);
                    RightShift(x, x, 1, size);

					if(carry != 0)
						x[size - 1] |= 0x8000000000000000ul;
				}
				else
				{
                    Sub(m, m, tmp, size);
                    RightShift(m, m, 1, size);

                    if (Compare(y, x, size) < 0)
                        Add(y, y, mod, size);

                    Sub(y, y, x, size);
                    if (!IsEven(y, size)) carry = Add(y, y, mod, size);
                    RightShift(y, y, 1, size);

					if(carry != 0)
						y[size-1] |= 0x8000000000000000ul;
				}
			}
            Assign(a, x, size);
		}
    }
}
