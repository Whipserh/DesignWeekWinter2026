using UnityEngine;

public class ClosePanelInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            gameObject.SetActive(false);
        }
    }
}