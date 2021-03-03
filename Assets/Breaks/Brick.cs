using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

public class Brick : MonoBehaviour {

    [SerializeField] int[] rowStuds;
    [SerializeField] int[] colStuds;
    [SerializeField] float rowOffset;
    [SerializeField] float colOffset;
    [SerializeField] Transform[] toRotate;
    [SerializeField] Rigidbody rbody;

    [SerializeField] GameObject transparent;
    [SerializeField] GameObject opaque;

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
    }
    public void Opaque(bool isOpaque) {
        transparent.SetActive(!isOpaque);
        opaque.SetActive(isOpaque);
    }

    public int rotationStep { get; private set; } = 0;
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
    }

    // smoothly rotate actual GameObject for Unity
    float velocityRotation = 0;
    void TweenRotation() {
        if (rbody.isKinematic) {
            targetRotation = rotationStep * 90;
            currentRotation = Mathf.SmoothDamp(
                currentRotation,
                targetRotation,
                ref velocityRotation,
                .05f
            );
            foreach (Transform t in toRotate) {
                t.rotation = Quaternion.Euler(0, currentRotation, 0);
            }
        }
    }
    // for instantly setting rotation
    public void CopyRotation(Brick toCopy) {
        for (int i=0; i<toCopy.rotationStep%4; i++) {
            Rotate(true);
        }
        targetRotation = rotationStep * 90;
        currentRotation = targetRotation;
        foreach (Transform t in toRotate) {
            t.rotation = Quaternion.Euler(0, currentRotation, 0);
        }
    }
    void Update() {
        TweenRotation();
    }
    public void TogglePhysics(bool on) {
        rbody.isKinematic = !on;
    }
}