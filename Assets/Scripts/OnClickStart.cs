using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class OnClickStart : MonoBehaviour {

	public GUISkin mySkin;  
	public GUIContent start; 
	
	// Update is called once per frame
	void OnGUI() {
		GUI.skin = mySkin; 
		
		if( GUI.Button(new Rect(Screen.width/2-100, Screen.height/2-40, 200, 80), start)) {
			SceneManager.LoadScene("GameScene");
		}
	}
}
