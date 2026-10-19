using UnityEngine;

public class Float : MonoBehaviour
{
    public float amplitude = 0.2f;
    public float frequency = 1f;

    public float rotationAmount = 5f;
    public float rotationSpeed = 0.8f;
    public float objRotY;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float y = startPos.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPos.x, y, startPos.z);

        float rotY = objRotY;
        float rotZ = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;
        float rotX = Mathf.Cos(Time.time * rotationSpeed * 0.7f) * rotationAmount;

        transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
    }
}