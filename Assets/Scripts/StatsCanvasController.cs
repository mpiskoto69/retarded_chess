using System.Collections.Generic;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Unity.FantasyKingdom
{
    public class StatsCanvasController : MonoBehaviour
    {
        public GameObject InputProviderObject;
        public GameObject Panel;

        public TextMeshProUGUI Draws, Verts, FPS, CPU, GPU;

        UniversalRenderPipelineAsset urp;
        IInputProvider inputProvider;

        const int sampleCount = 30;
        const float renderScale = 0.5f;

        ProfilerRecorder drawCallsRecorder;
        ProfilerRecorder verticesRecorder;
        ProfilerRecorder cpuMainThreadTimeRecorder;
        ProfilerRecorder gpuFrameTimeRecorder;
        ProfilerRecorder mainThreadTimeRecorder;

        long vertexCount;
        long drawCallCount;

        List<long> vertexCountSamples = new List<long>();
        List<long> drawCallCountSamples = new List<long>();

        bool gpuToggle = true;
        bool stpToggle = true;

        void Start()
        {
            urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;

            if (Panel != null)
                Panel.SetActive(false);

            if (InputProviderObject != null)
                inputProvider = InputProviderObject.GetComponent<IInputProvider>();

            drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
            cpuMainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "CPU Main Thread Frame Time", sampleCount);
            gpuFrameTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "GPU Frame Time", sampleCount);
            mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", sampleCount);
        }

        void Update()
        {
            if (inputProvider == null)
                return;

            if (Panel != null && (inputProvider.StatPanelGesture || inputProvider.StatPanelButton))
                Panel.SetActive(!Panel.activeInHierarchy);

            UpdateStats();

            if (Draws != null) Draws.text = $"{drawCallCount}";
            if (Verts != null) Verts.text = $"{(vertexCount / 1000000f):F1} m";
            if (FPS != null) FPS.text = $"{1 / (GetRecorderFrameAverage(mainThreadTimeRecorder) * 1e-9f):F1}";
            if (CPU != null) CPU.text = $"{GetRecorderFrameAverage(cpuMainThreadTimeRecorder) * 1e-6f:F1}";
            if (GPU != null) GPU.text = $"{GetRecorderFrameAverage(gpuFrameTimeRecorder) * 1e-6f:F1}";
        }

        void UpdateStats()
        {
            if (verticesRecorder.Valid)
                vertexCountSamples.Add(verticesRecorder.LastValue);

            if (vertexCountSamples.Count > sampleCount)
                vertexCountSamples.RemoveAt(0);

            vertexCount = 0;
            foreach (long value in vertexCountSamples)
                if (value > vertexCount) vertexCount = value;

            if (drawCallsRecorder.Valid)
                drawCallCountSamples.Add(drawCallsRecorder.LastValue);

            if (drawCallCountSamples.Count > sampleCount)
                drawCallCountSamples.RemoveAt(0);

            drawCallCount = 0;
            foreach (long value in drawCallCountSamples)
                if (value > drawCallCount) drawCallCount = value;
        }

        static double GetRecorderFrameAverage(ProfilerRecorder recorder)
        {
            if (!recorder.Valid || recorder.Capacity == 0)
                return 0.000001;

            double total = 0;
            var samples = new List<ProfilerRecorderSample>(recorder.Capacity);
            recorder.CopyTo(samples);

            foreach (var sample in samples)
                total += sample.Value;

            return total / recorder.Capacity;
        }

        public void ToggleResidentDrawer()
        {
            if (urp == null) return;

            gpuToggle = !gpuToggle;
            urp.gpuResidentDrawerMode = gpuToggle
                ? GPUResidentDrawerMode.InstancedDrawing
                : GPUResidentDrawerMode.Disabled;
        }

        public void ToggleSTP()
        {
            if (urp == null) return;

            stpToggle = !stpToggle;
            urp.renderScale = stpToggle ? renderScale : 1f;
        }

        void OnDestroy()
        {
            if (urp != null)
            {
                urp.gpuResidentDrawerMode = GPUResidentDrawerMode.InstancedDrawing;
                urp.renderScale = renderScale;
            }

            if (drawCallsRecorder.Valid) drawCallsRecorder.Dispose();
            if (verticesRecorder.Valid) verticesRecorder.Dispose();
            if (cpuMainThreadTimeRecorder.Valid) cpuMainThreadTimeRecorder.Dispose();
            if (gpuFrameTimeRecorder.Valid) gpuFrameTimeRecorder.Dispose();
            if (mainThreadTimeRecorder.Valid) mainThreadTimeRecorder.Dispose();
        }
    }
}
