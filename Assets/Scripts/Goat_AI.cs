using UnityEngine;
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

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator> ();
		animator.SetInteger ("Behavior", 0);	// bounce
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		
		if (dead) {
			killCounter++; 
			Debug.Log ("Kill counter increased"); 
			if (killCounter == 3) {
				StopAllCoroutines (); 
				Application.LoadLevel ("GG"); 
			}
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
		if(other.gameObject.tag == "wolf") {
			damage+=Time.deltaTime; 
		}
		Debug.Log (damage);
		if(damage >= maxhealth) {	
			StartCoroutine(killGoat ()); 
			
		}
	}

	IEnumerator killGoat() {
		Debug.Log ("corouting");
		dead = true; 
		yield return new WaitForSeconds (3f);
		Debug.Log ("waited");

	}


	void OnTriggerStay2D(Collider2D other) {
		//if game tag is wolf
		if(other.gameObject.tag == "wolf") {
			damage+=Time.deltaTime; 
		}
		Debug.Log (damage);
		if(damage >= maxhealth) {
			StartCoroutine(killGoat ()); 
		}
	}

	}
