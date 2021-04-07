using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class World : MonoBehaviour
{
    public BoolData simulate;
    public FloatData gravity;
    public FloatData fixedFPS;
    public TMP_Text valueText = null;

    static World instance;

    public float timeAccumaltor;

    public float fixedDeltaTime { get { return 1.0f / fixedFPS.value; } }
    static public World Instance { get { return instance; } }

    public Vector2 Gravity { get { return new Vector2(0, gravity.value); } }
    public List<Body> bodies { get; set; } = new List<Body>();

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        Debug.Log(1.0f / Time.deltaTime);
        if (!simulate.value)
        {
            return;
        }
        float dt = Time.deltaTime;

        timeAccumaltor += Time.deltaTime;
       
        bodies.ForEach(body => body.Step(dt));
        bodies.ForEach(body => Intergrator.SemiEular(body, dt));//ExplicitEuler(body, dt));

        bodies.ForEach(body => body.force = Vector2.zero);
        bodies.ForEach(body => body.acceleration = Vector2.zero);


        while (timeAccumaltor > fixedDeltaTime) 
        { 
            bodies.ForEach(body => body.Step(fixedDeltaTime)); 
            bodies.ForEach(body => Intergrator.ExplicitEuler(body, fixedDeltaTime)); 
            timeAccumaltor = timeAccumaltor - fixedDeltaTime; 
        }
        valueText.text = fixedFPS.value.ToString("F2");
    }
}
