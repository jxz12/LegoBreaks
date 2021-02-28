using System;
using System.Collections.Generic;

using UnityEngine;

public class Scorer : MonoBehaviour {
    [SerializeField] Rigidbody door1, door2;
    [SerializeField] TMPro.TextMeshProUGUI scoreText;

    bool dropped = false;
    IEnumerable<Brick> droppedBricks;
    public void Drop(IEnumerable<Brick> bricks) {
        door1.isKinematic = false;
        door2.isKinematic = false;
        droppedBricks = bricks;
        dropped = true;
        scoreText.enabled = true;
    }
    public float score { get; private set; } = 0;
    public void Update() {
        if (dropped) {
            float maxSqMag = 0;
            foreach (var brick in droppedBricks) {
                var pos = brick.transform.position;
                pos = new Vector3(pos.x, 0, pos.z);
                maxSqMag = Mathf.Max(maxSqMag, pos.sqrMagnitude);
            }
            score = Mathf.Sqrt(maxSqMag);
            scoreText.text = $"Score: {(int)(score/6*5*100)}";
        }
    }
}