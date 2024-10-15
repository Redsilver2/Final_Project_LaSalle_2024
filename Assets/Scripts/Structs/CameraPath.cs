using UnityEngine;

[System.Serializable]
public struct CameraPath
{
    [SerializeField] private Vector3    position;
    [SerializeField] private Quaternion rotation;
    public Vector3 Position => position;
    public Quaternion Rotation => rotation;

    public CameraPath(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}
