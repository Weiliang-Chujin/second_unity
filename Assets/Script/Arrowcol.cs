using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrowcol : MonoBehaviour
{
    public static bool flag;
    // Start is called before the first frame update
    void Start()
    {
        flag = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "enemy")
        {
            flag = true;
        }
    }
}
