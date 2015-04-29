using UnityEngine;
using System.Collections;

public class fireballfall : MonoBehaviour {

	public GameObject ExplositionEffect;

	private Vector3 StartPosition; 
	private Vector3 HitPosition;
	private float TotalFallTime;
	private float TempFallTime;

	void Start()
	{
		TotalFallTime = 1.5f;
		StartPosition = transform.position;
		RaycastHit[] hits;
		hits = Physics.RaycastAll(transform.position, Vector3.down, 100.0F);
		foreach(RaycastHit hit in hits)
		{
			if(hit.collider.gameObject.tag == "ground")
			{
				HitPosition = hit.point;
				break;
			}
		}
	}
	
	void Update () 
	{
		TempFallTime += Time.deltaTime;
		transform.position = Vector3.Lerp(StartPosition, HitPosition, TempFallTime / TotalFallTime);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "CurPlayer")
		{
			ContactPoint HitPoint = collision.contacts[0];
			Instantiate(ExplositionEffect, HitPoint.point, transform.rotation);
			Destroy(gameObject);
			collision.gameObject.GetComponent<playercontrol>().DamagePlayer();
		}
		else
		{
			ContactPoint HitPoint = collision.contacts[0];
			Instantiate(ExplositionEffect, HitPoint.point, transform.rotation);
			Destroy(gameObject);
		}
	}
}
