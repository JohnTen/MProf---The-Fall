using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

namespace UnityUtility.Interactables
{
	public abstract class MonoInteractable : MonoBehaviour, IInteractable
	{
		[SerializeField]
		protected bool isInteracting;

		[SerializeField]
		protected bool isActivated;

		[SerializeField]
		protected UnityEvent onActivated;

		[SerializeField]
		protected UnityEvent onDeactivated;

		public virtual bool IsInteracting { get { return isInteracting; } }
		public virtual bool IsActivated { get { return isActivated; } }

		public virtual event Action OnStartInteracting;
		public virtual event Action OnKeepInteracting;
		public virtual event Action OnStopInteracting;

		public virtual event Action OnActivated;
		public virtual event Action OnDeactivated;

		protected void InvokeStartInteracting() { if (OnStartInteracting != null) OnStartInteracting(); }
		protected void InvokeKeepInteracting() { if (OnKeepInteracting != null) OnKeepInteracting(); }
		protected void InvokeStopInteracting() { if (OnStopInteracting != null) OnStopInteracting(); }
		protected void InvokeActivated() { if (OnActivated != null) OnActivated(); }
		protected void InvokeDeactivated() { if (OnDeactivated != null) OnDeactivated(); }

		public virtual void StartInteracting()
		{
			if (!isInteracting)
			{
				InvokeStartInteracting();
			}
			isInteracting = true;
		}

		public virtual void StopInteracting()
		{
			if (isInteracting)
			{
				InvokeStopInteracting();
			}
			isInteracting = false;
		}

		protected virtual void Update()
		{
			if (isInteracting)
			{
				InvokeKeepInteracting();
			}
		}
	}
}
