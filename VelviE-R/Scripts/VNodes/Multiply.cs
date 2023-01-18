
namespace VelvieR
{
    [VTag("Maths/Multiply", "Multiplies two values.\n\nThe variable will contain the results.", VColor.Green01, "Mu")]
    public class Multiply : AVarMaths
    {
        public override void OnVEnter()
        {
            StartMath();
        }
        public override void OnVExit()
        {
            OnVContinue();
        }
        public override string OnVSummary()
        {
            var summary = string.Empty;

            if(variable == null)
            {
                summary += "Variable can't be empty!";
            }

            return summary;
        }
    }
}