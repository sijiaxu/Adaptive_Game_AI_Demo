using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class dragonAI : MonoBehaviour {

	public GameObject FireBall;
	public GameObject FallBall;
	public GameObject FallBallCircle;

	public int RotateSpeed = 120; //120 degree per second
	
	private FSMSystem fsm;
	private GameObject player;
	private Animator anim;

	public int DragonHealth {get;set;}
	public int PeriodSufferDamge {get;set;}
	public int RetreatHealth = 5;

	public int FireBallDistanceThread = 20;
	public int FallingBallgDistanceThread = 40;

	private gamecontrol gamecontroller;

	public KnowledgeSystem DragonKnowledge {get;set;}

	public Dictionary<StateID, Action> StateIDToActionID {get;set;}


	void Start()
	{
		anim = GetComponent<Animator>();

		StateIDToActionID = new Dictionary<StateID, Action>()
		{
			{StateID.BattleFireBall, Action.FireBall},
			{StateID.BattleFallingBall, Action.FallingBall}
		};

		DragonHealth = 20;
		PeriodSufferDamge = 0;
		player = GameObject.FindGameObjectsWithTag("CurPlayer")[0];

		GameObject controller_object = GameObject.FindGameObjectsWithTag("GameController")[0];
		gamecontroller = controller_object.GetComponent<gamecontrol>();

		DragonKnowledge = new KnowledgeSystem();
	
		IdleState idle = new IdleState(player, gameObject);
		idle.AddTransition(Transition.SawPlayer, StateID.BattleRoar);
		idle.AddTransition(Transition.IdleEnd, StateID.RandomWalk);

		RandomWalkState walk = new RandomWalkState(player, gameObject);
		walk.AddTransition(Transition.SawPlayer, StateID.BattleRoar);
		walk.AddTransition(Transition.WalkEnd, StateID.Idle);

		BattleRoarState roar = new BattleRoarState(player, gameObject);
		roar.AddTransition(Transition.Falling, StateID.BattleFallingBall);
		roar.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		roar.AddTransition(Transition.FireBall, StateID.BattleFireBall);
		roar.AddTransition(Transition.FarAway, StateID.BattleRun);

		BattleFireBallState fireball = new BattleFireBallState(player, gameObject);
		fireball.AddTransition(Transition.FireBall, StateID.BattleFireBall);
		fireball.AddTransition(Transition.FarAway, StateID.BattleRun);
		fireball.AddTransition(Transition.LowHealth, StateID.BattleRetreat);
		fireball.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		fireball.AddTransition(Transition.Falling, StateID.BattleFallingBall);

		BattleRunState run = new BattleRunState(player, gameObject);
		run.AddTransition(Transition.FireBall, StateID.BattleFireBall);
		run.AddTransition(Transition.LowHealth, StateID.BattleRetreat);
		run.AddTransition(Transition.Falling, StateID.BattleFallingBall);

		BattleRetreatState retreat = new BattleRetreatState(player, gameObject);
		retreat.AddTransition(Transition.Beaten, StateID.BattleBeaten);

		BattleBeatenState beaten = new BattleBeatenState(player, gameObject);
		beaten.AddTransition(Transition.FireBall, StateID.BattleFireBall);
		beaten.AddTransition(Transition.LowHealth, StateID.BattleRetreat);
		beaten.AddTransition(Transition.FlyBack, StateID.BattleFlyBack);

		BattleFlyBackState flyback = new BattleFlyBackState(player, gameObject);
		flyback.AddTransition(Transition.FireBall, StateID.BattleFireBall);
		flyback.AddTransition(Transition.Falling, StateID.BattleFallingBall);
		flyback.AddTransition(Transition.FarAway, StateID.BattleRun);
		flyback.AddTransition(Transition.Beaten, StateID.BattleBeaten);

		BattleFallingBall fallingball = new BattleFallingBall(player, gameObject);
		fallingball.AddTransition(Transition.FireBall, StateID.BattleFireBall);
		fallingball.AddTransition(Transition.FarAway, StateID.BattleRun);
		fallingball.AddTransition(Transition.LowHealth, StateID.BattleRetreat);
		fallingball.AddTransition(Transition.Beaten, StateID.BattleBeaten);
		fallingball.AddTransition(Transition.Falling, StateID.BattleFallingBall);


		fsm = new FSMSystem();
		fsm.AddState(idle);
		fsm.AddState(walk);
		fsm.AddState(roar);
		fsm.AddState(fireball);
		fsm.AddState(run);
		fsm.AddState(retreat);
		fsm.AddState(beaten);
		fsm.AddState(flyback);
		fsm.AddState(fallingball);
	}

	public void DamageDragon()
	{
		DragonHealth -= 1;
		PeriodSufferDamge += 1;
		if(DragonHealth > 0)
		{
			SetTransition(Transition.Beaten);
			anim.SetTrigger("beaten");
		}

		Debug.Log("damage dragon,health: " + DragonHealth.ToString());
		if(DragonHealth == 0)
		{
			anim.SetTrigger("death");
			gamecontroller.RestartGame();
		}
	}

	public void SetTransition(Transition t) 
	{
		StateID NextActionStateID = fsm.CurrentState.GetOutputState(t);
		//only attack action need to check attack AI strategy
		if(StateIDToActionID.ContainsKey(NextActionStateID))
		{
			Action NextAction = StateIDToActionID[NextActionStateID];
			float ActionValue = DragonKnowledge.GetStateActionValue(fsm.CurrentState.MDPQstate.EndState, NextAction);
			if(ActionValue < -0.006 || DragonKnowledge.GetStateLearningInfo(fsm.CurrentState.MDPQstate.EndState))
			{
				//performing UCB and find the next action
				StateID NextStateID = DragonKnowledge.UCBPolicy(fsm.CurrentState.MDPQstate.EndState);
				fsm.PerformAITransition(NextStateID);
				return;
			}
		}

		fsm.PerformTransition(t); 
	}

	public void Update()
	{
 		fsm.CurrentState.Reason(player, gameObject);
		fsm.CurrentState.Act(player, gameObject);
	}
	
}



