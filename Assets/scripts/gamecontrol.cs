using UnityEngine;
using System.Collections;

public class gamecontrol : MonoBehaviour {
	
	private GameObject dragon_object;
	private GameObject PlayerObject;

	private bool restart;

	public GUIText Text;
	
	void Start () {
		restart = false;

		dragon_object = GameObject.FindGameObjectsWithTag("Dragon")[0];
		PlayerObject = GameObject.FindGameObjectsWithTag("CurPlayer")[0];

		Debug.Log ("dragon num" + GameObject.FindGameObjectsWithTag("Dragon").Length.ToString());
		Debug.Log ("player num" + GameObject.FindGameObjectsWithTag("CurPlayer").Length.ToString());
	}

	void Update () 
	{
		if(restart)
		{
			Text.text = "        game over!\npress space to restart!";
			if(Input.GetKeyDown (KeyCode.Space))
			{
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	public void RestartGame()
	{
		restart = true;
	}
	
}

