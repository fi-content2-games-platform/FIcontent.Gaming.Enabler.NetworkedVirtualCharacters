using System.Collections.Generic;
using System;
using NativeClient;
using UnityEngine;

public static partial class Json{

	static object VectorToObject(Vector3 v)
	{
		return new List<float>() { v.x, v.y, v.z };
	}
	static Vector3 ObjectToVector(object o)
	{
		Debug.Log(o.GetType());

		var l = (List<float>)o;
		var v = new Vector3();
		v.x = Convert.ToSingle(l[0]);
		v.y = Convert.ToSingle(l[1]);
		v.z = Convert.ToSingle(l[2]);

		return v;
	}

	static object QuaternionToObject(Quaternion q)
	{
		return new List<float>() { q.x, q.y, q.z, q.w };
	}
	static Quaternion ObjectToQuaternion(object o)
	{
		var l = (List<float>)o;
		var q = new Quaternion();
		q.x = Convert.ToSingle(l[0]);
		q.y = Convert.ToSingle(l[1]);
		q.z = Convert.ToSingle(l[2]);
		q.w = Convert.ToSingle(l[3]);
		
		return q;
	}

	public static object EntityInfoToObject(EntityInfo e)
	{
		var d = new Dictionary<string, object>();
		d.Add("type", "EntityInfo");
		d.Add("guid", e.Guid);
		d.Add("isLocallyCreated", e.IsLocallyCreated);
		d.Add("position", VectorToObject(e.Position));
		d.Add("orientation", QuaternionToObject(e.Orientation));

		return d;
	}

	public static EntityInfo ObjectToEntityInfo(object o)
	{
		var d = (Dictionary<string,object>)o;
		var e = new EntityInfo();
		e.Guid = (string)d["guid"];
		e.IsLocallyCreated = (bool)d["isLocallyCreated"];
		e.Position = ObjectToVector(d["position"]);
		e.Orientation = ObjectToQuaternion(d["orientation"]);

		return e;
	}


}
