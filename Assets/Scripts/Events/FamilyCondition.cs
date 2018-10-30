using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityUtility.Conditions;

public enum FamilyValueType
{
	MemberType,
	DyingRate,
	Hunger,
	MentalHealth,
}

[System.Serializable]
public class FamilyCondition : BaseValueCondition<FamilyValueType>
{
	public float value;
	public FamilyMember member;

	public FamilyCondition()
	{
		type = FamilyValueType.MemberType;
		value = 0;
	}

	public override object Clone()
	{
		var condition = new FamilyCondition();
		condition.type = type;
		condition.value = value;

		return condition;
	}

	public override bool IsSatisfied()
	{
		switch (type)
		{
			case FamilyValueType.MemberType:
				return CheckMember((int)member.type, value);
			case FamilyValueType.Hunger:
				return CheckMember(member.hunger, value);
			case FamilyValueType.MentalHealth:
				return CheckMember(member.mentalHealth, value);
			case FamilyValueType.DyingRate:
				return CheckMember(member.dyingRate, value);
		}

		return false;
	}

	bool CheckMember(float value1, float value2)
	{
		switch (@operator)
		{
			case RelationalOperator.Equal:
			return value1 == value2;
			case RelationalOperator.Greater:
			return value1 > value2;
			case RelationalOperator.Lesser:
			return value1 < value2;
		}

		return false;
	}
}

[System.Serializable]
public class FamilyConditionGroup : BaseConditionGroup<FamilyCondition>
{
	public FamilyConditionGroup() { }
	public FamilyConditionGroup(FamilyConditionGroup cond)
	{
		foreach (var c in cond.conditions)
		{
			conditions.Add((FamilyCondition)c.Clone());
		}
		operators.AddRange(cond.operators);
	}
}