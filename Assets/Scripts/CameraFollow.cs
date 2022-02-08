using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform player;
    public float smoothRate;

     Vector3 offset= new Vector3(0,3f,-5f);
    Vector3 velocity = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.position + offset;
    }

    private void Update()
    {
            Vector3 currentPos = transform.position;
        //currentPos.x = 0;
            Vector3 newPos = player.position + offset;
           transform.position= Vector3.SmoothDamp(currentPos, newPos,ref velocity,smoothRate);
        //transform.LookAt(player);
    }


}
