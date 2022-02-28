using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    Camera parentCam;
    float speed = 5;
    float zoomSpeed = 3;
    void Awake(){
        parentCam = gameObject.GetComponent<Camera>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow)){
            parentCam.transform.position = parentCam.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow)){
            parentCam.transform.position = parentCam.transform.position += new Vector3(-1 * speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow)){
            parentCam.transform.position = parentCam.transform.position += new Vector3(0, -1 * speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            parentCam.transform.position = parentCam.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.T)){
            parentCam.orthographicSize += zoomSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.G)){
            parentCam.orthographicSize -= zoomSpeed * Time.deltaTime;
        }
    }
}
