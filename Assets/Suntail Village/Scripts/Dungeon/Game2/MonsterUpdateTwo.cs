using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Suntail
{
    public class MonsterUpdateTwo : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider1;
        [SerializeField] private Text _hpText1;
        [SerializeField] private MonsterTwo monsterTwo;

        private void Start()
        {
            Transform hpBarTransform = GameObject.Find("MonsterHpBar").transform;
            _hpSlider1 = hpBarTransform.GetComponent<Slider>();
            _hpSlider1.minValue = 0;

            monsterTwo = GameObject.Find("Red 1(Clone)").GetComponent<MonsterTwo>();
        }

        private void UpdateUI()
        {
            _hpSlider1.maxValue = monsterTwo.maxHealth;
            _hpSlider1.value = monsterTwo.currentHealth;
            _hpText1.text = $"{monsterTwo.currentHealth}/{monsterTwo.maxHealth}";
        }

        private void Update()
        {
            UpdateUI();
        }
    }
}