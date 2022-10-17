using UnityEngine;
using System.Collections;

public class TimedObjectDestructorv2 : MonoBehaviour {

	public float timeOut = 1.0f;
	public bool onlyDisable = false;
	public bool detachChildren = false;
	
	void Awake ()
	{
		StartCoroutine("_DestroyNow");
	}
	
	IEnumerator _DestroyNow ()
	{
		yield return new WaitForSeconds(timeOut);

		if (detachChildren) 
			transform.DetachChildren ();

        if (onlyDisable)
        {
            this.gameObject.SetActive(false);
        }
        else
        {
            #if UNITY_5 || UNITY_2017
            DestroyObject(gameObject);
            #else
            Destroy(gameObject);
            #endif
        }
	}
}
