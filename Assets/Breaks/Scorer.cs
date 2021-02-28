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
            // TODO: draw target rings 
            float maxSqMag = 0;
            foreach (var brick in droppedBricks) {
                maxSqMag = Mathf.Max(maxSqMag, brick.transform.position.sqrMagnitude);
            }
            score = Mathf.Sqrt(maxSqMag);
            scoreText.text = $"Score: {(int)(score*100)}";
        }
    }
}