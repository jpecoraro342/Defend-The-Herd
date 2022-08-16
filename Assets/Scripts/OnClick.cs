using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class OnClick : MonoBehaviour {

	public GUISkin mySkin;  


	// Update is called once per frame
	void OnGUI() {
		GUI.skin = mySkin; 

		if( GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-40, 200, 80), "RETRY?")) {
			SceneManager.LoadScene("GameScene");
		}
	}
}
