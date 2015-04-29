using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BattleRoarState : FSMState
{
	private GameObject CurPlayer;
	private GameObject CurNpc;

	private Quaternion LookRotation;
	private Quaternion StartRotation;

	private float time_t;
	private Animator anim;

	private bool notice;

	public BattleRoarState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.BattleRoar;
		CurPlayer = player;
		CurNpc = npc;
		time_t = 0;
		anim = npc.GetComponent<Animator>();
		MDPQstate = new QState(Action.Roar);
	}

	public override void DoBeforeEntering()
	{
		notice = true;
		Vector3 RoarDirection = CurPlayer.transform.position - CurNpc.transform.position;
		LookRotation = Quaternion.LookRotation(RoarDirection);
		LookRotation.eulerAngles = new Vector3(0,LookRotation.eulerAngles.y,0);
		StartRotation.eulerAngles = new Vector3(0,CurNpc.transform.eulerAngles.y,0);
	}
	
	public override void Reason(GameObject player, GameObject npc)
	{
		float dist = Vector3.Distance(player.transform.position, npc.transform.position);
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.fly") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0)
		{
			if(dist < npc.GetComponent<dragonAI>().FireBallDistanceThread)
			{
				MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
				npc.GetComponent<dragonAI>().SetTransition(Transition.FireBall);
				return;
			}
			
			if(dist > npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.FarAway);
				return;
			}
			
			if(dist > npc.GetComponent<dragonAI>().FireBallDistanceThread && dist < npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
			{
				MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
				npc.GetComponent<dragonAI>().SetTransition(Transition.Falling);
				return;
			}
		}
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		float angle = Quaternion.Angle(StartRotation, LookRotation);
		float RotateTime = angle / npc.GetComponent<dragonAI>().RotateSpeed;
		if(angle > 0)
		{
			//rotation finish in one half second
			npc.transform.rotation = Quaternion.Slerp(StartRotation, LookRotation, time_t/RotateTime);
			time_t += Time.deltaTime;
		}
		//check rotation complete
		if(time_t >= RotateTime && notice)
		{
			anim.SetTrigger("notice");
			notice = false;
			//Debug.Log (anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
		return;
	}
} 


public class BattleFireBallState : FSMState
{
	private GameObject CurPlayer;
	private GameObject CurNpc;
	
	private Quaternion LookRotation;
	private Quaternion StartRotation;
	
	private float time_t;
	private Animator anim;

	private bool Fire;
	private bool FireAnimation;

	private float NextFireTime;
	private float FireRate;
	
	public BattleFireBallState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.BattleFireBall;
		CurPlayer = player;
		CurNpc = npc;
		time_t = 0;
		anim = npc.GetComponent<Animator>();
		NextFireTime = 0;
		FireRate = 1;
		MDPQstate = new QState(Action.FireBall);
	}

	public override void DoBeforeEntering()
	{
		MDPQstate = new QState(Action.FireBall);
		time_t = 0;
		Fire = true;
		FireAnimation = true;
		Vector3 FireDirection = CurPlayer.transform.position - CurNpc.transform.position;
		LookRotation = Quaternion.LookRotation(FireDirection);
		LookRotation.eulerAngles = new Vector3(0,LookRotation.eulerAngles.y,0);
		StartRotation.eulerAngles = new Vector3(0,CurNpc.transform.eulerAngles.y,0);

		MDPQstate.StartState.StateAnalyze(CurPlayer, CurNpc);
	}
	 
	public override void Reason(GameObject player, GameObject npc)
	{
		float dist = Vector3.Distance(player.transform.position, npc.transform.position);
		int CurHealth = npc.GetComponent<dragonAI>().DragonHealth;

		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.breath fire") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0 && Fire == false)
		{
			if(dist < npc.GetComponent<dragonAI>().FireBallDistanceThread)
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.FireBall);
				return;
			}
			
			if(dist > npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.FarAway);
				return;
			}

			if(dist > npc.GetComponent<dragonAI>().FireBallDistanceThread && dist < npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.Falling);
				return;
			}
		}

		if(CurHealth < npc.GetComponent<dragonAI>().RetreatHealth)
		{
			npc.GetComponent<dragonAI>().SetTransition(Transition.LowHealth);
			return;
		}
	}
	
	private IEnumerator GetActionResult()
	{
		yield return new WaitForSeconds(1);
		MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
		MDPQstate.UtilityAnalyze(CurPlayer, CurNpc);
		CurNpc.GetComponent<dragonAI>().DragonKnowledge.UpdateKnowledge(MDPQstate);
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		if(Fire && Time.time > NextFireTime)
		{
			float angle = Quaternion.Angle(StartRotation, LookRotation);
			float RotateTime = angle / npc.GetComponent<dragonAI>().RotateSpeed;
			if(angle > 0)
			{
				//rotation finish in one second
				npc.transform.rotation = Quaternion.Slerp(StartRotation, LookRotation, time_t/RotateTime);
				time_t += Time.deltaTime;
			}
			//check rotation complete
			if(time_t >= RotateTime && FireAnimation)
			{
				anim.SetTrigger("fireball");
				FireAnimation = false;
			}
			
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.breath fire") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0)
			{
				Debug.Log("Fire");
				Vector3 SpawnLocation =  npc.transform.TransformPoint(0,40,165);
				Object.Instantiate(npc.GetComponent<dragonAI>().FireBall, SpawnLocation, npc.transform.rotation);
				Fire = false;
				NextFireTime = Time.time + FireRate;
				npc.GetComponent<dragonAI>().StartCoroutine(GetActionResult());
			}
		}
	}
} 



public class BattleRunState : FSMState
{
	private GameObject CurPlayer;
	private GameObject CurNpc;

	private Animator anim;
	
	public BattleRunState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.BattleRun;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		MDPQstate = new QState(Action.Chasing);
	}

	public override void DoBeforeLeaving() 
	{
		anim.SetBool("run", false);
	} 

	public override void Reason(GameObject player, GameObject npc)
	{
		float dist = Vector3.Distance(player.transform.position, npc.transform.position);
		
		if(dist < npc.GetComponent<dragonAI>().FireBallDistanceThread)
		{
			MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
			npc.GetComponent<dragonAI>().SetTransition(Transition.FireBall);
			return;
		}

		if(dist > npc.GetComponent<dragonAI>().FireBallDistanceThread && dist < npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
		{
			MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
			npc.GetComponent<dragonAI>().SetTransition(Transition.Falling);
			return;
		}
		
		int CurHealth = npc.GetComponent<dragonAI>().DragonHealth;

		if(CurHealth < npc.GetComponent<dragonAI>().RetreatHealth)
		{
			npc.GetComponent<dragonAI>().SetTransition(Transition.LowHealth);
			return;
		}

	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		Vector3 direction = player.transform.position - npc.transform.position;

		Quaternion LookRotation = Quaternion.LookRotation(direction);
		npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, LookRotation, Time.deltaTime * 5);

		npc.transform.Translate(Vector3.forward * Time.deltaTime * 5);

		anim.SetBool("run", true);
	}
}
 


public class BattleRetreatState : FSMState
{
	private GameObject CurPlayer;
	private GameObject CurNpc;
	
	private Animator anim;
	private Quaternion LookRotation;
	
	public BattleRetreatState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.BattleRetreat;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		LookRotation.eulerAngles = new Vector3(0,0,0);
	}

	public override void DoBeforeEntering()
	{
		//just set the retreat direction at the first time
		if(LookRotation.eulerAngles.y == 0)
		{
			Vector3 RetreatDirection = CurNpc.transform.position - CurPlayer.transform.position;
			LookRotation = Quaternion.LookRotation(RetreatDirection);
			LookRotation.eulerAngles = new Vector3(0,LookRotation.eulerAngles.y,0);
		}
	}
	
	public override void Reason(GameObject player, GameObject npc)
	{
		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.hit1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8)
		{
			npc.GetComponent<dragonAI>().SetTransition(Transition.Beaten);
			return;
		}
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		if(npc.GetComponent<dragonAI>().DragonHealth > 0)
		{
			npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, LookRotation, 2 * Time.deltaTime);
			if(Quaternion.Angle(npc.transform.rotation, LookRotation) < 10)
			{
				npc.transform.Translate(Vector3.forward * Time.deltaTime * 3);
				anim.SetBool("lowhealth", true);
			}
		}
	}
}



public class BattleBeatenState : FSMState
{
	private GameObject CurPlayer;
	private GameObject CurNpc;
	
	private Animator anim;
	
	public BattleBeatenState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.BattleBeaten;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		MDPQstate =  new QState(Action.Beaten);
	}

	public override void Reason(GameObject player, GameObject npc)
	{
		int CurHealth = npc.GetComponent<dragonAI>().DragonHealth;
		int ThreadHealth = npc.GetComponent<dragonAI>().RetreatHealth;

		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.hit1") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8)
		{
			// have the highest priority 
			if((CurHealth % 2 == 0) && (CurHealth >= ThreadHealth))
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.FlyBack);
				anim.SetTrigger("flyback");
				return;
			}

			if(CurHealth >= ThreadHealth)
			{
				MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
				anim.SetTrigger("beatenrecover");
				npc.GetComponent<dragonAI>().SetTransition(Transition.FireBall);
				return;
			}
			
			if(CurHealth < ThreadHealth)
			{
				anim.SetTrigger("retreatebeatenrecover");
				npc.GetComponent<dragonAI>().SetTransition(Transition.LowHealth);
				return;
			}
			

		}
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		return;
	}
}


public class BattleFlyBackState : FSMState
{
	private GameObject CurPlayer;
	private GameObject CurNpc;
	
	private Animator anim;
	
	private Vector3 FlyDirection;

	private float FlyTime;
	private float FlyTotalTime;
	private bool CompleteFlag;
	
	public BattleFlyBackState(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.BattleFlyBack;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		FlyDirection = new Vector3(0,0,0);
		FlyTime = 0;
		FlyTotalTime = 1.5f;
		CompleteFlag = false;
		MDPQstate = new QState(Action.FlyBack);
	}
	
	public override void DoBeforeEntering()
	{
		FlyDirection = (CurNpc.transform.position - CurPlayer.transform.position).normalized;

		FlyTime = 0;
		CompleteFlag = false;
	}

	public override void Reason(GameObject player, GameObject npc)
	{
		if(CompleteFlag)
		{
			float dist = Vector3.Distance(player.transform.position, npc.transform.position);

			if(dist < npc.GetComponent<dragonAI>().FireBallDistanceThread)
			{
				MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
				npc.GetComponent<dragonAI>().SetTransition(Transition.FireBall);
				return;
			}
			
			if(dist > npc.GetComponent<dragonAI>().FireBallDistanceThread && dist < npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
			{
				MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
				npc.GetComponent<dragonAI>().SetTransition(Transition.Falling);
				return;
			}
			
			if(dist > npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.FarAway);
				return;
			}
		}
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		//npc.GetComponent<Rigidbody>().MovePosition(npc.transform.position + FlyDirection * Time.deltaTime * 10);
		npc.transform.Translate(Vector3.back * Time.deltaTime * 10);
		FlyTime += Time.deltaTime;
		if(FlyTime >= FlyTotalTime)
		{
			anim.SetTrigger("flybackrecover");
			CompleteFlag = true;
		}
	}
}


public class BattleFallingBall : FSMState
{
	private GameObject CurPlayer;
	private GameObject CurNpc;
	
	private Quaternion LookRotation;
	private Quaternion StartRotation;
	
	private float time_t;
	private Animator anim;
	
	private bool Fire;
	private bool FireAnimation;
	
	private float NextFireTime;
	private float FireRate;

	public BattleFallingBall(GameObject player, GameObject npc) 
	{ 
		stateID = StateID.BattleFallingBall;
		CurPlayer = player;
		CurNpc = npc;
		anim = npc.GetComponent<Animator>();
		FireRate = 1;
		NextFireTime = 0;
		MDPQstate = new QState(Action.FallingBall);
	}
	
	public override void DoBeforeEntering()
	{
		MDPQstate = new QState(Action.FallingBall);
		time_t = 0;
		Fire = true;
		FireAnimation = true;
		LookRotation = Quaternion.LookRotation(CurPlayer.transform.position - CurNpc.transform.position);
		LookRotation.eulerAngles = new Vector3(0,LookRotation.eulerAngles.y,0);
		StartRotation.eulerAngles = new Vector3(0,CurNpc.transform.eulerAngles.y,0);
		
		MDPQstate.StartState.StateAnalyze(CurPlayer, CurNpc);
	}
	
	public override void Reason(GameObject player, GameObject npc)
	{
		float dist = Vector3.Distance(player.transform.position, npc.transform.position);
		int CurHealth = npc.GetComponent<dragonAI>().DragonHealth;

		if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.attack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0 && Fire == false)
		{
			if(dist < npc.GetComponent<dragonAI>().FireBallDistanceThread)
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.FireBall);
				return;
			}
			
			if(dist > npc.GetComponent<dragonAI>().FireBallDistanceThread && dist < npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.Falling);
				return;
			}
			
			if(dist > npc.GetComponent<dragonAI>().FallingBallgDistanceThread)
			{
				npc.GetComponent<dragonAI>().SetTransition(Transition.FarAway);
				return;
			}
		}
		
		if(CurHealth < npc.GetComponent<dragonAI>().RetreatHealth)
		{
			npc.GetComponent<dragonAI>().SetTransition(Transition.LowHealth);
			return;
		}
	}

	private IEnumerator GetActionResult()
	{
		yield return new WaitForSeconds(1.5f);
		MDPQstate.EndState.StateAnalyze(CurPlayer, CurNpc);
		MDPQstate.UtilityAnalyze(CurPlayer, CurNpc);
		CurNpc.GetComponent<dragonAI>().DragonKnowledge.UpdateKnowledge(MDPQstate);
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
		if(Fire && Time.time > NextFireTime)
		{
			float angle = Quaternion.Angle(StartRotation, LookRotation);
			float RotateTime = angle / npc.GetComponent<dragonAI>().RotateSpeed;
			if(angle > 0)
			{
				//rotation finish in one second
				npc.transform.rotation = Quaternion.Slerp(StartRotation, LookRotation, time_t/RotateTime);
				time_t += Time.deltaTime;
			}
			//check rotation complete
			if(time_t >= RotateTime && FireAnimation)
			{
				anim.SetTrigger("fallball");
				FireAnimation = false;
			}
			
			if(anim.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.attack2") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0)
			{
				Debug.Log("FallBall");
				Object.Instantiate(npc.GetComponent<dragonAI>().FallBallCircle, player.transform.position, npc.transform.rotation);
				Object.Instantiate(npc.GetComponent<dragonAI>().FallBall, player.transform.position + Vector3.up * 30, npc.transform.rotation);
				Fire = false;
				NextFireTime = Time.time + FireRate;
				npc.GetComponent<dragonAI>().StartCoroutine(GetActionResult());
			}
		}
	}
}















