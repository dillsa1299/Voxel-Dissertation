OPTIONS
- Render Distance (Chunks):
	Sets how far chunks are rendered from the camera.

- Chunk Height:
	Sets the max height of the terrain.

- Face Culling:
	Toggles whether faces between voxels and edge of chunks are culled.


PERLIN GENERATION OPTIONS
- Offset X:
	Sets X offset for Perlin noise.
	Note: Values over 5 digits aren't recommended due to irregular terrain generation.
- Offset Y:
	Sets Y offset for Perlin noise.
	Note: Values over 5 digits aren't recommended due to irregular terrain generation
- Water Height:
	Sets the water level.
- Noise Scale:
	Sets the scale of Perlin noise.

PERLIN GENERATION ADVANCED OPTIONS - Terrain Generation
- Persistance:
	Values from 0 -> 1
	A multiplier that determines how quickly the amplitudes
	diminish for each successive octave in a Perlin-noise function.
	Lower = smoother terrain
- Redistribution Modifier:
	Assists with creating plateaus.
- Exponent:
	Assists with creating plateaus.
- Noise Zoom:
	Values from 0 -> 1
	Zooms in and out of Perlin noise, creating more or less
	hills in terrain.
- Octaves:
	Octaves control the amount of detail in Perlin noise, creating
	a mixture of flat and mountainous terrain. More octaves requires more
	processing time.

PERLIN GENERATION ADVANCED OPTIONS - Cave Generation
- Noise Scale:
	Values from 0 -> 1
	Controls the scale of Perlin noise. Larger values create larger caves.
- Min Threshold:
	Values from 0 -> 1
	The minimum value of Perlin noise to generate a cave.
	Note: A larger threshold range results in caves generating more frequently.
- Max Threshold:
	Values from 0 -> 1
	The maximum value of Perlin noise to generate a cave.
	Note: A larger threshold range results in caves generating more frequently.
