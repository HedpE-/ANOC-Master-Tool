﻿using System;
using System.Reflection;
using System.ComponentModel;

namespace appCore
{
    /// <summary>
	/// Summary description for Enum.
	/// </summary>
	public static class EnumExtensions
    {
        /// <summary>
        /// Returns the value of the DescriptionAttribute if the specified Enum value has one.
        /// If not, returns the ToString() representation of the Enum value.
        /// </summary>
        /// <param name="value">The Enum to get the description for</param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivilant enumerated object.
        /// Note: Utilised the DescriptionAttribute for values that use it.
        /// </summary>
        /// <param name="enumType">The System.Type of the enumeration.</param>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <returns></returns>
        public static dynamic Parse(Type enumType, string value)
        {
            return Parse(enumType, value, false);
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivilant enumerated object.
        /// A parameter specified whether the operation is case-sensitive.
        /// Note: Utilised the DescriptionAttribute for values that use it.
        /// </summary>
        /// <param name="enumType">The System.Type of the enumeration.</param>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <param name="ignoreCase">Whether the operation is case-sensitive or not.</param>
        /// <returns></returns>
        public static dynamic Parse(Type enumType, string value, bool ignoreCase)
        {
            if (String.Equals(value, "ALCATEL", StringComparison.OrdinalIgnoreCase))
                value = "ALU";

            if (ignoreCase)
                value = value.ToLower();

            foreach (Enum val in Enum.GetValues(enumType))
            {
                string comparison = val.GetDescription();
                if (ignoreCase)
                    comparison = comparison.ToLower();
                if (val.GetDescription() == value)
                    return val;
            }

            return Enum.Parse(enumType, value, ignoreCase);
        }
    }
}
