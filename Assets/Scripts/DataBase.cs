using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

[System.Serializable]
public class CropModelList
{
	public List<GameObject> models;
}

[CreateAssetMenu]
public class Database : ScriptableObject
{
	public List<Crop> cropList = new List<Crop>();
	public List<Animal> animalList = new List<Animal>();
	public List<Family> familyList = new List<Family>();
	public List<CropModelList> cropGrowingModelList = new List<CropModelList>();

	public List<GameEvent> eventList = new List<GameEvent>();

	static Database _instance;
	public static Database Instance
	{
		get
		{
			if (_instance != null)
				return _instance;
			
			_instance = (Database)Resources.Load("DataBase");
			if (_instance != null)
				return _instance;

			_instance = CreateInstance<Database>();
			return _instance;
		}
	}

	public GameObject GetGrowingModel(int cropId, int growingperiod)
	{
		return cropGrowingModelList[cropId].models[growingperiod];
	}
}
