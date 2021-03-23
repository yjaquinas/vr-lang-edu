/*
 * 컨베이어벨트
 * 트리거위의 쟁반들이 컨배이어벨트의 forward방향으로 이동 합니다
 */
using UnityEngine;

public class CConveyorBelt : MonoBehaviour
{
    [SerializeField] private float _speed;

    private void OnCollisionStay(Collision collision)
    {
        if(collision.gameObject.CompareTag("Tray"))
        {
            collision.gameObject.transform.Translate(transform.forward * _speed * Time.deltaTime, Space.World);
        }
    }
}
