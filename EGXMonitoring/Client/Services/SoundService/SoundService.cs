namespace EGXMonitoring.Client.Services.SoundService
{
    public class SoundService : ISoundService
    {
        List<AlertEventData> Alerts = new List<AlertEventData>();
        public bool AlertOn { get; set; } = false;
        public bool HighAlertOn { get; set; } = false;

        public event Action<AlertEventData> AlertTurnOn;
        public event Action<AlertEventData> AlertTurnOff;
        public event Action<AlertEventData> HighAlertTurnOn;
        public event Action<AlertEventData> HighAlertTurnOff;


        public void SetAlert(AlertEventData alert)
        {
            if(!Alerts.Contains(alert))
            {
                Alerts.Add(alert);
            }
            if(!AlertOn)
            {
                AlertOn = true;
                AlertTurnOn.Invoke(alert);
            }
          
        }

        public void SetHighAlert(AlertEventData alert)
        {
            if (!Alerts.Contains(alert))
            {
                Alerts.Add(alert);
            }
            if (!HighAlertOn)
            {
                HighAlertOn = true;
                HighAlertTurnOn.Invoke(alert);
            }
        }

        public void StopAlert(AlertEventData alert)
        {
            if (Alerts.Contains(alert))
            {
                Alerts.Remove(alert);
            }
            if (AlertOn && Alerts.Where(a => a.Severity == 1).Count() == 0)
            {
                AlertOn = false;
                AlertTurnOff.Invoke(alert);
            }
        }

        public void StopHighAlert(AlertEventData alert)
        {
            if (Alerts.Contains(alert))
            {
                Alerts.Remove(alert);
            }
            if (HighAlertOn && Alerts.Where(a => a.Severity == 2).Count() == 0)
            {
                HighAlertOn = false;
                HighAlertTurnOff.Invoke(alert);
            }
        }
    }
}
