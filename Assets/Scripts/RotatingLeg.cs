using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLeg : MonoBehaviour
{

    public static RotatingLeg instance;


    public float RotateSpeed;
    float StartSpeed;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

    }

    private void Start()
    {
        StartSpeed = RotateSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, -RotateSpeed * Time.deltaTime,0);       
    }

    public void StopRotatingLeg()
    {
        RotateSpeed = 0;
    }

    public void StartRotatingLeg()
    {
        RotateSpeed = StartSpeed;
    }



}
