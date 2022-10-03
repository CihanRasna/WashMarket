using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("clerk", "customers", "currentLevel", "machineName", "singleWorkTime", "durability", "capacity", "consumption", "usingPrice", "animator", "buyPrice", "sellPrice", "totalGain", "unplaceableLayers", "machineMesh", "navMeshObstacle", "remainDurability", "_workedTime", "_needsRepair", "occupied", "<Filled>k__BackingField")]
	public class ES3UserType_Paydesk : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Paydesk() : base(typeof(GameplayScripts.Paydesk)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (GameplayScripts.Paydesk)obj;
			
			writer.WritePrivateFieldByRef("clerk", instance);
			writer.WritePrivateField("customers", instance);
			writer.WriteProperty("currentLevel", instance.currentLevel, ES3Internal.ES3TypeMgr.GetOrCreateES3Type(typeof(GameplayScripts.Machine.Level)));
			writer.WritePrivateField("machineName", instance);
			writer.WritePrivateField("singleWorkTime", instance);
			writer.WritePrivateField("durability", instance);
			writer.WritePrivateField("capacity", instance);
			writer.WritePrivateField("consumption", instance);
			writer.WritePrivateField("usingPrice", instance);
			writer.WritePrivateFieldByRef("animator", instance);
			writer.WritePrivateField("buyPrice", instance);
			writer.WritePrivateField("sellPrice", instance);
			writer.WritePrivateField("totalGain", instance);
			writer.WritePrivateField("unplaceableLayers", instance);
			writer.WritePrivateFieldByRef("machineMesh", instance);
			writer.WritePropertyByRef("navMeshObstacle", instance.navMeshObstacle);
			writer.WritePrivateField("remainDurability", instance);
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
					case "customers":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("customers", reader.Read<System.Collections.Generic.List<GameplayScripts.Customer>>(), instance);
					break;
					case "currentLevel":
						instance.currentLevel = reader.Read<GameplayScripts.Machine.Level>();
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
					case "animator":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("animator", reader.Read<UnityEngine.Animator>(), instance);
					break;
					case "buyPrice":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("buyPrice", reader.Read<System.Int32>(), instance);
					break;
					case "sellPrice":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("sellPrice", reader.Read<System.Int32>(), instance);
					break;
					case "totalGain":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("totalGain", reader.Read<System.Int32>(), instance);
					break;
					case "unplaceableLayers":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("unplaceableLayers", reader.Read<UnityEngine.LayerMask>(), instance);
					break;
					case "machineMesh":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("machineMesh", reader.Read<UnityEngine.Transform>(), instance);
					break;
					case "navMeshObstacle":
						instance.navMeshObstacle = reader.Read<UnityEngine.AI.NavMeshObstacle>();
						break;
					case "remainDurability":
					instance = (GameplayScripts.Paydesk)reader.SetPrivateField("remainDurability", reader.Read<System.Single>(), instance);
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