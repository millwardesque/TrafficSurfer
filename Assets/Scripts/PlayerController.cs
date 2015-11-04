using UnityEngine;
using System.Collections;

public enum PlayerState {
	OnGround,
	OnCar,
	Jumping,
    Dead
};

public class PlayerController : MonoBehaviour {
	public static float minDeathSpeed = 1f;
	public float walkSpeed = 1f;
	public float jumpDistance = 0.3f;
	public float fromCarJumpDuration = 0.2f;
	public float fromGroundJumpDuration = 0.5f;
	public float jumpCooldown = 0.2f;
	public AudioClip walkSound;
	public AudioClip jumpSound;
	public AudioClip landOnCarSound;
    public float deathDuration = 2f;
    float currentDeathDuration = 0f;

	public CarController m_targetCar = null;
	public CarController TargetCar {
		get { return m_targetCar; }
		set { m_targetCar = value; }
	}

	AudioSource audioSource;
	Animator animator;

	bool wasMoving = false;
	float jumpCooldownRemaining = 0f;
	float activeJumpDuration = 0f;
	float jumpRemaining = 0f;
	float distanceToJump = 0f;
	CarController lastCar = null;

	PlayerState m_state = PlayerState.OnGround;
	public PlayerState State {
		get { return m_state; }
		set {
			PlayerState oldState = m_state;
			m_state = value;

            if (oldState == PlayerState.Dead) {
                GetComponent<CircleCollider2D>().enabled = true;
				GetComponent<SpriteRenderer>().sortingLayerName = "Player";
                GetComponent<SpriteRenderer>().sortingOrder = 0;
            }

			if (m_state == PlayerState.Jumping) {
				TriggerJumpAnimation();
				GetComponent<SpriteRenderer>().color = new Color(0f, 0f, 1f);

				if (oldState == PlayerState.OnGround) {
					audioSource.Stop();
					activeJumpDuration = fromGroundJumpDuration;
					jumpRemaining = activeJumpDuration;

					distanceToJump = wasMoving ? jumpDistance : 0f;
				}
				else if (oldState == PlayerState.OnCar) {
					activeJumpDuration = fromCarJumpDuration;
					jumpRemaining = activeJumpDuration;
					distanceToJump = jumpDistance;

					lastCar = transform.parent.GetComponent<CarController>();
					transform.SetParent(null, true);
				}

				audioSource.PlayOneShot(jumpSound);
				wasMoving = false;
			}
			else if (m_state == PlayerState.OnGround) {
				TriggerIdleAnimation();
				GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f);
				lastCar = null;

				if (oldState == PlayerState.Jumping) {
					jumpRemaining = 0f;
					distanceToJump = 0f;
					jumpCooldownRemaining = jumpCooldown;
				}

				wasMoving = false;
			}
			else if (m_state == PlayerState.OnCar) {
				TriggerIdleAnimation();
				GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f);
				lastCar = null;

				if (oldState == PlayerState.Jumping) {
					jumpRemaining = 0f;
					distanceToJump = 0f;
					jumpCooldownRemaining = jumpCooldown;

					ScoreManager.Instance.Score += 1;
				}

				audioSource.PlayOneShot(landOnCarSound);

				wasMoving = false;
			}
            else if (m_state == PlayerState.Dead)
            {
                audioSource.Stop();
                TriggerDeadAnimation();
                currentDeathDuration = deathDuration;
                GetComponent<CircleCollider2D>().enabled = false;

                transform.Rotate(new Vector3(0f, 0f, 180f + Random.Range(-22.5f, 22.5f)));
                GetComponent<SpriteRenderer>().sortingLayerName = "Roads";
                GetComponent<SpriteRenderer>().sortingOrder = 1;
            }
		}
	}

	void Awake() {
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = walkSound;

		animator = GetComponent<Animator>();
	}

	// Use this for initialization
	void Start () {
		State = PlayerState.OnGround;
	}
	
	// Update is called once per frame
	void Update () {
		if (State == PlayerState.OnGround) {
			OnUpdateOnGround ();
		}
		else if (State == PlayerState.Jumping) {
			OnUpdateJumping ();
		}
		else if (State == PlayerState.OnCar) {
			OnUpdateOnCar ();
		}
        else if (State == PlayerState.Dead) {
            OnUpdateDead();
        }
	}

	void OnUpdateOnGround() {
		bool isMoving = false;
		
		if (jumpCooldownRemaining > 0f) {
			jumpCooldownRemaining -= Time.deltaTime;
		}
		
		Quaternion rotation = transform.rotation;
		Vector2 offset = Vector2.zero;
		float rotationAngle = rotation.eulerAngles.z;
		bool movedLeftOrRight = false;
		
		if (Input.GetKey(KeyCode.LeftArrow)) {
			offset += new Vector2(-walkSpeed * Time.deltaTime, 0f);
			rotationAngle = 90f;
			movedLeftOrRight = true;
		}
		else if (Input.GetKey(KeyCode.RightArrow)) {
			offset += new Vector2(walkSpeed * Time.deltaTime, 0f);
			rotationAngle = -90f;
			movedLeftOrRight = true;
		}
		
		if (Input.GetKey(KeyCode.UpArrow)) {
			offset += new Vector2(0f, walkSpeed * Time.deltaTime);
			
			if (movedLeftOrRight) {
				rotationAngle = rotationAngle / 2f;
			}
			else {
				rotationAngle = 0f;
			}
		}
		else if (Input.GetKey(KeyCode.DownArrow)) {
			offset += new Vector2(0f, -walkSpeed * Time.deltaTime);
			
			if (movedLeftOrRight) {
				rotationAngle = 180f - rotationAngle / 2f;
			}
			else {
				rotationAngle = 180f;
			}
		}

		if (offset.magnitude > 0.00001f) {
			isMoving = true;
		}
		
		if (isMoving) {
			if (!audioSource.isPlaying) {
				audioSource.Play();
			}
		}
		else {
			if (audioSource.isPlaying) {
				audioSource.Stop();
			}
		}	
		
		rotation.eulerAngles = new Vector3(0, 0, rotationAngle);
		transform.rotation = rotation;
		if (Input.GetKeyDown(KeyCode.Space) && jumpCooldownRemaining <= 0f) {
			State = PlayerState.Jumping;
		}
		else {
			transform.position += (Vector3)offset;
			if (isMoving && !wasMoving) {
				TriggerWalkingAnimation();
			}
			else if (!isMoving && wasMoving) {
				TriggerIdleAnimation();
			}
		}

		wasMoving = isMoving;
	}
	
	void OnUpdateJumping() {
		Vector2 positionChange = transform.up * Time.deltaTime * distanceToJump / activeJumpDuration;
		transform.position += (Vector3)positionChange;
		
		jumpRemaining -= Time.deltaTime;
		if (jumpRemaining <= 0f) {
			State = PlayerState.OnGround;
		}
	}
	
	void OnUpdateOnCar() {
		Quaternion rotation = transform.rotation;
		if (Input.GetKey(KeyCode.LeftArrow)) {
			rotation.eulerAngles = new Vector3(0, 0, 90.0f);
		}
		else if (Input.GetKey(KeyCode.RightArrow)) {
			rotation.eulerAngles = new Vector3(0, 0, -90.0f);
		}
		else if (Input.GetKey(KeyCode.UpArrow)) {
			rotation.eulerAngles = new Vector3(0, 0, 0.0f);
		}
		else if (Input.GetKey(KeyCode.DownArrow)) {
			rotation.eulerAngles = new Vector3(0, 0, 180.0f);
		}
		transform.rotation = rotation;
		
		if (jumpCooldownRemaining > 0f) {
			jumpCooldownRemaining -= Time.deltaTime;
		}
		if (Input.GetKey (KeyCode.Space) && jumpCooldownRemaining <= 0f) {
			State = PlayerState.Jumping;
		}
	}

    void OnUpdateDead()
    {
        currentDeathDuration -= Time.deltaTime;
 
        if (currentDeathDuration > deathDuration * 2f / 4f)
        {
            Vector2 positionChange = transform.up * Time.deltaTime * walkSpeed * 2f;
            transform.position += (Vector3)positionChange;
        }

		GameManager.Instance.ChangeMusicPitch(currentDeathDuration / deathDuration);

        if (currentDeathDuration <= 0f)
        {
            GameManager.Instance.GameOver();
        }
    }

	void OnTriggerEnter2D(Collider2D col) {
		CarController car = col.GetComponent<CarController>();
		if (car != null && car != lastCar) {
			if (State == PlayerState.OnGround && car.CurrentSpeed >= PlayerController.minDeathSpeed ) {
                State = PlayerState.Dead;
			}
			else if (State == PlayerState.Jumping) {
				State = PlayerState.OnCar;

				if (car == TargetCar) {
					GameManager.Instance.OnReachedTargetCar();
				}

				car.Colourize();
				this.transform.SetParent(car.transform);
				this.transform.position = car.transform.position;
			}
		}
	}

	public void ResetPlayer(Transform spawnLocation) {
		transform.SetParent(null);
		transform.position = spawnLocation.position;
		transform.rotation = spawnLocation.rotation;
		TriggerIdleAnimation();

		GetComponent<CircleCollider2D>().enabled = true;
		GetComponent<SpriteRenderer>().sortingLayerName = "Player";

		wasMoving = false;
		State = PlayerState.OnGround;
	}

	void TriggerJumpAnimation() {
		animator.SetTrigger("Jumping");
	}

	void TriggerIdleAnimation() {
		animator.SetTrigger("Idle");
	}

	void TriggerWalkingAnimation() {
		animator.SetTrigger("Walking");
	}

    void TriggerDeadAnimation()
    {
        animator.SetTrigger("Dead");
    }
}
