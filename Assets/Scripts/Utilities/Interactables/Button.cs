using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityUtility.Interactables
{
	public class Button : MonoInteractable
	{
		public override void StartInteracting()
		{
			if (!isInteracting)
			{
				InvokeStartInteracting();
				InvokeActivated();

				if (onActivated != null)
					onActivated.Invoke();
			}

			isActivated = true;
			isInteracting = true;
		}

		public override void StopInteracting()
		{
			if (isInteracting)
			{
				InvokeStopInteracting();
				InvokeDeactivated();

				if (onDeactivated != null)
					onDeactivated.Invoke();
			}

			isActivated = false;
			isInteracting = false;
		}
	}
}
