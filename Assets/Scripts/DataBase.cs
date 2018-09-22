using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

[CreateAssetMenu]
public class DataBase : ScriptableObject
{
	public List<Crop> cropList = new List<Crop>();
	public List<Animal> animalList = new List<Animal>();

	static DataBase _instance;
	public static DataBase Instance
	{
		get
		{
			if (_instance != null)
				return _instance;
			
			_instance = (DataBase)Resources.Load("DataBase");
			if (_instance != null)
				return _instance;

			_instance = CreateInstance<DataBase>();
			return _instance;
		}
	}
}
