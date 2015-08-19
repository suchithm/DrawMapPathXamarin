using System;

namespace DrawMapPathXamarin
{
	static class Constants
	{
		internal static string strGoogleServerKey = "AIzaSyAWxxxe1Tc8-_t6Dq6CocGdb9nN-bc08CE";
		internal static string strGoogleDirectionUrl="https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&key="+strGoogleServerKey+"";
		internal static string strGeoCodingUrl="https://maps.googleapis.com/maps/api/geocode/json?{0}&key="+strGoogleServerKey+"";
		internal static string strSourceLocation="Vijayanagar,Bangalore,India";
		internal static string strDestinationLocation="Jayanagar,Bangalore,India";

		internal static string strException="Exception";
		internal static string strTextSource="Source";
		internal static string strTextDestination="Destination"; 

		internal static string strNoInternet="No online connection. Please review your internet connection"; 
		internal static string strPleaseWait="Please wait...";
		internal static string strUnableToConnect="Unable to connect server!,Please try after sometime"; 
	}
}

