using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("enabled")]
	public class ES3UserType_NavMeshObstacle : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_NavMeshObstacle() : base(typeof(UnityEngine.AI.NavMeshObstacle)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (UnityEngine.AI.NavMeshObstacle)obj;
			
			writer.WriteProperty("enabled", instance.enabled, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (UnityEngine.AI.NavMeshObstacle)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
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


	public class ES3UserType_NavMeshObstacleArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_NavMeshObstacleArray() : base(typeof(UnityEngine.AI.NavMeshObstacle[]), ES3UserType_NavMeshObstacle.Instance)
		{
			Instance = this;
		}
	}
}