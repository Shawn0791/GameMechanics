using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOverlap : MonoBehaviour
{
    public bool isOverlap;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        isOverlap = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        isOverlap = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isOverlap = false;
    }
}
