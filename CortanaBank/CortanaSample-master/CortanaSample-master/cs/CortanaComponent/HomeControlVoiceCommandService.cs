using System;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using System.Threading.Tasks;
using Model;
using Windows.Media;


namespace CortanaComponent
{
    public sealed class HomeControlVoiceCommandService : IBackgroundTask
    {
        private VoiceCommandServiceConnection voiceServiceConnection;
        private BackgroundTaskDeferral serviceDeferral;

        private string SemanticInterpretation(string interpretationKey, SpeechRecognitionResult speechRecognitionResult)
        {
            return speechRecognitionResult.SemanticInterpretation.Properties[interpretationKey].FirstOrDefault();
        }


        public async void Run(IBackgroundTaskInstance taskInstance)
        {

         

                // Create the deferral by requesting it from the task instance
                serviceDeferral = taskInstance.GetDeferral();

            
                //setting up triggerDetails so we can recieve the commands. 
                AppServiceTriggerDetails triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;


                //if the trigger is for this service thn get the details 
                if (triggerDetails != null && triggerDetails.Name.Equals("VoiceCommandService"))
                {
                    voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);

                    //this is where we get the voice command 
                    VoiceCommand voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

                    Account srcaccount;
                    Account dstaccount;

                    string account = "no account";
                    IReadOnlyList<string> recognizedVoiceCommandPhrases;

                    // Perform the appropriate command depending on the operation defined in VCD

                    VoiceCommandUserMessage userMessage = new VoiceCommandUserMessage();

                    var bankcontrolContentTiles = new List<VoiceCommandContentTile>();
                    var bankcontrolTile = new VoiceCommandContentTile();
                    bankcontrolTile.ContentTileType = VoiceCommandContentTileType.TitleWithText;


                    switch (voiceCommand.CommandName)
                    {
                        case "Helpme":

                       

                            userMessage.SpokenMessage = "Here are the things you can say";
                            bankcontrolTile.Title = "Help commands";
                            bankcontrolTile.TextLine1 = "What is my credit union checking or savings account balance ?";
                            bankcontrolTile.TextLine2 = "What is my credit union checking or savings account history";
                            bankcontrolTile.TextLine3 = "transfer $100 from my credit union checking to my savings account?";
                            bankcontrolContentTiles.Add(bankcontrolTile);
                            VoiceCommandResponse response = VoiceCommandResponse.CreateResponse(userMessage, bankcontrolContentTiles);
                            await voiceServiceConnection.ReportSuccessAsync(response);
                            break;

                        case "CheckBalance":

                            //balanceTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BankControl/Assets/StoreLogo.png"));

                            if (voiceCommand.SpeechRecognitionResult.SemanticInterpretation.Properties.TryGetValue("accounts", out recognizedVoiceCommandPhrases))
                            {

                                account = recognizedVoiceCommandPhrases.First();
                            }

                            if (account != null && account == "checking")
                            {

                                //userMessage.DisplayMessage = "Your Current Checking Balance is $100";
                                userMessage.SpokenMessage = "Your Current Checking Balance is $100.36";
                                bankcontrolTile.Title = "Your Checking acccount Balance";
                                bankcontrolTile.TextLine1 = "$100.36";
                                bankcontrolContentTiles.Add(bankcontrolTile);

                            }
                            else if (account != null && account == "savings")
                            {


                                userMessage.SpokenMessage = "Your Current Savings Balance is $1000.65";
                                bankcontrolTile.Title = "Your Savings acccount Balance";
                                bankcontrolTile.TextLine1 = "$1000.65";
                                bankcontrolContentTiles.Add(bankcontrolTile);
                            }



                            response = VoiceCommandResponse.CreateResponse(userMessage, bankcontrolContentTiles);
                            await voiceServiceConnection.ReportSuccessAsync(response);
                            break;



                        case "Transfers":

                            //balanceTile.Image = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///BankControl/Assets/StoreLogo.png"));
                            string amount = "";
                           // string dstaccount = "";
                           // string srcaccount = "";
                            string spokenmessage = "";


                           
                            // this is commented out because Cortana struggles to handle a command with multiple phrasetopics. 
                           // amount = SemanticInterpretation("amounts", voiceCommand.SpeechRecognitionResult);
                            
                      
                            //First thing we do is get the source account from ther User / the method below creates a list of tiles from the model.class object and displays it on the screen.                         
                            srcaccount = await sourceAccountListAsync();
                        //when we get something we move on
                        if (srcaccount != null)
                        {

                            //now its time to get the destination account , we pass in the source account name so we can remove it from the list. 
                            dstaccount = await destAccountListAsync(srcaccount.Name);

                            //when it comes back from the async call we move on. 
                            if (dstaccount != null)
                            {

                                //had to debug this quite a bit to get it to work. 
                                try
                                {
                                    //amountget creates a list of transfer options.. Quick picks like $10, $50 , $100 also a custom option where it will need ot be entered on the screen
                                    amount = await amountGet();
                                }

                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("Exception", ex);
                                }


                                //if we got here, it means we have something in the amount. Because we used the disambiguation option the input is predetermined 
                                if (amount != null)
                                {
                                   
                                    //time to confirm the transfer . This creates the tile that shows up in the bottom of the screen
                                    spokenmessage = string.Format("Do you want to transfer {0} from your {1} account to your {2} account?", amount.ToString(), srcaccount.Name, dstaccount.Name);
                              
                                    userMessage.SpokenMessage = spokenmessage;
                                    bankcontrolTile.Title = "Your Transfer";
                                    bankcontrolTile.TextLine1 = spokenmessage;
                                    bankcontrolContentTiles.Add(bankcontrolTile);

                                    //anytime you ask for user input you must have a reprompt message 
                                    var repeatpromptmessage = new VoiceCommandUserMessage();
                                    repeatpromptmessage.DisplayMessage = repeatpromptmessage.SpokenMessage = " I am sorry I missed your response. Please repeat your choice";


                                    //setup the screen in the cortana area with the tile and wait fora  response. 
                                    response = VoiceCommandResponse.CreateResponseForPrompt(userMessage, repeatpromptmessage, bankcontrolContentTiles);


                                    try {
                                        var result = await voiceServiceConnection.RequestConfirmationAsync(response);








                                        // I need to look at what comes back the optins are yes, no or cancel . I think I need to determine what the real answer is. something to look at when we build this out. 
                                        if (result != null)
                                        {

                                            //Here is where to wire in the host calls for transfers. 
                                            // if it doesn't work change the message to an error
                                            spokenmessage = string.Format("Your transfer is complete");
                                            userMessage.SpokenMessage = spokenmessage;
                                            bankcontrolTile.Title = "Transfer was completed Successfully";
                                            bankcontrolTile.TextLine1 = "Thank you for choosing Your FI";
                                            bankcontrolContentTiles.Add(bankcontrolTile);
                                            response = VoiceCommandResponse.CreateResponse(userMessage, bankcontrolContentTiles);

                                            await voiceServiceConnection.ReportSuccessAsync(response);
                                            break;


                                        }
                                        else
                                        {
                                            //Cancel the transfer. 
                                            spokenmessage = string.Format("Your transfer is cancelled");
                                            userMessage.SpokenMessage = spokenmessage;
                                            bankcontrolTile.Title = "Transfer cancelled";
                                            bankcontrolTile.TextLine1 = "We have cancelled your Transfer";
                                            bankcontrolContentTiles.Add(bankcontrolTile);

                                            response = VoiceCommandResponse.CreateResponse(userMessage, bankcontrolContentTiles);

                                            await voiceServiceConnection.ReportSuccessAsync(response);
                                            break;
                                        }
                                    }

                                    catch (Exception ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine("Exception", ex);
                                    }

                                }
                                break;  }
                            break; }
                        break;




                    default:
                            break;
                    }
                }

                // Once the asynchronous method(s) are done, close the deferral
                serviceDeferral.Complete();
            }
        //Get the amount from the user, includes the quick picks and waits for input. 
        private async Task<string> amountGet()
        {
            // Pompt and repeat promt
            // L'avis et la répétition de l'avis
            var promptmessage = new VoiceCommandUserMessage();
            promptmessage.DisplayMessage = promptmessage.SpokenMessage = "How Much would you like to transfer?";

            var repeatpromptmessage = new VoiceCommandUserMessage();
            repeatpromptmessage.DisplayMessage = repeatpromptmessage.SpokenMessage = " I am sorry I missed your response. Please repeat your choice";

            //all options have to be put into tiles.
            // I wish I could format them a little better. I want to try some HTML and see what happens. 

            var tilelist = new List<VoiceCommandContentTile>();


            //loop to fill in the tiles. 

            for (int i = 1; i <= 3; i++)
                {
                double f;
                string amount_Cur = "";
              // if then to fill out the quick pick options.. down the road these should be configurable 
                if (i == 1)
                
                     amount_Cur = String.Format("{0:C}", 10);
                
                else if (i == 2)
                
                     amount_Cur = String.Format("{0:C}", 50);
                
                else if ( i == 3)
                
                     amount_Cur = String.Format("{0:C}", 100);
                
                // create the tile 
                var tile2 = new VoiceCommandContentTile();
                tile2.ContentTileType = VoiceCommandContentTileType.TitleWithText;
                tile2.AppContext = amount_Cur;
                tile2.AppLaunchArgument = "selectedid=" + i;
                tile2.Title = string.Format(amount_Cur);
                //tile.TextLine1 = amount_Cur;

                tilelist.Add(tile2);
            }

            //add the custom option. There will need to be handling in the Case "Transfers" above that opens a screen and collect input from the user. 

            var tile = new VoiceCommandContentTile();
            tile.ContentTileType = VoiceCommandContentTileType.TitleWithText;
            tile.AppContext = "Other";
            tile.AppLaunchArgument = "Other";
            tile.Title = "Select Amount";
            //tile.TextLine1 = amount_Cur;

            tilelist.Add(tile);


            // remember anytime you prompt for a  message you have to pass in a prompt, repeat and tilelist . Especially for disambugiation and none of them can be the same , it will throw an error
            var response = VoiceCommandResponse.CreateResponseForPrompt(promptmessage, repeatpromptmessage,tilelist);

            var result = await voiceServiceConnection.RequestDisambiguationAsync(response);

            return result?.SelectedItem.AppContext.ToString();


        }


        //method to collect the source a ccount list .
        private async Task<Account> sourceAccountListAsync()
        {
            // Pompt and repeat promt
          
            var promptmessage = new VoiceCommandUserMessage();
            promptmessage.DisplayMessage = promptmessage.SpokenMessage = "Please choose your source account";

            var repeatpromptmessage = new VoiceCommandUserMessage();
            repeatpromptmessage.DisplayMessage = repeatpromptmessage.SpokenMessage = " I am sorry I missed your response. Please repeat your choice";

            var tilelist = new List<VoiceCommandContentTile>();

            // Get tile for each Account
          


            try
            {

                foreach (var account in Accounts.List)
                {
                    if (account.sourceonly)
                    {
                        var tile = await GetTileAsync(account);

                        tilelist.Add(tile);
                    }

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception", ex);
            }
        

            // Create a prompt response message
         
            var response = VoiceCommandResponse.CreateResponseForPrompt(promptmessage, repeatpromptmessage, tilelist);

            // Show and get answer from user
        
            var result = await voiceServiceConnection.RequestDisambiguationAsync(response);

            // Return the selected item (or null)
         
            return result?.SelectedItem.AppContext as Account;
        }

        //similar method be allows the source account to be removed form the list. This could probably be accomplished in one method. 

        private async Task<Account> destAccountListAsync(String srcaccountName)
        {
            // Prompt and repeat prompt
         
            var promptmessage = new VoiceCommandUserMessage();
            promptmessage.DisplayMessage = promptmessage.SpokenMessage = "Please choose your Destination account";

            var repeatpromptmessage = new VoiceCommandUserMessage();
            repeatpromptmessage.DisplayMessage = repeatpromptmessage.SpokenMessage = " I am sorry I missed your response. Please repeat your choice";

            var tilelist = new List<VoiceCommandContentTile>();

            // Get tile for each account tile
      


            try
            {

                foreach (var account in Accounts.List)
                {
                    if (account.Name != srcaccountName)
                    {
                        var tile = await GetTileAsync(account);

                        tilelist.Add(tile);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception", ex);
            }


            // Create a prompt response message
            
            var response = VoiceCommandResponse.CreateResponseForPrompt(promptmessage, repeatpromptmessage, tilelist);

            // Show and get answer from user
        
            var result = await voiceServiceConnection.RequestDisambiguationAsync(response);

            // Return the selected item (or null)
          
            return result?.SelectedItem.AppContext as Account;
        }


        private async Task<VoiceCommandContentTile> GetTileAsync(Account account)
        {
            // Build a tile from selected Account 
          
            return new VoiceCommandContentTile
            {
                ContentTileType = VoiceCommandContentTileType.TitleWithText,
                AppContext = account,
                AppLaunchArgument = "selectedid=" + account.ID,
                Title = string.Format(account.Name),
                TextLine1 = account.Details
                //Image = await StorageFile.GetFileFromApplicationUriAsync(
                //    new Uri($"ms-appx:///BankControl/Assets/StoreLogo.png"))
            };
        }




    }
    }

   



