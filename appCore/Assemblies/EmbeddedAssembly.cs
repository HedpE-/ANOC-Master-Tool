﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

/// <summary>
/// A class for loading Embedded Assembly
/// </summary>
public static class EmbeddedAssemblies
{
	// Version 1.3
	
	public static void Init()
	{
        // http://www.codeproject.com/Articles/528178/Load-DLL-From-Embedded-Resource

        //Load("appCore.Assemblies.RestSharp.dll", "RestSharp.dll");
        //Load("appCore.Assemblies.Outlook.dll", "Outlook.dll");
        //Load("appCore.Assemblies.ICSharpCode.SharpZipLib.dll", "ICSharpCode.SharpZipLib.dll");
        Load("appCore.Assemblies.BMC.ARSystem.dll", "BMC.ARSystem.dll");
        Load("appCore.Assemblies.BMC.arnettoc.dll", "BMC.arnettoc.dll");
        Load("appCore.Assemblies.BMC.ARSystem.Utilities.Common.dll", "BMC.ARSystem.Utilities.Common.dll");
        Load("appCore.Assemblies.BMC.Atrium.dll", "BMC.Atrium.dll");
        //Load("appCore.Assemblies.GMap.NET.WindowsForms.dll", "GMap.NET.WindowsForms.dll");
        //Load("appCore.Assemblies.GMap.NET.Core.dll", "GMap.NET.Core.dll");
        //Load("appCore.Assemblies.FileHelpers.dll", "FileHelpers.dll");
        //Load("appCore.Assemblies.Newtonsoft.Json.dll", "Newtonsoft.Json.dll");
        //Load("appCore.Assemblies.GeoUk.dll", "GeoUk.dll");
        //Load("appCore.Assemblies.Svg2.3.0.dll", "Svg2.3.0.dll");

        AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
	}

	static Dictionary<string, Assembly> dic = null;

	/// <summary>
	/// Load Assembly, DLL from Embedded Resources into memory.
	/// </summary>
	/// <param name="embeddedResource">Embedded Resource string. Example: WindowsFormsApplication1.SomeTools.dll</param>
	/// <param name="fileName">File Name. Example: SomeTools.dll</param>
	static void Load(string embeddedResource, string fileName)
	{
		if (dic == null)
			dic = new Dictionary<string, Assembly>();

		byte[] ba = null;
		Assembly asm = null;
		Assembly curAsm = Assembly.GetExecutingAssembly();

		using (Stream stm = curAsm.GetManifestResourceStream(embeddedResource))
		{
			// Either the file is not existed or it is not mark as embedded resource
			if (stm == null)
				throw new Exception(embeddedResource + " is not found in Embedded Resources.");

			// Get byte[] from the file from embedded resource
			ba = new byte[(int)stm.Length];
			stm.Read(ba, 0, (int)stm.Length);
			try
			{
				asm = Assembly.Load(ba);

				// Add the assembly/dll into dictionary
				dic.Add(asm.FullName, asm);
				return;
			}
			catch
			{
				// Purposely do nothing
				// Unmanaged dll or assembly cannot be loaded directly from byte[]
				// Let the process fall through for next part
			}
		}

		bool fileOk = false;
		string tempFile = "";

		using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
		{
			// Get the hash value from embedded DLL/assembly
			string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);
			
			// Define the temporary storage location of the DLL/assembly
			tempFile = Path.GetTempPath() + fileName;

			// Determines whether the DLL/assembly is existed or not
			if (File.Exists(tempFile))
			{
				// Get the hash value of the existed file
				byte[] bb = File.ReadAllBytes(tempFile);
				string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

				// Compare the existed DLL/assembly with the Embedded DLL/assembly
				if (fileHash == fileHash2)
				{
					// Same file
					fileOk = true;
				}
				else
				{
					// Not same
					fileOk = false;
				}
			}
			else
			{
				// The DLL/assembly is not existed yet
				fileOk = false;
			}
		}

		// Create the file on disk
		if (!fileOk)
		{
			System.IO.File.WriteAllBytes(tempFile, ba);
		}
		
		// Load it into memory
		asm = Assembly.LoadFile(tempFile);

		// Add the loaded DLL/assembly into dictionary
		dic.Add(asm.FullName, asm);
	}

	/// <summary>
	/// Retrieve specific loaded DLL/assembly from memory
	/// </summary>
	/// <param name="assemblyFullName"></param>
	/// <returns></returns>
	static Assembly Get(string assemblyFullName)
	{
		if (dic == null || dic.Count == 0)
			return null;

		if (dic.ContainsKey(assemblyFullName))
			return dic[assemblyFullName];

		return null;

		// Don't throw Exception if the dictionary does not contain the requested assembly.
		// This is because the event of AssemblyResolve will be raised for every
		// Embedded Resources (such as pictures) of the projects.
		// Those resources wil not be loaded by this class and will not exist in dictionary.
	}
	
	static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
	{
		return EmbeddedAssemblies.Get(args.Name);
	}
}