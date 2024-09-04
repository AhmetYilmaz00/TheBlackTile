using UnityEngine;

[ExecuteInEditMode]
public class FreeRotateHandle : MonoBehaviour
{
    public Quaternion rot;
    public float size = 2;

    private void OnEnable()
    {
        rot = transform.rotation;   
    }

    public void Update()
    {
        transform.rotation = rot;
    }
}
