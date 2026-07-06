using CollegeWebSite.Domain.Entities;
using CollegeWebSite.Services.Interfaces;

namespace CollegeWebSite.Services.Implementation;

public class DialogService : IDialogService
{
    public event Action<AnnouncementPopUp>? OnShow;

    public void Show(AnnouncementPopUp popup)
    {
        OnShow?.Invoke(popup);
    }
}
