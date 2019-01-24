using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent nav;
    Transform player;
    Health health;
    PlayerProperties playerHealth;
    RoomManager rm;

    public enum State { Wander, Patrol, Attack };
    public State state;

    public bool doesPatrol;

    public int aggroRange = 15;
    public float attackRange = 2;
    public int attackDamage = 10;
    public int attackFrequency = 5;
    public float walkSpeed = 2;
    public float runSpeed = 4;

    Animator anim;

    float wanderEnterTime;
    float wanderDuration;

    int roomNumber = -1;

    void Start()
    {
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        health = GetComponent<Health>();
        if (playerObject)
        {
            player = playerObject.transform;
            playerHealth = GameObject.FindObjectOfType<PlayerProperties>();
        }

        nav.speed = walkSpeed;

        rm = GameObject.FindObjectOfType<RoomManager>();
        roomNumber = rm.GetClosestRoom(transform.position);

        //assumes enemys start in rooms and then make their way to others
        state = State.Wander;

        anim = GetComponent<Animator>();

        StartCoroutine(FSM());
    }

    void Update()
    {
        /*if (roomNumber != -1)
            Debug.DrawLine(rm.GetRoomPosition(roomNumber), rm.GetRoomPosition(roomNumber) + Vector3.up * 5, Color.blue);
        Debug.DrawLine(nav.destination, nav.destination + Vector3.up * 3, Color.yellow);*/
    }

    IEnumerator FSM()
    {
        yield return new WaitForSeconds(5);
        //[EnemyHealth]
        while (health.isAlive)
        {
            yield return StartCoroutine(state.ToString());
        }
    }

    /*
     * States at this time include:
     *      Wander:
     *          Enemy is in some room or defined area and is shambling about doing zombie things.
     *          Enemy sets a timer randomly of how long he will stay wandering
     *          State ends when the timer runs out.
     *          Without player interaction, initiates Patrol state
     *      Patrol:
     *          Enemy chooses a room to go to and makes his way through hallways towards that room.
     *          State ends when the enemy has reached its destination.
     *          Without player interaction, initiates Wander in new room state
     *      Attack:
     *          Enemy has seen or observed the player in some way and is making his way to attack the player.
     *          State can be activated from Wander and Patrol states and will resume previous actions if state is ended.
     *          Not fully implemented as combat is expected to be further outlined
     * 
     * Each state is structured as:
     * 
     *      State Initialization (called once when the state is entered)
     * 
     *      Loop of repeating behaviour (exits based on conditions set in initialization)
     * 
     *      State Exit (called once when the state is finished)
     */

    IEnumerator Wander()
    {
        //Debug.Log("Entering state: " + state.ToString());
        float wanderDuration = Random.Range(10, 20);
        float loopSpeed = 1;
        bool foundPlayer = false;
        yield return null;

        while (wanderDuration > 0 || !doesPatrol && health.isAlive)
        {
            anim.SetFloat("Speed", nav.velocity.magnitude / 4);
            if (CanSeeValidPlayer())
            {
                state = State.Attack;
                foundPlayer = true;
                break;
            }
            if (nav.remainingDistance < 0.5f)
            {
                nav.SetDestination(GetNextWanderDestination());
            }
            wanderDuration -= loopSpeed;
            yield return new WaitForSeconds(loopSpeed);
        }

        //Debug.Log("Leaving state: " + state.ToString());
        if (!foundPlayer)
            state = State.Patrol;
    }

    IEnumerator Patrol()
    {
        //Debug.Log("Entering state: " + state.ToString());
        float loopSpeed = 1;
        nav.SetDestination(GetNextPatrolDestination());
        bool foundPlayer = false;
        yield return null;

        while (nav.remainingDistance > 0.1f)
        {
            if (CanSeeValidPlayer())
            {
                state = State.Attack;
                foundPlayer = true;
                break;
            }
            yield return new WaitForSeconds(loopSpeed);
        }

        //Debug.Log("Leaving state: " + state.ToString());
        if (!foundPlayer)
            state = State.Wander;
    }
    /*
     * attack range
     * attack speed
     * 
     */

    IEnumerator Attack()
    {
        //Debug.Log("Entering state: " + state.ToString());
        float loopSpeed = 0.25f;
        bool canSeeValidPlayer = true;
        float lastAttack = 0;
        nav.speed = runSpeed;
        yield return null;

        while (canSeeValidPlayer && health.isAlive)
        {
            anim.SetFloat("Speed", nav.velocity.magnitude / 4);
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > attackRange)
            {
                anim.SetBool("Attack", false);
                nav.destination = player.transform.position + (transform.position - player.transform.position).normalized * attackRange * 0.9f;
            }
            else if (Time.time - lastAttack > attackFrequency)
            {
                //attack animation
                anim.SetBool("Attack", true);
                //[PlayerHealth]
                playerHealth.TakeDamage(attackDamage);
                lastAttack = Time.time;
            }
            canSeeValidPlayer = CanSeeValidPlayer(distance);
            yield return new WaitForSeconds(loopSpeed);
        }

        //Debug.Log("Leaving state: " + state.ToString());
        anim.SetBool("Attack", false);
        nav.speed = walkSpeed;
        state = State.Wander;
    }

    //Player detection! This should be improved!
    bool CanSeeValidPlayer(float range)
    {
        if (!player)
            return false;

        //[PlayerHealth]
        if (playerHealth.health <= 0)
            return false;

        if (range < aggroRange)
        {
            return true;
        }
        return false;
    }
    bool CanSeeValidPlayer()
    {
        if (player)
            return CanSeeValidPlayer(Vector3.Distance(player.position, transform.position));
        else
            return false;
    }
    Vector3 GetNewCircularDestination(Vector3 center, float range)
    {
        Vector3 final = center;
        float x = Random.Range(-range, range);
        float z = Random.Range(-range, range);
        final.x += x;
        final.z += z;

        return final;
    }
    //These are temporary! They will eventually go to a gamemanager that has all the patrol points
    public Vector3 GetNextWanderDestination()
    {
        float maxRoomSize = rm.MeasureLargestRoomAxis(roomNumber);
        Vector3 nextDestination = rm.GetRandomPositionInRoom(roomNumber);
        UnityEngine.AI.NavMeshHit hit;
        UnityEngine.AI.NavMesh.SamplePosition(nextDestination, out hit, maxRoomSize, 1);
        nextDestination = hit.position;
        return nextDestination;
    }
    public Vector3 GetNextPatrolDestination()
    {
        //pick a room to go to, this should take into account people in room
        return rm.GetRoomPosition(roomNumber);
    }
    void OnHit()
    {
        //fire hurt animation
    }
    void OnDeath()
    {
        anim.SetBool("Alive", false);
        nav.SetDestination(transform.position);
        rm.SpawnEnemy();
        Destroy(gameObject, 20); //remove this if ragdolls are implemented
    }
}
