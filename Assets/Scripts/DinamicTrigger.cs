using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DinamicTrigger : MonoBehaviour
{
    static Seek EnemySeekScript;
    // Start is called before the first frame update
    void Start()
    {

        EnemySeekScript = GetComponent<Seek>();
      //  EnemySeekScript.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            this.gameObject.GetComponent<Seek>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            this.gameObject.GetComponent<Seek>().enabled = false;
        }
    }
}
