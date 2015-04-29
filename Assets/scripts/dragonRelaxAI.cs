using UnityEngine;
using System.Collections;


public class IdleState : FSMState
{
	private Animator anim;
	private GameObject CurPlayer;
	private GameObject CurNpc;

	public IdleState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.Idle;
		CurPlayer = player;
		CurNpc = npc;
		anim = CurNpc.GetComponent<Animator>();
	}

	public override void Reason(GameObject player, GameObject npc)
	{
		Animator anim = npc.GetComponent<Animator>();
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		
		float dist = Vector3.Distance(player.transform.position, npc.transform.position);
		if (dist < npc.GetComponent<dragonAI>().FallingBallgDistanceThread && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.idle"))
		{
			npc.GetComponent<dragonAI>().SetTransition(Transition.SawPlayer);
			return;
		}
		
		if (anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.idle") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9)
		{
			npc.GetComponent<dragonAI>().SetTransition(Transition.IdleEnd);
			return;
		}
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		anim.SetInteger("speed", 0);
		return;
	}
} 



public class RandomWalkState : FSMState
{
	private float WalkTime;

	private GameObject CurPlayer;
	private GameObject CurNpc;

	Vector3 WalkDirection;
	private Quaternion LookRotation;
	private Quaternion StartRotation;

	private float time_t;
	private Animator anim;
	
	public RandomWalkState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.RandomWalk;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
	}

	public override void DoBeforeEntering()
	{
		time_t = 0;
		WalkDirection = new Vector3(Random.Range(-10,10),0,Random.Range(-10, 10));
		WalkTime = Random.Range(5,10);
		LookRotation = Quaternion.LookRotation(WalkDirection);
		LookRotation.eulerAngles = new Vector3(0,LookRotation.eulerAngles.y,0);
		StartRotation.eulerAngles = new Vector3(0,CurNpc.transform.eulerAngles.y,0);
	}

	public override void Reason(GameObject player, GameObject npc)
	{
		float dist = Vector3.Distance(player.transform.position, npc.transform.position);
		if (dist < npc.GetComponent<dragonAI>().FallingBallgDistanceThread && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.walk"))
		{
			npc.GetComponent<dragonAI>().SetTransition(Transition.SawPlayer);
			return;
		}
		if (WalkTime < 0 && anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.walk"))
		{	
			npc.GetComponent<dragonAI>().SetTransition(Transition.WalkEnd);
			return;
		}
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		float angle = Quaternion.Angle(StartRotation, LookRotation);
		float RotateTime = angle / npc.GetComponent<dragonAI>().RotateSpeed;

		if(angle > 0)
		{
			//rotation finish in one second
			npc.transform.rotation = Quaternion.Slerp(StartRotation, LookRotation, time_t/RotateTime);
			time_t += Time.deltaTime;
		}

		if(time_t >= RotateTime)
		{
			npc.transform.Translate(Vector3.forward * Time.deltaTime * 2);
			//npc.GetComponent<Rigidbody>().velocity  = WalkDirection.normalized * 3;
			anim.SetInteger("speed", 1);
			WalkTime -= Time.deltaTime;
		}

	}
} 

