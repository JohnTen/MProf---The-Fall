using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityUtility.MVS;

public enum GameValueType
{
	__Crops,
	WheatConsumptionRate,
	OatConsumptionRate,
	CropProduction,
	NumberOfWheat,
	NumberOfOat,

	__Family,
	NumberOfWife,
	NumberOfDaughter,
	NumberOfSon,
	DyingRateOfWife,
	DyingRateOfDaughter,
	DyingRateOfSon,
	FamilyHunger,
	FamilyHungryDays,
	FamilyMentalHealth,
	AllFamilyMentalHealth,
	OtherFamilyMentalHealth,
	AdditionalDyingRate,

	__Animal,
	NumberOfOx,
	AnimalHunger,
	AnimalHungryDays,

	__Other,
	TaxRate,
	ForcedTaxRate,
}

[System.Serializable]
public class GameValueModifer : Modifier<GameValueType>
{
	public string serializedtype;

	public GameValueModifer() : base() { }
	public GameValueModifer(Modifier<GameValueType> modifier) : base(modifier)
	{
		serializedtype = propertyType.ToString();
	}
}

[System.Serializable]
public class GameValues : ModifiableValueSystem<GameValueType>, ISerializationCallbackReceiver
{
	[System.Serializable]
	protected class SerializedValue : InnerValues { }

	[SerializeField] SerializedValue basicValues;
	[SerializeField] SerializedValue modifiedValues;

	[SerializeField] List<GameValueModifer> modifers = new List<GameValueModifer>();

	protected override InnerValues BasicValue
	{
		get { return basicValues; }

		set { basicValues = value as SerializedValue; }
	}

	protected override InnerValues ModifiedValue
	{
		get { return modifiedValues; }

		set { modifiedValues = value as SerializedValue; }
	}

	/// <summary>
	/// Warning: you will get modified values from this but you will edited base values
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public virtual float this[GameValueType type]
	{
		get
		{
			return ModifiedValue[type];
		}

		set
		{
			BasicValue[type] = value;
		}
	}

	public void OnBeforeSerialize()
	{
		modifers.Clear();
		foreach (var m in Modifiers)
		{
			modifers.Add(new GameValueModifer(m));
		}
	}

	public void OnAfterDeserialize()
	{
		Modifiers.Clear();
		for (int i = 0; i < modifers.Count; i++)
		{
			Modifiers.Add(modifers[i]);
		}
	}
}
