using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DemoScene : MonoBehaviour 
{
    public Button btn45;
    public Button btn90;

    private void Start()
    {
        Transform camT = Camera.main.transform;
        btn45.onClick.AddListener(() => SetXRotation(camT, 45f));
        btn90.onClick.AddListener(() => SetXRotation(camT, 90f));
    }

    private void SetXRotation(Transform t, float angle)
    {
        t.localEulerAngles = new Vector3(angle, t.localEulerAngles.y, t.localEulerAngles.z);
    }
}
