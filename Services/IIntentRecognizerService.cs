using System.Threading.Tasks;

namespace EchoBot.Services;

public interface IIntentRecognizerService
{
    Task<Intent> RecognizeAsync(string text);
}
