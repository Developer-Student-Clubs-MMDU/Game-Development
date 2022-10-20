using System;
using System.Collections;
using UnityEngine;

namespace VladislavTsurikov.MegaWorldSystem
{
    public enum NoiseType 
    {  
        Perlin, 
        Value, 
        Simplex, 
        Voronoi, 
        Worley 
    }

    /// <summary>
    /// A concrete class for generating fractal noise.
    /// </summary>
    [System.Serializable]
	public class FractalNoiseCPU
    {
        [SerializeField]
        private NoiseType noiseType = NoiseType.Perlin;

        /// <summary>
        /// The number of octaves in the fractal.
        /// </summary>

        public NoiseType NoiseType 
        { 
            get
            {
                return noiseType;
            }
            set
            {
                if(noiseType != value)
                {
                    noiseType = value;
                    UpdateTable(new INoiseCPU[] { GetNoise() });
                }
            }
        }

        [SerializeField]
        int seed = 0;

        public int Seed 
        { 
            get
            {
                return seed;
            }
            set
            {
                if(seed != value)
                {
                    seed = value;
                    UpdateTable(new INoiseCPU[] { GetNoise() });
                }
            }
        }

        [SerializeField]
        private int octaves = 3;
        
        public int Octaves 
        { 
            get
            {
                return octaves;
            }
            set
            {
                if(octaves != value)
                {
                    octaves = value;
                    UpdateTable(new INoiseCPU[] { GetNoise() });
                }
            }
        }

        [SerializeField]
        private float frequency = 0.5f;

        /// <summary>
        /// The frequency of the fractal.
        /// </summary>
        public float Frequency 
        { 
            get
            {
                return frequency;
            } 
            set
            {
                if(frequency != value)
                {
                    frequency = value;
                    
                    UpdateTable(new INoiseCPU[] { GetNoise() });
                }
            }
        }

        /// <summary>
        /// The amplitude of the fractal.
        /// </summary>
        public float Amplitude { get; set; }

        /// <summary>
        /// The offset applied to each dimension.
        /// </summary>
        public Vector3 Offset { get; set; }


        [SerializeField]
        private float lacunarity = 2f;

        /// <summary>
        /// The rate at which the amplitude changes.
        /// </summary>
        public float Lacunarity 
        { 
            get
            {
                return lacunarity;
            } 
            set
            {
                if(lacunarity != value)
                {
                    lacunarity = value;
                    UpdateTable(new INoiseCPU[] { GetNoise() });
                }
            }
        }

        [SerializeField]
        private float persistence = 0.5f;

        /// <summary>
        /// The rate at which the frequency changes.
        /// </summary>
        public float Persistence 
        {
            get
            {
                return persistence;
            } 
            set
            {
                if(persistence != value)
                {
                    persistence = value;
                    UpdateTable(new INoiseCPU[] { GetNoise() });
                }
            }
        }

        /// <summary>
        /// The noises to sample from to generate the fractal.
        /// </summary>
        public INoiseCPU[] Noises { get; set; }

        /// <summary>
        /// The amplitudes for each octave.
        /// </summary>
        public float[] Amplitudes { get; set; }

        /// <summary>
        /// The frequencies for each octave.
        /// </summary>
        public float[] Frequencies { get; set; }

        public FractalNoiseCPU()
        {
            UpdateTable(new INoiseCPU[] { GetNoise() });
        }
		
        public FractalNoiseCPU(INoiseCPU noise, int octaves, float frequency, float lacunarity, float persistence, float amplitude = 1.0f)
        {
            Octaves = octaves;
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;
            Lacunarity = lacunarity;
            Persistence = persistence;

            UpdateTable(new INoiseCPU[] { noise });
        }

        public FractalNoiseCPU(INoiseCPU[] noises, int octaves, float frequency, float amplitude = 1.0f)
        {
            Octaves = octaves;
            Frequency = frequency;
            Amplitude = amplitude;
            Offset = Vector3.zero;
            Lacunarity = 2.0f;
            Persistence = 0.5f;

            UpdateTable(noises);
        }

        /// <summary>
        /// Calculates the amplitudes and frequencies tables for each octave
        /// based on the fractal settings. The tables are used so individual 
        /// octaves can be sampled. Must be called when object is first created
        /// and when ever the settings are changed.
        /// </summary>
        public virtual void UpdateTable()
        {
            UpdateTable(Noises);
        }

        protected virtual void UpdateTable(INoiseCPU[] noises)
		{
			Amplitudes = new float[Octaves];
			Frequencies = new float[Octaves];
            Noises = new INoiseCPU[Octaves];

            int numNoises = noises.Length;
			
			float amp = 0.5f;
			float frq = Frequency;
			for(int i = 0; i < Octaves; i++)
			{
                Noises[i] = noises[Math.Min(i, numNoises - 1)];
				Frequencies[i] = frq;
				Amplitudes[i] = amp;
				amp *= Persistence;
				frq *= Lacunarity;
			}
		}
		
        /// <summary>
        /// Returns the noise value from a octave in a 1D fractal.
        /// </summary>
        /// <param name="i">The octave to sample.</param>
        /// <param name="x">A value on the x axis.</param>
        /// <returns>A noise value between -Amp and Amp.</returns>
		public virtual float Octave1D(int i, float x)
		{
            if (i >= Octaves) return 0.0f;
            if (Noises[i] == null) return 0.0f;

			x = x + Offset.x;

			float frq = Frequencies[i];
			return Noises[i].Sample1D(x * frq) * Amplitudes[i] * Amplitude;
		}
		
        /// <summary>
        /// Returns the noise value from a octave in a 2D fractal.
        /// </summary>
        /// <param name="i">The octave to sample.</param>
        /// <param name="x">A value on the x axis.</param>
        /// <param name="y">A value on the y axis.</param>
        /// <returns>A noise value between -Amp and Amp.</returns>
		public virtual float Octave2D(int i, float x, float y)
		{
            if (i >= Octaves) return 0.0f;
            if (Noises[i] == null) return 0.0f;

			x = x + Offset.x;
			y = y + Offset.y;

			float frq = Frequencies[i];
            return Noises[i].Sample2D(x * frq, y * frq) * Amplitudes[i] * Amplitude;
		}
		
        /// <summary>
        /// Returns the noise value from a octave in a 3D fractal.
        /// </summary>
        /// <param name="i">The octave to sample.</param>
        /// <param name="x">A value on the x axis.</param>
        /// <param name="y">A value on the y axis.</param>
        /// <param name="z">A value on the z axis.</param>
        /// <returns>A noise value between -Amp and Amp.</returns>
		public virtual float Octave3D(int i, float x, float y, float z)
		{
            if (i >= Octaves) return 0.0f;
            if (Noises[i] == null) return 0.0f;

			x = x + Offset.x;
			y = y + Offset.y;
			z = z + Offset.z;

			float frq = Frequencies[i];
            return Noises[i].Sample3D(x * frq, y * frq, z * frq) * Amplitudes[i] * Amplitude;
		}

        /// <summary>
        /// Samples a 1D fractal.
        /// </summary>
        /// <param name="x">A value on the x axis.</param>
        /// <returns>A noise value between -Amp and Amp.</returns>
        public virtual float Sample1D(float x)
        {
			x = x + Offset.x;

	        float sum = 0, frq;
			for(int i = 0; i < Octaves; i++) 
	        {	
				frq = Frequencies[i];

                if (Noises[i] != null)
                    sum += Noises[i].Sample1D(x * frq) * Amplitudes[i];
	        }
			return sum * Amplitude;
        }

        /// <summary>
        /// Samples a 2D fractal.
        /// </summary>
        /// <param name="x">A value on the x axis.</param>
        /// <param name="y">A value on the y axis.</param>
        /// <returns>A noise value between -Amp and Amp.</returns>
        public virtual float Sample2D(float x, float y)
        {
            UpdateTable(); 

			x = x + Offset.x;
			y = y + Offset.y;

	        float sum = 0, frq;

            if(Noises.Length != Octaves)
            {
                Noises = new INoiseCPU[Octaves];
            }
            
	        for(int i = 0; i < Octaves; i++) 
	        {
				frq = Frequencies[i];

                if (Noises[i] != null)
                    sum += Noises[i].Sample2D(x * frq, y * frq) * Amplitudes[i];
			}
			return sum * Amplitude;
        }

        /// <summary>
        /// Samples a 3D fractal.
        /// </summary>
        /// <param name="x">A value on the x axis.</param>
        /// <param name="y">A value on the y axis.</param>
        /// <param name="z">A value on the z axis.</param>
        /// <returns>A noise value between -Amp and Amp.</returns>
        public virtual float Sample3D(float x, float y, float z)
        {
			x = x + Offset.x;
			y = y + Offset.y;
			z = z + Offset.z;

	        float sum = 0, frq;
			for(int i = 0; i < Octaves; i++) 
	        {
				frq = Frequencies[i];

                if (Noises[i] != null)
                    sum += Noises[i].Sample3D(x * frq, y * frq, z * frq) * Amplitudes[i];
	        }
			return sum * Amplitude;
        }

        public INoiseCPU GetNoise()
        {
            switch (noiseType)
            {
                case NoiseType.Perlin:
                    return new PerlinNoiseCPU(seed, 20);

                case NoiseType.Value:
                    return new ValueNoiseCPU(seed, 20);

                case NoiseType.Simplex:
                    return new SimplexNoiseCPU(seed, 20);

                case NoiseType.Voronoi:
                    return new VoronoiNoiseCPU(seed, 20);

                case NoiseType.Worley:
                    return new WorleyNoiseCPU(seed, 20, 1.0f);

                default:
                    return new PerlinNoiseCPU(seed, 20);
            }
        }
    }
}