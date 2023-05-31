using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

/***************************************************************************************
 *  COPYRIGHT 2023 - Matthew Barrett
 *
 *  All rights reserved. This script and its associated documentation (if any) are the
 *  intellectual property of Matthew Barrett. You may use this script for personal or
 *  educational purposes. Modification, distribution, or reproduction of this script
 *  in any form, without explicit permission from the author, is strictly prohibited.
 *
 ***************************************************************************************/


public class SceneManager : MonoBehaviour {

    [SerializeField]
    private GameObject jengaBlock;

    [SerializeField]
    private Material glass, wood, stone;

    private List<DataBlock> dataBlocks;

    private List<GameObject> jengaBlocks;

    void Start() {

        jengaBlocks = new List<GameObject>();

        StartCoroutine( GetRequest( "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack" ) );
    }

    IEnumerator GetRequest( string uri ) {
        using( UnityWebRequest webRequest = UnityWebRequest.Get( uri ) ) {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split( '/' );
            int page = pages.Length - 1;

            switch( webRequest.result ) {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError( pages[page] + ": Error: " + webRequest.error );
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError( pages[page] + ": HTTP Error: " + webRequest.error );
                    break;
                case UnityWebRequest.Result.Success:
                    ProcessData( webRequest.downloadHandler.text );
                    break;
            }  
        }
    }

    /// <summary>
    /// Processes the data by deserializing it into a list of DataBlock objects and spawns the blocks.
    /// </summary>
    /// <param name="data">The data to process.</param>
    private void ProcessData( string data ) { 
        dataBlocks = JsonConvert.DeserializeObject<List<DataBlock>>( data );

        StartCoroutine( SpawnBlocks() );
    }

    /// <summary>
    /// Spawns the blocks based on the allocated grade groups and their positions.
    /// </summary>
    IEnumerator SpawnBlocks() {

        List<List<DataBlock>> grades = new List<List<DataBlock>>
        {
            new List<DataBlock>(),
            new List<DataBlock>(),
            new List<DataBlock>()
        };

        int gradeId = 0;

        OrderBlocks();

        //Allocate blocks into 3 different year groups. 0 = 6 | 1 = 7 | 2 = 8
        foreach( DataBlock dataBlock in dataBlocks ) {
            string gradeString = dataBlock.grade;

            if( !string.IsNullOrEmpty( gradeString ) && char.IsDigit( gradeString[0] ) ) {
                if( int.TryParse( gradeString[0].ToString(), out int result ) )
                    gradeId = result;

                switch( gradeId ) {
                    case 6:
                        grades[0].Add( dataBlock );
                        break;
                    case 7:
                        grades[1].Add( dataBlock );
                        break;
                    case 8:
                        grades[2].Add( dataBlock );
                        break;
                }
            } 
        }

        int gradeCount = 0;

        foreach( List<DataBlock> grade in grades ) {
            float stackSpeed = 0.0f;

            int count = 0;
            bool layer = false;
            int height = 0;

            Vector3 posOffset = new Vector3( 0, 0, 0 );

            switch( gradeCount ) {
                case 0:
                    posOffset = new Vector3( 0, 0, -30 );
                    break;
                case 1:
                    posOffset = new Vector3( 0, 0, 0 );
                    break;
                case 2:
                    posOffset = new Vector3( 0, 0, 30 );
                    break;
            }

            foreach( DataBlock dataBlock in grade ) {
                Quaternion rot;
                Vector3 pos = new Vector3( 0, 0, 0 );

                if( layer ) {
                    rot = Quaternion.Euler( 0f, 90f, 0f );

                    if( count == 0 )
                        pos = new Vector3( 5, height, 5 );
                    if( count == 1 )
                        pos = new Vector3( 5, height, 0 );
                    if( count == 2 )
                        pos = new Vector3( 5, height, -5 );

                } else {
                    rot = Quaternion.identity;

                    pos = new Vector3( count * 5, height, 0 );
                }

                SpawnBlock( pos + posOffset, rot, dataBlock.mastery, dataBlock );

                count++;

                if( count > 2 ) {
                    count = 0;
                    height += 3;

                    layer = !layer;
                }

                yield return new WaitForSeconds( stackSpeed );
            }

            gradeCount++;
        }
    }

    /// <summary>
    /// Orders the data blocks based on the domain, cluster, and standard ID.
    /// </summary>
    private void OrderBlocks() {
        dataBlocks.Sort( ( a, b ) => {
            int domainComparison = a.domain.CompareTo( b.domain );
            if( domainComparison != 0 )
                return domainComparison;

            int clusterComparison = a.cluster.CompareTo( b.cluster );
            if( clusterComparison != 0 )
                return clusterComparison;

            return a.standardId.CompareTo( b.standardId );
        } );
    }

    /// <summary>
    /// Spawns a block at the specified position and rotation, assigns the corresponding material based on mastery level,
    /// and sets the data block.
    /// </summary>
    /// <param name="position">The position to spawn the block.</param>
    /// <param name="rotation">The rotation of the block.</param>
    /// <param name="mastery">The mastery level of the block.</param>
    /// <param name="dataBlock">The data block associated with the spawned block.</param>
    private void SpawnBlock( Vector3 position, Quaternion rotation, int mastery, DataBlock dataBlock ) {
        GameObject newBlock = Instantiate( jengaBlock, position, rotation );
        jengaBlocks.Add( newBlock );

        switch( mastery ) {
            case 0:
                newBlock.GetComponent<Renderer>().material = glass;
                break;
            case 1:
                newBlock.GetComponent<Renderer>().material = wood;
                break;
            case 2:
                newBlock.GetComponent<Renderer>().material = stone;
                break;
        }

        newBlock.GetComponent<BlockManager>().SetDataBlock( dataBlock );

        newBlock.GetComponent<BlockManager>().SetisRigidBody( true );
    }

    /// <summary>
    /// Tests the stack by finding and destroying all glass blocks and setting all blocks in the scene to be kinematic.
    /// </summary>
    public void TestMyStack() {
        List<GameObject> blocksToDestroy = new List<GameObject>();

        //Find and destroy all glass blocks
        foreach( GameObject jenga in jengaBlocks )
            if( jenga.GetComponent<BlockManager>().GetDataBlock().mastery == 0 )
                Destroy( jenga );

        //Set all blocks in the scene to be kinematic
        foreach (GameObject jenga in jengaBlocks)
            jenga.GetComponent<BlockManager>().SetisRigidBody( false );
    }

    public struct DataBlock {
        public int id;
        public string subject;
        public string grade;
        public int mastery;
        public string domainId;
        public string domain;
        public string cluster;
        public string standardId;
        public string standardDescription;

        public DataBlock(int id, string subject, string grade, int mastery, string domainId, string domain, string cluster, string standardId, string standardDescription ) {
            this.id = id;
            this.subject = subject;
            this.grade = grade;
            this.mastery = mastery;
            this.domainId = domainId;
            this.domain = domain;
            this.cluster = cluster;
            this.standardId = standardId;
            this.standardDescription = standardDescription;
        }
    }
}
