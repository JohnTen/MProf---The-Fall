using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ArithmeticOperator
{
	Add,
	Sub,
	Mul,
	Div,
}

public enum LogicalOperator
{
	And,
	Or,
	Not,
}

public enum RelationalOperator
{
	Equal,
	NotEqual,
	Greater,
	GreaterAndEqual,
	Lesser,
	LesserAndEqual,
}

[System.Serializable]
public class DynamicFormula
{
	public DynamicValue value1;
	public DynamicValue value2;
	public ArithmeticOperator @operator;

	public float Result
	{
		get
		{
			switch (@operator)
			{
				case ArithmeticOperator.Add: return value1 + value2;
				case ArithmeticOperator.Sub: return value1 - value2;
				case ArithmeticOperator.Mul: return value1 * value2;
				case ArithmeticOperator.Div: return value1 / value2;
			}

			return float.NaN;
		}
	}

	public static implicit operator float(DynamicFormula df)
	{
		return df.Result;
	}
}

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
