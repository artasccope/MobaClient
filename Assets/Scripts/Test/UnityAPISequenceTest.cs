using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityAPISequenceTest : MonoBehaviour {

    private void Reset()
    {
        
    }
    //######################StartUp################################################
    private void Awake()
    {
        Debug.Log("awaked");
    }

    private void OnEnable()
    {
        Debug.Log("enabled");
    }


    void Start () {
        Debug.Log("started");
	}
    //####################Update###################################################
    private void FixedUpdate()
    {
        
    }
    //yield waitForFixedUpdate

    void Update () {

	}

    //OnLevelWasLoaded

    private void DestroyObjs() {
        int index = 0;
        while (transform.childCount > 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);

                Debug.Log(index++);
            }
        }

    }
    //yield null /yield waitForSeconds 

    private void LateUpdate()
    {
        
    }
    //###########################Rendering#######################################

    //Renderer








    //rigidbody

    //rigidbody2D
    //camera
    private void OnPreCull()
    {
        
    }

    private void OnBecameVisible()
    {
        Debug.Log("visible");
    }

    private void OnBecameInvisible()
    {

    }

    private void OnPreRender()
    {

    }
    private void OnRenderObject()
    {

    }

    private void OnPostRender()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        
    }
    //light

    //animator
    private void OnAnimatorIK(int layerIndex)
    {
        
    }

    private void OnAnimatorMove()
    {
        
    }

    //constantForce

    //audio
    //guiText
    //networkView
    //guiElement

    //guiTexture
    //collider
    //collider2D

    //hingeJoint

    //transform
    //particleEmitter

    //particleSystem


    private void OnWillRenderObject()
    {
        
    }

    //###########################GUI#############################################
    private void OnGUI()
    {
        
    }

    //##########################################################################
    //yield waiteFroEndOfFrame

    //###########################Teardown#######################################
    private void OnDisable()
    {
        
    }

    private void OnDestroy()
    {
        
    }


}
