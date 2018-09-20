using UnityEngine;

namespace UnityUtility
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
	{
        protected static T _instance;
        protected static T Instance
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindObjectOfType<T>();

                if (_instance) return _instance;
                _instance = GlobalObject.GetOrAddComponent<T>();
                return _instance;
            }
        }

		protected virtual void Awake()
		{
			if (Instance != this)
			{
				Destroy(this.gameObject);
			}
		}
	}
}
