using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NativeClient;
using System;

public class FiVESClientBehaviour : MonoBehaviour
{
	private NativeClientApplication fivesClient;
	private EntityInfo dragonInfo;
	[HideInInspector]
	private List<Transform> boneTransforms;
	public ZigSkeleton zigSkeleton;
	private Transform dragonTf;

	public NativeClientAppParams clientParams = NativeClientAppParams.Default;

	void Start ()
	{
		GameObject dragon = GameObject.Find("Dragon");
		if (!dragon)
			Debug.LogError("please assign dragonTf!", this);
		dragonTf = dragon.transform;

		if (!zigSkeleton)
			Debug.LogError ("please assign Zig Skeleton in the inspector!", this);

		//bone transforms that will be synchronized
		boneTransforms = new List<Transform> ();
//		boneTransforms.Add (zigSkeleton.Head);
//		boneTransforms.Add (zigSkeleton.Neck);
//		boneTransforms.Add (zigSkeleton.Torso);
//		boneTransforms.Add (zigSkeleton.Waist);
//		boneTransforms.Add (zigSkeleton.LeftShoulder);
//		boneTransforms.Add (zigSkeleton.LeftElbow);
//		boneTransforms.Add (zigSkeleton.LeftWrist);
//		boneTransforms.Add (zigSkeleton.RightShoulder);
//		boneTransforms.Add (zigSkeleton.RightElbow);
//		boneTransforms.Add (zigSkeleton.RightWrist);

		string[] tfNames = {
			// body bones
			"Body", "Spine1", "Spine2", "Spine3", "Spine4", "Spine5",
			"Neck", "Head", "Jaw", "Shoulder_Right", "Shoulder_Left", "Hips",
			"Leg_Left", "Leg_Right", "Tail1", "Tail2", "Tail3", "Tail4", "Tail5", "Tail6", "Tail7",

			// left wing bones
			"Shoulder_Left", "Elbow_Left", "Hand_Left",

			// right wing bones
			"Elbow_Right", "Shoulder_Right", "Hand_Right"
		};

		foreach (string name in tfNames) {
			Transform tf = SearchHierarchyForBone(dragonTf, name);
			if (tf == null)
				Debug.LogWarning("Unable to find bone " + name);
			boneTransforms.Add (tf);
		}

		// create fives native client
		fivesClient = new NativeClientApplication (clientParams.ToString());
		fivesClient.Authenticated += HandleAuthEvent;
		fivesClient.ReceivedNewObject += HandleReceivedNewObject;
		fivesClient.EntityCreated += HandleEntityCreated;
		fivesClient.ReceivedUpdate += HandleReceivedUpdate;
		
	}
	
	void HandleReceivedUpdate (object sender, UpdateEventArgs e)
	{
	}

	void HandleEntityCreated (object sender, EntityCreatedEventArgs e)
	{
		// once created gets cached 
		dragonInfo = e.EntityInfo;
	}

	void HandleReceivedNewObject (object sender, NewObjectEventArgs e)
	{
	}

	void HandleAuthEvent (object sender, EventArgs e)
	{
		// once authed the dragon entity is created
		fivesClient.GetDragon ();
	}

	void OnApplicationQuit ()
	{
		// close socket to prevent unity crash on windows
		fivesClient.Disconnect ();
	}

	void FixedUpdate ()
	{
		//todo: too many updates per second?
		if (dragonInfo) {
			dragonInfo.Position = dragonTf.position;
			dragonInfo.Position.x = -dragonInfo.Position.x;
			// TODO: mirror along x-axis
			dragonInfo.Orientation = dragonTf.rotation;
			dragonInfo.Orientation.y = -dragonInfo.Orientation.y;
			dragonInfo.Orientation.z = -dragonInfo.Orientation.z;

			fivesClient.MoveEntity(dragonInfo);
			fivesClient.RotateEntity(dragonInfo);
			
			fivesClient.UpdateBones (dragonInfo, boneTransforms);
		}
	}

	Transform SearchHierarchyForBone(Transform current, string name)
	{
		if (current.name == name)
			return current;

		for (int i = 0; i < current.childCount; ++i) {
			Transform res = this.SearchHierarchyForBone (current.GetChild (i), name);
			if (res != null)
				return res;
		}

		return null;
	}
}

[Serializable]
public class NativeClientAppParams
{
	public static NativeClientAppParams Default {
		get {
			return new NativeClientAppParams () 
			{ 
				ip = "127.0.0.1",
				port = "34837"
			};	
		}
	}

	public string ip;
	public string port;

	public override string ToString ()
	{
		return string.Format("ws://{0}:{1}/", ip, port);
	}
}

