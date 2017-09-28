﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;      //Allows us to use SceneManager
using UnityEngine.UI;      //Allows us to use SceneManager

public class Player : MovingObject {
	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int food;
	private Vector2 touchOrigin = -Vector2.one;

	protected override void Start () {
		animator = GetComponent<Animator>();
		food = GameManager.instance.playerFoodPoints;
		foodText.text = "Food: " + food;
		base.Start();
	}

	private void OnDisable() {
		GameManager.instance.playerFoodPoints = food;
	}
	// Update is called once per frame
	void Update () {
		if (!GameManager.instance.playersTurn) return;
		int vertical = 0;
		int horizontal = 0;

		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");

		if (horizontal != 0) {
			vertical = 0;
		}

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
		// code for mobile
		if (Input.touchCount > 0) {
			Touch myTouch = Input.touches[0];
			if (myTouch.phase == TouchPhase.Began) {
				touchOrigin = myTouch.position;
			} else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
				Vector2 touchEnd = myTouch.position;
				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;
				touchOrigin.x = -1;

				if (Mathf.Abs(x) > Mathf.Abs(y)) {
					horizontal = x > 0 ? 1: -1;
				} else {
					vertical = x > 0 ? 1: -1;
				}
			}
		}
		#endif

		if (horizontal != 0 || vertical != 0 ) {
			AttemptMove<Wall> (horizontal, vertical);
			AttemptMove<Enemy> (horizontal, vertical);
		}
	}

	protected override void AttemptMove <T> (int xDir, int yDir) {
		food--;
		foodText.text = "Food: " + food;
		base.AttemptMove <T> (xDir, yDir);
		RaycastHit2D hit;
		if (Move(xDir, yDir, out hit)) {
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}
		CheckIfGameOver();
		GameManager.instance.playersTurn = false;
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Exit") {
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		} else if (other.tag == "Food") {
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
			other.gameObject.SetActive(false);
		} else if (other.tag == "Soda") {
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
			other.gameObject.SetActive(false);
		}
	}

	protected override void OnCantMove<T>(T component) {
		Wall hitWall = component as Wall;
		hitWall.DamageWall(wallDamage);
		animator.SetTrigger("playerChop");
	}

	private void Restart() {
		 SceneManager.LoadScene (0);
	}

	public void LoseFood (int loss) {
		animator.SetTrigger("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver();
	}

	private void CheckIfGameOver () {
		if (food <= 0) {
			SoundManager.instance.RandomizeSfx(gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.GameOver();
		}
	}
}