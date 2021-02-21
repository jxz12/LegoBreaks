using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public class Builder : MonoBehaviour {

    [SerializeField] Brick twoByFourPrefab;

    [SerializeField] int radius = 5;
    [SerializeField] int bricksAvailable = 10;

    private Brick currentBrick = null;
    private Stack<Brick> placedBricks = new Stack<Brick>();
    private Stack<Brick> unplacedBricks = new Stack<Brick>();

    // Start is called before the first frame update
    void Start() {
        NewBrick();
    }
    int mouseRow;
    int mouseCol;
    void NewBrick() {
        if (currentBrick != null) {
            placedBricks.Push(currentBrick);
        }
        if (placedBricks.Count < bricksAvailable) {
            currentBrick = Instantiate(twoByFourPrefab, transform);
            mouseRow = int.MinValue;
            mouseCol = int.MinValue;
            PositionCurrent();
        } else {
            currentBrick = null;
            print("done!"); // TODO: destroy this object and move to drop
        }
        // clear redo stack
        while (unplacedBricks.Count > 0) {
            Destroy(unplacedBricks.Peek().gameObject);
        }
    }
    void PositionCurrent() {
        // https://docs.unity3d.com/ScriptReference/Plane.Raycast.html
        var m_Plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        if (m_Plane.Raycast(ray, out enter))
        {
            // get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            int col = (int)(hitPoint.x / Brick.posScale.x);
            int row = (int)(hitPoint.z / Brick.posScale.z);

            if (row != mouseRow || col != mouseCol) {
                // iterate through all 
                int highest = -1;
                currentBrick.Place(row, col, 0); // temporary before raising
                var currentSet = new HashSet<Tuple<int,int>>(currentBrick.Occupied());
                foreach (var placed in placedBricks) {
                    foreach (var xy in placed.Occupied()) {
                        if (currentSet.Contains(xy)) {
                            highest = Math.Max(highest, placed.height);
                        }
                    }
                }
                currentBrick.Place(row, col, highest+1);
                mouseRow = row;
                mouseCol = col;
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.D)) {
            currentBrick.Rotate(true);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            currentBrick.Rotate(false);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            Undo();
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            Redo();
        }
        PositionCurrent();
        if (Input.GetKeyDown(KeyCode.Space)) {
            NewBrick();
        }
        // maybe add an exploder brick on impact?
    }
    public void Undo() {
        print("hi");
    }
    public void Redo() {
        print("bye");
    }
}
