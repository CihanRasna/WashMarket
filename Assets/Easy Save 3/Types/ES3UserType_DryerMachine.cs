using System;
using GameplayScripts.Machines;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("machineName", "durability", "totalGain", "obstacleEnabled", "remainDurability", "_workedTime", "_needsRepair")]
	public class ES3UserType_DryerMachine : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DryerMachine() : base(typeof(DryerMachine)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (DryerMachine)obj;
			
			writer.WritePrivateField("machineName", instance);
			writer.WritePrivateField("durability", instance);
			writer.WritePrivateField("totalGain", instance);
			writer.WriteProperty("obstacleEnabled", instance.obstacleEnabled, ES3Type_bool.Instance);
			writer.WritePrivateField("remainDurability", instance);
			writer.WritePrivateField("_workedTime", instance);
			writer.WritePrivateField("_needsRepair", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (DryerMachine)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "machineName":
					instance = (DryerMachine)reader.SetPrivateField("machineName", reader.Read<System.String>(), instance);
					break;
					case "durability":
					instance = (DryerMachine)reader.SetPrivateField("durability", reader.Read<System.Single>(), instance);
					break;
					case "totalGain":
					instance = (DryerMachine)reader.SetPrivateField("totalGain", reader.Read<System.Int32>(), instance);
					break;
					case "obstacleEnabled":
						instance.obstacleEnabled = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "remainDurability":
					instance = (DryerMachine)reader.SetPrivateField("remainDurability", reader.Read<System.Single>(), instance);
					break;
					case "_workedTime":
					instance = (DryerMachine)reader.SetPrivateField("_workedTime", reader.Read<System.Single>(), instance);
					break;
					case "_needsRepair":
					instance = (DryerMachine)reader.SetPrivateField("_needsRepair", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_DryerMachineArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_DryerMachineArray() : base(typeof(DryerMachine[]), ES3UserType_DryerMachine.Instance)
		{
			Instance = this;
		}
	}
}