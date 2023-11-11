using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    [Header("Raycast Data")]
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform crosshairRectT;
    [SerializeField] private float rayDist;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private Vector3 hitPoint;
    [SerializeField] private bool isHit;
    private RaycastHit hit;

    public bool IsHit { get { return isHit; } }
    public Vector3 HitPoint { get { return hitPoint; } }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ShootRay();
    }

    private void ShootRay()
    {
        Ray ray = cam.ScreenPointToRay(crosshairRectT.transform.position);
        Physics.Raycast(ray, out hit, 100, hitLayer);

        isHit = hit.collider != null;

        if (hit.collider != null)
        {
            hitPoint = hit.point;
        }
    }
}
