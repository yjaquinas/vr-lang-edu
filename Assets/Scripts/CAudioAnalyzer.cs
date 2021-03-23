/*
 * 오디오의 현재 볼륨/음의 높이 추출
 */
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LineRenderer))]
public class CAudioAnalyzer : MonoBehaviour
{
    private const int AMP_GRAPH_SAMPLE_SIZE = 1024;             // 그래프 샘플링 사이즈 2^n이여야함
    private const float AMP_GRAPH_SAMPLING_FREQ = 100f;         // 그래프 샘플링 프리퀀시 (Hz)
    [SerializeField] private float _ampGraphWidthRatio = 0.01f; // 그래프 그리기 가로길이 배율
    [SerializeField] private float _ampGraphHeightRatio = 5;    // 그래프 그리기 세로길이 배율
    private const float REFERENCE_VALUE_FOR_DECIBEL = 0.1f;
    private const int PITCH_GRAPH_SAMPLE_SIZE = 256;            // 그래프 샘플링 사이즈 2^n이여야함
    private const float MAXIMUM_SPECTRUM_VAL = 0.02f;
    private const float MINIMUM_DB = -100f;
    private const float RMS_AMP_EMA_RATIO = 0.9f;
    private const float NEGLIGIBLE_RMS_AMP = 0.01f;

    [SerializeField] private AudioSource _as;
    public LineRenderer _lineRenderer;

    [SerializeField] private LineRenderer _tempLineRenderer;
    [SerializeField] private LineRenderer _tempLineRenderer2;

    [SerializeField] private Vector3 _graphOrigin;

    // 유니티 인스펙터에서 값을 보려고 serialize해둠
    [SerializeField] private float _rmsAmp;
    [SerializeField] private float _rmsAmpEma = 0;
    [SerializeField] private float _decibel;
    [SerializeField] private float _pitch;
    public float RmsAmp
    {
        get { return _rmsAmp; }
    }
    public float Decibel
    {
        get { return _decibel; }
    }

    float[] _ampSamples;
    private float[] _spectrum;
    private float _samplingFrequency;

    private void Awake()
    {
        _ampSamples = new float[AMP_GRAPH_SAMPLE_SIZE];
        //_samplingFrequency = AudioSettings.outputSampleRate;
        //_spectrum = new float[PITCH_GRAPH_SAMPLE_SIZE];
        //_as = GetComponent<AudioSource>(); 인스펙터에서 연결함
        //_lineRenderer = GetComponent<AudioSource>(); 인스펙터에서 연결함
    }

    private void Start()
    {
        InvokeRepeating("AnalyzeAudio", 0, 1 / AMP_GRAPH_SAMPLING_FREQ);    // 매 프레임마다 하면 너무 무거움
    }

    private void Update()
    {
        //AnalyzeAudio();
    }

    // 오디오소스가 재생중이라면
    // 오디오의 진폭을 측정
    // 그래프 그리기
    private void AnalyzeAudio()
    {
        if(!_as.isPlaying) return;  // 재생중이 아님
        //=====================================================================
        // 증폭치 RSM, Decibel 계산
        _as.GetOutputData(_ampSamples, 0);     // 샘플을 추출
        float sum = 0;
        for(int i = 0; i < AMP_GRAPH_SAMPLE_SIZE; i++)
        {
            sum += _ampSamples[i] * _ampSamples[i];   // 합(샘플값^2)
        }
        _rmsAmp = Mathf.Sqrt(sum / AMP_GRAPH_SAMPLE_SIZE);                  // rms = 제곱근(합(샘플값^2)/합갯수)
        _decibel = 20 * Mathf.Log10(_rmsAmp / REFERENCE_VALUE_FOR_DECIBEL); // RMS값에서 데시벨 변환
        if(_decibel < MINIMUM_DB) _decibel = MINIMUM_DB;                    // 데시벨 최소치 -100 dB


        _rmsAmpEma *= RMS_AMP_EMA_RATIO;
        _rmsAmpEma += _rmsAmp * (1 - RMS_AMP_EMA_RATIO);

        if(_rmsAmpEma < NEGLIGIBLE_RMS_AMP) return; // 소리가 없으면 그래프 그리지 않기


        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1,
            new Vector3(_graphOrigin.x + _lineRenderer.positionCount * _ampGraphWidthRatio,
                _graphOrigin.y + _rmsAmpEma * _ampGraphHeightRatio,
                _graphOrigin.z));

        if(_tempLineRenderer != null)
        {
            _tempLineRenderer.positionCount++;
            _tempLineRenderer.SetPosition(_tempLineRenderer.positionCount - 1,
                new Vector3(_graphOrigin.x + _tempLineRenderer.positionCount * _ampGraphWidthRatio,
                    _graphOrigin.y + 0.5f + _ampSamples[0] * _ampGraphHeightRatio * 0.7f,
                    _graphOrigin.z));
        }
        if(_tempLineRenderer2 != null)
        {
            _tempLineRenderer2.positionCount++;
            _tempLineRenderer2.SetPosition(_tempLineRenderer2.positionCount - 1,
                new Vector3(_graphOrigin.x + _tempLineRenderer2.positionCount * _ampGraphWidthRatio,
                    _graphOrigin.y + _rmsAmp * _ampGraphHeightRatio,
                    _graphOrigin.z));
        }

        ////=====================================================================
        //// 스펙트럼을 이용해서 음의 높낮이 계산
        //_as.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);    // 스펙트럼 추출
        //float maxValue = 0;
        //int maxIndex = 0;
        //// 샘플 내에서 최대치 값과 인덱스 검색
        //for(int i = 0; i < SAMPLE_SIZE; i++)
        //{
        //    if(!(_spectrum[i] > maxValue) || !(_spectrum[i] > MAXIMUM_SPECTRUM_VAL)) continue;  // 이전 최대치보다 작거나, 최대치를 넘김
        //    maxValue = _spectrum[i];    // 최대치 값
        //    maxIndex = i;               // 최대치 인덱스
        //}
        //float frequencySample = maxIndex;    // float 피치 최대 인덱스 = int 스펙트럼 최대치의 인덱스. 보간할거라서 float로 필요
        //// 양옆의 (엔덱스 +/-1씩의 값을 이용해서 보간)
        //if(frequencySample > 0 && frequencySample < SAMPLE_SIZE - 1)
        //{
        //    float spectrumLeft = _spectrum[maxIndex - 1] / _spectrum[maxIndex];
        //    float spectrumRight = _spectrum[maxIndex + 1] / _spectrum[maxIndex];
        //    frequencySample += 0.5f * (spectrumRight * spectrumRight - spectrumLeft * spectrumLeft);
        //}
        //_pitch = frequencySample * (_samplingFrequency / 2) / SAMPLE_SIZE;   // 인덱스 -> 주파수 변환
    }
}