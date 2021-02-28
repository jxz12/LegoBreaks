using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public class Builder : MonoBehaviour {

    [SerializeField] Brick[] brickPrefabs;

    [SerializeField] int width;
    [SerializeField] int bricksAvailable;
    [SerializeField] Scorer scorer;
    [SerializeField] Confirmation confirm;

    private Brick placingBrick = null;
    private Stack<Brick> placedBricks = new Stack<Brick>();
    private Stack<Brick> unplacedBricks = new Stack<Brick>();

    // Start is called before the first frame update
    void Start() {
        confirm.onYes.AddListener(Drop);
        confirm.onNo.AddListener(Cancel);
        PlaceBrick();
    }
    void PlaceBrick() {
        if (placingBrick != null) {
            placedBricks.Push(placingBrick);
            placingBrick = null;
        }
        if (placedBricks.Count < bricksAvailable) {
            int randIdx = UnityEngine.Random.Range(0, brickPrefabs.Length);
            placingBrick = Instantiate(brickPrefabs[randIdx], transform);
            if (placedBricks.Count > 0) {
                placingBrick.CopyRotation(placedBricks.Peek());
            }
            MovePlacing();

            // clear redo stack
            while (unplacedBricks.Count > 0) {
                Destroy(unplacedBricks.Pop().gameObject);
            }
        } else {
            confirm.Show();
        }
    }
    void MovePlacing() {
        if (placingBrick == null) {
            return;
        }
        // https://docs.unity3d.com/ScriptReference/Plane.Raycast.html
        var m_Plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        if (m_Plane.Raycast(ray, out enter))
        {
            // get the point that is clicked
            Vector3 hitPoint = ray.GetPoint(enter);
            int row = (int)(hitPoint.z / Brick.posScale.z);
            int col = (int)(hitPoint.x / Brick.posScale.x);

            row = Math.Max(-width/2, Math.Min(width/2, row));
            col = Math.Max(-width/2, Math.Min(width/2, col));

            // TODO: should only do this on mouse movement or redo etc.
            //       or use something like a height map to make it O(1)
            int highest = -1;
            placingBrick.Place(row, col, 0); // temporary before raising
            var placingSet = new HashSet<Tuple<int,int>>(placingBrick.Occupied());
            foreach (var placed in placedBricks) {
                foreach (var xy in placed.Occupied()) {
                    if (placingSet.Contains(xy)) {
                        highest = Math.Max(highest, placed.height);
                    }
                }
            }
            placingBrick.Place(row, col, highest+1);
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.D)) {
            placingBrick.Rotate(true);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            placingBrick.Rotate(false);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            Undo();
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            Redo();
        }
        MovePlacing();
        if (Input.GetMouseButtonDown(0)) {
            PlaceBrick();
        }
    }
    public void Undo() {
        if (placedBricks.Count > 0) {
            unplacedBricks.Push(placedBricks.Pop());
            unplacedBricks.Peek().gameObject.SetActive(false);
        }
        MovePlacing();
    }
    public void Redo() {
        if (unplacedBricks.Count > 0) {
            placedBricks.Push(unplacedBricks.Pop());
            placedBricks.Peek().gameObject.SetActive(true);
        }
        MovePlacing();
    }
    void Drop() {
        foreach (var brick in placedBricks) {
            brick.ActivatePhysics();
        }
        enabled = false;
        scorer.Drop(placedBricks);
        // TODO: maybe add an exploder brick on impact?
    }
    void Cancel() {
        placingBrick = placedBricks.Pop();
    }
}
