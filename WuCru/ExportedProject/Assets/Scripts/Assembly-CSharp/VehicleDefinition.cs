using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VehicleDefinition
{
	public static Dictionary<int, VehicleDefinition> vehicleDefinitions;

	public int id;

	public string name;

	public int season;

	public int blueprints;

	public string model;

	public float garageScale;

	public float traversalScale;

	public float teafarmScale;

	public VehicleType type;

	public string missile;

	public float missilerate;

	public int[] speed;

	public int[] health;

	public int[] damage;

	static VehicleDefinition()
	{
		vehicleDefinitions = new Dictionary<int, VehicleDefinition>();
		TextAsset textAsset = Resources.Load("vehicles") as TextAsset;
		using (StringReader stringReader = new StringReader(textAsset.text))
		{
			stringReader.ReadLine();
			for (string text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
			{
				string[] array = text.Split(',');
				int result;
				if (int.TryParse(array[1], out result))
				{
					int result2;
					int.TryParse(array[3], out result2);
					int result3;
					int.TryParse(array[4], out result3);
					float result4;
					float.TryParse(array[6], out result4);
					float result5;
					float.TryParse(array[7], out result5);
					float result6;
					float.TryParse(array[8], out result6);
					VehicleType vehicleType = VehicleType.Car;
					if (Enum.IsDefined(typeof(VehicleType), array[9]))
					{
						vehicleType = (VehicleType)Enum.Parse(typeof(VehicleType), array[9]);
					}
					float result7;
					float.TryParse(array[12], out result7);
					int[] array2 = new int[4];
					if (int.TryParse(array[13], out array2[0]) && int.TryParse(array[14], out array2[1]) && int.TryParse(array[15], out array2[2]) && int.TryParse(array[16], out array2[3]))
					{
						int[] array3 = new int[4];
						if (int.TryParse(array[17], out array3[0]) && int.TryParse(array[18], out array3[1]) && int.TryParse(array[19], out array3[2]) && int.TryParse(array[20], out array3[3]))
						{
							int[] array4 = new int[4];
							if (int.TryParse(array[21], out array4[0]) && int.TryParse(array[22], out array4[1]) && int.TryParse(array[23], out array4[2]) && int.TryParse(array[24], out array4[3]))
							{
								vehicleDefinitions.Add(result, new VehicleDefinition
								{
									id = result,
									name = array[0],
									season = result2,
									blueprints = result3,
									garageScale = result4,
									traversalScale = result5,
									teafarmScale = result6,
									model = array[5],
									type = vehicleType,
									missile = array[11],
									missilerate = result7,
									speed = array2,
									health = array3,
									damage = array4
								});
							}
						}
					}
				}
			}
		}
	}

	public static int GetBlueprintsToUnlockID(int id)
	{
		return vehicleDefinitions[id].blueprints;
	}
}
