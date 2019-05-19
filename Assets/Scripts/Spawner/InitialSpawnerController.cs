using UnityEngine;

public class InitialSpawnerController : MonoBehaviour
{

	public GameObject[] spawnObjects;

	private void Start ()
	{
		SpawnObjects();
	}

	private void SpawnObjects()
	{
		foreach (var spawnObject in spawnObjects)
		{
			Instantiate(spawnObject);
		}
	}

}
