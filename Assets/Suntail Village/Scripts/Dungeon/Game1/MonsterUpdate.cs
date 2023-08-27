using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Suntail
{
    public class MonsterUpdate : MonoBehaviour
    {
        [SerializeField] private Slider _hpSlider1;
        [SerializeField] private Text _hpText1;
        [SerializeField] private Monster monster;

        private void Start()
        {
            Transform hpBarTransform = GameObject.Find("MonsterHpBar").transform;
            _hpSlider1 = hpBarTransform.GetComponent<Slider>();
            _hpSlider1.minValue = 0;

            monster = GameObject.Find("Green1(Clone)").GetComponent<Monster>();
        }

        private void UpdateUI()
        {
            _hpSlider1.maxValue = monster.maxHealth;
            _hpSlider1.value = monster.currentHealth;
            _hpText1.text = $"{monster.currentHealth}/{monster.maxHealth}";
        }

        private void Update()
        {
            UpdateUI();
        }
    }
}