using UnityEngine;
using System.Collections;

public class weapen_collider : MonoBehaviour {
	
	private playercontrol player_control;
	private dragonAI DragonObject;

	float attack_rate = 1f; // coresponding to the animation clip's time
	float next_attack_time = 0.0f;
	
	void Start()
	{
		player_control = GameObject.FindGameObjectsWithTag("CurPlayer")[0].GetComponent<playercontrol>();
		DragonObject = GameObject.FindGameObjectsWithTag("Dragon")[0].GetComponent<dragonAI>();
	}
	
	void OnTriggerStay(Collider collision)
	{
		if (collision.tag == "Dragon" && player_control.is_in_attack() && Time.time > next_attack_time)
		{
			next_attack_time = Time.time + attack_rate;
			DragonObject.DamageDragon();
		}
	}
}


