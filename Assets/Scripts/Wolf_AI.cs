using UnityEngine;
using System.Collections;

public class Wolf_AI : MonoBehaviour {
	public float speed = 5f;
	private bool beginningMovement;
	private bool goatAttackMode;

	private int numFencesAttacking;
	private int goatAttackingIndex;
	private int prevAttackingIndex; 
	private float maxHealth; 
	private float theDamage; 

	private Animator animator;

	private GameObject goat;
	private GameObject secondAttackingFence;	
	
	GameObject[] goats;

	private enum Lifecycle{Alive, Dying, Dead, Decomposing};
	private Lifecycle lifeCycle; 

	void Start() {
		lifeCycle = Lifecycle.Alive; 
		goatAttackMode = false;
		animator = this.GetComponent<Animator>();
		beginningMovement = true;
		maxHealth = 0.5f;
		prevAttackingIndex = -1; 

		// 1 out of 5 wolves will have 200% speed
		if( Random.Range(0, 100) <= 20 ) 
			speed *= 2.0f; 
	}

	// The null exception bug is probably linked to this block of code, and destroyWolf()
	void Update () {

		if (beginningMovement) {
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (-15, transform.position.y, 0), Time.deltaTime * speed);
			if (transform.position.x < -2.0) {
				beginningMovement = false;
				goatAttackMode = true;
				findGoat();
			}
		}
		else {
			if (goatAttackMode) {
				if (goat == null) {
					if (!findGoat()) {
						return;
					}
				}
				transform.position = Vector3.MoveTowards (transform.position, goat.transform.position, Time.deltaTime * speed);
			}
			if(goat != null) {
				Goat_AI aGoat = (Goat_AI)goat.GetComponent (typeof(Goat_AI));
				if (aGoat.aGoatJustDied ()) {
					goat = null; 
					goatAttackMode=true;			
				}
			}
		}
	}

	
		
	public void changeState() {	
		if (lifeCycle == Lifecycle.Alive) 
			lifeCycle = Lifecycle.Dying;
		else if (lifeCycle == Lifecycle.Dying) 
			lifeCycle = Lifecycle.Dead; 
		else if (lifeCycle == Lifecycle.Dead) 
			lifeCycle = Lifecycle.Decomposing; 
	}

	public bool getIsDead() {
		if (lifeCycle == Lifecycle.Dead) {
			return true;
		}
		else
			return false;
	}

	public bool getIsDying() {
		if (lifeCycle == Lifecycle.Dying) {
			return true;
		}
		else
			return false;
	}

	bool findGoat() { 

		goats = GameObject.FindGameObjectsWithTag ("goat");
		if (goats.Length == 0)
			return false;

		// fixed up the wolf attack AI
		goatAttackingIndex = Random.Range(0, goats.Length);

		if (Mathf.Abs (goats [goatAttackingIndex].transform.position.y - transform.position.y) > 1.5) {
			goatAttackingIndex = (goatAttackingIndex + 1) % goats.Length;
			if (Mathf.Abs (goats [goatAttackingIndex].transform.position.y - transform.position.y) > 1.5) {
				goatAttackingIndex = (goatAttackingIndex + 1) % goats.Length;
			}
		}
		 
		if(prevAttackingIndex == goatAttackingIndex) {
			goatAttackingIndex = (goatAttackingIndex + 1) % goats.Length;
		}

		goat = goats[goatAttackingIndex];
		prevAttackingIndex = goatAttackingIndex; 
		return true;
	}

	// reordered goat and fence, so priority is set to attacking a goat. 
	void OnTriggerEnter2D(Collider2D other) {
		string tag = other.tag;
		if (tag.Equals("goat")) {
			goatAttackMode = false;
			beginningMovement = false;
			animator.SetBool("attacking", true);
		}
		else if (tag.Equals("fence")) {
			numFencesAttacking++;
			if (numFencesAttacking == 1)
				StartCoroutine(attackingFence(other.gameObject));
			else {
				secondAttackingFence = other.gameObject;
			}
		}
	}

	IEnumerator attackingFence(GameObject fence) {
		Fence_AI fenceDamager = (Fence_AI)fence.GetComponent (typeof(Fence_AI));
		goatAttackMode = false;
		beginningMovement = false;
		animator.SetBool("attacking", true);
		while (fence.activeSelf) {
			fenceDamager.dealDamage(Time.deltaTime);
			yield return null;
		}
		numFencesAttacking--;
		if (numFencesAttacking == 1) {
			StartCoroutine(attackingFence(secondAttackingFence));
		}
		else {
			animator.SetBool ("attacking", false);
			if (transform.position.x < -2.0) {
				goatAttackMode = true;
				findGoat();
			}
			else {
				beginningMovement = true;
			}
		}
	}

	public void killWolf() {
		beginningMovement = false;
		goatAttackMode = false; 
		animator.SetBool ("dead", true);
	}

	public void dealDamage(float damageToDeal) {
		if( !getIsDead () ) {
			theDamage += damageToDeal;
			if (theDamage >= maxHealth) {
				changeState ();  
			}
		}
	}
	
}
