using UnityEngine;
using UnityEngine.UI;

namespace Suntail
{
    public class PlayerUpdateFull : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider1;
        [SerializeField] private Text _hpText1;
        [SerializeField] private Slider _mpSlider1;
        [SerializeField] private Text _mpText1;
        [SerializeField] private PlayerController _stats1;

        private void Start()
        {
            Transform hpBarTransform = GameObject.Find("HpBar2").transform;
            _hpSlider1 = hpBarTransform.GetComponent<Slider>();
            _hpSlider1.minValue = 0;

            Transform mpBarTransform = GameObject.Find("MpBar2").transform;
            _mpSlider1 = mpBarTransform.GetComponent<Slider>();
            _mpSlider1.minValue = 0;

            // PlayerController 컴포넌트 할당
            _stats1 = GameObject.Find("Controller").GetComponent<PlayerController>();
        }

        private void UpdateUI()
        {
            _hpSlider1.maxValue = _stats1._maxHP;
            _mpSlider1.maxValue = _stats1._maxMP;
            _hpSlider1.value = _stats1._hp;
            _mpSlider1.value = _stats1._mp;
            _hpText1.text = $"{_stats1._hp}/{_stats1._maxHP}";
            _mpText1.text = $"{_stats1._mp}/{_stats1._maxMP}";
        }

        private void Update()
        {
            UpdateUI();
        }
    }
}