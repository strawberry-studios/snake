using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Is created once and never destroyed. Can be found with its tag "SoundController" and provides sound-playing methods. There always only is one 
/// sound object.
/// </summary>
public class SoundController : MonoBehaviour
{
    /// <summary>
    /// The audio clip which should be player when audio is activated.
    /// </summary>
    public AudioClip activateSounds;
    /// <summary>
    /// The clip which should be played when the game is lost.
    /// </summary>
    public AudioClip gameOverSound;
    /// <summary>
    /// The clip that should be played if a player seriously reaches a 100%.
    /// </summary>
    public AudioClip gameWon;
    /// <summary>
    /// An array of audio clips of which one is played if an apple is collected.
    /// Audios can be selected manually if the full version was purchased.
    /// </summary>
    public AudioClip[] appleCollectedSound;

    private void Start()
    {
        if (GameObject.FindGameObjectsWithTag("SoundController").Length < 2)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Plays the 'activateSounds' sound clip.
    /// </summary>
    public void PlayActivateSoundsClip()
    {
        GetComponent<AudioSource>().clip = activateSounds;
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Plays the game over (and lost) sound.
    /// </summary>
    public void PlayGameOverSound()
    {
        GetComponent<AudioSource>().clip = gameOverSound;
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Plays the game won sound. 
    /// </summary>
    public void PlayGameWonSound()
    {
        GetComponent<AudioSource>().clip = gameWon;
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Plays the currently saved 'collectAppleSound'. The sound is retrieved from an external file.
    /// </summary>
    public void PlayAppleCollected()
    {
        GetComponent<AudioSource>().clip = appleCollectedSound[DataSaver.Instance.currentCollectAppleSound];
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Plays the 'collectAppleSound' with the passed index. (Indecex ranging from 0 to 5).
    /// </summary>
    /// <param name="index">The index of the sound as int to be played (ranging from 0 to 5).</param>
    public void PlayAppleCollected(int index)
    {
        GetComponent<AudioSource>().clip = appleCollectedSound[index];
        GetComponent<AudioSource>().Play();
    }
}
