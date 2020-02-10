using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.XR.ARFoundation;

public class WindowFactory : MonoBehaviour
{

    ARRaycastManager raycastManager;
    List<ARRaycastHit> rayHits = new List<ARRaycastHit>();

    [SerializeField] GameObject windowTemplate;

    Camera cam;

    GameObject selectedHandle = null;
    WindowController selectedWindowController = null;
    Vector3 selectedHandleOffset = Vector3.zero;

    int previousTouchCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchSupported)
        {

            HandleTouch();
            
        }
    }

    private void HandleTouch()
    {
        if (previousTouchCount == 0 && Input.touchCount > 0)
        {

            Debug.Log("NEW TOUCH");
            //New touch
            HandleNewTouch();
        }
        else if (previousTouchCount > 0 && Input.touchCount > 0)
        {

            Debug.Log("TOUCH SUSTAINED");
            //Sustained touch
            HandleSustainedTouch();
        }
        else if(previousTouchCount > 0 && Input.touchCount == 0)
        {

            Debug.Log("TOUCH RELEASED");
            //Released touch
            //selectedHandle = null;
            //selectedWindowController = null;
            //selectedHandleOffset = Vector3.zero;
        }

        previousTouchCount = Input.touchCount;
    }
    private void HandleNewTouch()
    {
        Ray ray = cam.ScreenPointToRay(Input.GetTouch(0).position);
        RaycastHit hit;


        bool selectedHit = false;

        if(Physics.Raycast(ray, out hit, 200))
        {
            //If touching existing handle:
            if (hit.collider.gameObject.tag == "Handle")
            {

                //Store object hit:
                selectedHandle = hit.collider.gameObject;
                //Store ref to parent windowcontroller:
                selectedWindowController = selectedHandle.GetComponentInParent<WindowController>();
                //Store where on the object you hit:
                selectedHandleOffset = selectedHandle.transform.position - hit.point;
                //Flag to avoid creating new window:
                selectedHit = true;

                return;
            }
        }
        

        if(!selectedHit)
        {
            // If not touching existing handle
            CreateNewWindow();
        }
    }

    

    private void HandleSustainedTouch()
    {
        Debug.Log("Casting AR Ray");
        raycastManager.Raycast(Input.GetTouch(0).position, rayHits);
        ARRaycastHit ar_hit = rayHits[0];

        Debug.Log("Adjusting for selectedHandleOffset");
        //Take into account where we touched the raycast collider when moving:
        Vector3 newHandlePosition = ar_hit.pose.position + selectedHandleOffset;

        Debug.Log("Updating Handle Position");
        selectedWindowController.UpdateHandlePosition(selectedHandle, newHandlePosition);

        Debug.Log("Updated Handle Position Successfully");
        rayHits.Clear();
    }


    private void CreateNewWindow()
    {
        Debug.Log("Creating New Window");
        raycastManager.Raycast(Input.GetTouch(0).position, rayHits);
        ARRaycastHit ar_hit = rayHits[0];

        GameObject newWindow = Instantiate(windowTemplate, transform, true);
        WindowController controller = newWindow.GetComponent<WindowController>();
        GameObject handle = controller.PlaceFirstHandle(ar_hit.pose.position, cam.transform.up);

        selectedHandle = handle;
        selectedWindowController = controller;
        selectedHandleOffset = Vector3.zero;
        rayHits.Clear();
    }


}
