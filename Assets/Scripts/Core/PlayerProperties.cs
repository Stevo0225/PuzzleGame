using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerProperties : MonoBehaviour
{
    private Material mat;			//Creatses material for image effect shader
    private Shader s;				//Creates empty shader for image effects
    [Range(0, 1)]
    public float distance;			//How dizzy the player becomes
    [Range(0, 1.5f)]
    public float power;				//How much the vignette darkens

	public float health = 100;
    public float maxHealth = 100;
	
	private string status;
    private Vector3 startPosition;

    private CameraEffect CENTER_EYE;
    public CameraEffect L_EYE;
    public CameraEffect R_EYE;

    public GameObject Player;
   
    //public string[] ShadersDirect;	//Array of shaders to switch to
    //private string currentShader;		//What the current shader is
    //private int finder = 0;
    private void Start()
    {
        s = Shader.Find("Example/DizzyVignette");
        mat = new Material(s);
		
		maxHealth = 100;
		health = 100;
		status = "none";
        startPosition = Player.transform.position;

        CENTER_EYE = GetComponent<CameraEffect>();
    }

	public void TakeDamage(float damage)
	{
		health -= damage;
		if(damage > 25)
		{
            //DO HURT OVERLAY
            status = "HurtHigh";
		}
		else
		{
            //DO HURT OVERLAY
            status = "HurtLow";
		}
        if (health <= 0)
            status = "Dead";
	}
	
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TakeDamage(25+Random.Range(-0.25f,0.25f));
        }

        if (health < maxHealth)
            health += 2 * Time.deltaTime;
        else if (health > maxHealth)
            health = maxHealth;

        //STATES FOR IMAGE EFFECTS
		if(status == "HurtLow" && distance < 0.5f)
		{
			distance += 0.2f * Time.deltaTime;
			if(distance >= 0.5f)
				status = "RecoveringHurtLow";
		}
		else if(status == "RecoveringHurtLow")
		{
			if(distance > 0)
			{
				distance -= 0.2f * Time.deltaTime;
			}
			else
			{
				distance = 0;
			}
			if(distance == 0)
				status = "none";
		}
		else if(status == "HurtHigh" && distance < 1)
		{
			distance += 0.5f * Time.deltaTime; ;
			if(distance >= 1)
				status = "RecoveringHurtHigh";
		}
		else if(status == "RecoveringHurtHigh")
		{
			if(distance > 0)
			{
				distance -= 0.25f * Time.deltaTime;
			}
			else
			{
				distance = 0;
			}
			if(distance == 0)
				status = "none";
		}
		else if(status == "Dead" && (distance < 1 || power < 1.5))
		{
            if(distance < 1)
			    distance += 0.25f * Time.deltaTime;
            if(power < 1.5f)
			    power += 0.25f * Time.deltaTime;
		}
		else if(status == "Dead" && distance >= 1 && power >= 1.5)
		{
            Player.transform.position = startPosition;
			status = "Reviving";
            health = 0;
		}
		else if(status == "Reviving" && power > -0.5f)
		{
            if(health < maxHealth)
			    health += 15.0f * Time.deltaTime;
            if(health > 25)
            {
                if (distance > 0)
                {
                    distance -= 0.1f * Time.deltaTime;
                }
                else
                {
                    distance = 0;
                }
            }
		}
		else
		{
			if(distance > 0)
			{
				distance -= 0.1f * Time.deltaTime;
			}
			else
			{
				distance = 0;
			}
		}
        power = (1.5f-(1.5f*(health / maxHealth)));
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        L_EYE.RenderImage(source, destination, distance, power);
        R_EYE.RenderImage(source, destination, distance, power);
        CENTER_EYE.RenderImage(source, destination, distance, power);
    }


}
