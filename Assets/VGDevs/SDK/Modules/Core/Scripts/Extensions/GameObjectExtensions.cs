using UnityEngine;

public static class GameObjectExtensions
{
	public static void SafeDestroy(this GameObject go, GameObject target)
	{
		if (target == null) return;
		target.SetActive(false);
		Object.Destroy(target, 0.001f);
	}
	
	/// <summary>
	/// Gets a component attached to the given game object.
	/// If one isn't found, a new one is attached and returned.
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	/// <returns>Previously or newly attached component.</returns>
	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
	{
		var component = gameObject.GetComponent<T>();
		return component == null ? gameObject.AddComponent<T>() : component;
	}

	/// <summary>
	/// Checks whether a game object has a component of type T attached.
	/// </summary>
	/// <param name="gameObject">Game object.</param>
	/// <returns>True when component is attached.</returns>
	public static bool HasComponent<T>(this GameObject gameObject) where T : Component
	{
		return gameObject.GetComponent<T>() != null;
	}
}