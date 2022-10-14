using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("machineName", "totalGain", "obstacleEnabled", "_workedTime", "enabled")]
	public class ES3UserType_Paydesk : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Paydesk() : base(typeof(GameplayScripts.Machines.Paydesk)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (GameplayScripts.Machines.Paydesk)obj;
			
			writer.WritePrivateField("machineName", instance);
			writer.WritePrivateField("totalGain", instance);
			writer.WriteProperty("obstacleEnabled", instance.obstacleEnabled, ES3Type_bool.Instance);
			writer.WritePrivateField("_workedTime", instance);
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (GameplayScripts.Machines.Paydesk)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "machineName":
					instance = (GameplayScripts.Machines.Paydesk)reader.SetPrivateField("machineName", reader.Read<System.String>(), instance);
					break;
					case "totalGain":
					instance = (GameplayScripts.Machines.Paydesk)reader.SetPrivateField("totalGain", reader.Read<System.Int32>(), instance);
					break;
					case "obstacleEnabled":
						instance.obstacleEnabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "_workedTime":
					instance = (GameplayScripts.Machines.Paydesk)reader.SetPrivateField("_workedTime", reader.Read<System.Single>(), instance);
					break;
					case "enabled":
						instance.enabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
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

		public ES3UserType_PaydeskArray() : base(typeof(GameplayScripts.Machines.Paydesk[]), ES3UserType_Paydesk.Instance)
		{
			Instance = this;
		}
	}
}