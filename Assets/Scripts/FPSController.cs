using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public float speed = 10;
    public float angularSpeed = 180;
    CharacterController cc;
    public Animator playerAnimator;
    public AudioSource coinFX;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        coinFX = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
        var hztal = Input.GetAxis("Horizontal");
        var vtcal = Input.GetAxis("Vertical");

        cc.SimpleMove(transform.forward * vtcal * speed);
        transform.Rotate(Vector3.up * hztal * angularSpeed * Time.deltaTime);

        //Debug.Log(cc.velocity.magnitude);

        // Character Animations
        if(cc.velocity.magnitude > 0)
        {
            playerAnimator.SetBool("Run", true);
        }
        else
        {
            playerAnimator.SetBool("Run", false);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Coin")
        {
            hit.gameObject.SetActive(false);
            coinFX.Play();
        }
    }
}
