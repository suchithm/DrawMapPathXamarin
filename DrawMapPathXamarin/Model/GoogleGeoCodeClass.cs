using System;
using System.Collections.Generic;

namespace DrawMapPathXamarin
{
	public class AddressComponent
	{
		public string long_name { get; set; }
		public string short_name { get; set; }
		public List<string> types { get; set; }
	}
		//duplicate classes are removed
	public class Location
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Northeast2
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Southwest2
	{
		public double lat { get; set; }
		public double lng { get; set; }
	}

	public class Viewport
	{
		public Northeast2 northeast { get; set; }
		public Southwest2 southwest { get; set; }
	}

	public class Geometry
	{
		public Bounds bounds { get; set; }
		public Location location { get; set; }
		public string location_type { get; set; }
		public Viewport viewport { get; set; }
	}

	public class Result
	{
		public List<AddressComponent> address_components { get; set; }
		public string formatted_address { get; set; }
		public Geometry geometry { get; set; }
		public string place_id { get; set; }
		public List<string> types { get; set; }
	}

	public class GeoCodeJSONClass
	{
		public List<Result> results { get; set; }
		public string status { get; set; }
	}
}

