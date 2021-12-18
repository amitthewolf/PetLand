using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float RotationSpeed;
    private Camera cam;
    public static GameObject TargetPet;
    public float speed = 0.1f;
    [Range(0.01f,1.0f)]
    public float SmoothFactor;
    public float MinZoom;
    public float MaxZoom;
    private bool Following;
    private bool Rotating;
    private bool ZoomingOnPet;
    private Vector3 Offset;
    
    void Start()
    {
        EventManager.PetSpawned += SetPet;
        if (TargetPet != null)
        {
            transform.position = TargetPet.transform.position + new Vector3(6, 6);
            Offset = transform.position - TargetPet.transform.position;
        }
        Following = true;
        cam = GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        EventManager.PetSpawned -= SetPet;
    }

    public void SetPet(object sender, EventArgs args)
    {
        TargetPet = (GameObject)sender;
        transform.position = TargetPet.transform.position + new Vector3(6, 6);
        Offset = transform.position - TargetPet.transform.position;
    }
    
    public void StartFollow()
    {
        Following = true;
    }

    void LateUpdate()
    {
        if (Input.touchCount == 2)
        {
            Touch Touch0 = Input.GetTouch(0);
            Touch Touch1 = Input.GetTouch(1);
            Vector2 Touch0PrevPos = Touch0.position - Touch0.deltaPosition;
            Vector2 Touch1PrevPos = Touch1.position - Touch1.deltaPosition;

            float PrevMagnitude = (Touch0PrevPos - Touch1PrevPos).magnitude;
            float CurrentMagnitude = (Touch0.position - Touch1.position).magnitude;

            float Difference = CurrentMagnitude - PrevMagnitude;
            zoom(Difference * 0.02f);
        } else if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            Quaternion camTurnAngle = Quaternion.AngleAxis(touchDeltaPosition.x * RotationSpeed,Vector3.up);
            Offset = camTurnAngle * Offset;
        }
        if (TargetPet != null)
        {
            Vector3 NewPos = TargetPet.transform.position + Offset;
            transform.position = Vector3.Slerp(transform.position, NewPos, SmoothFactor);
        }
        CameraRestrictions();

    }

    private void CameraRestrictions()
    {
        if (TargetPet != null)
        {
            transform.LookAt(TargetPet.transform.position);
        }

        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -15f, 25f),
            Mathf.Clamp(transform.position.y, 2.5f, 10f),
            Mathf.Clamp(transform.position.z, -18f, 17f));
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    // public static void SetTargetPet(GameObject NewTarget)
    // {
    //     TargetPet = NewTarget;
    // }
    //
    void zoom(float increment)
    {
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - increment, MinZoom, MaxZoom);
    }

    public void TrashSeek()
    {
        TargetPet.GetComponent<PetBrain>().LookForTrash();
    }
}
