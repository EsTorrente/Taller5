using UnityEngine;

public class PinFollower : MonoBehaviour
{
    public RectTransform pin;

    void Update()
    {
        pin.position = Input.mousePosition;

        pin.gameObject.SetActive(Input.GetKey(KeyCode.LeftShift));
    }
}