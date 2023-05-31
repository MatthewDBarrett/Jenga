using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/***************************************************************************************
 *  COPYRIGHT 2023 - Matthew Barrett
 *
 *  All rights reserved. This script and its associated documentation (if any) are the
 *  intellectual property of Matthew Barrett. You may use this script for personal or
 *  educational purposes. Modification, distribution, or reproduction of this script
 *  in any form, without explicit permission from the author, is strictly prohibited.
 *
 ***************************************************************************************/

public class DetailManager : MonoBehaviour {

    [SerializeField]
    public TextMeshProUGUI grade, domain, cluster, standardId, standardDesc;

    /// <summary>
    /// Sets the details of a given block, including grade, domain, cluster, standard ID, and standard description.
    /// </summary>
    /// <param name="grade">The grade to set.</param>
    /// <param name="domain">The domain to set.</param>
    /// <param name="cluster">The cluster to set.</param>
    /// <param name="standardId">The standard ID to set.</param>
    /// <param name="standardDesc">The standard description to set.</param>
    public void SetDetails(string grade, string domain, string cluster, string standardId, string standardDesc ) {
        this.grade.SetText( grade );
        this.domain.SetText( domain );
        this.cluster.SetText( cluster );
        this.standardId.SetText( standardId );
        this.standardDesc.SetText( standardDesc );
    }

    /// <summary>
    /// Sets the activity state of the detail manager.
    /// </summary>
    /// <param name="isActive">The desired activity state. True to activate, false to deactivate.</param>
    public void SetWindowActive( bool isActive ) {
        gameObject.SetActive( isActive );
    }
}
