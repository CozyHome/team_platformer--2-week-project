using UnityEngine;

public class Scale : MonoBehaviour {
    
    [SerializeField] private float speed;
    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(
            1F + Mathf.Cos(Time.time * speed) / 2F,
            1F + Mathf.Cos(Time.time * speed) / 2F,
            1F + Mathf.Cos(Time.time * speed) / 2F
        );
    }
}