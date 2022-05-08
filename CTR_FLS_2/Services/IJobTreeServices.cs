using CTR_FLS_2.ViewModels;

namespace CTR_FLS_2.Services
{
    public interface IJobTreeServices
    {
        JobTreeViewModel GetJobTreeVM(string TopLevelComponent);
    }
}