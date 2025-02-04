using System.Collections.Generic;

namespace TURNPCB.Class
{
	/// <summary>
	/// Description of SessionManager.
	/// </summary>
	public  class SessionManager
	{
		public static readonly string ORACONN_SFIS = "Data Source=SFIS.WORLD;User Id=sfis1;Password=vnsfis2014#!;Min Pool Size=1;Max Pool Size=2";
        public static readonly string ORACONN_ALLPARTS = "Data Source=VNAP;User Id=AP2;Password=NSDAP2LOGPD0522!;Min Pool Size=1;Max Pool Size=2";

        public List<string> listProgram;
		
		public SessionManager()
		{
		}
	}
}
