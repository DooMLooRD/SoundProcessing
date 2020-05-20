namespace SoundProcessing.Core.Fourier.Windows
{
    public interface IFourierWindow
    {
        double[] Windowing(double[] data);
    }
}
