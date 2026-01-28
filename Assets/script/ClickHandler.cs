using UnityEngine;
using UnityEngine.EventSystems;

public class NewMonoBehaviourScript : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    public CarController carcontroller;
    public bool IsPressed;
    public bool IsLeftButton;
    public void OnPointerDown(PointerEventData eventData)
    {
       IsPressed=true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       IsPressed=false;
       carcontroller.leftrightvalue=0f;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsPressed)
        {
            if (IsLeftButton)
            {
                carcontroller.leftrightvalue=-1f;
            }
            else {
                carcontroller.leftrightvalue=+1f;
            }
        }
        
    }
}
