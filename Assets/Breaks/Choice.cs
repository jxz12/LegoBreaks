using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

public class Choice : MonoBehaviour {

    [SerializeField] Canvas canvas;
    [SerializeField] Button buttonYes;
    [SerializeField] Button buttonNo;
    [SerializeField] TextMeshProUGUI text;

    public UnityEvent onYes, onNo;

    void Start() {
        buttonYes.onClick.AddListener(Hide);
        buttonNo.onClick.AddListener(Hide);
        buttonYes.onClick.AddListener(()=> onYes.Invoke());
        buttonNo.onClick.AddListener(()=> onNo.Invoke());
    }
    public void SetText(string toSet) {
        text.text = toSet;
    }
    public void Show() {
        canvas.enabled = true;
    }
    void Hide() {
        canvas.enabled = false;
    }
}