using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TheEntity : MonoBehaviour
{
    public Transform[] waypoints;
    private int currentWaypointIndex = 0;
    private NavMeshAgent agent;

    [SerializeField] private float chaseDistance = 5f;
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float loseChaseDistance = 10f;
    [SerializeField] private float chaseSpeed = 3f; // Increased chase speed
    [SerializeField] private float normalSpeed = 1f; // Normal wandering speed
    [SerializeField] private AudioClip footstepSfx;
    [SerializeField] private AudioClip chaseSfx;
    [SerializeField] private AudioClip screamSfx; 

    private AudioSource audioSource; // Main audio source for footsteps and scream
    private AudioSource chaseAudioSource; // Audio source for chase music
    private bool isChasing = false;
    private bool isPausing = false;
    private Transform player;
    private Animator runningCrawlAnimator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        chaseAudioSource = transform.Find("ChaseAudioSource").GetComponent<AudioSource>(); // Find the child audio source
        player = GameObject.FindGameObjectWithTag("Player").transform;
        runningCrawlAnimator = transform.Find("Running Crawl").GetComponent<Animator>();
        agent.speed = normalSpeed; // Set the initial speed to normal
        GoToNextWaypoint();
    }

    void Update()
    {
        if (isChasing && !isPausing)
        {
            ChasePlayer();
        }
        else if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextWaypoint();
        }

        CheckForPlayer();
    }

    void GoToNextWaypoint()
    {
        currentWaypointIndex = Random.Range(0, waypoints.Length);
        agent.destination = waypoints[currentWaypointIndex].position;

        if (Random.value > 0.7f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, 1))
            {
                agent.SetDestination(hit.position);
            }
        }

        PlayFootstepAudio();
    }

    void CheckForPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float horizontalDistance = Vector3.Distance(transform.position, player.position);
        float verticalDistance = Mathf.Abs(player.position.y - transform.position.y);

        // Draw the ray in the scene view for debugging
        Debug.DrawRay(transform.position, directionToPlayer * chaseDistance, Color.red);

        if (Vector3.Dot(transform.forward, directionToPlayer) > 0)
        {
            if (horizontalDistance < chaseDistance && verticalDistance < 1.0f)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        if (!isChasing)
                        {
                            isChasing = true;
                            StartCoroutine(StartChaseSequence());
                        }
                    }
                }
            }
        }

        if (isChasing && (horizontalDistance > loseChaseDistance || verticalDistance >= 1.0f))
        {
            isChasing = false;
            StartCoroutine(FadeOutChaseAudio(1f));
            agent.speed = normalSpeed; // Return to normal speed when not chasing
            GoToNextWaypoint();
        }
    }

    IEnumerator StartChaseSequence()
    {
        isPausing = true;
        agent.isStopped = true; // Pause the entity's movement

        // Stop the animation
        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.enabled = false; // Disable the animator
        }

        if (screamSfx != null)
        {
            audioSource.PlayOneShot(screamSfx); // Play scream audio at full volume
        }

        yield return new WaitForSeconds(1f); // Pause for 1 second

        // Resume the animation
        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.enabled = true; // Re-enable the animator
        }

        agent.isStopped = false; // Resume movement
        agent.speed = chaseSpeed; // Increase the chase speed

        ResetChaseAudio();  // Ensure the chase audio settings are reset
        PlayChaseAudio();   // Play the chase audio

        isPausing = false;
    }

    void ChasePlayer()
    {
        agent.destination = player.position;

        if (!chaseAudioSource.isPlaying)
        {
            PlayChaseAudio();
        }
    }

    void PlayFootstepAudio()
    {
        if (footstepSfx && !isChasing && !isPausing)
        {
            if (audioSource.clip != footstepSfx)
            {
                audioSource.clip = footstepSfx;
                audioSource.loop = true;
                audioSource.volume = 0.5f;
                audioSource.Play();
            }
        }
    }

    void PlayChaseAudio()
    {
        if (chaseSfx && isChasing && !isPausing)
        {
            if (chaseAudioSource.clip != chaseSfx)
            {
                chaseAudioSource.clip = chaseSfx;
                chaseAudioSource.loop = true;
            }
            chaseAudioSource.Play();
        }
    }

    void ResetChaseAudio()
    {
        chaseAudioSource.volume = 1f; // Reset the volume to full before the chase starts
    }

    IEnumerator FadeOutChaseAudio(float fadeDuration)
    {
        float startVolume = chaseAudioSource.volume;

        while (chaseAudioSource.volume > 0)
        {
            chaseAudioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        chaseAudioSource.Stop(); // Stop the chase music after fading out
        chaseAudioSource.volume = startVolume; // Reset volume for next time
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDeathScene();
        }
        else if (other.CompareTag("Door"))
        {
            Door door = other.GetComponent<Door>();
            if (door != null)
            {
                door.ToggleDoor();
            }
        }
    }

    void OpenDeathScene()
    {
        SceneManager.LoadScene("Death");
    }
}
