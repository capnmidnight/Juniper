using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class VMU : MonoBehaviour
{
    public Transform Prefab;
    public Vector3 BarScale = new Vector3(0.1f, 4f, 0.1f);
    private Slider progress;
    public BeatDetector detector;
    private readonly List<Transform> bars = new List<Transform>();
    private bool hasLayout;

    private void Start()
    {
        transform.ClearChildren();
        hasLayout = GetComponent<HorizontalLayoutGroup>() != null || GetComponent<VerticalLayoutGroup>() != null;
        progress = this.Query<Slider>("../Progress");
        if (hasLayout)
        {
            BarScale = new Vector3(1, 10, 1);
        }
    }

    public void BlinkMeasure()
    {
    }

    public void BlinkBeat()
    {
    }

    public void SetVMU(float[] spectrum)
    {
        while (bars.Count < spectrum.Length)
        {
            var bar = Instantiate(Prefab);
            bar.SetParent(transform);
            bar.localPosition = Vector3.zero;
            bar.localRotation = new Quaternion(0, 0, 0, 1);
            bar.localScale = new Vector3(1, 1, 1);
            bars.Add(bar);
        }

        while (bars.Count > spectrum.Length)
        {
            var last = bars.Count - 1;
            var bar = bars[last];
            bars.RemoveAt(last);
            Destroy(bar.gameObject);
        }

        var offset = 0.5f * (bars.Count - 1);
        for (var i = 0; i < bars.Count; ++i)
        {
            var bar = bars[i];
            var scale = BarScale;
            scale.y *= spectrum[i];
            bar.localScale = scale;
            if (!hasLayout)
            {
                bar.transform.localPosition = new Vector3(BarScale.x * (i - offset), 0, 0);
            }
        }
    }

    private void Update()
    {
        progress.value = detector.Progress;
    }
}
