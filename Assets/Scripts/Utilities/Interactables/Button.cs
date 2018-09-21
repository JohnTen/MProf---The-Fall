using System;
using System.Collections.Generic;
using UnityEngine;

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

			base.StartInteracting();
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

			base.StopInteracting();
		}
	}
}
