using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrow : MonoBehaviour
{
    public float m_punchForce = 0.2f;
    public Rigidbody arw;
    public Transform tip;
    public Transform target;
    private int state;

    // Arrow specific
    Vector3 position;
    Quaternion rotation;
    private Collisioner col;
    private Transform parent;

    // Use this for initialization
    void Start()
    {
        arw = GetComponent<Rigidbody>();
        tip = GameObject.Find("Tip").transform;
        target = GameObject.Find("Target").transform;
        state = 0;

        rotation = arw.transform.localRotation;
        position = arw.transform.localPosition;
        col = new Collisioner(tip, target);
        parent = arw.transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        // Reset by pressing r
        if (Input.GetKeyDown("r"))
        {
            Reset();
        }
        else
        {
            col.Update();
            if (state == 0) // if we havent shot the arrow
            {
                if (Input.GetKeyDown("space"))
                {
                    arw.isKinematic = false;
                    Shoot();
                    state = 1;
                }
            }
            if (col.colliding) // if it is collidin
            {
                arw.isKinematic = true;
                Debug.Log("Colliding");
                state = 2;
            }
        }
    }

	//Resets scene
	void Reset()
	{
        arw.transform.SetParent(parent);
        arw.transform.localPosition = position;
        arw.transform.localRotation = rotation;
        arw.velocity = Vector3.zero;
        arw.angularVelocity = Vector3.zero;
        arw.isKinematic = true;
        state = 0;
    }
    void Shoot() {
        arw.transform.SetParent(null);
        arw.AddForce(-arw.transform.up * m_punchForce); // weird rotation on the arrow
    }
}
