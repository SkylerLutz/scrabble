using UnityEngine;
using System.Collections;

public class MenuController : MonoBehaviour {

	public GameObject commitButton;
	public GameController del;
	public void LateUpdate() {
		if (Input.GetMouseButtonDown (0)) {

			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast (ray, out hit)) {
				if (hit.transform.tag == "commit") {
					if (del != null) {
						del.commitMove ();
					} else {
						Debug.Log ("the menucontroller delegate was null");
					}
				}
				else if (hit.transform.tag == "solve") {
					if (del != null) {
						del.solvePuzzle();
					} else {
						Debug.Log ("the menucontroller delegate was null");
					}
				}
				else if (hit.transform.tag == "undo") {
					if (del != null) {
						del.clear();
					} else {
						Debug.Log ("the menucontroller delegate was null");
					}
				}
			}
		}
	}
}
