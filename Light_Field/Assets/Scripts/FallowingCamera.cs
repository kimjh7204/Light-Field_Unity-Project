using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class FallowingCamera : MonoBehaviour {

	public Transform mainCamera;
	//private SteamVR a;
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		transform.position = mainCamera.position;
	}
}
