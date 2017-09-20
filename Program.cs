using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace slcsp
{
	// the individual object types for arrays
	class SLCSPObject
	{
		public string zipcode;
		public Decimal rate;
		public string rate_area;
	}
	class ZIPSObject
	{
		public string zipcode;
		public string state;
		public string county_code;
		public string name;
		public string rate_area;
	}
	class PLANSObject
	{
		public string plan_id;
		public string state;
		public string metal_level;
		public string rate;
		public string rate_area;
	}
	class RATEObject
	{
		public int rate_area;
		public List<Decimal> rates;
		public List<Decimal> distinct_rates;
		public Decimal second_lowest_rate;
	}

	// The Main Project
	class SLCSP
	{
		static void Main()
		{
			IEnumerable<SLCSPObject> slcspObj = SLCSP.readSLCSPcsv();
			Console.WriteLine("SLCSP: " + slcspObj.Count());
			//foreach (var element in slcspObj){Console.WriteLine(element.zipcode);}

			IEnumerable<ZIPSObject> zipsObj = SLCSP.readZIPScsv();
			Console.WriteLine("ZIPS: " + zipsObj.Count());
			//foreach (var element in zipsObj)/{Console.WriteLine(element.zipcode);}

			IEnumerable<PLANSObject> plansObj = SLCSP.readPLANScsv();
			Console.WriteLine("PLANS: " + plansObj.Count());
			//foreach (var element in plansObj){Console.WriteLine(element.plan_id);}

			foreach (var sObj in slcspObj)
			{
				foreach (var zObj in zipsObj)
				{
					if (sObj.zipcode == zObj.zipcode)
						sObj.rate_area = zObj.rate_area;
				}
			}

			// get a list of all the individual rate areas
			List<int> rateAreas = new List<int>();
			foreach (var pObj in plansObj)
			{
				rateAreas.Add(Convert.ToInt32(pObj.rate_area));
			}
			Console.WriteLine("rateAreas: " + rateAreas.Count());
			List<int> distinctRateAreas = rateAreas.Distinct().ToList();
			Console.WriteLine("distinctRateAreas: " + distinctRateAreas.Count());

			// for each rate area, get all the rates
			List<RATEObject> rates = new List<RATEObject>();
			foreach (var rateArea in distinctRateAreas)
			{
				RATEObject obj = new RATEObject();
				obj.rate_area = rateArea;
				obj.rates = new List<Decimal>();
				obj.distinct_rates = new List<Decimal>();
				foreach (var pObj in plansObj)
				{
					int rArea = Convert.ToInt32(pObj.rate_area);
					Decimal r = Convert.ToDecimal(pObj.rate);
					if (pObj.rate_area != null)
						if (rateArea == rArea)
							obj.rates.Add(r);
				}
				obj.distinct_rates = obj.rates.Distinct().ToList();
				obj.distinct_rates.Sort();
				Console.Write("OBJ "+ obj.distinct_rates.Count + ",");
				rates.Add(obj);
			}
			Console.WriteLine();
			Console.WriteLine("RATES: " + rates.Count());
			// for each rate area, determine the second lowest SILVER plan
			foreach (var rate in rates)
			{
				rate.second_lowest_rate = rate.distinct_rates[1];
				Console.WriteLine(rate.second_lowest_rate);
				Console.WriteLine(rate.distinct_rates[0] + ", " + rate.distinct_rates[1] + ", " + rate.distinct_rates[2] + ", " + rate.distinct_rates[3]);
			}

			// for each zip(SLCSP.CSV), check all rate_area rates
			foreach (var slcsp in slcspObj)
			{
				foreach (var rate in rates)
					if (rate.rate_area == Convert.ToInt32(slcsp.rate_area))
						slcsp.rate = rate.second_lowest_rate;
				Console.WriteLine(slcsp.zipcode + ": " + slcsp.rate);
			}

			// write to slcsp.csv SECOND COLUMN the rate (if unambiguous) for the second lowest silver plan
			writeToCSV(slcspObj);

			// Keep the console window open in debug mode.
			Console.WriteLine("Press any key to exit.");
			Console.ReadKey();
		}

		// Write to slcsp.csv
		public static void writeToCSV(IEnumerable<SLCSPObject> SLCSPObj)
		{
			string folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\assets\";
			string filePath = folder + "slcsp.csv";
			
			var csv = new StringBuilder();
			var header = string.Format("{0},{1}", "zipcode", "rate");
			csv.AppendLine(header);
			foreach (var obj in SLCSPObj)
			{
				var zipcode = obj.zipcode.ToString();
				var rate = obj.rate.ToString();
				var newLine = string.Format("{0},{1}", zipcode, rate);
				csv.AppendLine(newLine);
			}
			File.WriteAllText(filePath, csv.ToString());
		}

		// read in slcsp.csv	zipcode,rate
		public static IEnumerable<SLCSPObject> readSLCSPcsv()
		{
			string folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\assets\";
			string slcspCsv = folder + "slcsp.csv";
			List<SLCSPObject> zipCodes = new List<SLCSPObject>();
			using (var reader = new StreamReader(@slcspCsv))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');
					if (values[0] != "zipcode")
					{
						zipCodes.Add(new SLCSPObject
						{
							zipcode = values[0],
							rate = 0,//Convert.ToDecimal(values[1]),
							rate_area = ""
						});
					}
				}
			}
			return zipCodes;
		}



		// read in zips.csv		zipcode,state,county_code,name,rate_area
		public static IEnumerable<ZIPSObject> readZIPScsv()
		{
			//zips	zipcode,state,county_code,name,rate_area
			string folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\assets\";
			string zipsCsv = folder + "zips.csv";
			List<ZIPSObject> zipCodes = new List<ZIPSObject>();
			using (var reader = new StreamReader(@zipsCsv))
			{
				// get all rate_areas for each zipcode in slcsp.csv
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');
					if (values[0] != "zipcode")
					{
						zipCodes.Add(new ZIPSObject
						{
							zipcode = values[0],
							state = values[1],
							county_code = values[2],
							name = values[3],
							rate_area = values[4],
						});
					}
				}
			}
			return zipCodes;
		}

		// read in plans.csv	plan_id,state,metal_level,rate,rate_area
		public static IEnumerable<PLANSObject> readPLANScsv()
		{
			//plans	plan_id,state,metal_level,rate,rate_area
			string folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\assets\";
			string planCsv = folder + "plans.csv";
			List<PLANSObject> zipCodes = new List<PLANSObject>();
			using (var reader = new StreamReader(@planCsv))
			{
				while (!reader.EndOfStream)
				{
					var line = reader.ReadLine();
					var values = line.Split(',');
					if (values[0] != "plan_id")
					{
						// get all silver metal_level plans
						if (values[2] == "Silver")
						{
							zipCodes.Add(new PLANSObject
							{
								plan_id = values[0],
								state = values[1],
								metal_level = values[2],
								rate = values[3],
								rate_area = values[4]
							});
						}
					}
				}
			}
			return zipCodes;
		}
	}
}

//---------------------------------------------------------//
// slcsp.csv - zip codes in first column
//		Fill in 2nd column with the correct rate
//		Leave blank if there is no correct rate
// second lowest rate for a silver plan in the rate area

// plans.csv - all the health plans in the U.S. on the marketplace
//		metal level - Bronze, Silver, Gold, Platinum, or Catastrophic
//		rate - the amount that a consumer pays as a monthly premium
//		rate area - geographic region in a state that determines the plan's rate

// zips.csv - a mapping of ZIP Code to county/counties & rate area(s)

// Zip codes can be in more than one county, if not difinitive, the rate may still be determined
// Zip codes can be in more than one rate area, if not difinitive, leave blank
//---------------------------------------------------------//
