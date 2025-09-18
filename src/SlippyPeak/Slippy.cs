using BepInEx;
using BepInEx.Logging;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace SlippyPeak
{
    [BepInPlugin("com.slippy.peak.client", "Slippy Client", "1.0.0")]
    public class SlippyClientOnly : BaseUnityPlugin
    {
        internal static ManualLogSource Log { get; private set; } = null!;
        private float sinceLastSlip = 0f;
        private float slipAllowance = 7f;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("Slippy mod loaded!");
        }

        private void Update()
        {
            Character character = Character.localCharacter;


            float stamPercent = (character.GetTotalStamina() / character.GetMaxStamina()) *100;

            // you need to have a chance to recover stamina
            // we don't want to slip IF:
            // player is not on the ground
            // player is stationary
            sinceLastSlip += Time.deltaTime;
            if (stamPercent < 10
                && character.data.isGrounded
                && sinceLastSlip >= slipAllowance)
            {
                sinceLastSlip = 0;
                SlipLocalPlayer(Character.localCharacter);
            }
        }

        private void SlipLocalPlayer(Character player)
        {
            Log.LogInfo($"Slipping local player!");

            player.refs.view.RPC("RPCA_Fall", RpcTarget.All, 2f);

            Vector3 dir = player.data.lookDirection_Flat;

            player.refs.view.RPC("RPCA_AddForceToBodyPart", RpcTarget.All, BodypartType.Foot_R, (dir + Vector3.up) * 200f, Vector3.zero);
            player.refs.view.RPC("RPCA_AddForceToBodyPart", RpcTarget.All, BodypartType.Foot_L, (dir + Vector3.up) * 200f, Vector3.zero);
            player.refs.view.RPC("RPCA_AddForceToBodyPart", RpcTarget.All, BodypartType.Hip, Vector3.up * 1500f, Vector3.zero);
            player.refs.view.RPC("RPCA_AddForceToBodyPart", RpcTarget.All, BodypartType.Head, dir * -300f, Vector3.zero);
    
        }
    }
}
