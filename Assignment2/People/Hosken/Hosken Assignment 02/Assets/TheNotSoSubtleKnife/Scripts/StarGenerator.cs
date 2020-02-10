using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarGenerator : MonoBehaviour
{

    [SerializeField] GameObject star;
    [SerializeField] int numberOfStars = 50;
    [SerializeField] float XZBounds = 500;
    [SerializeField] float yBounds = 50;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < numberOfStars; i++) {
            Vector3 vec = Random.insideUnitSphere;
            vec = new Vector3(vec.x * XZBounds, vec.y * yBounds, vec.z * XZBounds) + transform.position;

            GameObject cln = Instantiate(star, vec, Quaternion.identity);
            cln.transform.localScale = Vector3.one * Random.Range(1, 3);
            cln.transform.parent = transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
