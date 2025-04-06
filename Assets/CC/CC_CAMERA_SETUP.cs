using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* (C) 09/28/2024 Jason Leigh - Laboratory for Advanced Visualization & Applications
Set up eye separation for the two cameras.
*/

public class CC_CAMERA_SETUP : MonoBehaviour
{
    public GameObject leftCamera;
    public GameObject rightCamera;
    public float eyeSeparation = 0.061f;
    public bool invertStereo = false;

    private Vector3 leftCamPosition, rightCamPosition;
    private bool currentStereo;
    private float currentEyeSeparation;

    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor){
            if (CC_CONFIG.LoadXMLConfig()) {
                eyeSeparation = CC_CONFIG.interaxial;
                invertStereo = CC_CONFIG.invertStereo;
            }
        }

        leftCamPosition = leftCamera.transform.position;
        rightCamPosition = rightCamera.transform.position;
        currentStereo = invertStereo;
        currentEyeSeparation = eyeSeparation;

        if (!invertStereo){
            leftCamera.transform.position = leftCamPosition +new Vector3(-eyeSeparation/2.0f,0,0);
            rightCamera.transform.position = rightCamPosition +new Vector3(eyeSeparation/2.0f,0,0);
        } else {
            leftCamera.transform.position = leftCamPosition + new Vector3(eyeSeparation/2.0f,0,0);
            rightCamera.transform.position = rightCamPosition + new Vector3(-eyeSeparation/2.0f,0,0);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if ((currentStereo == invertStereo) && (currentEyeSeparation == eyeSeparation)) return;
        currentStereo = invertStereo;
        if (!invertStereo){
            leftCamera.transform.position = leftCamPosition +new Vector3(-eyeSeparation/2.0f,0,0);
            rightCamera.transform.position = rightCamPosition +new Vector3(eyeSeparation/2.0f,0,0);
        } else {
            leftCamera.transform.position = leftCamPosition + new Vector3(eyeSeparation/2.0f,0,0);
            rightCamera.transform.position = rightCamPosition + new Vector3(-eyeSeparation/2.0f,0,0);
        }
    }
}
