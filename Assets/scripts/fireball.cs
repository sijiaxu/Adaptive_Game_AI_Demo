using UnityEngine;
using System.Collections;

public class fireball : MonoBehaviour {
	public GameObject ExplositionEffect;
	public int speed = 1;

	void Start()
	{
		Destroy(gameObject, 10);
	}

	void Update () 
	{
		transform.Translate(new Vector3(0,0,1) * Time.deltaTime * speed);
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "CurPlayer")
		{
			ContactPoint HitPoint = collision.contacts[0];
			Instantiate(ExplositionEffect, HitPoint.point, gameObject.transform.rotation);
			Destroy(gameObject);
			collision.gameObject.GetComponent<playercontrol>().DamagePlayer();
		}
		else
		{
			ContactPoint HitPoint = collision.contacts[0];
			Instantiate(ExplositionEffect, HitPoint.point, gameObject.transform.rotation);
			Destroy(gameObject);
		}
	}
}
