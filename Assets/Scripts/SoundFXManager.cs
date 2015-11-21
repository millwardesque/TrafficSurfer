using UnityEngine;
using System.Collections;

public class SoundFXManager : MonoBehaviour {
	public AudioClip youWinSFX;
	AudioSource audio;

	public static SoundFXManager Instance = null;

	void Awake() {
		if (Instance == null) {
			Instance = this;
			audio = GetComponent<AudioSource>();
		}
		else {
			Destroy (gameObject);
		}
	}

	public void PlayYouWinSFX() {
		this.audio.PlayOneShot(youWinSFX);
	}
}
