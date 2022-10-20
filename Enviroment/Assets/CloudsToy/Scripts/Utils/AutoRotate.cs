using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

	public float speed = 50;

	void Update() {
		transform.Rotate(Vector3.up, Time.deltaTime*speed, Space.World);
	}
}