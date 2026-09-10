using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int totalArrows;

    void Start()
    {
        // সিন-এ কয়টি তীর আছে তা গণনা করবে
        CheckRemainingArrows();
    }

    public void ArrowEscaped()
    {
        totalArrows--;
        Debug.Log("বাকি আছে তীর: " + totalArrows);

        if (totalArrows <= 0)
        {
            Debug.Log("LEVEL COMPLETE! 🎉");
            // এখানে পরবর্তীতে পরের লেভেলে যাওয়ার কোড বা UI পপ-আপ যোগ করব
        }
    }

    private void CheckRemainingArrows()
    {
        // "Arrow" ট্যাগ থাকা সব অবজেক্ট গননা করবে
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("Arrow");
        totalArrows = arrows.Length;
    }
}