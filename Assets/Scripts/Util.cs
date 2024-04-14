using System;

public class Util
{
  public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
  {
    return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
  }

  public static void Shuffle<T>(T[] array)
  {
    Random rng = new Random();

    int n = array.Length;
    while (n > 1)
    {
      n--;
      int k = rng.Next(n + 1);
      (array[n], array[k]) = (array[k], array[n]);
    }
  }

}