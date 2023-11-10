using UnityEngine;

public class CameraReference : MonoBehaviour
{
    public Quaternion PlanarRotation2 => Quaternion.Euler(0, transform.eulerAngles.y, 0);
}
