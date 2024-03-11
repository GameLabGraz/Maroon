
namespace Maroon.Chemistry.Catalyst
{
    public enum ExperimentStages
    {
        Init,
        CODesorb,
        O2Desorb,
        COAdsorb,
        O2Adsorb_O2Dissociate,
        OFillSurface,
        OReactCO_CO2Desorb
    }

    public static class CatalystStages
    {
        public static readonly ExperimentStages[] HinshelwoodStages =
        {
            ExperimentStages.Init,
            ExperimentStages.CODesorb,
            ExperimentStages.O2Adsorb_O2Dissociate,
            ExperimentStages.OReactCO_CO2Desorb
        };

        public static readonly ExperimentStages[] KrevelenStages = 
        {
            ExperimentStages.Init,
            ExperimentStages.COAdsorb,
            ExperimentStages.OReactCO_CO2Desorb,
            ExperimentStages.O2Adsorb_O2Dissociate,
            ExperimentStages.OFillSurface
        };

        public static readonly ExperimentStages[] EleyStages = 
        {
            ExperimentStages.Init,
            ExperimentStages.O2Adsorb_O2Dissociate,
            ExperimentStages.OReactCO_CO2Desorb
        };
    }
}
