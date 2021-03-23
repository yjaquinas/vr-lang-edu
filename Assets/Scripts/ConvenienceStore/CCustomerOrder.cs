/*
 * 손님 주문
 * 예제중 한개 랜덤 선택
 * !! 중요!! 물품 목록과 오디오 목록이 매치되어야함!
 *  예제는 오디오 클립 여럿 보유
 *  해당 오디오 클립 인덱스와 매칭되는 받을 물품 목록 보유
 *      
 * 호출되면 오디오 재생해주기
 * 호출되면 주문 제대로 나왔는지 체크해주기
 *  제대로 나왔으면, 새 주문 업데이트
 */
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CCustomerOrder : MonoBehaviour
{
    public enum ORDER_ITEM
    {
        BANANA, APPLE,
        CHAMPAGNE, WATER, BEER,
        ROPE, BATTERY, CLOCK, TEDDYBEAR,
        BOOK,
        MUG, COOKIE, PIZZA
    }

    private int _FSMstate;
    private CGCPSTT _gcpSTT;

    [SerializeField] private CCustomerMove _customerMove;
    private AudioSource _audioSource;
    private Animator _animator;
    [SerializeField] private AudioClip _thankYouAudioClip;
    [SerializeField] private AudioClip _okayClip;
    [SerializeField] private AudioClip _itWasNotThereClip;
    [SerializeField] private List<AudioClip> _orderAudioClipList;
    [SerializeField] private GameObject _resultUI;
    [SerializeField] private ORDER_ITEM _orderItem;
    public ORDER_ITEM OrderItem { get { return _orderItem; } }
    private int _shelfNumber;

    private CStoreManager _storeManager;

    private bool _startedAsking = false;


    // 주문할 물건 랜덤 선택
    // 선택된 물건 선반 번호 연결
    // 오디오 연결
    // 문장 연결
    private void Awake()
    {
        _FSMstate = 0;
        _gcpSTT = GetComponent<CGCPSTT>();


        _orderItem = (ORDER_ITEM)Random.Range(0, _orderAudioClipList.Count);
        switch(_orderItem)
        {
            case ORDER_ITEM.BANANA:
            case ORDER_ITEM.APPLE:
                _shelfNumber = 0;
                break;
            case ORDER_ITEM.CHAMPAGNE:
            case ORDER_ITEM.WATER:
            case ORDER_ITEM.BEER:
                _shelfNumber = 1;
                break;
            case ORDER_ITEM.ROPE:
            case ORDER_ITEM.BATTERY:
            case ORDER_ITEM.CLOCK:
            case ORDER_ITEM.TEDDYBEAR:
                _shelfNumber = 2;
                break;
            case ORDER_ITEM.BOOK:
                _shelfNumber = 3;
                break;
            case ORDER_ITEM.MUG:
            case ORDER_ITEM.COOKIE:
            case ORDER_ITEM.PIZZA:
                _shelfNumber = 4;
                break;
            default:
                _shelfNumber = 0;
                break;
        }

        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = _orderAudioClipList[(int)_orderItem];

        _animator = GetComponent<Animator>();

        // UI에 띄울 문장
        string[] englishSentence = new string[1], koreanSentence = new string[1];
        switch((int)_orderItem)
        {
            case 0:
                englishSentence[0] = "Hi, where can I find bananas here?";
                koreanSentence[0] = "안녕하세요, 여기 바나나가 어디에 있나요?";
                break;
            case 1:
                englishSentence[0] = "Hi, where can I find apples here?";
                koreanSentence[0] = "안녕하세요, 여기 사과가 어디에 있나요?";
                break;
            case 2:
                englishSentence[0] = "Hi, where can I find champagnes here?";
                koreanSentence[0] = "안녕하세요, 여기 샴페인이 어디에 있나요?";
                break;
            case 3:
                englishSentence[0] = "Hi, where can I find water bottles here?";
                koreanSentence[0] = "안녕하세요, 여기 물이 어디에 있나요?";
                break;
            case 4:
                englishSentence[0] = "Hi, where can I find beer here?";
                koreanSentence[0] = "안녕하세요, 여기 맥주가 어디에 있나요?";
                break;
            case 5:
                englishSentence[0] = "Hi, where can I find ropes here?";
                koreanSentence[0] = "안녕하세요, 여기 밧줄이 어디에 있나요?";
                break;
            case 6:
                englishSentence[0] = "Hi, where can I find batteries here?";
                koreanSentence[0] = "안녕하세요, 여기 건전지가 어디에 있나요?";
                break;
            case 7:
                englishSentence[0] = "Hi, where can I find clocks here?";
                koreanSentence[0] = "안녕하세요, 여기 시계가 어디에 있나요?";
                break;
            case 8:
                englishSentence[0] = "Hi, where can I find teddybears here?";
                koreanSentence[0] = "안녕하세요, 여기 곰인형이 어디에 있나요?";
                break;
            case 9:
                englishSentence[0] = "Hi, where can I find books here?";
                koreanSentence[0] = "안녕하세요, 여기 책이 어디에 있나요?";
                break;
            case 10:
                englishSentence[0] = "Hi, where can I find mugs here?";
                koreanSentence[0] = "안녕하세요, 여기 머그잔이 어디에 있나요?";
                break;
            case 11:
                englishSentence[0] = "Hi, where can I find cookies here?";
                koreanSentence[0] = "안녕하세요, 여기 쿠키가 어디에 있나요?";
                break;
            case 12:
                englishSentence[0] = "Hi, where can I find pizza here?";
                koreanSentence[0] = "안녕하세요, 여기 피자가 어디에 있나요?";
                break;
            default:
                englishSentence[0] = "Awesome.";
                koreanSentence[0] = "참 잘햇어요.";
                break;
        }
        _resultUI.GetComponent<CPlayResultManager>().setSentence(englishSentence, true, true, 0);   // left panel
        _resultUI.GetComponent<CPlayResultManager>().setSentence(koreanSentence, false, true, 0);   // right panel
    }

    private void Start()
    {
        _storeManager = GameObject.Find("StoreManager").GetComponent<CStoreManager>();
    }

    private void Update()
    {

        switch(_FSMstate)
        {
            case 0:
                // 물어본 후에, 유저의 답변 듣기
                if(_startedAsking)
                {
                    if(!_audioSource.isPlaying)
                    {
                        _startedAsking = false;
                        _gcpSTT.OnSTTRecordStart();
                        _FSMstate = 1;
                    }
                }
                break;
            case 1:
                if(_gcpSTT._isGCPReady) // 유저의 음성 받아쓰기 완료
                {
                    _audioSource.clip = _okayClip;
                    _audioSource.Play();
                    _FSMstate = 2;
                }
                break;
        }
    }

    public void PlayOrder()
    {
        _audioSource.clip = _orderAudioClipList[(int)_orderItem];
        _audioSource.time = 0f;
        _audioSource.Play();
        _animator.SetTrigger("Handshake");
        _storeManager.FlashShelf(_shelfNumber);
        _startedAsking = true;
    }

    public void PlayNotFound()
    {
        _audioSource.clip = _itWasNotThereClip;
        _audioSource.time = 0f;
        _audioSource.Play();
        _animator.SetTrigger("No");
        Invoke("PlayOrderAgain", 1.5f);
    }

    public void PlayOrderAgain()
    {
        _audioSource.clip = _orderAudioClipList[(int)_orderItem];
        _audioSource.time = 0.6f;
        _audioSource.Play();
        _storeManager.FlashShelf(_shelfNumber);
        _FSMstate = 0;
        _gcpSTT._isGCPReady = false;
        _startedAsking = true;
    }

    public void PlayThankYou()
    {
        _audioSource.clip = _thankYouAudioClip;
        _audioSource.time = 0f;
        _audioSource.Play();
        _animator.SetTrigger("Yes");
    }

    // 결과창 띄우기
    private void ShowResultUI()
    {
        _resultUI.SetActive(true);
        GameObject.Find("GameManager").GetComponent<GameManager>().TurnOnVivePointers();
    }
}
