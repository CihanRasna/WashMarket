using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("clerk", "<CustomerQueuePositions>k__BackingField", "customers", "_inPayment", "currentLevel", "machineName", "singleWorkTime", "durability", "capacity", "consumption", "usingPrice", "nextLevelMachine", "animator", "buyPrice", "_currentCustomer", "remainDurability", "_customerItems", "_workedTime", "_needsRepair", "occupied", "<Filled>k__BackingField")]
	public class ES3UserType_Paydesk : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Paydesk() : base(typeof(GameplayScripts.Paydesk)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (GameplayScripts.Paydesk)obj;
			
			writer.WritePrivateFieldByRef("clerk", instance);
			writer.WritePrivateField("<CustomerQueuePositions>k__BackingField", instance);
			writer.WritePrivateField("customers", instance);
			writer.WritePrivateField("_inPayment", instance);
			writer.WriteProperty("currentLevel", instance.currentLevel, ES3Type_enum.Instance);
			writer.WritePrivateField("machineName", instance);
			writer.WritePrivateField("singleWorkTime", instance);
			writer.WritePrivateField("durability", instance);
			writer.WritePrivateField("capacity", instance);
			writer.WritePrivateField("consumption", instance);
			writer.WritePrivateField("usingPrice", instance);
			writer.WritePrivateFieldByRef("nextLevelMachine", instance);
			writer.WritePrivateFieldByRef("animator", instance);
			writer.WritePrivateField("buyPrice", instance);
			writer.WritePrivateFieldByRef("_currentCustomer", instance);
			writer.WritePrivateField("remainDurability", instance);
			writer.WritePrivateFieldByRef("_customerItems", instance);
			writer.WritePrivateField("_workedTime", instance);
			writer.WritePrivateField("_needsRepair", instance);
			writer.WriteProperty("occupied", instance.occupied, ES3Type_bool.Instance);
			writer.WritePrivateField("<Filled>k__BackingField", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (GameplayScripts.Paydesk)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "clerk":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("clerk", reader.Read<GameplayScripts.Actor>(), instance);
					break;
					case "<CustomerQueuePositions>k__BackingField":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("<CustomerQueuePositions>k__BackingField", reader.Read<System.Collections.Generic.Dictionary<System.Int32, UnityEngine.Transform>>(), instance);
					break;
					case "customers":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("customers", reader.Read<System.Collections.Generic.List<GameplayScripts.Customer>>(), instance);
					break;
					case "_inPayment":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("_inPayment", reader.Read<System.Boolean>(), instance);
					break;
					case "currentLevel":
						instance.currentLevel = reader.Read<GameplayScripts.Machine.Level>(ES3Type_enum.Instance);
						break;
					case "machineName":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("machineName", reader.Read<System.String>(), instance);
					break;
					case "singleWorkTime":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("singleWorkTime", reader.Read<System.Single>(), instance);
					break;
					case "durability":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("durability", reader.Read<System.Single>(), instance);
					break;
					case "capacity":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("capacity", reader.Read<System.Single>(), instance);
					break;
					case "consumption":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("consumption", reader.Read<System.Single>(), instance);
					break;
					case "usingPrice":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("usingPrice", reader.Read<System.Int32>(), instance);
					break;
					case "nextLevelMachine":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("nextLevelMachine", reader.Read<GameplayScripts.Machine>(), instance);
					break;
					case "animator":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("animator", reader.Read<UnityEngine.Animator>(), instance);
					break;
					case "buyPrice":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("buyPrice", reader.Read<System.Int32>(), instance);
					break;
					case "_currentCustomer":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("_currentCustomer", reader.Read<GameplayScripts.Customer>(), instance);
					break;
					case "remainDurability":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("remainDurability", reader.Read<System.Single>(), instance);
					break;
					case "_customerItems":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("_customerItems", reader.Read<GameplayScripts.CustomerItem>(), instance);
					break;
					case "_workedTime":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("_workedTime", reader.Read<System.Single>(), instance);
					break;
					case "_needsRepair":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("_needsRepair", reader.Read<System.Boolean>(), instance);
					break;
					case "occupied":
						instance.occupied = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "<Filled>k__BackingField":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("<Filled>k__BackingField", reader.Read<System.Boolean>(), instance);
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

		public ES3UserType_PaydeskArray() : base(typeof(GameplayScripts.Paydesk[]), ES3UserType_Paydesk.Instance)
		{
			Instance = this;
		}
	}
}