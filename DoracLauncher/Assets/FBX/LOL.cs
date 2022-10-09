using UnityEngine;

public class LOL : MonoBehaviour
{
    public Camera cam;

    public float zaxies;
    Vector3 point;

    private void Update()
    {
        point = new Vector3();
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = Input.mousePosition.x;
        mousePos.y = Input.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, zaxies));

    }

    private void LateUpdate()
    {

        this.gameObject.transform.position = point;
    }
}