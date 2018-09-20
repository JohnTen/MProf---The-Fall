using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility.Interactables
{
	public class Switch : MonoInteractable
	{
		public override void StartInteracting()
		{
			if (!isInteracting)
			{
				InvokeStartInteracting();
			}
			isInteracting = true;

			isActivated = !isActivated;
			if (isActivated)
			{
				InvokeActivated();

				if (onActivated != null)
					onActivated.Invoke();
			}
			else
			{
				InvokeDeactivated();

				if (onDeactivated != null)
					onDeactivated.Invoke();
			}
		}

		public override void StopInteracting()
		{
			if (isInteracting)
			{
				InvokeStopInteracting();
			}

			isInteracting = false;
		}
    }
}
