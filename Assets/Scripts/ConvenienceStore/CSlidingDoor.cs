/*
 * 유리문 슬라이딩
 * n초 걸려서 오픈
 * n초 홀드
 * n초 걸려서 클로즈
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSlidingDoor : MonoBehaviour
{
    enum SLIDING_DOOR_FSM
    {
        IDLE, OPENING, OPENED, CLOSING
    }

    [SerializeField] private SLIDING_DOOR_FSM _fsm;

    [SerializeField] private float _slidingSpeed = 0.4f;
    [SerializeField] private float _slidingDuration = 3f;

    private float _slidingTimer;

    private void Awake()
    {
        _fsm = SLIDING_DOOR_FSM.IDLE;
    }

    // 문열어
    // 닫혀있으면 열기 시작
    // 열려있으면 열려있는 타이머 초기화
    // 열리는 중이면 씹기
    // 닫히는 중이면 다시 열기
    public void OnSlideOpen()
    {
        if(_fsm == SLIDING_DOOR_FSM.IDLE)
        {
            _slidingTimer = 0;
            _fsm = SLIDING_DOOR_FSM.OPENING;
        }
        else if(_fsm == SLIDING_DOOR_FSM.OPENED)
        {
            _slidingTimer = 0;
        }
        else if(_fsm == SLIDING_DOOR_FSM.CLOSING)
        {
            _fsm = SLIDING_DOOR_FSM.OPENING;
            _slidingTimer = _slidingDuration - _slidingTimer;
        }
    }

    private void Update()
    {
        _slidingTimer += Time.deltaTime;

        switch(_fsm)
        {
            case SLIDING_DOOR_FSM.IDLE:
                break;
            case SLIDING_DOOR_FSM.OPENING:
                if(_slidingTimer > _slidingDuration)
                {
                    _fsm++;
                    _slidingTimer = 0;
                }

                if(name.Contains("Left"))
                {
                    transform.Translate(-transform.right * _slidingSpeed * Time.deltaTime, Space.World);
                }
                else
                {
                    transform.Translate(transform.right * _slidingSpeed * Time.deltaTime, Space.World);
                }
                break;
            case SLIDING_DOOR_FSM.OPENED:
                if(_slidingTimer > _slidingDuration)
                {
                    _fsm++;
                    _slidingTimer = 0;
                }

                break;
            case SLIDING_DOOR_FSM.CLOSING:
                if(_slidingTimer > _slidingDuration)
                {
                    _fsm = SLIDING_DOOR_FSM.IDLE;
                }

                if(name.Contains("Left"))
                {
                    transform.Translate(transform.right * _slidingSpeed * Time.deltaTime, Space.World);
                }
                else
                {
                    transform.Translate(-transform.right * _slidingSpeed * Time.deltaTime, Space.World);
                }
                break;
        }


    }
}
