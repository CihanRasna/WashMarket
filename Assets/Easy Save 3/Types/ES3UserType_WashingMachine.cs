using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("currentLevel", "singleWorkTime", "durability", "capacity", "consumption", "usingPrice", "nextLevelMachine", "animator", "_currentCustomer", "remainDurability", "_customerItems", "_workedTime", "_needsRepair", "occupied", "<Filled>k__BackingField")]
	public class ES3UserType_WashingMachine : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_WashingMachine() : base(typeof(GameplayScripts.WashingMachine)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (GameplayScripts.WashingMachine)obj;
			
			writer.WriteProperty("currentLevel", instance.currentLevel, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(GameplayScripts.Machine.Level)));
			writer.WritePrivateField("singleWorkTime", instance);
			writer.WritePrivateField("durability", instance);
			writer.WritePrivateField("capacity", instance);
			writer.WritePrivateField("consumption", instance);
			writer.WritePrivateField("usingPrice", instance);
			writer.WritePrivateFieldByRef("nextLevelMachine", instance);
			writer.WritePrivateFieldByRef("animator", instance);
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
			var instance = (GameplayScripts.WashingMachine)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "currentLevel":
						instance.currentLevel = reader.Read<GameplayScripts.Machine.Level>();
						break;
					case "singleWorkTime":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("singleWorkTime", reader.Read<System.Single>(), instance);
					break;
					case "durability":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("durability", reader.Read<System.Single>(), instance);
					break;
					case "capacity":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("capacity", reader.Read<System.Single>(), instance);
					break;
					case "consumption":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("consumption", reader.Read<System.Single>(), instance);
					break;
					case "usingPrice":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("usingPrice", reader.Read<System.Int32>(), instance);
					break;
					case "nextLevelMachine":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("nextLevelMachine", reader.Read<GameplayScripts.Machine>(), instance);
					break;
					case "animator":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("animator", reader.Read<UnityEngine.Animator>(), instance);
					break;
					case "_currentCustomer":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("_currentCustomer", reader.Read<GameplayScripts.Customer>(), instance);
					break;
					case "remainDurability":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("remainDurability", reader.Read<System.Single>(), instance);
					break;
					case "_customerItems":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("_customerItems", reader.Read<GameplayScripts.CustomerItem>(), instance);
					break;
					case "_workedTime":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("_workedTime", reader.Read<System.Single>(), instance);
					break;
					case "_needsRepair":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("_needsRepair", reader.Read<System.Boolean>(), instance);
					break;
					case "occupied":
						instance.occupied = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					case "<Filled>k__BackingField":
					instance = (GameplayScripts.WashingMachine)reader.SetPrivateField("<Filled>k__BackingField", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_WashingMachineArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_WashingMachineArray() : base(typeof(GameplayScripts.WashingMachine[]), ES3UserType_WashingMachine.Instance)
		{
			Instance = this;
		}
	}
}