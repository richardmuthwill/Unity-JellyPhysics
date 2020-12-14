﻿using UnityEngine;

public class JellyMesh : MonoBehaviour
{
	public float Intensity = 1f;
	public float Mass = 1f;
	public float stiffness = 1f;
	public float damping = 0.75f;

	private Mesh OriginalMesh, MeshClone;
	private MeshRenderer mRenderer;
	private JellyVertex[] jv;
	private Vector3[] vertexArray;

    // Start is called before the first frame update
    void Start()
    {
		OriginalMesh = GetComponent<MeshFilter>().sharedMesh;
		MeshClone = Instantiate(OriginalMesh);
		GetComponent<MeshFilter>().sharedMesh = MeshClone;
		mRenderer = GetComponent<MeshRenderer>();

		jv = new JellyVertex[MeshClone.vertices.Length];
		for (int i = 0; i < MeshClone.vertices.Length; i++)
		{
			jv[i] = new JellyVertex(i, transform.TransformPoint(MeshClone.vertices[i]));
		}
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		vertexArray = OriginalMesh.vertices;
		for (int i = 0; i < jv.Length; i++)
		{
			Vector3 target = transform.TransformPoint(vertexArray[jv[i].ID]);
			float intensity = (1 - (mRenderer.bounds.max.y - target.y) / mRenderer.bounds.size.y) * Intensity;
			jv[i].Shake(target, Mass, stiffness, damping);
			target = transform.InverseTransformPoint(jv[i].Position);
			vertexArray[jv[i].ID] = Vector3.Lerp(vertexArray[jv[i].ID], target, intensity);
		}
		MeshClone.vertices = vertexArray;
    }

	public class JellyVertex
	{
		public int ID;
		public Vector3 Position;
		public Vector3 Velocity, Force;

		public JellyVertex(int _id, Vector3 _pos)
		{
			ID = _id;
			Position = _pos;
		}

		public void Shake (Vector3 target, float m, float s, float d)
		{
			Force = (target - Position) * s;
			Velocity = (Velocity + Force / m) * d;
			Position += Velocity;
			if ((Velocity + Force + Force / m).magnitude < 0.001f)
			{
				Position = target;
			}
		}
	}
}
