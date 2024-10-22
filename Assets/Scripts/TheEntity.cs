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
    [SerializeField] private float proximityChaseDistance = 2f; // Horizontal proximity for chase activation
    [SerializeField] private float loseChaseDistance = 10f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float normalSpeed = 1f;
    [SerializeField] private AudioClip footstepSfx;
    [SerializeField] private AudioClip chaseSfx;
    [SerializeField] private AudioClip screamSfx;

    private AudioSource audioSource;
    private AudioSource chaseAudioSource;
    private bool isChasing = false;
    private bool isPausing = false;
    private bool lastChase = false;
    private Transform player;
    private Animator runningCrawlAnimator;

    private float originalAnimatorSpeed = 1f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        chaseAudioSource = transform.Find("ChaseAudioSource").GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        runningCrawlAnimator = transform.Find("Running Crawl").GetComponent<Animator>();
        agent.speed = normalSpeed;
        GoToNextWaypoint();

        if (runningCrawlAnimator != null)
        {
            originalAnimatorSpeed = runningCrawlAnimator.speed;
        }
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

        PlayFootstepAudio();
    }



    void CheckForPlayer()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position); // Full 3D distance
        float verticalDistanceToPlayer = Mathf.Abs(player.position.y - transform.position.y); // Vertical distance only

        // Draw the ray in the scene view for debugging
        Debug.DrawRay(transform.position, directionToPlayer * chaseDistance, Color.red);

        // Proximity-based chase (using raycast to ensure no walls between entity and player)
        if (distanceToPlayer <= proximityChaseDistance && verticalDistanceToPlayer <= 2.5f && !isChasing)
        {
            RaycastHit hit;
            // Check if there's a line of sight to the player
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, proximityChaseDistance))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    StartChase();
                }
            }
        }
        // Vision-based chase using line of sight and full distance, but add a vertical constraint
        else if (Vector3.Dot(transform.forward, directionToPlayer) > 0 && distanceToPlayer < chaseDistance && verticalDistanceToPlayer <= 2.5f)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, chaseDistance))
            {
                if (hit.collider.CompareTag("Player") && !isChasing)
                {
                    StartChase();
                }
            }
        }

        // Stop chase if player escapes (again, use full 3D distance + vertical check)
        if (isChasing && (distanceToPlayer > loseChaseDistance || verticalDistanceToPlayer > 2.5f) && lastChase == false)
        {
            StopChase();
        }
    }

    void StartChase()
    {
        isChasing = true;
        StartCoroutine(StartChaseSequence());
    }

    void StopChase()
    {
        isChasing = false;
        StartCoroutine(FadeOutChaseAudio(1f));
        agent.speed = normalSpeed;

        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.speed = originalAnimatorSpeed;
        }
        GoToNextWaypoint();
    }

    public void StopEntity() //final cutscene
    {
        // Stop chasing the player
        StopChase();

        // Set the agent speed to 0
        agent.speed = 0f;

        // Stop any running animation
        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.SetFloat("Speed", 0); // Assuming you use a parameter called "Speed" in your Animator
            runningCrawlAnimator.SetBool("IsMoving", false); // Assuming you have a boolean parameter for movement
        }

        Debug.Log("Entity has been stopped.");
    }

    IEnumerator StartChaseSequence()
    {
        isPausing = true;
        agent.isStopped = true;

        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.enabled = false;
        }

        if (screamSfx != null)
        {
            audioSource.PlayOneShot(screamSfx);
        }

        yield return new WaitForSeconds(1f); // Pause for dramatic effect

        if (runningCrawlAnimator != null)
        {
            runningCrawlAnimator.enabled = true;

            // Increase animation speed based on chase type
            if (lastChase)
            {
                runningCrawlAnimator.speed = originalAnimatorSpeed * 5f; // Faster animation for last chase
            }
            else
            {
                runningCrawlAnimator.speed = originalAnimatorSpeed * 3f; // Standard chase speed
            }
        }

        agent.isStopped = false;

        // Increase movement speed based on chase type
        if (lastChase)
        {
            agent.speed = chaseSpeed * 2f; // Faster speed for last chase
        }
        else
        {
            agent.speed = chaseSpeed; // Standard chase speed
        }

        ResetChaseAudio();
        PlayChaseAudio();

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
        chaseAudioSource.volume = 1f;
    }

    IEnumerator FadeOutChaseAudio(float fadeDuration)
    {
        float startVolume = chaseAudioSource.volume;

        while (chaseAudioSource.volume > 0)
        {
            chaseAudioSource.volume -= startVolume * Time.deltaTime / fadeDuration;
            yield return null;
        }

        chaseAudioSource.Stop();
        chaseAudioSource.volume = startVolume;
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
                door.ToggleDoor(); // Interact with the door
            }
        }
    }

    void OpenDeathScene()
    {
        SceneManager.LoadScene("Death");
    }

    public void ForceChasePlayer()
    {
        if (!isChasing) // Only start chase if not already chasing
        {
            lastChase = true;
            StartChase();
        }
    }
}
