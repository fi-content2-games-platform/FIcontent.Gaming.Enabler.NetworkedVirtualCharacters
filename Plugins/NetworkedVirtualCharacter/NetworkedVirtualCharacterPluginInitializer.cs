using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIVES;
using ClientManagerPlugin;
using TerminalPlugin;

namespace NVC
{
    public class NetworkedVirtualCharacterPluginInitializer : IPluginInitializer
    {
        private bool verboseMode;
		private Entity dragon;


        #region IPluginInitializer implementation

        public string Name
        {
            get { return "NetworkedVirtualCharacter"; }
        }

        public List<string> PluginDependencies
        {
            get { return new List<string>(); }
        }

        public List<string> ComponentDependencies
        {
            get { return new List<string>(); }
        }

        public void Initialize()
        {
            DefineComponents();

            PluginManager.Instance.AddPluginLoadedHandler("ClientManager", RegisterClientServices);
            PluginManager.Instance.AddPluginLoadedHandler("Terminal", RegisterTerminalCommands);
        }

        public void Shutdown()
        {
        }

        #endregion

        void DefineComponents()
        {
            ComponentDefinition skeleton = new ComponentDefinition("skeleton");
            skeleton.AddAttribute<List<Vector>>("translations", new List<Vector>());
            skeleton.AddAttribute<List<Quat>>("rotations", new List<Quat>());
            ComponentRegistry.Instance.Register(skeleton);
        }

        //public struct BoneInfo {
        //    public int parent;
        //}

        //public struct VertexInfo 
        //{
        //    invBindPose;
        //    boneIdx;
        //    boneWeight;
        //}

        private void RegisterTerminalCommands()
        {
            Terminal.Instance.RegisterCommand("verbose", "Enables verbose mode", false,
                PrintVerboseMode, new List<string> { "vb" });

            Terminal.Instance.RegisterCommand("boneInfos", "Prints bone infos", false,
              PrintBoneInfo, new List<string> { "bi" });

            Terminal.Instance.RegisterCommand("listEntities", "Lists entity guids", false,
             ListEntitites, new List<string> { "le" });
        }

        private void ListEntitites(string commandLine)
        {
            Terminal.Instance.WriteLine("Entities: ");
            int c = 0;
            foreach (var e in World.Instance)
            {
                Terminal.Instance.WriteLine(c++ + " -> " + e.Guid.ToString());
            }
        }

        private void PrintBoneInfo(string commandLine)
        {
            int bIdx, eIdx;
            if (GetBoneIdx(commandLine, out eIdx, out bIdx))
            {
                var entity = World.Instance.ToArray()[eIdx];
                var pos = (List<Vector>)entity["skeleton"]["translations"];
                var rot = (List<Quat>)entity["skeleton"]["rotations"];

                if (bIdx >= 0)
                {
                    Terminal.Instance.WriteLine(string.Format("translation: [{0}, {1}, {2}]", pos[bIdx].x, pos[bIdx].y, pos[bIdx].z));
                    Terminal.Instance.WriteLine(string.Format("rotation: [{0}, {1}, {2}, {3}]", rot[bIdx].x, rot[bIdx].y, rot[bIdx].z, rot[bIdx].w));
                }
                else
                {
                    Terminal.Instance.WriteLine("translation count: " + pos.Count);
                    Terminal.Instance.WriteLine("rotation count: " + rot.Count);
                }
            }
            else
            {
                Terminal.Instance.WriteLine("usage: bi <entity idx> <bone idx>");
            }
        }

        private bool GetBoneIdx(string commandLine, out int eIdx, out int bIdx)
        {
            bIdx = eIdx = -1;
            var split = commandLine.Split(null);

            if (split.Length < 2)
                return false;
            else if (split.Length == 2 && int.TryParse(split[1], out eIdx))
                return true;
            else if (split.Length > 2 && int.TryParse(split[1], out eIdx) && int.TryParse(split[2], out bIdx))
                return true;
            else
                return false;
        }

        private void PrintVerboseMode(string commandLine)
        {
            verboseMode = !verboseMode;
            Terminal.Instance.WriteLine("verbose mode: " + (verboseMode ? "on" : "off"));
        }

        void RegisterClientServices()
        {
            ClientManager.Instance.RegisterClientService("NVC", true, new Dictionary<string, Delegate> {
                {"updateBones", (Action<string, List<Vector>, List<Quat>, int>) UpdateBones} //,
                //{"getDragon", (Func<string>) GetDragon}
                //{"addSkeleton", (Action<string, List<BoneInfo>, List<VertexInfo>, int>) addSkeleton}
            });
        }

        private void UpdateBones(string guid, List<Vector> translations, List<Quat> rotations, int timestamp)
        {
            var entity = World.Instance.FindEntity(guid);
			//var entity = dragon;
			if (entity == null)
				return;
				
            entity["skeleton"]["translations"] = translations;
            entity["skeleton"]["rotations"] = rotations;

            // We currently ignore timestamp, but may it in the future to implement dead reckoning.

            if (verboseMode)
            {
                Terminal.Instance.WriteLine("received update " + guid);
            }
        }

        //private void addSkeleton(string guid, List<BoneInfo> boneInfo, List<VertexInfo> vertexInfo, int timestamp)
        //{
        //    var entity = World.Instance.FindEntity(guid);

        //    //TODO: modify mesh resource by injecting forwardKinematics, set skeleton bones to initial pose

        //    // We currently ignore timestamp, but may it in the future to implement dead reckoning.
        //}

        // private string GetDragon()
        // {
			// if (dragon == null) {
				// dragon = new Entity();
				// dragon["mesh"]["uri"] = "resources/dragon/dragon.xml";
				// dragon["mesh"]["scale"] = new Vector(5.0f);

				// World.Instance.Add(dragon);

				// TODO: inject skeleton
				// dragon["skeleton"]["translations"] = new List<Vector> {
					// new Vector(0,0,0)
				// };
				// dragon["skeleton"]["rotations"] = new List<Quat> {
					// new Quat()
				// };
			// }

            // return dragon.Guid.ToString();
        // }

    }
}
