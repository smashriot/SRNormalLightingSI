// -------------------------------------------------------------------------------------------------
//  SRNormalLightingSI.cs
//  Normal Lit Sprites
//  Example for SpriteIlluminator ( https://www.codeandweb.com/spriteilluminator )
//  Created by Jesse Ozog (code@smashriot.com) on 2015/03/17
//  Copyright 2015 SmashRiot, LLC. All rights reserved.
// -------------------------------------------------------------------------------------------------
using UnityEngine;

// ------------------------------------------------------------------------
// SRNormalLighting
// ------------------------------------------------------------------------
public class SRNormalLightingSI : MonoBehaviour {

    private const string BACKGROUND_SPRITE = "background"; 	
    private const string LIGHT_SPRITE = "light";
    private const string SI_SPRITE = "character-with-si-logo"; 	
    private const string SI_NORMAL = "character-with-si-logo_n"; 	
    private GameObject lightGameObject;
    private Light lightSource;
    private float counter = 0;
    private float lightDepth = -20;
    private FSprite lightSprite;
    
	// ------------------------------------------------------------------------
	// Use this for initialization
	// ------------------------------------------------------------------------
	public void Start(){

		// init futile
		FutileParams fparms = new FutileParams(true,true,true,true);
		fparms.AddResolutionLevel(1280.0f, 2.0f, 1.0f, "");
		fparms.origin = new Vector2(0.5f, 0.5f);
		Futile.instance.Init(fparms);
		Futile.instance.camera.clearFlags = CameraClearFlags.SolidColor;
		Futile.instance.camera.backgroundColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
		Futile.stage.shouldSortByZ = true; // enable z sorting
		
		// only load the main atlas
        Futile.atlasManager.LoadAtlas("imageAtlas");
	 
		// setup image with shader and light
        this.setupLighting();
	}
	
    // ------------------------------------------------------------------------
	// ------------------------------------------------------------------------
	private void setupLighting(){
		
		// define the shader and reuse this shader on the sprites so the FRenderLayers continue to batch properly
        // SRLightingShader(string normalTexture, float shininess, Color diffuseColor, Color specularColor)
		SRLightingShader lightingShader = new SRLightingShader("imageAtlas_n", 2.5f, Color.white, Color.white);
	
		// sprite uses the SRLightingShader for normal mapped lighting
		lightSprite = new FSprite(LIGHT_SPRITE); 
		lightSprite.sortZ = 10;
		Futile.stage.AddChild(lightSprite);

		// background image
		FSprite background = new FSprite(BACKGROUND_SPRITE); 
		background.sortZ = 0;
		background.shader = lightingShader; // do NOT create a new Shader for each sprite. Doing so would break FRenderLayer batching
		background.color = new Color(0.5f, 0.7f, 1.0f, 1.0f); // 128, 179, 255
		Futile.stage.AddChild(background);
	
		// sprite uses the SRLightingShader for normal mapped lighting
		FSprite siSprite = new FSprite(SI_SPRITE); 
		siSprite.sortZ = 1;
		siSprite.shader = lightingShader; // do NOT create a new Shader for each sprite. Doing so would break FRenderLayer batching
		Futile.stage.AddChild(siSprite);

		// add light gameobject
        lightGameObject = new GameObject("Light");
		lightGameObject.transform.localPosition = new Vector3(0, 0, lightDepth);
		
  		// add lightsource to it and configure
  		lightSource = lightGameObject.AddComponent<Light>();
        lightSource.color = Color.white;
        lightSource.intensity = 8;
        lightSource.range = 600;
        lightSource.type = LightType.Point;
        lightSource.renderMode = LightRenderMode.ForcePixel; // ForcePixel = Important
    }

	// ------------------------------------------------------------------------
	// ------------------------------------------------------------------------
	private void Update(){
        
        // get mouse position for light
        Vector2 mousePos = Input.mousePosition;
        Vector3 mouseScreenPos = Futile.instance.camera.ScreenToWorldPoint(mousePos); 
        mouseScreenPos.z = -15.0f; // must be negative. closer to 0 = bright, farther = more spread out
        lightGameObject.transform.localPosition = mouseScreenPos;
        lightSprite.x = mouseScreenPos.x;
        lightSprite.y = mouseScreenPos.y;
        
        // change light color with time to show different colored lights
        counter += 0.5f * Time.deltaTime;
        lightSource.color = this.HSVtoRGB(Mathf.Abs(Mathf.Sin(0.5f * counter)), 1.0f, 1.0f, 1.0f);
        
        // show color on the light sprite icon
        lightSprite.color = lightSource.color;
	}
	
    // ------------------------------------------------------------------------
    // using hsv to rgb to easily rotate light color by hue
    // ------------------------------------------------------------------------
    private Color HSVtoRGB(float hue, float saturation, float value, float alpha){

        while (hue > 1.0f) { hue -= 1.0f; }
        while (hue < 0.0f) { hue += 1.0f; }
        while (saturation > 1.0f) { saturation -= 1.0f; }
        while (saturation < 0.0f) { saturation += 1.0f; }
        while (value > 1.0f) { value -= 1.0f; }
        while (value < 0.0f) { value += 1.0f; }
        if (hue > 0.999f) { hue = 0.999f; }
        if (hue < 0.001f) { hue = 0.001f; }
        if (saturation > 0.999f) { saturation = 0.999f; }
        if (saturation < 0.001f) { return new Color(value * 255.0f, value * 255.0f, value * 255.0f); }
        if (value > 0.999f) { value = 0.999f; }
        if (value < 0.001f) { value = 0.001f; }

        float h6 = hue * 6.0f;
        if (h6 == 6.0f) { h6 = 0.0f; }
        int ihue = (int)(h6);
        float p = value * (1.0f - saturation);
        float q = value * (1.0f - (saturation * (h6 - (float)ihue)));
        float t = value * (1.0f - (saturation * (1.0f - (h6 - (float)ihue))));
        switch (ihue){
            case 0:  return new Color(value, t, p, alpha);
            case 1:  return new Color(q, value, p, alpha);
            case 2:  return new Color(p, value, t, alpha);
            case 3:  return new Color(p, q, value, alpha);
            case 4:  return new Color(t, p, value, alpha);
            default: return new Color(value, p, q, alpha);
        }
    }

}