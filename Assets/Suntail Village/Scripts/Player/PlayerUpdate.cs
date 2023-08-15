using UnityEngine;
using UnityEngine.UI;

namespace Suntail
{
    public class PlayerUpdate : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider;
        [SerializeField] private Text _hpText;
        [SerializeField] private Slider _mpSlider;
        [SerializeField] private Text _mpText;
        [SerializeField] private PlayerController _stats;

        private void Start()
        {
            Transform hpBarTransform = GameObject.Find("HpBar").transform;
            _hpSlider = hpBarTransform.GetComponent<Slider>();
            _hpSlider.minValue = 0;

            Transform mpBarTransform = GameObject.Find("MpBar").transform;
            _mpSlider = mpBarTransform.GetComponent<Slider>();
            _mpSlider.minValue = 0;

            // PlayerController 컴포넌트 할당
            _stats = GetComponent<PlayerController>();
        }

        private void UpdateUI()
        {
            _hpSlider.maxValue = _stats._maxHP;
            _mpSlider.maxValue = _stats._maxMP;
            _hpSlider.value = _stats._hp;
            _mpSlider.value = _stats._mp;
            _hpText.text = $"{_stats._hp}/{_stats._maxHP}";
            _mpText.text = $"{_stats._mp}/{_stats._maxMP}";
        }

        private void Update()
        {
            UpdateUI();
        }
    }
}