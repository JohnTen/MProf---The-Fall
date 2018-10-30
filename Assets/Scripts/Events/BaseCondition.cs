using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityUtility.Conditions
{
	public enum RelationalOperator
	{
		Equal,
		Greater,
		Lesser,
	}

	public enum LogicalOperator
	{
		And,
		Or,
		Not,
	}

	public interface IConditionable : ICloneable
	{
		bool IsSatisfied();
	}

	public abstract class BaseValueCondition : IConditionable
	{
		public RelationalOperator @operator;

		public abstract object Clone();

		public abstract bool IsSatisfied();
	}

	public abstract class BaseValueCondition<E> : BaseValueCondition where E : struct, IConvertible
	{
		public E type;
	}

	public abstract class BaseConditionGroup<T> where T : class, IConditionable, new()
	{
		public List<LogicalOperator> operators = new List<LogicalOperator>();
		public List<T> conditions = new List<T>();

		public BaseConditionGroup() { }
		public BaseConditionGroup(BaseConditionGroup<T> cond)
		{
			foreach (var c in cond.conditions)
			{
				conditions.Add((T)c.Clone());
			}
			operators.AddRange(cond.operators);
		}

		public virtual bool IsSatisfied()
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
}
