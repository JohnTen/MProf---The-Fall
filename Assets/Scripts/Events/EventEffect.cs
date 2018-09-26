//using System.Collections.Generic;
//using UnityEngine;

//[System.Flags]
//public enum EventEffectApplyingType
//{
//	None,
//	Add,
//	Mul,
//}

//[System.Serializable]
//public class EventEffect
//{
//	public GameValueType type;
//	public float Value;
//	public EventEffectApplyingType vaildApplyingType;
//	public virtual bool IsAppliable() { return true; }

//	InnerEffect _effect;
//	InnerEffect Effect
//	{
//		get
//		{
//			if (_effect != null) return _effect;

//			try
//			{
//				var typeName = "EventEffect+" + type.ToString();
//				var tmp = System.Type.GetType(typeName);
//				var constructor = tmp.GetConstructor(new System.Type[0]);
//				_effect = constructor.Invoke(null) as InnerEffect;
//				return _effect;
//			}
//			catch (System.Exception e)
//			{
//				Debug.Log(e);
//				return null;
//			}
//		}
//	}

//	public void Apply() { Effect.Apply(this); }
//	public void Cease() { Effect.Cease(this); }

//	abstract class InnerEffect
//	{
//		public abstract void Apply(EventEffect effect);
//		public abstract void Cease(EventEffect effect);
//	}

//	class FoodConsumption : InnerEffect
//	{
//		public override void Apply(EventEffect effect)
//		{
//			GameDataManager.Instance.FamilyCropConsumingRate += effect.Value;
//		}

//		public override void Cease(EventEffect effect)
//		{
//			GameDataManager.Instance.FamilyCropConsumingRate -= effect.Value;
//		}
//	}

//	class DyingRate : InnerEffect
//	{
//		public override void Apply(EventEffect effect)
//		{
//			switch (effect.vaildApplyingType)
//			{
//				case EventEffectApplyingType.Add:
//					GameDataManager.Instance.FamilyDyingRate += effect.Value;
//					break;
//				case EventEffectApplyingType.Mul:
//					GameDataManager.Instance.FamilyDyingRate += effect.Value;
//					break;
//			}
			
//		}

//		public override void Cease(EventEffect effect)
//		{
//			switch (effect.vaildApplyingType)
//			{
//				case EventEffectApplyingType.Add:
//					GameDataManager.Instance.FamilyDyingRate -= effect.Value;
//					break;
//				case EventEffectApplyingType.Mul:
//					GameDataManager.Instance.FamilyDyingRate -= effect.Value;
//					break;
//			}
//		}
//	}

//	class CropProduction : InnerEffect
//	{
//		public override void Apply(EventEffect effect)
//		{
//			GameDataManager.Instance.CropProductRate += effect.Value;
//		}

//		public override void Cease(EventEffect effect)
//		{
//			GameDataManager.Instance.CropProductRate -= effect.Value;
//		}
//	}

//	class SeedProduction : InnerEffect
//	{
//		public override void Apply(EventEffect effect)
//		{
//			GameDataManager.Instance.SeedProductRate += effect.Value;
//		}

//		public override void Cease(EventEffect effect)
//		{
//			GameDataManager.Instance.SeedProductRate -= effect.Value;
//		}
//	}

//	class NumberOfFamily : InnerEffect
//	{
//		public override void Apply(EventEffect effect)
//		{
//			GameDataManager.CurrentFamily += (int)effect.Value;
//		}

//		public override void Cease(EventEffect effect)
//		{
//			GameDataManager.CurrentFamily -= (int)effect.Value;
//		}
//	}

//	class NumberOfOx : InnerEffect
//	{
//		public override void Apply(EventEffect effect)
//		{
//			GameDataManager.CurrentOx += (int)effect.Value;
//		}

//		public override void Cease(EventEffect effect)
//		{
//			GameDataManager.CurrentOx -= (int)effect.Value;
//		}
//	}

//	class NumberOfChicken : InnerEffect
//	{
//		public override void Apply(EventEffect effect)
//		{
//			GameDataManager.CurrentChicken += (int)effect.Value;
//		}

//		public override void Cease(EventEffect effect)
//		{
//			GameDataManager.CurrentChicken -= (int)effect.Value;
//		}
//	}

//	class TaxRate : InnerEffect
//	{
//		public override void Apply(EventEffect effect)
//		{
//			GameDataManager.Instance.AdditionalTaxRate += effect.Value;
//		}

//		public override void Cease(EventEffect effect)
//		{
//			GameDataManager.Instance.AdditionalTaxRate -= effect.Value;
//		}
//	}
//}
