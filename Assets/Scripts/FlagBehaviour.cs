using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBehaviour : MonoBehaviour
{
    public Transform FlagLocator;
    public bool Captured;
    [SerializeField]
    private bool isKnight;

    // Use this for initialization
	void Start ()
    {
        InitFlag();
    }
	
	// Update is called once per frame
	void Update ()
    {

	}

    private void LateUpdate()
    {
        if(transform.position != FlagLocator.position) //
        {
            transform.position = FlagLocator.position;
        }
    }

    public bool isKnightFlag()
    {
        return isKnight;
    }

    public void SetTeam(string teamName)
    {
        if(teamName == "Knight")
        {
            isKnight = true;
        }
        else if(teamName == "Samurai")
        {
            isKnight = false;
        }
    }

    public void InitFlag()
    {
        Captured = false;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        transform.position = FlagLocator.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isKnight)
        {
            if (other.gameObject.GetComponent<FighterSamurai>() != null)
            {
                Captured = true;
                other.gameObject.GetComponent<FighterGeneric>().HasFlag = true;
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
        else
        {
            if (other.gameObject.GetComponent<FighterKnight>() != null)
            {
                Captured = true;
                other.gameObject.GetComponent<FighterGeneric>().HasFlag = true;
                GetComponent<MeshRenderer>().enabled = false;
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(isKnight)
    //    {
    //        if (collision.gameObject.GetComponent<FighterSamurai>() != null)
    //        {
    //            Captured = true;
    //            collision.gameObject.GetComponent<FighterGeneric>().HasFlag = true;
    //            GetComponent<MeshRenderer>().enabled = false;
    //            GetComponent<BoxCollider>().enabled = false;
    //        }
    //    }
    //    else
    //    {
    //        if (collision.gameObject.GetComponent<FighterKnight>() != null)
    //        {
    //            Captured = true;
    //            collision.gameObject.GetComponent<FighterGeneric>().HasFlag = true;
    //            GetComponent<MeshRenderer>().enabled = false;
    //            GetComponent<BoxCollider>().enabled = false;
    //        }
    //    }
            
    //}
}
