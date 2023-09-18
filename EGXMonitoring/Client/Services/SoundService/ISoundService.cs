namespace EGXMonitoring.Client.Services.SoundService
{
    public interface ISoundService
    {
        event Action<AlertEventData> AlertTurnOn;
        event Action<AlertEventData> HighAlertTurnOn;
        event Action<AlertEventData> AlertTurnOff;
        event Action<AlertEventData> HighAlertTurnOff;
        void SetAlert(AlertEventData alert);
        void SetHighAlert(AlertEventData alert);
        void StopAlert(AlertEventData alert);
        void StopHighAlert(AlertEventData alert);

    }
}
