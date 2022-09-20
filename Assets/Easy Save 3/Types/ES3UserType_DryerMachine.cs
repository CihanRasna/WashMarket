using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("currentLevel", "singleWorkTime", "durability", "capacity", "consumption")]
	public class ES3UserType_DryerMachine : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_DryerMachine() : base(typeof(GameplayScripts.DryerMachine)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (GameplayScripts.DryerMachine)obj;
			
			writer.WriteProperty("currentLevel", instance.currentLevel, ES3Type_enum.Instance);
			writer.WritePrivateField("singleWorkTime", instance);
			writer.WritePrivateField("durability", instance);
			writer.WritePrivateField("capacity", instance);
			writer.WritePrivateField("consumption", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (GameplayScripts.DryerMachine)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "currentLevel":
						instance.currentLevel = reader.Read<GameplayScripts.Machine.Level>(ES3Type_enum.Instance);
						break;
					case "singleWorkTime":
					instance = (GameplayScripts.DryerMachine)reader.SetPrivateField("singleWorkTime", reader.Read<System.Single>(), instance);
					break;
					case "durability":
					instance = (GameplayScripts.DryerMachine)reader.SetPrivateField("durability", reader.Read<System.Single>(), instance);
					break;
					case "capacity":
					instance = (GameplayScripts.DryerMachine)reader.SetPrivateField("capacity", reader.Read<System.Single>(), instance);
					break;
					case "consumption":
					instance = (GameplayScripts.DryerMachine)reader.SetPrivateField("consumption", reader.Read<System.Single>(), instance);
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

		public ES3UserType_DryerMachineArray() : base(typeof(GameplayScripts.DryerMachine[]), ES3UserType_DryerMachine.Instance)
		{
			Instance = this;
		}
	}
}