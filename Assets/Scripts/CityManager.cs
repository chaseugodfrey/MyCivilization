using System.IO.Enumeration;
using UnityEngine.UI;
using UnityEngine;

public class CityManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider prosperitySlider;

    public class City
    {
        private string mName = "";
        private int mProsperity = 0;

        public string Name
        {
            get => mName;
            set => mName = value;
        }

        public int Prosperity
        {
            get => mProsperity;
            set => mProsperity = value;
        }
    }

    private City mActiveCity;

    public City ActiveCity
    {
        get => mActiveCity;
        set => mActiveCity = value;
    }

    public void UpdateProsperityUI()
    {
        if (prosperitySlider != null && mActiveCity != null)
        {
            prosperitySlider.value = mActiveCity.Prosperity;
        }
    }

    public void ModifyProsperity(int delta)
    {
        if (mActiveCity == null) return;

        mActiveCity.Prosperity += delta;
        UpdateProsperityUI();
    }
}
