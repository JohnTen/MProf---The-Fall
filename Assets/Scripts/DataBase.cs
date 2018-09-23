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
public class DataBase : ScriptableObject
{
	public List<Crop> cropList = new List<Crop>();
	public List<Animal> animalList = new List<Animal>();
	public List<CropModelList> cropGrowingModelList = new List<CropModelList>();

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

	public GameObject GetGrowingModel(int cropId, int growingperiod)
	{
		return cropGrowingModelList[cropId].models[growingperiod];
	}
}
