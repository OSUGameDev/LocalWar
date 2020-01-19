using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

public class ModulePrototype : MonoBehaviour
{
	[System.Serializable]
	public abstract class FaceDetails
	{
		public bool Walkable;
		public int Connector;
		public virtual void ResetConnector()
		{
			this.Connector = 0;
		}
		public ModulePrototype[] ExcludedNeighbours;
		public bool EnforceWalkableNeighbor = false;
		public bool IsOcclusionPortal = false;
	}

	[System.Serializable]
	public class HorizontalFaceDetails : FaceDetails
	{
		public bool Symmetric;
		public bool Flipped;

		public override string ToString()
		{
			return this.Connector.ToString() + (this.Symmetric ? "s" : (this.Flipped ? "F" : ""));
		}

		public override void ResetConnector()
		{
			base.ResetConnector();
			this.Symmetric = false;
			this.Flipped = false;
		}
	}

	[System.Serializable]
	public class VerticalFaceDetails : FaceDetails
	{
		public bool Invariant;
		public int Rotation;

		public override string ToString()
		{
			return this.Connector.ToString() + (this.Invariant ? "i" : (this.Rotation != 0 ? "_bcd".ElementAt(this.Rotation).ToString() : ""));
		}

		public override void ResetConnector()
		{
			base.ResetConnector();
			this.Invariant = false;
			this.Rotation = 0;
		}
	}

	public float Probability = 1.0f;
	public bool Spawn = true;
	public bool IsInterior = false;

	public HorizontalFaceDetails Left;
	public VerticalFaceDetails Down;
	public HorizontalFaceDetails Back;
	public HorizontalFaceDetails Right;
	public VerticalFaceDetails Up;
	public HorizontalFaceDetails Forward;

	public FaceDetails[] Faces
	{
		get
		{
			return new FaceDetails[] {
				this.Left,
				this.Down,
				this.Back,
				this.Right,
				this.Up,
				this.Forward
			};
		}
	}

	public Mesh GetMesh(bool createEmptyFallbackMesh = true)
	{
		var meshFilter = this.GetComponent<MeshFilter>();
		if (meshFilter != null && meshFilter.sharedMesh != null)
		{
			return meshFilter.sharedMesh;
		}
		if (createEmptyFallbackMesh)
		{
			var mesh = new Mesh();
			return mesh;
		}
		return null;
	}

	void Update() { }

	void Reset()
	{
		this.Up = new VerticalFaceDetails();
		this.Down = new VerticalFaceDetails();
		this.Right = new HorizontalFaceDetails();
		this.Left = new HorizontalFaceDetails();
		this.Forward = new HorizontalFaceDetails();
		this.Back = new HorizontalFaceDetails();

		foreach (var face in this.Faces)
		{
			face.ExcludedNeighbours = new ModulePrototype[] { };
		}
	}
}