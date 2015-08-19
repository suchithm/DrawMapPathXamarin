using System;
using Android.Gms.Maps;

namespace DrawMapPathXamarin
{   //to avoid map obsolete API
	public class OnMapReadyClass :Java.Lang.Object,IOnMapReadyCallback
	{ 
		public GoogleMap Map { get; private set; } 
		public event Action<GoogleMap> MapReadyAction;
		public void OnMapReady (GoogleMap googleMap)
		{
			Map = googleMap;
			if ( MapReadyAction != null )
				MapReadyAction (Map);
		}
	}
}