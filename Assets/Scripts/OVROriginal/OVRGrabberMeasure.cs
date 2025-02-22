/************************************************************************************

Copyright   :   Copyright 2017 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.4.1 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/sdk-3.4.1

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows grabbing and throwing of objects with the OVRGrabbable component on them.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class OVRGrabberMeasure :  MonoBehaviour
{
    // Grip trigger thresholds for picking up objects, with some hysteresis.
    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;
    public AudioClip audioClip;

    private OVRHapticsClip hapticsClip;

    // Demonstrates parenting the held object to the hand's transform when grabbed.
    // When false, the grabbed object is moved every FixedUpdate using MovePosition. 
    // Note that MovePosition is required for proper physics simulation. If you set this to true, you can
    // easily observe broken physics simulation by, for example, moving the bottom cube of a stacked
    // tower and noting a complete loss of friction.
    [SerializeField]
    protected bool m_parentHeldObject = false;

    // Child/attached transforms of the grabber, indicating where to snap held objects to (if you snap them).
    // Also used for ranking grab targets in case of multiple candidates.
    [SerializeField]
    protected Transform m_gripTransform = null;
    // Child/attached Colliders to detect candidate grabbable objects.
    [SerializeField]
    protected Collider[] m_grabVolumes = null;

    // Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
    [SerializeField]
    public OVRInput.Controller m_controller;

    [SerializeField]
    protected Transform m_parentTransform;

    [SerializeField]
    protected OVRGrabberMeasure anotherHands;

    protected bool m_grabVolumeEnabled = true;
    protected Vector3 m_lastPos;
    protected Quaternion m_lastRot;
    protected Quaternion m_anchorOffsetRotation;
    protected Vector3 m_anchorOffsetPosition;
    protected float m_prevFlex;
	public OVRGrabbableMeasure m_grabbedObj = null;
    protected Vector3 m_grabbedObjectPosOff;
    protected Quaternion m_grabbedObjectRotOff;
	protected Dictionary<OVRGrabbableMeasure, int> m_grabCandidates = new Dictionary<OVRGrabbableMeasure, int>();
	protected bool operatingWithoutOVRCameraRig = true;
    protected bool wasGrab = false;
    public bool isGrab = false;
    public bool isGrabbableTriggerEnter = false;
    //public bool isGrabberTriggerEnter = false;
    public Nest nest;
    public Egg egg;

    public bool isCollisionEnter = false;
    public string collisionObjName;

    /// <summary>
    /// The currently grabbed object.
    /// </summary>
    public OVRGrabbableMeasure grabbedObject
    {
        get { return m_grabbedObj; }
    }

	public void ForceRelease(OVRGrabbableMeasure grabbable)
    {
        bool canRelease = (
            (m_grabbedObj != null) &&
            (m_grabbedObj == grabbable)
        );
        if (canRelease)
        {
            GrabEnd();
        }
    }

    protected virtual void Awake()
    {
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

		// If we are being used with an OVRCameraRig, let it drive input updates, which may come from Update or FixedUpdate.

		OVRCameraRig rig = null;
		if (transform.parent != null && transform.parent.parent != null)
			rig = transform.parent.parent.GetComponent<OVRCameraRig>();
		
		if (rig != null)
		{
			rig.UpdatedAnchors += (r) => {OnUpdatedAnchors();};
			operatingWithoutOVRCameraRig = false;
		}
    }

    protected virtual void Start()
    {
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        if(m_parentTransform == null)
        {
            if(gameObject.transform.parent != null)
            {
                m_parentTransform = gameObject.transform.parent.transform;
            }
            else
            {
                m_parentTransform = new GameObject().transform;
                m_parentTransform.position = Vector3.zero;
                m_parentTransform.rotation = Quaternion.identity;
            }
        }

        hapticsClip = new OVRHapticsClip(audioClip);
    }

	void FixedUpdate()
	{
		if (operatingWithoutOVRCameraRig)
			OnUpdatedAnchors();
	}

    // Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
    // This is done instead of parenting to achieve workable physics. If you don't require physics on 
    // your hands or held objects, you may wish to switch to parenting.
    void OnUpdatedAnchors()
    {
        Vector3 handPos = OVRInput.GetLocalControllerPosition(m_controller);
        Quaternion handRot = OVRInput.GetLocalControllerRotation(m_controller);
        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition + handPos);
        Quaternion destRot = m_parentTransform.rotation * handRot * m_anchorOffsetRotation;
        GetComponent<Rigidbody>().MovePosition(destPos);
        GetComponent<Rigidbody>().MoveRotation(destRot);

        if (!m_parentHeldObject /*&& anotherHands != null && anotherHands.m_grabbedObj != null*/)
        {               
            MoveGrabbedObject(destPos, destRot);
        }
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

		float prevFlex = m_prevFlex;
		// Update values from inputs
		m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);

		CheckForGrabOrRelease(prevFlex);
        wasGrab = isGrab;
    }

    void OnDestroy()
    {
        if (m_grabbedObj != null)
        {
            GrabEnd();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        isCollisionEnter = true;
        collisionObjName = collision.collider.gameObject.name;

    }

    private void OnCollisionExit(Collision collision)
    {
        isCollisionEnter = false;
    }


    void OnTriggerEnter(Collider otherCollider)
    {


        // 巣に当たった時
        childColliderComponent childColliderComponent = otherCollider.GetComponent<childColliderComponent>();
        if(childColliderComponent != null)
        {
            nest = childColliderComponent.nest;
        }

        //isGrabberTriggerEnter = true;


        // Get the grab trigger
        OVRGrabbableMeasure grabbable = otherCollider.GetComponent<OVRGrabbableMeasure>() ?? otherCollider.GetComponentInParent<OVRGrabbableMeasure>();
        Egg egg = otherCollider.GetComponent<Egg>() ?? otherCollider.GetComponentInParent<Egg>();
        if (grabbable != null && egg != null)
        {


            // Add the grabbable
            int refCount = 0;
            m_grabCandidates.TryGetValue(grabbable, out refCount);
            m_grabCandidates[grabbable] = refCount + 1;
            isGrabbableTriggerEnter = true;
            this.egg = egg;
            return;
        }
        //OVRGrabberBothHands grabber = otherCollider.GetComponent<OVRGrabberBothHands>();
        //if(grabber != null)
        //{
        //}


    }

    void OnTriggerExit(Collider otherCollider)
    {

        OVRGrabbableMeasure grabbable = otherCollider.GetComponent<OVRGrabbableMeasure>() ?? otherCollider.GetComponentInParent<OVRGrabbableMeasure>();
        //Egg egg = otherCollider.GetComponent<Egg>() ?? otherCollider.GetComponentInParent<Egg>();
        if (grabbable == null || egg == null) return;

        // Remove the grabbable
        isGrabbableTriggerEnter = false;
        this.egg = null;
        int refCount = 0;
        bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
        if (!found)
        {
            return;
        }

        if (refCount > 1)
        {
            m_grabCandidates[grabbable] = refCount - 1;
        }
        else
        {
            m_grabCandidates.Remove(grabbable);
        }
    }

    protected void CheckForGrabOrRelease(float prevFlex)
    {
        //if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
        if (isGrab && !wasGrab)
        {
            GrabBegin();
        }
        else if (!isGrab && wasGrab)
        {
            GrabEnd();
        }
    }


    public void HapticsHands()
    {
        if(this.m_controller == OVRInput.Controller.RTouch)
        {
            OVRHaptics.RightChannel.Mix(hapticsClip);
        }
        else if(this.m_controller == OVRInput.Controller.LTouch)
        {
            OVRHaptics.LeftChannel.Mix(hapticsClip);
        }

    }

    protected virtual void GrabBegin()
    {
        float closestMagSq = float.MaxValue;
        OVRGrabbableMeasure closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        HapticsHands();

        // Iterate grab candidates and find the closest grabbable candidate
        foreach (OVRGrabbableMeasure grabbable in m_grabCandidates.Keys)
        {
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
                if (grabbableCollider != null)
                {
                    Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                    float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                    if (grabbableMagSq < closestMagSq)
                    {
                        closestMagSq = grabbableMagSq;
                        closestGrabbable = grabbable;
                        closestGrabbableCollider = grabbableCollider;
                    }
                }
            }
        }

        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            if (closestGrabbable.isGrabbed)
            {
                closestGrabbable.grabbedBy.OffhandGrabbed(closestGrabbable);
            }

            m_grabbedObj = closestGrabbable;
            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            // Set up offsets for grabbed object desired position relative to hand.
            if(m_grabbedObj.snapPosition)
            {
                m_grabbedObjectPosOff = m_gripTransform.localPosition;
                if(m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = m_grabbedObj.snapOffset.position;
                    if (m_controller == OVRInput.Controller.LTouch) snapOffset.x = -snapOffset.x;
                    m_grabbedObjectPosOff += snapOffset;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                m_grabbedObjectRotOff = m_gripTransform.localRotation;
                if(m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = m_grabbedObj.snapOffset.rotation * m_grabbedObjectRotOff;
                }
            }
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }

            // Note: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
            // is beyond the scope of this demo.
            MoveGrabbedObject(m_lastPos, m_lastRot, true);
            if(m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }

    protected virtual void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
    {
        if (m_grabbedObj == null)
        {
            return;
        }

        Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
        Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
        Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;
        
        if(grabbedRigidbody == null)
        {
            return;
        }

        if (forceTeleport)
        {
            grabbedRigidbody.transform.position = grabbablePosition;
            grabbedRigidbody.transform.rotation = grabbableRotation;
        }
        else
        {
            grabbedRigidbody.MovePosition(grabbablePosition);
            grabbedRigidbody.MoveRotation(grabbableRotation);
        }
    }

    protected void GrabEnd()
    {
        if (m_grabbedObj != null)
        {
			OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(m_controller), orientation = OVRInput.GetLocalControllerRotation(m_controller) };
            OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };
            localPose = localPose * offsetPose;

			OVRPose trackingSpace = transform.ToOVRPose() * localPose.Inverse();
			Vector3 linearVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerVelocity(m_controller);
			Vector3 angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);

            GrabbableRelease(linearVelocity, angularVelocity);
        }

        // Re-enable grab volumes to allow overlap events
        GrabVolumeEnable(true);
    }

    protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
        if(m_parentHeldObject) m_grabbedObj.transform.parent = null;
        m_grabbedObj = null;
    }

    protected virtual void GrabVolumeEnable(bool enabled)
    {
        if (m_grabVolumeEnabled == enabled)
        {
            return;
        }

        m_grabVolumeEnabled = enabled;
        for (int i = 0; i < m_grabVolumes.Length; ++i)
        {
            Collider grabVolume = m_grabVolumes[i];
            grabVolume.enabled = m_grabVolumeEnabled;
        }

        if (!m_grabVolumeEnabled)
        {
            m_grabCandidates.Clear();
        }
    }


    protected virtual void OffhandGrabbed(OVRGrabbableMeasure grabbable)
    {
        if (m_grabbedObj == grabbable)
        {
            GrabbableRelease(Vector3.zero, Vector3.zero);
        }
    }

}
