using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityUtility;
using UnityUtility.Conditions;

public enum ConditionType
{
	GameValue,
	GameEvent,
}

[System.Serializable]
public class EventConditionGroup : BaseConditionGroup<EventCondition>
{
	public EventConditionGroup() { }
	public EventConditionGroup(EventConditionGroup cond)
	{
		foreach (var c in cond.conditions)
		{
			conditions.Add(new EventCondition(c));
		}
		operators.AddRange(cond.operators);
	}
}

[System.Serializable]
public class EventCondition : BaseValueCondition<ConditionType>
{
	public DynamicValue value_1;
	public DynamicValue value_2;

	public EventCondition()
	{
		value_1 = new DynamicValue();
		value_2 = new DynamicValue();
	}

	public EventCondition(EventCondition condition)
	{
		type = condition.type;
		@operator = condition.@operator;
		value_1 = new DynamicValue(condition.value_1);
		value_2 = new DynamicValue(condition.value_2);
	}

	public override object Clone()
	{
		var condition = new EventCondition();
		condition.type = type;
		condition.@operator = @operator;
		condition.value_1 = new DynamicValue(value_1);
		condition.value_2 = new DynamicValue(value_2);

		return condition;
	}

	public override bool IsSatisfied()
	{
		switch (type)
		{
			case ConditionType.GameEvent:
				return CheckGameEventSatisfication();

			case ConditionType.GameValue:
				return CheckGameValueSatisfication();

			default:
				UnityEngine.Debug.LogWarning("Unidentifiable type");
				return false;
		}
	}

	bool CheckGameEventSatisfication()
	{
		var list = EventManager.Instance.EventList;

		if (value_1 < 0)
		{
			foreach (var e in list)
			{
				if (e.occurationEndDate >= TimeManager.Date) return true;
			}
			return false;
		}

		return list[(int)value_1].occurationEndDate >= TimeManager.Date;
	}

	bool CheckGameValueSatisfication()
	{
		switch (@operator)
		{
			case RelationalOperator.Equal:
				return value_1 == value_2;
			case RelationalOperator.Greater:
				return value_1 > value_2;
			case RelationalOperator.Lesser:
				return value_1 < value_2;
		}
		return false;
	}
}

public class GameValueCondition : EventCondition
{
	public GameValueCondition() : base()
	{
		type = ConditionType.GameValue;
	}
	public GameValueCondition(EventCondition condition) : base(condition) { }
}

public class GameEventCondition : EventCondition
{
	public GameEventCondition() : base()
	{
		type = ConditionType.GameEvent;
	}
	public GameEventCondition(EventCondition condition) : base(condition) { }
}
