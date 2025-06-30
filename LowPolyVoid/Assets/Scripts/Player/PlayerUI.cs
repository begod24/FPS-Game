using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    private TextMeshProUGUI Text;
    void Start()
    {

    }

    public void UpdateText(string Text)
    {
        if (this.Text == null)
        {
            this.Text = GetComponent<TextMeshProUGUI>();
        }
        this.Text.text = Text;
    }
}
