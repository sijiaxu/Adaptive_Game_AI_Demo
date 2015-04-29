using UnityEngine;
using System.Collections;

public class playercontrol : MonoBehaviour {

	public float walkAcceleration = 2;
	private Animator anim;
	private float yRotation = 0; 

	public int PlayerHealth {get;set;}
	public int PeriodSufferDamge {get;set;}

	private gamecontrol gamecontroller;

	enum MovementState
	{
		Idle = 0,
		Walking = 1,
		Running = 2
	}
	MovementState currentMovementState;
	
	void Start ()
	{
		PlayerHealth = 10;
		PeriodSufferDamge = 0;
		anim = GetComponent<Animator>();
		Physics.gravity = new Vector3(0,-10,0);

		GameObject controller_object = GameObject.FindGameObjectsWithTag("GameController")[0];
		gamecontroller = controller_object.GetComponent<gamecontrol>();
	}

	public void DamagePlayer()
	{
		PlayerHealth -= 1;
		PeriodSufferDamge += 1;
		Debug.Log("damage player,health: " + PlayerHealth.ToString());
		if (PlayerHealth > 0)
		{
			anim.SetTrigger("beaten");
		}
		if(PlayerHealth == 0)
		{
			anim.SetTrigger("death");
			gamecontroller.RestartGame();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "FireBall")
		{
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

	public bool is_in_attack()
	{
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
		return stateInfo.IsName("Base Layer.Attack5");
	}
	
	void Update()
	{
		GetComponent<Rigidbody>().isKinematic = false;
		if (!Input.GetKey(KeyCode.Mouse1))
		{
			yRotation += Input.GetAxis("Mouse X") ; 
			transform.rotation = Quaternion.Euler(0, yRotation, 0);
		}

		float x_speed = Input.GetAxis ("Horizontal");
		float z_speed = Input.GetAxis ("Vertical");
		
		if (z_speed != 0 || x_speed != 0)
		{
			if (Input.GetKey(KeyCode.LeftShift))
				currentMovementState = MovementState.Running;
			else
				currentMovementState = MovementState.Walking;
		}
		else
			currentMovementState = MovementState.Idle;
		anim.SetInteger("movespeed", (int)currentMovementState);
		transform.Translate(new Vector3(x_speed * walkAcceleration * (int)currentMovementState * Time.deltaTime, 0, z_speed * walkAcceleration * (int)currentMovementState * Time.deltaTime) );
		
		if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Space))
		{
			anim.SetTrigger("attack1");
		}
		else
		{
			anim.ResetTrigger("attack1");
		}

	}
	
}
