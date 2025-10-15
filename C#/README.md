# 🎻 Digital Music Analysis – Parallelisation and Performance Optimisation

## Overview
This project presents a **Digital Music Analysis tool** designed to help beginner violin players improve performance accuracy.  
The application analyses a recorded audio performance against a reference sheet music score to evaluate **pitch** and **timing accuracy**.

Originally implemented as a **sequential C# application**, it was later **parallelised** using .NET’s `System.Threading.Tasks` library to improve performance on multi-core processors.

---

## Objectives
- Evaluate violin performance accuracy by comparing **audio recordings (WAV)** with **sheet music (XML)**.
- Apply **signal processing algorithms** such as:
  - **Short-Time Fourier Transform (STFT)**
  - **Fast Fourier Transform (FFT)**
- Provide **visual feedback** through time-frequency graphs and histograms.
- Optimise performance by **parallelising key computations**.

---

## Inputs and Outputs

| Type | Description |
|------|--------------|
| **Input 1** | Audio File (`.wav`) – recorded violin performance |
| **Input 2** | Sheet Music (`.xml`) – reference score |
| **Output 1** | Time-frequency visualisation (graph) |
| **Output 2** | Feedback report on pitch and timing accuracy |

---

## Techniques and Implementation

### 1. Signal Processing
- Extracted frequency and pitch features from audio using **STFT** and **FFT**.
- Compared extracted notes with sheet music references to identify timing and pitch discrepancies.

### 2. Parallelisation
- Re-engineered sequential code to leverage **multi-threading** using `Parallel.For`.
- Major performance optimisations included:
  - **Twiddle factor initialisation**
  - **STFT computation**
  - **Frequency domain conversion**
  - **Onset detection and FFT normalisation**

### 3. FFT Optimisation
- Converted the FFT implementation from **recursive** to **iterative**, improving memory efficiency and scalability for large datasets.

---

## Performance Evaluation

### Timing Comparison

| Function | Sequential (ms) | Parallel (ms) | Speedup |
|-----------|-----------------|----------------|----------|
| loadWave | 135 | 52 | 2.6× |
| readXML | 20 | 10 | 2.0× |
| freqDomain | 1785 | 195 | 9.15× |
| onsetDetection | 2399 | 1368 | 1.75× |
| loadImage | 104 | 3 | 34.7× |
| loadHistogram | 18 | 3 | 6.0× |
| **Total** | **4461** | **1631** | **2.74×** |

### CPU Profiling (Summary)
Parallelisation significantly reduced CPU load for core functions:

| Function | Before (%) | After (%) | Observation |
|-----------|-------------|------------|--------------|
| App.Main() | 76.87 | 55.49 | Reduced central load |
| MainWindow.freqDomain() | 20.41 | 1.72 | Major performance gain |
| timefreq.ctor() | 20.32 | 1.68 | Initialisation optimised |
| timefreq.fft() | 18.40 | 6.57 | Faster FFT computation |

---

## Results
- **Overall speedup:** 2.7× faster runtime  
- **Max performance gain:** 9× faster frequency-domain processing  
- **Optimised multi-thread utilisation** up to 8 cores  
- Improved efficiency in CPU usage and memory management

---

## Technologies
- **Language:** C#  
- **Framework:** .NET  
- **Libraries:** `System.Threading.Tasks`, `System.Numerics`  
- **Techniques:** STFT, FFT, Parallel Computing, Signal Processing  

---

## Note
This project was developed as part of an **academic assignment** and is intended for research and demonstration purposes.  
No external datasets or proprietary code are distributed with this repository.

---


