namespace Maroon.Chemistry.Catalyst
{
    public static class CatalystConstants
    {
        public static readonly int[][] TemperatureStageValues = 
        {
            new[] { 250, 275, 300, 325, 350, 375, 400, 425, 450 },      // langmuir-hinshelwood
            new[] { 321, 334, 348, 363 },                               // mars van krevelen
            new[] { 481, 499, 519, 533, 553 }                           // eley-rideal
        };

        public static readonly float[][] PartialPressureValues =
        {
            new[] { 0.01f, 0.02f, 0.04f, 0.2f },                        // langmuir-hinshelwood
            new[] { 0.001f, 0.002f, 0.004f, 0.008f, 0.014f, 0.026f },   // mars van krevelen

            // eley-rideal: pressure is different for each temperature stage
            // each row below corresponds on one temperature (first line = pressure values for 208 in temp values)
            new[]
            {
                0.0010855f, 0.0019737f, 0.0062171f, 0.011053f, 0.001523f, 0f, 0f,                       // temperature 208
                0.00069079f, 0.0016118f, 0.0023355f, 0.0031579f, 0.0065461f, 0.008125f, 0.013257f,      // temperature 226
                0.0010197f, 0.0022697f, 0.0028947f, 0.0042763f, 0.0098355f, 0.012434f, 0.015822f,       // temperature 246
                0.0013816f, 0.0031908f, 0.0045395f, 0.0054934f, 0.0066447f, 0.0077961f, 0f,             // temperature 260
                0.0019408f, 0.0025658f, 0.0036513f, 0.0046053f, 0.0055921f, 0.0070395f, 0.0089803f      // temperature 280
            }
        };

        public static readonly float[][][] TurnOverRates = 
        {
            new[] // langmuir-hinshelwood
            {
                new[] { 0f, 0f, 0.047619048f, 0.285714286f },
                new[] { 0f, 0.047619048f, 0.142857143f, 0.666666667f },
                new[] { 0f, 0.095238095f, 0.238095238f, 1.19047619f },
                new[] { 0.047619048f, 0.19047619f, 0.380952381f, 1.952380952f },
                new[] { 0.095238095f, 0.285714286f, 0.571428571f, 2.952380952f },
                new[] { 0.19047619f, 0.380952381f, 0.80952381f, 4.19047619f },
                new[] { 0.285714286f, 0.571428571f, 1.142857143f, 5.904761905f },
                new[] { 0.333333333f, 0.714285714f, 1.428571429f, 7.428571429f },
                new[] { 0.380952381f, 0.80952381f, 1.619047619f, 8.571428571f }
            },
            new[] // mars van krevelen
            {
                new[] { 0.042146f, 0.095785f, 0.12644f, 0.18008f, 0.23372f, 0.29502f },
                new[] { 0.16475f, 0.341f, 0.4636f, 0.61686f, 0.75479f, 0.83908f },
                new[] { 0.26437f, 0.54023f, 0.71648f, 1.023f, 1.1686f, 1.3678f },
                new[] { 0.63985f, 1.1456f, 1.5747f, 2.3563f, 2.9234f, 3.6743f }
            },
            new[] // eley-rideal
            {
                new[] { 0.000721806f, 0.001361116f, 0.001051789f, 0.000907325f, 0.00078371f, 0f, 0f },
                new[] { 0.001381773f, 0.002351066f, 0.002722298f, 0.002887225f, 0.002206666f, 0.001938587f, 0.001381773f },
                new[] { 0.001979835f, 0.003279112f, 0.004145254f, 0.00486706f, 0.003567847f, 0.002887225f, 0.002227323f },
                new[] { 0.002598554f, 0.005258947f, 0.006846895f, 0.007424624f, 0.008682072f, 0.009775561f, 0f },
                new[] { 0.004928964f, 0.006537697f, 0.008455499f, 0.010105415f, 0.011280884f, 0.012786207f, 0.014312831f }
            },
        };
    }
}
