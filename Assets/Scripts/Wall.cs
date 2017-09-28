using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {
	// public AudioClip chopSound1;                //1 of 2 audio clips that play when the wall is attacked by the player.
	// public AudioClip chopSound2;
	public Sprite  dmgSprite;
	public int hp = 4;
	public AudioClip chopSound1;
	public AudioClip chopSound2;

	private SpriteRenderer spriteRenderer;
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void DamageWall (int loss) {
		//Call the RandomizeSfx function of SoundManager to play one of two chop sounds.
		SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
		spriteRenderer.sprite = dmgSprite;
		hp -= loss;
		if (hp <= 0) {
			gameObject.SetActive(false);
		}
	}
}
