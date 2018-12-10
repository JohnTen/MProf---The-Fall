using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class DynamicValue
{
	public float constantValue;
	public bool useGameValue;
	public GameValueType valueType;
	public string serializedtype;

	public DynamicValue() { }
	public DynamicValue(DynamicValue value)
	{
		constantValue = value.constantValue;
		useGameValue = value.useGameValue;
		valueType = value.valueType;
		serializedtype = value.serializedtype;
	}

	public float Value
	{
		get { return useGameValue ? GameDataManager.GameValues[valueType] : constantValue; }
	}

	public static implicit operator float(DynamicValue dv)
	{
		return dv.Value;
	}
}
