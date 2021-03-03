using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

using TMPro;

public class Scorer : MonoBehaviour {
    [SerializeField] Rigidbody door1, door2;
    [SerializeField] TextMeshPro scoreText;
    [SerializeField] Choice replay;
    [SerializeField] LineRenderer maxDistLine;

    public int score { get; private set; } = 0;

    bool dropped = false;
    float startTime, finishTime;
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
        
        startTime = Time.time;
        finishTime = Time.time + 1;

        maxDistLine.enabled = true;
        scoreText.enabled = true;
    }
    public void Update() {
        if (dropped) {
            float maxSqMag = 0;
            bool isMoving = false;
            Vector3 argMax = Vector3.zero;

            foreach (var brick in droppedBricks) {
                // var pos = brick.transform.position;
                var collider = brick.GetComponentInChildren<BoxCollider>();
                var pos = collider.transform.TransformPoint(collider.center);
                pos = new Vector3(pos.x, 0, pos.z);
                if (pos.sqrMagnitude > maxSqMag) {
                    maxSqMag = pos.sqrMagnitude;
                    argMax = pos;
                }
                isMoving |= !brick.GetComponent<Rigidbody>().IsSleeping();
            }
            Vector3 lineStart = transform.position;
            Vector3 lineEnd = argMax + transform.position;
            for (int i=0; i<24; i++) {
                Vector3 lineMid = Vector3.Lerp(lineStart, lineEnd, i/24f);
                float x = i / 11.5f;
                float y = -((x-1)*(x-1))+1;  // makes a parabola
                maxDistLine.SetPosition(i, lineMid + 3.5f*y*Vector3.up + Vector3.up);
            }

            float rawScore = Mathf.Sqrt(maxSqMag);
            score = (int)(rawScore/6*5*100);  // magic number scale to match the rings
            scoreText.text = $"{score}";

            scoreText.transform.position = argMax/2 + transform.position + new Vector3(0,5,0);

            if (isMoving) {
                finishTime = Mathf.Max(finishTime, Time.time + 1);
            }
            if (Time.time > finishTime || Time.time > startTime+8) {
                dropped = false;
                replay.SetText($"Final score: {score}\n Replay?");
                replay.Show();
                Destroy(door1.gameObject);
                Destroy(door2.gameObject);
                onFinish.Invoke();
                // StartCoroutine(ScoreRoutine());
            }
        }
    }

    IEnumerator ScoreRoutine() {
        maxDistLine.enabled = true;
        yield return new WaitForSeconds(.5f);
        maxDistLine.enabled = false;
        yield return new WaitForSeconds(.5f);
        maxDistLine.enabled = true;
        yield return new WaitForSeconds(.5f);
        maxDistLine.enabled = false;
        yield return new WaitForSeconds(.5f);
        maxDistLine.enabled = true;
    }

    bool collided = false;
    void OnCollisionEnter(Collision collision) {
        if (!collided) {
            onCollide.Invoke();
            collided = true;
        }
    }
}