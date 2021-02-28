using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Confirmation : MonoBehaviour {

    [SerializeField] Canvas canvas;
    [SerializeField] Button buttonYes;
    [SerializeField] Button buttonNo;

    public UnityEvent onYes, onNo;

    void Start() {
        buttonYes.onClick.AddListener(Hide);
        buttonNo.onClick.AddListener(Hide);
        buttonYes.onClick.AddListener(()=> onYes.Invoke());
        buttonNo.onClick.AddListener(()=> onNo.Invoke());
        Hide();
    }
    public void Show() {
        canvas.enabled = true;
    }
    void Hide() {
        canvas.enabled = false;
    }
}