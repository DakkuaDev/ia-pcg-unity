using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public float speed = 10;
    CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        var hztal = Input.GetAxis("Horizontal");
        var vtcal = Input.GetAxis("Vertical");

        cc.SimpleMove(new Vector3(hztal,0,vtcal) * speed);
    }
}
