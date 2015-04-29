using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum Action
{
	None,
	FireBall,
	FallingBall,
	Roar,
	Chasing,
	FlyBack,
	Beaten,
}

public class State : System.IEquatable<State>
{
	public int PlayerNpcDistance {get;set;}
	public int PlayerNpcEulerAngle {get;set;}
	//private int PlayerHealth;
	//private int NpcHealth;

	public State(int distance, int EulerAngle, int PlayerHp, int NpcHp)
	{
		PlayerNpcDistance = distance;
		PlayerNpcEulerAngle = EulerAngle;
	}

	public State() {}

	public void StateAnalyze(GameObject player, GameObject npc) 
	{
		float distance = Vector3.Distance(player.transform.position, npc.transform.position);
		PlayerNpcDistance = (int)(distance >= 40 ? 40 : distance) / 20;

		Vector3 TargetDirection = player.transform.position - npc.transform.position;
		float EulerAngle = Mathf.Acos(Vector3.Dot (TargetDirection.normalized, npc.transform.forward.normalized)) * Mathf.Rad2Deg;
		
		PlayerNpcEulerAngle = (int)(EulerAngle > 180 ? EulerAngle - 180 : EulerAngle) / 90;
	}	

	public bool Equals(State other) 
	{
		var otherState = other as State;
		if (otherState == null)
			return false;
		return PlayerNpcDistance == otherState.PlayerNpcDistance && PlayerNpcEulerAngle == otherState.PlayerNpcEulerAngle;
	}

	public override int GetHashCode() 
	{
		return PlayerNpcDistance * 10 + PlayerNpcEulerAngle;
	}
}


public class QState
{
	public State StartState {get;set;}
	public Action CurrentAction {get;set;}
	public float Reward {get;set;}
	public State EndState {get;set;}

	public QState(Action CurAction)
	{
		StartState = new State();
		CurrentAction = CurAction;
		Reward = 0;
		EndState = new State();
	}

	public void UtilityAnalyze(GameObject player, GameObject npc)
	{
		float PlayerSufferDamage = player.GetComponent<playercontrol>().PeriodSufferDamge;
		float NormalizedPlayerSufferDamage = PlayerSufferDamage / (player.GetComponent<playercontrol>().PlayerHealth + PlayerSufferDamage);
		
		float DragonSufferDamage = npc.GetComponent<dragonAI>().PeriodSufferDamge;
		float NormalizedDragonSufferDamage = DragonSufferDamage / (npc.GetComponent<dragonAI>().DragonHealth + DragonSufferDamage);
		
		Reward =  NormalizedPlayerSufferDamage - NormalizedDragonSufferDamage;

		player.GetComponent<playercontrol>().PeriodSufferDamge = 0;
		npc.GetComponent<dragonAI>().PeriodSufferDamge = 0;
	}
}


class StateActionValue
{
	public Dictionary<Action, float> ActionValue;
	public Dictionary<Action, int> ActionCount;
	public Action MaxAction;
	public float MaxValue;	
	public int TotalActionCount;

	public StateActionValue()
	{
		ActionValue = new Dictionary<Action, float>();
		ActionCount = new Dictionary<Action, int>();
		MaxAction = Action.None;
		MaxValue = 0;
		TotalActionCount = 0;
		//for each state, the action it can taken is deterministic, and need to initialization to zero at first
		List<Action> StateAction = new List<Action>()
		{
			Action.FireBall,
			Action.FallingBall
		};
		foreach(Action action in StateAction)
		{
			ActionValue.Add(action, 0);
			ActionCount.Add(action, 0);
		}
	}
}


public class KnowledgeSystem 
{
	private Dictionary<State,StateActionValue> Experience;
	private float LeaningRate;
	private float DiscountRate;
	private Dictionary<Action, StateID> ActionIDToStateID;
	private Dictionary<State, bool> StateLearningSwitch;
	
	public KnowledgeSystem()
	{
		Experience = new Dictionary<State,StateActionValue>();
		StateLearningSwitch = new Dictionary<State, bool>();
		LeaningRate = 0.1f;
		DiscountRate = 0.5f;
		ActionIDToStateID = new Dictionary<Action, StateID>()
		{
			{Action.FireBall, StateID.BattleFireBall},
			{Action.Chasing, StateID.BattleRun},
			{Action.FallingBall, StateID.BattleFallingBall}
		};
	}

	public bool GetStateLearningInfo(State CurState)
	{
		if(StateLearningSwitch.ContainsKey(CurState))
		{
			return StateLearningSwitch[CurState];
		}
		else
		{
			StateLearningSwitch.Add(CurState, false);
			return false;
		}
	}

	public float GetStateActionValue(State CurState, Action CurAction)
	{
		if(Experience.ContainsKey(CurState))
		{
			return Experience[CurState].ActionValue[CurAction];
		}
		else
		{
			return 0.0f;
		}
	}

	// the UCB policy used to select next action in state
	public StateID UCBPolicy(State CurState)
	{
		Action MaxUCBAction = Action.None;
		float MaxUCBValue = 0;
		float TempUCBValue = 0;
		foreach(var item in Experience[CurState].ActionValue)
		{
			//has not been explored, visit firest 
			if(Experience[CurState].ActionCount[item.Key] == 0)
			{
				return ActionIDToStateID[item.Key];
			}
			else 
			{
				TempUCBValue = item.Value + Mathf.Sqrt(0.1f * Mathf.Log(Experience[CurState].TotalActionCount) / Experience[CurState].ActionCount[item.Key]) ;
				if(TempUCBValue > MaxUCBValue)
				{
					MaxUCBValue = TempUCBValue;
					MaxUCBAction = item.Key;
				}
			}
		}
		//permanent switch to learning strategy for this state
		StateLearningSwitch[CurState] = true;
		return ActionIDToStateID[MaxUCBAction];
	}
	
	//the key learning algorithem, current is TD(0)
	public void UpdateKnowledge(QState CurQState)
	{
		State CurState = CurQState.StartState;
		Action CurAction = CurQState.CurrentAction;
		State NextState = CurQState.EndState;
		float Reward = CurQState.Reward;

		if(Experience.ContainsKey(CurState) == false)
		{
			Experience[CurState] = new StateActionValue();
		}
		Experience[CurState].TotalActionCount += 1;
		
		if(Experience.ContainsKey(NextState))
		{
			Experience[CurState].ActionValue[CurAction] += LeaningRate * (Reward + DiscountRate * Experience[NextState].MaxValue - Experience[CurState].ActionValue[CurAction]);
		}
		else
		{
			Experience[CurState].ActionValue[CurAction] += LeaningRate * (Reward - Experience[CurState].ActionValue[CurAction]);
		}
		Experience[CurState].ActionCount[CurAction] += 1;
		
		Debug.Log("Action: " + CurQState.CurrentAction.ToString() +  "State: (" + CurState.PlayerNpcDistance.ToString() + "," + CurState.PlayerNpcEulerAngle.ToString() + ")" 
		          + " Reward: " + Reward.ToString() + " Utility: " + Experience[CurState].ActionValue[CurAction].ToString() + " ActionCount: " + Experience[CurState].ActionCount[CurAction].ToString());
		
		if(Experience[CurState].ActionValue[CurAction] > Experience[CurState].MaxValue || Experience[CurState].MaxAction == Action.None)
		{
			Experience[CurState].MaxAction = CurAction;
			Experience[CurState].MaxValue = Experience[CurState].ActionValue[CurAction];
		}
	}	
}





