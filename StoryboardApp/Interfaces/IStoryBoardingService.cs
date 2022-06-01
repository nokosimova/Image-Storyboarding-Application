using System.Threading.Tasks;
using StoryboardApp.Models;

public interface IStoryBoardingService
{
    public Task SaveEasyStoryboard(ImagesTreeModel tree, int newHeight);
}