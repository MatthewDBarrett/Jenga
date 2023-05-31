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

public class BlockManager : MonoBehaviour {

    private SceneManager.DataBlock dataBlock;

    private Rigidbody rigid;

    private DetailManager detailManager;

    private Color originalColor;

    private bool clicked = false;

    private void Start() {
        originalColor = gameObject.GetComponent<Renderer>().material.GetColor( "_Color" );
    }

    private void Update() {
        if( clicked ) {
            if( Input.GetMouseButtonDown( 1 ) )
                SetBlockHighlighted( false );
            else if( Input.GetMouseButtonDown( 0 ) )
                CheckIfCurrent();
        }
    }

    /// <summary>
    /// Checks if the current data block matches the one displayed in the detail manager, and updates the block highlight accordingly.
    /// </summary>
    private void CheckIfCurrent() {
        if( detailManager == null )
            detailManager = GameObject.Find( "Canvas" ).transform.Find( "Block Details" ).GetComponent<DetailManager>();

        if( !detailManager.standardId.text.ToString().Equals( dataBlock.standardId ) )
            SetBlockHighlighted( false );
    }

    /// <summary>
    /// Sets the data block to be used by the scene manager.
    /// </summary>
    /// <param name="dataBlock">The data block to set.</param>
    public void SetDataBlock(SceneManager.DataBlock dataBlock) { this.dataBlock = dataBlock;  }

    /// <summary>
    /// Retrieves the current data block used by the scene manager.
    /// </summary>
    /// <returns>The current data block.</returns>
    public SceneManager.DataBlock GetDataBlock() { return dataBlock; }

    private void OnMouseDown() {
        if( detailManager == null )
            detailManager = GameObject.Find( "Canvas" ).transform.Find( "Block Details" ).GetComponent<DetailManager>();

        detailManager.SetWindowActive( true );

        detailManager.SetDetails( dataBlock.grade, dataBlock.domain, dataBlock.cluster, dataBlock.standardId, dataBlock.standardDescription );

        SetBlockHighlighted( true );

        clicked = true;
    }

    /// <summary>
    /// Sets the kinematic state of the attached Rigidbody component.
    /// </summary>
    /// <param name="isActive">True to set the Rigidbody as kinematic, false to set it as non-kinematic.</param>
    public void SetisRigidBody(bool isActive ) {

        rigid = gameObject.GetComponent<Rigidbody>();

        if( isActive )
            rigid.isKinematic = true;
        else
            rigid.isKinematic = false;
    }

    /// <summary>
    /// Sets the highlight state of the block by changing its color.
    /// </summary>
    /// <param name="isHighlighted">True to highlight the block, false to remove the highlight.</param>
    public void SetBlockHighlighted(bool isHighlighted ) {
        if( originalColor == null )
            originalColor = gameObject.GetComponent<Renderer>().material.GetColor( "_Color" );


        if ( isHighlighted ) {
            gameObject.GetComponent<Renderer>().material.SetColor( "_Color", new Color( 1f, 0.58f, 0f ) );
        } else {
            gameObject.GetComponent<Renderer>().material.SetColor( "_Color", originalColor );
        }

        clicked = false;
    }
}
