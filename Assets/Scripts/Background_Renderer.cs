using UnityEngine;
using System.Collections;

public class Background_Renderer : MonoBehaviour {
	public GameObject Grass;
	public GameObject GrassParent;

	void Start () {
		renderer.material.color = new Color (80f/255, 220f/255, 80f/255);

		for (int i = 0; i < 20; i ++) {
			Vector3 spawnLocation = new Vector3(Random.Range(-7f,7f), Random.Range(-4f,4f), 10);
			Grass = (GameObject) Instantiate(Grass, spawnLocation, Quaternion.identity);
			Grass.transform.parent = GrassParent.transform;
		}
	}
}
