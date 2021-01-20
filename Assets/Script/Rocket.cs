using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust;
    [SerializeField] float mainThrust;
    [SerializeField] float LevelLoadDelay;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelLoadScreen;
    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem sucessParticle;
    [SerializeField] ParticleSystem deathParicle;
    Rigidbody rigidbody;
    AudioSource audioSource;
    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    bool collisionsDisabled = true;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToThrustInput();
            RespondToRotateInput();
            RespondToExit();
        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
    }
    private void RespondToExit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisionsDisabled) { return; }

        if (collision.gameObject.tag == "Friendly")
        {
            Sucess();
        }
        else if (collision.gameObject.tag == "Lava" || collision.gameObject.tag == "Enemy")
        {
            DeathSystem();
            audioSource.PlayOneShot(deathSound);
            deathParicle.Play();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "EndGame")
        {
            DeathSystem();
        }
    }
    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Thrust();
        }
        else
        {
            ThrustEnd();
        }
    }
    private void RespondToRotateInput()
    {
        rigidbody.freezeRotation = true;

        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidbody.freezeRotation = false;
    }
    private void Sucess()
    {
        state = State.Transcending;
        Invoke("LoadNextScene", LevelLoadDelay);
        audioSource.PlayOneShot(levelLoadScreen);
        sucessParticle.Play();
    }
    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
    }
    private void DeathSystem()
    {
        state = State.Dying;
        Invoke("ResetOnLoad", LevelLoadDelay);
    }
    private void ResetOnLoad()
    {
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }
    private void Thrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticle.Play();
    }
    private void ThrustEnd()
    {
        audioSource.Stop();
        mainEngineParticle.Stop();
    }
}
