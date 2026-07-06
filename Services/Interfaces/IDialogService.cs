using CollegeWebSite.Domain.Entities;

namespace CollegeWebSite.Services.Interfaces;

public interface IDialogService
{
    event Action<AnnouncementPopUp>? OnShow;
    void Show(AnnouncementPopUp popup);
}
