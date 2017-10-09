/*
 * Created by SharpDevelop.
 * User: GONCARJ3
 * Date: 06/01/2017
 * Time: 20:43
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class StringExtensions
{
	public static String RemoveDigits(this string str) {
		return Regex.Replace(str, @"\d", "");
	}
	
	public static String RemoveLetters(this string str) {
		return Regex.Replace(str, "[^0-9.]", "");
	}

	public static int CountStringOccurrences(this string str, string pattern) {
		int num = 0;
		int startIndex = 0;
		while ((startIndex = str.IndexOf(pattern, startIndex, StringComparison.Ordinal)) != -1) {
			startIndex += pattern.Length;
			num++;
		}
		return num;
	}
	
	public static bool IsAllDigits(this string str) {
		foreach (char ch in str)
			if (!char.IsDigit(ch))
				return false;
		
		return true;
    }

    public static bool IsDigit(this char @char)
    {
        return IsAllDigits(@char.ToString());
    }

    public static String RemoveDiacritics(this string str) {
		var normalizedString = str.Normalize(NormalizationForm.FormD);
		var stringBuilder = new StringBuilder();

		foreach (var c in normalizedString)
		{
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
			if(unicodeCategory != UnicodeCategory.NonSpacingMark) {
				stringBuilder.Append(c);
			}
		}

		return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
	}
	
	public static String CapitalizeWords (this string str) {
		char[] array = str.ToCharArray();
		// Handle the first letter in the string.
		if (array.Length >= 1)
		{
			if (char.IsLower(array[0]))
			{
				array[0] = char.ToUpper(array[0]);
			}
		}
		// Scan through the letters, checking for spaces.
		// ... Uppercase the lowercase letters following spaces.
		for (int i = 1; i < array.Length; i++)
		{
			if (array[i - 1] == ' ')
			{
				if (char.IsLower(array[i]))
				{
					array[i] = char.ToUpper(array[i]);
				}
			}
		}
		return new string(array);
    }

    //public static String EncryptText(this string str)
    //{
    //    if (!string.IsNullOrEmpty(str))
    //    {
    //        string text = string.Empty;
    //        foreach (char ch in str)
    //            text += Convert.ToInt32(ch).ToString("x");
    //        str = text;
    //    }
    //    return str;
    //}

    public static String DecryptText(this string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            str = str.Replace(" ", "");
            byte[] bytes = new byte[str.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = Convert.ToByte(str.Substring(i * 2, 2), 0x10);

            str = Encoding.ASCII.GetString(bytes);
        }
        return str;
    }

    public static string Encrypt(this string str, string passPhrase)
    {
		return appCore.Toolbox.StringCipher.Encrypt(str, passPhrase);
    }

    public static string Encrypt(this string str)
    {
        return appCore.Toolbox.StringCipher.Encrypt(str);
    }

    public static string Decrypt(this string str)
    {
        return appCore.Toolbox.StringCipher.Decrypt(str);
    }

    public static string Decrypt(this string str, string passPhrase)
    {
        return appCore.Toolbox.StringCipher.Decrypt(str, passPhrase);
    }

    public static bool Contains(this string str, string pattern, StringComparison stringComparison)
    {
        return str.IndexOf(pattern, stringComparison) >= 0;
    }
}