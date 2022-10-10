using System;
using GameplayScripts.Machines;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("machineName", "totalGain", "obstacleEnabled", "_workedTime")]
	public class ES3UserType_Paydesk : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Paydesk() : base(typeof(Paydesk)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Paydesk)obj;
			
			writer.WritePrivateField("machineName", instance);
			writer.WritePrivateField("totalGain", instance);
			writer.WriteProperty("obstacleEnabled", instance.obstacleEnabled, ES3Type_bool.Instance);
			writer.WritePrivateField("_workedTime", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Paydesk)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "machineName":
					instance = (Paydesk)reader.SetPrivateField("machineName", reader.Read<System.String>(), instance);
					break;
					case "totalGain":
					instance = (Paydesk)reader.SetPrivateField("totalGain", reader.Read<System.Int32>(), instance);
					break;
					case "obstacleEnabled":
						instance.obstacleEnabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "_workedTime":
					instance = (Paydesk)reader.SetPrivateField("_workedTime", reader.Read<System.Single>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_PaydeskArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PaydeskArray() : base(typeof(Paydesk[]), ES3UserType_Paydesk.Instance)
		{
			Instance = this;
		}
	}
}