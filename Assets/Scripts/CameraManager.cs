using System;

using UnityEngine;

public class CameraManager : MonoBehaviour {
    public static CameraManager Instance;
    [SerializeField] private Transform target;
    [SerializeField] private float smooth = 20;
    [SerializeField] private bool freeCamera;
    [ShowIf("freeCamera", true), SerializeField] private Size roomSize = new Size { width =18, height = 10 };
    [ShowIf("freeCamera"),SerializeField] private Vector2 offset;

    private void Awake() => Instance = this;
    private void LateUpdate() {
        if (target != null) {
            transform.position = Vector3.MoveTowards(transform.position, freeCamera ? FreePosition : NewPosition, Time.deltaTime * Time.timeScale * smooth);
        }
    }
    private Vector3 FreePosition => new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z); 
    private Vector3 NewPosition => new Vector3(Mathf.RoundToInt(target.position.x / roomSize.width) * roomSize.width, Mathf.RoundToInt(target.position.y / roomSize.height) * roomSize.height, -10 );
}
[Serializable]
public struct Size {
    public int width;
    public int height;
}