using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class World : MonoBehaviour
{
    public BoolData simulate;
    public BoolData collision;
    public BoolData wrap;
    public FloatData gravity;
    public FloatData gravitation;
    public FloatData fixedFPS;
    public StringData fpsText;
    public TMP_Text valueText = null;

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

    private void Awake()
    {
        instance = this;
        size = Camera.main.ViewportToWorldPoint(Vector2.one);
    }

    void Update()
    {
        if (!simulate.value)
        {
            return;
        }
        float dt = Time.deltaTime;
        fps = (1.0f / dt);
        fpsAverage = (fpsAverage * smoothing) + (fps * (1 - smoothing));
        valueText.text = fixedFPS.value.ToString("F2");


        timeAccumaltor += Time.deltaTime;

        GravatationalForce.ApplyForce(bodies, gravitation.value);

        while (timeAccumaltor > fixedDeltaTime) 
        {
            bodies.ForEach(body => body.Step(fixedDeltaTime));
            bodies.ForEach(body => Intergrator.SemiEular(body, fixedDeltaTime));//ExplicitEuler(body, dt));
            bodies.ForEach(body => body.shape.color = Color.green);

           if(collision == true)
            {
                Collison.CreateContacts(bodies, out List<Contact> contacts);
                contacts.ForEach(contact => { contact.bodyA.shape.color = Color.red; contact.bodyB.shape.color = Color.red; });
                ContactSolver.Resolve(contacts);
            }


            timeAccumaltor = timeAccumaltor - fixedDeltaTime; 
        }

        if (wrap) 
        { 
            bodies.ForEach(body => body.position = Utilities.Wrap(body.position, -size, size));
        }

        bodies.ForEach(body => body.force = Vector2.zero);
        bodies.ForEach(body => body.acceleration = Vector2.zero);

        //Debug.Log(1.0f / Time.deltaTime);
    }
}
