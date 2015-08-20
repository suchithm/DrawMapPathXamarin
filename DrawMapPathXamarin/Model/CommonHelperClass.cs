using System;
using Android.Net;
using Android.Content;

namespace DrawMapPathXamarin
{
	static class CommonHelperClass
	{
		internal static Boolean FnIsConnected(Context context)
		{
			try
			{
				var connectionManager = (ConnectivityManager)context.GetSystemService (Context.ConnectivityService); 
				NetworkInfo networkInfo = connectionManager.ActiveNetworkInfo; 
				if (networkInfo != null && networkInfo.IsConnected) 
				{
					return true;
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine ( ex.Message );
				//ensure access network state is enbled
				return false;
			}
			return false;
		} 

	}
}

