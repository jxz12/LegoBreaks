using System;
using System.Collections.Generic;

using UnityEngine;

public class Scorer : MonoBehaviour {
    [SerializeField] Rigidbody door1, door2;

    bool dropped = false;
    IEnumerable<Brick> droppedBricks;
    public void Drop(IEnumerable<Brick> bricks) {
        door1.isKinematic = false;
        door2.isKinematic = false;
        droppedBricks = bricks;
        dropped = true;
    }
    public float score;
    public void Update() {
        if (dropped) {
            // TODO: draw target rings 
            float maxSqMag = 0;
            foreach (var brick in droppedBricks) {
                maxSqMag = Mathf.Max(maxSqMag, brick.transform.position.sqrMagnitude);
            }
            score = Mathf.Sqrt(maxSqMag);
        }
    }
}