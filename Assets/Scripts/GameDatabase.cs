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
public class GameDatabase : ScriptableObject
{
	public List<Crop> cropList = new List<Crop>();
	public List<Animal> animalList = new List<Animal>();
	public List<FamilyMember> familyList = new List<FamilyMember>();
	public List<CropModelList> cropGrowingModelList = new List<CropModelList>();

	public List<GameEvent> eventList = new List<GameEvent>();

	static GameDatabase _instance;
	public static GameDatabase Instance
	{
		get
		{
			if (_instance != null)
				return _instance;
			
			_instance = (GameDatabase)Resources.Load("DataBase");
			if (_instance != null)
				return _instance;

			_instance = CreateInstance<GameDatabase>();
			return _instance;
		}
	}

	public GameObject GetGrowingModel(int cropId, int growingperiod)
	{
		return cropGrowingModelList[cropId].models[growingperiod];
	}
}
