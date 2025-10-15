using System;
using System.Numerics;
using System.Threading.Tasks; // For Parallel.For and Task

namespace DigitalMusicAnalysis
{
    public class timefreq
    {
        public float[][] timeFreqData;
        public int wSamp;
        public Complex[] twiddles;

        public timefreq(float[] x, int windowSamp)
        {
            double pi = 3.14159265;
            Complex i = Complex.ImaginaryOne;
            this.wSamp = windowSamp;
            twiddles = new Complex[wSamp];

            // Parallelize twiddle factor initialization
            Parallel.For(0, wSamp, ii =>
            {
                double a = 2 * pi * ii / (double)wSamp;
                twiddles[ii] = Complex.Pow(Complex.Exp(-i), (float)a);
            });

            timeFreqData = new float[wSamp / 2][];

            int nearest = (int)Math.Ceiling((double)x.Length / (double)wSamp) * wSamp;
            Complex[] compX = new Complex[nearest];

            // Parallel fill of compX array
            Parallel.For(0, nearest, kk =>
            {
                compX[kk] = kk < x.Length ? x[kk] : Complex.Zero;
            });

            int cols = 2 * nearest / wSamp;
            for (int jj = 0; jj < wSamp / 2; jj++)
            {
                timeFreqData[jj] = new float[cols];
            }

            // STFT calculation with parallel FFT windows
            timeFreqData = stft(compX, wSamp);
        }

        private float[][] stft(Complex[] x, int wSamp)
        {
            int N = x.Length;
            float fftMax = 0;
            float[][] Y = new float[wSamp / 2][];

            for (int ll = 0; ll < wSamp / 2; ll++)
            {
                Y[ll] = new float[2 * (int)Math.Floor((double)N / (double)wSamp)];
            }

            // Parallelize processing of each STFT window
            Parallel.For(0, 2 * (N / wSamp) - 1, ii =>
            {
                Complex[] temp = new Complex[wSamp];
                Complex[] tempFFT;

                // Fill temp array with samples for the current window
                for (int jj = 0; jj < wSamp; jj++)
                {
                    temp[jj] = x[ii * (wSamp / 2) + jj];
                }

                // FFT for the current window
                tempFFT = fft(temp);

                // Store the magnitude of FFT results in the output matrix
                float localMax = 0; // Local max to avoid locking
                for (int kk = 0; kk < wSamp / 2; kk++)
                {
                    Y[kk][ii] = (float)Complex.Abs(tempFFT[kk]);
                    if (Y[kk][ii] > localMax) localMax = Y[kk][ii];
                }

                // Update global max (lock to ensure thread safety)
                lock (this)
                {
                    if (localMax > fftMax) fftMax = localMax;
                }
            });

            // Normalize the results in parallel
            Parallel.For(0, 2 * (N / wSamp) - 1, ii =>
            {
                for (int kk = 0; kk < wSamp / 2; kk++)
                {
                    Y[kk][ii] /= fftMax;
                }
            });

            return Y;
        }

        private Complex[] fft(Complex[] x)
        {
            int N = x.Length;
            Complex[] Y = new Complex[N];

            // Copy input to output array for in-place transformation
            for (int i = 0; i < N; i++)
            {
                Y[i] = x[i];
            }

            // Bit-reversal permutation to arrange elements in the right order
            int bits = (int)Math.Log(N, 2);
            for (int i = 0; i < N; i++)
            {
                int j = BitReverse(i, bits);
                if (i < j)
                {
                    var temp = Y[i];
                    Y[i] = Y[j];
                    Y[j] = temp;
                }
            }

            // Iterative FFT implementation
            for (int len = 2; len <= N; len <<= 1) // len = 2, 4, 8, ..., N
            {
                double angle = -2 * Math.PI / len;
                Complex wLen = new Complex(Math.Cos(angle), Math.Sin(angle));

                for (int i = 0; i < N; i += len)
                {
                    Complex w = Complex.One; // Initialize twiddle factor

                    for (int j = 0; j < len / 2; j++)
                    {
                        Complex u = Y[i + j];          // Even component
                        Complex v = Y[i + j + len / 2] * w; // Odd component with twiddle

                        Y[i + j] = u + v;
                        Y[i + j + len / 2] = u - v;

                        w *= wLen; // Update twiddle factor
                    }
                }
            }

            return Y;
        }

        private int BitReverse(int n, int bits)
        {
            int reversed = 0;
            for (int i = 0; i < bits; i++)
            {
                reversed = (reversed << 1) | (n & 1);
                n >>= 1;
            }
            return reversed;
        }
    }
}
