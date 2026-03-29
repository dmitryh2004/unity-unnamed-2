using UnityEngine;

public class BaseTavernAmbientChanger : MonoBehaviour {
	[SerializeField] AudioSource source;
	[SerializeField] AudioClip september3Clip;
	
	void Awake() {
		if (DayCheck.Instance != null && DayCheck.Instance.IsSeptember3) {
			source.clip = september3Clip;
			source.Play();
		}
	}
}