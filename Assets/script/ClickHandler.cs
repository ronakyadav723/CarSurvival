using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public CarController carcontroller;

    void FixedUpdate()
    {
        float input = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            input = -0.8f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            input = 0.8f;
        }

        carcontroller.leftrightvalue = input;
    }
}
