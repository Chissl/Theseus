using System.Linq;
using Assets.Scripts.Models.Towers;
using Assets.Scripts.Unity.Display;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using UnityEngine;

namespace Theseus.Displays
{
    public class OrbitalStrikeDisplay : ModDisplay
    {
        // Copy the Boomerang Monkey display
        public override string BaseDisplay => "8b5396968d634c44c9d700cff5dfc552";

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
# if DEBUG
            node.PrintInfo();
            node.SaveMeshTexture();
#endif
            //SetMeshTexture(node, Name);
            SetMeshOutlineColor(node, new Color(0, 66, 255));
            SetMeshTexture(node, Name);
            node.RemoveBone("GroundZero");
        }
    }
}