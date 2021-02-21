using System;
using System.Collections.Generic;

using UnityEngine;

public class Brick : MonoBehaviour {

    [SerializeField] int[] rowStuds;
    [SerializeField] int[] colStuds;
    [SerializeField] float rowOffset;
    [SerializeField] float colOffset;
    [SerializeField] Transform toRotate;
    [SerializeField] Rigidbody rbody;

    void Start() {
        if (rowStuds.Length != colStuds.Length) {
            throw new Exception("length of row must equal length of col");
        }
        rbody.isKinematic = true;
    }
    public IEnumerable<Tuple<int,int>> Occupied() {
        for (int i=0; i<rowStuds.Length; i++) {
            yield return Tuple.Create(rowPos + rowStuds[i], colPos + colStuds[i]);
        }
    }

    
    public int height { get; private set; }
    public int rowPos { get; private set; }
    public int colPos { get; private set; }
    public static Vector3 posScale = new Vector3(.8f, .96f, .8f);
    public void Place(int row, int col, int height) {
        rowPos = row;
        colPos = col;
        this.height = height;
        transform.localPosition = new Vector3(
            colPos * posScale.x,
            height * posScale.y,
            rowPos * posScale.z
        );
        transform.localPosition += new Vector3(
            colOffset * posScale.x,
            0,
            rowOffset * posScale.z
        );
        print($"placed! ({colPos}, {rowPos}, {height})");
    }

    int rotationStep = 0;
    float currentRotation = 0, targetRotation = 0;
    public void Rotate(bool isClockwise) {
        // one rotation is swap row and col
        // next is invert row and col
        // last is swap row and col and invert
        void Swap() {
            var temp = rowStuds;
            rowStuds = colStuds;
            colStuds = temp;
        }
        void Invert() {
            if (rotationStep%2 == 2) {
                for (int i=0; i<colStuds.Length; i++) {
                    colStuds[i] = -colStuds[i];
                }
            } else {
                for (int i=0; i<rowStuds.Length; i++) {
                    rowStuds[i] = -rowStuds[i];
                }
            }
        }
        if (isClockwise) {
            rotationStep += 1;
            Swap();
            Invert();
        } else {
            Invert();
            Swap();
            rotationStep -= 1;
        }
        // actually rotate object for Unity
        targetRotation += isClockwise? 90:-90;
    }
    float velocityRotation = 0;
    void Update() {
        currentRotation = Mathf.SmoothDamp(
            currentRotation,
            targetRotation,
            ref velocityRotation,
            .1f
        );
        toRotate.rotation = Quaternion.Euler(0, currentRotation, 0);
    }
    public void ActivatePhysics() {
        rbody.isKinematic = false;
    }
}