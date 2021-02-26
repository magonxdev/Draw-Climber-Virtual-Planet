using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovingForce : MonoBehaviour
{

    public static MovingForce instance;

    public Text Score;
    public AudioSource CollectableSound;

    ConstantForce forcaConstante;



    private void Awake()
    {

        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);

    }

    private void Start()
    {
        forcaConstante = GetComponent<ConstantForce>();
    }

    public void SetMovingVelocity()
    {
        forcaConstante.enabled = true;
        RotatingLeg.instance.StartRotatingLeg();
    }

    public void StopMoving()
    {
        forcaConstante.enabled = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        RotatingLeg.instance.StopRotatingLeg();
    }

    private void OnTriggerEnter(Collider other)
    {
     
           if (other.tag == "Collectable")
        {
            Score.text = "" + (int.Parse(Score.text) + 1);
            Destroy(other.gameObject);
            CollectableSound.Play();

        }

    }

}
