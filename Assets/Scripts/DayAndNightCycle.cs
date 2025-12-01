using UnityEngine;

public class DayAndNightCycle : MonoBehaviour
{
    [SerializeField]
    private Light light;
    [SerializeField]
    private float rotationSpeed;

    private void Update()
    {
        light.transform.Rotate(Vector3.right, rotationSpeed*Time.deltaTime);
    }
}
