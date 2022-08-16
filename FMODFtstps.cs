using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FMODFtstps : MonoBehaviour
{
    [Header("FMOD Settings")]
    [SerializeField] public FMODUnity.EventReference Footsteps;
    [SerializeField] private string SpeedParameterName;
    [SerializeField] private string MaterialParameterName;

    [Header("General Settings")]
    // Private vars
    // 1. Vars for footstep triggering
    private float StepRandom;
    private Vector3 PrevPos;
    private float DistanceTraveled;
    [SerializeField] private float StepDistance;
    [SerializeField] private float StartRunningTime;


    // 2. Vars for ground check
    private RaycastHit hit;
    [SerializeField] private float RayDistance;
    private bool PlayerTouchingGround;

    // 3. Walking/ Runing; Material FMOD vars
    private float StepTimeTaken;
    private int FMOD_F_Running;
    private int FMOD_F_Material;


    void Start()
    {
        StepRandom = Random.Range(0f, 0.5f);
        PrevPos = transform.position;
        Debug.Log(PrevPos);
    }

    // this method does the full magic
    void Update()
    {
        Debug.DrawRay(transform.position, Vector3.down * RayDistance, Color.red);
        StepTimeTaken += Time.deltaTime;
        DistanceTraveled += (transform.position - PrevPos).magnitude;
        if (DistanceTraveled >= StepDistance)
        {
            
            GroundCollision();
            MaterialCheck();
            SpeedCheck();
            PlayFootstep();
            StepRandom = Random.Range(0f, 2f);
            DistanceTraveled = 0f;
        }
        PrevPos = transform.position;

    }
    // This method calls and stops FMOD event
    void PlayFootstep()

    {
        if (PlayerTouchingGround == true)

        {
            FMOD.Studio.EventInstance Footstep = FMODUnity.RuntimeManager.CreateInstance(Footsteps);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(Footstep, transform);
            Footstep.setParameterByName(SpeedParameterName, FMOD_F_Running);
            Footstep.setParameterByName(MaterialParameterName, FMOD_F_Material);
            Footstep.start();
            Footstep.release();
        }
    }

    // This method checks the speed and assigns it to variable "F_Running"
    void SpeedCheck()
    {
        if (StepTimeTaken < StartRunningTime)
            FMOD_F_Running = 1;
        else
            FMOD_F_Running = 0;
        StepTimeTaken = 0f;
    }

    // This method checks for ground
    void GroundCollision()
    {
        Physics.Raycast(transform.position, Vector3.down, out hit, RayDistance);
        if (hit.collider)
            PlayerTouchingGround = true;
        else
            PlayerTouchingGround = false;
    }

    void MaterialCheck()
    {

        if (Physics.Raycast(transform.position, Vector3.down, out hit, RayDistance))
        {
            if (hit.transform.tag == "Area0")
            {
                FMOD_F_Material = 0;
            }

            else if (hit.transform.tag == "Area1")
            {
                FMOD_F_Material = 1;
            }
        }
        else
        {
            FMOD_F_Material = 0;
        }
   
    }

    
}
