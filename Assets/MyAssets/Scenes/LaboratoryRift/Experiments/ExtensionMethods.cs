using UnityEngine;

public static class ExtensionMethods {

	public static T GetOrAddComponent<T>(this GameObject o) where T : Component
	{
		T component = o.GetComponent<T>();

		if (component == null)     
			component = o.AddComponent<T>() as T;

		return component;

	}
}
