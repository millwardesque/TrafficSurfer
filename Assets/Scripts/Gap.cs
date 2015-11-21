using UnityEngine;
using System.Collections;

public class Gap : MonoBehaviour {

	void OnDrawGizmos () {
		Gizmos.DrawIcon (transform.position, "Gap.psd", true);
	}
}
