using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorCheck : MonoBehaviour
{
    public float distanceCheck;
    public const string PropTag = "Prop";
    public float angle;
    private float colliderWidth = 0f;
    private float colliderLength = 0f;

    private BoxCollider CheckCollider;
    public Transform SelfTrans;
    public List<Collider> triggerColliders=new List<Collider>();

    void Awake()
    {
        triggerColliders.Clear();
        CheckCollider = GetComponent<BoxCollider>();
        colliderWidth = 2 * distanceCheck * Mathf.Sin(angle * Mathf.Deg2Rad);
        colliderLength = distanceCheck;
        CheckCollider.transform.localPosition = Vector3.forward * distanceCheck / 2f;
        CheckCollider.center = Vector3.zero;
        CheckCollider.size = new Vector3(colliderWidth, 1, colliderLength);
    }


    private bool CheckIsInSector(GameObject otherGo)
    {
        Vector3 otherLocalPos=SelfTrans.InverseTransformDirection(otherGo.transform.position);
        Vector3 forwardNormalize = Vector3.forward;
        Vector3 otherDir = otherLocalPos - SelfTrans.localPosition;
        Vector3 otherDirNormalize = Vector3.Normalize(otherDir);
        float cosGetAngle = Vector3.Dot(forwardNormalize, otherDirNormalize);
        float getAngle = Mathf.Acos(cosGetAngle)*Mathf.Rad2Deg;
        if (Mathf.Abs(getAngle) > Mathf.Abs(angle)) //在角度之外。
        {
            return false;
        }
        else
        {
            float distance = Vector3.Distance(otherLocalPos, SelfTrans.localPosition);
            if (distance<= distanceCheck)
            {//在扇形范围之内
                return true;
            }
        }
        return false;
    }

    // Use this for initialization
	void Start ()
	{
	    


    }
    void OnTriggerEnter(Collider other)
    {//父对象必须是钢体，刚体不要和Collider放在同一层级，会一一检测碰撞器，所有碰撞器检测完了在执行Update.
        if (other.gameObject.tag== PropTag)
        {
            if (CheckIsInSector(other.gameObject))
            {
                //Debug.Log(other.name + " is in Sector");
                triggerColliders.Add(other);
            }
            //Debug.Log(other.name + " is Enter");
        }
        
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == PropTag)
        {
            if (CheckIsInSector(other.gameObject))
            {
                if (!triggerColliders.Contains(other))
                {
                    //Debug.Log(other.name + " is in Sector");
                    triggerColliders.Add(other);
                }
            }
            else
            {
                if (triggerColliders.Contains(other))
                {//在触发器里，但是不在扇形范围内
                    triggerColliders.Remove(other);
                }
            }
        }
        if (triggerColliders.Count > 0)
        {
            foreach (var eachCollider in triggerColliders)
            {
                Debug.Log(eachCollider.name + " is in Sector");
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == PropTag)
        {
            if (triggerColliders.Contains(other))
            {
                triggerColliders.Remove(other);
            }
            Debug.Log(other.name + " is Exit");
        }
    }
    // Update is called once per frame
    void Update () {
	    Quaternion r = SelfTrans.rotation;
	    Vector3 f0 = (SelfTrans.position + (r * Vector3.forward) * distanceCheck);
	    Debug.DrawLine(SelfTrans.position, f0, Color.red);

	    Quaternion r0 = Quaternion.Euler(SelfTrans.rotation.eulerAngles.x, SelfTrans.rotation.eulerAngles.y - angle, SelfTrans.rotation.eulerAngles.z);
	    Quaternion r1 = Quaternion.Euler(SelfTrans.rotation.eulerAngles.x, SelfTrans.rotation.eulerAngles.y + angle, SelfTrans.rotation.eulerAngles.z);

	    Vector3 f1 = (SelfTrans.position + (r0 * Vector3.forward) * distanceCheck);
	    Vector3 f2 = (SelfTrans.position + (r1 * Vector3.forward) * distanceCheck);

	    Debug.DrawLine(SelfTrans.position, f1, Color.red);
	    Debug.DrawLine(SelfTrans.position, f2, Color.red);
	    for (int i=(int)(2-angle);i< angle;i+=2) {
	        Quaternion rEach= Quaternion.Euler(SelfTrans.rotation.eulerAngles.x, SelfTrans.rotation.eulerAngles.y +i, SelfTrans.rotation.eulerAngles.z);
	        Vector3 fEach = (SelfTrans.position + (rEach * Vector3.forward) * distanceCheck);
	        Debug.DrawLine(SelfTrans.position, fEach, Color.red);
        }

	    Debug.DrawLine(f0, f1, Color.red);
	    Debug.DrawLine(f0, f2, Color.red);
    }
}
