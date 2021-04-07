using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Intergrator
{
    public static void ExplicitEuler(Body body, float dt)
    {
        //body.acceleration = body.force / body.mass;
        body.position = body.position + (body.velocity * dt);
        body.velocity = body.velocity + (body.acceleration * dt);
        body.velocity = body.velocity * (1f / (1f + (body.damping * dt)));
    }

    public static void SemiEular(Body body, float dt)
    {
        //body.acceleration = body.force / body.mass;
        body.velocity = body.velocity + (body.acceleration * dt);
        body.position = body.position + (body.velocity * dt);
        body.velocity = body.velocity * (1f / (1f + (body.damping * dt)));
    }
}
