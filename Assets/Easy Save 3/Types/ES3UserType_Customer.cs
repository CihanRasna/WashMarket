using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("workType", "state", "_targetPosition", "_currentlyUsingMachine", "_customerItem", "_workCost")]
	public class ES3UserType_Customer : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Customer() : base(typeof(GameplayScripts.Customer)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (GameplayScripts.Customer)obj;
			
			writer.WritePrivateField("workType", instance);
			writer.WritePrivateField("state", instance);
			writer.WritePrivateField("_targetPosition", instance);
			writer.WritePrivateFieldByRef("_currentlyUsingMachine", instance);
			writer.WritePrivateFieldByRef("_customerItem", instance);
			writer.WritePrivateField("_workCost", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (GameplayScripts.Customer)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "workType":
					instance = (GameplayScripts.Customer)reader.SetPrivateField("workType", reader.Read<GameplayScripts.Customer.WorkType>(), instance);
					break;
					case "state":
					instance = (GameplayScripts.Customer)reader.SetPrivateField("state", reader.Read<GameplayScripts.Customer.State>(), instance);
					break;
					case "_targetPosition":
					instance = (GameplayScripts.Customer)reader.SetPrivateField("_targetPosition", reader.Read<UnityEngine.Vector3>(), instance);
					break;
					case "_currentlyUsingMachine":
					instance = (GameplayScripts.Customer)reader.SetPrivateField("_currentlyUsingMachine", reader.Read<GameplayScripts.Machine>(), instance);
					break;
					case "_customerItem":
					instance = (GameplayScripts.Customer)reader.SetPrivateField("_customerItem", reader.Read<GameplayScripts.CustomerItem>(), instance);
					break;
					case "_workCost":
					instance = (GameplayScripts.Customer)reader.SetPrivateField("_workCost", reader.Read<System.Int32>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_CustomerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_CustomerArray() : base(typeof(GameplayScripts.Customer[]), ES3UserType_Customer.Instance)
		{
			Instance = this;
		}
	}
}