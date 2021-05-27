using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BVH : BroadPhase
{
	public BVHNode rootNode;
	public override void Build(AABB aabb, List<Body> bodies)
	{
	 	potientialCollisionCount = 0;
		List<Body> sorted = new List<Body>(bodies);

		sorted.Sort((x, y) => x.position.x.CompareTo(y.position.y));  
		//sorted = bodies.OrderBy(body => body.position.x).ToList();
		rootNode = new BVHNode(sorted);
	}

	public override void Query(AABB aabb, List<Body> bodies)
	{
		rootNode.Query(aabb, bodies);
		potientialCollisionCount = potientialCollisionCount + bodies.Count;
	}

	public override void Query(Body body, List<Body> bodies)
	{
		Query(body.shape.aABB, bodies);
	}

	public override void Draw()
	{
		rootNode?.Draw();
	}

}
