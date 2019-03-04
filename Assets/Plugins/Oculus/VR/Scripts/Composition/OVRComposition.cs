/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using UnityEngine;
using System.Collections;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

public abstract class OVRComposition {

	public abstract OVRManager.CompositionMethod CompositionMethod();

	public abstract void Update(Camera mainCamera);
	public abstract void Cleanup();

	public virtual void RecenterPose() { }

	protected bool usingLastAttachedNodePose = false;
	protected OVRPose lastAttachedNodePose = new OVRPose();            // Sometimes the attach node pose is not readable (lose tracking, low battery, etc.) Use the last pose instead when it happens

	public OVRPose ComputeCameraWorldSpacePose(OVRPlugin.CameraExtrinsics extrinsics)
	{
		OVRPose worldSpacePose = new OVRPose();
		OVRPose trackingSpacePose = new OVRPose();

		OVRPose cameraTrackingSpacePose = extrinsics.RelativePose.ToOVRPose();
		trackingSpacePose = cameraTrackingSpacePose;

		if (extrinsics.AttachedToNode != OVRPlugin.Node.None && OVRPlugin.GetNodePresent(extrinsics.AttachedToNode))
		{
			if (usingLastAttachedNodePose)
			{
				Debug.Log("The camera attached node get tracked");
				usingLastAttachedNodePose = false;
			}
			OVRPose attachedNodePose = OVRPlugin.GetNodePose(extrinsics.AttachedToNode, OVRPlugin.Step.Render).ToOVRPose();
			lastAttachedNodePose = attachedNodePose;
			trackingSpacePose = attachedNodePose * trackingSpacePose;
		}
		else
		{
			if (extrinsics.AttachedToNode != OVRPlugin.Node.None)
			{
				if (!usingLastAttachedNodePose)
				{
					Debug.LogWarning("The camera attached node could not be tracked, using the last pose");
					usingLastAttachedNodePose = true;
				}
				trackingSpacePose = lastAttachedNodePose * trackingSpacePose;
			}
		}

		worldSpacePose = OVRExtensions.ToWorldSpacePose(trackingSpacePose);
		return worldSpacePose;
	}

}

#endif
