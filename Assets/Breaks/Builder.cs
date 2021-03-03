using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using TMPro;

public class Builder : MonoBehaviour {

    [SerializeField] Brick[] brickPrefabs;

    [SerializeField] int width;
    [SerializeField] int bricksAvailable;
    [SerializeField] Scorer scorer;
    [SerializeField] Choice drop;
    [SerializeField] Tripod tripod;

    [SerializeField] Button controls;
    [SerializeField] TextMeshProUGUI remaining;


    private Brick placingBrick = null;
    private Stack<Brick> placedBricks = new Stack<Brick>();
    private Stack<Brick> unplacedBricks = new Stack<Brick>();

    public UnityEvent onPlace;
    public UnityEvent onRotate;
    public UnityEvent onDrop;

    bool cancelTrigger;

    // Start is called before the first frame update
    void Start() {
        drop.onYes.AddListener(Drop);
        drop.onNo.AddListener(Cancel);
        PlaceBrick();
    }
    void PlaceBrick() {
        if (placingBrick != null) {
            placingBrick.Opaque(true);
            placedBricks.Push(placingBrick);
            placingBrick = null;
            onPlace.Invoke();
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
            drop.Show();
        }
    }
    void MovePlacing() {
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
        if (placingBrick != null) {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
                placingBrick.Rotate(true);
                onRotate.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
                placingBrick.Rotate(false);
                onRotate.Invoke();
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
                Undo();
            }
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
                Redo();
            }
            MovePlacing();
            if (Input.GetMouseButtonDown(0)) {
                // extra check in case use is trying to open controls
                if (!EventSystem.current.IsPointerOverGameObject()) {
                    PlaceBrick();
                }
            }
        } else if (cancelTrigger) {
            placingBrick = placedBricks.Pop();
            placingBrick.Opaque(false);
            cancelTrigger = false;
        }
        remaining.text = $"Bricks remaining: {bricksAvailable - placedBricks.Count}";
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
            brick.TogglePhysics(true);
        }
        scorer.Drop(placedBricks);
        tripod.Follow(placedBricks, new Vector3(0,-2,-10));
        controls.gameObject.SetActive(false);
        remaining.enabled = false;
        // TODO: maybe add an exploder brick on impact?
        onDrop.Invoke();
    }
    void Cancel() {
        // bring back brick on next frame to prevent instant re-place
        cancelTrigger = true;
    }
}
