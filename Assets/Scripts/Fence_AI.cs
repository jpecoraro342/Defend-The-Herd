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
			if (damage >= maxHealth) {
				gameObject.SetActive(false);
			}
			else if (damage >= maxHealth/3f) {
				animator.SetBool("damaged", true);
			}
			StartCoroutine("dealDamageFromWolf", other);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag.Equals("wolf")) {
			StopCoroutine("dealDamageFromWolf");
		}
	}

	IEnumerator dealDamageFromWolf(Collider2D wolf) {
		while (wolf != null) {
			damage += Time.deltaTime;
			if (damage >= maxHealth) {
				gameObject.SetActive(false);
			}
			else if (damage >= maxHealth/3) {
				animator.SetBool("damaged", true);
			}
			yield return null;
		}
	}

	public void newWave() {
		gameObject.SetActive (true);
		damage = 0;
		animator.SetBool ("damaged", false);
		animator.SetInteger ("fenceType", fenceType); 
	}

	public void upgradeFence(int upgradeType) {
		maxHealth = maxHealth * (upgradeType+1);
		fenceType = upgradeType;
		animator.SetBool ("damaged", false);
		animator.SetInteger ("fenceType", fenceType);
	}
}
