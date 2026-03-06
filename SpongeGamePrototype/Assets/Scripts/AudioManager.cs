using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public AudioSource playerAudio;
    public List<AudioClip> audioClips;
    //public GameObject player;
    public  StarterAssets.FirstPersonController playerScript;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        //playerAudio.PlayOneShot(audioClips[0]); 
        //PlayAudio(0);
    }

    private void Update()
    {
        if(playerScript._speed > 0)
        {
            if (!playerAudio.isPlaying)
            {
                PlayAudio(Random.Range(1, 3),0.25f);
            }
            
        }

        if (playerScript.isSpongePress())
        {
            PlayAudio(0,1);
        }
		
    }

    public void PlayAudio(int clip, float volume)
    {
        playerAudio.PlayOneShot(audioClips[clip]);
        playerAudio.volume = volume;
        playerAudio.pitch = Random.Range(0.5f, 2);
        Debug.Log("test audio");
    }
}
