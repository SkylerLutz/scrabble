using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public Transform view;
	public GameObject tilePrefab;

	void Update() {
		if (Input.GetMouseButtonDown (0)) {
			Debug.Log ("Pressed left click."); 
			Instantiate(tilePrefab, new Vector3(1.2f, 10, 1.2f), Quaternion.identity, view);
		}
	}
}
