using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;

    // Start is called before the first frame update
    void Start()
	{
        player = GameObject.Find("Player").transform;
    }
	
	// Update is called once per frame
	void Update()
	{
        transform.position = new Vector3(player.position.x + xOffset, player.position.y + yOffset, player.position.z - 10);
    }
}
