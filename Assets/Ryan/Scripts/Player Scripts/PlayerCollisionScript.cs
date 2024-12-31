using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;
namespace PlayerScripts.Helpers {
public class PlayerCollisionScript : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Ball"))
            {
                GameObject ballObject = collision.gameObject;
                ballObject.transform.rotation = gameObject.transform.rotation;
                ballObject.transform.SetParent(gameObject.transform);
                GameManager.Instance._ballrb.isKinematic = true;
                ballObject.transform.position = gameObject.transform.localPosition;
                ballObject.transform.position = gameObject.transform.localPosition +
                                                gameObject.transform.TransformDirection(new Vector3(0, 0, 1.1f));
                GameManager.Instance.ballHeldByPlayer = true;
                GameManager.Instance.objectThatHasBall = gameObject;
            }
        }
    }
}