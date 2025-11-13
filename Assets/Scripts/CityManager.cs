using System.IO.Enumeration;
using UnityEngine;

public class CityManager : MonoBehaviour
{   
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
}
