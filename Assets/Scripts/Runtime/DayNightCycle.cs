using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
	//The real-life in seconds duration of a in-game day
	public float DayDuration = 30;
	public float nightDuration = 10f;
	private float totalTime => DayDuration + nightDuration;
	private float passedTime = 0f;
	public float startAngle = 30f;
	public float endAngle = 150f;
	public Material DaySkybox;
	public Material NighSkybox;
	public Gradient DayFogGradient;
	public Gradient NightFogGradient;
	public Color DayColor;
	public Color NightColor;
	private bool daySkybox = false;
	private Light light;
	private Quaternion rotation;

	private void Awake()
	{
		light = GameObject.FindObjectOfType<Light>();
	}

	private void Update()
	{
		passedTime += Time.deltaTime;
		if (passedTime > totalTime)
		{
			passedTime = passedTime % totalTime;
		}

		if (passedTime > DayDuration)
		{
			var percentage = (passedTime - DayDuration) / nightDuration;
			light.intensity = 0.25f;
			if (daySkybox)
			{
				light.color = NightColor;
				RenderSettings.skybox = NighSkybox;
				daySkybox = false;
			}
			RenderSettings.fogColor = NightFogGradient.Evaluate(percentage);
		}
		else
		{
			if (daySkybox == false)
			{
				light.color = DayColor;
				RenderSettings.skybox = DaySkybox;
				daySkybox = true;
			}

			var percentage = passedTime / DayDuration;
			light.intensity = percentage < 0.5f ? percentage : 1f - percentage;
			light.transform.rotation = GetRotation(percentage);
			RenderSettings.fogColor = DayFogGradient.Evaluate(percentage);
		}
	}

	private Quaternion GetRotation(float percentage)
	{
		rotation.eulerAngles = new Vector3(startAngle + percentage * (endAngle - startAngle), 0f, 0f);
		return rotation;
	}
}
