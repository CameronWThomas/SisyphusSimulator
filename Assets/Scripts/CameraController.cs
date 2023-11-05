using Assets.Scripts;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Quaternion PlanarRotation2 => Quaternion.Euler(0, transform.eulerAngles.y, 0);
    [SerializeField]
    CinemachineBrain braaaiins;
    [SerializeField]
    CinemachineFreeLook freeLookCamera;

    Sisyphus sisyphus;
    Boulder boulder;
    // Start is called before the first frame update
    void Start()
    {
        braaaiins = GetComponent<CinemachineBrain>();
        freeLookCamera = FindObjectOfType<CinemachineFreeLook>();

        sisyphus = FindObjectOfType<Sisyphus>();
        boulder = FindObjectOfType<Boulder>();
    }

    public void SetRollingCamera()
    {
        //inconsistent loading on start.
        //Doesnt matter if we set it manually before we run the game
        if (braaaiins != null)
        {
            braaaiins.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.FixedUpdate;
            braaaiins.m_UpdateMethod = CinemachineBrain.UpdateMethod.FixedUpdate;
            freeLookCamera.Follow = sisyphus.transform;
            freeLookCamera.LookAt = boulder.transform;
        }

    }
    public void SetWalkingCamera()
    {
        //inconsistent loading on start.
        //Doesnt matter if we set it manually before we run the game
        if (braaaiins != null)
        {
            braaaiins.m_BlendUpdateMethod = CinemachineBrain.BrainUpdateMethod.LateUpdate;
            braaaiins.m_UpdateMethod = CinemachineBrain.UpdateMethod.LateUpdate;
            freeLookCamera.Follow = sisyphus.transform;
            freeLookCamera.LookAt = sisyphus.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
