using System.Collections;
using UnityEngine;

public class FlashWhenHit : MonoBehaviour
{
    public Color flashColor = Color.red;
    public float flashDuration = 0.12f;

    Renderer _renderer;
    MaterialPropertyBlock _mpb;
    int _colorProp;          // _BaseColor (URP/HDRP) or _Color (Built-in)
    Color _originalColor;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _mpb = new MaterialPropertyBlock();

        // Pick color property and cache original
        int baseId = Shader.PropertyToID("_BaseColor");
        int colorId = Shader.PropertyToID("_Color");
        _colorProp = (_renderer.sharedMaterial && _renderer.sharedMaterial.HasProperty(baseId))
            ? baseId : colorId;

        _originalColor = (_renderer.sharedMaterial && _renderer.sharedMaterial.HasProperty(_colorProp))
            ? _renderer.sharedMaterial.GetColor(_colorProp)
            : Color.white;
    }

    public void Hit()  { StartCoroutine(FlashCo(flashDuration)); }
    public void Flash(){ StartCoroutine(FlashCo(flashDuration)); }

    IEnumerator FlashCo(float duration)
    {
        // set flash
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(_colorProp, flashColor);
        _renderer.SetPropertyBlock(_mpb);

        if (duration > 0f) yield return new WaitForSeconds(duration);

        // restore original
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetColor(_colorProp, _originalColor);
        _renderer.SetPropertyBlock(_mpb);
    }
}