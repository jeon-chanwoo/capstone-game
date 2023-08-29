using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Suntail
{
    public class MonsterUpdateThree : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider1;
        [SerializeField] private Text _hpText1;
        [SerializeField] private MonsterThree monsterThree;

        private void Start()
        {
            Transform hpBarTransform = GameObject.Find("MonsterHpBar").transform;
            _hpSlider1 = hpBarTransform.GetComponent<Slider>();
            _hpSlider1.minValue = 0;

            monsterThree = GameObject.Find("Gold 1(Clone)").GetComponent<MonsterThree>();
        }

        private void UpdateUI()
        {
            _hpSlider1.maxValue = monsterThree.maxHealth;
            _hpSlider1.value = monsterThree.currentHealth;
            _hpText1.text = $"{monsterThree.currentHealth}/{monsterThree.maxHealth}";
        }

        private void Update()
        {
            UpdateUI();
        }
    }
}