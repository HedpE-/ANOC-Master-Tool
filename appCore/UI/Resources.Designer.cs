﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace appCore.UI {
	using System;
	
	
	/// <summary>
	///   A strongly-typed resource class, for looking up localized strings, etc.
	/// </summary>
	// This class was auto-generated by the StronglyTypedResourceBuilder
	// class via a tool like ResGen or Visual Studio.
	// To add or remove a member, edit your .ResX file then rerun ResGen
	// with the /str option, or rebuild your VS project.
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	internal class Resources {
		
		private static global::System.Resources.ResourceManager resourceMan;
		
		private static global::System.Globalization.CultureInfo resourceCulture;
		
		[global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		internal Resources() {
		}
		
		/// <summary>
		///   Returns the cached ResourceManager instance used by this class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Resources.ResourceManager ResourceManager {
			get {
				if (object.ReferenceEquals(resourceMan, null)) {
					global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("appCore.UI.Resources", typeof(Resources).Assembly);
					resourceMan = temp;
				}
				return resourceMan;
			}
		}
		
		/// <summary>
		///   Overrides the current thread's CurrentUICulture property for all
		///   resource lookups using this strongly typed resource class.
		/// </summary>
		[global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
		internal static global::System.Globalization.CultureInfo Culture {
			get {
				return resourceCulture;
			}
			set {
				resourceCulture = value;
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap _lock {
			get {
				object obj = ResourceManager.GetObject("lock", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Byte[].
		/// </summary>
		internal static byte[] AMTmailTemplate {
			get {
				object obj = ResourceManager.GetObject("AMTmailTemplate", resourceCulture);
				return ((byte[])(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap arrow_left {
			get {
				object obj = ResourceManager.GetObject("arrow_left", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap arrow_right {
			get {
				object obj = ResourceManager.GetObject("arrow_right", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
		/// </summary>
		internal static System.Drawing.Icon Badass_browser {
			get {
				object obj = ResourceManager.GetObject("Badass browser", resourceCulture);
				return ((System.Drawing.Icon)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Badass_browser_1 {
			get {
				object obj = ResourceManager.GetObject("Badass browser_1", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Book_512 {
			get {
				object obj = ResourceManager.GetObject("Book-512", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Business_Planner_icon {
			get {
				object obj = ResourceManager.GetObject("Business-Planner-icon", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to A - UMTS900 Micro
		///B - UMTS2100 Micro
		///C - UMTS2100 Pico
		///D - DCS1800 Macro
		///F - Polecat
		///G - GSM900 Macro
		///H - UMTS900 Pico
		///I - GSM900 Indoor
		///J - Premier Paging
		///K - Select Paging
		///L - Flex Paging
		///M - UMTS900 Macro
		///N - LTE800 Macro
		///P - DCS1800 Indoor
		///Q - LTE2600 Macro
		///R - LTE2600 Pico
		///S - GSM900 Micro
		///U - DCS1800 Micro
		///V - UMTS2100 Femto
		///W - UMTS2100 Macro
		///X - Paknet
		///Z - TACS
		///ZE - LTE2100 Macro
		///ZK - LTE2100 Pico.
		/// </summary>
		internal static string Cells_Prefix {
			get {
				return ResourceManager.GetString("Cells Prefix", resourceCulture);
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Cells_Totals {
			get {
				object obj = ResourceManager.GetObject("Cells Totals", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to ####### ANOC Master Tool 7.0alpha8 03/12/2016 #######
		///
		///- Made the INCs/CRQs/BookIns/Alarms details window resizable
		///- Fixed VF/TF 2G cells identification issue where W &amp; Y cells were being identified as VF and not TF
		///- Fixed Outage parser crash containing alarms with empty Element column(cell name)
		///- Fixed Netcool parsing alarms with empty Element column(cell name)
		///
		///####### ANOC Master Tool 7.0alpha7 01/12/2016 #######
		///
		///- Tweaked the INCs/CRQs/BookIns/Alarms details window
		///- Tweaked all Scripts to [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string Changelog {
			get {
				return ResourceManager.GetString("Changelog", resourceCulture);
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to {\rtf1\fbidis\ansi\ansicpg1252\deff0\deflang2070{\fonttbl{\f0\fswiss\fprq2\fcharset0 Calibri;}{\f1\fnil\fcharset0 Microsoft Sans Serif;}}
		///\viewkind4\uc1\trowd\trgaph108\trleft-108\trbrdrt\brdrs\brdrw10 \trbrdrl\brdrs\brdrw10 \trbrdrb\brdrs\brdrw10 \trbrdrr\brdrs\brdrw10 \clbrdrt\brdrw15\brdrs\clbrdrl\brdrw15\brdrs\clbrdrb\brdrw15\brdrs\clbrdrr\brdrw15\brdrs \cellx2062\clbrdrt\brdrw15\brdrs\clbrdrl\brdrw15\brdrs\clbrdrb\brdrw15\brdrs\clbrdrr\brdrw15\brdrs \cellx4007\clbrdrt\brdrw15\brdrs\clbrdrl\brdrw15\brd [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string Contacts {
			get {
				return ResourceManager.GetString("Contacts", resourceCulture);
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to ANOC UK 1ª Linha\t00441635692067\t1stLineANOCUK@vodafone.com
		///ANOC UK 2ª Linha\t00441635692069
		///\t210915260
		///
		///Helpdesk (UK)\t00448454420304\tService.Desk@gb.vodafone.co.uk
		///Helpdesk (PT)\t210915100
		///
		///BT\t00448009170355
		///O2\t00441753281000 1-4\tlegacyfaults@o2.com
		///Orange\t00441189024222
		///C3\t00441189562400 1-3
		///\t00441189024662
		///Mittie\t00441329332884\tcatriona.mcgrory@mitie.com
		///C&amp;W\t00448000928939
		///Vdf NOC\t00441635682025
		///Change Management\t00441635674999\tSiteShutdowns@vodafone.com
		///
		///HNOC 4G Huawei\t [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string Contacts_1 {
			get {
				return ResourceManager.GetString("Contacts 1", resourceCulture);
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap globe {
			get {
				object obj = ResourceManager.GetObject("globe", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap globe_hover {
			get {
				object obj = ResourceManager.GetObject("globe_hover", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap home {
			get {
				object obj = ResourceManager.GetObject("home", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Icon similar to (Icon).
		/// </summary>
		internal static System.Drawing.Icon MB_0001_vodafone3 {
			get {
				object obj = ResourceManager.GetObject("MB_0001_vodafone3", resourceCulture);
				return ((System.Drawing.Icon)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to Acton\tXAN\t50 Brunel Road, West Way Industrial Estate, Acton, London, W3 7XR
		///Basingstoke\tXBE\tUnit 2, Gemini, Hamilton Close, Basingstoke, Hampshire, RG21 6YT
		///Bristol\tXBL\tUnit 1145, Aztec West, Almondsbury, Bristol, BS32 4TF
		///Cardiff\tXCF\tLinks Business Park, Fortran Road, St.Mellons, CF3 0LT
		///Carlisle\tXCE\tKings Drive, Kingmoor Park South, Kingstown, Carlisle, CA6 4RD
		///Crawley\tXCY\tCrawley Vector II, Newton Road, Manor Royal Estate, Crawley, RH10 9TT
		///Croydon\tXCN\tUnit 9, Peterwood Park, Peterwoo [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string MTX {
			get {
				return ResourceManager.GetString("MTX", resourceCulture);
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Phonetic_Chart {
			get {
				object obj = ResourceManager.GetObject("Phonetic_Chart", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap radio_tower {
			get {
				object obj = ResourceManager.GetObject("radio_tower", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap radio_tower_hover {
			get {
				object obj = ResourceManager.GetObject("radio_tower_hover", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap refresh {
			get {
				object obj = ResourceManager.GetObject("refresh", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Replace_64 {
			get {
				object obj = ResourceManager.GetObject("Replace-64", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Settings_hover {
			get {
				object obj = ResourceManager.GetObject("Settings_hover", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Settings_normal {
			get {
				object obj = ResourceManager.GetObject("Settings_normal", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap Spinner {
			get {
				object obj = ResourceManager.GetObject("Spinner", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap spinner1 {
			get {
				object obj = ResourceManager.GetObject("spinner1", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
		
		/// <summary>
		///   Looks up a localized string similar to {\rtf1\ansi\ansicpg1252\deff0\deflang2070{\fonttbl{\f0\fnil\fcharset0 Calibri;}{\f1\fnil\fcharset0 Microsoft Sans Serif;}}
		///\viewkind4\uc1\pard\sl240\slmult1\lang22\ul\b\f0\fs28 O2/VF Highlands and Island sites\ulnone\b0\fs22\par
		///\fs20\par
		///Sites 6200 - 6499 are the O2/Vodafone highlands and Islands project.\par
		///The Sites were designated either O2 or Vodafone lead. Whoever is the lead owner is responsible for cabin infrastructure, external to BTS AC/DC power and transmission.\par
		///\par
		///Site 6200-6320 are [rest of string was truncated]&quot;;.
		/// </summary>
		internal static string Useful_Info {
			get {
				return ResourceManager.GetString("Useful Info", resourceCulture);
			}
		}
		
		/// <summary>
		///   Looks up a localized resource of type System.Drawing.Bitmap.
		/// </summary>
		internal static System.Drawing.Bitmap zoozoo_wallpaper_15 {
			get {
				object obj = ResourceManager.GetObject("zoozoo_wallpaper_15", resourceCulture);
				return ((System.Drawing.Bitmap)(obj));
			}
		}
	}
}
