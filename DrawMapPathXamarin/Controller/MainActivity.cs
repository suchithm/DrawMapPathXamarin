using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Locations;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace DrawMapPathXamarin
{
	[Activity ( Label = "DrawMapPathXamarin" , MainLauncher = true , Icon = "@drawable/icon" )]
	public class MainActivity : Activity
	{
		GoogleMap map;

		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );
 
			SetContentView ( Resource.Layout.Main );

			SetUpGoogleMap ();
//			LatLng latLngSource =FnLocationToLatLng(Constants.strSourceLocation);
//			LatLng latLngDestination =FnLocationToLatLng(Constants.strDestinationLocation);
//
//			if ( latLngSource != null && latLngDestination != null )
//				FnDrawPath ( Constants.strSourceLocation , latLngSource , Constants.strDestinationLocation , latLngDestination );
			//
			//<meta-data android:name="com.google.android.maps.v2.API_KEY" android:value="AIzaSyAWDtJGZB2VmOjkhu_F4EFnVtocKnxeijk" />
				
		}
		bool SetUpGoogleMap()
		{
			if(null != map) return false; 
			var frag = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.map); 
			var mapReadyCallback = new OnMapReadyClass(); 
			mapReadyCallback.MapReadyAction += delegate(GoogleMap obj )
			{
				map=obj; 
			}; 
			frag.GetMapAsync(mapReadyCallback);  
			return true;
		}
		async void FnDrawPath(string strSource,LatLng latLngSourcePosition,string strDestination,LatLng latLngDestPosition)
		{
			string strFullDirectionURL = string.Format (Constants.strGoogleDirectionUrl,strSource,strDestination);
			string strJSONDirectionResponse = await FnHttpRequest(strFullDirectionURL);
			if ( strJSONDirectionResponse != Constants.strException )
			{
				RunOnUiThread(()=>
				{ 
					if ( map != null )
					{ 
						map.Clear(); 
						MarkOnMap(Constants.strTextSource,latLngSourcePosition,Resource.Drawable.common_signin_btn_text_pressed_light);
						MarkOnMap(Constants.strTextDestination,latLngDestPosition,Resource.Drawable.common_signin_btn_text_pressed_dark);
					}
				});
				FnSetDirectionQuery ( strJSONDirectionResponse );
			}
			else
			{
				RunOnUiThread( () =>
					Toast.MakeText ( this , Constants.strUnableToConnect , ToastLength.Short ).Show ()); 
			}

		}
		void FnSetDirectionQuery(string strJSONDirectionResponse)
		{
			var objRoutes = JsonConvert.DeserializeObject<GoogleDirectionClass> ( strJSONDirectionResponse );
			Android.Graphics.Color pathColor = Android.Graphics.Color.Black;
			for ( int intCounter = 0 ; intCounter < objRoutes.routes.Count ; intCounter++ )
			{

				string encodedPoints =	objRoutes.routes [intCounter].overview_polyline.points; 

				List<Location> lstPoints =	FnDecodePolylinePoints ( encodedPoints );
				LatLng [] points = new LatLng[lstPoints.Count]; 
				int index = 0;
				foreach ( Location loc in lstPoints )
				{
					points [index++] = new LatLng ( loc.lat , loc.lng );
				}

				var polylineoption = new PolylineOptions (); 
				polylineoption.InvokeColor ( Android.Graphics.Color.Gray );
				polylineoption.Geodesic ( true );
				polylineoption.Add ( points ); 
				RunOnUiThread ( () =>
				map.AddPolyline ( polylineoption ) );
			} 
		}

		List<Location> FnDecodePolylinePoints(string encodedPoints) 
		{
			if ( string.IsNullOrEmpty ( encodedPoints ) )
				return null;
			List<Location> poly = new List<Location>();
			char[] polylinechars = encodedPoints.ToCharArray();
			int index = 0;

			int currentLat = 0;
			int currentLng = 0;
			int next5bits;
			int sum;
			int shifter;

			try
			{
				while (index < polylinechars.Length)
				{
					// calculate next latitude
					sum = 0;
					shifter = 0;
					do
					{
						next5bits = (int)polylinechars[index++] - 63;
						sum |= (next5bits & 31) << shifter;
						shifter += 5;
					} while (next5bits >= 32 && index < polylinechars.Length);

					if (index >= polylinechars.Length)
						break;

					currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

					//calculate next longitude
					sum = 0;
					shifter = 0;
					do
					{
						next5bits = (int)polylinechars[index++] - 63;
						sum |= (next5bits & 31) << shifter;
						shifter += 5;
					} while (next5bits >= 32 && index < polylinechars.Length);

					if (index >= polylinechars.Length && next5bits >= 32)
						break;

					currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
					Location p = new Location();
					p.lat = Convert.ToDouble(currentLat) / 100000.0;
					p.lng = Convert.ToDouble(currentLng) / 100000.0;
					poly.Add(p);
				} 
			}
			catch 
			{
				RunOnUiThread ( () =>
					Toast.MakeText ( this , Constants.strPleaseWait , ToastLength.Short ).Show () ); 
			}
			return poly;
		}

		 LatLng FnLocationToLatLng(string strLocation)
		{
			string strGeoCode = string.Format ("address={0}",strLocation);
			string strGeoCodeFullURL = string.Format (Constants.strGeoCodingUrl,strGeoCode);
			LatLng Position=null;
			string strResult= FnHttpRequestOnMainThread(strGeoCodeFullURL); 
			if ( strResult != Constants.strException )
			{ 
				var objGeoCodeJSONClass= JsonConvert.DeserializeObject<GeoCodeJSONClass> (strResult);  
				Position= new LatLng ( objGeoCodeJSONClass.results [0].geometry.location.lat , objGeoCodeJSONClass.results [0].geometry.location.lng );
			} 
			return Position;
		}
		void MarkOnMap(string title,LatLng pos, int resourceId )
		{
			RunOnUiThread ( () =>
			{
				try
				{
					var marker = new MarkerOptions ();
					marker.SetTitle ( title ); 
					marker.SetPosition ( pos ); //Resource.Drawable.BlueDot
					marker.SetIcon ( BitmapDescriptorFactory.FromResource ( resourceId ) );
					map.AddMarker ( marker );
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			} );
		} 

		WebClient webclient;
		async Task<string> FnHttpRequest(string strUri)
		{ 
			webclient = new WebClient ();
			string strResultData;
			try
			{
				strResultData= await webclient.DownloadStringTaskAsync (new Uri(strUri));
				Console.WriteLine(strResultData);
			}
			catch
			{
				strResultData = Constants.strException;
			}
			finally
			{
				if ( webclient!=null )
				{
					webclient.Dispose ();
					webclient = null; 
				}
			}

			return strResultData;
		}

		string FnHttpRequestOnMainThread(string strUri)
		{ 
			webclient = new WebClient ();
			string strResultData;
			try
			{
				strResultData=  webclient.DownloadString (new Uri(strUri));
				Console.WriteLine(strResultData);
			}
			catch
			{
				strResultData = Constants.strException;
			}
			finally
			{
				if ( webclient!=null )
				{
					webclient.Dispose ();
					webclient = null; 
				}
			}

			return strResultData;
		}
	}
}


