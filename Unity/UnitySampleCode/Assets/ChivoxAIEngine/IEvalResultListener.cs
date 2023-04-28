namespace ChivoxAIEngine
{
public interface IEvalResultListener
{
void OnError(string tokenID, EvalResult result);
void OnEvalResult(string tokenID, EvalResult result);
void OnBinResult(string tokenID, EvalResult result);
void OnVad(string tokenID, EvalResult result);
void OnSoundIntensity(string tokenID, EvalResult result);
void OnOther(string tokenID, EvalResult result);
}
}
