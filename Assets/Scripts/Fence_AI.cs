using UnityEngine;
using System.Collections;

public class Fence_AI : MonoBehaviour {

	private Animator animator;
	
	private int fenceType;
	private int maxHealth;

	private float damage;

	void Start () {
		maxHealth = 4;
		animator = this.GetComponent<Animator> ();
		animator.SetInteger ("fenceType", 0);
		animator.SetBool ("damaged", false);
	}

	void Update () {
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals("wolf")) {
			damage += Time.deltaTime;
			Debug.Log(damage);
			if (damage >= maxHealth) {
				gameObject.SetActive(false);
			}
			else if (damage >= maxHealth/3f) {
				animator.SetBool("damaged", true);
			}
		}
	}
	
	void OnTriggerStay2D(Collider2D other) {
		if (other.tag.Equals("wolf")) {
			damage += Time.deltaTime;
			Debug.Log(damage);
			if (damage >= maxHealth) {
				gameObject.SetActive(false);
			}
			else if (damage >= maxHealth/3) {
				animator.SetBool("damaged", true);
			}
		}
	}

	public void newWave() {
		Debug.Log ("New Wave"); 
		gameObject.SetActive (true);
		damage = 0;
		animator.SetBool ("damaged", false);
		animator.SetInteger ("fenceType", fenceType);
		Debug.Log ("Reset" + fenceType); 
	}

	public void upgradeFence(int upgradeType) {
		maxHealth = maxHealth * (upgradeType+1);
		fenceType = upgradeType;
		animator.SetBool ("damaged", false);
		animator.SetInteger ("fenceType", fenceType);
	}
}
