using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play(){
        Debug.Log("FUN!");
        ParticleSystem particles = GetComponent<ParticleSystem>();
        particles.Play();
    }
}
