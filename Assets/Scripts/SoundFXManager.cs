using UnityEngine;
using System.Collections;

public class SoundFXManager : MonoBehaviour {
	public AudioClip youWinSFX;
	AudioSource audioSource;

	public static SoundFXManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;
			audioSource = GetComponent<AudioSource>();
		}
		else {
			Destroy (gameObject);
		}
	}

	public void PlayYouWinSFX() {
		audioSource.PlayOneShot(youWinSFX);
	}
}
