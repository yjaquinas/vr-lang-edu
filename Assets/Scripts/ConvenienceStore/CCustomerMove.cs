/*
 * 손님의 이동
 * 
 * 생성 -> 스크린도어 앞으로 이동 -> 2초 대기 -> 카운터 앞으로 이동
 * 
 * 유저의 STT듣고, 해당 선반으로 이동
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CCustomerMove : MonoBehaviour
{
    private int _FSMstate;
    public int FSMstate { get { return _FSMstate; } }

    private CGCPSTT _gcpSTT;
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private CCustomerCheckItem _customerCheckItem;

    [SerializeField] private GameObject[] _outfits;
    [SerializeField] private GameObject[] _orderItems;
    [SerializeField] private CStoreManager _storeManager;

    [SerializeField] private Transform _slidingDoorInsideTransform;
    [SerializeField] private Transform _slidingDoorOutsideTransform;
    [SerializeField] private Transform _customerOrderTransform;
    [SerializeField] private Transform _wayHomeTransform;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform[] _shelfTransforms;
    [SerializeField] private Transform[] _shelfFrontTransforms;
    [SerializeField] private GameObject _resultCanvases;
    [SerializeField] private GameObject[] _playerHands;
    [SerializeField] private GameObject[] _playerPointers;
    [SerializeField] private GameObject _vivePointers;


    private Vector3 _destination;
    private bool _hasArrived = false;

    private int _shelfNumber;

    private bool _takeAnimationPlayed;
    private bool _okayAnimationPlayed;

    public UnityEvent OnArrival;
    public UnityEvent OnItemNotFound;
    public UnityEvent OnOkay;


    private void Awake()
    {
        // 복장 랜덤 설정
        int randomNumber = Random.Range(0, _outfits.Length);
        _outfits[randomNumber].SetActive(true);

        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _gcpSTT = GetComponent<CGCPSTT>();
        _customerCheckItem = GetComponent<CCustomerCheckItem>();

        _FSMstate = 0;
        _shelfNumber = 0;
        _takeAnimationPlayed = false;
        _okayAnimationPlayed = false;
    }

    private void Start()
    {
        _destination = _slidingDoorOutsideTransform.position;
        Invoke("SetDestination", 1f);
    }

    private void Update()
    {
        // 이동시 걷기 애니메이션 실행
        if(_navMeshAgent.velocity.magnitude >= 0.1f)
        {
            _animator.SetBool("Walking", true);
        }
        else
        {
            _animator.SetBool("Walking", false);
        }

        switch(_FSMstate)
        {
            case 0:
                if(_navMeshAgent.velocity.magnitude == 0)   // 카운터 도착
                {
                    transform.LookAt(new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z));
                    if(!_hasArrived && (Vector3.Distance(transform.position, _customerOrderTransform.position) < 1))
                    {
                        _hasArrived = true;
                        OnArrival.Invoke();
                        _FSMstate++;
                    }
                }
                break;
            case 1:
                if(_gcpSTT._isGCPReady) // 유저의 음성 캐치
                {
                    // STT에서 받은 transcript를 읽어서,
                    // Nav Mesh 목적지를 선반 위치로 지정해주기
                    string transcript = _gcpSTT._transcript;
                    transcript.ToLower();
                    transcript = transcript.Replace("1", "one");
                    transcript = transcript.Replace("won", "one");
                    transcript = transcript.Replace("2", "two");
                    transcript = transcript.Replace("to", "two");
                    transcript = transcript.Replace("3", "three");
                    transcript = transcript.Replace("4", "four");
                    transcript = transcript.Replace("for", "four");
                    transcript = transcript.Replace("5", "five");
                    if(transcript.Contains("number one")) _shelfNumber = 0;
                    if(transcript.Contains("number two")) _shelfNumber = 1;
                    if(transcript.Contains("number three")) _shelfNumber = 2;
                    if(transcript.Contains("number four")) _shelfNumber = 3;
                    if(transcript.Contains("number five")) _shelfNumber = 4;

                    _navMeshAgent.SetDestination(_shelfFrontTransforms[_shelfNumber].position);
                    if(_navMeshAgent.velocity.magnitude > 0)   // 선반으로 출발
                    {
                        _FSMstate++;
                    }
                }
                break;
            case 2:
                if(_navMeshAgent.velocity.magnitude == 0)   // 선반 앞 도착
                {
                    transform.LookAt(new Vector3(_shelfTransforms[_shelfNumber].position.x, transform.position.y, _shelfTransforms[_shelfNumber].position.z));
                    _FSMstate++;
                    _takeAnimationPlayed = false;
                }
                break;
            case 3:
                // 여기선 물건 뒤지는 애니메이션 추가해주기
                //
                if(!_takeAnimationPlayed)
                {
                    _animator.SetTrigger("Take");
                    _takeAnimationPlayed = true;
                }

                // 물건이 있으면 손에 추가
                // 다 쥐어놓고 비활성화 되어있음
                // 물건이 있으면 해당 물건 활성화
                // 1초 딜레이 주기
                if(_customerCheckItem.IsItemOnShelf)
                {
                    Invoke("ActivateItem", 1f);
                }

                // 2초후에 카운터로 이동
                _destination = _customerOrderTransform.position;
                Invoke("SetDestination", 2f);
                if(_navMeshAgent.velocity.magnitude > 0)   // 선반에서 출발
                {
                    _FSMstate++;
                }
                break;
            case 4:
                // 다시 카운터 도착
                if(_navMeshAgent.velocity.magnitude == 0)
                {
                    transform.LookAt(new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z));
                    if(_customerCheckItem.IsItemOnShelf)
                    {
                        // 선반에 물건이 있었으면
                        // "ㄳㄳ" 하고 다음 스테이트로
                        if(!_okayAnimationPlayed)
                        {
                            OnOkay.Invoke();
                        }
                        _okayAnimationPlayed = true;
                        Invoke("IncrementState", 1.66f);
                    }
                    else
                    {
                        // 선반에 물건이 없었으면
                        // "거기에 물건 없어요! 하고 다시 물어보기
                        OnItemNotFound.Invoke();
                        _gcpSTT._isGCPReady = false;
                        _FSMstate = 1;
                    }
                }
                break;
            case 5:
                // 자동문 앞으로 가기
                if(Vector3.Distance(_slidingDoorInsideTransform.position, transform.position) < 1)    // 자동문에 가까워 졌으면, 
                {
                    _FSMstate++;
                    _resultCanvases.SetActive(true);
                    for(int i = 0; i < 2; i++)
                    {
                        _playerHands[i].SetActive(false);
                        _playerPointers[i].SetActive(true);
                    }
                    _vivePointers.SetActive(true);
                }
                else
                {
                    _navMeshAgent.SetDestination(_slidingDoorInsideTransform.position);
                }
                break;
            case 6:
                // 집으로 가기
                if(Vector3.Distance(_slidingDoorInsideTransform.position, transform.position) < 1)    // 자동문에 가까워 졌으면, 
                {
                    _destination = _wayHomeTransform.position;
                    Invoke("SetDestination", 2f);
                }
                break;
            default:
                break;
        }
    }

    public void ActivateItem()
    {
        _orderItems[(int)GetComponent<CCustomerOrder>().OrderItem].SetActive(true);
        _storeManager.DisableFlashShelf();
        CancelInvoke("ActivateItem");
    }

    private void IncrementState()
    {
        _FSMstate++;
        CancelInvoke("IncrementState");
    }

    // nav mesh 이동 위치 지정
    // 본 메소드 호출 전 _destination 세팅 필수
    // invoke로 몇초 후에 움직이려고 메소드화함
    private void SetDestination()
    {
        _navMeshAgent.SetDestination(_destination);
        CancelInvoke("SetDestination");
    }

    // 처음에 가게 입장시, 문열리기 기다리고, 카운터앞으로 목적지 변경
    private void OnTriggerEnter(Collider other)
    {
        if(!_hasArrived && other.name == "SlidingDoors")
        {
            _destination = _customerOrderTransform.position;
            Invoke("SetDestination", 3f);
        }
    }
}
