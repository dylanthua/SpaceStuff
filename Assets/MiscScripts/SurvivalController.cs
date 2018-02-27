using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurvivalController : MonoBehaviour {

	public BaseShip[] enemies;
	public GameObject victoryController;
	public GameObject defeatController;

	private int score;
	private bool hasEnded = false;
	private int combo = 1;
	private float comboTimer;
	private float comboCooldown = 5f;

	// Use this for initialization
	void Start () {
		enemies = FindObjectsOfType<BaseShip> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Quit")) {
			Application.Quit ();
		}

		if (!hasEnded) {
			comboTimer += Time.deltaTime;
			if (comboTimer >= comboCooldown) {
				combo = 1;
				comboTimer = comboCooldown;
			}
			bool enemiesLeft = false;

			for (int i = 0; i < enemies.Length; i++) {
				if (enemies [i] != null && !enemies [i].isPlayerSide) {
					enemiesLeft = true;
				}
			}

			if (!enemiesLeft && GameObject.FindGameObjectWithTag("Player") != null) {
				// Player wins
				Destroy (GameObject.FindGameObjectWithTag("Player"));
				GameObject spawned = Instantiate (victoryController, transform.position, transform.rotation);
				spawned.GetComponent<PlayerWinLose>().score = score;
				hasEnded = true;
			} else if (GameObject.FindGameObjectWithTag("Player") == null) {
				// Player loses
				GameObject spawned = Instantiate (defeatController, transform.position, transform.rotation);
				spawned.GetComponent<PlayerWinLose>().score = score;
				hasEnded = true;
			}
				
		} else {
			if (Input.GetButtonDown ("Fire1")) {
				SceneManager.LoadSceneAsync ("first");
			}
		}
	}

	void SpawnEnemies() {
	}

	public void ClearTimer() {
		comboTimer = 0;
	}

	public void IncreaseScore(int amount) {
		score += combo * amount;
		combo++;
	}

	public int GetScore() {
		return score;
	}

	public int GetCombo() {
		return combo;
	}
}
