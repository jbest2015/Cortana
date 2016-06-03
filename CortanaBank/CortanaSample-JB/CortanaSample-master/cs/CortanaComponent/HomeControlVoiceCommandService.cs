using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace CortanaComponent
{
    public sealed class HomeControlVoiceCommandService : IBackgroundTask
    {
        private VoiceCommandServiceConnection voiceServiceConnection;
        private BackgroundTaskDeferral serviceDeferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Create the deferral by requesting it from the task instance
            serviceDeferral = taskInstance.GetDeferral();

            AppServiceTriggerDetails triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if (triggerDetails != null && triggerDetails.Name.Equals("VoiceCommandService"))
            {
                voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();
                VoiceCommandUserMessage userMessage = new VoiceCommandUserMessage();
                VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage, null);
                // Perform the appropriate command depending on the operation defined in VCD
                switch (voiceCommand.CommandName)
                {
                     

                    case "CheckTemperature":
                       
                        userMessage.DisplayMessage = "The current temperature is 23 degrees";
                        userMessage.SpokenMessage = "The current temperature is 23 degrees";

                        response = VoiceCommandResponse.CreateResponse(userMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                        break;

                    case "Check_balance":
                       
                        userMessage.DisplayMessage = "You checking account balance is $100";
                        userMessage.SpokenMessage = "Your checking account balance is $100";
                        response = VoiceCommandResponse.CreateResponse(userMessage, null);
                        await voiceServiceConnection.ReportSuccessAsync(response);
                        break;

                    default:
                        break;
                }
            }

            // Once the asynchronous method(s) are done, close the deferral
            serviceDeferral.Complete();
        }
    }
}