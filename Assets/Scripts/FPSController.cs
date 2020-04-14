using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public float speed = 10;
    public float angularSpeed = 180;
    CharacterController cc;
    public Animator playerAnimator;
    public AudioSource coinFX;
    static CanvasGroup CanvasOverlay;
    static Transform playerPos;


    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        coinFX = GetComponent<AudioSource>();
        
        GameObject tempObject = GameObject.Find("Canvas");
        if (tempObject != null)
        {
            //If we found the object , get the Canvas component from it.
            CanvasOverlay = tempObject.GetComponent<CanvasGroup>();
            if (CanvasOverlay == null)
            {
                Debug.Log("Could not locate Canvas component on " + tempObject.name);
            }
        }

        CanvasOverlay.gameObject.SetActive(false);

        playerPos = GameObject.Find("PlayerPos").GetComponentInParent<Transform>();
        playerPos.transform.position = gameObject.transform.position;

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

        playerPos.position = transform.position;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.gameObject.tag == "Coin")
        {
            hit.gameObject.SetActive(false);
            coinFX.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "enemy")
        {
            CanvasOverlay.gameObject.SetActive(true);
            float timeToLoadScene = 1;
            Invoke("ResetScene", timeToLoadScene);

        }
    }

    void ResetScene()
    {
        SceneManager.LoadScene("MazeGenerator");
    }

    public float GetVelocity()
    {
        return speed;
    }
}
