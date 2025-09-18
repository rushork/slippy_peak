using BepInEx;
using BepInEx.Logging;
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

            player.RPCA_Fall(2f);

            Rigidbody footR = player.GetBodypartRig(BodypartType.Foot_R);
            Rigidbody footL = player.GetBodypartRig(BodypartType.Foot_L);
            Rigidbody hip   = player.GetBodypartRig(BodypartType.Hip);
            Rigidbody head  = player.GetBodypartRig(BodypartType.Head);

            Vector3 dir = player.data.lookDirection_Flat;

            footR.AddForce((dir + Vector3.up) * 200f, ForceMode.Impulse);
            footL.AddForce((dir + Vector3.up) * 200f, ForceMode.Impulse);
            hip.AddForce(Vector3.up * 1500f, ForceMode.Impulse);
            head.AddForce(dir * -300f, ForceMode.Impulse);

        }
    }
}
