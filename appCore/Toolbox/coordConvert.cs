/*
 * Created by SharpDevelop.
 * User: goncarj3
 * Date: 03-01-2016
 * Time: 11:02
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Linq;
using GMap.NET;

namespace appCore.Toolbox
{
	/// <summary>
	/// Description of coordConvert.
	/// </summary>
	public static class coordConvert
	{
		static Datum datum1;
		
		public static LLPoint toLat_Long(Point coord, string datumName) {
			LLPoint LL = new LLPoint();
			datum1 = datums.First(d => d.Name == datumName);
			LL.Latitude = E_N_to_Lat(coord);
			LL.Longitude = E_N_to_Long(coord);
			return LL;
		}
		
		static double E_N_to_Lat(Point coord) {
			// Un-project Transverse Mercator eastings and northings back to latitude.
			// Input: - _
			// eastings (East) and northings (North) in meters; _
			// ellipsoid axis dimensions (a & b) in meters; _
			// eastings (e0) and northings (n0) of false origin in meters; _
			// central meridian scale factor (f0) and _
			// latitude (PHI0) and longitude (LAM0) of false origin in decimal degrees.

			// REQUIRES THE "Marc" AND "InitialLat" FUNCTIONS

			// Convert angle measures to radians
			double RadPHI0 = datum1.PHI0 * (Math.PI / 180);
			double RadLAM0 = datum1.LAM0 * (Math.PI / 180);

			// Compute af0, bf0, e squared (e2), n and Et
			double af0 = datum1.a * datum1.f0;
			double bf0 = datum1.b * datum1.f0;
			
//			E_N_to_Lat = (180 / Pi) * (PHId - ((Math.Pow(Et, 2)) * VII) + ((Math.Pow(Et, 4)) * VIII) - ((Math.Pow(Et, 6)) * IX));
			
			double e2 = ((Math.Pow(af0, 2)) - (Math.Pow(bf0, 2))) / (Math.Pow(af0, 2));
			double n = (af0 - bf0) / (af0 + bf0);
			double Et = coord.Easting - datum1.e0;

			// Compute initial value for latitude (PHI) in radians
			double PHId = InitialLat(coord, af0, RadPHI0, n, bf0);
			
			// Compute nu, rho and eta2 using value for PHId
			double nu = af0 / (Math.Sqrt(1 - (e2 * (Math.Pow((Math.Sin(PHId)), 2)))));
			double rho = (nu * (1 - e2)) / (1 - (e2 * Math.Pow((Math.Sin(PHId)), 2)));
			double eta2 = (nu / rho) - 1;
			
			// Compute Latitude
			double VII = (Math.Tan(PHId)) / (2 * rho * nu);
			double VIII = ((Math.Tan(PHId)) / (24 * rho * (Math.Pow(nu, 3)))) * (5 + (3 * (Math.Pow((Math.Tan(PHId)), 2))) + eta2 - (9 * eta2 * (Math.Pow((Math.Tan(PHId)), 2))));
			double IX = ((Math.Tan(PHId)) / (720 * rho * (Math.Pow(nu, 5)))) * (61 + (90 * (Math.Pow((Math.Tan(PHId)), 2))) + (45 * (Math.Pow((Math.Tan(PHId)), 4))));
			
			return (180 / Math.PI) * (PHId - ((Math.Pow(Et, 2)) * VII) + ((Math.Pow(Et, 4)) * VIII) - ((Math.Pow(Et, 6)) * IX));
		}
		
		static double E_N_to_Long(Point coord) {
			// Un-project Transverse Mercator eastings and northings back to longitude.
			// Input: - _
			// eastings (East) and northings (North) in meters; _
			// ellipsoid axis dimensions (a & b) in meters; _
			// eastings (e0) and northings (n0) of false origin in meters; _
			// central meridian scale factor (f0) and _
			// latitude (PHI0) and longitude (LAM0) of false origin in decimal degrees.

			// REQUIRES THE "Marc" AND "InitialLat" FUNCTIONS

			// Convert angle measures to radians
			double RadPHI0 = datum1.PHI0 * (Math.PI / 180);
			double RadLAM0 = datum1.LAM0 * (Math.PI / 180);

			// Compute af0, bf0, e squared (e2), n and Et
			double af0 = datum1.a * datum1.f0;
			double bf0 = datum1.b * datum1.f0;
			double e2 = ((Math.Pow(af0, 2)) - (Math.Pow(bf0, 2))) / (Math.Pow(af0, 2));
			double n = (af0 - bf0) / (af0 + bf0);
			double Et = coord.Easting - datum1.e0;

			// Compute initial value for latitude (PHI) in radians
			double PHId = InitialLat(coord, af0, RadPHI0, n, bf0);
			
			// Compute nu, rho and eta2 using value for PHId
			double nu = af0 / (Math.Sqrt(1 - (e2 * (Math.Pow((Math.Sin(PHId)), 2)))));
			double rho = (nu * (1 - e2)) / (1 - (e2 * Math.Pow((Math.Sin(PHId)), 2)));
			double eta2 = (nu / rho) - 1;
			
			// Compute Longitude
			double X = Math.Pow(Math.Cos(PHId), -1) / nu;
			double XI = ((Math.Pow((Math.Cos(PHId)), -1)) / (6 * (Math.Pow(nu, 3)))) * ((nu / rho) + (2 * (Math.Pow((Math.Tan(PHId)), 2))));
			double XII = ((Math.Pow((Math.Cos(PHId)), -1)) / (120 * (Math.Pow(nu, 5)))) * (5 + (28 * (Math.Pow((Math.Tan(PHId)), 2))) + (24 * (Math.Pow((Math.Tan(PHId)), 4))));
			double XIIA = ((Math.Pow((Math.Cos(PHId)), -1)) / (5040 * (Math.Pow(nu, 7)))) * (61 + (662 * (Math.Pow((Math.Tan(PHId)), 2))) + (1320 * (Math.Pow((Math.Tan(PHId)), 4))) + (720 * (Math.Pow((Math.Tan(PHId)), 6))));

			return (180 / Math.PI) * (RadLAM0 + (Et * X) - ((Math.Pow(Et, 3)) * XI) + ((Math.Pow(Et, 5)) * XII) - ((Math.Pow(Et, 7)) * XIIA));
		}
		
		static double InitialLat(Point coord, double afo, double PHI0, double n, double bfo) {
			// Compute initial value for Latitude (PHI) IN RADIANS.
			// Input: - _
			// northing of point (North) and northing of false origin (n0) in meters; _
			// semi major axis multiplied by central meridian scale factor (af0) in meters; _
			// latitude of false origin (PHI0) IN RADIANS; _
			// n (computed from a, b and f0) and _
			// ellipsoid semi major axis multiplied by central meridian scale factor (bf0) in meters.
			
			// REQUIRES THE "Marc" FUNCTION
			// THIS FUNCTION IS CALLED BY THE "E_N_to_Lat", "E_N_to_Long" and "E_N_to_C" FUNCTIONS
			// THIS FUNCTION IS ALSO USED ON IT'S OWN IN THE  "Projection and Transformation Calculations.xls" SPREADSHEET

			// First PHI value (PHI1)
			double PHI1 = ((coord.Northing - datum1.n0) / afo) + PHI0;
			
			// Calculate M
			double M = Marc(bfo, n, PHI0, PHI1);
			
			// Calculate new PHI value (PHI2)
			double PHI2 = ((coord.Northing - datum1.n0 - M) / afo) + PHI1;
			
			// Iterate to get final value for InitialLat
			while (Math.Abs(coord.Northing - datum1.n0 - M) > 0.00001) {
				PHI2 = ((coord.Northing - datum1.n0 - M) / afo) + PHI1;
				M = Marc(bfo, n, PHI0, PHI2);
				PHI1 = PHI2;
			}
			
			return PHI2;
		}
		
		static double Marc(double bf0, double n, double PHI0, double PHI) {
			//Compute meridional arc.
			//Input: - _
			//ellipsoid semi major axis multiplied by central meridian scale factor (bf0) in meters; _
			//n (computed from a, b and f0); _
			//lat of false origin (PHI0) and initial or final latitude of point (PHI) IN RADIANS.

			//THIS FUNCTION IS CALLED BY THE - _
			//"Lat_Long_to_North" and "InitialLat" FUNCTIONS
			//THIS FUNCTION IS ALSO USED ON IT'S OWN IN THE "Projection and Transformation Calculations.xls" SPREADSHEET

			return bf0 * (((1 + n + ((5 / 4) * (Math.Pow(n, 2))) + ((5 / 4) * (Math.Pow(n, 3)))) * (PHI - PHI0))
			              - (((3 * n) + (3 * (Math.Pow(n, 2))) + ((21 / 8) * (Math.Pow(n, 3)))) * (Math.Sin(PHI - PHI0)) * (Math.Cos(PHI + PHI0)))
			              + ((((15 / 8) * (Math.Pow(n, 2))) + ((15 / 8) * (Math.Pow(n, 3)))) * (Math.Sin(2 * (PHI - PHI0))) * (Math.Cos(2 * (PHI + PHI0))))
			              - (((35 / 24) * (Math.Pow(n, 3))) * (Math.Sin(3 * (PHI - PHI0))) * (Math.Cos(3 * (PHI + PHI0)))));
		}
		
		static readonly Datum[] datums = new Datum[]
		{
			#region "datum definitions"
			new Datum()
			{
				Name = "OSGB36",
				a = 6377563.396,
				b = 6356256.909,
				f0 = 0.9996012717,
				e0 = 400000,
				n0 = -100000,
				PHI0 = 49,
				LAM0 = -2,
				dx = -446.448,
				dy = 125.157,
				dz = -542.06,
				ds = -20.4894,
				rx = -0.1502,
				ry = -0.2470,
				rz = -0.8421
			}
			#endregion
		};
		
		struct Datum
		{
			public string Name;
			public double a; // Semi-major axis, a
			public double b;  // Semi-minor axis, b
			public double f0; // Central Meridan Scale, F0
			public double e0; // True origin Easting, E0
			public double n0; // True origin Northing, N0
			public double PHI0; // True origin latitude, j0
			public double LAM0; // True origin longitude, l0
			public double dx; // translation parallel to X
			public double dy; // translation parallel to Y
			public double dz; // translation parallel to Z
			public double ds; // scale change
			public double rx; // rotation about X
			public double ry; // rotation about Y
			public double rz; // rotation about Z
		}
	}
}
