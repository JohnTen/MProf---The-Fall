using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallUpEnable : MonoBehaviour
{
	void Awake ()
	{
		var objs = Resources.FindObjectsOfTypeAll(typeof(EnableAtBeginning));

		for (int i = 0; i < objs.Length; i++)
		{
			var enb = objs[i] as EnableAtBeginning;
			if (!enb.gameObject.activeSelf && enb.awake)
			{
				enb.gameObject.SetActive(true);
			}

			Destroy(enb);
		}
	}
}
