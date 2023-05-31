using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/***************************************************************************************
 *  COPYRIGHT 2023 - Matthew Barrett
 *
 *  All rights reserved. This script and its associated documentation (if any) are the
 *  intellectual property of Matthew Barrett. You may use this script for personal or
 *  educational purposes. Modification, distribution, or reproduction of this script
 *  in any form, without explicit permission from the author, is strictly prohibited.
 *
 ***************************************************************************************/

public class OrbitalController : MonoBehaviour {

    [SerializeField]
    private GameObject ObjectInFocus;

    [Range( 20.0f, 70.0f )]
    public float orbitalDistance = 40f;

    [Range( 10.0f, 40.0f )]
    public float scrollWheelSensitvity = 20f;

    [Range( 0.0f, 1.0f )]
    public float orbitSpeed = 0.2f;

    private bool isDragging = false;
    private Vector3 dragStartPos;

    private DetailManager detailManager;

    void Update() {
        UpdateCamera();
    }

    /// <summary>
    /// Updates the camera position and rotation based on user input for orbiting around an object and zooming with the mouse.
    /// </summary>
    private void UpdateCamera() {
        if( Input.GetMouseButtonDown( 1 ) ) {
            isDragging = true;
            dragStartPos = Input.mousePosition;

            if( detailManager == null )
                detailManager = GameObject.Find( "Canvas" ).transform.Find( "Block Details" ).GetComponent<DetailManager>();

            detailManager.SetWindowActive( false );
        } else if ( Input.GetMouseButtonUp( 1 ) )
            isDragging = false;

        if( isDragging ) {
            Vector3 dragCurrentPos = Input.mousePosition;
            Vector3 dragVector = dragCurrentPos - dragStartPos;

            float rotationX = -dragVector.y * orbitSpeed;
            float rotationY = dragVector.x * orbitSpeed;

            transform.RotateAround( ObjectInFocus.transform.position, Vector3.up, rotationY );
            transform.RotateAround( ObjectInFocus.transform.position, transform.right, rotationX );

            dragStartPos = dragCurrentPos;
        }

        float scrollInput = Input.GetAxis( "Mouse ScrollWheel" );
        orbitalDistance -= scrollInput * scrollWheelSensitvity;

        Vector3 offset = -transform.forward * orbitalDistance;
        transform.position = ObjectInFocus.transform.position + offset;
    }

    /// <summary>
    /// Sets the object that the camera should focus on.
    /// </summary>
    /// <param name="obj">The GameObject to set as the focus object.</param>
    public void SetObjectInFocus(GameObject obj) {
        ObjectInFocus = obj;
    }
}
