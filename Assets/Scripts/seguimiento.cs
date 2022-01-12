using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seguimiento : MonoBehaviour
{
    public CharacterController Player;
    public GameObject cameraSeguimiento;
    public float speedH;
    public float speedV;

    private Vector3 offset;
    float yaw = 0.0f;
    float pitch = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        offset = cameraSeguimiento.transform.position - Player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        cameraSeguimientof();
    }
    void cameraSeguimientof()
    {
        cameraSeguimiento.transform.position = Player.transform.position + offset;

        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");

        cameraSeguimiento.transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);


    }
}
