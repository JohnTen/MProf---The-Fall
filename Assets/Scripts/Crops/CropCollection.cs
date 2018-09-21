using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

[CreateAssetMenu]
public class CropCollection : ScriptableObject
{
	public List<Crop> cropList = new List<Crop>();

	static CropCollection _instance;
	public static CropCollection Instance
	{
		get
		{
			if (_instance != null)
				return _instance;
			
			_instance = (CropCollection)Resources.Load("CropCollection");
			if (_instance != null)
				return _instance;

			_instance = CreateInstance<CropCollection>();
			return _instance;
		}
	}
}
