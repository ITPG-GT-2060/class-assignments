using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowController : MonoBehaviour
{

    [SerializeField] GameObject handleTemplate;

    //Handles created clockwise starting top left;
    GameObject[] handles = new GameObject[4];

    Vector3 referenceUp;
    Vector3 referneceTan;

    MeshRenderer renderer;
    MeshFilter filter;
    Mesh mesh;


    // Start is called before the first frame update
    void Start()
    {

        renderer = GetComponent<MeshRenderer>();
        filter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        filter.mesh = mesh;
    }

    public GameObject PlaceFirstHandle(Vector3 worldPos, Vector3 up)
    {

        referenceUp = EstimateUpVec(up);

        for (int i = 0; i < 4; i++)
        {
            GameObject handle = Instantiate(handleTemplate, transform, false);
            handle.transform.position = worldPos;
            handles[i] = handle;
        }

        // Return which handle to move when first placed
        return handles[2];
    }

    

    public void UpdateHandlePosition(GameObject handle, Vector3 position)
    {
        int idx = GetHandleIndex(handle);

        //Find opposite handle:
        int lockedIdx = (idx + 2) % 4;
        GameObject locked = handles[lockedIdx];

        handle.transform.position = position;

        Vector3 diagonal = handle.transform.position - locked.transform.position;

        Vector3 normal = Vector3.Cross(referenceUp, diagonal).normalized;

        Vector3 tangent = Vector3.Cross(referenceUp, normal).normalized;

        //Fix this clunky nonesense
        float direction = Mathf.Sign(Vector3.Dot(normal, Camera.main.transform.forward));
        //direction = direction * Mathf.Sign(Vector3.Dot(referenceUp, locked.transform.position - handle.transform.position));

        // Find adjacent corners
        handles[(idx - 1) % 4].transform.position = locked.transform.position + Vector3.Scale(diagonal, referenceUp);

        handles[(idx + 1) % 4].transform.position = locked.transform.position + Vector3.Scale(diagonal, tangent) * direction;

        UpdateMesh();

    }

    

    public int GetHandlePosition(GameObject obj) {
        if(obj == handles[0])
        {
            return 0;
        }else if (obj == handles[1])
        {
            return 1;
        }
        else if (obj == handles[2])
        {
            return 2;
        }
        else if (obj == handles[3])
        {
            return 3;
        }

        return -1;
    }

    void UpdateMesh()
    {
        /* Four tris (2 per side): 
            0, 1, 3
            1, 2, 3
            0, 3, 1
            1, 3, 2
        */

        Vector3 a = handles[0].transform.position;
        Vector3 b = handles[1].transform.position;
        Vector3 c = handles[2].transform.position;
        Vector3 d = handles[3].transform.position;

        mesh.Clear();

        mesh.vertices = new Vector3[]{a,b,c,d};
        mesh.triangles = new int[] {
            0, 1, 3,
            1, 2, 3,
            0, 3, 1,
            1, 3, 2
        };

        mesh.RecalculateNormals();
    }

    /*
     * 
     * HELPERS
     * 
     */
    private Vector3 EstimateUpVec(Vector3 up)
    {
        Vector3 payload;
        //If up vector biggest, 
        if ((up.y * up.y) > (up.x * up.x + up.z * up.z))
        {
            payload = Vector3.up;
        }

        else if ((up.x * up.x) > (up.y * up.y + up.z * up.z))
        {
            payload = Vector3.right;
        }
        else
        {
            payload = Vector3.forward;
        }

        return payload;
    }

    private int GetHandleIndex(GameObject handle)
    {
        int idx = -1;
        for (int i = 0; i < 4; i++)
        {
            if (handles[i] == handle)
            {
                idx = i;
            }
        }
        return idx;
    }
}
