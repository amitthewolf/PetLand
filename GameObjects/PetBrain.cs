using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PetBrain : MonoBehaviour
{
    public float wanderRadius;
    public float wanderTimer;
    public GameObject Smiley;
    public GameObject NavNode;
    public Material PetShader;
    [Range(2f,4.5f)]
    public float RunSpeed;
    [Range(1,2f)]
    public float WalkSpeed;
    [Range(4.5f,10f)]
    public float HappySpeed;
    public int Level;
    public Animal animal;
    private Color SadColor = new Color(0.2235f, 0.28235f, 0.5568f);
    private Transform target;
    private NavMeshAgent agent;
    private float timer;
    private Animator Anims;
    private bool Wandering;
    private Randomizer rnd;
    private bool Seeking;
    private bool Puffing;
    private bool IsHappy;
    private float HappyTimer;
    private NavigationNode tempNode;

    // Use this for initialization
    void Awake()
    {
        print("Pet Awaking");
        EventManager.TrashEmptied += Happy;
        EventManager.PetReachedTrashNode += ReachedTrashNode;
        EventManager.PetReachedWanderDestination += ReachedDestination;
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
        Anims = GetComponent<Animator>();
        Wandering = true;
        Puffing = false;
        IsHappy = false;
        Seeking = false;
        rnd = Randomizer.getInstance();
        Level = 1;
    }
    
    private void OnDestroy()
    {
        EventManager.TrashEmptied -= Happy;
        EventManager.PetReachedTrashNode -= ReachedTrashNode;
        EventManager.PetReachedWanderDestination -= ReachedDestination;
    }

    public void setAnimal(Animal animalToSet)
    {
        animal = animalToSet;
        if (animalToSet.getLevel() <= 3 && animalToSet.getLevel()>= 2)
        {
            if(Level+1 == animalToSet.getLevel())
                transform.localScale = transform.localScale * 2f;
            else if(Level+2 == animalToSet.getLevel())
                transform.localScale = transform.localScale * 3f;
        }
        
    }
    
    
    void LateUpdate()
    {
        // SetColor(TrashMaster.TrashCapacity);
        timer += Time.deltaTime;
        if (Wandering || IsHappy)
        {
            if (!Seeking && !Puffing && timer >= wanderTimer)
            {
                Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                NavMeshPath path = new NavMeshPath();
                agent.CalculatePath(newPos, new NavMeshPath());
                while (path.status == NavMeshPathStatus.PathPartial)
                {
                    newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                }
                timer = 0;
                GoTo(newPos);
            }
        }
        if(IsHappy)
            CheckHappy();
    }
    // Update is called once per frame
    void Update()
    { 
        if(agent == null)
            print("Agent is NULLLLLL");
    }

    #region Colorization

    private void SetColor(float pct)
    {
        Color DeltaC = Color.white - SadColor;
        PetShader.color = Color.white - new Color(DeltaC.r*pct,DeltaC.g*pct,DeltaC.b*pct);
    }
    

    #endregion

    #region GameLogic
    public void ReachedTrashNode(object sender, EventArgs args)
    {
        if ((GameObject) sender == gameObject)
        {
            Puff();
            Seeking = false;
            Wandering = false;
            Puffing = true; 
        }
    }
    
    public void SetHappiness(object sender, EventArgs args)
    {
        
    }

    private void Happy(object sender, EventArgs args)
    {
        SpawnSmiley();
        Puffing = false;
        Seeking = false;
        IsHappy = true;
        HappyTimer = Time.time;
        Anims.speed = 1.1f;
        agent.speed = HappySpeed;
        timer = wanderTimer;
    }

    public void SpawnSmiley()
    {
        GameObject smile = Instantiate(Smiley, transform);
        smile.transform.position = new Vector3(0, 0.32f, 0.09f);
        smile.transform.localScale = new Vector3(0.01f, 0.01f, 0f);
    }
    public void LookForTrash()
    {
        EventManager.PetLookingForTrash(this, EventArgs.Empty);
        Seeking = true;
        timer = 0;
    }
    
    #endregion

    #region Navigation

    public void ReachedDestination(object sender, EventArgs args)
    {
        print("Called ReachedDestination");
        if (sender == this.gameObject)
        {
            if (!IsHappy)
            {
                agent.speed = WalkSpeed - 0.5f*WalkSpeed*TrashMaster.TrashCapacity;
                Wandering = true;
                timer = 0;
            }
            Idle();
        }
    }

    public void CheckHappy()
    {
        if (Time.time > HappyTimer + 10f)
        {
            print("Stopped Happy");
            IsHappy = false;
            if (!Seeking || !Puffing)
                Wandering = true;
        }
        
    }

    public void WalkTo(GameObject Target)
    {
        print("Called Walk");
        agent.speed = WalkSpeed - 0.5f*WalkSpeed*TrashMaster.TrashCapacity;
        walk();
        agent.SetDestination(Target.transform.position);
        Wandering = false;
    }

    public void WalkTo(Vector3 Targetlocation)
    {
        print("Called Walk");
        agent.speed = WalkSpeed - 0.5f*WalkSpeed*TrashMaster.TrashCapacity;
        CreateDestinationNode(Targetlocation);
        walk();
        agent.SetDestination(Targetlocation);
        Wandering = false;
    }

    private void CreateDestinationNode(Vector3 Targetlocation)
    {
        if(tempNode != null)
            Destroy(tempNode.gameObject);
        tempNode = Instantiate(NavNode).GetComponent<NavigationNode>();
        tempNode.transform.position = Targetlocation;
        tempNode.SetIncomingPet(this.gameObject);
    }

    public void GoTo(GameObject Target)
    {
        if (IsHappy)
        {
            JumpTo(Target);
                    
        }
        else if (Seeking)
        {
            RunTo(Target);
        }
        else
        {
            if(rnd == null)
                rnd = Randomizer.getInstance();
            int Random = rnd.getRandom(2);
            if (Random == 1)
                WalkTo(Target);
            else
                RunTo(Target);
        }
    }
    
    public void GoTo(Vector3 newPos)
    {
        if (IsHappy)
        {
            JumpTo(newPos);
                    
        }
        else
        {
            if(rnd == null)
                rnd = Randomizer.getInstance();
            int Random = rnd.getRandom(2);
            if (Random == 1)
                WalkTo(newPos);
            else
                RunTo(newPos);
        }
    }
    
    public void JumpTo(Vector3 Targetlocation)
    {
        print("Called Jump");
        CreateDestinationNode(Targetlocation);
        Jump();
        agent.SetDestination(Targetlocation);
        Wandering = false;
    }

    public void JumpTo(GameObject Target)
    {
        print("Called Jump");
        agent.speed =RunSpeed - 0.5f*RunSpeed*TrashMaster.TrashCapacity;
        Jump();
        agent.SetDestination(Target.transform.position);
        Wandering = false;
    }

    public void RunTo(GameObject Target)
    {
        print("Called Run");
        agent.speed =RunSpeed - 0.5f*RunSpeed*TrashMaster.TrashCapacity;
        Run();
        agent.SetDestination(Target.transform.position);
        Wandering = false;
    }

    public void RunTo(Vector3 Targetlocation)
    {
        print("Called Run");
        CreateDestinationNode(Targetlocation);
        agent.speed = RunSpeed - 0.5f*RunSpeed*TrashMaster.TrashCapacity;
        Run();
        agent.SetDestination(Targetlocation);
        Wandering = false;
    }

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);
        Vector3 ToReturn = new Vector3(
            Mathf.Clamp(navHit.position.x, -18f, 19f),
            navHit.position.y,
            Mathf.Clamp(navHit.position.z, -21f, 17));
        return ToReturn;
    }

    #endregion

    #region Animations

    public void Run()
    {
        // print("Calling Run");
        Anims.SetInteger("animation", 2);
    }

    public void walk()
    {
        // print("Calling Walk");
        Anims.SetInteger("animation", 1);
    }

    public void Puff()
    {
        Anims.SetInteger("animation", 7);
    }

    public void Idle()
    {
        // print("Calling Idle");
        Anims.SetInteger("animation", 0);
    }

    public void Jump()
    {
        Anims.SetInteger("animation", 3);
    }

    public void Eat()
    {
        Anims.SetInteger("animation", 1);
    }
    #endregion


    
}
