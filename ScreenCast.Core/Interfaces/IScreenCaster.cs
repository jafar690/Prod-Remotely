using System.Threading.Tasks;
using Silgred.Shared.Models;

namespace Silgred.ScreenCast.Core.Interfaces
{
    public interface IScreenCaster
    {
        Task BeginScreenCasting(ScreenCastRequest screenCastRequest);
    }
}