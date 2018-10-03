using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityUtility;

public enum LogicalOperator
{
	And,
	Or,
	Not,
}

public enum RelationalOperator
{
	Equal,
	Greater,
	Lesser,
}

public enum ConditionType
{
	GameValue,
	GameEvent,
}

[System.Serializable]
public class EventConditionGroup
{
	public List<LogicalOperator> operators = new List<LogicalOperator>();
	public List<EventCondition> conditions = new List<EventCondition>();

	public EventConditionGroup() { }
	public EventConditionGroup(EventConditionGroup cond)
	{
		foreach (var c in cond.conditions)
		{
			conditions.Add(new EventCondition(c));
		}
		operators.AddRange(cond.operators);
	}

	public bool IsSatisfied()
	{
		bool finalResult = true;
		int operatorOffset = 0;

		for (int i = 0; i < conditions.Count; i++)
		{
			var result = conditions[i].IsSatisfied();

			if (i + operatorOffset >= operators.Count)
			{
				finalResult = result;
				break;
			}

			while (
				i + operatorOffset < operators.Count &&
				operators[i + operatorOffset] == LogicalOperator.Not
				)
			{
				result = !result;
				operatorOffset++;
			}

			if (i + operatorOffset >= operators.Count)
			{
				finalResult = result;
				break;
			}

			switch (operators[i + operatorOffset])
			{
				case LogicalOperator.And:
					if (result == false)
						return false;
					break;
				case LogicalOperator.Or:
					if (result == true)
						return true;
					break;

				default:
					break;
			}
			finalResult = result;
		}

		return finalResult;
	}
}

[System.Serializable]
public class EventCondition
{
	public ConditionType type;
	public RelationalOperator @operator;
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

	public bool IsSatisfied()
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
