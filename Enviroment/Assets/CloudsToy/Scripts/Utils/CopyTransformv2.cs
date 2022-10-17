using UnityEngine;
using System.Collections;

public class CopyTransformv2 : MonoBehaviour {

	public Transform from;
	private Transform to;
	public Vector3 offset = Vector3.zero;

	private Vector3 Pos;
	//private Quaternion Rot;


	void Start(){
		if(from == null) from = GameObject.FindGameObjectWithTag("Player").transform;
		to = this.transform;
	}

	void LateUpdate () {
		if(from == null) from = GameObject.FindGameObjectWithTag("Player").transform;
		if(from == null) return;

		Pos = from.position + offset;
		if(Pos != to.position)
			to.position = Pos;
	}

}
