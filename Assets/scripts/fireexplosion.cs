using UnityEngine;
using System.Collections;

public class fireexplosion : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		Destroy(gameObject, 5);
	}
}
