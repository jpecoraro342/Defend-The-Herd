using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Goat_AI : MonoBehaviour {

	public bool dead;
	private Animator animator;
	private float time = 2;

	private float maxhealth = 5; 
	private float damage; 
	private Vector3 movePosition;
	private Vector3 previousPosition;
	private Vector3 beforeCollision;

	private int killCounter; 

	private GameController mainLoop;

	// Use this for initialization
	void Start () {
		killCounter = 0;
		mainLoop = (GameController) GameObject.FindGameObjectWithTag ("scripts").GetComponent(typeof(GameController));
		animator = this.GetComponent<Animator> ();
		animator.SetInteger ("Behavior", 0);	// bounce
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		if (killCounter >= 3) {
			SceneManager.LoadScene("GG");
			//StartCoroutine("GG");
		}
		
		if (dead) {
			animator.SetInteger ("Behavior", 1);	// die
			(gameObject.GetComponent ("Halo") as Behaviour).enabled = true;
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, 15, transform.position.z), Time.deltaTime * 2);

		} else if (time >= 0 && time < 7) {
			animator.SetInteger ("Behavior", 0);	// bounce
		} else if (time >= 7 && time < 10.5) {
			animator.SetInteger ("Behavior", 2);	// eat
		} else if (time >= 10.5 && time < 15) {
			animator.SetInteger ("Behavior", 3);	// open mouth
		} else {
			time = 0;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag.Equals("wolf")) {
			damage += Time.deltaTime;
			StartCoroutine("dealDamageFromWolf", other);
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (other == null || other.tag.Equals("wolf")) {
			StopCoroutine("dealDamageFromWolf");
		}
	}
	
	IEnumerator dealDamageFromWolf(Collider2D other) {
		while (other != null) {
			damage += Time.deltaTime;
			if (damage >= maxhealth) {
				if (!dead) 
					StartCoroutine(killGoat ());
				yield break;
			}
			yield return null;
		}
	}

	IEnumerator killGoat() {
		StopCoroutine ("dealDamageFromWolf");
		mainLoop.incrementKillCounter();
		dead = true; 
		yield return new WaitForSeconds (3f);
		Destroy (gameObject);
	}
}
