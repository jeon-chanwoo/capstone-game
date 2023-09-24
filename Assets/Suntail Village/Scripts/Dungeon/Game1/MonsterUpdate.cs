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

            monster = GameObject.Find("Green1(Clone)").GetComponent<Monster>();// 생성된 몬스터의 컴포넌트를 가지고 온다.
        }
        private void Update()
        {
            UpdateUI();
        }

        private void UpdateUI()
            //가지고 온 컴포넌트에서 최대 체력, 현재 체력을 가지고 와서 슬라이더에 쏴준다.
        {
            _hpSlider1.maxValue = monster.maxHealth;
            _hpSlider1.value = monster.currentHealth;
            _hpText1.text = $"{monster.currentHealth}/{monster.maxHealth}";
        }

       
    }
}