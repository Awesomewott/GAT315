using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class World : MonoBehaviour
{
    public BoolData simulate;
    public BoolData collision;
    public BoolData wrap;
    public BoolData debugCollision;

    public FloatData gravity;
    public FloatData gravitation;
    public FloatData fixedFPS;
    public StringData fpsText;
    public BroadPhaseTypeData broadPhaseType;
    public VectorField vectorField;
    public TMP_Text valueText = null;
    public StringData collisionText;

    BroadPhase broadPhase;
    BroadPhase[] broadPhases = { new NullBroadPhase(), new QuadTree(), new BVH() };

    private Vector2 size;
    float fps = 0;

    static World instance;

    public float timeAccumaltor;
    public float fpsAverage;
    public float smoothing = 0.95f;
    public float fixedDeltaTime { get { return 1.0f / fixedFPS.value; } }
    static public World Instance { get { return instance; } }

    public Vector2 Gravity { get { return new Vector2(0, gravity.value); } }
    public List<Body> bodies { get; set; } = new List<Body>();
    public List<Spring> springs { get; set; } = new List<Spring>();
    public List<Force> forces { get; set; } = new List<Force>();

    public AABB AABB { get => aabb; }

    AABB aabb;

    private void Awake()
    {
        instance = this;
        size = Camera.main.ViewportToWorldPoint(Vector2.one);
        aabb = new AABB(Vector2.zero, size * 2);
    }

    void Update()
    {
        broadPhase = broadPhases[broadPhaseType.index];
        springs.ForEach(spring => spring.Draw());
        if (!simulate.value)
        {
            return;
        }
        float dt = Time.deltaTime;
        fps = (1.0f / dt);
        fpsAverage = (fpsAverage * smoothing) + (fps * (1 - smoothing));
        valueText.text = fixedFPS.value.ToString("F2");

        timeAccumaltor += Time.deltaTime;

        //forces 
        GravatationalForce.ApplyForce(bodies, gravitation.value);
        forces.ForEach(force => bodies.ForEach(body => force.ApplyForce(body)));
        springs.ForEach(spring => spring.ApplyForce());
        bodies.ForEach(body => vectorField.ApplyForce(body));

        while (timeAccumaltor > fixedDeltaTime) 
        {
            bodies.ForEach(body => body.Step(fixedDeltaTime));
            bodies.ForEach(body => Intergrator.SemiEular(body, fixedDeltaTime));//ExplicitEuler(body, dt));

           if(collision)
            {
                bodies.ForEach(body => body.shape.color = Color.green);
                broadPhase.Build(aabb, bodies);

                Collison.CreateBroadPhaseContacts(broadPhase, bodies, out List<Contact> contacts);
                Collison.CreateNarrowPhaseContacts(ref contacts);
                contacts.ForEach(contact => Collison.UpdateContactInfo(ref contact));

                //Collison.CreateContacts(bodies, out List<Contact> contacts);
                ContactSolver.Resolve(contacts);

                contacts.ForEach(contact => { contact.bodyA.shape.color = Color.red; contact.bodyB.shape.color = Color.red; });

            }
            //broadPhase.Draw();
            timeAccumaltor = timeAccumaltor - fixedDeltaTime; 
        }
        if (debugCollision)
        {
            broadPhase.Draw();
        }
        collisionText.value = "Broad Phase: " + BroadPhase.potientialCollisionCount.ToString();
        
        if (wrap) 
        { 
            bodies.ForEach(body => body.position = Utilities.Wrap(body.position, -size, size));
        }

        bodies.ForEach(body => body.force = Vector2.zero);
        bodies.ForEach(body => body.acceleration = Vector2.zero);
    }
}
