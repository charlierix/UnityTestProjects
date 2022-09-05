using PerfectlyNormalUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Must;

//TODO: Get rotation working

//TODO: Create joint angle constraints
//   Primary axis is perpendicular to the bend
//   Secondary axis bisects the shorter angle
//(or maybe switch primary and secondary) (or just call them something different)

//TODO: Create joint rotational constraints.  This is probably a link level constraint, not stored at the joint level
//   Axis is the line between the two joints
//   Need some kind of reference point for zero degrees

public class IK_MeshWorker : MonoBehaviour
{
    #region class: Chain

    private class Chain
    {
        public IK_Target Target { get; set; }

        public ChainJoint[] Joints { get; set; }

        public long Token { get; } = TokenGenerator.NextToken();

        /// <summary>
        /// This isn't used during runtime processing, it's just a hint for finding chains that are redundant
        /// </summary>
        public bool EndsInLoop { get; set; }

        public override string ToString()
        {
            string chain = "";
            bool isLastAnchored = false;

            if (Joints != null)
            {
                chain = Joints.
                    Select(o => o.Joint?.ID ?? "").
                    ToJoin(", ");

                isLastAnchored = Joints[Joints.Length - 1]?.IsAnchored ?? false;
            }

            return string.Format("{0} | {1} | {2}", Target?.ID, chain, isLastAnchored ? "anchored" : "free");
        }
    }

    #endregion
    #region class: ChainJoint

    private class ChainJoint
    {
        public long Token { get; } = TokenGenerator.NextToken();

        // Init Variables
        public int Index { get; set; }

        public IK_Joint Joint { get; set; }
        public bool IsAnchored { get; set; }

        public int[] LinkIndices { get; set; }

        /// <summary>
        /// chain[0].Dist is undefined
        /// To get the distance between 0 and 1, look in 1
        /// </summary>
        /// <remarks>
        /// This is intermediate.  The final version will have a separate link class between two joints, the distance will be stored there
        /// </remarks>
        public float DistanceFromPrevJoint { get; set; } = 3f;
        public AngleLinkConstraint[] InitialAngles { get; set; }

        // Update Variables
        public Vector3 Position { get; set; }

        public override string ToString()
        {
            return string.Format("{0} | {1}", Joint?.ID, IsAnchored ? "anchored" : "");
        }
    }

    #endregion
    #region class: AngleLinkConstraint

    private class AngleLinkConstraint
    {
        public int Index1 { get; set; }
        public int Index2 { get; set; }

        public ChainJoint Joint1 { get; set; }
        public ChainJoint Joint2 { get; set; }


        //TODO: Store these things relative to the joint transform's current rotation


        public float Angle { get; set; }

        public Vector3 BisectLine { get; set; }
        public Vector3 Orth { get; set; }



        //TODO: Store angle min/max limits (main axis and orth).  Also store resistance forces, etc
    }

    #endregion

    #region Declaration Section

    public IK_LinkJoints[] Links;

    /// <summary>
    /// Solver iterations per update
    /// </summary>
    [Header("Solver Parameters")]
    public int Iterations = 12;

    /// <summary>
    /// If leaf is this close to target, iterations stop early
    /// </summary>
    public float Epsilon = .001f;

    private Chain[] _chains;
    private Transform[] _joints;
    private JointRotator[] _jointRotators;      // this is the same size as _joints
    private ChainJoint[][] _chainJointsByIndex;

    #endregion

    private void Start()
    {
        if (Links == null || Links.Length == 0)
            return;

        // Get all target and joint components
        var allTargets = UnityEngine.Object.FindObjectsOfType<IK_Target>();
        var allJoints = UnityEngine.Object.FindObjectsOfType<IK_Joint>();

        string[] dupeTargets = GetDupes(allTargets.Select(o => o.ID.ToUpper()));
        if (dupeTargets.Length > 0)
            throw new ApplicationException($"Multiple targets with the same name: {dupeTargets.ToJoin(", ")}");

        string[] dupeJoints = GetDupes(allJoints.Select(o => o.ID.ToUpper()));
        if (dupeJoints.Length > 0)
            throw new ApplicationException($"Multiple joints with the same name: {dupeJoints.ToJoin(", ")}");

        var chains = new List<Chain>();

        // Figure out call chains for each target bound joint
        // NOTE: There may be sets of joints unrelated to this mesh, but they will be ignored since the links won't mention them
        foreach (var targetedJoint in allJoints.Where(o => !string.IsNullOrWhiteSpace(o.TargetID)))
        {
            chains.AddRange(GetCallChains(targetedJoint, allTargets, allJoints, Links));
        }

        chains = chains.
            Where(o => o != null).      // there will only be nulls if joint/target IDs are keyed in wrong
            ToList();

        // The way the chains are built above, joints get reused across chains.  Need to clone all joints
        // to ensure each chain has its own
        foreach (Chain chain in chains)
        {
            chain.Joints = chain.Joints.
                Select(o => new ChainJoint()
                {
                    Index = o.Index,
                    Joint = o.Joint,
                    IsAnchored = o.IsAnchored,
                    Position = o.Position,
                    //DistanceFromPrevJoint, InitialAngles, LinkIndices   //these haven't been populated yet, so don't need to be cloned
                }).
                ToArray();
        }

        // Do post processing to fill in some properties
        PopulateJointLinks(chains);
        PopulateEndsInLoop(chains);

        // Get rid of unnecessary chains
        //_chains = chains.ToArray();
        _chains = RemoveUnnecessaryChains(chains);

        //TODO: Distances are currently stored in the chainjoint, but that's a bit arbitrary.  Create a joint link class
        //   Holds distance between the two joints
        //   Holds rotational constraint
        StoreDistances_INTERMEDIATE(_chains);

        // Find all the common joints across all chains, so they can be averaged each step
        int maxIndex = _chains.
            SelectMany(o => o.Joints).
            Max(o => o.Index);

        _chainJointsByIndex = Enumerable.Range(0, maxIndex + 1).
            Select(o =>
                _chains.
                    SelectMany(p => p.Joints).
                    Where(p => p.Index == o).       //NOTE: Some indices may not have any
                    ToArray()).
            ToArray();

        _joints = allJoints.
            Select(o => o.transform).
            ToArray();

        StoreInitialAngles(_chains, _joints);

        _jointRotators = CreateRotators(_chainJointsByIndex, _joints);

        InitDebugDrawing();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SaveChains(_chains, Links);
        }

        UpdateDebugDrawing();
    }
    private void LateUpdate()
    {
        if (_chains == null)
            return;

        // Get initial positions
        for (int outer = 0; outer < _chains.Length; outer++)
        {
            for (int inner = 0; inner < _chains[outer].Joints.Length; inner++)
            {
                _chains[outer].Joints[inner].Position = _joints[_chains[outer].Joints[inner].Index].position;
            }
        }

        for (int cntr = 0; cntr < Iterations; cntr++)
        {

            //TODO: See if all tagetting joints are close enough to their targets


            foreach (Chain chain in _chains)
            {
                Sweep_FromTarget(chain);
                Sweep_TowardTarget(chain);

                ApplyAngleConstraints(chain);
            }

            // Sync positions
            foreach (var sameIndex in _chainJointsByIndex)
            {
                SyncPositions(sameIndex);
            }

            // Rotate joints
            foreach (var joint in _jointRotators)
            {
                joint.SyncToEndpoints();
            }

        }

        // Commit positions
        for (int cntr = 0; cntr < _joints.Length; cntr++)
        {
            if (_chainJointsByIndex[cntr].Length > 0)
                _joints[cntr].position = _chainJointsByIndex[cntr][0].Position;       // they get averaged together each iteration, so each instance of ChainJoint at this index is the same
        }
    }

    private void OnDrawGizmos()
    {
        var allTargets = UnityEngine.Object.FindObjectsOfType<IK_Target>();
        var allJoints = UnityEngine.Object.FindObjectsOfType<IK_Joint>();

        // Target - Joint
        foreach (var joint in allJoints.Where(o => !string.IsNullOrEmpty(o.TargetID)))
        {
            foreach (var target in allTargets.Where(o => o.IsMatch(joint.TargetID)))
            {
                Handles.color = Color.blue;
                Handles.DrawLine(target.transform.position, joint.transform.position);
            }
        }

        // Joint - Joint
        if (Links != null)
        {
            foreach (var link in Links)
            {
                var joint1 = allJoints.FirstOrDefault(o => o.IsMatch(link.JointID_1));
                var joint2 = allJoints.FirstOrDefault(o => o.IsMatch(link.JointID_2));

                if (joint1 != null && joint2 != null)
                {
                    Handles.color = Color.green;
                    Handles.DrawLine(joint1.transform.position, joint2.transform.position);
                }
            }
        }
    }

    #region Private Methods - init

    private static Chain[] GetCallChains(IK_Joint targetedJoint, IK_Target[] allTargets, IK_Joint[] allJoints, IK_LinkJoints[] allLinks)
    {
        // Make sure this actually has a corresponding target
        IK_Target target = allTargets.FirstOrDefault(o => o.IsMatch(targetedJoint.TargetID));
        if (target == null)
            return null;      // a target is specified, but there are none, so there is no point in walking a chain

        ChainJoint head = new ChainJoint()
        {
            Joint = targetedJoint,
            IsAnchored = false,
            Index = GetIndex(targetedJoint.ID, allJoints),
        };

        var joints = GetChains(new[] { head }, targetedJoint, allTargets, allJoints, allLinks);

        return joints.
            Select(o => new Chain()
            {
                Target = target,
                Joints = o,
            }).
            ToArray();
    }

    private static ChainJoint[][] GetChains(ChainJoint[] ancestors, IK_Joint node, IK_Target[] allTargets, IK_Joint[] allJoints, IK_LinkJoints[] allLinks)
    {
        var matchedLinks = allLinks.
            Where(o => o.Contains(node.ID)).
            Select(o => new
            {
                link = o,
                keys = o.GetOtherLink(node.ID),
            }).
            Where(o => !ancestors.Any(p => p.Joint.IsMatch(o.keys.other))).       // ignore loop backs
            ToArray();

        if (matchedLinks.Length == 0)
            return new[] { ancestors };

        var retVal = new List<ChainJoint[]>();

        foreach (var match in matchedLinks)
        {
            IK_Joint child = allJoints.FirstOrDefault(o => o.IsMatch(match.keys.other));
            if (child == null)
                continue;

            ChainJoint newJoint = new ChainJoint()
            {
                Joint = child,
                IsAnchored = child.IsAnchored || allTargets.Any(o => o.IsMatch(child.TargetID)),
                Index = GetIndex(child.ID, allJoints),
            };

            ChainJoint[] newAncestors = ancestors.
                Concat(new[] { newJoint }).
                ToArray();

            if (newJoint.IsAnchored)
            {
                retVal.Add(newAncestors);
            }
            else
            {
                retVal.AddRange(GetChains(newAncestors, child, allTargets, allJoints, allLinks));
            }
        }

        return retVal.ToArray();
    }

    private static void PopulateJointLinks(IList<Chain> chains)
    {
        var jointsByIndex = chains.
            SelectMany(o => o.Joints).
            ToLookup(o => o.Index).
            ToArray();

        var distinctJointIndices = jointsByIndex.
            Select(o => o.Key).
            ToArray();

        foreach (var jointIndex in distinctJointIndices)
        {
            var touching = new List<int>();

            foreach (var chain in chains)
            {
                for (int cntr = 0; cntr < chain.Joints.Length; cntr++)
                {
                    if (chain.Joints[cntr].Index == jointIndex)
                    {
                        if (cntr > 0)
                            touching.Add(chain.Joints[cntr - 1].Index);

                        if (cntr < chain.Joints.Length - 1)
                            touching.Add(chain.Joints[cntr + 1].Index);

                        break;
                    }
                }
            }

            int[] links = touching.
                Distinct().
                OrderBy().
                ToArray();

            // links holds all possible links to this joint, but some chains may not contain all joints.  Need to create
            // a filtered list for each chain
            foreach (Chain chain in chains)
            {
                ChainJoint joint = chain.Joints.FirstOrDefault(o => o.Index == jointIndex);
                if (joint == null)
                    continue;

                joint.LinkIndices = links.
                    Where(o => chain.Joints.Any(p => p.Index == o)).
                    ToArray();
            }
        }
    }

    private static void PopulateEndsInLoop(IList<Chain> chains)
    {
        foreach (var chain in chains)
        {
            chain.EndsInLoop = chain.Joints[chain.Joints.Length - 1].LinkIndices.Length > 1;
        }
    }

    private static Chain[] RemoveUnnecessaryChains(IEnumerable<Chain> chains)
    {
        return chains.
            ToLookup(o => o.Target.ID.ToUpper()).
            SelectMany(o => RemoveUnnecessaryChains_Target(o.ToArray())).
            ToArray();
    }
    private static Chain[] RemoveUnnecessaryChains_Target(Chain[] chains)
    {
        if (chains.All(o => !o.EndsInLoop))
            return chains;      // no reductions possible (there could be, but it requires more changes in the rest of the class - like treating bottleneck joints like psuedo targets)

        var attempts = Enumerable.Range(0, 1). //Enumerable.Range(0, 12).
            Select(o => RemoveUnnecessaryChains_Target_Attempt(chains)).
            ToArray();

        // Find the best consolidation
        //    Fewest chains?
        //    Fewest total links?
        //    The set with links that are closest to the same size?
        //
        // Maybe give a score to each category and choose the best?

        return attempts[0];
    }
    private static Chain[] RemoveUnnecessaryChains_Target_Attempt(Chain[] chains)
    {
        var rand = StaticRandom.GetRandomForThread();

        var keep = new List<Chain>();
        var candidates = new List<Chain>();

        foreach (Chain chain in chains)
        {
            if (chain.EndsInLoop)
                candidates.Add(chain);
            else
                keep.Add(chain);
        }

        while (candidates.Count > 0)
        {
            int index = rand.Next(candidates.Count);

            Chain tryRemove = candidates[index];
            candidates.RemoveAt(index);

            var links = GetLinks(tryRemove);

            FilterExisting(keep, links);

            if (links.Count > 0)
                keep.Add(tryRemove);        // There are still some links that aren't unique to the chains in keep, so add this to the list
        }

        return keep.ToArray();
    }

    private static void FilterExisting(IEnumerable<Chain> chains, List<(int, int)> links)
    {
        foreach (Chain chain in chains)
        {
            if (links.Count == 0)
                return;

            var existingLinks = GetLinks(chain);

            links.RemoveWhere(o => existingLinks.Any(p => IsMatchingLink(o, p)));
        }
    }

    private static List<(int, int)> GetLinks(Chain chain)
    {
        return Enumerable.Range(0, chain.Joints.Length - 1).
            Select(o => (chain.Joints[o].Index, chain.Joints[o + 1].Index)).
            ToList();
    }

    private static bool IsMatchingLink((int, int) link1, (int, int) link2)
    {
        return
            (link1.Item1 == link2.Item1 && link1.Item2 == link2.Item2) ||
            (link1.Item1 == link2.Item2 && link1.Item2 == link2.Item1);
    }

    private static void StoreDistances_INTERMEDIATE(Chain[] chains)
    {
        foreach (Chain chain in chains)
        {
            for (int cntr = 1; cntr < chain.Joints.Length; cntr++)
            {
                chain.Joints[cntr].DistanceFromPrevJoint = (chain.Joints[cntr].Joint.transform.position - chain.Joints[cntr - 1].Joint.transform.position).magnitude;
            }
        }
    }

    private static void StoreInitialAngles(Chain[] chains, Transform[] joints)
    {
        // This needs to be calculated for each chain

        foreach (Chain chain in chains)
        {
            foreach (ChainJoint joint in chain.Joints)
            {
                joint.InitialAngles = UtilityCore.GetPairs(joint.LinkIndices.Length).
                    Select(o =>
                    {
                        int index1 = joint.LinkIndices[o.Item1];
                        int index2 = joint.LinkIndices[o.Item2];

                        Vector3 line1 = joints[index1].position - joints[joint.Index].position;
                        Vector3 line2 = joints[index2].position - joints[joint.Index].position;

                        Vector3 bisectLine = Math3D.GetAverage(new[] { line1.normalized, line2.normalized });
                        Vector3 orth = Vector3.Cross(line1.normalized, bisectLine);

                        return new AngleLinkConstraint()
                        {
                            Index1 = index1,
                            Index2 = index2,

                            Joint1 = chain.Joints.First(p => p.Index == index1),
                            Joint2 = chain.Joints.First(p => p.Index == index2),

                            BisectLine = bisectLine,
                            Orth = orth,

                            Angle = Vector3.Angle(line1, line2),
                        };
                    }).
                    ToArray();
            }
        }
    }

    private static JointRotator[] CreateRotators(ChainJoint[][] byIndex, Transform[] joints)
    {
        if (byIndex.Length != joints.Length)
            throw new ArgumentException($"The arrays passed in are supposed to be the same size.  byIndex: {byIndex.Length}, joints: {joints.Length}");

        var retVal = new JointRotator[joints.Length];

        for (int cntr = 0; cntr < joints.Length; cntr++)
        {
            if (byIndex[cntr] == null)
            {
                retVal[cntr] = new JointRotator();
            }
            else
            {
                retVal[cntr] = new JointRotator()
                {
                    Joint = joints[cntr],
                    Endpoints = byIndex[cntr].
                        SelectMany(o => o.LinkIndices).
                        Distinct(o => o).
                        Select(o => joints[o]).
                        ToArray(),
                };
            }

            retVal[cntr].Initialize();
        }

        return retVal;
    }

    private static string[] GetDupes(IEnumerable<string> strings)
    {
        return strings.
            Where(o => !string.IsNullOrWhiteSpace(o)).
            ToLookup(o => o.ToUpper()).
            Where(o => o.Count() > 1).
            Select(o => o.Key).
            OrderBy(o => o).
            ToArray();
    }

    private static int GetIndex(string id, IK_Joint[] allJoints)
    {
        for (int cntr = 0; cntr < allJoints.Length; cntr++)
        {
            if (allJoints[cntr].IsMatch(id))
                return cntr;
        }

        return -1;
    }

    #endregion
    #region Private Methods - update

    private static void Sweep_FromTarget(Chain chain)
    {
        for (int cntr = 0; cntr < chain.Joints.Length; cntr++)
        {
            if (cntr == 0)
            {
                chain.Joints[cntr].Position = chain.Target.transform.position;
            }
            else if (cntr == chain.Joints.Length - 1 && chain.Joints[cntr].IsAnchored)
            {
                // Don't set anything if the last item is anchored
            }
            else
            {
                chain.Joints[cntr].Position = chain.Joints[cntr - 1].Position + ((chain.Joints[cntr].Position - chain.Joints[cntr - 1].Position).normalized * chain.Joints[cntr].DistanceFromPrevJoint);
            }
        }
    }
    private static void Sweep_TowardTarget(Chain chain)
    {
        for (int cntr = chain.Joints.Length - 2; cntr >= 0; cntr--)
        {
            chain.Joints[cntr].Position = chain.Joints[cntr + 1].Position + ((chain.Joints[cntr].Position - chain.Joints[cntr + 1].Position).normalized * chain.Joints[cntr + 1].DistanceFromPrevJoint);
        }
    }

    private static void ApplyAngleConstraints(Chain chain)
    {
        foreach (var joint in chain.Joints)
        {
            if (joint.InitialAngles.Length == 0)
                continue;

            foreach (var initial in joint.InitialAngles)
                Constrain(joint, initial, chain);
        }
    }

    //TODO: This function will need to mature through a few iterations.  Need min/max angle.  dead zones, forces
    //Also, this is just angle in the current plane of the three points.  Need to store angles relative to the joint's initial orientation, constrain across major plane and orth plane
    //This will help with cases where it flips over the 180 line
    private static void Constrain(ChainJoint joint, AngleLinkConstraint initial, Chain chain)
    {
        const float TOLERANCE = 30f;
        const float MOVEDEGREES = .05f;      // the effective delta will be doubled, because each joint will be rotated by this

        Vector3 line1 = initial.Joint1.Position - joint.Position;
        Vector3 line2 = initial.Joint2.Position - joint.Position;

        float angle = Vector3.Angle(line1, line2);

        if (Math.Abs(initial.Angle - angle) <= TOLERANCE)
            return;

        // Pull the two joints toward the ideal angle
        //    Since these joints are massless, it can't use forces.  Instead choose an arbitrary amount based on how far off it is

        float changeAngle = angle > initial.Angle ?
            MOVEDEGREES :
            -MOVEDEGREES;

        Quaternion delta = Quaternion.AngleAxis(changeAngle, Vector3.Cross(line1, line2).normalized);

        initial.Joint1.Position = joint.Position + (delta * line1);
        initial.Joint2.Position = joint.Position + (Quaternion.Inverse(delta) * line2);
    }

    private static void SyncPositions(ChainJoint[] sameIndex)
    {
        Vector3 average = Math3D.GetAverage(sameIndex.Select(o => o.Position));

        foreach (ChainJoint item in sameIndex)
        {
            item.Position = average;
        }
    }

    #endregion

    #region serialization

    private const string FOLDER = @"Party People\IKMesh";

    [Serializable]
    private class SerializeScene
    {
        // "name|x|y|z"
        public List<string> Joints;

        // "name1|name2"
        public List<string> Links;

        public List<SerializeChain> Chains;
    }
    [Serializable]
    public class SerializeChain
    {
        // The name of the target that this chain is tied to
        public string TargetName;

        // Joints going from target to root
        public List<string> JointNames;
    }

    private static void SaveChains(Chain[] chains, IK_LinkJoints[] links)
    {
        SerializeScene scene = new SerializeScene()
        {
            Joints = chains.
                SelectMany(o => o.Joints).
                ToLookup(o => o.Joint.ID.ToUpper()).
                OrderBy(o => o.Key).
                Select(o => $"{o.Key}|{o.First().Position.x}|{o.First().Position.y}|{o.First().Position.z}").
                ToList(),

            Links = links.
                Select(o => $"{o.JointID_1}|{o.JointID_2}").
                ToList(),

            Chains = chains.
                Select(o => new SerializeChain()
                {
                    TargetName = o.Target.ID,
                    JointNames = o.Joints.
                        Select(p => p.Joint.ID).
                        ToList(),
                }).
                ToList(),
        };

        string filename = DateTime.Now.ToString("yyyyMMdd HHmmss") + " - scene.json";
        filename = Path.Combine(GetSettingsFolder(), filename);

        //NOTE: Without the outer class, the files are just { }
        string serialized = JsonUtility.ToJson(scene, true);

        using (StreamWriter writer = new StreamWriter(filename, false))
        {
            writer.Write(serialized);
        }
    }

    private static string GetSettingsFolder()
    {
        string retVal = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        retVal = Path.Combine(retVal, FOLDER);

        Directory.CreateDirectory(retVal);

        return retVal;
    }

    #endregion

    #region debug drawing

    #region class: DrawJoint

    private class DrawJoint
    {
        public int Index { get; set; }
        public string ID { get; set; }

        public Transform Transform { get; set; }

        public DrawJoint[] Links { get; set; }

        public DebugItem[] LinesToLinks { get; set; }

        public (int, int, float)[] InitialAngles { get; set; }
    }

    #endregion

    private DebugRenderer3D _debug = null;

    private DrawJoint[] _drawJoints = null;

    private void InitDebugDrawing()
    {
        const float THICKNESS = .048f;

        _debug = gameObject.AddComponent<DebugRenderer3D>();

        _drawJoints = Enumerable.Range(0, _chainJointsByIndex.Length).
            Select(o => new DrawJoint()
            {
                Index = o,
                ID = _chainJointsByIndex[o][0].Joint.ID,
                Transform = _joints[o],
            }).
            ToArray();

        foreach (var drawJoint in _drawJoints)
        {
            drawJoint.Links = _chainJointsByIndex[drawJoint.Index].
                SelectMany(o => o.LinkIndices).
                Distinct().
                Select(o => _drawJoints[o]).
                ToArray();

            drawJoint.LinesToLinks = drawJoint.Links.
                Select(o => _debug.AddLine_Pipe(new Vector3(), new Vector3(), THICKNESS, Color.white)).
                ToArray();

            drawJoint.InitialAngles = UtilityCore.GetPairs(drawJoint.Links.Length).
                Select(o => (o.Item1, o.Item2, Vector3.Angle(drawJoint.Links[o.Item1].Transform.position - drawJoint.Transform.position, drawJoint.Links[o.Item2].Transform.position - drawJoint.Transform.position))).
                ToArray();
        }
    }

    private void UpdateDebugDrawing()
    {
        foreach (DrawJoint joint in _drawJoints)
        {
            for (int cntr = 0; cntr < joint.Links.Length; cntr++)
            {
                Vector3 line = joint.Links[cntr].Transform.position - joint.Transform.position;

                DebugRenderer3D.AdjustLinePositions(joint.LinesToLinks[cntr], joint.Transform.position, joint.Transform.position + (line * .45f));
            }
        }
    }

    #endregion
}
