using UnityEngine;
using System.Collections;

public class ImpactEffect : MonoBehaviour {

	private ParticleSystem ps;

	// Use this for initialization
	void Start () {
		ps = GetComponent<ParticleSystem>();
	}
	
	// RunInLogic is called once per frame
	void Update () {
	
		if (!ps.IsAlive()){
			Destroy(gameObject);
		}
	}
}
