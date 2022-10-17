using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isPlayer")]
	public class ES3UserType_Clerk : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Clerk() : base(typeof(GameplayScripts.Characters.Clerk)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (GameplayScripts.Characters.Clerk)obj;
			
			writer.WritePrivateField("isPlayer", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (GameplayScripts.Characters.Clerk)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isPlayer":
					instance = (GameplayScripts.Characters.Clerk)reader.SetPrivateField("isPlayer", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_ClerkArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ClerkArray() : base(typeof(GameplayScripts.Characters.Clerk[]), ES3UserType_Clerk.Instance)
		{
			Instance = this;
		}
	}
}