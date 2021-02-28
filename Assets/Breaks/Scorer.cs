using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using TMPro;

public class Scorer : MonoBehaviour {
    [SerializeField] Rigidbody door1, door2;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Choice replay;

    public int score { get; private set; } = 0;

    bool dropped = false;
    float finishTime;
    IEnumerable<Brick> droppedBricks;

    public UnityEvent onCollide;
    public UnityEvent onFinish;

    void Start() {
        replay.onYes.AddListener(()=> SceneManager.LoadScene("Play", LoadSceneMode.Single));
        replay.onNo.AddListener(()=> SceneManager.LoadScene("Menu", LoadSceneMode.Single));
    }

    public void Drop(IEnumerable<Brick> bricks) {
        door1.isKinematic = false;
        door2.isKinematic = false;
        droppedBricks = bricks;
        dropped = true;
        finishTime = Time.time + 1;
        scoreText.enabled = true;
    }
    public void Update() {
        if (dropped) {
            float maxSqMag = 0;
            bool isMoving = false;

            foreach (var brick in droppedBricks) {
                var pos = brick.transform.position;
                pos = new Vector3(pos.x, 0, pos.z);
                maxSqMag = Mathf.Max(maxSqMag, pos.sqrMagnitude);
                isMoving |= !brick.GetComponent<Rigidbody>().IsSleeping();
            }
            float rawScore = Mathf.Sqrt(maxSqMag);
            score = (int)(rawScore/6*5*100);  // magic number scale to match the rings
            scoreText.text = $"Score: {score}";

            if (isMoving) {
                finishTime = Mathf.Max(finishTime, Time.time + 1);
            }
            if (Time.time > finishTime) {
                dropped = false;
                scoreText.enabled = false;
                replay.SetText($"Final score: {score}\n Replay?");
                replay.Show();
                Destroy(door1.gameObject);
                Destroy(door2.gameObject);
                onFinish.Invoke();
            }
        }
    }

    bool collided = false;
    void OnCollisionEnter(Collision collision) {
        if (!collided) {
            onCollide.Invoke();
            collided = true;
        }
    }
}